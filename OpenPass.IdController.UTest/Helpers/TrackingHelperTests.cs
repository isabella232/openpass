using Criteo.UserIdentification;
using Criteo.UserIdentification.Services;
using Moq;
using NUnit.Framework;
using OpenPass.IdController.Helpers;

namespace OpenPass.IdController.UTest.Helpers
{
    [TestFixture]
    public class TrackingHelperTests
    {
        private Mock<IIdentificationBundleHelper> _identificationBundleHelperMock;

        private TrackingHelper _trackingHelper;

        [SetUp]
        public void Init()
        {
            _identificationBundleHelperMock = new Mock<IIdentificationBundleHelper>();
            _trackingHelper = new TrackingHelper(_identificationBundleHelperMock.Object);
        }

        [TestCase("")]
        [TestCase(null)]
        public void TryGetCtoBundleCookie_BundleEmpty_ShouldReturnNull(string ctoBundle)
        {
            // Arrange
            IdentificationBundle? parsedCtoBundle;

            // Act
            var identificationBundle = _trackingHelper.TryGetCtoBundleCookie(ctoBundle);

            // Assert
            Assert.IsNull(identificationBundle);

            _identificationBundleHelperMock.Verify(x => x.TryDecryptIdentificationBundle(ctoBundle, out parsedCtoBundle), Times.Never);
        }

        [Test]
        public void TryGetCtoBundleCookie_BundleExists_ShouldReturnIdentificationBundle()
        {
            // Arrange
            var ctoBundle = "ctoBundle";
            IdentificationBundle? parsedCtoBundle = new IdentificationBundle();
            _identificationBundleHelperMock.Setup(x => x.TryDecryptIdentificationBundle(ctoBundle, out parsedCtoBundle));

            // Act
            var identificationBundle = _trackingHelper.TryGetCtoBundleCookie(ctoBundle);

            // Assert
            Assert.IsNotNull(identificationBundle);
        }
    }
}
