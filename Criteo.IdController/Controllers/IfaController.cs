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
        private readonly IIdentifierGeneratorHelper _identifierGeneratorHelper;
        private readonly IMetricsRegistry _metricsRegistry;
        private readonly string IfaCookieName = "ifa";

        public IfaController(IIdentifierGeneratorHelper identifierGeneratorHelper, IMetricsRegistry metricRegistry)
        {
            _identifierGeneratorHelper = identifierGeneratorHelper;
            _metricsRegistry = metricRegistry;
        }

        [HttpGet]
        [HttpGet("get")]
        public IActionResult GetOrCreateIfa()
        {
            string ifa;

            if (Request.Cookies.TryGetValue(IfaCookieName, out var ifaCookie))
            {
                ifa = ifaCookie;
                _metricsRegistry.GetOrRegister($"{metricPrefix}.get.reuse", () => new Counter(Granularity.CoarseGrain)).Increment();
            }
            else
            {
                ifa = _identifierGeneratorHelper.GenerateIdentifier().ToString();
                _metricsRegistry.GetOrRegister($"{metricPrefix}.get.create", () => new Counter(Granularity.CoarseGrain)).Increment();
            }

            // TODO: add options (domain, expires, SameSite, Secure)
            Response.Cookies.Append(IfaCookieName, ifa);

            return Ok(new { ifa });
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
