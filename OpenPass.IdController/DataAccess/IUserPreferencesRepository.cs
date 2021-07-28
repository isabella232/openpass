using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OpenPass.IdController.Models;

namespace OpenPass.IdController.DataAccess
{
    public interface IUserPreferencesRepository
    {
        Task<UserPreferences> GetPreferences(string ifa);
    }

    /// <summary>
    /// Implementation for mocking the real storage while implementing it.
    /// </summary>
    public class StaticUserPreferencesRepository : IUserPreferencesRepository
    {
        public Task<UserPreferences> GetPreferences(string ifa)
        {
            var hash = ifa.GetHashCode();
            var domainCounter = hash % 2 + 1;
            var domains = new List<string> { $"hash{hash.ToString()}.com", $"partner{domainCounter}.com" };
            var preferences = new UserPreferences() { DomainsWithConsent = domains };
            return Task.FromResult(preferences);
        }
    }
}
