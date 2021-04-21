using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Criteo.IdController.Helpers;
using Criteo.IdController.Helpers.Adapters;

namespace Criteo.IdController.Controllers
{
    [Route("api/[controller]")]
    public class UnAuthenticatedController : Controller
    {
        private static readonly string _metricPrefix = "unauthenticated";

        private readonly IIdentifierAdapter _uid2Adapter;
        private readonly ICookieHelper _cookieHelper;
        private readonly IMetricHelper _metricHelper;

        public UnAuthenticatedController(IIdentifierAdapter uid2Adapter, IMetricHelper metricHelper, ICookieHelper cookieHelper)
        {
            _uid2Adapter = uid2Adapter;
            _metricHelper = metricHelper;
            _cookieHelper = cookieHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrCreateIfa()
        {
            string token;

            if (_cookieHelper.TryGetIdentifierCookie(Request.Cookies, out var tokenCookie))
            {
                token = tokenCookie;
                _metricHelper.SendCounterMetric($"{_metricPrefix}.get.create.reuse");
            }
            else
            {
                // Generate a random email to generate an UID2 token for an anonymous user
                var randomId = GenerateRandomEmail();
                token = await _uid2Adapter.GetId(randomId);

                if (string.IsNullOrEmpty(token))
                {
                    _metricHelper.SendCounterMetric($"{_metricPrefix}.get.create.error.no_token");
                    return NotFound();
                }

                _metricHelper.SendCounterMetric($"{_metricPrefix}.get.create.ok");
            }

            // Set cookie
            _cookieHelper.SetIdentifierCookie(Response.Cookies, token);

            return Ok(new { token });
        }

        [HttpGet("delete")]
        public IActionResult DeleteIfa()
        {
            _metricHelper.SendCounterMetric($"{_metricPrefix}.delete");
            _cookieHelper.RemoveIdentifierCookie(Response.Cookies);

            return Ok();
        }

        private string GenerateRandomEmail() => $"{Guid.NewGuid()}@openpass.com";
    }
}
