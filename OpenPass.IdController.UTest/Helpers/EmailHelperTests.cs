using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using OpenPass.IdController.Helpers;

namespace OpenPass.IdController.UTest.Helpers
{
    [TestFixture]
    public class EmailHelperTests
    {
        private IEmailHelper _emailHelper;

        [SetUp]
        public void Setup()
        {
            _emailHelper = new EmailHelper(
                Mock.Of<IMetricHelper>(),
                Mock.Of<IViewRenderHelper>(),
                Mock.Of<IEmailConfiguration>());
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

        #region Email configuration

        [Test]
        public void UserConfigurationTest()
        {
            // Arrange
            var fakeSettings = new Dictionary<string, string> {
                { "Email:Server:Host", "mail.example.com" },
                { "Email:Server:Port", "25" },
                { "Email:Server:EnableSsl", "true" },
                { "Email:Sender:DisplayName", "DisplayName" },
                { "Email:Sender:Address", "sender@example.com" },
                { "Email:Authentication:UserName", "AuthName" },
                { "Email:Authentication:Password", "AuthPass" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(fakeSettings)
                .Build();

            // Act
            var emailConfig = new EmailConfiguration(configuration);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(emailConfig.MailServer, "mail.example.com");
                Assert.AreEqual(emailConfig.MailServerPort, 25);
                Assert.AreEqual(emailConfig.MailServerSsl, true);
                Assert.AreEqual(emailConfig.SenderDisplayName, "DisplayName");
                Assert.AreEqual(emailConfig.SenderEmailAddress, "sender@example.com");
                Assert.AreEqual(emailConfig.AuthUserName, "AuthName");
                Assert.AreEqual(emailConfig.AuthPassword, "AuthPass");
            });
        }

        [Test]
        public void DefaultConfigurationTest()
        {
            // Arrange
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(null)
                .Build();

            // Act
            var emailConfig = new EmailConfiguration(configuration);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(emailConfig.MailServer, "mail-relay.service.consul");
                Assert.AreEqual(emailConfig.MailServerPort, 25);
                Assert.AreEqual(emailConfig.MailServerSsl, false);
                Assert.AreEqual(emailConfig.SenderDisplayName, "OpenPass");
                Assert.AreEqual(emailConfig.SenderEmailAddress, "user-first@criteo.com");
                Assert.AreEqual(emailConfig.AuthUserName, null);
                Assert.AreEqual(emailConfig.AuthPassword, null);
            });
        }

        #endregion Email configuration
    }
}
