using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Criteo.AspNetCore.Helpers;
using Criteo.ConfigAsCode;
using Criteo.IdController.Helpers;
using Criteo.Services;
using Criteo.Services.Glup;
using Criteo.Services.Graphite;
using Criteo.UserAgent.Provider;
using Criteo.UserIdentification.Services;
using Criteo.UserIdentification.Services.IdentityMapping;
using Metrics;
using Sdk.Interfaces.Hosting;
using Sdk.Interfaces.KeyValueStore;
using Sdk.Monitoring;
using Sdk.ProductionResources.ConnectionStrings;

namespace Criteo.IdController
{
    internal class Startup
    {
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        /// <summary>
        /// Holds key/value configuration data, read from, in order, every step overriding the previous one:
        ///  - the appsettings.json file
        ///  - the appsettings.[Environment].json file based on current environment (Development, Sandbox, Preprod, Prod)
        ///  - the command line arguments
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Registers response compression services
            services.AddResponseCompression();

            services.AddCriteoServices(registrar =>
            {
                // Registers an IMetricsRegistry instance for further usage, so you can register and use Metrics.
                var metricsRegistry = registrar.AddMetricsRegistry();
                // Registers an IConsulServiceLocator implementation. Current service is read from the configuration/command line arguments automatically.
                var serviceLocator = registrar.AddServiceLocator(metricsRegistry);

                // Registers an IConfigAsCodeService that will read its configuration data from the SQL databases, with dependencies
                var keyValueStore = registrar.AddConsulKeyValueStore(metricsRegistry);
                var sdkConfigurationService = registrar.AddSdkConfigurationService(keyValueStore, metricsRegistry, serviceLocator);

                var sqlConnections = registrar.AddSqlConnections(serviceLocator, metricsRegistry, sdkConfigurationService);
                var kafkaConsumer = registrar.AddKafkaConsumer(metricsRegistry, serviceLocator, sdkConfigurationService);
                var storageManager = registrar.AddStorageManager(metricsRegistry, serviceLocator, keyValueStore, sdkConfigurationService);
                var configAsCode = registrar.AddConfigAsCode(metricsRegistry, serviceLocator, storageManager, kafkaConsumer, sqlConnections);

                // Enables tracing & request correlation
                var kafkaProducer = registrar.AddKafkaProducer(metricsRegistry, serviceLocator, sdkConfigurationService);
                registrar.AddTracing(metricsRegistry, kafkaProducer);

                // Add GraphiteHelper for the UserAgent library
                registrar.AddGraphiteHelper(serviceLocator, new GraphiteSettings { ApplicationName = "identification-id-controller" });

                // Register glup
                registrar.AddGlup(metricsRegistry, serviceLocator, kafkaProducer, configAsCode);
            });

            // Configuration helper
            services.AddSingleton<IConfigurationHelper>(r =>
            {
                var cacService = r.GetService<IConfigAsCodeService>();
                return new ConfigurationHelper(cacService);
            });

            // Internal Mapping helper (revocable id)
            services.AddSingleton<IInternalMappingHelper>(r =>
            {
                var storageManager = r.GetService<IStorageManager>();
                var glupService = r.GetService<IGlupService>();
                var cacService = r.GetService<IConfigAsCodeService>();
                var graphiteHelper = r.GetService<IGraphiteHelper>();

                var identityMapper = new IdentityMapper(storageManager, glupService, cacService, graphiteHelper, UserIdentificationContext.UserCentricAdId);

                return new InternalMappingHelper(cacService, identityMapper);
            });

            // Glup helper
            services.AddSingleton<IGlupHelper>(r =>
            {
                // Instantiate User Agent parsing library
                var glupService = r.GetService<IGlupService>();
                var serviceLifecycleManager = r.GetService<IServiceLifecycleManager>();
                var sqlDbConnectionService = r.GetService<ISqlDbConnectionService>();
                var graphiteHelper = r.GetService<IGraphiteHelper>();
                var cacService = r.GetService<IConfigAsCodeService>();
                var storageManager = r.GetService<IStorageManager>();
                var agentSource = UserAgentProviderProvider.CreateAgentSource(
                    serviceLifecycleManager,
                    sqlDbConnectionService,
                    graphiteHelper,
                    glupService,
                    cacService,
                    storageManager);

                // Force preload to have the offline db and avoid having runtime errors
                agentSource.Preload();

                return new GlupHelper(glupService, agentSource);
            });

            // User Management helper
            services.AddSingleton<IUserManagementHelper, UserManagementHelper>();

             // Email helper
             services.AddSingleton<IEmailHelper>(r =>
             {
                 var metricsRegistry = r.GetService<IMetricsRegistry>();
                 var emailConfiguration = new EmailConfiguration(Configuration);

                 return new EmailHelper(metricsRegistry, emailConfiguration);
             });

            // Add in-memory cache to store OTPs temporarily
            services.AddMemoryCache();

            services.AddMvc(options =>
            {
                // filters added here are applied for *all* controllers & actions that passed the middlewares chain.

                // adds metrics for app monitoring. Should remain the last filter added in this block.
                // In Pure DI mode, pass the IMetricsRegistry you've built
                //options.Filters.AddCriteoMonitoringFilters();
            });

            // Register Admin handlers
            services.AddSdkAdminHandlers();

            // Registers cross-origin resource sharing services
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IApplicationLifetime appLifetime)
        {
            // Add a CORS middleware. Must be called before UseMvc().
            string allowedOrigins = _env.IsDevelopment() ? "*" : (Configuration["allowedOrigins"] ?? string.Empty);
            app.UseCors(builder => builder.WithOrigins(allowedOrigins.Split(',')).AllowAnyMethod().AllowAnyHeader().AllowCredentials());

            // Enables response compression when applicable
            app.UseResponseCompression();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                // generic (non controller-specific) routes are defined here
            });
        }
    }
}
