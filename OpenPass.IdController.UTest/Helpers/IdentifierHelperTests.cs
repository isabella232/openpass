using Criteo.UserIdentification;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using OpenPass.IdController.Helpers;
using static Criteo.Glup.IdController.Types;

namespace OpenPass.IdController.UTest.Helpers
{
    [TestFixture]
    public class IdentifierHelperTests
    {
        private Mock<IMetricHelper> _metricHelperMock;
        private Mock<IGlupHelper> _glupHelperMock;
        private Mock<ICookieHelper> _cookieHelperMock;

        private IdentifierHelper _identifierHelper;

        [SetUp]
        public void Setup()
        {
            _metricHelperMock = new Mock<IMetricHelper>();
            _metricHelperMock.Setup(mh => mh.SendCounterMetric(It.IsAny<string>()));
            _cookieHelperMock = new Mock<ICookieHelper>();
            _glupHelperMock = new Mock<IGlupHelper>();

            _identifierHelper = new IdentifierHelper(
                _metricHelperMock.Object,
                _cookieHelperMock.Object,
                _glupHelperMock.Object);
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
            var token = _identifierHelper.GetOrCreateIfaToken(It.IsAny<IRequestCookieCollection>(), It.IsAny<string>(), originHost, It.IsAny<string>());

            // Assert
            Assert.AreEqual(expectedIfaToken, token);
            _glupHelperMock.Verify(g => g.EmitGlup(
                    It.Is<EventType>(e => e == EventType.ReuseIfa),
                    It.Is<string>(h => h == originHost),
                    It.IsAny<string>(),
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
            var token = _identifierHelper.GetOrCreateIfaToken(It.IsAny<IRequestCookieCollection>(), It.IsAny<string>(), originHost, It.IsAny<string>());

            // Assert
            Assert.AreNotEqual(expectedIfaToken, token);
            _glupHelperMock.Verify(g => g.EmitGlup(
                    It.Is<EventType>(e => e == EventType.NewIfa),
                    It.Is<string>(h => h == originHost),
                    It.IsAny<string>(),
                    It.IsAny<LocalWebId?>(),
                    It.IsAny<CriteoId?>(),
                    It.IsAny<UserCentricAdId?>()),
                Times.Once);
        }
    }
}
