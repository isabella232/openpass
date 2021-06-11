using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Criteo.UserIdentification;
using Criteo.UserIdentification.Services;
using Newtonsoft.Json;
using OpenPass.IdController.Models.Tracking;
using static Criteo.Glup.IdController.Types;

namespace OpenPass.IdController.Helpers
{
    public interface ITrackingHelper
    {
        IdentificationBundle? TryGetCtoBundle(string ctoBundle);

        TrackingModel TryGetWidgetParameters(string trackedDataJson);

        Task<TrackingContext> BuildTrackingContextAsync(EventType eventType, string trackedData);

        Task<TrackingContext> BuildTrackingContextAsync(string trackedData);
    }

    public class TrackingHelper : ITrackingHelper
    {
        private readonly IIdentificationBundleHelper _identificationBundleHelper;
        private readonly IInternalMappingHelper _internalMappingHelper;

        public TrackingHelper(IIdentificationBundleHelper identificationBundleHelper, IInternalMappingHelper internalMappingHelper)
        {
            _identificationBundleHelper = identificationBundleHelper;
            _internalMappingHelper = internalMappingHelper;
        }

        public IdentificationBundle? TryGetCtoBundle(string ctoBundle)
        {
            IdentificationBundle? parsedCtoBundle = null;
            if (!string.IsNullOrEmpty(ctoBundle))
            {
                _identificationBundleHelper.TryDecryptIdentificationBundle(ctoBundle, out parsedCtoBundle);
            }

            return parsedCtoBundle;
        }

        public TrackingModel TryGetWidgetParameters(string trackedDataJson)
        {
            TrackingModel model = null;

            if (!string.IsNullOrEmpty(trackedDataJson))
            {
                model = JsonConvert.DeserializeObject<TrackingModel>(trackedDataJson);
            }

            return model;
        }

        public async Task<TrackingContext> BuildTrackingContextAsync(EventType eventType, string trackedData)
        {
            var context = await BuildTrackingContextAsync(trackedData);
            context.EventType = eventType;

            return context;
        }

        public async Task<TrackingContext> BuildTrackingContextAsync(string trackedData)
        {
            var widjetParameters = TryGetWidgetParameters(trackedData);

            var context = new TrackingContext
            {
                LocalWebId = await TryGetInternalLocalWebIdAsync(widjetParameters.CtoBundle),
                Ifa = widjetParameters.Ifa,
                Uid2 = widjetParameters.Uid2
            };

            SetWidgetParameters(widjetParameters, context);

            return context;
        }

        private async Task<LocalWebId?> TryGetInternalLocalWebIdAsync(string ctoBundle)
        {
            var identificationBundle = TryGetCtoBundle(ctoBundle);
            LocalWebId? internalLocalWebId = null;
            if (identificationBundle != null && identificationBundle.HasValue)
            {
                internalLocalWebId = await _internalMappingHelper.GetInternalLocalWebId(identificationBundle.Value.LocalWebId);
            }

            return internalLocalWebId;
        }

        private void SetWidgetParameters(TrackingModel model, TrackingContext trackingContext)
        {
            if (model != null && model.Provider.HasValue)
                trackingContext.Provider = GetEnumMemberAttrValue(typeof(Provider), model.Provider.Value);
            if (model != null && model.Session.HasValue)
                trackingContext.Session = GetEnumMemberAttrValue(typeof(Session), model.Session.Value);
            if (model != null && model.Variant.HasValue)
                trackingContext.Variant = GetEnumMemberAttrValue(typeof(Variant), model.Variant.Value);
            if (model != null && model.View.HasValue)
                trackingContext.View = GetEnumMemberAttrValue(typeof(View), model.View.Value);
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
    }
}
