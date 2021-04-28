using Microsoft.Extensions.Configuration;

namespace OpenPass.IdController.Helpers
{
    public interface IEmailConfiguration
    {
        string MailServer { get; }
        int MailServerPort { get; }
        bool MailServerSsl { get; }
        string SenderDisplayName { get; }
        string SenderEmailAddress { get; }
        string AuthUserName { get; }
        string AuthPassword { get; }
    }

    public class EmailConfiguration : IEmailConfiguration
    {
        private readonly IConfiguration _configuration;

        public EmailConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Mail server configuration
        public string MailServer => _configuration.GetValue<string>("Email:Server:Host") ?? "mail-relay.service.consul";

        public int MailServerPort => _configuration.GetValue<int?>("Email:Server:Port") ?? 25;
        public bool MailServerSsl => _configuration.GetValue<bool?>("Email:Server:EnableSsl") ?? false;

        // Sender configuration
        public string SenderDisplayName => _configuration.GetValue<string>("Email:Sender:DisplayName") ?? "OpenPass";

        public string SenderEmailAddress => _configuration.GetValue<string>("Email:Sender:Address") ?? "openpass@criteo.com";  // FIXME: use external domain once bought

        // Authentication configuration
        public string AuthUserName => _configuration.GetValue<string>("Email:Authentication:UserName");

        public string AuthPassword => _configuration.GetValue<string>("Email:Authentication:Password");
    }
}
