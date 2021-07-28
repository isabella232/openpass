namespace OpenPass.IdController.Helpers
{
    public interface IConfigurationHelper
    {/// <summary>
     /// A partner can integrate OpenPass with it CMP. To help
     /// to manage those integrations, we use a feature flag
     /// which include the partner domain as a dimension.
     /// </summary>
        bool CmpIntegrationEnable(string domain);
    }

    public class ConfigurationHelper : IConfigurationHelper
    {
        public ConfigurationHelper()
        {
        }

        #region Parameter-specific code

        public bool CmpIntegrationEnable(string domain)
        {
            return false;
        }

        #endregion Parameter-specific code
    }
}
