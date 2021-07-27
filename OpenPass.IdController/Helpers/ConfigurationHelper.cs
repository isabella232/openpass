using Criteo.ConfigAsCode;
using Criteo.Configuration.Repository.IdController;

namespace OpenPass.IdController.Helpers
{
    public interface IConfigurationHelper
    {
        /// <summary>
        /// A partner can integrate OpenPass with it CMP. To help
        /// to manage those integrations, we use a feature flag
        /// which include the partner domain as a dimension.
        /// </summary>
        bool CmpIntegrationEnable(string domain);

        double EmitGlupsRatio(string domain = "");

        bool EnableOtp { get; }
    }

    public class ConfigurationHelper : IConfigurationHelper
    {
        private readonly IConfigAsCodeService _cacService;

        // CaC parameters
        private EmitGlupsDomainRatio.ParameterImpl _emitGlupsRatioParameter;

        private EnableOTP.ParameterImpl _enableOtpParameter;
        private EnableOTP.Query _enableOtpQuery;

        public ConfigurationHelper(IConfigAsCodeService cacService)
        {
            _cacService = cacService;

            // CaC parameters
            _emitGlupsRatioParameter = EmitGlupsDomainRatio.CreateParameter(_cacService);

            _enableOtpParameter = EnableOTP.CreateParameter(_cacService);
            _enableOtpQuery = new EnableOTP.Query();
        }

        #region Parameter-specific code

        public bool CmpIntegrationEnable(string domain)
        {
            return false;
        }

        public double EmitGlupsRatio(string domain = "")
        {
            // This CaC is mainly used to get the ratio per domain to glup events
            var query = new EmitGlupsDomainRatio.Query(domain);
            return _emitGlupsRatioParameter?.Get(query, 1.0) ?? 1.0;
        }

        public bool EnableOtp => _enableOtpParameter?.Get(_enableOtpQuery, true) ?? true;

        #endregion Parameter-specific code
    }
}
