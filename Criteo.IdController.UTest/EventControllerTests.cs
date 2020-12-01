using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Criteo.IdController.Controllers;
using Criteo.IdController.Helpers;
using Criteo.Identification.Schemas;
using Criteo.Services.Glup;
using Criteo.UserAgent;
using Criteo.UserIdentification;
using Metrics;
using static Criteo.Glup.IdController.Types;
using IdControllerGlup = Criteo.Glup.IdController;
using CriteoId = Criteo.UserIdentification.CriteoId;

namespace Criteo.IdController.UTest
{
    [TestFixture]
    public class EventControllerTests
    {
        private EventController _eventController;
        private Mock<IConfigurationHelper> _configurationHelperMock;
        private Mock<IGlupService> _glupServiceMock;
        private Mock<IAgentSource> _agentSourceMock;
        private Mock<IMetricsRegistry> _metricRegistryMock;
        private Mock<IInternalMappingHelper> _internalMappingHelperMock;

        private const string _testingLwid = "00000000-0000-0000-0000-000000000001";
        private const string _testingUid = "00000000-0000-0000-0000-000000000002";
        private const string _testingIfa = "00000000-0000-0000-0000-000000000003";


        [SetUp]
        public void Setup()
        {
            _configurationHelperMock = new Mock<IConfigurationHelper>();
            _configurationHelperMock.Setup(x => x.EmitGlupsRatio(It.IsAny<string>())).Returns(1.0); // activate glupping by default
            _glupServiceMock = new Mock<IGlupService>();
            _agentSourceMock = new Mock<IAgentSource>();
            _metricRegistryMock = new Mock<IMetricsRegistry>();
            _metricRegistryMock.Setup(mr => mr.GetOrRegister(It.IsAny<string>(), It.IsAny<Func<Counter>>())).Returns(new Counter(Granularity.CoarseGrain));
            _internalMappingHelperMock = new Mock<IInternalMappingHelper>();
            _internalMappingHelperMock.Setup(x => x.GetInternalCriteoId(It.IsAny<CriteoId?>())).ReturnsAsync((CriteoId? criteoId) => criteoId);
            _internalMappingHelperMock.Setup(x => x.GetInternalLocalWebId(It.IsAny<LocalWebId?>())).ReturnsAsync((LocalWebId? lwid) => lwid);
            _internalMappingHelperMock.Setup(x => x.GetInternalUserCentricAdId(It.IsAny<UserCentricAdId?>())).ReturnsAsync((UserCentricAdId? ucaid) => ucaid);

            _eventController = new EventController(_configurationHelperMock.Object, _glupServiceMock.Object, _agentSourceMock.Object, _metricRegistryMock.Object, _internalMappingHelperMock.Object);
        }

        [TestCase(EventType.BannerRequest, "originHost.com")]
        public async Task GlupEmittedWhenRequiredParametersArePresent(EventType eventType, string originHost)
        {
            await _eventController.SaveEvent(eventType, originHost, null, null, null);

            _glupServiceMock.Verify(
                x => x.Emit(It.Is<IdControllerGlup>(g => (g.Event == eventType) && (g.OriginHost == originHost))),
                Times.Once);
        }

        [TestCase(EventType.Unknown, "originHost.com")]
        [TestCase(EventType.Unknown, null)]
        [TestCase(EventType.BannerRequest, null)]
        public async Task GlupNotEmittedWhenRequiredParametersAreNotPresent(EventType eventType, string originHost)
        {
            await _eventController.SaveEvent(eventType, originHost, null, null, null);

            _glupServiceMock.Verify(
                x => x.Emit(It.IsAny<IdControllerGlup>()),
                Times.Never);
        }

        [TestCase(0.0, false)]
        [TestCase(1.0, true)]
        public async Task GlupEmissionAppliesRatio(double ratio, bool expectGlup)
        {
            // use edge cases to test ratio (regardless of the value of the randomly generated value)
            _configurationHelperMock.Setup(x => x.EmitGlupsRatio(It.IsAny<string>())).Returns(ratio);

            await _eventController.SaveEvent(EventType.BannerRequest, "originHost.com", null, null, null);

            _glupServiceMock.Verify(
                x => x.Emit(It.IsAny<IdControllerGlup>()),
                expectGlup ? Times.Once() : Times.Never());
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
        public async Task UserAgentParsingReceivesExpectedUid(string localwebid, string uid, string ifa, string expectedUid)
        {
            // This test also checks that the Guid is properly parsed if exists
            await _eventController.SaveEvent(EventType.BannerRequest, "originHost.com", localwebid, uid, ifa);

            var parsedExpectedUid = !string.IsNullOrEmpty(expectedUid) ? Guid.Parse(expectedUid) : (Guid?) null;
            _agentSourceMock.Verify(
                x => x.Get(It.IsAny<AgentKey>(), It.Is<Guid?>(g => g.Equals(parsedExpectedUid))),
                Times.Once);
        }

        [Theory]
        public async Task GlupSupportsRevocableId(bool revocable)
        {
            var host = "originHost.com";
            var expectedLwid = _testingLwid;
            var expectedUid = _testingUid;
            var expectedIfa = _testingIfa;

            if (revocable)
            {
                var revLwid = LocalWebId.CreateNew(host);
                var revCriteoId = CriteoId.New();
                var revIfa = UserCentricAdId.New();

                // Override fakes
                _internalMappingHelperMock.Setup(x => x.GetInternalLocalWebId(It.IsAny<LocalWebId?>())).ReturnsAsync((LocalWebId? lwid) => revLwid);
                _internalMappingHelperMock.Setup(x => x.GetInternalCriteoId(It.IsAny<CriteoId?>())).ReturnsAsync((CriteoId? criteoId) => revCriteoId);
                _internalMappingHelperMock.Setup(x => x.GetInternalUserCentricAdId(It.IsAny<UserCentricAdId?>())).ReturnsAsync((UserCentricAdId? ucaid) => revIfa);

                // Update expected uids
                expectedLwid = revLwid.CriteoId.ToString();
                expectedUid = revCriteoId.Value.ToString();
                expectedIfa = revIfa.Value.ToString();
            }

            await _eventController.SaveEvent(EventType.BannerRequest, host, _testingLwid, _testingUid, _testingIfa);

            _glupServiceMock.Verify(
                x => x.Emit(It.Is<IdControllerGlup>(g => (g.LocalWebId == expectedLwid) && (g.Uid == expectedUid) && (g.Ifa == expectedIfa))),
                Times.Once);
        }
    }
}
