using OpenPass.IdController.Models.Configuration;

namespace OpenPass.IdController.Helpers.Configuration
{
    public interface IConfigurationManager
    {
        SmtpSettings SmtpSettings { get; }
        Uid2Configuration Uid2Configuration { get; }
    }
}
