using System;
using Microsoft.AspNetCore.Mvc;
using OpenPass.IdController.Helpers;
using OpenPass.IdController.Models;
using static Criteo.Glup.IdController.Types;

namespace OpenPass.IdController.Controllers
{
    [Route("api/[controller]")]
    public class UnAuthenticatedController : Controller
    {
        private static readonly string _metricPrefix = "unauthenticated";

        private readonly ICookieHelper _cookieHelper;
        private readonly IMetricHelper _metricHelper;
        private readonly IGlupHelper _glupHelper;

        public UnAuthenticatedController(IMetricHelper metricHelper, ICookieHelper cookieHelper, IGlupHelper glupHelper)
        {
            _metricHelper = metricHelper;
            _cookieHelper = cookieHelper;
            _glupHelper = glupHelper;
        }

        [HttpPost]
        public IActionResult CreateIfa(
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromBody] GenerateRequest request
        )
        {
            var prefix = $"{_metricPrefix}.create";

            if (_cookieHelper.TryGetUid2AdvertisingCookie(Request.Cookies, out var uid2Identifier))
            {
                _metricHelper.SendCounterMetric($"{prefix}.uid2");
            }
            if (_cookieHelper.TryGetIdentifierForAdvertisingCookie(Request.Cookies, out var token))
            {
                _metricHelper.SendCounterMetric($"{prefix}.reuse");
                _glupHelper.EmitGlup(EventType.ReuseIfa, request.OriginHost, userAgent);
            }
            else
            {
                // Generate a random guid token for an anonymous user
                token = GenerateRandomGuid();
                _metricHelper.SendCounterMetric($"{prefix}.ok");
                _glupHelper.EmitGlup(EventType.NewIfa, request.OriginHost, userAgent);
            }

            // Set cookie
            _cookieHelper.SetIdentifierForAdvertisingCookie(Response.Cookies, token);

            return Ok(new { token, uid2Identifier });
        }

        [HttpGet("delete")]
        public IActionResult DeleteIfa()
        {
            _metricHelper.SendCounterMetric($"{_metricPrefix}.delete");
            _cookieHelper.RemoveUid2AdvertisingCookie(Response.Cookies);

            return Ok();
        }

        private string GenerateRandomGuid() => Guid.NewGuid().ToString();
    }
}
