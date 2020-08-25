using Criteo.IdController.Controllers;
using NUnit.Framework;

namespace Criteo.IdController.UTest
{
    [TestFixture]
    public class HelloWorldControllerTests
    {
        private HelloWorldController _helloWorldController;

        [SetUp]
        public void Setup()
        {
            _helloWorldController = new HelloWorldController();
        }

        [Test]
        public void TestHelloWorld()
        {
            var response = _helloWorldController.Get();

            Assert.AreEqual("Hello World", response);
        }
    }
}