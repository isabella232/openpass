using System;
using System.Threading.Tasks;
using Criteo.IdController.Helpers;
using Criteo.IdController.Helpers.Adapters;
using Criteo.IdController.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using static Criteo.Glup.IdController.Types;

namespace Criteo.IdController.Controllers
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
        private readonly IIdentifierAdapter _uid2Adapter;
        private readonly IEmailHelper _emailHelper;
        private readonly IGlupHelper _glupHelper;
        private readonly ICodeGeneratorHelper _codeGeneratorHelper;
        private readonly ICookieHelper _cookieHelper;

        public AuthenticatedController(
            IHostingEnvironment hostingEnvironment,
            IMetricHelper metricRegistry,
            IMemoryCache memoryCache,
            IConfigurationHelper configurationHelper,
            IIdentifierAdapter uid2Adapter,
            IEmailHelper emailHelper,
            IGlupHelper glupHelper,
            ICodeGeneratorHelper codeGeneratorHelper,
            ICookieHelper cookieHelper)
        {
            _hostingEnvironment = hostingEnvironment;
            _metricHelper = metricRegistry;
            _activeOtps = memoryCache;
            _configurationHelper = configurationHelper;
            _uid2Adapter = uid2Adapter;
            _emailHelper = emailHelper;
            _glupHelper = glupHelper;
            _codeGeneratorHelper = codeGeneratorHelper;
            _cookieHelper = cookieHelper;
        }

        #region One-time password (OTP)

        [HttpPost("otp/generate")]
        public IActionResult GenerateOtp(
            [FromHeader(Name = "User-Agent")] string userAgent,
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

            // 1. Generate OTP and add it to cache (keyed by email)
            var otp = _codeGeneratorHelper.GenerateRandomCode();
            _activeOtps.Set(request.Email, otp, TimeSpan.FromMinutes(_otpCodeLifetimeMinutes));

            // TODO: Check how to properly do this quick fix for OTP validation testing when doing development
            if (_hostingEnvironment.IsDevelopment())
                Console.Out.WriteLine($"New OTP code generated (valid for {_otpCodeLifetimeMinutes} minutes): {request.Email} -> {otp}");

            // 2. Send email (async -> don't wait)
            _emailHelper.SendOtpEmail(request.Email, otp);

            // 3. Emit glup
            _glupHelper.EmitGlup(EventType.EmailEntered, request.OriginHost, userAgent);

            // Metrics
            _metricHelper.SendCounterMetric($"{prefix}.ok");

            // Status code 204 -> resource created but not content returned
            return NoContent();
        }

        [HttpPost("otp/validate")]
        public async Task<IActionResult> ValidateOtp(
            [FromHeader(Name = "User-Agent")] string userAgent,
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
                // Emit glup
                _glupHelper.EmitGlup(EventType.EmailValidated, request.OriginHost, userAgent);

                // Retrieve UID2 token, set cookie and send token back in payload
                var token = await _uid2Adapter.GetId(request.Email);
                // TODO: Check token is not null, what do we do in that case?
                _cookieHelper.SetIdentifierCookie(Response.Cookies, token);

                return Ok(new { token });
            }

            _metricHelper.SendCounterMetric($"{prefix}.invalid");

            return NotFound(); // TODO: Discuss what to return here
        }

        #endregion One-time password (OTP)

        #region External SSO services

        [HttpPost("sso")]
        public async Task<IActionResult> GenerateEmailToken(
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromBody] GenerateRequest request)
        {
            var prefix = $"{_metricPrefix}.sso.generate";

            if (!_emailHelper.IsValidEmail(request.Email))
            {
                _metricHelper.SendCounterMetric($"{prefix}.bad_request");
                return BadRequest();
            }

            // 1. Emit glup
            // TODO: Create a new event type for SSO (even per SSO service)
            _glupHelper.EmitGlup(EventType.EmailValidated, request.OriginHost, userAgent);

            // 2. Retrieve UID2 token, set cookie and send token back in payload
            var token = await _uid2Adapter.GetId(request.Email);
            // TODO: Check token is not null, what do we do in that case?
            _cookieHelper.SetIdentifierCookie(Response.Cookies, token);

            // Metrics
            _metricHelper.SendCounterMetric($"{prefix}.ok");

            return Ok(new { token });
        }

        #endregion External SSO services
    }
}
