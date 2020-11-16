using System.Threading.Tasks;
using Criteo.ConfigAsCode;
using Criteo.Configuration.Repository.IdController;
using Criteo.UserIdentification;
using Criteo.UserIdentification.Services.IdentityMapping;

namespace Criteo.IdController.Helpers
{
    internal interface IInternalMappingHelper
    {
        Task<CriteoId?> GetInternalCriteoId(CriteoId? criteoId);
        Task<LocalWebId?> GetInternalLocalWebId(LocalWebId? localWebId);
        Task<UserCentricAdId?> GetInternalUserCentricAdId(UserCentricAdId? userCentricAdId);
    }

    internal class InternalMappingHelper : IInternalMappingHelper
    {
        private readonly IConfigAsCodeService _cacService;
        private readonly IIdentityMapper _identityMapper;

        private readonly RevocableIdEnabledInIdController.ParameterImpl _revocableIdEnabledParameter;
        private readonly RevocableIdEnabledInIdController.Query _revocableIdEnabledQuery;

        internal InternalMappingHelper(IConfigAsCodeService cacService, IIdentityMapper identityMapper)
        {
            _cacService = cacService;
            _identityMapper = identityMapper;

            _revocableIdEnabledParameter = RevocableIdEnabledInIdController.CreateParameter(cacService);
            _revocableIdEnabledQuery = new RevocableIdEnabledInIdController.Query();
        }

        private bool RevocableIdEnabled => _revocableIdEnabledParameter?.Get(_revocableIdEnabledQuery, false) ?? false;

        #region Individual mappings
        public async Task<CriteoId?> GetInternalCriteoId(CriteoId? criteoId)
        {
            if (!RevocableIdEnabled)
                return criteoId;

            if (!criteoId.HasValue)
                return null;

            var externalIdentity = new UserIdentity(criteoId.Value);
            var identityMapping = await GetIdentityMapping(externalIdentity);

            var internalIdCopy = new CriteoId(identityMapping.InternalIdentity.InternalId.Value.ToGuid());

            return internalIdCopy;
        }

        public async Task<LocalWebId?> GetInternalLocalWebId(LocalWebId? localWebId)
        {
            if (!RevocableIdEnabled)
                return localWebId;

            var criteoId = localWebId?.CriteoId;

            if (!criteoId.HasValue)
                return null;

            var externalIdentity = new UserIdentity(criteoId.Value, new Consent(), localWebId.Value);
            var identityMapping = await GetIdentityMapping(externalIdentity);

            var internalLocalWebId = LocalWebId.Parse(identityMapping.InternalIdentity.InternalId.Value.ToGuid().ToString(), localWebId.Value.Domain);

            return internalLocalWebId;
        }

        public async Task<UserCentricAdId?> GetInternalUserCentricAdId(UserCentricAdId? userCentricAdId)
        {
            if (!RevocableIdEnabled)
                return userCentricAdId;

            if (!userCentricAdId.HasValue)
                return null;

            var criteoId = new CriteoId(userCentricAdId.Value.Value);

            var externalIdentity = new UserIdentity(criteoId, new Consent(), userCentricAdId.Value);
            var identityMapping = await GetIdentityMapping(externalIdentity);

            var internalUserCentricAdId = new UserCentricAdId(identityMapping.InternalIdentity.InternalId.Value.ToGuid());

            return internalUserCentricAdId;
        }
        #endregion

        #region Helpers
        private async Task<IdentityMappingResult> GetIdentityMapping(UserIdentity externalIdentity, bool roundtrip = true, bool creation = true)
        {
            // roundtrip is set to true by default because the origin is an external bundle -> ids already persisted
            // creation is set to true by default to start populating the IdentityMapperStorage with already-existing external ids
            return await _identityMapper.GetOrCreateInternalIdentity(externalIdentity, roundtrip, creation);
        }
        #endregion
    }
}
