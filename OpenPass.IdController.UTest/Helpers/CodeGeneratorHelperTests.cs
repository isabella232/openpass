using NUnit.Framework;
using OpenPass.IdController.Helpers;

namespace OpenPass.IdController.UTest.Helpers
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
            // Arrange & Act
            var code = _codeGeneratorHelper.GenerateRandomCode();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(code);
                Assert.AreEqual(_codeLength, code.Length);
            });
        }

        [Test]
        public void RandomGenerationTest()
        {
            // Arrange & Act
            var firstCode = _codeGeneratorHelper.GenerateRandomCode();
            var secondCode = _codeGeneratorHelper.GenerateRandomCode();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(firstCode);
                Assert.IsNotNull(secondCode);
                Assert.AreNotEqual(firstCode, secondCode);
            });
        }

        [Test]
        public void ValidCodeTest()
        {
            // Arrange
            var validCode = "123456";

            // Act
            var validation = _codeGeneratorHelper.IsValidCode(validCode);

            // Assert
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
            // Arrange & Act
            var validation = _codeGeneratorHelper.IsValidCode(code);

            // Assert
            Assert.IsFalse(validation);
        }
    }
}
