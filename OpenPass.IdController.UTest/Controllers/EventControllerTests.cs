using System.Threading.Tasks;
using Criteo.UserIdentification;
using Moq;
using NUnit.Framework;
using OpenPass.IdController.Controllers;
using OpenPass.IdController.Helpers;
using OpenPass.IdController.Models;
using static Criteo.Glup.IdController.Types;
using CriteoId = Criteo.UserIdentification.CriteoId;

namespace OpenPass.IdController.UTest.Controllers
{
    [TestFixture]
    public class EventControllerTests
    {
        private EventController _eventController;
        private Mock<IConfigurationHelper> _configurationHelperMock;
        private Mock<IGlupHelper> _glupHelperMock;
        private Mock<IMetricHelper> _metricHelperMock;
        private Mock<IInternalMappingHelper> _internalMappingHelperMock;

        private const string _testUserAgent = "TestUserAgent";
        private const string _testingLwid = "00000000-0000-0000-0000-000000000001";
        private const string _testingUid = "00000000-0000-0000-0000-000000000002";
        private const string _testingIfa = "00000000-0000-0000-0000-000000000003";

        [SetUp]
        public void Setup()
        {
            _configurationHelperMock = new Mock<IConfigurationHelper>();
            _configurationHelperMock.Setup(x => x.EmitGlupsRatio(It.IsAny<string>())).Returns(1.0); // activate glupping by default
            _glupHelperMock = new Mock<IGlupHelper>();
            _metricHelperMock = new Mock<IMetricHelper>();
            _metricHelperMock.Setup(mr => mr.SendCounterMetric(It.IsAny<string>()));
            _internalMappingHelperMock = new Mock<IInternalMappingHelper>();
            _internalMappingHelperMock.Setup(x => x.GetInternalCriteoId(It.IsAny<CriteoId?>())).ReturnsAsync((CriteoId? criteoId) => criteoId);
            _internalMappingHelperMock.Setup(x => x.GetInternalLocalWebId(It.IsAny<LocalWebId?>())).ReturnsAsync((LocalWebId? lwid) => lwid);
            _internalMappingHelperMock.Setup(x => x.GetInternalUserCentricAdId(It.IsAny<UserCentricAdId?>())).ReturnsAsync((UserCentricAdId? ucaid) => ucaid);

            _eventController = new EventController(_configurationHelperMock.Object, _metricHelperMock.Object, _internalMappingHelperMock.Object, _glupHelperMock.Object);
        }

        [TestCase(EventType.BannerRequest, "originHost.com")]
        public async Task GlupEmittedWhenRequiredParametersArePresent(EventType eventType, string originHost)
        {
            // Arrange
            var request = new EventRequest
            {
                EventType = eventType
            };
            // Act
            await _eventController.SaveEvent(_testUserAgent, originHost, request);

            // Assert
            _glupHelperMock.Verify(
                x => x.EmitGlup(
                    It.Is<EventType>(e => e == eventType),
                    It.Is<string>(o => o == originHost),
                    It.IsAny<string>(),
                    It.IsAny<LocalWebId?>(),
                    It.IsAny<CriteoId?>(),
                    It.IsAny<UserCentricAdId?>()),
                Times.Once);
        }

        [TestCase(EventType.Unknown, "originHost.com")]
        [TestCase(EventType.Unknown, null)]
        [TestCase(EventType.BannerRequest, null)]
        public async Task GlupNotEmittedWhenRequiredParametersAreNotPresent(EventType eventType, string originHost)
        {
            // Arrange
            var request = new EventRequest
            {
                EventType = eventType
            };
            // Act
            await _eventController.SaveEvent(_testUserAgent, originHost, request);

            // Assert
            _glupHelperMock.Verify(
                x => x.EmitGlup(
                    It.IsAny<EventType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<LocalWebId?>(),
                    It.IsAny<CriteoId?>(),
                    It.IsAny<UserCentricAdId?>()),
                Times.Never);
        }

        [TestCase(0.0, false)]
        [TestCase(1.0, true)]
        public async Task GlupEmissionAppliesRatio(double ratio, bool expectGlup)
        {
            // Arrange
            // use edge cases to test ratio (regardless of the value of the randomly generated value)
            _configurationHelperMock.Setup(x => x.EmitGlupsRatio(It.IsAny<string>())).Returns(ratio);

            var request = new EventRequest
            {
                EventType = EventType.BannerRequest
            };
            // Act
            await _eventController.SaveEvent(_testUserAgent, "originHost.com", request);

            // Assert
            _glupHelperMock.Verify(
                x => x.EmitGlup(
                    It.IsAny<EventType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<LocalWebId?>(),
                    It.IsAny<CriteoId?>(),
                    It.IsAny<UserCentricAdId?>()),
                expectGlup ? Times.Once() : Times.Never());
        }

        [Test]
        public async Task GlupSupportsRevocableId()
        {
            // Arrange
            var host = "originHost.com";

            var revLwid = LocalWebId.CreateNew(host);
            var revCriteoId = CriteoId.New();
            var revIfa = UserCentricAdId.New();

            // Override fakes
            _internalMappingHelperMock.Setup(x => x.GetInternalLocalWebId(It.IsAny<LocalWebId?>())).ReturnsAsync((LocalWebId? lwid) => revLwid);
            _internalMappingHelperMock.Setup(x => x.GetInternalCriteoId(It.IsAny<CriteoId?>())).ReturnsAsync((CriteoId? criteoId) => revCriteoId);
            _internalMappingHelperMock.Setup(x => x.GetInternalUserCentricAdId(It.IsAny<UserCentricAdId?>())).ReturnsAsync((UserCentricAdId? ucaid) => revIfa);

            // Update expected uids
            var expectedLwid = revLwid.CriteoId.ToString();
            var expectedUid = revCriteoId.Value.ToString();
            var expectedIfa = revIfa.Value.ToString();

            var request = new EventRequest
            {
                EventType = EventType.BannerRequest,
                LocalWebId = _testingLwid,
                Uid = _testingUid,
                Ifa = _testingIfa
            };

            // Act
            await _eventController.SaveEvent(_testUserAgent, host, request);

            // Assert
            _glupHelperMock.Verify(
                x => x.EmitGlup(
                    It.IsAny<EventType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.Is<LocalWebId?>(lwid => lwid.Value.Id == expectedLwid),
                    It.Is<CriteoId?>(uid => uid.ToString() == expectedUid),
                    It.Is<UserCentricAdId?>(ifa => ifa.ToString() == expectedIfa)),
                Times.Once);
        }
    }
}
