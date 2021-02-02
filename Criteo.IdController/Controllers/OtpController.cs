using System;
using System.Linq;
using Criteo.IdController.Helpers;
using Microsoft.AspNetCore.Mvc;
using Metrics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using static Criteo.Glup.IdController.Types;

namespace Criteo.IdController.Controllers
{
    [Route("api/[controller]")]
    public class OtpController : Controller
    {
        private const int _otpCodeLifetimeMinutes = 15;
        private const int _otpCodeLength = 6;
        private static readonly string _codeCharacters = "1234567890";
        private static readonly string _metricPrefix = "otp";

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMetricsRegistry _metricsRegistry;
        private readonly IMemoryCache _activeOtps; // Mapping: (email -> OTP)
        private readonly IConfigurationHelper _configurationHelper;
        private readonly IEmailHelper _emailHelper;
        private readonly IGlupHelper _glupHelper;

        private readonly Random _randomGenerator;

        public OtpController(
            IHostingEnvironment hostingEnvironment,
            IMetricsRegistry metricRegistry,
            IMemoryCache memoryCache,
            IConfigurationHelper configurationHelper,
            IEmailHelper emailHelper,
            IGlupHelper glupHelper)
        {
            _hostingEnvironment = hostingEnvironment;
            _metricsRegistry = metricRegistry;
            _activeOtps = memoryCache;
            _configurationHelper = configurationHelper;
            _emailHelper = emailHelper;
            _glupHelper = glupHelper;
            _randomGenerator = new Random();
        }

        [HttpPost("generate")]
        public IActionResult GenerateOtp(string email, string originHost = null)
        {
            var prefix = $"{_metricPrefix}.generate";

            if (!_configurationHelper.EnableOtp)
            {
                SendMetric($"{prefix}.forbidden");
                // Status code 404 -> resource not found (best way to say not available)
                return NotFound();
            }

            if (!IsValidEmail(email))
            {
                SendMetric($"{prefix}.bad_request");
                return BadRequest();
            }

            // TODO: Validate email

            // 1. Generate OTP and add it to cache (keyed by email)
            var otp = GenerateRandomCode();
            _activeOtps.Set(email, otp, TimeSpan.FromMinutes(_otpCodeLifetimeMinutes));

            // TODO: Check how to properly do this quick fix for OTP validation testing when doing development
            if (_hostingEnvironment.IsDevelopment())
                Console.Out.WriteLine($"New OTP code generated (valid for {_otpCodeLifetimeMinutes} minutes): {email} -> {otp}");

            // 2. Send email
            _emailHelper.SendOtpEmail(email, otp);

            // 3. Emit glup
            var userAgentString = HttpContext?.Request?.Headers?["User-Agent"];
            _glupHelper.EmitGlup(EventType.EmailEntered, originHost, userAgentString);

            // Status code 204 -> resource created but not content returned
            return NoContent();
        }

        [HttpPost("validate")]
        public IActionResult ValidateOtp(string email, string otp, string originHost = null)
        {
            var prefix = $"{_metricPrefix}.validate";

            if (!_configurationHelper.EnableOtp)
            {
                SendMetric($"{prefix}.forbidden");
                // Status code 404 -> resource not found (best way to say not available)
                return NotFound();
            }

            if (!(IsValidEmail(email) && IsValidOtp(otp)))
            {
                SendMetric($"{prefix}.bad_request");
                return BadRequest();
            }

            // Get code from cache and validate
            if (_activeOtps.TryGetValue(email, out string validOtp) && (otp == validOtp))
            {
                SendMetric($"{prefix}.valid");
                // Remove code: OTP is valid only once
                _activeOtps.Remove(email);
                // Emit glup
                var userAgentString = HttpContext?.Request?.Headers?["User-Agent"];
                _glupHelper.EmitGlup(EventType.EmailValidated, originHost, userAgentString);

                // TODO: Generate + set cookie?
                return Ok();
            }

            SendMetric($"{prefix}.invalid");

            return NotFound(); // TODO: Discuss what to return here
        }

        #region Helpers
        private string GenerateRandomCode()
        {
            return new string(Enumerable.Repeat(_codeCharacters, _otpCodeLength)
                .Select(s => s[_randomGenerator.Next(s.Length)]).ToArray());
        }

        private void SendMetric(string metric)
        {
            _metricsRegistry.GetOrRegister(metric, () => new Counter(Granularity.CoarseGrain)).Increment();
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                var mail = new System.Net.Mail.MailAddress(email);
                return mail.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidOtp(string otp)
        {
            return !string.IsNullOrEmpty(otp) && (otp.Length == _otpCodeLength) && otp.All(c => _codeCharacters.Contains(c));
        }
        #endregion
    }
}
