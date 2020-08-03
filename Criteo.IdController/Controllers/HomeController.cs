using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Criteo.IdController.Controllers
{
    /// <summary>
    /// This provides a default home page, in case the app is serving UI content (SPA application)
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // "dist" is the directory containing the built UI application, that should be published alongside with it from your UI project.
            return new PhysicalFileResult(Path.Combine(_hostingEnvironment.ContentRootPath, "dist/index.html"), new MediaTypeHeaderValue("text/html"));
        }
    }
}
