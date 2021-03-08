using System;
using Microsoft.AspNetCore.Http;

namespace Criteo.IdController.Helpers
{
    public interface ICookieHelper
    {
        bool TryGetIdentifierCookie(IRequestCookieCollection cookieContainer, out string value);
        void SetIdentifierCookie(IResponseCookies cookieContainer, string value);
        void RemoveIdentifierCookie(IResponseCookies cookieContainer);
    }

    public class CookieHelper : ICookieHelper
    {
        private const int _cookieLifetimeDays = 30;
        private const string _identifierCookieName = "openpass_token";

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
        #endregion

        #region Cookie-specific methods
        public bool TryGetIdentifierCookie(IRequestCookieCollection cookieContainer, out string value) =>
            TryGetCookie(cookieContainer, _identifierCookieName, out value);

        public void SetIdentifierCookie(IResponseCookies cookieContainer, string value) =>
            SetCookie(cookieContainer, _identifierCookieName, value);

        public void RemoveIdentifierCookie(IResponseCookies cookieContainer) =>
            RemoveCookie(cookieContainer, _identifierCookieName);
        #endregion
    }
}
