using System.Threading.Tasks;

namespace Criteo.IdController.Helpers.Adapters
{
    public interface IIdentifierAdapter
    {
        Task<string> GetId(string pii);
    }
}
