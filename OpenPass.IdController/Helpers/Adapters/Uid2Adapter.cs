using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OpenPass.IdController.Helpers.Adapters
{
    public class Uid2Adapter : IIdentifierAdapter, IDisposable
    {
        private const string _criteoApiKey = "";
        private const string _uid2MappingEndpoint = "https://integ.uidapi.com/identity/map";
        private const string _uid2EmailParameterName = "email"; // Expected: email=email@example.com
        private const string _prefix = "uid2adapter";

        private readonly HttpClient _httpClient;
        private readonly IMetricHelper _metricHelper;

        public Uid2Adapter(HttpClient httpClient, IMetricHelper metricHelper)
        {
            _httpClient = httpClient;
            _metricHelper = metricHelper;
        }

        public Uid2Adapter(IMetricHelper metricHelper)
            : this(new HttpClient(), metricHelper)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _criteoApiKey);
        }

        public async Task<string> GetId(string emailIdentifier)
        {
            var prefix = $"{_prefix}.getid";
            if (string.IsNullOrEmpty(emailIdentifier))
            {
                _metricHelper.SendCounterMetric($"{prefix}.invalid.emailIdentifier");
                return null;
            }

            var requestUri = GetRequestUri(emailIdentifier);
            var response = await _httpClient.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                _metricHelper.SendCounterMetric($"{prefix}.invalid.response");
                return null;
            }

            var token = await response.Content.ReadAsStringAsync();
            return token;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        private static Uri GetRequestUri(string email) =>
            new Uri(_uid2MappingEndpoint).AddQueryParameter(_uid2EmailParameterName, email);
    }
}
