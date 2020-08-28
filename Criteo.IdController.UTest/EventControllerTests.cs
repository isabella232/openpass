using System.Net;
using Moq;
using NUnit.Framework;
using Criteo.IdController.Controllers;
using Criteo.Services.Glup;
using Criteo.UserAgent;
using Microsoft.AspNetCore.Mvc;
using static Criteo.Glup.IdController.Types;
using IdControllerGlup = Criteo.Glup.IdController;

namespace Criteo.IdController.UTest
{
    [TestFixture]
    public class EventControllerTests
    {
        private EventController _eventController;
        private Mock<IGlupService> _glupServiceMock;
        private Mock<IAgentSource> _agentSourceMock;

        [SetUp]
        public void Setup()
        {
            _glupServiceMock = new Mock<IGlupService>();
            _agentSourceMock = new Mock<IAgentSource>();
            _eventController = new EventController(_glupServiceMock.Object, _agentSourceMock.Object);
        }

        [TestCase(EventType.BannerRequest, "originHost.com")]
        public void GlupEmittedWhenRequiredParametersArePresent(EventType eventType, string originHost)
        {
            _eventController.SaveEvent(eventType, originHost, null, null, null);

            _glupServiceMock.Verify(
                x => x.Emit(It.Is<IdControllerGlup>(g => (g.Event == eventType) && (g.OriginHost == originHost))),
                Times.Once);
        }

        [TestCase(EventType.Unknown, "originHost.com")]
        [TestCase(EventType.Unknown, null)]
        [TestCase(EventType.BannerRequest, null)]
        public void GlupNotEmittedWhenRequiredParametersAreNotPresent(EventType eventType, string originHost)
        {
            _eventController.SaveEvent(eventType, originHost, null, null, null);

            _glupServiceMock.Verify(
                x => x.Emit(It.IsAny<IdControllerGlup>()),
                Times.Never);
        }
    }
}
