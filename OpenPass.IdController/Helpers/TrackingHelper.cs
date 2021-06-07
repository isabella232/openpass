using Criteo.UserIdentification;
using Criteo.UserIdentification.Services;

namespace OpenPass.IdController.Helpers
{
    public interface ITrackingHelper
    {
        IdentificationBundle? TryGetCtoBundleCookie(string ctoBundle);
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
    }
}
