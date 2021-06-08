using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Criteo.UserIdentification;
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

        [HttpPost]
        public async Task<IActionResult> SaveEvent(
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromHeader(Name = "x-origin-host")] string originHost,
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

            var internalLocalWebId = Guid.TryParse(request.LocalWebId, out _) // LocalWebId parses when accessing the id, causing a runtime exception if invalid Guid
                ? await _internalMappingHelper.GetInternalLocalWebId(LocalWebId.Parse(request.LocalWebId, originHost))
                : null;
            var internalUid = await _internalMappingHelper.GetInternalCriteoId(CriteoId.Parse(request.Uid));
            var internalUserCentricAdId = await _internalMappingHelper.GetInternalUserCentricAdId(UserCentricAdId.Parse(request.Ifa));

            // Using sampling ratio for an endpoint generating glups directly
            var samplingRatio = _configurationHelper.EmitGlupsRatio(originHost);
            if (_randomGenerator.NextDouble() > samplingRatio)
            {
                _metricHelper.SendCounterMetric($"{saveEventPrefix}.over_sampling_ratio");
            }
            else
            {
                _metricHelper.SendCounterMetric($"{saveEventPrefix}.emit_glup.${request.EventType}");
                _glupHelper.EmitGlup(request.EventType, originHost, userAgent, internalLocalWebId, internalUid, internalUserCentricAdId);
            }

            return Ok(new { result = true }); // 200 OK with dummy content
        }
    }
}
