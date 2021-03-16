using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Criteo.IdController.Controllers;
using Criteo.IdController.Helpers;
using Criteo.IdController.Helpers.Adapters;
using Criteo.UserIdentification;
using Metrics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using static Criteo.Glup.IdController.Types;

namespace Criteo.IdController.UTest.Controllers
{
    [TestFixture]
    public class AuthenticatedControllerTests
    {
        private const string _testUserAgent = "TestUserAgent";

        private Mock<IHostingEnvironment> _hostingEnvironmentMock;
        private Mock<IMetricsRegistry> _metricRegistryMock;
        private Mock<IMemoryCache> _memoryCache;
        private Mock<IConfigurationHelper> _configurationHelperMock;
        private Mock<IIdentifierAdapter> _uid2AdapterMock;
        private Mock<IEmailHelper> _emailHelperMock;
        private Mock<IGlupHelper> _glupHelperMock;
        private Mock<ICodeGeneratorHelper> _codeGeneratorHelperMock;
        private Mock<ICookieHelper> _cookieHelperMock;

        private AuthenticatedController _authenticatedController;

        [SetUp]
        public void Setup()
        {
            _hostingEnvironmentMock = new Mock<IHostingEnvironment>();
            _configurationHelperMock = new Mock<IConfigurationHelper>();
            _configurationHelperMock.Setup(c => c.EnableOtp).Returns(true);
            _metricRegistryMock = new Mock<IMetricsRegistry>();
            _metricRegistryMock.Setup(mr => mr.GetOrRegister(It.IsAny<string>(), It.IsAny<Func<Counter>>())).Returns(new Counter(Granularity.CoarseGrain));
            _memoryCache = new Mock<IMemoryCache>();
            // We use the Set method but cannot mock it because it is an extension
            // then we mock the CreateEntry method that is the native one used under the hood
            var cachEntry = Mock.Of<ICacheEntry>();
            _memoryCache
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(cachEntry);
            _configurationHelperMock = new Mock<IConfigurationHelper>();
            _configurationHelperMock.Setup(c => c.EnableOtp).Returns(true);
            _emailHelperMock = new Mock<IEmailHelper>();
            _emailHelperMock.Setup(e => e.IsValidEmail(It.IsAny<string>())).Returns(true);
            _glupHelperMock = new Mock<IGlupHelper>();
            _codeGeneratorHelperMock = new Mock<ICodeGeneratorHelper>();
            _codeGeneratorHelperMock.Setup(c => c.IsValidCode(It.IsAny<string>())).Returns(true);
            _uid2AdapterMock = new Mock<IIdentifierAdapter>();
            _cookieHelperMock = new Mock<ICookieHelper>();

            _authenticatedController = new AuthenticatedController(_hostingEnvironmentMock.Object, _metricRegistryMock.Object,
                _memoryCache.Object, _configurationHelperMock.Object, _uid2AdapterMock.Object, _emailHelperMock.Object, _glupHelperMock.Object,
                _codeGeneratorHelperMock.Object, _cookieHelperMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
        }

        #region One-time password (OTP)
        [Test]
        public void ForbiddenWhenGenerationNotEnabled()
        {
            _configurationHelperMock.Setup(c => c.EnableOtp).Returns(false);
            var request = new AuthenticatedController.GenerateRequest() { Email = "example@mail.com" };
            var response = _authenticatedController.GenerateOtp(_testUserAgent, request);

            Assert.IsAssignableFrom<NotFoundResult>(response);
        }

        [Test]
        public async Task ForbiddenWhenValidationNotEnabled()
        {
            _configurationHelperMock.Setup(c => c.EnableOtp).Returns(false);
            var request = new AuthenticatedController.ValidateRequest() { Email = "example@mail.com", Otp = "123456" };
            var response = await _authenticatedController.ValidateOtp(_testUserAgent, request);

            Assert.IsAssignableFrom<NotFoundResult>(response);
        }

        [Test]
        public void BadRequestWhenGenerationEmailIsInvalid()
        {
            _emailHelperMock.Setup(e => e.IsValidEmail(It.IsAny<string>())).Returns(false);

            var request = new AuthenticatedController.GenerateRequest();
            var response = _authenticatedController.GenerateOtp(_testUserAgent, request);

            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Test]
        public async Task BadRequestWhenValidationEmailIsInvalid()
        {
            _emailHelperMock.Setup(e => e.IsValidEmail(It.IsAny<string>())).Returns(false);

            var request = new AuthenticatedController.ValidateRequest();
            var response = await _authenticatedController.ValidateOtp(_testUserAgent, request);

            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Test]
        public async Task BadRequestWhenValidationOtpIsInvalid()
        {
            _codeGeneratorHelperMock.Setup(c => c.IsValidCode(It.IsAny<string>())).Returns(false);

            var request = new AuthenticatedController.ValidateRequest();
            var response = await _authenticatedController.ValidateOtp(_testUserAgent, request);

            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Test]
        public async Task BadRequestWhenValidationEmailAndOtpAreInvalid()
        {
            _emailHelperMock.Setup(e => e.IsValidEmail(It.IsAny<string>())).Returns(false);
            _codeGeneratorHelperMock.Setup(c => c.IsValidCode(It.IsAny<string>())).Returns(false);

            var request = new AuthenticatedController.ValidateRequest();
            var response = await _authenticatedController.ValidateOtp(_testUserAgent, request);

            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Test]
        public void ValidRequestGeneration()
        {
            var request = new AuthenticatedController.GenerateRequest();
            var response = _authenticatedController.GenerateOtp(_testUserAgent, request);

            Assert.IsAssignableFrom<NoContentResult>(response);
            _codeGeneratorHelperMock.Verify(c => c.GenerateRandomCode(), Times.Once);
        }

        [Test]
        public async Task ValidRequestValidation()
        {
            object otp = "123456";
            var returnedToken = "FreshUID2token";
            _memoryCache
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out otp))
                .Returns(true);
            _uid2AdapterMock.Setup(c => c.GetId(It.IsAny<string>())).ReturnsAsync(returnedToken);
            var request = new AuthenticatedController.ValidateRequest() { Email = "example@mail.com", Otp = "123456" };

