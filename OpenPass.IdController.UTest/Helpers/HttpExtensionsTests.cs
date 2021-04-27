using System;
using NUnit.Framework;
using OpenPass.IdController.Helpers;

namespace OpenPass.IdController.UTest.Helpers
{
    [TestFixture]
    public class HttpExtensionsTests
    {
        [Test]
        public void TestSimpleQueryParameterString()
        {
            // Arrange && Act
            var uri = new Uri("http://example.com").AddQueryParameter("key", "value");

            // Assert
            Assert.AreEqual("http://example.com/?key=value", uri.ToString());
        }

        [Test]
        public void TestMultipleQueryParameterString()
        {
            // Arrange && Act
            var uri = new Uri("http://example.com")
                .AddQueryParameter("first", "value1")
                .AddQueryParameter("second", "value2");

            // Assert
            Assert.AreEqual("http://example.com/?first=value1&second=value2", uri.ToString());
        }

        [Test]
        public void TestSinglePathQueryParameterString()
        {
            // Arrange && Act
            var uri = new Uri("http://example.com/path").AddQueryParameter("key", "value");

            // Assert
            Assert.AreEqual("http://example.com/path?key=value", uri.ToString());
        }

        [Test]
        public void TestMultiplePathQueryParameterString()
        {
            // Arrange && Act
            var uri = new Uri("http://example.com/multiple/path").AddQueryParameter("key", "value");

            // Assert
            Assert.AreEqual("http://example.com/multiple/path?key=value", uri.ToString());
        }
    }
}
