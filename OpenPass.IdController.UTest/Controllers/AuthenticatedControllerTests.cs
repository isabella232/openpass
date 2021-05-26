using System.Threading.Tasks;
using Criteo.UserIdentification;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using OpenPass.IdController.Controllers;
using OpenPass.IdController.Helpers;
using OpenPass.IdController.Models;
using static Criteo.Glup.IdController.Types;

namespace OpenPass.IdController.UTest.Controllers
{
    [TestFixture]
    public class AuthenticatedControllerTests
    {
        private const string _testUserAgent = "TestUserAgent";

        private Mock<IHostingEnvironment> _hostingEnvironmentMock;
        private Mock<IMetricHelper> _metricHelperMock;
        private Mock<IMemoryCache> _memoryCache;
        private Mock<IConfigurationHelper> _configurationHelperMock;
        private Mock<IEmailHelper> _emailHelperMock;
        private Mock<IGlupHelper> _glupHelperMock;
        private Mock<ICodeGeneratorHelper> _codeGeneratorHelperMock;
        private Mock<ICookieHelper> _cookieHelperMock;
        private Mock<IIdentifierHelper> _identifierHelperMock;

        private AuthenticatedController _authenticatedController;

        [SetUp]
        public void Setup()
        {
            _hostingEnvironmentMock = new Mock<IHostingEnvironment>();
            _configurationHelperMock = new Mock<IConfigurationHelper>();
            _configurationHelperMock.Setup(c => c.EnableOtp).Returns(true);
            _metricHelperMock = new Mock<IMetricHelper>();
            _metricHelperMock.Setup(mh => mh.SendCounterMetric(It.IsAny<string>()));
            _identifierHelperMock = new Mock<IIdentifierHelper>();
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
            _cookieHelperMock = new Mock<ICookieHelper>();

            _authenticatedController = new AuthenticatedController(_hostingEnvironmentMock.Object, _metricHelperMock.Object,
                _memoryCache.Object, _configurationHelperMock.Object, _emailHelperMock.Object, _glupHelperMock.Object,
                _codeGeneratorHelperMock.Object, _cookieHelperMock.Object, _identifierHelperMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
        }

        #region One-time password (OTP)

        [Test]
        public void ForbiddenWhenGenerationNotEnabled()
        {
            // Arrange
            _configurationHelperMock.Setup(c => c.EnableOtp).Returns(false);
            var request = new GenerateRequest { Email = "example@mail.com" };

            // Act
            var response = _authenticatedController.GenerateOtp(_testUserAgent, request);

            // Assert
            Assert.IsAssignableFrom<NotFoundResult>(response);
        }

        [Test]
        public async Task ForbiddenWhenValidationNotEnabled()
        {
            // Arrange
            _configurationHelperMock.Setup(c => c.EnableOtp).Returns(false);
            var request = new ValidateRequest{ Email = "example@mail.com", Otp = "123456" };

            // Act
            var response = await _authenticatedController.ValidateOtp(_testUserAgent, request);

            // Assert
            Assert.IsAssignableFrom<NotFoundResult>(response);
        }

        [Test]
        public void BadRequestWhenGenerationEmailIsInvalid()
        {
            // Arrange
            _emailHelperMock.Setup(e => e.IsValidEmail(It.IsAny<string>())).Returns(false);

            var request = new GenerateRequest();

            // Act
            var response = _authenticatedController.GenerateOtp(_testUserAgent, request);

            // Assert
            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Test]
        public async Task BadRequestWhenValidationEmailIsInvalid()
        {
            // Arrange
            _emailHelperMock.Setup(e => e.IsValidEmail(It.IsAny<string>())).Returns(false);

            var request = new ValidateRequest();

            // Act
            var response = await _authenticatedController.ValidateOtp(_testUserAgent, request);

            // Assert
            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Test]
        public async Task BadRequestWhenValidationOtpIsInvalid()
        {
            // Arrange
            _codeGeneratorHelperMock.Setup(c => c.IsValidCode(It.IsAny<string>())).Returns(false);

            var request = new ValidateRequest();

            // Act
            var response = await _authenticatedController.ValidateOtp(_testUserAgent, request);

            // Assert
            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Test]
        public async Task BadRequestWhenValidationEmailAndOtpAreInvalid()
        {
            // Arrange
            _emailHelperMock.Setup(e => e.IsValidEmail(It.IsAny<string>())).Returns(false);
            _codeGeneratorHelperMock.Setup(c => c.IsValidCode(It.IsAny<string>())).Returns(false);

            var request = new ValidateRequest();

            // Act
            var response = await _authenticatedController.ValidateOtp(_testUserAgent, request);

            // Assert
            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Test]
        public void ValidRequestGeneration()
        {
            // Arrange
            var request = new GenerateRequest();

            // Act
            var response = _authenticatedController.GenerateOtp(_testUserAgent, request);

            // Assert
            Assert.IsAssignableFrom<NoContentResult>(response);
            _codeGeneratorHelperMock.Verify(c => c.GenerateRandomCode(), Times.Once);
        }

        [Test]
        public async Task ValidRequestValidation()
        {
            var email = "example@mail.com";
            object otp = "123456";
            var expectedUid2Token = "FreshUID2token";
            _memoryCache
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out otp))
                .Returns(true);
            _identifierHelperMock.Setup(x => x.TryGetUid2TokenAsync(It.IsAny<IResponseCookies>(),
                It.IsAny<EventType>(),
                It.IsAny<string>(),
                _testUserAgent,
                email,
                It.IsAny<string>()))
                .ReturnsAsync(expectedUid2Token);
            var request = new ValidateRequest { Email = email, Otp = "123456" };

