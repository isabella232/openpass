using Criteo.ConfigAsCode;
using Criteo.Configuration.Repository.IdController;

namespace Criteo.IdController.Helpers
{
    public interface IConfigurationHelper
    {
        double EmitGlupsRatio(string domain = "");
    }

    public class ConfigurationHelper : IConfigurationHelper
    {
        private readonly IConfigAsCodeService _cacService;

        // CaC parameters
        private EmitGlupsDomainRatio.ParameterImpl _emitGlupsRatioParameter;

        public ConfigurationHelper(IConfigAsCodeService cacService)
        {
            _cacService = cacService;

            // CaC parameters
            _emitGlupsRatioParameter = EmitGlupsDomainRatio.CreateParameter(_cacService);
        }

        #region Parameter-specific code
        public double EmitGlupsRatio(string domain = "")
        {
            // This CaC is mainly used to get the ratio per domain to glup events
            var query = new EmitGlupsDomainRatio.Query(domain);
            return _emitGlupsRatioParameter?.Get(query, 0.0) ?? 0.0;
        }
        #endregion
    }
}
