using System;
using System.Threading.Tasks;
using Criteo.IdController.Helpers;
using Microsoft.AspNetCore.Mvc;
using Criteo.UserAgent;
using Criteo.Services.Glup;
using Criteo.UserIdentification;
using Metrics;
using static Criteo.Glup.IdController.Types;
using IdControllerGlup = Criteo.Glup.IdController;

namespace Criteo.IdController.Controllers
{
    [Route("api/[controller]")]
    public class EventController : Controller
    {
        private static readonly string metricPrefix = "event.";
        private readonly IConfigurationHelper _configurationHelper;
        private readonly IGlupService _glupService;
        private readonly IAgentSource _agentSource;
        private readonly IMetricsRegistry _metricsRegistry;
        private readonly IInternalMappingHelper _internalMappingHelper;
        private readonly Random _randomGenerator;

        public EventController(
            IConfigurationHelper configurationHelper,
            IGlupService glupService,
            IAgentSource agentSource,
            IMetricsRegistry metricRegistry,
            IInternalMappingHelper internalMappingHelper)
        {
            _configurationHelper = configurationHelper;
            _glupService = glupService;
            _agentSource = agentSource;
            _metricsRegistry = metricRegistry;
            _internalMappingHelper = internalMappingHelper;
            _randomGenerator = new Random();
        }

        [HttpPost]
        public async Task<IActionResult> SaveEvent(
            EventType eventType,
            string originHost,
            string localwebid,
            string uid,
            string ifa)
        {

            _metricsRegistry.GetOrRegister($"{metricPrefix}.save_event", () => new Counter(Granularity.CoarseGrain)).Increment();

            // the controller tries to parse the EventType from the integer received
            // EventType.Unknown is either unsuccessful or indeed a evenType = 0, invalid in both cases
            if (eventType == EventType.Unknown || string.IsNullOrEmpty(originHost))
            {
                _metricsRegistry.GetOrRegister($"{metricPrefix}.save_event.bad_request", () => new Counter(Granularity.CoarseGrain)).Increment();
                return BadRequest();
            }

            var internalLocalWebId = Guid.TryParse(localwebid, out _) // LocalWebId parses when accessing the id, causing a runtime exception if invalid Guid
                ? await _internalMappingHelper.GetInternalLocalWebId(LocalWebId.Parse(localwebid, originHost))
                : null;
            var internalUid = await _internalMappingHelper.GetInternalCriteoId(CriteoId.Parse(uid));
            var internalUserCentricAdId = await _internalMappingHelper.GetInternalUserCentricAdId(UserCentricAdId.Parse(ifa));

            var userAgentString = HttpContext?.Request?.Headers?["User-Agent"];
            var uidForUAlib = internalUserCentricAdId?.Value ?? internalUid?.Value ?? internalLocalWebId?.CriteoId?.Value;
            var parsedUserAgent = GetUserAgent(userAgentString, uidForUAlib);

            EmitGlup(eventType, originHost, parsedUserAgent, internalLocalWebId, internalUid, internalUserCentricAdId);

            return Ok(new { result = true }); // 200 OK - send content to avoid 500 Internal Error from the load-balancer
        }

        #region Helpers
        private IAgent GetUserAgent(string userAgentString, Guid? uid)
        {
            var agentKey = new AgentKey { UserAgentHeader = userAgentString };
            var result = _agentSource.Get(agentKey, uid);
            return result.Agent;
        }

        private void EmitGlup(
            EventType eventType,
            string originHost,
            IAgent userAgent,
            LocalWebId? localwebid,
            CriteoId? uid,
            UserCentricAdId? ifa)
        {
            // Using sampling ratio for an endpoint generating glups directly
            var samplingRatio = _configurationHelper.EmitGlupsRatio(originHost);
            if (_randomGenerator.NextDouble() > samplingRatio)
            {
                _metricsRegistry.GetOrRegister($"{metricPrefix}.save_event.emit_glup.over_sampling_ratio", () => new Counter(Granularity.CoarseGrain)).Increment();
                return;
            }

            _metricsRegistry.GetOrRegister($"{metricPrefix}.save_event.emit_glup", () => new Counter(Granularity.CoarseGrain)).Increment();

            // Create glup event with required fields
            var glup = new IdControllerGlup()
            {
                Event = eventType,
                OriginHost = originHost
            };

            // User Agent
            if (userAgent?.UserAgentHash != null)
                glup.Uahashfull = userAgent.UserAgentHash;
            if (userAgent?.BrowserName != null)
                glup.UaBrowserFamily = userAgent.BrowserName;
            if (userAgent?.BrowserMajorVersion != null)
                glup.UaBrowserMajor = userAgent.BrowserMajorVersion;
            if (userAgent?.BrowserMinorVersion != null)
                glup.UaBrowserMinor = userAgent.BrowserMinorVersion;
            if (userAgent?.FormFactor != null)
                glup.UaFormFactor = userAgent.FormFactor;
            if (userAgent?.OperatingSystemName != null)
                glup.UaOsFamily = userAgent.OperatingSystemName;
            if (userAgent?.OperatingSystemMajorVersion != null)
                glup.UaOsMajor = userAgent.OperatingSystemMajorVersion;
            if (userAgent?.OperatingSystemMinorVersion != null)
                glup.UaOsMinor = userAgent.OperatingSystemMinorVersion;

            // Optional
            if (localwebid.HasValue && localwebid.Value.CriteoId.HasValue)
                glup.LocalWebId = localwebid.Value.CriteoId.Value.ToString();
            if (uid.HasValue)
                glup.Uid = uid.Value.ToString();
            if (ifa.HasValue)
                glup.Ifa = ifa.Value.ToString();

            // Go!
            _glupService.Emit(glup);
        }
        #endregion
    }
}
