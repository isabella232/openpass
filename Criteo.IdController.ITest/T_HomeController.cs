using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Criteo.IdController.ITest
{
    public class T_HomeController
    {
        private static HttpClient Client => ITestSetUpFixture.Client;

        [Test]
        public async Task TestGet()
        {
            var response = await Client.GetAsync("");

            response.EnsureSuccessStatusCode();
            string htmlContent = await response.Content.ReadAsStringAsync();
            Assert.True(htmlContent.Contains("Home"), @"Wrong HTML content: {htmlContent}");
        }
    }
}
