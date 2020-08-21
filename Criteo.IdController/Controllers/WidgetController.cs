using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Criteo.IdController.Controllers
{
    [Route("api/[controller]")]
    public class WidgetController : Controller
    {
        private static readonly string _widgetIframeTemplate = @"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset=""utf-8"" />
                <title>IdController widget</title>
                <style>
                    {0}
                </style>
            </head>
            <body>
                <h1>Improve your ad experience</h1>
                <p>Discover more relevant ad content.</p>
                <p>Opt-in to receive your tailored ad experience.</p>
                <button>Learn more</button>
            </body>
            </html>";

        private static readonly string _widgetStyleTemplate = @"
            body {{
                background: white;
                height: 100%;
                max-height: 150px;
                max-width: 300px;
                width: 100%;
            }}
            h1 {{
                font-size: 1.5em;
            }}
            p {{
                font-size: 1em;
            }}
            ";

        public WidgetController() { }

        [HttpGet]
        public ContentResult Get()
        {
            // TODO: Add variables to the style template and inject them here
            var style = string.Format(_widgetStyleTemplate);
            var iframe = string.Format(_widgetIframeTemplate, style);

            return new ContentResult()
            {
                ContentType = "text/html",
                StatusCode = (int) HttpStatusCode.OK,
                Content = iframe
            };
        }
    }
}
