using Microsoft.AspNetCore.Mvc;

namespace Criteo.IdController.Controllers
{
    /// <summary>
    /// This is an example Controller.
    /// You should:
    /// - Rename it and change the code for your needs, or delete it entirely
    /// - Create new Controllers as usual, templates are available in VS using the "Add ->" context menu
    /// </summary>
    [Route("api/[controller]")]
    public class HelloWorldController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return "Hello World";
        }
    }
}
