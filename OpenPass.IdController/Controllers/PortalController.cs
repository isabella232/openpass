using Microsoft.AspNetCore.Mvc;
using OpenPass.IdController.Helpers;
using static Criteo.Glup.IdController.Types;

namespace OpenPass.IdController.Controllers
{
    [Route("api/[controller]")]
    public class PortalController : Controller
    {
        private const string _metricPrefix = "controls";

        private readonly IMetricHelper _metricHelper;
        private readonly ICookieHelper _cookieHelper;
        private readonly IGlupHelper _glupHelper;
        private readonly ITrackingHelper _trackingHelper;

        public PortalController(
            IMetricHelper metricHelper,
            ICookieHelper cookieHelper,
            IGlupHelper glupHelper,
            ITrackingHelper trackingHelper)
        {
            _metricHelper = metricHelper;
            _cookieHelper = cookieHelper;
            _glupHelper = glupHelper;
            _trackingHelper = trackingHelper;
        }

        [HttpPost("controls/optout")]
        public IActionResult OptOut(
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromHeader(Name = "x-origin-host")] string originHost,
            [FromHeader(Name = "x-tracked-data")] string trackedData)
        {
            // Apply internal opt-out
            _cookieHelper.RemoveUid2AdvertisingCookie(Response.Cookies);
            _cookieHelper.RemoveIdentifierForAdvertisingCookie(Response.Cookies);
            _cookieHelper.SetOptoutCookie(Response.Cookies, "1");

            // TODO/OPT: call partners to apply opt-out on their side. This needs to be designed.

            // Emit metric and glup
            var optoutMetricPrefix = $"{_metricPrefix}.optout";
            _metricHelper.SendCounterMetric($"{optoutMetricPrefix}");

            var trackingModel = _trackingHelper.TryGetWidgetParameters(trackedData);

            _glupHelper.EmitGlup(EventType.OptOut, originHost, userAgent, trackingModel);

            return Ok();
        }

        [HttpPost("controls/optin")]
        public IActionResult OptIn(
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromHeader(Name = "x-origin-host")] string originHost,
            [FromHeader(Name = "x-tracked-data")] string trackedData)
        {
            // Apply opt-in
            _cookieHelper.RemoveOptoutCookie(Response.Cookies);

            // Emit metric and glup
            var optoutMetricPrefix = $"{_metricPrefix}.optin";
            _metricHelper.SendCounterMetric($"{optoutMetricPrefix}");

            var trackingModel = _trackingHelper.TryGetWidgetParameters(trackedData);

            _glupHelper.EmitGlup(EventType.OptIn, originHost, userAgent, trackingModel);

            return Ok();
        }
    }
}
