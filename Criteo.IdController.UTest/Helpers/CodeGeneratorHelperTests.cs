using Criteo.IdController.Helpers;
using NUnit.Framework;

namespace Criteo.IdController.UTest.Helpers
{
    [TestFixture]
    public class CodeGeneratorHelperTests
    {
        private const int _codeLength = 6;

        private ICodeGeneratorHelper _codeGeneratorHelper;

        [SetUp]
        public void SetUp()
        {
            _codeGeneratorHelper = new CodeGeneratorHelper();
        }

        [Test]
        public void FixedLengthCodeTest()
        {
            var code = _codeGeneratorHelper.GenerateRandomCode();

            Assert.IsNotNull(code);
            Assert.AreEqual(_codeLength, code.Length);
        }

        [Test]
        public void RandomGenerationTest()
        {
            var firstCode = _codeGeneratorHelper.GenerateRandomCode();
            var secondCode = _codeGeneratorHelper.GenerateRandomCode();

            Assert.IsNotNull(firstCode);
            Assert.IsNotNull(secondCode);
            Assert.AreNotEqual(firstCode, secondCode);
        }

        [Test]
        public void ValidCodeTest()
        {
            var validCode = "123456";
            var validation = _codeGeneratorHelper.IsValidCode(validCode);

            Assert.IsTrue(validation);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("-")]
        [TestCase("123")]
        [TestCase("1234567")]
        [TestCase("abcdef")]
        [TestCase("123abc")]
        public void InvalidCodeTest(string code)
        {
            var validation = _codeGeneratorHelper.IsValidCode(code);

            Assert.IsFalse(validation);
        }
    }
}
