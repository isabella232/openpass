using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPass.IdController.Models
{
    public class UserPreferences
    {
        public IList<string> DomainsWithConsent { get; set; }

        public bool Consent(string domain)
        {
            return DomainsWithConsent.Contains(domain);
        }
    }
}
