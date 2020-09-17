using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Criteo.IdController.Helpers
{
    public interface IUserManagementHelper
    {
        Guid GenerateIfa();
        bool TryGetOrCreateIfaMappingFromPii(string pii, out Guid ifa);
    }

    public class UserManagementHelper : IUserManagementHelper
    {
        // Store the mapping: pii -> ifa <String, Guid>
        private readonly ConcurrentDictionary<string, Guid> _piiToIfaMapping = new ConcurrentDictionary<string, Guid>();

        public Guid GenerateIfa()
        {
            return Guid.NewGuid();
        }

        // This method always returns and IFA in reference to be used by the client,
        // though the returned boolean determines if the mapping was stored successfully
        public bool TryGetOrCreateIfaMappingFromPii(string pii, out Guid ifa)
        {
            if (_piiToIfaMapping.TryGetValue(pii, out ifa))
                return true;

            ifa = GenerateIfa();
            return _piiToIfaMapping.TryAdd(pii, ifa);
        }
    }
}
