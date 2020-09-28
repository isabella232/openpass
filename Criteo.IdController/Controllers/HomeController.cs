using System.IO;
using Metrics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Criteo.IdController.Controllers
{
    /// <summary>
    /// This provides a default home page, in case the app is serving UI content (SPA application)
    /// </summary>
    [Route("/")]
    public class HomeController : Controller
    {
        private static readonly string metricPrefix = "home.";
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMetricsRegistry _metricsRegistry;

        public HomeController(IHostingEnvironment hostingEnvironment, IMetricsRegistry metricRegistry)
        {
            _hostingEnvironment = hostingEnvironment;
            _metricsRegistry = metricRegistry;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _metricsRegistry.GetOrRegister($"{metricPrefix}.home", () => new Counter(Granularity.CoarseGrain)).Increment();
            // "dist" is the directory containing the built UI application, that should be published alongside with it from your UI project.
            return new PhysicalFileResult(Path.Combine(_hostingEnvironment.WebRootPath, "index.html"), new MediaTypeHeaderValue("text/html"));
        }
    }
}
