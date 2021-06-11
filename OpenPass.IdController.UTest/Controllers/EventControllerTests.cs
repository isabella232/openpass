using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using OpenPass.IdController.Controllers;
using OpenPass.IdController.Helpers;
using OpenPass.IdController.Models;
using OpenPass.IdController.Models.Tracking;
using static Criteo.Glup.IdController.Types;

namespace OpenPass.IdController.UTest.Controllers
{
    [TestFixture]
    public class EventControllerTests
    {
        private EventController _eventController;
        private Mock<IConfigurationHelper> _configurationHelperMock;
        private Mock<IGlupHelper> _glupHelperMock;
        private Mock<IMetricHelper> _metricHelperMock;
        private Mock<ITrackingHelper> _trackingHelperMock;

        private const string _testUserAgent = "TestUserAgent";

        [SetUp]
        public void Setup()
        {
            _configurationHelperMock = new Mock<IConfigurationHelper>();
            _configurationHelperMock.Setup(x => x.EmitGlupsRatio(It.IsAny<string>())).Returns(1.0); // activate glupping by default
            _glupHelperMock = new Mock<IGlupHelper>();
            _metricHelperMock = new Mock<IMetricHelper>();
            _trackingHelperMock = new Mock<ITrackingHelper>();
            _metricHelperMock.Setup(mr => mr.SendCounterMetric(It.IsAny<string>()));
            
            _eventController = new EventController(
                _configurationHelperMock.Object,
                _metricHelperMock.Object,
                _glupHelperMock.Object,
                _trackingHelperMock.Object);
        }

        [Test]
        public async Task GlupEmittedWhenRequiredParametersArePresent()
        {
            // Arrange
            var originHost = "originHost.com";
            var eventType = EventType.BannerRequest;
            var request = new EventRequest
            {
                EventType = eventType
            };
            // Act
            await _eventController.SaveEvent(_testUserAgent, originHost, It.IsAny<string>(), request);

            // Assert
            _glupHelperMock.Verify(
                x => x.EmitGlup(
                    It.Is<string>(o => o == originHost),
                    It.IsAny<string>(),
                    It.IsAny<TrackingContext>()),
                Times.Once);
        }

        [Test]
        public async Task GlupNotEmittedWhenRequestIsNull()
        {
            // Arrange & Act
            await _eventController.SaveEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null);

            // Assert
            _glupHelperMock.Verify(
                x => x.EmitGlup(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<TrackingContext>()),
                Times.Never);
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
            await _eventController.SaveEvent(_testUserAgent, It.IsAny<string>(), originHost, request);

            // Assert
            _glupHelperMock.Verify(
                x => x.EmitGlup(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<TrackingContext>()),
                Times.Never);
        }

        [TestCase(0.0, 0)]
        [TestCase(1.0, 1)]
        public async Task GlupEmissionAppliesRatio(double ratio, int expectGlupCount)
        {
            // Arrange
            // use edge cases to test ratio (regardless of the value of the randomly generated value)
            _configurationHelperMock.Setup(x => x.EmitGlupsRatio(It.IsAny<string>())).Returns(ratio);

            var request = new EventRequest
            {
                EventType = EventType.BannerRequest
            };
            // Act
            await _eventController.SaveEvent(_testUserAgent, "originHost.com", It.IsAny<string>(), request);

            // Assert
            _glupHelperMock.Verify(
                x => x.EmitGlup(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<TrackingContext>()),
                Times.Exactly(expectGlupCount));
        }
    }
}
