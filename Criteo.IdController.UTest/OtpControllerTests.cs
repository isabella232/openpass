using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Criteo.IdController.Controllers;
using Criteo.IdController.Helpers;
using Criteo.UserIdentification;
using Metrics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using static Criteo.Glup.IdController.Types;

namespace Criteo.IdController.UTest
{
    [TestFixture]
    public class OtpControllerTests
    {
        private const string _testUserAgent = "TestUserAgent";
        private const string _cookieName = "openpass_token";
        private const int _cookieLifetimeDays = 390;

        private OtpController _otpController;

        private Mock<IHostingEnvironment> _hostingEnvironmentMock;
        private Mock<IMetricsRegistry> _metricRegistryMock;
        private Mock<IMemoryCache> _memoryCache;
        private Mock<IConfigurationHelper> _configurationHelperMock;
        private Mock<IEmailHelper> _emailHelperMock;
        private Mock<IGlupHelper> _glupHelperMock;
        private Mock<ICodeGeneratorHelper> _codeGeneratorHelperMock;

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

            _otpController = GetOtpController();
        }

        [Test]
        public void ForbiddenWhenGenerationNotEnabled()
        {
            _configurationHelperMock.Setup(c => c.EnableOtp).Returns(false);
            var request = new OtpController.GenerateRequest() { Email = "example@mail.com" };
            var response = _otpController.GenerateOtp(_testUserAgent, request);

            Assert.IsAssignableFrom<NotFoundResult>(response);
        }

        [Test]
        public void ForbiddenWhenValidationNotEnabled()
        {
            _configurationHelperMock.Setup(c => c.EnableOtp).Returns(false);
            var request = new OtpController.ValidateRequest() { Email = "example@mail.com", Otp = "123456" };
            var response = _otpController.ValidateOtp(_testUserAgent, request);

            Assert.IsAssignableFrom<NotFoundResult>(response);
        }

        [Test]
        public void BadRequestWhenGenerationEmailIsInvalid()
        {
            _emailHelperMock.Setup(e => e.IsValidEmail(It.IsAny<string>())).Returns(false);

            var request = new OtpController.GenerateRequest();
            var response = _otpController.GenerateOtp(_testUserAgent, request);

            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Test]
        public void BadRequestWhenValidationEmailIsInvalid()
        {
            _emailHelperMock.Setup(e => e.IsValidEmail(It.IsAny<string>())).Returns(false);

            var request = new OtpController.ValidateRequest();
            var response = _otpController.ValidateOtp(_testUserAgent, request);

            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Test]
        public void BadRequestWhenValidationOtpIsInvalid()
        {
            _codeGeneratorHelperMock.Setup(c => c.IsValidCode(It.IsAny<string>())).Returns(false);

            var request = new OtpController.ValidateRequest();
            var response = _otpController.ValidateOtp(_testUserAgent, request);

            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Test]
        public void BadRequestWhenValidationEmailAndOtpAreInvalid()
        {
            _emailHelperMock.Setup(e => e.IsValidEmail(It.IsAny<string>())).Returns(false);
            _codeGeneratorHelperMock.Setup(c => c.IsValidCode(It.IsAny<string>())).Returns(false);

            var request = new OtpController.ValidateRequest();
            var response = _otpController.ValidateOtp(_testUserAgent, request);

            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Test]
        public void ValidRequestGeneration()
        {
            var request = new OtpController.GenerateRequest();
            var response = _otpController.GenerateOtp(_testUserAgent, request);

            Assert.IsAssignableFrom<NoContentResult>(response);
        }

