using Criteo.UserIdentification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OpenPass.IdController.Controllers;
using OpenPass.IdController.Helpers;
using OpenPass.IdController.Models;
using static Criteo.Glup.IdController.Types;


namespace OpenPass.IdController.UTest.Controllers
{
    [TestFixture]
    public class UnAuthenticatedControllerTests
    {
        private const string _testUserAgent = "TestUserAgent";
        private const string _testOriginHost = "origin.host";

        private Mock<IMetricHelper> _metricHelperMock;
        private Mock<ICookieHelper> _cookieHelperMock;
        private Mock<IGlupHelper> _glupHelperMock;
        private UnAuthenticatedController _unauthenticatedController;
        private GenerateRequest _request;

        [SetUp]
        public void Setup()
        {
            _metricHelperMock = new Mock<IMetricHelper>();
            _metricHelperMock.Setup(mr => mr.SendCounterMetric(It.IsAny<string>()));
            _cookieHelperMock = new Mock<ICookieHelper>();
            _glupHelperMock = new Mock<IGlupHelper>();
            _request = new GenerateRequest { OriginHost = _testOriginHost };

            _unauthenticatedController = new UnAuthenticatedController(_metricHelperMock.Object, _cookieHelperMock.Object, _glupHelperMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
        }

        [Test]
        public void CreateIfa_OldCookieExists_RemoveAndCreateNewCookie()
        {
            // Arrange
            string placeholder = "uid2cookie";
            _cookieHelperMock.Setup(c => c.TryGetUid2AdvertisingCookie(It.IsAny<IRequestCookieCollection>(), out placeholder)).Returns(true);

            // Act
            var response = _unauthenticatedController.CreateIfa(_testUserAgent, _request);

            // Returned IFA
            var data = GetResponseData(response);
            var token = (string) data.token;
            var uid2Identifier = (string) data.uid2Identifier;

            // Assert
            Assert.IsNotNull(token);
            Assert.IsNotNull(uid2Identifier);

            // Cookie is set set
            _cookieHelperMock.Verify(c => c.SetIdentifierForAdvertisingCookie(
                It.IsAny<IResponseCookies>(),
                It.Is<string>(k => k == token)), Times.Once);

            // Glup is emitted
            _glupHelperMock.Verify(g => g.EmitGlup(
                    It.Is<EventType>(e => e == EventType.NewIfa),
                    It.Is<string>(h => h == _testOriginHost),
                    It.IsAny<string>(),
                    It.IsAny<LocalWebId?>(),
                    It.IsAny<CriteoId?>(),
                    It.IsAny<UserCentricAdId?>()),
                Times.Once);
        }

        [Test]
        public void CreateIfa_NoOldCookiesExist_CreateNewGuidIdentifierCookie()
        {
            // Arrange
            var newCookie = "newcookie";
            string placeholder;
            _cookieHelperMock.Setup(c => c.TryGetUid2AdvertisingCookie(It.IsAny<IRequestCookieCollection>(), out placeholder)).Returns(false);
            _cookieHelperMock.Setup(c => c.TryGetIdentifierForAdvertisingCookie(It.IsAny<IRequestCookieCollection>(), out newCookie)).Returns(true);

            // Act
            var response = _unauthenticatedController.CreateIfa(_testUserAgent, _request);

            // Returned identifier
            var data = GetResponseData(response);
            var token = (string) data.token;
            var uid2Identifier = (string) data.uid2Identifier;

            // Assert
            Assert.IsNotNull(token);
            Assert.IsNull(uid2Identifier);

            // Cookie is set
            _cookieHelperMock.Verify(c => c.SetIdentifierForAdvertisingCookie(
                It.IsAny<IResponseCookies>(),
                It.Is<string>(k => k == token)), Times.Once);

            // Glup is emitted
            _glupHelperMock.Verify(g => g.EmitGlup(
                    It.Is<EventType>(e => e == EventType.ReuseIfa),
                    It.Is<string>(h => h == _testOriginHost),
                    It.IsAny<string>(),
                    It.IsAny<LocalWebId?>(),
                    It.IsAny<CriteoId?>(),
                    It.IsAny<UserCentricAdId?>()),
                Times.Once);
        }

        [Test]
        public void CreateIfa_NoCookiesExist_CreateNewGuidIdentifierCookie()
        {
            // Arrange
            string placeholder;
            _cookieHelperMock.Setup(c => c.TryGetUid2AdvertisingCookie(It.IsAny<IRequestCookieCollection>(), out placeholder)).Returns(false);
            _cookieHelperMock.Setup(c => c.TryGetIdentifierForAdvertisingCookie(It.IsAny<IRequestCookieCollection>(), out placeholder)).Returns(false);

            // Act
            var response = _unauthenticatedController.CreateIfa(_testUserAgent, _request);

            // Returned identifier
            var data = GetResponseData(response);
            var token = (string) data.token;
            var uid2Identifier = (string) data.uid2Identifier;

            // Assert
            Assert.IsNotNull(token);
            Assert.IsNull(uid2Identifier);

            // Cookie is set
            _cookieHelperMock.Verify(c => c.SetIdentifierForAdvertisingCookie(
                It.IsAny<IResponseCookies>(),
                It.Is<string>(k => k == token)), Times.Once);

            // Glup is emitted
            _glupHelperMock.Verify(g => g.EmitGlup(
                    It.Is<EventType>(e => e == EventType.NewIfa),
                    It.Is<string>(h => h == _testOriginHost),
                    It.IsAny<string>(),
                    It.IsAny<LocalWebId?>(),
                    It.IsAny<CriteoId?>(),
                    It.IsAny<UserCentricAdId?>()),
                Times.Once);
        }

        [Test]
        public void TestDeleteIfa()
        {
            // Arrange
            var response = _unauthenticatedController.DeleteIfa();

            // Act
            var data = GetResponseData(response);
            Assert.IsNull(data);

            // Assert
            _cookieHelperMock.Verify(c => c.RemoveUid2AdvertisingCookie(It.IsAny<IResponseCookies>()), Times.Once);
        }

        #region Helpers

        private static dynamic GetResponseData(IActionResult response)
        {
            var responseContent = response as OkObjectResult;
            var data = responseContent?.Value;

            return data;
        }

        #endregion Helpers
    }
}
