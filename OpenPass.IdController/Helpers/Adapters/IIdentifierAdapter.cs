using System.Threading.Tasks;

namespace OpenPass.IdController.Helpers.Adapters
{
    public interface IIdentifierAdapter
    {
        Task<string> GetId(string pii);
    }
}
