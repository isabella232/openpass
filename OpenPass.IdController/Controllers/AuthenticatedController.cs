using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using static Criteo.Glup.IdController.Types;
using OpenPass.IdController.Helpers;
using OpenPass.IdController.Models;

namespace OpenPass.IdController.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticatedController : Controller
    {
        private const int _otpCodeLifetimeMinutes = 15;
        private const string _metricPrefix = "authenticated";

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMetricHelper _metricHelper;
        private readonly IMemoryCache _activeOtps; // Mapping: (email -> OTP)
        private readonly IConfigurationHelper _configurationHelper;
        private readonly IEmailHelper _emailHelper;
        private readonly IGlupHelper _glupHelper;
        private readonly ICodeGeneratorHelper _codeGeneratorHelper;
        private readonly ICookieHelper _cookieHelper;
        private readonly IIdentifierHelper _identifierHelper;
        private readonly ITrackingHelper _trackingHelper;

        public AuthenticatedController(
            IHostingEnvironment hostingEnvironment,
            IMetricHelper metricRegistry,
            IMemoryCache memoryCache,
            IConfigurationHelper configurationHelper,
            IEmailHelper emailHelper,
            IGlupHelper glupHelper,
            ICodeGeneratorHelper codeGeneratorHelper,
            ICookieHelper cookieHelper,
            IIdentifierHelper identifierHelper,
            ITrackingHelper trackingHelper)
        {
            _hostingEnvironment = hostingEnvironment;
            _metricHelper = metricRegistry;
            _activeOtps = memoryCache;
            _configurationHelper = configurationHelper;
            _emailHelper = emailHelper;
            _glupHelper = glupHelper;
            _codeGeneratorHelper = codeGeneratorHelper;
            _cookieHelper = cookieHelper;
            _identifierHelper = identifierHelper;
            _trackingHelper = trackingHelper;
        }

        #region One-time password (OTP)

        [HttpPost("otp/generate")]
        public async Task<IActionResult> GenerateOtp(
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromHeader(Name = "x-origin-host")] string originHost,
            [FromHeader(Name = "x-tracked-data")] string trackedData,
            [FromBody] GenerateRequest request)
        {
            var prefix = $"{_metricPrefix}.otp.generate";

            if (!_configurationHelper.EnableOtp)
            {
                _metricHelper.SendCounterMetric($"{prefix}.forbidden");
                // Status code 404 -> resource not found (best way to say not available)
                return NotFound();
            }

            if (!_emailHelper.IsValidEmail(request.Email))
            {
                _metricHelper.SendCounterMetric($"{prefix}.bad_request");
                return BadRequest();
            }

            var trackingContext = await _trackingHelper.BuildTrackingContextAsync(EventType.EmailEntered, trackedData);

            // 1. Generate OTP and add it to cache (keyed by email)
            var otp = _codeGeneratorHelper.GenerateRandomCode();
            _activeOtps.Set(request.Email, otp, TimeSpan.FromMinutes(_otpCodeLifetimeMinutes));

            if (_hostingEnvironment.IsDevelopment())
                Console.Out.WriteLine($"New OTP code generated (valid for {_otpCodeLifetimeMinutes} minutes): {request.Email} -> {otp}");

            // 2. Send email (async -> don't wait)
            _emailHelper.SendOtpEmail(request.Email, otp);

            // 3. Emit glup
            _glupHelper.EmitGlup(originHost, userAgent, trackingContext);

            // Metrics
            _metricHelper.SendCounterMetric($"{prefix}.ok");

            // Status code 204 -> resource created but not content returned
            return NoContent();
        }

        [HttpPost("otp/validate")]
        public async Task<IActionResult> ValidateOtp(
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromHeader(Name = "x-origin-host")] string originHost,
            [FromHeader(Name = "x-tracked-data")] string trackedData,
            [FromBody] ValidateRequest request)
        {
            var prefix = $"{_metricPrefix}.otp.validate";

            if (!_configurationHelper.EnableOtp)
            {
                _metricHelper.SendCounterMetric($"{prefix}.forbidden");
                // Status code 404 -> resource not found (best way to say not available)
                return NotFound();
            }

            if (!(_emailHelper.IsValidEmail(request.Email) && _codeGeneratorHelper.IsValidCode(request.Otp)))
            {
                _metricHelper.SendCounterMetric($"{prefix}.bad_request");
                return BadRequest();
            }

            // Get code from cache and validate
            if (_activeOtps.TryGetValue(request.Email, out string validOtp) && (request.Otp == validOtp))
            {
                _metricHelper.SendCounterMetric($"{prefix}.valid");

                // Remove code: OTP is valid only once
                _activeOtps.Remove(request.Email);

                var trackingContext = await _trackingHelper.BuildTrackingContextAsync(EventType.EmailValidated, trackedData);
                // Retrieve UID2 token, set cookie and send token back in payload
                var uid2Token = await _identifierHelper.TryGetUid2TokenAsync(Response.Cookies, trackingContext,
                    originHost, userAgent, request.Email, prefix);

                var ifaToken = _identifierHelper.GetOrCreateIfaToken(Request.Cookies, trackingContext, prefix, originHost, userAgent);

                // Set cookie
                _cookieHelper.SetIdentifierForAdvertisingCookie(Response.Cookies, ifaToken);

                return Ok(new { ifaToken, uid2Token });
            }
            else
            {
                _metricHelper.SendCounterMetric($"{prefix}.invalid");
            }

            return NotFound();
        }

        #endregion One-time password (OTP)

        #region External SSO services

        [HttpPost("sso")]
        public async Task<IActionResult> GenerateEmailToken(
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromHeader(Name = "x-origin-host")] string originHost,
            [FromHeader(Name = "x-tracked-data")] string trackedData,
            [FromBody] GenerateRequest request)
        {
            var prefix = $"{_metricPrefix}.sso.generate";

            if (!_emailHelper.IsValidEmail(request.Email))
            {
                _metricHelper.SendCounterMetric($"{prefix}.bad_request");
                return BadRequest();
            }

            var trackingContext = await _trackingHelper.BuildTrackingContextAsync(request.EventType, trackedData);

            // Retrieve UID2 token, set cookie and send token back in payload
            var uid2Token = await _identifierHelper.TryGetUid2TokenAsync(Response.Cookies, trackingContext,
                    originHost, userAgent, request.Email, prefix);

            var ifaToken = _identifierHelper.GetOrCreateIfaToken(Request.Cookies, trackingContext, prefix, originHost, userAgent);

            // Set cookie
            _cookieHelper.SetIdentifierForAdvertisingCookie(Response.Cookies, ifaToken);

            return Ok(new { ifaToken, uid2Token });
        }

        #endregion External SSO services
    }
}
