using System.IO;
using Criteo.DevKit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using OpenPass.IdController.Helpers;

namespace OpenPass.IdController.Controllers
{
    [Route("open-pass")]
    public class OpenPassController : Controller
    {
        private static readonly string _metricPrefix = "open-pass";
        private static readonly string _distFolderName = "dist";
        private static readonly string _widgetJsPath = "widget/assets/widget.min.js";
        private static readonly string _widgetIndexHtmlPath = "dist/widget/index.html";
        private static readonly string _distIndexHtmlPath = "dist/index.html";
        private static readonly string _mediaTypeHeaderValue = "text/html";

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMetricHelper _metricHelper;

        public OpenPassController(IHostingEnvironment hostingEnvironment, IMetricHelper metricHelper)
        {
            _hostingEnvironment = hostingEnvironment;
            _metricHelper = metricHelper;
        }

        [HttpGet("widget-app")]
        public IActionResult Widget()
        {
            _metricHelper.SendCounterMetric($"{_metricPrefix}.widget-app");
            var scriptPath = Path.Combine(
                _hostingEnvironment.ContentRootPath,
                _distFolderName,
                 HostingEnvironmentHelper.GetEnvironment() == HostingEnvironment.PreProd ? "preprod" : string.Empty,
                 _widgetJsPath
             );

            return new PhysicalFileResult(scriptPath, new MediaTypeHeaderValue("application/javascript"));
        }

        [HttpGet("iframe-app")]
        public IActionResult Iframe()
        {
            _metricHelper.SendCounterMetric($"{_metricPrefix}.widget-iframe");
            return new PhysicalFileResult(Path.Combine(_hostingEnvironment.ContentRootPath, _widgetIndexHtmlPath), new MediaTypeHeaderValue(_mediaTypeHeaderValue));
        }

        [HttpGet("{*anything}")]
        public IActionResult Index()
        {
            _metricHelper.SendCounterMetric($"{_metricPrefix}.open-pass-SPA");
            // "dist" is the directory containing the built UI application, that should be published alongside with it from your UI project.
            return new PhysicalFileResult(Path.Combine(_hostingEnvironment.ContentRootPath, _distIndexHtmlPath), new MediaTypeHeaderValue(_mediaTypeHeaderValue));
        }
    }
}
