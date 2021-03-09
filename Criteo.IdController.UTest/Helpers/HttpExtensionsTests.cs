using System;
using Criteo.IdController.Helpers;
using NUnit.Framework;

namespace Criteo.IdController.UTest.Helpers
{
    [TestFixture]
    public class HttpExtensionsTests
    {
        [Test]
        public void TestSimpleQueryParameterString()
        {
            var uri = new Uri("http://example.com").AddQueryParameter("key", "value");
            Assert.AreEqual("http://example.com/?key=value", uri.ToString());
        }

        [Test]
        public void TestMultipleQueryParameterString()
        {
            var uri = new Uri("http://example.com")
                .AddQueryParameter("first", "value1")
                .AddQueryParameter("second", "value2");

            Assert.AreEqual("http://example.com/?first=value1&second=value2", uri.ToString());
        }

        [Test]
        public void TestSinglePathQueryParameterString()
        {
            var uri = new Uri("http://example.com/path").AddQueryParameter("key", "value");
            Assert.AreEqual("http://example.com/path?key=value", uri.ToString());
        }

        [Test]
        public void TestMultiplePathQueryParameterString()
        {
            var uri = new Uri("http://example.com/multiple/path").AddQueryParameter("key", "value");
            Assert.AreEqual("http://example.com/multiple/path?key=value", uri.ToString());
        }
    }
}
