using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using HttpCookie = Microsoft.Net.Http.Headers.SetCookieHeaderValue;

namespace Criteo.IdController.ITest
{
    [TestFixture]
    internal class IfaControllerTests
    {
        private readonly string IfaCookieName = "ifa";
        private static HttpClient _httpClient => ITestSetUpFixture.Client;
        private const string _controllerEndpoint = "api/ifa";

        [Test]
        public async Task TestCreateIfa()
        {
            var response = await _httpClient.GetAsync($"{_controllerEndpoint}/get");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
            Assert.NotNull(cookies);

            var cookieList = HttpCookie.ParseList((IList<string>) cookies);
            Assert.AreEqual(1, cookieList.Count);

            var cookieIfa = cookieList.First();
            Assert.AreEqual(IfaCookieName, cookieIfa.Name);

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            Assert.NotNull(json[IfaCookieName]);

            var ifaFromContent = json[IfaCookieName].ToString();

            Assert.AreEqual(ifaFromContent, cookieIfa.Value);
        }

        [Test]
        public async Task TestGetIfa()
        {
            var ifaUserSide = "62BB805D-9E63-4FE0-AB7D-4B514F86D63C";

            var message = new HttpRequestMessage(HttpMethod.Get, $"{_controllerEndpoint}/get");
            message.Headers.Add("Cookie", $"{IfaCookieName}={ifaUserSide}");

            var response = await _httpClient.SendAsync(message);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
            Assert.NotNull(cookies);

            var cookieList = HttpCookie.ParseList((IList<string>) cookies);
            Assert.AreEqual(1, cookieList.Count);

            var cookieIfa = cookieList.First();
            Assert.AreEqual(IfaCookieName, cookieIfa.Name);

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            Assert.NotNull(json[IfaCookieName]);

            var ifaFromContent = json[IfaCookieName].ToString();

            Assert.AreEqual(ifaFromContent, cookieIfa.Value);
            Assert.AreEqual(ifaUserSide, ifaFromContent);
        }

        [Test]
        public async Task TestDeleteIfa()
        {
            var response = await _httpClient.GetAsync($"{_controllerEndpoint}/delete");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
            Assert.NotNull(cookies);

            var cookieList = HttpCookie.ParseList((IList<string>) cookies);
            Assert.AreEqual(1, cookieList.Count);

            var cookieIfa = cookieList.First();
            Assert.AreEqual(IfaCookieName, cookieIfa.Name);

            Assert.IsEmpty(cookieIfa.Value.ToString());
        }
    }
}
