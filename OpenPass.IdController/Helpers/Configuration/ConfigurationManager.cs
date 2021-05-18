using Microsoft.Extensions.Configuration;
using OpenPass.IdController.Models.Configuration;

namespace OpenPass.IdController.Helpers.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly IConfiguration _configuration;

        public ConfigurationManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SmtpSettings SmtpSettings => GetSectionByName<SmtpSettings>(ConfigSectionNames.SmtpSettings);
        public Uid2Configuration Uid2Configuration => GetSectionByName<Uid2Configuration>(ConfigSectionNames.Uid2Configuration);

        private T GetSectionByName<T>(string sectionName)
            where T : class
        {
            return _configuration.GetSection(sectionName).Get<T>();
        }
    }
}
