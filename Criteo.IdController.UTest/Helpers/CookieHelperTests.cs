using System;
using Criteo.IdController.Helpers;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace Criteo.IdController.UTest.Helpers
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
            string placeholder;
            var cookieContainerMock = new Mock<IRequestCookieCollection>();

            _cookieHelper.TryGetIdentifierCookie(cookieContainerMock.Object, out _);

            cookieContainerMock.Verify(c => c.TryGetValue(It.Is<string>(k => k == _identifierCookieName), out placeholder), Times.Once);
        }

        [Test]
        public void SetIdentifierCookieTest()
        {
            var cookieContainerMock = new Mock<IResponseCookies>();
            _cookieHelper.SetIdentifierCookie(cookieContainerMock.Object, "value");

            var expectedExpire = DateTime.Today.AddDays(_cookieLifetimeDays);
            cookieContainerMock.Verify(c =>
                c.Append(
                    It.Is<string>(k => k == _identifierCookieName),
                    It.IsAny<string>(),
                    It.Is<CookieOptions>(co => co.Expires.Value.DateTime.Equals(expectedExpire))));
        }

        [Test]
        public void RemoveIdentifierCookieTest()
        {
            var cookieContainerMock = new Mock<IResponseCookies>();

            _cookieHelper.RemoveIdentifierCookie(cookieContainerMock.Object);

            cookieContainerMock.Verify(c => c.Delete(It.Is<string>(k => k == _identifierCookieName)), Times.Once);
        }

        #endregion Cookie-specific
    }
}
