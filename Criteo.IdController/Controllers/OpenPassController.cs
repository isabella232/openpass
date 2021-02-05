using System.IO;
using Metrics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Criteo.IdController.Controllers
{
    [Route("open-pass")]
    public class OpenPassController : Controller
    {

        private static readonly string metricPrefix = "open-pass.";
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMetricsRegistry _metricsRegistry;

        public OpenPassController(IHostingEnvironment hostingEnvironment, IMetricsRegistry metricRegistry)
        {
            _hostingEnvironment = hostingEnvironment;
            _metricsRegistry = metricRegistry;
        }

        [HttpGet("{*anything}")]
        public IActionResult Index()
        {
            _metricsRegistry.GetOrRegister($"{metricPrefix}.open-pass-SPA", () => new Counter(Granularity.CoarseGrain)).Increment();
            // "dist" is the directory containing the built UI application, that should be published alongside with it from your UI project.
            return new PhysicalFileResult(Path.Combine(_hostingEnvironment.ContentRootPath, "dist/index.html"), new MediaTypeHeaderValue("text/html"));
        }
    }
}
