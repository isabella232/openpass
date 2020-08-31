using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Criteo.AspNetCore.Administration;
using Criteo.AspNetCore.Helpers;
using Criteo.Services;
using Criteo.Services.Graphite;

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
                // Registers ISqlDbConnectionService for SQL DB access
                var sqlConnections = registrar.AddSqlConnections(serviceLocator);

                // Registers an IConfigAsCodeService that will read its configuration data from the SQL databases, with dependencies
                var keyValueStore = registrar.AddConsulKeyValueStore(metricsRegistry);
                var sdkConfigurationService = registrar.AddSdkConfigurationService(keyValueStore, metricsRegistry, serviceLocator);
                var kafkaConsumer = registrar.AddKafkaConsumer(metricsRegistry, serviceLocator, sdkConfigurationService);
                var storageManager = registrar.AddStorageManager(metricsRegistry, serviceLocator, keyValueStore);
                registrar.AddConfigAsCode(metricsRegistry, serviceLocator, storageManager, kafkaConsumer, sqlConnections);

                // Enables tracing & request correlation
                var kafkaProducer = registrar.AddKafkaProducer(metricsRegistry, serviceLocator, sdkConfigurationService);
                registrar.AddTracing(metricsRegistry, kafkaProducer);

                // Registers an IGraphiteHelper
                registrar.AddGraphiteHelper(serviceLocator, new GraphiteSettings
                {
                    ApplicationName = "identification-id-controller"
                });
            });

            services.AddMvc(options =>
            {
                // filters added here are applied for *all* controllers & actions that passed the middlewares chain.

                // adds metrics for app monitoring. Should remain the last filter added in this block.
                // In Pure DI mode, pass the IMetricsRegistry you've built
                //options.Filters.AddCriteoMonitoringFilters();
            });

            // (Optional) You might implement a dedicated HealthCheck for your app, checking your app state (not your dependencies)
            // Otherwise you will rely on the default one: ApplicationStateAwareHealthCheck
            services.AddHealthCheck<ApplicationStateAwareHealthCheck>();

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

            // Serve static JS files
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(_env.ContentRootPath, "dist/js")),
                RequestPath = "/js"
            });

            app.UseMvc(routes =>
            {
                // generic (non controller-specific) routes are defined here

                // This defaults to a controller and action if the requested route doesn't exist (instead of a 404)
                routes.MapSpaFallbackRoute("spa-fallback", new { controller = "Home", action = "Index" });
            });
        }
    }
}