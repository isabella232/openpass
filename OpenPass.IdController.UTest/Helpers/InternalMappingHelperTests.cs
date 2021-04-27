using System;
using System.Threading.Tasks;
using Criteo.ConfigAsCode;
using Criteo.Configuration.Repository.IdController;
using Criteo.Identification.Schemas;
using Criteo.UserIdentification;
using Criteo.UserIdentification.Services.IdentityMapping;
using Moq;
using NUnit.Framework;
using OpenPass.IdController.Helpers;
using CriteoId = Criteo.UserIdentification.CriteoId;

namespace OpenPass.IdController.UTest.Helpers
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

        [Test]
        public async Task GetInternalCriteoId_CaCParameterIsEnabled_InternalCriteoIdIsNotTheSame()
        {
            // Arrange
            var criteoId = CriteoId.New();
            var identityMappingHelper = GetMockedIdentificationBundleMappingHelper();

            // Act
            var internalCriteoId = await identityMappingHelper.GetInternalCriteoId(criteoId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(internalCriteoId);
                Assert.AreNotEqual(criteoId, internalCriteoId);
            });
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task GetInternalCriteoId_CaCParameterIsSet_CriteoIdIsNull(bool revocable)
        {
            // Arrange
            var identityMappingHelper = GetMockedIdentificationBundleMappingHelper(revocable);

            // Act
            var internalCriteoId = await identityMappingHelper.GetInternalCriteoId(null);

            // Assert
            Assert.IsNull(internalCriteoId);
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task GetInternalLocalWebId_CaCParameterIsSet_WebIdIsNull(bool revocable)
        {
            // Arrange
            var identityMappingHelper = GetMockedIdentificationBundleMappingHelper(revocable);

            // Act
            var internalLocalWebId = await identityMappingHelper.GetInternalLocalWebId(null);

            // Assert
            Assert.IsNull(internalLocalWebId);
        }

        [Test]
        public async Task GetInternalLocalWebId_CaCParameterIsEnabled_WebIdIsNotTheSame()
        {
            // Arrange
            var localWebId = LocalWebId.CreateNew("testdomain.com");
            var identityMappingHelper = GetMockedIdentificationBundleMappingHelper();

            // Act
            var internalLocalWebId = await identityMappingHelper.GetInternalLocalWebId(localWebId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(internalLocalWebId);
                Assert.AreNotEqual(localWebId, internalLocalWebId);
            });
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task GetInternalUserCentricAdId_CaCParameterIsSet_UserCentricAdIdIsNull(bool revocable)
        {
            // Arrange
            var identityMappingHelper = GetMockedIdentificationBundleMappingHelper(revocable);

            // Act
            var internalDnaId = await identityMappingHelper.GetInternalUserCentricAdId(null);

            // Assert
            Assert.IsNull(internalDnaId);
        }

        [Test]
        public async Task GetInternalUserCentricAdId_CaCParameterIsEnabled_UserCentricAdIdIsNotTheSame()
        {
            // Arrange
            var userCentricAdId = UserCentricAdId.New();
            var identityMappingHelper = GetMockedIdentificationBundleMappingHelper();

            // Act
            var internalDnaId = await identityMappingHelper.GetInternalUserCentricAdId(userCentricAdId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(internalDnaId);
                Assert.AreNotEqual(userCentricAdId, internalDnaId);
            });
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

        #endregion Helpers
    }
}
