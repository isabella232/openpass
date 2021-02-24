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
        private readonly ICookieHelper _cookieHelper;

        public IfaController(IIdentifierGeneratorHelper identifierGeneratorHelper, IMetricsRegistry metricRegistry, ICookieHelper cookieHelper)
        {
            _identifierGeneratorHelper = identifierGeneratorHelper;
            _metricsRegistry = metricRegistry;
            _cookieHelper = cookieHelper;
        }

        [HttpGet]
        [HttpGet("get")]
        public IActionResult GetOrCreateIfa()
        {
            string identifier;

            if (_cookieHelper.TryGetIdentifierCookie(Request.Cookies, out var identifierCookie))
            {
                identifier = identifierCookie;
                _metricsRegistry.GetOrRegister($"{metricPrefix}.get.reuse", () => new Counter(Granularity.CoarseGrain)).Increment();
            }
            else
            {
                identifier = _identifierGeneratorHelper.GenerateIdentifier().ToString();
                _metricsRegistry.GetOrRegister($"{metricPrefix}.get.create", () => new Counter(Granularity.CoarseGrain)).Increment();
            }

            // Set cookie
            _cookieHelper.SetIdentifierCookie(Response.Cookies, identifier);

            return Ok(new { token = identifier });
        }

        [HttpGet("delete")]
        public IActionResult DeleteIfa()
        {
            _metricsRegistry.GetOrRegister($"{metricPrefix}.delete", () => new Counter(Granularity.CoarseGrain)).Increment();
            _cookieHelper.RemoveIdentifierCookie(Response.Cookies);

            return Ok();
        }
    }
}