            var response = await _authenticatedController.ValidateOtp(_testUserAgent, request);

            // token in JSON response
            Assert.IsAssignableFrom<OkObjectResult>(response);
            var responseData = GetResponseData(response);
            var token = (string) responseData.token;
            Assert.AreEqual(returnedToken, token);

            // token in cookie
            _cookieHelperMock.Verify(c => c.SetIdentifierCookie(
                It.IsAny<IResponseCookies>(),
                It.Is<string>(t => t == token)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("origin.com")]
        public void GenerationGlupEmitted(string originHost)
        {
            var request = new AuthenticatedController.GenerateRequest() { Email = "example@mail.com", OriginHost = originHost };
            _authenticatedController.GenerateOtp(_testUserAgent, request);

            _glupHelperMock.Verify(g => g.EmitGlup(
                    It.Is<EventType>(e => e == EventType.EmailEntered),
                    It.Is<string>(h => h == originHost),
                    It.IsAny<string>(),
                    It.IsAny<LocalWebId?>(),
                    It.IsAny<CriteoId?>(),
                    It.IsAny<UserCentricAdId?>()),
                Times.Once);
        }

        [TestCase(null)]
        [TestCase("origin.com")]
        public async Task ValidationGlupEmitted(string originHost)
        {
            object code = "123456";
            _memoryCache
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out code))
                .Returns(true);

            var request = new AuthenticatedController.ValidateRequest() { Email = "example@mail.com", Otp = (string) code, OriginHost = originHost };
            await _authenticatedController.ValidateOtp(_testUserAgent, request);

            _glupHelperMock.Verify(g => g.EmitGlup(
                It.Is<EventType>(e => e == EventType.EmailValidated),
                It.Is<string>(h => h == originHost),
                It.IsAny<string>(),
                It.IsAny<LocalWebId?>(),
                It.IsAny<CriteoId?>(),
                It.IsAny<UserCentricAdId?>()),
                Times.Once);
        }

        [Test]
        public void GenerateOTPAndAddToCache()
        {
            var email = "example@mail.com";
            var request = new AuthenticatedController.GenerateRequest() { Email = email };
            _authenticatedController.GenerateOtp(_testUserAgent, request);
            _memoryCache.Verify(m => m.CreateEntry(It.Is<string>(s => s == email)), Times.Once);
        }

        [Test]
        public void GenerateOTPAndSendEmail()
        {
            var email = "example@mail.com";
            var code = "123456";

            _codeGeneratorHelperMock.Setup(c => c.GenerateRandomCode()).Returns(code);

            var request = new AuthenticatedController.GenerateRequest() { Email = email };
            _authenticatedController.GenerateOtp(_testUserAgent, request);
            _emailHelperMock.Verify(e => e.SendOtpEmail(It.Is<string>(s => s == email), It.Is<string>(c => c == code)), Times.Once);
        }

        [Test]
        public async Task OTPSuccessfulFullValidation()
        {
            var email = "example@mail.com";
            var code = "123456";
            var returnedToken = "FreshUID2token";

            _codeGeneratorHelperMock.Setup(c => c.GenerateRandomCode()).Returns(code);
            _uid2AdapterMock.Setup(c => c.GetId(It.IsAny<string>())).ReturnsAsync(returnedToken);

            // Generate
            var requestGenerate = new AuthenticatedController.GenerateRequest() { Email = email };
            var response = _authenticatedController.GenerateOtp(_testUserAgent, requestGenerate);
            Assert.IsAssignableFrom<NoContentResult>(response);

            // Validate
            object cacheCode = code;
            _memoryCache
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheCode))
                .Returns(true);
            var requestValidate = new AuthenticatedController.ValidateRequest() { Email = email, Otp = (string) code };
            response = await _authenticatedController.ValidateOtp(_testUserAgent, requestValidate);

            Assert.IsAssignableFrom<OkObjectResult>(response);
            var responseData = GetResponseData(response);
            var token = (string) responseData.token;
            Assert.AreEqual(returnedToken, token);
            // token in cookie
            _cookieHelperMock.Verify(c => c.SetIdentifierCookie(
                It.IsAny<IResponseCookies>(),
                It.Is<string>(t => t == token)), Times.Once);
        }

        [Test]
        public async Task OTPFailedFullValidation()
        {
            var email = "example@mail.com";
            object code = "123456";

            // Generate
            var requestGenerate = new AuthenticatedController.GenerateRequest() { Email = email };
            var response = _authenticatedController.GenerateOtp(_testUserAgent, requestGenerate);
            Assert.IsAssignableFrom<NoContentResult>(response);

            // Validate
            var erroneousCode = "012345";
            _memoryCache
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out code))
                .Returns(true);
            var requestValidate = new AuthenticatedController.ValidateRequest() { Email = email, Otp = erroneousCode };
            response = await _authenticatedController.ValidateOtp(_testUserAgent, requestValidate);
            Assert.IsAssignableFrom<NotFoundResult>(response);
            _cookieHelperMock.Verify(c => c.SetIdentifierCookie(
                It.IsAny<IResponseCookies>(), It.IsAny<string>()), Times.Never);
        }
        #endregion

        #region Helpers
        private dynamic GetResponseData(IActionResult response)
        {
            var responseContent = response as OkObjectResult;
            var data = responseContent?.Value;

            return data;
        }
        #endregion
    }
}
