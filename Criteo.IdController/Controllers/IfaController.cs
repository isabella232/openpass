using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Criteo.IdController.Helpers;
using Criteo.IdController.Helpers.Adapters;
using Metrics;

namespace Criteo.IdController.Controllers
{
    [Route("api/[controller]")]
    public class IfaController : Controller
    {
        private static readonly string metricPrefix = "ifa.";

        private readonly IIdentifierAdapter _uid2Adapter;
        private readonly IMetricsRegistry _metricsRegistry;
        private readonly ICookieHelper _cookieHelper;

        public IfaController(IIdentifierAdapter uid2Adapter, IMetricsRegistry metricRegistry, ICookieHelper cookieHelper)
        {
            _uid2Adapter = uid2Adapter;
            _metricsRegistry = metricRegistry;
            _cookieHelper = cookieHelper;
        }

        [HttpGet]
        [HttpGet("get")]
        public async Task<IActionResult> GetOrCreateIfa()
        {
            string token;

            if (_cookieHelper.TryGetIdentifierCookie(Request.Cookies, out var tokenCookie))
            {
                token = tokenCookie;
                _metricsRegistry.GetOrRegister($"{metricPrefix}.get.reuse", () => new Counter(Granularity.CoarseGrain)).Increment();
            }
            else
            {
                // Generate a random PII to generate an UID2 token for an anonymous user
                var randomId = GenerateRandomIdentifier();
                token = await _uid2Adapter.GetId(randomId);
                // TODO: Check that token is not null and what to do in that case
                _metricsRegistry.GetOrRegister($"{metricPrefix}.get.create", () => new Counter(Granularity.CoarseGrain)).Increment();
            }

            // Set cookie
            _cookieHelper.SetIdentifierCookie(Response.Cookies, token);

            return Ok(new { token });
        }

        [HttpGet("delete")]
        public IActionResult DeleteIfa()
        {
            _metricsRegistry.GetOrRegister($"{metricPrefix}.delete", () => new Counter(Granularity.CoarseGrain)).Increment();
            _cookieHelper.RemoveIdentifierCookie(Response.Cookies);

            return Ok();
        }

        private string GenerateRandomIdentifier() => Guid.NewGuid().ToString();
    }
}
