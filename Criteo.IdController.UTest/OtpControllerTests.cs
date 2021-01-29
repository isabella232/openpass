using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Criteo.IdController.Controllers;
using Criteo.IdController.Helpers;
using Criteo.Services.Glup;
using Criteo.UserAgent;
using Criteo.UserIdentification;
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

        [SetUp]
        public void Setup()
        {
            _configurationHelperMock = new Mock<IConfigurationHelper>();
            _configurationHelperMock.Setup(c => c.EnableOtp).Returns(true);
            _metricRegistryMock = new Mock<IMetricsRegistry>();
            _metricRegistryMock.Setup(mr => mr.GetOrRegister(It.IsAny<string>(), It.IsAny<Func<Counter>>())).Returns(new Counter(Granularity.CoarseGrain));

            _otpController = new OtpController(_configurationHelperMock.Object, _metricRegistryMock.Object);
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


    }
}
