using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Criteo.IdController.Controllers
{
    [Route("api/[controller]")]
    public class WidgetController : Controller
    {
        public WidgetController() { }

        [HttpGet]
        public IActionResult InitialBanner()
        {
            return View();
        }

        // Options banner, which contains the acceptance (or not) to be included into the id-network
        [HttpGet("options")]
        public IActionResult OptionsBanner()
        {
            return View();
        }

        // 'Learn more' site, which shows more information about how the id-network works
        [HttpGet("learn")]
        public IActionResult LearnMoreSite()
        {
            return View();
        }
    }
}
