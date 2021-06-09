using System;
using System.Linq;
using System.Runtime.Serialization;
using Criteo.Services.Glup;
using Criteo.UserAgent;
using Criteo.UserIdentification;
using OpenPass.IdController.Models.Tracking;
using static Criteo.Glup.IdController.Types;
using IdControllerGlup = Criteo.Glup.IdController;

namespace OpenPass.IdController.Helpers
{
    public interface IGlupHelper
    {
        void EmitGlup(
            EventType eventType,
            string originHost,
            string userAgentString,
            TrackingModel trackingModel,
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
            TrackingModel trackingModel,
            LocalWebId? localwebid = null,
            CriteoId? uid = null,
            UserCentricAdId? ifa = null)
        {
            // Create glup event with required fields
            var glup = new IdControllerGlup
            {
                Event = eventType,
                OriginHost = originHost ?? "" // must never be null
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

            SetWidgetParameters(trackingModel, glup);

            // Go!
            _glupService.Emit(glup);
        }

        #region Helpers
        private void SetWidgetParameters(TrackingModel model, IdControllerGlup glup)
        {
            if (model != null && model.Provider.HasValue)
                glup.Provider = GetEnumMemberAttrValue(typeof(Provider), model.Provider.Value);
            if (model != null && model.Session.HasValue)
                glup.Session = GetEnumMemberAttrValue(typeof(Session), model.Session.Value);
            if (model != null && model.Variant.HasValue)
                glup.Variant = GetEnumMemberAttrValue(typeof(Variant), model.Variant.Value);
            if (model != null && model.View.HasValue)
                glup.View = GetEnumMemberAttrValue(typeof(View), model.View.Value);
        }

        private string GetEnumMemberAttrValue(Type enumType, object enumVal)
        {
            var memInfo = enumType.GetMember(enumVal.ToString());
            var attr = memInfo[0].GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault();
            if (attr != null)
            {
                return attr.Value;
            }

            return null;
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
