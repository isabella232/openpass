using System;
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

        private const string _testingLwid = "00000000-0000-0000-0000-000000000001";
        private const string _testingUid = "00000000-0000-0000-0000-000000000002";
        private const string _testingIfa = "00000000-0000-0000-0000-000000000003";


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

        [TestCase(null, null, null, null)]
        [TestCase("invalidUid", null, null, null)]
        [TestCase(_testingLwid, null, null, _testingLwid)]
        [TestCase(null, _testingUid, null, _testingUid)]
        [TestCase(null, null, _testingIfa, _testingIfa)]
        [TestCase(_testingLwid, _testingUid, null, _testingUid)]
        [TestCase(_testingLwid, null, _testingIfa, _testingIfa)]
        [TestCase(null, _testingUid, _testingIfa, _testingIfa)]
        [TestCase(_testingLwid, _testingUid, _testingIfa, _testingIfa)]
        public void UserAgentParsingReceivesExpectedUid(string localwebid, string uid, string ifa, string expectedUid)
        {
            // This test also checks that the Guid is properly parsed if exists
            _eventController.SaveEvent(EventType.BannerRequest, "originHost.com", localwebid, uid, ifa);

            var parsedExpectedUid = !string.IsNullOrEmpty(expectedUid) ? Guid.Parse(expectedUid) : (Guid?) null;
            _agentSourceMock.Verify(
                x => x.Get(It.IsAny<AgentKey>(), It.Is<Guid?>(g => g.Equals(parsedExpectedUid))),
                Times.Once);
        }
    }
}