            var response = await _authenticatedController.ValidateOtp(_testUserAgent, request);

            // token in JSON response
            Assert.IsAssignableFrom<OkObjectResult>(response);
            var responseData = GetResponseData(response);
            var uid2Token = (string) responseData.uid2Token;
            Assert.AreEqual(expectedUid2Token, uid2Token);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("origin.com")]
        public void GenerationGlupEmitted(string originHost)
        {
            // Arrange
            var request = new GenerateRequest { Email = "example@mail.com", OriginHost = originHost };

            // Act
            _authenticatedController.GenerateOtp(_testUserAgent, request);

            // Assert
            _glupHelperMock.Verify(g => g.EmitGlup(
                    It.Is<EventType>(e => e == EventType.EmailEntered),
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
            // Arrange
            var email = "example@mail.com";
            var request = new GenerateRequest { Email = email };

            // Act
            _authenticatedController.GenerateOtp(_testUserAgent, request);

            // Assert
            _memoryCache.Verify(m => m.CreateEntry(It.Is<string>(s => s == email)), Times.Once);
        }

        [Test]
        public void GenerateOTPAndSendEmail()
        {
            // Arrange
            var email = "example@mail.com";
            var code = "123456";

            _codeGeneratorHelperMock.Setup(c => c.GenerateRandomCode()).Returns(code);

            var request = new GenerateRequest { Email = email };

            // Act
            _authenticatedController.GenerateOtp(_testUserAgent, request);

            // Assert
            _emailHelperMock.Verify(e => e.SendOtpEmail(It.Is<string>(s => s == email), It.Is<string>(c => c == code)), Times.Once);
        }

        [Test]
        public async Task OTPSuccessfulFullValidation()
        {
            var email = "example@mail.com";
            var code = "123456";
            var expectedUid2Token = "FreshUID2token";
            var expectedIfaToken = "ifaToken";

            _codeGeneratorHelperMock.Setup(c => c.GenerateRandomCode()).Returns(code);
            _identifierHelperMock.Setup(x => x.TryGetUid2TokenAsync(It.IsAny<IResponseCookies>(),
                It.IsAny<EventType>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                email,
                It.IsAny<string>()))
                .ReturnsAsync(expectedUid2Token);
            _identifierHelperMock.Setup(x=>x.GetOrCreateIfaToken(It.IsAny<IRequestCookieCollection>(), It.IsAny<string>(), It.IsAny<string>(), _testUserAgent))
                .Returns(expectedIfaToken);

            // Generate
            var requestGenerate = new GenerateRequest { Email = email };
            var response = _authenticatedController.GenerateOtp(_testUserAgent, requestGenerate);
            Assert.IsAssignableFrom<NoContentResult>(response);

            // Validate
            object cacheCode = code;
            _memoryCache
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheCode))
                .Returns(true);
            var requestValidate = new ValidateRequest { Email = email, Otp = (string) code };
            response = await _authenticatedController.ValidateOtp(_testUserAgent, requestValidate);

            Assert.IsAssignableFrom<OkObjectResult>(response);
            var responseData = GetResponseData(response);
            var uid2Token = (string) responseData.uid2Token;
            var ifaToken = (string) responseData.ifaToken;

            Assert.AreEqual(expectedUid2Token, uid2Token);
            Assert.AreEqual(expectedIfaToken, ifaToken);

            // token in cookie
            _cookieHelperMock.Verify(x => x.SetIdentifierForAdvertisingCookie(
                It.IsAny<IResponseCookies>(),
                It.Is<string>(token => token == ifaToken)),
               Times.Once);
        }

