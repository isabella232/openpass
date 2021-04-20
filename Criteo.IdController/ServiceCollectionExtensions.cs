using Criteo.ConfigAsCode;
using Criteo.IdController.Helpers;
using Criteo.IdController.Helpers.Adapters;
using Criteo.Services.Glup;
using Criteo.UserAgent.Provider;
using Criteo.UserIdentification.Services;
using Criteo.UserIdentification.Services.IdentityMapping;
using Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Interfaces.Hosting;
using Sdk.Interfaces.KeyValueStore;
using Sdk.Monitoring;
using Sdk.ProductionResources.ConnectionStrings;
using Sdk.Secrets;

namespace Criteo.IdController
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurationHelper(this IServiceCollection services)
        {
            services.AddSingleton<IConfigurationHelper>(r =>
            {
                var cacService = r.GetService<IConfigAsCodeService>();
                return new ConfigurationHelper(cacService);
            });

            return services;
        }

        public static IServiceCollection AddInternalMappingHelper(this IServiceCollection services)
        {
            services.AddSingleton<IInternalMappingHelper>(r =>
            {
                var storageManager = r.GetService<IStorageManager>();
                var glupService = r.GetService<IGlupService>();
                var cacService = r.GetService<IConfigAsCodeService>();
                var graphiteHelper = r.GetService<IGraphiteHelper>();
                var secretsResolver = r.GetService<ISecretsResolver>();

                var identityMapper = new IdentityMapper(storageManager, glupService, cacService, graphiteHelper, UserIdentificationContext.UserCentricAdId, secretsResolver);

                return new InternalMappingHelper(cacService, identityMapper);
            });

            return services;
        }

        public static IServiceCollection AddGlupHelper(this IServiceCollection services)
        {
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

            return services;
        }

        public static IServiceCollection AddViewRenderHelper(this IServiceCollection services)
        {
            services.AddSingleton<IViewRenderHelper, ViewRenderHelper>();
            return services;
        }

        public static IServiceCollection AddEmailHelper(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IEmailHelper>(r =>
            {
                var metricsRegistry = r.GetService<IMetricsRegistry>();
                var viewRender = r.GetService<IViewRenderHelper>();
                var emailConfiguration = new EmailConfiguration(configuration);

                return new EmailHelper(metricsRegistry, viewRender, emailConfiguration);
            });

            return services;
        }

        public static IServiceCollection AddCodeGeneratorHelper(this IServiceCollection services)
        {
            services.AddSingleton<ICodeGeneratorHelper, CodeGeneratorHelper>();

            return services;
        }

        public static IServiceCollection AddCookieHelper(this IServiceCollection services)
        {
            services.AddSingleton<ICookieHelper, CookieHelper>();

            return services;
        }

        public static IServiceCollection AddMetricHelper(this IServiceCollection services)
        {
            services.AddSingleton<IMetricHelper, MetricHelper>();

            return services;
        }

        public static IServiceCollection AddUid2Adapter(this IServiceCollection services)
        {
            services.AddSingleton<IIdentifierAdapter, Uid2Adapter>();

            return services;
        }
    }
}
