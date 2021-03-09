using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Criteo.IdController.Helpers.Adapters
{
    public class Uid2Adapter : IIdentifierAdapter, IDisposable
    {
        private const string _criteoApiKey = "";
        private const string _uid2MappingEndpoint = "https://integ.uidapi.com/identity/map";
        private const string _uid2EmailParameterName = "email"; // Expected: email=email@example.com

        private readonly HttpClient _httpClient;

        public Uid2Adapter(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Uid2Adapter()
            : this(new HttpClient())
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _criteoApiKey);
        }

        public async Task<string> GetId(string pii)
        {
            if (string.IsNullOrEmpty(pii))
            {
                // TODO: Emit metric
                return null;
            }

            var requestUri = GetRequestUri(pii);
            var response = await _httpClient.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                // TODO: Emit metric
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