        [Test]
        public async Task OTPFailedFullValidation()
        {
            // Arrange
            var email = "example@mail.com";
            object code = "123456";

            // Generate
            var requestGenerate = new GenerateRequest { Email = email };
            var response = _authenticatedController.GenerateOtp(_testUserAgent, requestGenerate);
            Assert.IsAssignableFrom<NoContentResult>(response);

            // Validate
            var erroneousCode = "012345";
            _memoryCache
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out code))
                .Returns(true);
            var requestValidate = new ValidateRequest { Email = email, Otp = erroneousCode };
            response = await _authenticatedController.ValidateOtp(_testUserAgent, requestValidate);
            Assert.IsAssignableFrom<NotFoundResult>(response);
            _cookieHelperMock.Verify(c => c.SetUid2AdvertisingCookie(
                It.IsAny<IResponseCookies>(), It.IsAny<string>()), Times.Never);
        }

        #endregion One-time password (OTP)

        #region External SSO services

        [Test]
        public async Task BadRequestWhenSSOEmailIsInvalid()
        {
            _emailHelperMock.Setup(e => e.IsValidEmail(It.IsAny<string>())).Returns(false);

            var request = new GenerateRequest();
            var response = await _authenticatedController.GenerateEmailToken(_testUserAgent, request);

            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Test]
        public async Task ValidSSOTokenGeneration()
        {
            // Arrange
            const string expectedUid2Token = "FreshUID2token";
            const string expectedIfaToken = "ifaToken";
            var email = "example@gmail.com";
            _identifierHelperMock.Setup(x => x.TryGetUid2TokenAsync(It.IsAny<IResponseCookies>(),
                It.IsAny<EventType>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                email,
                It.IsAny<string>()))
                .ReturnsAsync(expectedUid2Token);
            _identifierHelperMock.Setup(x => x.GetOrCreateIfaToken(It.IsAny<IRequestCookieCollection>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(expectedIfaToken);
            var request = new GenerateRequest { Email = email };

            // Act
            var response = await _authenticatedController.GenerateEmailToken(_testUserAgent, request);

            // token in JSON response
            Assert.IsAssignableFrom<OkObjectResult>(response);
            var responseData = GetResponseData(response);
            var uid2Token = (string) responseData.uid2Token;
            var ifaToken = (string) responseData.ifaToken;

            // Assert
            Assert.AreEqual(expectedUid2Token, uid2Token);
            Assert.AreEqual(expectedIfaToken, ifaToken);
        }

        #endregion External SSO services

        #region Helpers

        private dynamic GetResponseData(IActionResult response)
        {
            var responseContent = response as OkObjectResult;
            var data = responseContent?.Value;

            return data;
        }

        #endregion Helpers
    }
}
