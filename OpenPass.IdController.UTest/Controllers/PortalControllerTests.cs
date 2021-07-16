using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OpenPass.IdController.Controllers;
using OpenPass.IdController.Helpers;

namespace OpenPass.IdController.UTest.Controllers
{
    internal class PortalControllerTests
    {
        private Mock<IMetricHelper> _metricHelperMock;
        private Mock<ICookieHelper> _cookieHelperMock;
        private PortalController _portalController;

        [SetUp]
        public void Setup()
        {
            _metricHelperMock = new Mock<IMetricHelper>();
            _metricHelperMock.Setup(mr => mr.SendCounterMetric(It.IsAny<string>()));
            _cookieHelperMock = new Mock<ICookieHelper>();

            _portalController =
                new PortalController(_metricHelperMock.Object, _cookieHelperMock.Object)
                {
                    ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
                };
        }

        [Test]
        public void TestOptout()
        {
            // Act
            var response = _portalController.OptOut();

            // Assert
            Assert.IsAssignableFrom<OkResult>(response);

            _cookieHelperMock.Verify(c => c.RemoveUid2AdvertisingCookie(
                It.IsAny<IResponseCookies>()), Times.Once);
            _cookieHelperMock.Verify(c => c.RemoveIdentifierForAdvertisingCookie(
                It.IsAny<IResponseCookies>()), Times.Once);
            _cookieHelperMock.Verify(c => c.SetOptoutCookie(
                It.IsAny<IResponseCookies>(),
                It.Is<string>(k => k == "1")), Times.Once);
        }

        [Test]
        public void TestOptin()
        {
            // Act
            var response = _portalController.OptIn();

            // Assert
            Assert.IsAssignableFrom<OkResult>(response);

            _cookieHelperMock.Verify(c => c.RemoveOptoutCookie(
                It.IsAny<IResponseCookies>()), Times.Once);
        }
    }
}
