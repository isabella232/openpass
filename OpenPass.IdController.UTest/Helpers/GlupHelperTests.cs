using System;
using Criteo.Services.Glup;
using Criteo.UserAgent;
using Criteo.UserIdentification;
using Moq;
using NUnit.Framework;
using OpenPass.IdController.Helpers;
using OpenPass.IdController.Models.Tracking;
using static Criteo.Glup.IdController.Types;
using IdControllerGlup = Criteo.Glup.IdController;

namespace OpenPass.IdController.UTest.Helpers
{
    [TestFixture]
    public class GlupHelperTests
    {
        private Mock<IGlupService> _glupServiceMock;
        private Mock<IAgentSource> _agentSourceMock;
        private GlupHelper _glupHelper;

        private const string _testingLwid = "00000000-0000-0000-0000-000000000001";
        private const string _testingUid = "00000000-0000-0000-0000-000000000002";
        private const string _testingIfa = "00000000-0000-0000-0000-000000000003";

        [SetUp]
        public void Init()
        {
            _glupServiceMock = new Mock<IGlupService>();
            _agentSourceMock = new Mock<IAgentSource>();
            _glupHelper = new GlupHelper(_glupServiceMock.Object, _agentSourceMock.Object);
        }

        [Test]
        public void TestGlupIsEmitted()
        {
            // Arrange && Act
            _glupHelper.EmitGlup("origin.com", "userAgent", new TrackingContext());

            // Assert
            _glupServiceMock.Verify(g => g.Emit(It.IsAny<IdControllerGlup>()), Times.Once);
        }

        [TestCase(null, null)]
        [TestCase(_testingLwid, _testingLwid)]
        public void UserAgentParsingReceivesExpectedUid(string lwidString, string expectedUid)
        {
            // Arrange
            var lwid = LocalWebId.Parse(lwidString, "originHost.com");

            var trackingContext = new TrackingContext
            {
                EventType = EventType.Unknown,
                LocalWebId = lwid
            };
            //  Act
            _glupHelper.EmitGlup("originHost.com", "userAgent", trackingContext);

            var parsedExpectedUid = !string.IsNullOrEmpty(expectedUid) ? Guid.Parse(expectedUid) : (Guid?) null;

            // Assert
            _agentSourceMock.Verify(
                x => x.Get(It.IsAny<AgentKey>(), It.Is<Guid?>(g => g.Equals(parsedExpectedUid))),
                Times.Once);
        }

        [Test]
        public void EmitGlup_TrackingContextIsEmpty_ShouldNotSetWidgetProperties()
        {
            _glupHelper.EmitGlup(string.Empty, string.Empty, new TrackingContext());

            // Assert
            _glupServiceMock.Verify(g => g.Emit(It.Is<IdControllerGlup>(
                x => string.IsNullOrEmpty(x.Variant) &&
                    string.IsNullOrEmpty(x.View) &&
                    string.IsNullOrEmpty(x.Provider) &&
                    string.IsNullOrEmpty(x.Session)))
                , Times.Once);
        }
    }
}
