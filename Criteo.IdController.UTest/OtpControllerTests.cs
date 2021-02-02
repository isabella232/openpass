using System;
using Moq;
using NUnit.Framework;
using Criteo.IdController.Controllers;
using Criteo.IdController.Helpers;
using Metrics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using static Criteo.Glup.IdController.Types;
using IdControllerGlup = Criteo.Glup.IdController;

namespace Criteo.IdController.UTest
{
    [TestFixture]
    public class OtpControllerTests
    {
        private const int _otpCodeLength = 6;

        private OtpController _otpController;

        private Mock<IMetricsRegistry> _metricRegistryMock;
        private Mock<IMemoryCache> _memoryCache;
        private Mock<IConfigurationHelper> _configurationHelperMock;
        private Mock<IEmailHelper> _emailHelperMock;

        [SetUp]
        public void Setup()
        {
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

            _otpController = new OtpController(_metricRegistryMock.Object, _memoryCache.Object, _configurationHelperMock.Object, _emailHelperMock.Object);
        }

        [Test]
        public void ForbiddenWhenFeatureNotEnabled()
        {
            _configurationHelperMock.Setup(c => c.EnableOtp).Returns(false);
            var response = _otpController.GenerateOtp("example@mail.com");

            Assert.IsAssignableFrom<NotFoundResult>(response);
        }

        [TestCase(null)]
        [TestCase("")]
        public void BadRequestWhenEmailIsNotProvided(string email)
        {
            var response = _otpController.GenerateOtp(email);

            Assert.IsAssignableFrom<BadRequestResult>(response);
        }

        [Test]
        public void ValidRequest()
        {
            var response = _otpController.GenerateOtp("example@mail.com");

            Assert.IsAssignableFrom<NoContentResult>(response);
        }

        [Test]
        public void GenerateOTPAndAddToCache()
        {
            var email = "example@mail.com";
            _otpController.GenerateOtp(email);
            _memoryCache.Verify(m => m.CreateEntry(It.Is<string>(s => s == email)));
        }

        [Test]
        public void GenerateOTPAndSendEmail()
        {
            var email = "example@mail.com";
            _otpController.GenerateOtp(email);
            _emailHelperMock.Verify(e => e.SendOtpEmail(It.Is<string>(s => s == email), It.IsAny<string>()));
        }

        [Test]
        public void OTPCodesAreProperlyGenerated()
        {
            string firstCode = null;
            string lastCode = null;

            var email = "example@mail.com";
            _emailHelperMock.Setup(e => e.SendOtpEmail(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((_, otp) => firstCode = otp);
            _otpController.GenerateOtp(email);
            _emailHelperMock.Setup(e => e.SendOtpEmail(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>((_, otp) => lastCode = otp);
            _otpController.GenerateOtp(email);

            foreach (var code in new[] { firstCode, lastCode })
            {
                Assert.IsNotNull(code);
                Assert.AreEqual(_otpCodeLength, code.Length);
            }
            Assert.AreNotEqual(firstCode, lastCode);
        }
    }
}
