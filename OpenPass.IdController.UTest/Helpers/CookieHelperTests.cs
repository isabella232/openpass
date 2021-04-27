using System;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using OpenPass.IdController.Helpers;

namespace OpenPass.IdController.UTest.Helpers
{
    [TestFixture]
    public class CookieHelperTests
    {
        private const int _cookieLifetimeDays = 30;
        private const string _identifierCookieName = "__uid2_advertising_token";

        private ICookieHelper _cookieHelper;

        [SetUp]
        public void SetUp()
        {
            _cookieHelper = new CookieHelper();
        }

        #region Cookie-specific

        [Test]
        public void GetIdentifierCookieTest()
        {
            // Arrange
            string placeholder;
            var cookieContainerMock = new Mock<IRequestCookieCollection>();

            // Act
            _cookieHelper.TryGetIdentifierCookie(cookieContainerMock.Object, out _);

            // Assert
            cookieContainerMock.Verify(c => c.TryGetValue(It.Is<string>(k => k == _identifierCookieName), out placeholder), Times.Once);
        }

        [Test]
        public void SetIdentifierCookieTest()
        {
            // Arrange
            var cookieContainerMock = new Mock<IResponseCookies>();

            // Act
            _cookieHelper.SetIdentifierCookie(cookieContainerMock.Object, "value");

            var expectedExpire = DateTime.Today.AddDays(_cookieLifetimeDays);

            // Assert
            cookieContainerMock.Verify(c =>
                c.Append(
                    It.Is<string>(k => k == _identifierCookieName),
                    It.IsAny<string>(),
                    It.Is<CookieOptions>(co => co.Expires.Value.DateTime.Equals(expectedExpire))));
        }

        [Test]
        public void RemoveIdentifierCookieTest()
        {
            // Arrange
            var cookieContainerMock = new Mock<IResponseCookies>();

            // Act
            _cookieHelper.RemoveIdentifierCookie(cookieContainerMock.Object);

            // Assert
            cookieContainerMock.Verify(c => c.Delete(It.Is<string>(k => k == _identifierCookieName)), Times.Once);
        }

        #endregion Cookie-specific
    }
}