        [Test]
        public void ValidRequestValidation()
        {
            object otp = "123456";
            _memoryCache
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out otp))
                .Returns(true);
            var request = new OtpController.ValidateRequest() { Email = "example@mail.com", Otp = "123456" };

            var response = _otpController.ValidateOtp(_testUserAgent, request);

            // token in JSON response
            Assert.IsAssignableFrom<OkObjectResult>(response);
            var responseData = GetResponseData(response);
            Assert.IsTrue(Guid.TryParse((string) responseData.token, out _));

            // token in cookie
            var cookie = GetSetCookieHeaderToken(_otpController.HttpContext.Response);
            Assert.NotNull(cookie);
            Assert.IsTrue(Guid.TryParse(cookie.Value, out _));
            var expectedExpire = DateTime.Today.AddDays(_cookieLifetimeDays);
            Assert.That(cookie?.Expires?.DateTime, Is.EqualTo(expectedExpire).Within(1).Days);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("origin.com")]
        public void GenerationGlupEmitted(string originHost)
        {
            var request = new OtpController.GenerateRequest() { Email = "example@mail.com", OriginHost = originHost };
            _otpController.GenerateOtp(_testUserAgent, request);

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
        public void ValidationGlupEmitted(string originHost)
        {
            object code = "123456";
            _memoryCache
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out code))
                .Returns(true);

            var request = new OtpController.ValidateRequest() { Email = "example@mail.com", Otp = (string) code, OriginHost = originHost };
            _otpController.ValidateOtp(_testUserAgent, request);

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
            var request = new OtpController.GenerateRequest() { Email = email };
            _otpController.GenerateOtp(_testUserAgent, request);
            _memoryCache.Verify(m => m.CreateEntry(It.Is<string>(s => s == email)), Times.Once);
        }

        [Test]
        public void GenerateOTPAndSendEmail()
        {
            var email = "example@mail.com";
            var code = "123456";

            _codeGeneratorHelperMock.Setup(c => c.GenerateRandomCode()).Returns(code);

            var request = new OtpController.GenerateRequest() { Email = email };
            _otpController.GenerateOtp(_testUserAgent, request);
            _emailHelperMock.Verify(e => e.SendOtpEmail(It.Is<string>(s => s == email), It.Is<string>(c => c == code)), Times.Once);
        }

        [Test]
        public void OTPSuccessfulFullValidation()
        {
            var email = "example@mail.com";
            var code = "123456";

            _codeGeneratorHelperMock.Setup(c => c.GenerateRandomCode()).Returns(code);

            // Generate
            var requestGenerate = new OtpController.GenerateRequest() { Email = email };
            var response = _otpController.GenerateOtp(_testUserAgent, requestGenerate);
            Assert.IsAssignableFrom<NoContentResult>(response);

            // Validate
            object cacheCode = code;
            _memoryCache
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheCode))
                .Returns(true);
            var requestValidate = new OtpController.ValidateRequest() { Email = email, Otp = (string) code };
            response = _otpController.ValidateOtp(_testUserAgent, requestValidate);
            Assert.IsAssignableFrom<OkObjectResult>(response);
        }

        [Test]
        public void OTPFailedFullValidation()
        {
            var email = "example@mail.com";
            object code = "123456";

            // Generate
            var requestGenerate = new OtpController.GenerateRequest() { Email = email };
            var response = _otpController.GenerateOtp(_testUserAgent, requestGenerate);
            Assert.IsAssignableFrom<NoContentResult>(response);

            // Validate
            var erroneousCode = "012345";
            _memoryCache
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out code))
                .Returns(true);
            var requestValidate = new OtpController.ValidateRequest() { Email = email, Otp = erroneousCode };
            response = _otpController.ValidateOtp(_testUserAgent, requestValidate);
            Assert.IsAssignableFrom<NotFoundResult>(response);
        }

        #region Helpers
        private OtpController GetOtpController(Dictionary<string, string> cookies = null)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Cookies = new RequestCookieCollection(cookies);

            var otpController = new OtpController(_hostingEnvironmentMock.Object, _metricRegistryMock.Object,
                _memoryCache.Object, _configurationHelperMock.Object, _emailHelperMock.Object, _glupHelperMock.Object,
                _codeGeneratorHelperMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };

            return otpController;
        }

        private dynamic GetResponseData(IActionResult response)
        {
            var responseContent = response as OkObjectResult;
            var data = responseContent?.Value;

            return data;
        }

        private SetCookieHeaderValue GetSetCookieHeaderToken(HttpResponse response)
        {
            var setCookies = response.GetTypedHeaders().SetCookie;
            var cookie = setCookies?.FirstOrDefault(c => c.Name == _cookieName);

            return cookie;
        }
        #endregion
    }
}
