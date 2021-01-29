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
        private OtpController _otpController;
        private Mock<IConfigurationHelper> _configurationHelperMock;
        private Mock<IMetricsRegistry> _metricRegistryMock;
        private Mock<IMemoryCache> _memoryCache;

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

            _otpController = new OtpController(_configurationHelperMock.Object, _metricRegistryMock.Object, _memoryCache.Object);
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

        // TODO: Check the OTP generation (different codes). Will be done in ITest when introducing email helper.
    }
}
