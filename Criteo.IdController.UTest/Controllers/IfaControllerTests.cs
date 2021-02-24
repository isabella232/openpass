using System;
using Moq;
using NUnit.Framework;
using Criteo.IdController.Controllers;
using Criteo.IdController.Helpers;
using Metrics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Criteo.IdController.UTest.Controllers
{
    [TestFixture]
    public class IfaControllerTests
    {
        private Mock<IIdentifierGeneratorHelper> _identifierGeneratorHelperMock;
        private Mock<IMetricsRegistry> _metricRegistryMock;
        private Mock<ICookieHelper> _cookieHelperMock;
        private IfaController _ifaController;

        [SetUp]
        public void Setup()
        {
            _identifierGeneratorHelperMock = new Mock<IIdentifierGeneratorHelper>();
            _metricRegistryMock = new Mock<IMetricsRegistry>();
            _metricRegistryMock.Setup(mr => mr.GetOrRegister(It.IsAny<string>(), It.IsAny<Func<Counter>>())).Returns(new Counter(Granularity.CoarseGrain));
            _cookieHelperMock = new Mock<ICookieHelper>();

            _ifaController = new IfaController(_identifierGeneratorHelperMock.Object, _metricRegistryMock.Object, _cookieHelperMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
        }

        [Test]
        public void TestCreateIdentifier()
        {
            var identifier = Guid.NewGuid();
            string placeholder;
            _cookieHelperMock.Setup(c => c.TryGetIdentifierCookie(It.IsAny<IRequestCookieCollection>(), out placeholder)).Returns(false);
            _identifierGeneratorHelperMock.Setup(i => i.GenerateIdentifier()).Returns(identifier);

            var response = _ifaController.GetOrCreateIfa();

            // Returned identifier
            var data = GetResponseData(response);
            var token = (string) data.token;
            Assert.AreEqual(identifier.ToString(), token);

            // Identifier generated
            _identifierGeneratorHelperMock.Verify(i => i.GenerateIdentifier(), Times.Once);

            // Cookie is set set
            _cookieHelperMock.Verify(c => c.SetIdentifierCookie(
                It.IsAny<IResponseCookies>(),
                It.Is<string>(k => k == token)), Times.Once);
        }

        [Test]
        public void TestGetIdentifierFromCookie()
        {
            var idUserSide = Guid.NewGuid().ToString();
            _cookieHelperMock.Setup(c => c.TryGetIdentifierCookie(It.IsAny<IRequestCookieCollection>(), out idUserSide)).Returns(true);

            var response = _ifaController.GetOrCreateIfa();

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
