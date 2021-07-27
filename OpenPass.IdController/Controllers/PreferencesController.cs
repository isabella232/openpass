using System.Threading.Tasks;
using Criteo.Protobuf.UniversalCatalog;
using Microsoft.AspNetCore.Mvc;
using OpenPass.IdController.DataAccess;
using OpenPass.IdController.Helpers;
using OpenPass.IdController.Models;

namespace OpenPass.IdController.Controllers
{
    /// <summary>
    /// Endpoints related to the preferences. It is both
    /// partner and user preferences.
    /// </summary>
    [Route("api/[controller]")]
    public class PreferencesController : Controller
    {
        private const string _metricPrefix = "preferences";
        private static readonly string _perDomainBadRequestMetricPrefix = $"{_metricPrefix}.per_domain.bad_request";
        private static readonly string _perDomainBadRequestNoOriginMetric = $"{_perDomainBadRequestMetricPrefix}.no_origin";
        private static readonly string _perDomainBadRequestNoIfaMetric = $"{_perDomainBadRequestMetricPrefix}.no_ifa";

        private readonly IConfigurationHelper _configurationHelper;
        private readonly IMetricHelper _metricRegistry;
        private readonly IUserPreferencesRepository _userPreferencesRepository;

        public PreferencesController(
            IConfigurationHelper configurationHelper,
            IMetricHelper metricRegistry,
            IUserPreferencesRepository userPreferencesRepository)
        {
            _configurationHelper = configurationHelper;
            _metricRegistry = metricRegistry;
            _userPreferencesRepository = userPreferencesRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPreferencesPerDomain(
            [FromHeader(Name = "x-origin-host")] string domain,
            [FromQuery(Name = "ifa")] string ifa)
        {
            if (!ValidatePreferencesPerDomainInputs(domain, ifa, out var badRequestMessage))
                return BadRequest(badRequestMessage);

            var response = await GetPreferencesResponsePerDomain(domain, ifa);
            return Ok(response);
        }

        private async Task<PreferencesResponse> GetPreferencesResponsePerDomain(string domain, string ifa)
        {
            var cmpIntegrationEnable = _configurationHelper.CmpIntegrationEnable(domain);
            var preferences = await _userPreferencesRepository.GetPreferences(ifa);
            return new PreferencesResponse()
            {
                DomainPreferences = new DomainPreferences()
                {
                    Name = domain,
                    CmpIntegrationEnable = cmpIntegrationEnable
                },
                UserPreferences = new UserPreferencesForDomain()
                {
                    Consent = preferences.Consent(domain)
                }
            };
        }

        private bool ValidatePreferencesPerDomainInputs(string domain, string ifa, out string badRequestMessage)
        {
            if (string.IsNullOrEmpty(domain) && string.IsNullOrEmpty(ifa))
            {
                _metricRegistry.SendCounterMetric(_perDomainBadRequestNoOriginMetric);
                _metricRegistry.SendCounterMetric(_perDomainBadRequestNoIfaMetric);
                badRequestMessage = "No x-origin-host; No ifa";
                return false;
            }

            if (string.IsNullOrEmpty(domain))
            {
                _metricRegistry.SendCounterMetric(_perDomainBadRequestNoOriginMetric);
                badRequestMessage = "No x-origin-host";
                return false;
            }

            if (string.IsNullOrEmpty(ifa))
            {
                _metricRegistry.SendCounterMetric(_perDomainBadRequestNoIfaMetric);
                badRequestMessage = "No ifa";
                return false;
            }

            badRequestMessage = default;
            return true;
        }
    }
}
