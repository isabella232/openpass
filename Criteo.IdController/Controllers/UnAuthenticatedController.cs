using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Criteo.IdController.Helpers;
using Criteo.IdController.Helpers.Adapters;
using Metrics;

namespace Criteo.IdController.Controllers
{
    [Route("api/[controller]")]
    public class UnAuthenticatedController : Controller
    {
        private static readonly string metricPrefix = "unauthenticated";

        private readonly IIdentifierAdapter _uid2Adapter;
        private readonly IMetricsRegistry _metricsRegistry;
        private readonly ICookieHelper _cookieHelper;

        public UnAuthenticatedController(IIdentifierAdapter uid2Adapter, IMetricsRegistry metricRegistry, ICookieHelper cookieHelper)
        {
            _uid2Adapter = uid2Adapter;
            _metricsRegistry = metricRegistry;
            _cookieHelper = cookieHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrCreateIfa()
        {
            string token;

            if (_cookieHelper.TryGetIdentifierCookie(Request.Cookies, out var tokenCookie))
            {
                token = tokenCookie;
                _metricsRegistry.GetOrRegister($"{metricPrefix}.get.create.reuse", () => new Counter(Granularity.CoarseGrain)).Increment();
            }
            else
            {
                // Generate a random email to generate an UID2 token for an anonymous user
                var randomId = GenerateRandomEmail();
                token = await _uid2Adapter.GetId(randomId);

                if (string.IsNullOrEmpty(token))
                {
                    _metricsRegistry.GetOrRegister($"{metricPrefix}.get.create.error.no_token", () => new Counter(Granularity.CoarseGrain)).Increment();
                    return NotFound();
                }

                _metricsRegistry.GetOrRegister($"{metricPrefix}.get.create.ok", () => new Counter(Granularity.CoarseGrain)).Increment();
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

        private string GenerateRandomEmail() => $"{Guid.NewGuid()}@openpass.com";
    }
}
