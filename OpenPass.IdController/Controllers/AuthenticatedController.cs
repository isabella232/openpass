using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using OpenPass.IdController.Helpers;
using OpenPass.IdController.Helpers.Adapters;
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
        private readonly IIdentifierAdapter _uid2Adapter;
        private readonly IEmailHelper _emailHelper;
        private readonly ICodeGeneratorHelper _codeGeneratorHelper;
        private readonly ICookieHelper _cookieHelper;

        public AuthenticatedController(
            IHostingEnvironment hostingEnvironment,
            IMetricHelper metricRegistry,
            IMemoryCache memoryCache,
            IIdentifierAdapter uid2Adapter,
            IEmailHelper emailHelper,
            ICodeGeneratorHelper codeGeneratorHelper,
            ICookieHelper cookieHelper)
        {
            _hostingEnvironment = hostingEnvironment;
            _metricHelper = metricRegistry;
            _activeOtps = memoryCache;
            _uid2Adapter = uid2Adapter;
            _emailHelper = emailHelper;
            _codeGeneratorHelper = codeGeneratorHelper;
            _cookieHelper = cookieHelper;
        }

        #region One-time password (OTP)

        /// <summary>
        /// Send email with one time password
        /// </summary>
        /// <param name="request"></param>
        /// <returns>No content</returns>
        [HttpPost("otp/generate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GenerateOtp([FromBody] GenerateRequest request)
        {
            var prefix = $"{_metricPrefix}.otp.generate";

            if (!_emailHelper.IsValidEmail(request.Email))
            {
                _metricHelper.SendCounterMetric($"{prefix}.bad_request");
                return BadRequest();
            }

            // Generate OTP and add it to cache (keyed by email)
            var otp = _codeGeneratorHelper.GenerateRandomCode();
            _activeOtps.Set(request.Email, otp, TimeSpan.FromMinutes(_otpCodeLifetimeMinutes));

            if (_hostingEnvironment.IsDevelopment())
                Console.Out.WriteLine($"New OTP code generated (valid for {_otpCodeLifetimeMinutes} minutes): {request.Email} -> {otp}");

            // Send email (async -> don't wait)
            _emailHelper.SendOtpEmail(request.Email, otp);

            // Metrics
            _metricHelper.SendCounterMetric($"{prefix}.ok");

            // Status code 204 -> resource created but not content returned
            return NoContent();
        }

        /// <summary>
        /// Validate one time password
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Token if it valid</returns>
        [HttpPost("otp/validate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ValidateOtp([FromBody] ValidateRequest request)
        {
            var prefix = $"{_metricPrefix}.otp.validate";

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

                // Retrieve UID2 token, set cookie and send token back in payload
                var token = await _uid2Adapter.GetId(request.Email);
                if (string.IsNullOrEmpty(token))
                {
                    _metricHelper.SendCounterMetric($"{prefix}.error.no_token");
                }
                else
                {
                    _metricHelper.SendCounterMetric($"{prefix}.ok");

                    // Set cookie
                    _cookieHelper.SetIdentifierCookie(Response.Cookies, token);
                    return Ok(new { token });
                }
            }
            else
            {
                _metricHelper.SendCounterMetric($"{prefix}.invalid");
            }

            return NotFound();
        }

        #endregion One-time password (OTP)

        #region External SSO services

        /// <summary>
        /// SSO login (Facebook, Google)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>generated token</returns>
        [HttpPost("sso")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GenerateEmailToken([FromBody] GenerateRequest request)
        {
            var prefix = $"{_metricPrefix}.sso.generate";

            if (!_emailHelper.IsValidEmail(request.Email))
            {
                _metricHelper.SendCounterMetric($"{prefix}.bad_request");
                return BadRequest();
            }

            // 1. Emit glup
            //_glupHelper.EmitGlup(request.EventType, request.OriginHost, userAgent);

            // 2. Retrieve UID2 token, set cookie and send token back in payload
            var token = await _uid2Adapter.GetId(request.Email);

            if (string.IsNullOrEmpty(token))
            {
                _metricHelper.SendCounterMetric($"{prefix}.error.no_token");
                return NotFound();
            }

            _cookieHelper.SetIdentifierCookie(Response.Cookies, token);

            // Metrics
            _metricHelper.SendCounterMetric($"{prefix}.ok");

            return Ok(new { token });
        }

        #endregion External SSO services
    }
}
