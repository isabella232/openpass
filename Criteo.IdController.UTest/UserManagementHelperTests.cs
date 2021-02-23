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
    }
}
