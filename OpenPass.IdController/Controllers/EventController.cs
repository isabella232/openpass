using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static Criteo.Glup.IdController.Types;
using OpenPass.IdController.Models;
using OpenPass.IdController.Helpers;

namespace OpenPass.IdController.Controllers
{
    [Route("api/[controller]")]
    public class EventController : Controller
    {
        private static readonly string _metricPrefix = "event";
        private readonly IConfigurationHelper _configurationHelper;
        private readonly IMetricHelper _metricHelper;

        private readonly IGlupHelper _glupHelper;
        private readonly Random _randomGenerator;
        private readonly ITrackingHelper _trackingHelper;

        public EventController(
            IConfigurationHelper configurationHelper,
            IMetricHelper metricHelper,
            IGlupHelper glupHelper,
            ITrackingHelper trackingHelper)
        {
            _configurationHelper = configurationHelper;
            _metricHelper = metricHelper;
            _glupHelper = glupHelper;
            _randomGenerator = new Random();
            _trackingHelper = trackingHelper;

        }

        [HttpPost]
        public async Task<IActionResult> SaveEvent(
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromHeader(Name = "x-origin-host")] string originHost,
            [FromHeader(Name = "x-tracked-data")] string trackedData,
            [FromBody] EventRequest request)
        {
            var saveEventPrefix = $"{_metricPrefix}.save_event";

            // the controller tries to parse the EventType from the integer received
            // EventType.Unknown is either unsuccessful or indeed a evenType = 0, invalid in both cases
            if (request == null || request.EventType == EventType.Unknown || string.IsNullOrEmpty(originHost))
            {
                _metricHelper.SendCounterMetric($"{saveEventPrefix}.bad_request");
                return BadRequest();
            }

            // Using sampling ratio for an endpoint generating glups directly
            var samplingRatio = _configurationHelper.EmitGlupsRatio(originHost);
            if (_randomGenerator.NextDouble() > samplingRatio)
            {
                _metricHelper.SendCounterMetric($"{saveEventPrefix}.over_sampling_ratio");
            }
            else
            {
                var trackingContext = await _trackingHelper.BuildTrackingContextAsync(request.EventType, trackedData);
                _metricHelper.SendCounterMetric($"{saveEventPrefix}.emit_glup.${request.EventType}");
                _glupHelper.EmitGlup(originHost, userAgent, trackingContext);
            }

            return Ok(new { result = true }); // 200 OK with dummy content
        }
    }
}
