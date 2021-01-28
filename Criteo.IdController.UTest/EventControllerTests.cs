using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Criteo.IdController.Controllers;
using Criteo.IdController.Helpers;
using Criteo.UserAgent;
using Criteo.UserIdentification;
using Metrics;
using static Criteo.Glup.IdController.Types;
using CriteoId = Criteo.UserIdentification.CriteoId;

namespace Criteo.IdController.UTest
{
    [TestFixture]
    public class EventControllerTests
    {
        private EventController _eventController;
        private Mock<IConfigurationHelper> _configurationHelperMock;
        private Mock<IGlupHelper> _glupHelperMock;
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
            _glupHelperMock = new Mock<IGlupHelper>();
            _metricRegistryMock = new Mock<IMetricsRegistry>();
            _metricRegistryMock.Setup(mr => mr.GetOrRegister(It.IsAny<string>(), It.IsAny<Func<Counter>>())).Returns(new Counter(Granularity.CoarseGrain));
            _internalMappingHelperMock = new Mock<IInternalMappingHelper>();
            _internalMappingHelperMock.Setup(x => x.GetInternalCriteoId(It.IsAny<CriteoId?>())).ReturnsAsync((CriteoId? criteoId) => criteoId);
            _internalMappingHelperMock.Setup(x => x.GetInternalLocalWebId(It.IsAny<LocalWebId?>())).ReturnsAsync((LocalWebId? lwid) => lwid);
            _internalMappingHelperMock.Setup(x => x.GetInternalUserCentricAdId(It.IsAny<UserCentricAdId?>())).ReturnsAsync((UserCentricAdId? ucaid) => ucaid);

            _eventController = new EventController(_configurationHelperMock.Object, _metricRegistryMock.Object, _internalMappingHelperMock.Object, _glupHelperMock.Object);
        }

        [TestCase(EventType.BannerRequest, "originHost.com")]
        public async Task GlupEmittedWhenRequiredParametersArePresent(EventType eventType, string originHost)
        {
            await _eventController.SaveEvent(eventType, originHost, null, null, null);

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
            await _eventController.SaveEvent(eventType, originHost, null, null, null);

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
            // use edge cases to test ratio (regardless of the value of the randomly generated value)
            _configurationHelperMock.Setup(x => x.EmitGlupsRatio(It.IsAny<string>())).Returns(ratio);

            await _eventController.SaveEvent(EventType.BannerRequest, "originHost.com", null, null, null);

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
