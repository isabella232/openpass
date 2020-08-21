using System.Net;
using Criteo.IdController.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Criteo.IdController.UTest
{
    [TestFixture]
    public class WidgetControllerTests
    {
        private WidgetController _widgetController;

        [SetUp]
        public void Setup()
        {
            _widgetController = new WidgetController();
        }

        [Test]
        public void TestWidget()
        {
            var response = _widgetController.Get();

            Assert.IsNotNull(response);
            Assert.AreEqual("text/html", response.ContentType);
            Assert.AreEqual((int) HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotEmpty(response.Content);
        }
    }
}