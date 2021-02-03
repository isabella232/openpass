using System;
using Moq;
using NUnit.Framework;
using Criteo.IdController.Controllers;
using Criteo.IdController.Helpers;
using Criteo.UserIdentification;
using Metrics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using static Criteo.Glup.IdController.Types;

namespace Criteo.IdController.UTest
{
    [TestFixture]
    public class OtpControllerTests
    {
        private const int _otpCodeLength = 6;
        private const string _testUserAgent = "TestUserAgent";

        private OtpController _otpController;

        private Mock<IHostingEnvironment> _hostingEnvironmentMock;
        private Mock<IMetricsRegistry> _metricRegistryMock;
        private Mock<IMemoryCache> _memoryCache;
        private Mock<IConfigurationHelper> _configurationHelperMock;
        private Mock<IEmailHelper> _emailHelperMock;
        private Mock<IGlupHelper> _glupHelperMock;

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
            _glupHelperMock = new Mock<IGlupHelper>();

            _otpController = new OtpController(
                _hostingEnvironmentMock.Object,
                _metricRegistryMock.Object,
                _memoryCache.Object,
                _configurationHelperMock.Object,
                _emailHelperMock.Object,
                _glupHelperMock.Object);
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
        public void BadRequestWhenGenerationEmailIsInvalid(
            [Values(null, "", "mail.com")] string email)
        {
            var request = new OtpController.GenerateRequest() { Email = email };
            var response = _otpController.GenerateOtp(_testUserAgent, request);

            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Test]
        public void BadRequestWhenValidationEmailIsInvalid(
            [Values(null, "", "mail.com")] string email)
        {
            var validOtp = "123456";
            var request = new OtpController.ValidateRequest() { Email = email, Otp = validOtp };
            var response = _otpController.ValidateOtp(_testUserAgent, request);

            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Test]
        public void BadRequestWhenValidationOtpIsInvalid(
            [Values(null, "", "abcdef", "123abc", "123", "1234567")] string otp)
        {
            var validEmail = "example@mail.com";
            var request = new OtpController.ValidateRequest() { Email = validEmail, Otp = otp };
            var response = _otpController.ValidateOtp(_testUserAgent, request);

            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Test]
        public void ValidRequestGeneration()
        {
            var request = new OtpController.GenerateRequest() { Email = "example@mail.com" };
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

            Assert.IsAssignableFrom<OkResult>(response);
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
            var request = new OtpController.GenerateRequest() { Email = email };
            _otpController.GenerateOtp(_testUserAgent, request);
            _emailHelperMock.Verify(e => e.SendOtpEmail(It.Is<string>(s => s == email), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void OTPCodesAreProperlyGenerated()
        {
            string firstCode = null;
            string lastCode = null;

            var email = "example@mail.com";
            var request = new OtpController.GenerateRequest() { Email = email };
            _emailHelperMock.Setup(e => e.SendOtpEmail(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((_, otp) => firstCode = otp);
            _otpController.GenerateOtp(_testUserAgent, request);
            _emailHelperMock.Setup(e => e.SendOtpEmail(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((_, otp) => lastCode = otp);
            _otpController.GenerateOtp(_testUserAgent, request);

            foreach (var code in new[] { firstCode, lastCode })
            {
                Assert.IsNotNull(code);
                Assert.AreEqual(_otpCodeLength, code.Length);
            }
            Assert.AreNotEqual(firstCode, lastCode);
        }

        [Test]
        public void OTPSuccessfulFullValidation()
        {
            var email = "example@mail.com";

            // Generate
            object code = null;
            var requestGenerate = new OtpController.GenerateRequest() { Email = email };
            _emailHelperMock.Setup(e => e.SendOtpEmail(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((_, otp) => code = otp);
            var response = _otpController.GenerateOtp(_testUserAgent, requestGenerate);
            Assert.IsAssignableFrom<NoContentResult>(response);

            // Validate
            _memoryCache
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out code))
                .Returns(true);
            var requestValidate = new OtpController.ValidateRequest() { Email = email, Otp = (string) code };
            response = _otpController.ValidateOtp(_testUserAgent, requestValidate);
            Assert.IsAssignableFrom<OkResult>(response);
        }

        [Test]
        public void OTPFailedFullValidation()
        {
            var email = "example@mail.com";

            // Generate
            object code = null;
            var requestGenerate = new OtpController.GenerateRequest() { Email = email };
            _emailHelperMock.Setup(e => e.SendOtpEmail(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((_, otp) => code = otp);
            var response = _otpController.GenerateOtp(_testUserAgent, requestGenerate);
            Assert.IsAssignableFrom<NoContentResult>(response);

            // Validate
            _memoryCache
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out code))
                .Returns(true);
            var nextCode = (int.Parse((string) code) + 1) % 99999; // Force different code (avoid overflow)
            var erroneousCode = $"{nextCode:000000}"; // Add leading zeros if necessary
            var requestValidate = new OtpController.ValidateRequest() { Email = email, Otp = erroneousCode };
            response = _otpController.ValidateOtp(_testUserAgent, requestValidate);
            Assert.IsAssignableFrom<NotFoundResult>(response);
        }
    }
}
