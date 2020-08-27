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
    }
}
