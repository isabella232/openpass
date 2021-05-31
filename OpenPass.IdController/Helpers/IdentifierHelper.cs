using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OpenPass.IdController.Helpers.Adapters;

namespace OpenPass.IdController.Helpers
{
    public interface IIdentifierHelper
    {
        string GetOrCreateIfaToken(IRequestCookieCollection cookieContainer, string metricPrefix);

        Task<string> TryGetUid2TokenAsync(IResponseCookies cookieContainer, string email, string metricPrefix);
    }

    public class IdentifierHelper : IIdentifierHelper
    {
        private readonly ICookieHelper _cookieHelper;
        private readonly IMetricHelper _metricHelper;
        private readonly IIdentifierAdapter _uid2Adapter;

        public IdentifierHelper(
            IMetricHelper metricHelper,
            ICookieHelper cookieHelper,
            IIdentifierAdapter uid2Adapter)
        {
            _metricHelper = metricHelper;
            _cookieHelper = cookieHelper;
            _uid2Adapter = uid2Adapter;
        }

        public string GetOrCreateIfaToken(IRequestCookieCollection cookieContainer, string metricPrefix)
        {
            if (_cookieHelper.TryGetIdentifierForAdvertisingCookie(cookieContainer, out var ifaToken))
            {
                _metricHelper.SendCounterMetric($"{metricPrefix}.reuse");
            }
            else
            {
                // Generate a random guid token for an anonymous user
                ifaToken = GenerateRandomGuid();
                _metricHelper.SendCounterMetric($"{metricPrefix}.ok");
            }

            return ifaToken;
        }

        public async Task<string> TryGetUid2TokenAsync(IResponseCookies cookieContainer, string email, string metricPrefix)
        {
            var uid2Token = await _uid2Adapter.GetId(email);

            if (string.IsNullOrEmpty(uid2Token))
            {
                _metricHelper.SendCounterMetric($"{metricPrefix}.error.no_token");
            }
            else
            {
                _cookieHelper.SetUid2AdvertisingCookie(cookieContainer, uid2Token);

                _metricHelper.SendCounterMetric($"{metricPrefix}.ok");
            }

            return uid2Token;
        }
        private string GenerateRandomGuid() => Guid.NewGuid().ToString();
    }
}
