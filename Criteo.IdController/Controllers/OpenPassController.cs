using System.IO;
using Criteo.DevKit;
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
        private static readonly string _distFolderName = "dist";
        private static readonly string _widgetJsPath = "widget/assets/widget.js";
        private static readonly string _widgetIndexHtmlPath = "dist/widget/index.html";
        private static readonly string _distIndexHtmlPath = "dist/index.html";
        private static readonly string _mediaTypeHeaderValue = "text/html";

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMetricsRegistry _metricsRegistry;

        public OpenPassController(IHostingEnvironment hostingEnvironment, IMetricsRegistry metricRegistry)
        {
            _hostingEnvironment = hostingEnvironment;
            _metricsRegistry = metricRegistry;
        }

        [HttpGet("widget-app")]
        public IActionResult Widget([FromQuery(Name = "isIframe")] string isIframe, [FromQuery(Name = "iframeParent")] string iframeParent)
        {
            var scriptPath = Path.Combine(
                _hostingEnvironment.ContentRootPath,
                _distFolderName,
                 HostingEnvironmentHelper.GetEnvironment() == HostingEnvironment.PreProd ? "preprod" : string.Empty,
                 _widgetJsPath
             );

            if (isIframe?.Length > 0)
            {
                // TODO: implement iframe loading
                return Ok(isIframe);
            }
            else
            {
                return new PhysicalFileResult(scriptPath, new MediaTypeHeaderValue("application/javascript"));
            }
        }

        [HttpGet("iframe-app")]
        public IActionResult Iframe()
        {
            _metricsRegistry.GetOrRegister($"{metricPrefix}.widget-iframe", () => new Counter(Granularity.CoarseGrain)).Increment();
            return new PhysicalFileResult(Path.Combine(_hostingEnvironment.ContentRootPath, _widgetIndexHtmlPath), new MediaTypeHeaderValue(_mediaTypeHeaderValue));
        }

        [HttpGet("{*anything}")]
        public IActionResult Index()
        {
            _metricsRegistry.GetOrRegister($"{metricPrefix}.open-pass-SPA", () => new Counter(Granularity.CoarseGrain)).Increment();
            // "dist" is the directory containing the built UI application, that should be published alongside with it from your UI project.
            return new PhysicalFileResult(Path.Combine(_hostingEnvironment.ContentRootPath, _distIndexHtmlPath), new MediaTypeHeaderValue(_mediaTypeHeaderValue));
        }
    }
}
