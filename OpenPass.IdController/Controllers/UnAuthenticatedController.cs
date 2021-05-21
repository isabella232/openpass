using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenPass.IdController.Helpers;
using OpenPass.IdController.Helpers.Adapters;
using OpenPass.IdController.Models;
using static Criteo.Glup.IdController.Types;

namespace OpenPass.IdController.Controllers
{
    [Route("api/[controller]")]
    public class UnAuthenticatedController : Controller
    {
        private static readonly string _metricPrefix = "unauthenticated";

        private readonly IIdentifierAdapter _uid2Adapter;
        private readonly ICookieHelper _cookieHelper;
        private readonly IMetricHelper _metricHelper;
        private readonly IGlupHelper _glupHelper;

        public UnAuthenticatedController(IIdentifierAdapter uid2Adapter, IMetricHelper metricHelper, ICookieHelper cookieHelper, IGlupHelper glupHelper)
        {
            _uid2Adapter = uid2Adapter;
            _metricHelper = metricHelper;
            _cookieHelper = cookieHelper;
            _glupHelper = glupHelper;
        }

        [HttpPost]
        public async Task<IActionResult> GetOrCreateIfa(
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromBody] GenerateRequest request
        )
        {
            string token;
            var prefix = $"{_metricPrefix}.get.create";

            if (_cookieHelper.TryGetIdentifierCookie(Request.Cookies, out var tokenCookie))
            {
                token = tokenCookie;
                _metricHelper.SendCounterMetric($"{prefix}.reuse");
                _glupHelper.EmitGlup(EventType.ReuseIfa, request.OriginHost, userAgent);
            }
            else
            {
                // Generate a random email to generate an UID2 token for an anonymous user
                var randomId = GenerateRandomEmail();
                token = await _uid2Adapter.GetId(randomId);

                if (string.IsNullOrEmpty(token))
                {
                    _metricHelper.SendCounterMetric($"{prefix}.error.no_token");
                    return NotFound();
                }

                _metricHelper.SendCounterMetric($"{prefix}.ok");
                _glupHelper.EmitGlup(EventType.NewIfa, request.OriginHost, userAgent);
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
