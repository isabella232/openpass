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
        public void Placeholder()
        {
            Assert.Pass();
        }
    }
}
