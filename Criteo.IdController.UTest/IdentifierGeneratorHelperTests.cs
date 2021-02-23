using Criteo.IdController.Helpers;
using NUnit.Framework;

namespace Criteo.IdController.UTest
{
    [TestFixture]
    public class IdentifierGeneratorHelperTests
    {
        private IIdentifierGeneratorHelper _identifierGeneratorHelper;

        [SetUp]
        public void SetUp()
        {
            _identifierGeneratorHelper = new IdentifierGeneratorHelper();
        }

        [Test]
        public void TestGenerateNewIfaEveryTime()
        {
            var firstIfa = _identifierGeneratorHelper.GenerateIdentifier();
            var secondIfa = _identifierGeneratorHelper.GenerateIdentifier();

            Assert.AreNotEqual(firstIfa, secondIfa);
        }
    }
}
