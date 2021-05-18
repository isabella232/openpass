using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OpenPass.IdController.Helpers;
using OpenPass.IdController.Helpers.Configuration;
using OpenPass.IdController.Models.Configuration;

namespace OpenPass.IdController.UTest.Helpers
{
    [TestFixture]
    public class EmailHelperTests
    {
        private IEmailHelper _emailHelper;
        private Mock<IConfigurationManager> _configurationManager;

        [SetUp]
        public void Setup()
        {
            _configurationManager = new Mock<IConfigurationManager>();

            var smtpSettings = new SmtpSettings
            {
                Host = "test@test.com",
                Port = 1,
                Address = "test@test.com",
                EnableSsl = false,
                DisplayName = "test"
            };
            _configurationManager.Setup(x => x.SmtpSettings).Returns(smtpSettings);

            _emailHelper = new EmailHelper(
                Mock.Of<IMetricHelper>(),
                Mock.Of<IViewRenderHelper>(),
                _configurationManager.Object,
                Mock.Of<ILogger<EmailHelper>>());
        }

        [Test]
        public void ValidEmailTest()
        {
            // Arrange
            var validEmail = "hello@example.com";

            // Act
            var validation = _emailHelper.IsValidEmail(validEmail);

            // Assert
            Assert.IsTrue(validation);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("email.com")]
        [TestCase("@email.com")]
        [TestCase("email@")]
        [TestCase("email@.com")]
        public void InvalidEmailTest(string email)
        {
            // Arrange && Act
            var validation = _emailHelper.IsValidEmail(email);

            // Assert
            Assert.IsFalse(validation);
        }
    }
}
