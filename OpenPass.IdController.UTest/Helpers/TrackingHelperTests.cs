using System;
using System.Threading.Tasks;
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

        [Test]
        public async Task BuildTrackingContextAsync_ParseLocalWebId_ShouldReturnValidLocalWebId()
        {
            // Arrange
            var ctoBundle = "string";
            LocalWebId? localWebId = LocalWebId.CreateNew("test@domain.com");

            IdentificationBundle? parsedCtoBundle = new IdentificationBundle
                (It.IsAny<DateTime>(), It.IsAny<DateTime>(), null, localWebId, null, IdentificationBundle.IdentificationBundleSource.Unknown, null, null);
            var json = $"{{\"uid2\":null,\"ifa\":null,\"ctoBundle\":\"{ctoBundle}\"}}";

            _identificationBundleHelperMock.Setup(x => x.TryDecryptIdentificationBundle(ctoBundle, out parsedCtoBundle)).Returns(true);
            _internalMappingHelperMock.Setup(x => x.GetInternalLocalWebId(localWebId)).ReturnsAsync(localWebId);

            // Act
            var context = await _trackingHelper.BuildTrackingContextAsync(json);

            // Assert
            Assert.AreEqual(context.LocalWebId, localWebId);
            Assert.IsNull(context.Uid2);
            Assert.IsNull(context.Ifa);
        }


        [Test]
        public async Task BuildTrackingContextAsync_ParseIfa_ShouldReturnValidIfa()
        {
            // Arrange
            var ifa = Guid.NewGuid().ToString();
            var ctoBundle = "string";
            var json = $"{{\"uid2\":null,\"ifa\":\"{ifa}\",\"ctoBundle\":\"{ctoBundle}\"}}";

            IdentificationBundle? parsedCtoBundle = null;

            _identificationBundleHelperMock.Setup(x => x.TryDecryptIdentificationBundle(ctoBundle, out parsedCtoBundle)).Returns(true);

            // Act
            var context = await _trackingHelper.BuildTrackingContextAsync(json);

            // Assert
            Assert.IsNull(context.LocalWebId);
            Assert.IsNull(context.Uid2);
            Assert.AreEqual(ifa, context.Ifa);
        }

        [Test]
        public async Task BuildTrackingContextAsync_ParseUid2_ShouldReturnValidUid2()
        {
            // Arrange
            var uid2 = Guid.NewGuid().ToString();
            var ctoBundle = "string";
            var json = $"{{\"uid2\":\"{uid2}\",\"ifa\":null,\"ctoBundle\":\"{ctoBundle}\"}}";

            IdentificationBundle? parsedCtoBundle = null;

            _identificationBundleHelperMock.Setup(x => x.TryDecryptIdentificationBundle(ctoBundle, out parsedCtoBundle)).Returns(true);

            // Act
            var context = await _trackingHelper.BuildTrackingContextAsync(json);

            // Assert
            Assert.IsNull(context.LocalWebId);
            Assert.IsNull(context.Ifa);
            Assert.AreEqual(uid2, context.Uid2);
        }
    }
}
