using System;
using Microsoft.AspNetCore.Mvc;

namespace Criteo.IdController.Controllers
{
    [Route("api/[controller]")]
    public class IfaController : Controller
    {
        private readonly string IfaCookieName = "ifa";

        public IfaController() { }

        [HttpGet("get")]
        public IActionResult GetOrCreateIfa()
        {

            if (!Request.Cookies.TryGetValue(IfaCookieName, out var ifa))
            {
                ifa = Guid.NewGuid().ToString();
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
