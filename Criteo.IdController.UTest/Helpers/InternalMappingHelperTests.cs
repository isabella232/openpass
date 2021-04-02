using System;
using System.Threading.Tasks;
using Criteo.ConfigAsCode;
using Criteo.Configuration.Repository.IdController;
using Criteo.IdController.Helpers;
using Criteo.Identification.Schemas;
using Criteo.UserIdentification;
using Criteo.UserIdentification.Services.IdentityMapping;
using Moq;
using NUnit.Framework;
using CriteoId = Criteo.UserIdentification.CriteoId;

namespace Criteo.IdController.UTest.Helpers
{
    [TestFixture]
    public class IdentityMappingHelperTests
    {
        private ConfigAsCodeServiceMock _cacServiceMock;
        private Mock<IIdentityMapper> _identityMapperMock;

        [SetUp]
        public void Init()
        {
            _cacServiceMock = new ConfigAsCodeServiceMock();
            _identityMapperMock = new Mock<IIdentityMapper>();
        }

        [Theory]
        public async Task TestGetInternalCriteoId(bool revocable, bool isNull)
        {
            var criteoId = isNull ? (CriteoId?)null : CriteoId.New();
            var identityMappingHelper = GetMockedIdentificationBundleMappingHelper(revocable);
            var internalCriteoId = await identityMappingHelper.GetInternalCriteoId(criteoId);

            if (!revocable)
            {
                Assert.AreEqual(criteoId, internalCriteoId);
            }
            else
            {
                if (isNull)
                {
                    Assert.IsNull(internalCriteoId);
                }
                else
                {
                    Assert.IsNotNull(internalCriteoId);
                    Assert.AreNotEqual(criteoId, internalCriteoId);
                }
            }
        }

        [Theory]
        public async Task TestGetInternalLocalWebId(bool revocable, bool isNull)
        {
            var localWebId = isNull ? (LocalWebId?)null : LocalWebId.CreateNew("testdomain.com");
            var identityMappingHelper = GetMockedIdentificationBundleMappingHelper(revocable);
            var internalLocalWebId = await identityMappingHelper.GetInternalLocalWebId(localWebId);

            if (!revocable)
            {
                Assert.AreEqual(localWebId, internalLocalWebId);
            }
            else
            {
                if (isNull)
                {
                    Assert.IsNull(internalLocalWebId);
                }
                else
                {
                    Assert.IsNotNull(internalLocalWebId);
                    Assert.AreNotEqual(localWebId, internalLocalWebId);
                }
            }
        }

        [Theory]
        public async Task TestGetInternalUserCentricAdId(bool revocable, bool isNull)
        {
            var userCentricAdId = isNull ? (UserCentricAdId?)null : UserCentricAdId.New();
            var identityMappingHelper = GetMockedIdentificationBundleMappingHelper(revocable);
            var internalDnaId = await identityMappingHelper.GetInternalUserCentricAdId(userCentricAdId);

            if (!revocable)
            {
                Assert.AreEqual(userCentricAdId, internalDnaId);
            }
            else
            {
                if (isNull)
                {
                    Assert.IsNull(internalDnaId);
                }
                else
                {
                    Assert.IsNotNull(internalDnaId);
                    Assert.AreNotEqual(userCentricAdId, internalDnaId);
                }
            }
        }

        #region Helpers
        private IInternalMappingHelper GetMockedIdentificationBundleMappingHelper(bool revocable = true)
        {
            // Set revocableId CaC value (default active for these tests)
            _cacServiceMock.AddDefaultValue(RevocableIdEnabledInIdController.ParameterKey, revocable);

            // Mock IdentityMapping library call
            var identityMapping = GetIdentityMappingResult();
            _identityMapperMock.Setup(i =>
                    i.GetOrCreateInternalIdentity(It.IsAny<UserIdentity>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IdentityMappingContext>()))
                .Returns(Task.FromResult(identityMapping));

            return new InternalMappingHelper(_cacServiceMock, _identityMapperMock.Object);
        }

        private IdentityMappingResult GetIdentityMappingResult()
        {
            var internalIdentity = new InternalIdentity()
            {
                InternalId = new InternalId()
                {
                    Value = Uuid.FromGuid(Guid.NewGuid())
                }
            };
            var identityMappingResult = new IdentityMappingResult(internalIdentity, new UserIdentity(), true);

            return identityMappingResult;
        }
        #endregion
    }
}
