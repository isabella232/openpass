using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Criteo.IdController.ITest
{
    public class T_HelloWorldController
    {
        private static HttpClient Client => ITestSetUpFixture.Client;

        [Test]
        public async Task TestGet()
        {
            var response = await Client.GetAsync("api/helloworld");

            response.EnsureSuccessStatusCode();
        }
    }
}
