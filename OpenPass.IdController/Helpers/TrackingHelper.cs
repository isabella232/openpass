using Criteo.UserIdentification;
using Criteo.UserIdentification.Services;
using Newtonsoft.Json;
using OpenPass.IdController.Models.Tracking;

namespace OpenPass.IdController.Helpers
{
    public interface ITrackingHelper
    {
        IdentificationBundle? TryGetCtoBundleCookie(string ctoBundle);

        TrackingModel TryGetWidgetParameters(string trackedDataJson);
    }

    public class TrackingHelper : ITrackingHelper
    {
        private readonly IIdentificationBundleHelper _identificationBundleHelper;

        public TrackingHelper(IIdentificationBundleHelper identificationBundleHelper)
        {
            _identificationBundleHelper = identificationBundleHelper;
        }

        public IdentificationBundle? TryGetCtoBundleCookie(string ctoBundle)
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
    }
}
