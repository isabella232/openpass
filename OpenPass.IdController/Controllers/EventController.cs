using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OpenPass.IdController.Models;
using OpenPass.IdController.Helpers;

namespace OpenPass.IdController.Controllers
{
    [Route("api/[controller]")]
    public class EventController : Controller
    {
        private static readonly string _metricPrefix = "event";
        private readonly IMetricHelper _metricHelper;

        public EventController(IMetricHelper metricHelper)
        {
            _metricHelper = metricHelper;
        }

        /// <summary>
        /// Track events from UI
        /// </summary>
        /// <param name="request"></param>
        /// <param name="originHost"></param>
        /// <returns>true if event is tracked</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult SaveEvent(
            [FromBody] EventRequest request,
            [FromHeader(Name = "x-origin-host")] string originHost)
        {
            var saveEventPrefix = $"{_metricPrefix}.save_event";

            // the controller tries to parse the EventType from the integer received
            // EventType.Unknown is either unsuccessful or indeed a evenType = 0, invalid in both cases
            if (request == null || request.EventType == EventType.Unknown || string.IsNullOrEmpty(originHost))
            {
                _metricHelper.SendCounterMetric($"{saveEventPrefix}.bad_request");
                return BadRequest();
            }

            return Ok(new { result = true }); // 200 OK with dummy content
        }
    }
}
