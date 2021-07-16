using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IEmailHelper _emailHelper;
        private readonly ICodeGeneratorHelper _codeGeneratorHelper;
        private readonly ICookieHelper _cookieHelper;
        private readonly IIdentifierHelper _identifierHelper;

        public AuthenticatedController(
            IHostingEnvironment hostingEnvironment,
            IMetricHelper metricRegistry,
            IMemoryCache memoryCache,
            IEmailHelper emailHelper,
            ICodeGeneratorHelper codeGeneratorHelper,
            ICookieHelper cookieHelper,
            IIdentifierHelper identifierHelper)
        {
            _hostingEnvironment = hostingEnvironment;
            _metricHelper = metricRegistry;
            _activeOtps = memoryCache;
            _emailHelper = emailHelper;
            _codeGeneratorHelper = codeGeneratorHelper;
            _cookieHelper = cookieHelper;
            _identifierHelper = identifierHelper;
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
                var uid2Token = await _identifierHelper.TryGetUid2TokenAsync(Response.Cookies, request.Email, prefix);

                var ifaToken = _identifierHelper.GetOrCreateIfaToken(Request.Cookies, prefix);

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

            // Retrieve UID2 token, set cookie and send token back in payload
            var uid2Token = await _identifierHelper.TryGetUid2TokenAsync(Response.Cookies, request.Email, prefix);

            var ifaToken = _identifierHelper.GetOrCreateIfaToken(Request.Cookies, prefix);

            // Set cookie
            _cookieHelper.SetIdentifierForAdvertisingCookie(Response.Cookies, ifaToken);

            return Ok(new { ifaToken, uid2Token });
        }

        #endregion External SSO services
    }
}
