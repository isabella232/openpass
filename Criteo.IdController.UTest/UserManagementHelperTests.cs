using System;
using System.Collections.Generic;
using System.Text;
using Criteo.IdController.Helpers;
using NUnit.Framework;

namespace Criteo.IdController.UTest
{
    [TestFixture]
    public class UserManagementHelperTests
    {
        private IUserManagementHelper _userManagementHelper;

        [SetUp]
        public void SetUp()
        {
            _userManagementHelper = new UserManagementHelper();
        }

        [Test]
        public void TestGenerateNewIfaEveryTime()
        {
            var firstIfa = _userManagementHelper.GenerateIfa();
            var secondIfa = _userManagementHelper.GenerateIfa();

            Assert.AreNotEqual(firstIfa, secondIfa);
        }

        [Test]
        public void TestReturnSameIfaForSamePii()
        {
            var pii = "pii";
            var firstResult = _userManagementHelper.TryGetOrCreateIfaMappingFromPii(pii, out var firstIfa);
            var secondResult = _userManagementHelper.TryGetOrCreateIfaMappingFromPii(pii, out var secondIfa);

            Assert.IsTrue(firstResult);
            Assert.IsTrue(secondResult);
            Assert.AreEqual(firstIfa, secondIfa);
        }

        [Test]
        public void TestReturnDifferentIfaForDifferentPii()
        {
            var firstResult = _userManagementHelper.TryGetOrCreateIfaMappingFromPii("pii1", out var firstIfa);
            var secondResult = _userManagementHelper.TryGetOrCreateIfaMappingFromPii("pii2", out var secondIfa);

            Assert.IsTrue(firstResult);
            Assert.IsTrue(secondResult);
            Assert.AreNotEqual(firstIfa, secondIfa);
        }
    }
}
