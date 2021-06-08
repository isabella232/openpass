using System.Threading.Tasks;
using Criteo.UserIdentification;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using OpenPass.IdController.Helpers;
using OpenPass.IdController.Helpers.Adapters;
using OpenPass.IdController.Models.Tracking;
using static Criteo.Glup.IdController.Types;

namespace OpenPass.IdController.UTest.Helpers
{
    [TestFixture]
    public class IdentifierHelperTests
    {
        private Mock<IMetricHelper> _metricHelperMock;
        private Mock<IGlupHelper> _glupHelperMock;
        private Mock<ICookieHelper> _cookieHelperMock;
        private Mock<IIdentifierAdapter> _uid2AdapterMock;

        private IdentifierHelper _identifierHelper;

        [SetUp]
        public void Setup()
        {
            _metricHelperMock = new Mock<IMetricHelper>();
            _metricHelperMock.Setup(mh => mh.SendCounterMetric(It.IsAny<string>()));
            _cookieHelperMock = new Mock<ICookieHelper>();
            _glupHelperMock = new Mock<IGlupHelper>();
            _uid2AdapterMock = new Mock<IIdentifierAdapter>();

            _identifierHelper = new IdentifierHelper(
                _metricHelperMock.Object,
                _cookieHelperMock.Object,
                _glupHelperMock.Object,
                _uid2AdapterMock.Object);
        }

        [Test]
        public void GetOrCreateIfaToken_IfaTokenExists_TokensAreTheSame()
        {
            // Arrange
            var expectedIfaToken = "ifaToken";
            var originHost = "origin";
            _cookieHelperMock.Setup(x => x.TryGetIdentifierForAdvertisingCookie(It.IsAny<IRequestCookieCollection>(), out expectedIfaToken))
                .Returns(true);

            // Act
            var token = _identifierHelper.GetOrCreateIfaToken(
                It.IsAny<IRequestCookieCollection>(),
                It.IsAny<TrackingModel>(),
                It.IsAny<string>(),
                originHost,
                It.IsAny<string>());

            // Assert
            Assert.AreEqual(expectedIfaToken, token);
            _glupHelperMock.Verify(g => g.EmitGlup(
                    It.Is<EventType>(e => e == EventType.ReuseIfa),
                    It.Is<string>(h => h == originHost),
                    It.IsAny<string>(),
                    It.IsAny<TrackingModel>(),
                    It.IsAny<LocalWebId?>(),
                    It.IsAny<CriteoId?>(),
                    It.IsAny<UserCentricAdId?>()),
                Times.Once);
        }

        [Test]
        public void GetOrCreateIfaToken_IfaTokenDoesNotExists_NewTokenIsCreated()
        {
            // Arrange
            var expectedIfaToken = string.Empty;
            var originHost = "origin";
            _cookieHelperMock.Setup(x => x.TryGetIdentifierForAdvertisingCookie(It.IsAny<IRequestCookieCollection>(), out expectedIfaToken))
                .Returns(false);

            // Act
            var token = _identifierHelper.GetOrCreateIfaToken(
                It.IsAny<IRequestCookieCollection>(),
                It.IsAny<TrackingModel>(),
                It.IsAny<string>(),
                originHost,
                It.IsAny<string>());

            // Assert
            Assert.AreNotEqual(expectedIfaToken, token);
            _glupHelperMock.Verify(g => g.EmitGlup(
                    It.Is<EventType>(e => e == EventType.NewIfa),
                    It.Is<string>(h => h == originHost),
                    It.IsAny<string>(),
                    It.IsAny<TrackingModel>(),
                    It.IsAny<LocalWebId?>(),
                    It.IsAny<CriteoId?>(),
                    It.IsAny<UserCentricAdId?>()),
                Times.Once);
        }

        [TestCase("token", 1)]
        [TestCase("", 0)]
        [TestCase(null, 0)]
        public async Task TryGetUid2TokenAsync_AllDataPassed_ShouldWorkCorrectly(string expectedUid2Token, int timesCalled)
        {
            // Arrange
            var userAgent = "userAgent";
            var originHost = "originHost";
            var email = "test@test.com";
            _uid2AdapterMock.Setup(x => x.GetId(email)).ReturnsAsync(expectedUid2Token);

            // Act
            var uid2Token = await _identifierHelper.TryGetUid2TokenAsync(
                It.IsAny<IResponseCookies>(),

                    It.IsAny<TrackingModel>(),
                EventType.EmailValidated,
                originHost,
                userAgent,
                email,
                It.IsAny<string>());

            // Assert
            Assert.AreEqual(expectedUid2Token, uid2Token);
            _glupHelperMock.Verify(g => g.EmitGlup(
                It.Is<EventType>(e => e == EventType.EmailValidated),
                It.Is<string>(h => h == originHost),
                It.Is<string>(a => a == userAgent),
                It.IsAny<TrackingModel>(),
                It.IsAny<LocalWebId?>(),
                It.IsAny<CriteoId?>(),
                It.IsAny<UserCentricAdId?>()),
                Times.Once);

            _cookieHelperMock.Verify(x =>
                x.SetUid2AdvertisingCookie(It.IsAny<IResponseCookies>(), It.Is<string>(token => token == uid2Token)),
                Times.Exactly(timesCalled));
        }
    }
}
