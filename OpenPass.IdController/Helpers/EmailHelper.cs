using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Criteo.Logging;

namespace OpenPass.IdController.Helpers
{
    public interface IEmailHelper
    {
        Task SendOtpEmail(string recipient, string otp);

        bool IsValidEmail(string email);
    }

    public class EmailHelper : IEmailHelper
    {
        private static readonly string _metricPrefix = "email.";

        private static readonly ILogger _logger = LogManager.GetLogger<EmailHelper>();
        private readonly IMetricHelper _metricsHelper;
        private readonly IViewRenderHelper _viewRenderHelper;
        private readonly IEmailConfiguration _config;
        private readonly SmtpClient _smtpClient;

        public EmailHelper(IMetricHelper metricsHelper, IViewRenderHelper viewRenderHelper, IEmailConfiguration emailConfiguration)
        {
            _metricsHelper = metricsHelper;
            _viewRenderHelper = viewRenderHelper;
            _config = emailConfiguration;

            _smtpClient = CreateSmtpClient();
        }

        private SmtpClient CreateSmtpClient()
        {
            var smtpClient = new SmtpClient(_config.MailServer, _config.MailServerPort) { EnableSsl = _config.MailServerSsl };

            if (!(string.IsNullOrEmpty(_config.AuthUserName) || string.IsNullOrEmpty(_config.AuthPassword)))
                smtpClient.Credentials = new NetworkCredential(_config.AuthUserName, _config.AuthPassword);

            // Add callback for metrics and logs
            smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

            return smtpClient;
        }

        public async Task SendOtpEmail(string recipient, string otp)
        {
            var viewData = new Dictionary<string, object>
            {
                { "Code", otp }
            };

            var subject = "OpenPass Verification Code";
            var body = await _viewRenderHelper.RenderToStringAsync("Email/VerificationCode", null, viewData);

            SendEmailAsync(recipient, subject, body, "otp", isBodyHtml: true);
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                var mail = new MailAddress(email);
                return mail.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void SendEmailAsync(string recipient, string subject, string body, string token, bool isBodyHtml = false)
        {
            var senderMailAddress = new MailAddress(_config.SenderEmailAddress, _config.SenderDisplayName);
            var recipientMailAddress = new MailAddress(recipient);

            var message = new MailMessage
            {
                From = senderMailAddress,
                To = { recipientMailAddress },
                Subject = subject,
                Body =  body,
                IsBodyHtml = isBodyHtml
            };

            _smtpClient.SendAsync(message, token);
        }

        #region Callback

        // Using async method to avoid blocking the calling thread, this callback
        // will be called when the operation is completed
        private void SendCompletedCallback(object token, AsyncCompletedEventArgs e)
        {
            var metric = $"{_metricPrefix}.{token}";
            if (e.Error != null)
            {
                _metricsHelper.SendCounterMetric($"{metric}.error");
                _logger.Log(LogLevel.Error, "Error when sending email", e.Error);
            }
            else
            {
                _metricsHelper.SendCounterMetric($"{metric}.success");
            }
        }

        #endregion Callback
    }
}
