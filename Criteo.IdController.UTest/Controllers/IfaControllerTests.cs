using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Criteo.IdController.Controllers;
using Criteo.IdController.Helpers;
using Criteo.IdController.Helpers.Adapters;
using Metrics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Criteo.IdController.UTest.Controllers
{
    [TestFixture]
    public class IfaControllerTests
    {
        private Mock<IIdentifierAdapter> _uid2AdapterMock;
        private Mock<IMetricsRegistry> _metricRegistryMock;
        private Mock<ICookieHelper> _cookieHelperMock;
        private IfaController _ifaController;

        [SetUp]
        public void Setup()
        {
            _uid2AdapterMock = new Mock<IIdentifierAdapter>();
            _metricRegistryMock = new Mock<IMetricsRegistry>();
            _metricRegistryMock.Setup(mr => mr.GetOrRegister(It.IsAny<string>(), It.IsAny<Func<Counter>>())).Returns(new Counter(Granularity.CoarseGrain));
            _cookieHelperMock = new Mock<ICookieHelper>();

            _ifaController = new IfaController(_uid2AdapterMock.Object, _metricRegistryMock.Object, _cookieHelperMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
        }

        [Test]
        public async Task TestCreateIdentifier()
        {
            var returnedToken = "FreshUID2token";
            string placeholder;
            _cookieHelperMock.Setup(c => c.TryGetIdentifierCookie(It.IsAny<IRequestCookieCollection>(), out placeholder)).Returns(false);
            _uid2AdapterMock.Setup(c => c.GetId(It.IsAny<string>())).ReturnsAsync(returnedToken);

            var response = await _ifaController.GetOrCreateIfa();

            // Returned identifier
            var data = GetResponseData(response);
            var token = (string) data.token;
            Assert.AreEqual(returnedToken, token);

            // Identifier generated
            _uid2AdapterMock.Verify(a => a.GetId(It.IsAny<string>()), Times.Once);

            // Cookie is set set
            _cookieHelperMock.Verify(c => c.SetIdentifierCookie(
                It.IsAny<IResponseCookies>(),
                It.Is<string>(k => k == token)), Times.Once);
        }

        [Test]
        public async Task TestAdapterError()
        {
            const string returnedToken = null;
            string placeholder;
            _cookieHelperMock.Setup(c => c.TryGetIdentifierCookie(It.IsAny<IRequestCookieCollection>(), out placeholder)).Returns(false);
            _uid2AdapterMock.Setup(c => c.GetId(It.IsAny<string>())).ReturnsAsync(returnedToken);

            var response = await _ifaController.GetOrCreateIfa();

            // Not found -> adapter not available
            Assert.IsAssignableFrom<NotFoundResult>(response);

            // Cookie is set set
            _cookieHelperMock.Verify(c => c.SetIdentifierCookie(
                It.IsAny<IResponseCookies>(),
                It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task TestGetIdentifierFromCookie()
        {
            var idUserSide = Guid.NewGuid().ToString();
            _cookieHelperMock.Setup(c => c.TryGetIdentifierCookie(It.IsAny<IRequestCookieCollection>(), out idUserSide)).Returns(true);

            var response = await _ifaController.GetOrCreateIfa();

            // Returned IFA
            var data = GetResponseData(response);
            var token = data.token;
            Assert.AreEqual(idUserSide, token);

            // Cookie is set set
            _cookieHelperMock.Verify(c => c.SetIdentifierCookie(
                It.IsAny<IResponseCookies>(),
                It.Is<string>(k => k == idUserSide)), Times.Once);
        }

        [Test]
        public void TestDeleteIfa()
        {
            var response = _ifaController.DeleteIfa();

            // Returned IFA
            var data = GetResponseData(response);
            Assert.IsNull(data);

            // Cookie is removed
            _cookieHelperMock.Verify(c => c.RemoveIdentifierCookie(It.IsAny<IResponseCookies>()), Times.Once);
        }

        #region Helpers
        private static dynamic GetResponseData(IActionResult response)
        {
            var responseContent = response as OkObjectResult;
            var data = responseContent?.Value;

            return data;
        }
        #endregion
    }
}
