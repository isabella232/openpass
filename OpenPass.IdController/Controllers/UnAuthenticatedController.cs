using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OpenPass.IdController.Helpers;
using OpenPass.IdController.Helpers.Adapters;

namespace OpenPass.IdController.Controllers
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

        /// <summary>
        /// Get or create unique identifier
        /// </summary>
        /// <returns>generated token</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrCreateIfa()
        {
            string token;
            var prefix = $"{_metricPrefix}.get.create";

            if (_cookieHelper.TryGetIdentifierCookie(Request.Cookies, out var tokenCookie))
            {
                token = tokenCookie;
                _metricHelper.SendCounterMetric($"{prefix}.reuse");
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
            }

            // Set cookie
            _cookieHelper.SetIdentifierCookie(Response.Cookies, token);

            return Ok(new { token });
        }

        private string GenerateRandomEmail() => $"{Guid.NewGuid()}@openpass.com";
    }
}
