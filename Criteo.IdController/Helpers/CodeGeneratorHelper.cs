using System;
using System.Linq;

namespace Criteo.IdController.Helpers
{
    public interface ICodeGeneratorHelper
    {
        string GenerateRandomCode();
        bool IsValidCode(string code);
    }

    public class CodeGeneratorHelper : ICodeGeneratorHelper
    {
        private const string _codeCharacters = "1234567890";
        private const int _codeLength = 6;

        private readonly Random _randomGenerator;

        public CodeGeneratorHelper()
        {
            _randomGenerator = new Random();
        }

        public string GenerateRandomCode()
        {
            return new string(Enumerable.Repeat(_codeCharacters, _codeLength)
                .Select(s => s[_randomGenerator.Next(s.Length)]).ToArray());
        }

        public bool IsValidCode(string code)
        {
            return !string.IsNullOrEmpty(code)
                && code.Length == _codeLength
                && code.All(c => _codeCharacters.Contains(c));
        }
    }
}
