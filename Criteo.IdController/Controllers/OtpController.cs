using System;
using System.Linq;
using Criteo.IdController.Helpers;
using Microsoft.AspNetCore.Mvc;
using Metrics;
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

        private readonly IConfigurationHelper _configurationHelper;
        private readonly IMetricsRegistry _metricsRegistry;
        private readonly IMemoryCache _activeOtps; // Mapping: (email -> OTP)
        private readonly Random _randomGenerator;

        public OtpController(
            IConfigurationHelper configurationHelper,
            IMetricsRegistry metricRegistry,
            IMemoryCache memoryCache)
        {
            _configurationHelper = configurationHelper;
            _metricsRegistry = metricRegistry;
            _activeOtps = memoryCache;
            _randomGenerator = new Random();
        }

        [HttpPost]
        public IActionResult GenerateOtp(string email)
        {
            var prefix = $"{_metricPrefix}.generate";

            if (!_configurationHelper.EnableOtp)
            {
                SendMetric($"{prefix}.forbidden");
                // Status code 404 -> resource not found (best way to say not available)
                return NotFound();
            }

            if (string.IsNullOrEmpty(email))
            {
                SendMetric($"{prefix}.bad_request");
                return BadRequest();
            }

            // Generate OTP and add it to cache (keyed by email)
            var otp = GenerateRandomCode();
            _activeOtps.Set(email, otp, TimeSpan.FromMinutes(_otpCodeLifetimeMinutes));
            // TODO: Send email
            // TODO: Emit glup for analytics

            // Status code 204 -> resource created but not content returned
            return NoContent();
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
        #endregion
    }
}
