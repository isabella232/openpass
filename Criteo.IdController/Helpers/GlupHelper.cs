using System;
using Criteo.Services.Glup;
using Criteo.UserAgent;
using Criteo.UserIdentification;
using IdControllerGlup = Criteo.Glup.IdController;
using static Criteo.Glup.IdController.Types;

namespace Criteo.IdController.Helpers
{
    public interface IGlupHelper
    {
        void EmitGlup(
            EventType eventType,
            string originHost,
            string userAgentString,
            LocalWebId? localwebid = null,
            CriteoId? uid = null,
            UserCentricAdId? ifa = null);
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
            EventType eventType,
            string originHost,
            string userAgentString,
            LocalWebId? localwebid = null,
            CriteoId? uid = null,
            UserCentricAdId? ifa = null)
        {
            // Create glup event with required fields
            var glup = new IdControllerGlup()
            {
                Event = eventType,
                OriginHost = originHost
            };

            var uidForUAlib = ifa?.Value ?? uid?.Value ?? localwebid?.CriteoId?.Value;
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
            if (localwebid.HasValue && localwebid.Value.CriteoId.HasValue)
                glup.LocalWebId = localwebid.Value.CriteoId.Value.ToString();
            if (uid.HasValue)
                glup.Uid = uid.Value.ToString();
            if (ifa.HasValue)
                glup.Ifa = ifa.Value.ToString();

            // Go!
            _glupService.Emit(glup);
        }

        #region Helpers
        private IAgent GetUserAgent(string userAgentString, Guid? uid)
        {
            var agentKey = new AgentKey { UserAgentHeader = userAgentString };
            var result = _agentSource.Get(agentKey, uid);
            return result.Agent;
        }
        #endregion
    }
}
