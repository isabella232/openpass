namespace OpenPass.IdController.Models.Configuration
{
    public class SmtpSettings
    {
        public string Host { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }

        public string DisplayName { get; set; }

        public string Address { get; set; }

        public bool EnableSsl { get; set; }
    }
}
