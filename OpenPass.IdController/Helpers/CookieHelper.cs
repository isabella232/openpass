using System;
using Microsoft.AspNetCore.Http;

namespace OpenPass.IdController.Helpers
{
    public interface ICookieHelper
    {
        bool TryGetIdentifierCookie(IRequestCookieCollection cookieContainer, out string value);

        void SetIdentifierCookie(IResponseCookies cookieContainer, string value);

        void RemoveIdentifierCookie(IResponseCookies cookieContainer);

        bool TryGetOptoutCookie(IRequestCookieCollection cookieContainer, out string value);

        void SetOptoutCookie(IResponseCookies cookieContainer, string value);

        void RemoveOptoutCookie(IResponseCookies cookieContainer);
    }

    public class CookieHelper : ICookieHelper
    {
        private const int _cookieLifetimeDays = 30;
        private const string _identifierCookieName = "__uid2_advertising_token";
        private const string _optoutCookieName = "__optout";

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

        private void RemoveCookie(IResponseCookies cookieContainer, string name) =>
            cookieContainer.Delete(name);

        #endregion Generic methods

        #region Cookie-specific methods

        public bool TryGetIdentifierCookie(IRequestCookieCollection cookieContainer, out string value) =>
            TryGetCookie(cookieContainer, _identifierCookieName, out value);

        public void SetIdentifierCookie(IResponseCookies cookieContainer, string value) =>
            SetCookie(cookieContainer, _identifierCookieName, value);

        public void RemoveIdentifierCookie(IResponseCookies cookieContainer) =>
            RemoveCookie(cookieContainer, _identifierCookieName);

        public bool TryGetOptoutCookie(IRequestCookieCollection cookieContainer, out string value) =>
            TryGetCookie(cookieContainer, _optoutCookieName, out value);

        public void SetOptoutCookie(IResponseCookies cookieContainer, string value) =>
            SetCookie(cookieContainer, _optoutCookieName, value);

        public void RemoveOptoutCookie(IResponseCookies cookieContainer) =>
            RemoveCookie(cookieContainer, _optoutCookieName);

        #endregion Cookie-specific methods
    }
}
