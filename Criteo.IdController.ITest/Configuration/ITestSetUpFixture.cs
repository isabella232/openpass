using System;
using System.Net.Http;
using Criteo.Testing.Isolation;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace Criteo.IdController.ITest
{
    [SetUpFixture]
    public class ITestSetUpFixture
    {
        public static InProcessTestHelper InProcessTestHelper { get; private set; }
        public static HttpClient Client { get; private set; }

        [OneTimeSetUp]
        public static void RunBeforeTheFirstTest()
        {
            try
            {
                InProcessTestHelper = new InProcessTestHelper(typeof(ITestSetUpFixture));
                Client = InProcessTestHelper
                    .CreateInProcessAspNetCoreFactory<Program>(Program.ConfigureApp)
                    .CreateClient(new WebApplicationFactoryClientOptions()
                    {
                        HandleCookies = false
                    });
            }
            catch (Exception e)
            {
                // Workaround for https://github.com/nunit/nunit/issues/2466.
                Assert.Fail("An exception was thrown during OneTimeSetUp: " + e);
            }
        }

        [OneTimeTearDown]
        public void RunAfterTheLastTest()
        {
            try
            {
                Client.Dispose();
            }
            finally
            {
                InProcessTestHelper.Dispose();
            }
        }
    }
}
