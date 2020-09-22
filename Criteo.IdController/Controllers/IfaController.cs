using System;
using Microsoft.AspNetCore.Mvc;
using Criteo.IdController.Helpers;
using Metrics;

namespace Criteo.IdController.Controllers
{
    [Route("api/[controller]")]
    public class IfaController : Controller
    {
        private readonly IUserManagementHelper _userManagementHelper;
        private readonly IMetricsRegistry _metricsRegistry;
        private readonly string IfaCookieName = "ifa";

        public IfaController(IUserManagementHelper userManagementHelper, IMetricsRegistry metricRegistry)
        {
            _userManagementHelper = userManagementHelper;
            _metricsRegistry = metricRegistry;
        }

        [HttpGet("get")]
        public IActionResult GetOrCreateIfa(string pii)
        {
            var piiPresent = !string.IsNullOrEmpty(pii);
            string ifa;

            // PII present -> get or create mapped IFA (ignoring the one in cookies)
            // PII not present -> reuse the one in cookies if available or create a new one
            if (piiPresent)
            {
                _userManagementHelper.TryGetOrCreateIfaMappingFromPii(pii, out var retrievedIfa);
                ifa = retrievedIfa.ToString();
            }
            else
            {
                if (Request.Cookies.TryGetValue(IfaCookieName, out var ifaCookie))
                    ifa = ifaCookie;
                else
                    ifa = _userManagementHelper.GenerateIfa().ToString();
            }

            // TODO: add options (domain, expires, SameSite, Secure)
            Response.Cookies.Append(IfaCookieName, ifa);

            return Ok(new
            {
                ifa = ifa
            });
        }

        [HttpGet("delete")]
        public IActionResult DeleteIfa()
        {
            Response.Cookies.Delete(IfaCookieName);

            return Ok();
        }
    }
}
