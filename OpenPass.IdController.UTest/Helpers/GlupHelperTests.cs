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
            _glupHelper.EmitGlup(EventType.Unknown, "origin.com", "userAgent", It.IsAny<TrackingModel>());

            // Assert
            _glupServiceMock.Verify(g => g.Emit(It.IsAny<IdControllerGlup>()), Times.Once);
        }

        [TestCase(null, null, null, null)]
        [TestCase(_testingLwid, null, null, _testingLwid)]
        [TestCase(null, _testingUid, null, _testingUid)]
        [TestCase(null, null, _testingIfa, _testingIfa)]
        [TestCase(_testingLwid, _testingUid, null, _testingUid)]
        [TestCase(_testingLwid, null, _testingIfa, _testingIfa)]
        [TestCase(null, _testingUid, _testingIfa, _testingIfa)]
        [TestCase(_testingLwid, _testingUid, _testingIfa, _testingIfa)]
        public void UserAgentParsingReceivesExpectedUid(string lwidString, string uidString, string ifaString, string expectedUid)
        {
            // Arrange
            var lwid = LocalWebId.Parse(lwidString, "originHost.com");
            var uid = CriteoId.Parse(uidString);
            var ifa = UserCentricAdId.Parse(ifaString);

            //  Act
            _glupHelper.EmitGlup(EventType.Unknown, "originHost.com", "userAgent", It.IsAny<TrackingModel>(), lwid, uid, ifa);

            var parsedExpectedUid = !string.IsNullOrEmpty(expectedUid) ? Guid.Parse(expectedUid) : (Guid?) null;

            // Assert
            _agentSourceMock.Verify(
                x => x.Get(It.IsAny<AgentKey>(), It.Is<Guid?>(g => g.Equals(parsedExpectedUid))),
                Times.Once);
        }

        [Test]
        public void EmitGlup_TrackingModelIsPassed_ShouldGetEnumMemberValueCorrectly()
        {
            var trackingModel = new TrackingModel
            {
                View = View.Modal,
                Provider = Provider.Publisher,
                Variant = Variant.InSite,
                Session = Session.Authenticated
            };

            _glupHelper.EmitGlup(EventType.Unknown, string.Empty, string.Empty, trackingModel);

            // Assert
            _glupServiceMock.Verify(g => g.Emit(It.Is<IdControllerGlup>(
                x => x.Variant.Equals("in-site") &&
                    x.View.Equals("modal") &&
                    x.Provider.Equals("publisher") &&
                    x.Session.Equals("authenticated")))
                , Times.Once);
        }

        [Test]
        public void EmitGlup_TrackingModelIsNull_ShouldNotSetWidgetProperties()
        {

            _glupHelper.EmitGlup(EventType.Unknown, string.Empty, string.Empty, null);

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
