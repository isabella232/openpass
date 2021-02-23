using System;

namespace Criteo.IdController.Helpers
{
    public interface IIdentifierGeneratorHelper
    {
        Guid GenerateIdentifier();
    }

    public class IdentifierGeneratorHelper : IIdentifierGeneratorHelper
    {
        public Guid GenerateIdentifier()
        {
            return Guid.NewGuid();
        }
    }
}
