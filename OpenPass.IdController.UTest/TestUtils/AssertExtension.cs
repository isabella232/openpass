using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace OpenPass.IdController.UTest.TestUtils
{
    static class AssertExtension
    {
        public static void AreEquivalent(ObjectResult expected, IActionResult actual)
        {
            var actualResult = actual as ObjectResult;
            Assert.AreEqual(expected.GetType(), actual.GetType());
            Assert.AreEqual(expected.StatusCode, actualResult.StatusCode);
            Assert.AreEqual(expected.Value, actualResult.Value);
        }
    }
}
