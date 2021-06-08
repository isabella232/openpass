using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OpenPass.IdController.Helpers.Adapters;
using OpenPass.IdController.Models.Tracking;
using static Criteo.Glup.IdController.Types;

namespace OpenPass.IdController.Helpers
{
    public interface IIdentifierHelper
    {
        string GetOrCreateIfaToken(IRequestCookieCollection cookieContainer, TrackingModel trackingModel, string metricPrefix, string originHost, string userAgent);

        Task<string> TryGetUid2TokenAsync(IResponseCookies cookieContainer, TrackingModel trackingModel, EventType eventType, string originHost, string userAgent, string email, string metricPrefix);
    }

    public class IdentifierHelper : IIdentifierHelper
    {
        private readonly ICookieHelper _cookieHelper;
        private readonly IMetricHelper _metricHelper;
        private readonly IGlupHelper _glupHelper;
        private readonly IIdentifierAdapter _uid2Adapter;

        public IdentifierHelper(
            IMetricHelper metricHelper,
            ICookieHelper cookieHelper,
            IGlupHelper glupHelper,
            IIdentifierAdapter uid2Adapter)
        {
            _metricHelper = metricHelper;
            _cookieHelper = cookieHelper;
            _glupHelper = glupHelper;
            _uid2Adapter = uid2Adapter;
        }

        public string GetOrCreateIfaToken(IRequestCookieCollection cookieContainer, TrackingModel trackingModel, string metricPrefix, string originHost, string userAgent)
        {
            if (_cookieHelper.TryGetIdentifierForAdvertisingCookie(cookieContainer, out var ifaToken))
            {
                _metricHelper.SendCounterMetric($"{metricPrefix}.reuse");
                _glupHelper.EmitGlup(EventType.ReuseIfa, originHost, userAgent, trackingModel);
            }
            else
            {
                // Generate a random guid token for an anonymous user
                ifaToken = GenerateRandomGuid();
                _metricHelper.SendCounterMetric($"{metricPrefix}.ok");
                _glupHelper.EmitGlup(EventType.NewIfa, originHost, userAgent, trackingModel);
            }

            return ifaToken;
        }

        public async Task<string> TryGetUid2TokenAsync(IResponseCookies cookieContainer, TrackingModel trackingModel, EventType eventType, string originHost, string userAgent, string email, string metricPrefix)
        {
            _glupHelper.EmitGlup(eventType, originHost, userAgent, trackingModel);

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
