using Microsoft.AspNetCore.Mvc;
using OpenPass.IdController.Helpers;

namespace OpenPass.IdController.Controllers
{
    [Route("api/[controller]")]
    public class PortalController : Controller
    {
        private const string _metricPrefix = "controls";

        private readonly IMetricHelper _metricHelper;
        private readonly ICookieHelper _cookieHelper;

        public PortalController(IMetricHelper metricHelper, ICookieHelper cookieHelper)
        {
            _metricHelper = metricHelper;
            _cookieHelper = cookieHelper;
        }

        /// <summary>
        /// Perfrom Optout.
        /// Remove Ifa and Uid2 cookies and set Optout cookie
        /// </summary>
        /// <returns></returns>
        [HttpPost("controls/optout")]
        public IActionResult OptOut()
        {
            // Apply internal opt-out
            _cookieHelper.RemoveUid2AdvertisingCookie(Response.Cookies);
            _cookieHelper.RemoveIdentifierForAdvertisingCookie(Response.Cookies);
            _cookieHelper.SetOptoutCookie(Response.Cookies, "1");

            // Emit metric and glup
            var optoutMetricPrefix = $"{_metricPrefix}.optout";
            _metricHelper.SendCounterMetric($"{optoutMetricPrefix}");

            return Ok();
        }

        /// <summary>
        /// Perfrom OptIn and remove Optout cookie
        /// </summary>
        /// <returns></returns>
        [HttpPost("controls/optin")]
        public IActionResult OptIn()
        {
            // Apply opt-in
            _cookieHelper.RemoveOptoutCookie(Response.Cookies);

            // Emit metric and glup
            var optoutMetricPrefix = $"{_metricPrefix}.optin";
            _metricHelper.SendCounterMetric($"{optoutMetricPrefix}");

            return Ok();
        }
    }
}
