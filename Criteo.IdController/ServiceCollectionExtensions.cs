using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Criteo.ConfigAsCode;
using Criteo.IdController.Helpers;
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

                var identityMapper = new IdentityMapper(storageManager, glupService, cacService, graphiteHelper, UserIdentificationContext.UserCentricAdId);

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

        public static IServiceCollection AddUserManagementHelper(this IServiceCollection services)
        {
            services.AddSingleton<IUserManagementHelper, UserManagementHelper>();

            return services;
        }

        public static IServiceCollection AddEmailHelper(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IEmailHelper>(r =>
            {
                var metricsRegistry = r.GetService<IMetricsRegistry>();
                var emailConfiguration = new EmailConfiguration(configuration);

                return new EmailHelper(metricsRegistry, emailConfiguration);
            });

            return services;
        }
    }
}
