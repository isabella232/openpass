using System;
using Criteo.Services.Glup;
using Criteo.UserAgent;
using OpenPass.IdController.Models.Tracking;
using IdControllerGlup = Criteo.Glup.IdController;

namespace OpenPass.IdController.Helpers
{
    public interface IGlupHelper
    {
        void EmitGlup(
            string originHost,
            string userAgentString,
            TrackingContext trackingContext);
    }

    public class GlupHelper : IGlupHelper
    {
        private readonly IGlupService _glupService;
        private readonly IAgentSource _agentSource;

        public GlupHelper(IGlupService glupService, IAgentSource agentSource)
        {
            _glupService = glupService;
            _agentSource = agentSource;
        }

        public void EmitGlup(
            string originHost,
            string userAgentString,
            TrackingContext trackingContext)
        {
            // Create glup event with required fields
            var glup = new IdControllerGlup
            {
                Event = trackingContext.EventType,
                OriginHost = originHost ?? "" // must never be null
            };

            var uidForUAlib = trackingContext.LocalWebId?.CriteoId?.Value;
            var userAgent = GetUserAgent(userAgentString, uidForUAlib);

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
            if (trackingContext.LocalWebId.HasValue && trackingContext.LocalWebId.Value.CriteoId.HasValue)
                glup.LocalWebId = trackingContext.LocalWebId.Value.CriteoId.Value.ToString();
            if (!string.IsNullOrEmpty(trackingContext.Uid2))
                glup.Uid = trackingContext.Uid2;
            if (!string.IsNullOrEmpty(trackingContext.Ifa))
                glup.Ifa = trackingContext.Ifa;

            SetWidgetParameters(trackingContext, glup);

            // Go!
            _glupService.Emit(glup);
        }

        #region Helpers

        private void SetWidgetParameters(TrackingContext context, IdControllerGlup glup)
        {
            if (!string.IsNullOrEmpty(context.Provider))
                glup.Provider = context.Provider;
            if (!string.IsNullOrEmpty(context.Session))
                glup.Session = context.Session;
            if (!string.IsNullOrEmpty(context.Variant))
                glup.Variant = context.Variant;
            if (!string.IsNullOrEmpty(context.View))
                glup.View = context.View;
        }

        private IAgent GetUserAgent(string userAgentString, Guid? uid)
        {
            var agentKey = new AgentKey { UserAgentHeader = userAgentString };
            var result = _agentSource.Get(agentKey, uid);
            return result.Agent;
        }

        #endregion Helpers
    }
}
