using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OpenPass.IdController.Helpers;

namespace OpenPass.IdController.Controllers
{
    [Route("api/[controller]")]
    public class UnAuthenticatedController : Controller
    {
        private static readonly string _metricPrefix = "unauthenticated";

        private readonly ICookieHelper _cookieHelper;
        private readonly IMetricHelper _metricHelper;
        private readonly IIdentifierHelper _identifierHelper;

        public UnAuthenticatedController(
            IMetricHelper metricHelper,
            ICookieHelper cookieHelper,
            IIdentifierHelper identifierHelper)
        {
            _metricHelper = metricHelper;
            _cookieHelper = cookieHelper;
            _identifierHelper = identifierHelper;
        }

        /// <summary>
        /// Get or create unique identifier
        /// </summary>
        /// <returns>generated token</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult CreateIfa()
        {
            var prefix = $"{_metricPrefix}.create";

            if (_cookieHelper.TryGetUid2AdvertisingCookie(Request.Cookies, out var uid2Token))
            {
                _metricHelper.SendCounterMetric($"{prefix}.uid2");
            }

            var ifaToken = _identifierHelper.GetOrCreateIfaToken(Request.Cookies, prefix);

            // Set cookie
            _cookieHelper.SetIdentifierForAdvertisingCookie(Response.Cookies, ifaToken);

            return Ok(new { ifaToken, uid2Token });
        }
    }
}
