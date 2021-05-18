using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using OpenPass.IdController.Helpers.Configuration;

namespace OpenPass.IdController.Helpers.Adapters
{
    public class Uid2Adapter : IIdentifierAdapter, IDisposable
    {
        private const string _uid2EmailParameterName = "email"; // Expected: email=email@example.com
        private const string _prefix = "uid2adapter";

        private readonly HttpClient _httpClient;
        private readonly IMetricHelper _metricHelper;
        private readonly IConfigurationManager _configurationManager;

        public Uid2Adapter(HttpClient httpClient, IMetricHelper metricHelper, IConfigurationManager configurationManager)
        {
            _httpClient = httpClient;
            _metricHelper = metricHelper;
            _configurationManager = configurationManager;
        }

        public Uid2Adapter(IMetricHelper metricHelper, IConfigurationManager configurationManager)
            : this(new HttpClient(), metricHelper, configurationManager)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", configurationManager.Uid2Configuration.ApiKey);
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

        private Uri GetRequestUri(string email) =>
            new Uri(_configurationManager.Uid2Configuration.Endpoint).AddQueryParameter(_uid2EmailParameterName, email);
    }
}
