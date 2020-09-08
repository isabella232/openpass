using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Criteo.IdController.ITest
{
    [TestFixture]
    class IfaControllerTests
    {
        private static HttpClient _httpClient => ITestSetUpFixture.Client;
        private const string _controllerEndpoint = "api/ifa";

        [Ignore("Waiting for the IFA controller to run this test")]
        public async Task TestGenerateIfa()
        {
            var response = await _httpClient.GetAsync($"{_controllerEndpoint}/generate");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var cookie = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
            Assert.NotNull(cookie);
        }

        [Test] // TODO: Remove this test when one is actually implemented, this is because at least one test is needed for the CI
        public void DummyTest()
        {
            Assert.Pass();
        }
    }
}
