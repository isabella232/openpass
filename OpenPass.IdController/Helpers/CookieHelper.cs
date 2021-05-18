using System;
using Microsoft.AspNetCore.Http;

namespace OpenPass.IdController.Helpers
{
    public interface ICookieHelper
    {
        bool TryGetIdentifierCookie(IRequestCookieCollection cookieContainer, out string value);

        void SetIdentifierCookie(IResponseCookies cookieContainer, string value);
    }

    public class CookieHelper : ICookieHelper
    {
        private const int _cookieLifetimeDays = 30;
        private const string _identifierCookieName = "__uid2_advertising_token";

        private readonly CookieOptions _defaultCookieOptions;

        public CookieHelper()
        {
            _defaultCookieOptions = new CookieOptions
            {
                Expires = new DateTimeOffset(DateTime.Today.AddDays(_cookieLifetimeDays)),
                Secure = true
            };
        }

        #region Generic methods

        private bool TryGetCookie(IRequestCookieCollection cookieContainer, string name, out string value) =>
            cookieContainer.TryGetValue(name, out value);

        private void SetCookie(IResponseCookies cookieContainer, string name, string value) =>
            cookieContainer.Append(name, value, _defaultCookieOptions);

        #endregion Generic methods

        #region Cookie-specific methods

        public bool TryGetIdentifierCookie(IRequestCookieCollection cookieContainer, out string value) =>
            TryGetCookie(cookieContainer, _identifierCookieName, out value);

        public void SetIdentifierCookie(IResponseCookies cookieContainer, string value) =>
            SetCookie(cookieContainer, _identifierCookieName, value);

        #endregion Cookie-specific methods
    }
}
