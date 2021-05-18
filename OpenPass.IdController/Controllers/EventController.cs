using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OpenPass.IdController.Models;
using OpenPass.IdController.Helpers;

namespace OpenPass.IdController.Controllers
{
    [Route("api/[controller]")]
    public class EventController : Controller
    {
        private static readonly string _metricPrefix = "event.";
        private readonly IMetricHelper _metricHelper;

        public EventController(IMetricHelper metricHelper)
        {
            _metricHelper = metricHelper;
        }

        /// <summary>
        /// Track events from UI
        /// </summary>
        /// <param name="request"></param>
        /// <returns>true if event is tracked</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult SaveEvent([FromBody] EventRequest request)
        {
            _metricHelper.SendCounterMetric($"{_metricPrefix}.save_event");

            // the controller tries to parse the EventType from the integer received
            // EventType.Unknown is either unsuccessful or indeed a evenType = 0, invalid in both cases
            if (request.EventType == EventType.Unknown || string.IsNullOrEmpty(request.OriginHost))
            {
                _metricHelper.SendCounterMetric($"{_metricPrefix}.save_event.bad_request");
                return BadRequest();
            }

            return Ok(new { result = true }); // 200 OK with dummy content
        }
    }
}
