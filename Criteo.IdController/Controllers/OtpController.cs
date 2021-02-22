using System;
using System.Linq;
using Criteo.IdController.Helpers;
using Microsoft.AspNetCore.Mvc;
using Metrics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using static Criteo.Glup.IdController.Types;

namespace Criteo.IdController.Controllers
{
    [Route("api/[controller]")]
    public class OtpController : Controller
    {
        private const int _otpCodeLifetimeMinutes = 15;
        private const string _metricPrefix = "otp";
        private const string _cookieName = "openpass_token";
        private const int _cookieLifetimeDays = 390;

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMetricsRegistry _metricsRegistry;
        private readonly IMemoryCache _activeOtps; // Mapping: (email -> OTP)
        private readonly IConfigurationHelper _configurationHelper;
        private readonly IEmailHelper _emailHelper;
        private readonly IGlupHelper _glupHelper;
        private readonly ICodeGeneratorHelper _codeGeneratorHelper;

        private readonly CookieOptions _defaultCookieOptions;

        public OtpController(
            IHostingEnvironment hostingEnvironment,
            IMetricsRegistry metricRegistry,
            IMemoryCache memoryCache,
            IConfigurationHelper configurationHelper,
            IEmailHelper emailHelper,
            IGlupHelper glupHelper,
            ICodeGeneratorHelper codeGeneratorHelper)
        {
            _hostingEnvironment = hostingEnvironment;
            _metricsRegistry = metricRegistry;
            _activeOtps = memoryCache;
            _configurationHelper = configurationHelper;
            _emailHelper = emailHelper;
            _glupHelper = glupHelper;
            _codeGeneratorHelper = codeGeneratorHelper;

            _defaultCookieOptions = new CookieOptions
            {
                Expires = new DateTimeOffset(DateTime.Today.AddDays(_cookieLifetimeDays))
            };
        }

        #region Request models
        public abstract class GenericRequest
        {
            public string Email { get; set; }
            public string OriginHost { get; set; }
        }

        public class GenerateRequest : GenericRequest
        { }

        public class ValidateRequest : GenericRequest
        {
            public string Otp { get; set; }
        }
        #endregion

        [HttpPost("generate")]
        public IActionResult GenerateOtp(
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromBody] GenerateRequest request)
        {
            var prefix = $"{_metricPrefix}.generate";

            if (!_configurationHelper.EnableOtp)
            {
                SendMetric($"{prefix}.forbidden");
                // Status code 404 -> resource not found (best way to say not available)
                return NotFound();
            }

            if (!_emailHelper.IsValidEmail(request.Email))
            {
                SendMetric($"{prefix}.bad_request");
                return BadRequest();
            }

            // 1. Generate OTP and add it to cache (keyed by email)
            var otp = _codeGeneratorHelper.GenerateRandomCode();
            _activeOtps.Set(request.Email, otp, TimeSpan.FromMinutes(_otpCodeLifetimeMinutes));

            // TODO: Check how to properly do this quick fix for OTP validation testing when doing development
            if (_hostingEnvironment.IsDevelopment())
                Console.Out.WriteLine($"New OTP code generated (valid for {_otpCodeLifetimeMinutes} minutes): {request.Email} -> {otp}");

            // 2. Send email
            _emailHelper.SendOtpEmail(request.Email, otp);

            // 3. Emit glup
            _glupHelper.EmitGlup(EventType.EmailEntered, request.OriginHost, userAgent);

            // Status code 204 -> resource created but not content returned
            return NoContent();
        }

        [HttpPost("validate")]
        public IActionResult ValidateOtp(
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromBody] ValidateRequest request)
        {
            var prefix = $"{_metricPrefix}.validate";

            if (!_configurationHelper.EnableOtp)
            {
                SendMetric($"{prefix}.forbidden");
                // Status code 404 -> resource not found (best way to say not available)
                return NotFound();
            }

            if (!(_emailHelper.IsValidEmail(request.Email) && _codeGeneratorHelper.IsValidCode(request.Otp)))
            {
                SendMetric($"{prefix}.bad_request");
                return BadRequest();
            }

            // Get code from cache and validate
            if (_activeOtps.TryGetValue(request.Email, out string validOtp) && (request.Otp == validOtp))
            {
                SendMetric($"{prefix}.valid");

                // Remove code: OTP is valid only once
                _activeOtps.Remove(request.Email);
                // Emit glup
                _glupHelper.EmitGlup(EventType.EmailValidated, request.OriginHost, userAgent);

                // Set cookie and send token back in payload
                var token = Guid.NewGuid().ToString();
                // TODO: Set a proper session cookie which is not the token (Secure and HttpOnly)
                Response.Cookies.Append(_cookieName, token, _defaultCookieOptions);

                return Ok(new { token });
            }

            SendMetric($"{prefix}.invalid");

            return NotFound(); // TODO: Discuss what to return here
        }

        #region Helpers
        private void SendMetric(string metric)
        {
            _metricsRegistry.GetOrRegister(metric, () => new Counter(Granularity.CoarseGrain)).Increment();
        }
        #endregion
    }
}
