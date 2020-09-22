using Metrics;
using Microsoft.AspNetCore.Mvc;

namespace Criteo.IdController.Controllers
{
    [Route("api/[controller]")]
    public class WidgetController : Controller
    {
        private readonly IMetricsRegistry _metricsRegistry;

        public WidgetController(IMetricsRegistry metricRegistry)
        {
            _metricsRegistry = metricRegistry;
        }

        [HttpGet]
        public IActionResult InitialBanner(string originHost)
        {
            ViewData["originHost"] = originHost;
            return View();
        }

        // Options banner, which contains the acceptance (or not) to be included into the id-network
        [HttpGet("options")]
        public IActionResult OptionsBanner(string originHost)
        {
            ViewData["originHost"] = originHost;
            return View();
        }

        // 'Learn more' site, which shows more information about how the id-network works
        [HttpGet("learn")]
        public IActionResult LearnMoreSite(string originHost)
        {
            ViewData["originHost"] = originHost;
            return View();
        }
    }
}
