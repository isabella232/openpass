using System;
using System.Threading.Tasks;
using Criteo.IdController.Helpers;
using Microsoft.AspNetCore.Mvc;
using Criteo.UserIdentification;
using static Criteo.Glup.IdController.Types;

namespace Criteo.IdController.Controllers
{
    [Route("api/[controller]")]
    public class EventController : Controller
    {
        private static readonly string _metricPrefix = "event.";
        private readonly IConfigurationHelper _configurationHelper;
        private readonly IMetricHelper _metricHelper;
        private readonly IInternalMappingHelper _internalMappingHelper;

        private readonly IGlupHelper _glupHelper;
        private readonly Random _randomGenerator;

        public EventController(
            IConfigurationHelper configurationHelper,
            IMetricHelper metricHelper,
            IInternalMappingHelper internalMappingHelper,
            IGlupHelper glupHelper)
        {
            _configurationHelper = configurationHelper;
            _metricHelper = metricHelper;
            _internalMappingHelper = internalMappingHelper;
            _glupHelper = glupHelper;
            _randomGenerator = new Random();
        }

        #region Request model

        public class EventRequest
        {
            public EventType EventType { get; set; }
            public string OriginHost { get; set; }
            public string LocalWebId { get; set; }
            public string Uid { get; set; }
            public string Ifa { get; set; }
        }

        #endregion Request model

        [HttpPost]
        public async Task<IActionResult> SaveEvent(
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromBody] EventRequest request)
        {
            _metricHelper.SendCounterMetric($"{_metricPrefix}.save_event");

            // the controller tries to parse the EventType from the integer received
            // EventType.Unknown is either unsuccessful or indeed a evenType = 0, invalid in both cases
            if (request.EventType == EventType.Unknown || string.IsNullOrEmpty(request.OriginHost))
            {
                _metricHelper.SendCounterMetric($"{_metricPrefix}.save_event.bad_request");
                return BadRequest();
            }

            var internalLocalWebId = Guid.TryParse(request.LocalWebId, out _) // LocalWebId parses when accessing the id, causing a runtime exception if invalid Guid
                ? await _internalMappingHelper.GetInternalLocalWebId(LocalWebId.Parse(request.LocalWebId, request.OriginHost))
                : null;
            var internalUid = await _internalMappingHelper.GetInternalCriteoId(CriteoId.Parse(request.Uid));
            var internalUserCentricAdId = await _internalMappingHelper.GetInternalUserCentricAdId(UserCentricAdId.Parse(request.Ifa));

            // Using sampling ratio for an endpoint generating glups directly
            var samplingRatio = _configurationHelper.EmitGlupsRatio(request.OriginHost);
            if (_randomGenerator.NextDouble() > samplingRatio)
            {
                _metricHelper.SendCounterMetric($"{_metricPrefix}.save_event.emit_glup.over_sampling_ratio");
            }
            else
            {
                _metricHelper.SendCounterMetric($"{_metricPrefix}.save_event.emit_glup");
                _glupHelper.EmitGlup(request.EventType, request.OriginHost, userAgent, internalLocalWebId, internalUid, internalUserCentricAdId);
            }

            return Ok(new { result = true }); // 200 OK with dummy content
        }
    }
}
