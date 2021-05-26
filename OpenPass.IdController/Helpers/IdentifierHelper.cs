using System;
using Microsoft.AspNetCore.Http;
using static Criteo.Glup.IdController.Types;

namespace OpenPass.IdController.Helpers
{
    public interface IIdentifierHelper
    {
        string GetOrCreateIfaToken(IRequestCookieCollection cookieContainer, string metricPrefix, string originHost, string userAgent);
    }

    public class IdentifierHelper : IIdentifierHelper
    {
        private readonly ICookieHelper _cookieHelper;
        private readonly IMetricHelper _metricHelper;
        private readonly IGlupHelper _glupHelper;

        public IdentifierHelper(IMetricHelper metricHelper, ICookieHelper cookieHelper, IGlupHelper glupHelper)
        {
            _metricHelper = metricHelper;
            _cookieHelper = cookieHelper;
            _glupHelper = glupHelper;
        }

        public string GetOrCreateIfaToken(IRequestCookieCollection cookieContainer, string metricPrefix, string originHost, string userAgent)
        {
            if (_cookieHelper.TryGetIdentifierForAdvertisingCookie(cookieContainer, out var ifaToken))
            {
                _metricHelper.SendCounterMetric($"{metricPrefix}.reuse");
                _glupHelper.EmitGlup(EventType.ReuseIfa, originHost, userAgent);
            }
            else
            {
                // Generate a random guid token for an anonymous user
                ifaToken = GenerateRandomGuid();
                _metricHelper.SendCounterMetric($"{metricPrefix}.ok");
                _glupHelper.EmitGlup(EventType.NewIfa, originHost, userAgent);
            }

            return ifaToken;
        }

        private string GenerateRandomGuid() => Guid.NewGuid().ToString();
    }
}
