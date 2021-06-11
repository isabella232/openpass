using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OpenPass.IdController.Controllers;
using OpenPass.IdController.Helpers;

namespace OpenPass.IdController.UTest.Controllers
{
    class PortalControllerTests
    {
        private const string _testUserAgent = "TestUserAgent";

        private Mock<IMetricHelper> _metricHelperMock;
        private Mock<ICookieHelper> _cookieHelperMock;
        private Mock<IGlupHelper> _glupHelperMock;
        private PortalController _portalController;
        private Mock<ITrackingHelper> _trackingHelperMock;

        [SetUp]
        public void Setup()
        {
            _metricHelperMock = new Mock<IMetricHelper>();
            _metricHelperMock.Setup(mr => mr.SendCounterMetric(It.IsAny<string>()));
            _cookieHelperMock = new Mock<ICookieHelper>();
            _glupHelperMock = new Mock<IGlupHelper>();
            _trackingHelperMock = new Mock<ITrackingHelper>();

            _portalController =
                new PortalController(_metricHelperMock.Object, _cookieHelperMock.Object, _glupHelperMock.Object, _trackingHelperMock.Object)
                {
                    ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
                };
        }

        [Test]
        public async Task TestOptout()
        {
            // Act
            var response = await _portalController.OptOut(_testUserAgent, It.IsAny<string>(), It.IsAny<string>());

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
        public async Task TestOptin()
        {
            // Act
            var response = await _portalController.OptIn(_testUserAgent, It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.IsAssignableFrom<OkResult>(response);

            _cookieHelperMock.Verify(c => c.RemoveOptoutCookie(
                It.IsAny<IResponseCookies>()), Times.Once);
        }
    }
}
