using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenPass.IdController.Helpers.Configuration;

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

        private readonly IMetricHelper _metricsHelper;
        private readonly ILogger _logger;

        private readonly IViewRenderHelper _viewRenderHelper;
        private readonly IConfigurationManager _configurationManager;
        private readonly SmtpClient _smtpClient;

        public EmailHelper(
            IMetricHelper metricsHelper,
            IViewRenderHelper viewRenderHelper,
            IConfigurationManager configurationManager,
            ILogger<EmailHelper> logger)
        {
            _metricsHelper = metricsHelper;
            _viewRenderHelper = viewRenderHelper;
            _configurationManager = configurationManager;
            _logger = logger;

            _smtpClient = CreateSmtpClient();
        }

        private SmtpClient CreateSmtpClient()
        {
            var smtpClient = new SmtpClient(_configurationManager.SmtpSettings.Host, _configurationManager.SmtpSettings.Port)
            {
                EnableSsl = _configurationManager.SmtpSettings.EnableSsl
            };

            if (!(string.IsNullOrEmpty(_configurationManager.SmtpSettings.UserName)
                    || string.IsNullOrEmpty(_configurationManager.SmtpSettings.Password)))
                smtpClient.Credentials =
                    new NetworkCredential(_configurationManager.SmtpSettings.UserName, _configurationManager.SmtpSettings.Password);

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
            var senderMailAddress =
                new MailAddress(_configurationManager.SmtpSettings.Address, _configurationManager.SmtpSettings.DisplayName);
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
                _logger.LogError("Error when sending email", e.Error);
            }
            else
            {
                _metricsHelper.SendCounterMetric($"{metric}.success");
            }
        }

        #endregion Callback
    }
}
