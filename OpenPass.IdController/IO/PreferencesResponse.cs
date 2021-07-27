using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPass.IdController.Models
{
    public class PreferencesResponse : IEquatable<PreferencesResponse>
    {
        public DomainPreferences DomainPreferences { get; set; }

        public UserPreferencesForDomain UserPreferences { get; set; }

        public bool Equals(PreferencesResponse other)
        {
            return other != null
                && DomainPreferences.Equals(other.DomainPreferences)
                && UserPreferences.Equals(other.UserPreferences);
        }
    }

    public class DomainPreferences : IEquatable<DomainPreferences>
    {
        public string Name { get; set; }
        public bool CmpIntegrationEnable { get; set; }

        public bool Equals(DomainPreferences other)
        {
            return other != null
                && Name == other.Name
                && CmpIntegrationEnable == other.CmpIntegrationEnable;
        }
    }

    /// <summary>
    /// Represents the User Preferences from the perspective of
    /// a partner.
    /// </summary>
    public class UserPreferencesForDomain : IEquatable<UserPreferencesForDomain>
    {
        /// <summary>
        /// Express the fact that the user consent to use OpenPass with
        /// the website of the partner or not.
        /// </summary>
        public bool Consent { get; set; }

        public bool Equals(UserPreferencesForDomain other)
        {
            return other != null
                && Consent == other.Consent;
        }
    }
}
