using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using OpenPass.IdController.Helpers;
using OpenPass.IdController.Helpers.Adapters;
using OpenPass.IdController.Helpers.Configuration;
using OpenPass.IdController.Models.Configuration;

namespace OpenPass.IdController.UTest.Helpers.Adapters
{
    [TestFixture]
    public class Uid2HelperTests
    {
        private Mock<IMetricHelper> _metricHelperMock;
        private Mock<HttpMessageHandler> _messageHandlerMock;
        private Mock<IConfigurationManager> _configurationManager;
        private Uid2Adapter _uid2Helper;

        [SetUp]
        public void SetUp()
        {
            _metricHelperMock = new Mock<IMetricHelper>();
            _messageHandlerMock = new Mock<HttpMessageHandler>();
            _configurationManager = new Mock<IConfigurationManager>();
            var uid2ConfigurationModel = new Uid2Configuration
            {
                ApiKey = "1234567890",
                Endpoint = "http://test.com"
            };
            _configurationManager.Setup(x => x.Uid2Configuration).Returns(uid2ConfigurationModel);

            var httpClient = new HttpClient(_messageHandlerMock.Object);

            _uid2Helper = new Uid2Adapter(httpClient, _metricHelperMock.Object, _configurationManager.Object);
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task TestInvalidEmail(string email)
        {
            // Arrange & Act
            var token = await _uid2Helper.GetId(email);

            // Assert
            Assert.IsNull(token);
            _messageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public async Task TestHttpRequestPerformed()
        {
            // Arrange
            var email = "email@example.com";
            var encodedEmail = HttpUtility.UrlEncode(email);
            MockHttpResponse(HttpStatusCode.OK);

            // Act
            await _uid2Helper.GetId(email);

            // Assert
            _messageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(
                    r => r.Method == HttpMethod.Get && r.RequestUri.AbsoluteUri.Contains(encodedEmail)),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public async Task TestSuccessfulCreation()
        {
            // Arrange
            var email = "email@example.com";
            var expectedToken = "FreshUID2token";
            MockHttpResponse(HttpStatusCode.OK, expectedToken);

            // Act
            var token = await _uid2Helper.GetId(email);

            // Assert
            Assert.AreEqual(expectedToken, token);
        }

        [TestCase(HttpStatusCode.NotFound)]
        [TestCase(HttpStatusCode.ServiceUnavailable)]
        [TestCase(HttpStatusCode.InternalServerError)]
        public async Task TestServiceUnavailable(HttpStatusCode statusCode)
        {
            // Arrange
            MockHttpResponse(statusCode);

            // Act
            var token = await _uid2Helper.GetId("email@example.com");

            // Assert
            Assert.IsNull(token);
        }

        #region Helpers

        private void MockHttpResponse(HttpStatusCode statusCode, string token = "token")
        {
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(token)
            };

            _messageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);
        }

        #endregion Helpers
    }
}
