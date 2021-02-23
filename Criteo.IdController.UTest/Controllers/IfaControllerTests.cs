using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Criteo.IdController.Controllers;
using Criteo.IdController.Helpers;
using Metrics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Criteo.IdController.UTest.Controllers
{
    [TestFixture]
    public class IfaControllerTests
    {
        private Mock<IIdentifierGeneratorHelper> _identifierGeneratorHelperMock;
        private Mock<IMetricsRegistry> _metricRegistryMock;

        [SetUp]
        public void Setup()
        {
            _identifierGeneratorHelperMock = new Mock<IIdentifierGeneratorHelper>();
            _metricRegistryMock = new Mock<IMetricsRegistry>();
            _metricRegistryMock.Setup(mr => mr.GetOrRegister(It.IsAny<string>(), It.IsAny<Func<Counter>>())).Returns(new Counter(Granularity.CoarseGrain));
        }

        [Test]
        public void TestCreateIfaWithoutPii()
        {
            var ifaController = GetIfaController();

            var response = ifaController.GetOrCreateIfa();

            // Returned IFA
            var data = GetResponseData(response);
            Assert.NotNull(data);

            // IFA in cookie
            var cookie = GetSetCookieHeaderIfa(ifaController.HttpContext.Response);
            Assert.NotNull(cookie);
        }

        [Test]
        public void TestGetIfaFromCookie()
        {
            var ifaUserSide = "62BB805D-9E63-4FE0-AB7D-4B514F86D63C";

            var cookies = new Dictionary<string, string> { { "ifa", ifaUserSide } };
            var ifaController = GetIfaController(cookies);

            var response = ifaController.GetOrCreateIfa();

            // Returned IFA
            var data = GetResponseData(response);
            Assert.NotNull(data);
            StringAssert.Contains(ifaUserSide, data);

            // IFA in cookie
            var cookie = GetSetCookieHeaderIfa(ifaController.HttpContext.Response);
            Assert.NotNull(cookie);
            StringAssert.Contains(ifaUserSide, cookie.Value.ToString());
        }

        [Test]
        public void TestDeleteIfa()
        {
            var ifaUserSide = "62BB805D-9E63-4FE0-AB7D-4B514F86D63C";

            var cookies = new Dictionary<string, string> { { "ifa", ifaUserSide } };
            var ifaController = GetIfaController(cookies);

            var response = ifaController.DeleteIfa();

            // Returned IFA
            var data = GetResponseData(response);
            Assert.IsNull(data);

            // IFA in cookie (set to an empty value and already expired)
            var cookie = GetSetCookieHeaderIfa(ifaController.HttpContext.Response);
            Assert.IsNotNull(cookie);
            Assert.AreEqual(0, cookie.Value.Length);
        }

        #region Helpers
        private IfaController GetIfaController(Dictionary<string, string> cookies = null)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Cookies = new RequestCookieCollection(cookies);

            var ifaController = new IfaController(_identifierGeneratorHelperMock.Object, _metricRegistryMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };

            return ifaController;
        }

        private SetCookieHeaderValue GetSetCookieHeaderIfa(HttpResponse response)
        {
            var setCookies = response.GetTypedHeaders().SetCookie;
            var cookie = setCookies?.FirstOrDefault(c => c.Name == "ifa");

            return cookie;
        }

        private string GetResponseData(IActionResult response)
        {
            var responseContent = response as OkObjectResult;
            var data = responseContent?.Value.ToString();

            return data;
        }
        #endregion
    }
}
