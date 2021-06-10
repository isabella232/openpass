using Criteo.UserIdentification;
using Criteo.UserIdentification.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OpenPass.IdController.Helpers;
using OpenPass.IdController.Models.Tracking;

namespace OpenPass.IdController.UTest.Helpers
{
    [TestFixture]
    public class TrackingHelperTests
    {
        private Mock<IIdentificationBundleHelper> _identificationBundleHelperMock;
        private Mock<IInternalMappingHelper> _internalMappingHelperMock;

        private TrackingHelper _trackingHelper;

        [SetUp]
        public void Init()
        {
            _identificationBundleHelperMock = new Mock<IIdentificationBundleHelper>();
            _internalMappingHelperMock = new Mock<IInternalMappingHelper>();
            _trackingHelper = new TrackingHelper(_identificationBundleHelperMock.Object, _internalMappingHelperMock.Object);
        }

        [Test]
        public void TryGetWidgetParameters_JsonIsValid_ShouldDeserializeIntoModel()
        {
            // Arrange
            var json = "{\"view\":\"modal\",\"variant\":\"in-site\",\"session\":\"unauthenticated\",\"provider\":\"advertiser\"}";

            var expectedModel = new TrackingModel
            {
                Provider = Provider.Advertiser,
                Session = Session.Unauthenticated,
                Variant = Variant.InSite,
                View = View.Modal
            };

            // Act
            var actualData = _trackingHelper.TryGetWidgetParameters(json);

            // Assert
            actualData.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void TryGetWidgetParameters_JsonIsNotPassed_ShouldReturnNull()
        {
            // Arrange & Act
            var actualData = _trackingHelper.TryGetWidgetParameters(string.Empty);

            // Assert
            Assert.IsNull(actualData);
        }

        [TestCase("")]
        [TestCase(null)]
        public void TryGetCtoBundle_BundleEmpty_ShouldReturnNull(string ctoBundle)
        {
            // Arrange
            IdentificationBundle? parsedCtoBundle;

            // Act
            var identificationBundle = _trackingHelper.TryGetCtoBundle(ctoBundle);

            // Assert
            Assert.IsNull(identificationBundle);

            _identificationBundleHelperMock.Verify(x => x.TryDecryptIdentificationBundle(ctoBundle, out parsedCtoBundle), Times.Never);
        }

        [Test]
        public void TryGetCtoBundle_BundleExists_ShouldReturnIdentificationBundle()
        {
            // Arrange
            var ctoBundle = "ctoBundle";
            IdentificationBundle? parsedCtoBundle = new IdentificationBundle();
            _identificationBundleHelperMock.Setup(x => x.TryDecryptIdentificationBundle(ctoBundle, out parsedCtoBundle));

            // Act
            var identificationBundle = _trackingHelper.TryGetCtoBundle(ctoBundle);

            // Assert
            Assert.IsNotNull(identificationBundle);
        }
    }
}
