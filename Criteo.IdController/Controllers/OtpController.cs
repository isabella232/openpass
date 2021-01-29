using Criteo.IdController.Helpers;
using Microsoft.AspNetCore.Mvc;
using Metrics;
using static Criteo.Glup.IdController.Types;

namespace Criteo.IdController.Controllers
{
    [Route("api/[controller]")]
    public class OtpController : Controller
    {
        private static readonly string metricPrefix = "otp";
        private readonly IConfigurationHelper _configurationHelper;
        private readonly IMetricsRegistry _metricsRegistry;

        public OtpController(
            IConfigurationHelper configurationHelper,
            IMetricsRegistry metricRegistry)
        {
            _configurationHelper = configurationHelper;
            _metricsRegistry = metricRegistry;
        }

        [HttpPost]
        public IActionResult GenerateOtp(string email)
        {
            var prefix = $"{metricPrefix}.generate";

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

            // TODO: Generate code and send email
            // TODO: Emit glup for analytics

            // Status code 204 -> resource created but not content returned
            return NoContent();
        }

        #region Helpers
        private void SendMetric(string metric)
        {
            _metricsRegistry.GetOrRegister(metric, () => new Counter(Granularity.CoarseGrain)).Increment();
        }
        #endregion
    }
}
