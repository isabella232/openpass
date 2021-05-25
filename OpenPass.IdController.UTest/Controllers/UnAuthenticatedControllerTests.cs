using System;
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
        public void TestCreateIdentifier()
        {
            string placeholder;
            _cookieHelperMock.Setup(c => c.TryGetIdentifierCookie(It.IsAny<IRequestCookieCollection>(), out placeholder)).Returns(false);

            var response = _unauthenticatedController.CreateIfa(_testUserAgent, _request);

            // Returned identifier
            var data = GetResponseData(response);
            var token = (string) data.token;

            // Assert
            // Cookie is set
            _cookieHelperMock.Verify(c => c.SetIdentifierCookie(
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
        public void TestGetIdentifierFromCookie()
        {
            // Arrange
            var idUserSide = Guid.NewGuid().ToString();
            _cookieHelperMock.Setup(c => c.TryGetIdentifierCookie(It.IsAny<IRequestCookieCollection>(), out idUserSide)).Returns(true);

            // Act
            var response = _unauthenticatedController.CreateIfa(_testUserAgent, _request);

            // Returned IFA
            var data = GetResponseData(response);
            var token = data.token;

            // Assert
            Assert.AreEqual(idUserSide, token);

            // Cookie is set set
            _cookieHelperMock.Verify(c => c.SetIdentifierCookie(
                It.IsAny<IResponseCookies>(),
                It.Is<string>(k => k == idUserSide)), Times.Once);

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
        public void TestDeleteIfa()
        {
            // Arrange
            var response = _unauthenticatedController.DeleteIfa();

            // Act
            var data = GetResponseData(response);
            Assert.IsNull(data);

            // Assert
            _cookieHelperMock.Verify(c => c.RemoveIdentifierCookie(It.IsAny<IResponseCookies>()), Times.Once);
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
