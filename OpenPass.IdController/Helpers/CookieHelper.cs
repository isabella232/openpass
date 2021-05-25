using System;
using Microsoft.AspNetCore.Http;

namespace OpenPass.IdController.Helpers
{
    public interface ICookieHelper
    {
        bool TryGetUid2AdvertisingCookie(IRequestCookieCollection cookieContainer, out string value);

        void SetUid2AdvertisingCookie(IResponseCookies cookieContainer, string value);

        void RemoveUid2AdvertisingCookie(IResponseCookies cookieContainer);

        bool TryGetOptoutCookie(IRequestCookieCollection cookieContainer, out string value);

        void SetOptoutCookie(IResponseCookies cookieContainer, string value);

        void RemoveOptoutCookie(IResponseCookies cookieContainer);

        bool TryGetIdentifierForAdvertisingCookie(IRequestCookieCollection cookieContainer, out string value);

        void SetIdentifierForAdvertisingCookie(IResponseCookies cookieContainer, string value);

        void RemoveIdentifierForAdvertisingCookie(IResponseCookies cookieContainer);
    }

    public class CookieHelper : ICookieHelper
    {
        private const int _cookieLifetimeDays = 30;
        private const string _uid2AdvertisingCookieName = "__uid2_advertising_token";
        private const string _identifierForAdvertisingCookieName = "__ifa";
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

        public bool TryGetUid2AdvertisingCookie(IRequestCookieCollection cookieContainer, out string value) =>
            TryGetCookie(cookieContainer, _uid2AdvertisingCookieName, out value);

        public void SetUid2AdvertisingCookie(IResponseCookies cookieContainer, string value) =>
            SetCookie(cookieContainer, _uid2AdvertisingCookieName, value);

        public void RemoveUid2AdvertisingCookie(IResponseCookies cookieContainer) =>
            RemoveCookie(cookieContainer, _uid2AdvertisingCookieName);

        public bool TryGetOptoutCookie(IRequestCookieCollection cookieContainer, out string value) =>
            TryGetCookie(cookieContainer, _optoutCookieName, out value);

        public void SetOptoutCookie(IResponseCookies cookieContainer, string value) =>
            SetCookie(cookieContainer, _optoutCookieName, value);

        public void RemoveOptoutCookie(IResponseCookies cookieContainer) =>
            RemoveCookie(cookieContainer, _optoutCookieName);

        public bool TryGetIdentifierForAdvertisingCookie(IRequestCookieCollection cookieContainer, out string value) =>
            TryGetCookie(cookieContainer, _identifierForAdvertisingCookieName, out value);

        public void SetIdentifierForAdvertisingCookie(IResponseCookies cookieContainer, string value) =>
            SetCookie(cookieContainer, _identifierForAdvertisingCookieName, value);

        public void RemoveIdentifierForAdvertisingCookie(IResponseCookies cookieContainer) =>
            RemoveCookie(cookieContainer, _identifierForAdvertisingCookieName);

        #endregion Cookie-specific methods
    }
}
