using System;
using Microsoft.AspNetCore.Mvc;
using Criteo.IdController.Helpers;
using Metrics;

namespace Criteo.IdController.Controllers
{
    [Route("api/[controller]")]
    public class IfaController : Controller
    {
        private static readonly string metricPrefix = "ifa.";
        private readonly IUserManagementHelper _userManagementHelper;
        private readonly IMetricsRegistry _metricsRegistry;
        private readonly string IfaCookieName = "ifa";

        public IfaController(IUserManagementHelper userManagementHelper, IMetricsRegistry metricRegistry)
        {
            _userManagementHelper = userManagementHelper;
            _metricsRegistry = metricRegistry;
        }

        [HttpGet("get")]
        public IActionResult GetOrCreateIfa(string pii)
        {

            _metricsRegistry.GetOrRegister($"{metricPrefix}.get_create", () => new Counter(Granularity.CoarseGrain)).Increment();
            var piiPresent = !string.IsNullOrEmpty(pii);
            string ifa;

            // PII present -> get or create mapped IFA (ignoring the one in cookies)
            // PII not present -> reuse the one in cookies if available or create a new one
            if (piiPresent)
            {
                _metricsRegistry.GetOrRegister($"{metricPrefix}.get_create.pii_present", () => new Counter(Granularity.CoarseGrain)).Increment();
                _userManagementHelper.TryGetOrCreateIfaMappingFromPii(pii, out var retrievedIfa);
                ifa = retrievedIfa.ToString();
            }
            else
            {
                _metricsRegistry.GetOrRegister($"{metricPrefix}.get_create.pii_not_present", () => new Counter(Granularity.CoarseGrain)).Increment();
                if (Request.Cookies.TryGetValue(IfaCookieName, out var ifaCookie))
                    ifa = ifaCookie;
                else
                    ifa = _userManagementHelper.GenerateIfa().ToString();
            }

            // TODO: add options (domain, expires, SameSite, Secure)
            Response.Cookies.Append(IfaCookieName, ifa);

            return Ok(new
            {
                ifa = ifa
            });
        }

        [HttpGet("delete")]
        public IActionResult DeleteIfa()
        {
            _metricsRegistry.GetOrRegister($"{metricPrefix}.delete", () => new Counter(Granularity.CoarseGrain)).Increment();
            Response.Cookies.Delete(IfaCookieName);

            return Ok();
        }
    }
}
