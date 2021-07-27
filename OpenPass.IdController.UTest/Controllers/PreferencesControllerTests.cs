using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OpenPass.IdController.Controllers;
using OpenPass.IdController.DataAccess;
using OpenPass.IdController.Helpers;
using OpenPass.IdController.Models;
using OpenPass.IdController.UTest.TestUtils;

namespace OpenPass.IdController.UTest.Controllers
{
    [TestFixture]
    class PreferencesControllerTests
    {
        private const string _ifa = "dac2ed18-d518-40ba-8682-75077e6cf402";
        private const string _domain = "partner.com";
        private static readonly UserPreferences _userPreferencesWithDomain = new UserPreferences()
        {
            DomainsWithConsent = new List<string>() { _domain, "anotherdomain.com" }
        };
        private static readonly UserPreferences _userPreferencesWithoutDomain = new UserPreferences()
        {
            DomainsWithConsent = new List<string>() { "anotherdomain.com" }
        };

        private Mock<IMetricHelper> _metricHelperMock;
        private Mock<IConfigurationHelper> _configurationHelperMock;
        private Mock<IUserPreferencesRepository> _userPreferencesRepository;

        private PreferencesController _controller;

        [SetUp]
        public void Setup()
        {
            _configurationHelperMock = new Mock<IConfigurationHelper>();
            _metricHelperMock = new Mock<IMetricHelper>();
            _userPreferencesRepository = new Mock<IUserPreferencesRepository>();
            _controller = new PreferencesController(
                _configurationHelperMock.Object,
                _metricHelperMock.Object,
                _userPreferencesRepository.Object);
        }

        [Test]
        public async Task PerDomainNoDomainNoIfa()
        {
            var expected = new BadRequestObjectResult("No x-origin-host; No ifa");

            var result = await _controller.GetPreferencesPerDomain(string.Empty, string.Empty);

            AssertExtension.AreEquivalent(expected, result);
            VerifyMetricCall("preferences.per_domain.bad_request.no_origin");
            VerifyMetricCall("preferences.per_domain.bad_request.no_ifa");
        }

        [Test]
        public async Task PerDomainNoOrigin()
        {
            var expected = new BadRequestObjectResult("No x-origin-host");

            var result = await _controller.GetPreferencesPerDomain(string.Empty, _ifa);

            AssertExtension.AreEquivalent(expected, result);
            VerifyMetricCall("preferences.per_domain.bad_request.no_origin");
        }

        [Test]
        public async Task PerDomainNoIfa()
        {
            var expected = new BadRequestObjectResult("No ifa");

            var result = await _controller.GetPreferencesPerDomain(_domain, string.Empty);

            AssertExtension.AreEquivalent(expected, result);
            VerifyMetricCall("preferences.per_domain.bad_request.no_ifa");
        }

        [Test]
        public async Task PerDomainOkWithConsent()
        {
            SetupGetUserPreferences(_ifa, _userPreferencesWithDomain);
            SetupCmpIntegrationEnable(_domain, enable: true);
            var expected = new OkObjectResult(new PreferencesResponse()
            {
                DomainPreferences = new DomainPreferences()
                {
                    Name = _domain,
                    CmpIntegrationEnable = true
                },
                UserPreferences = new UserPreferencesForDomain()
                {
                    Consent = true
                }
            });

            var result = await _controller.GetPreferencesPerDomain(_domain, _ifa);

            AssertExtension.AreEquivalent(expected, result);
        }

        [Test]
        public async Task PerDomainOkWithoutConsent()
        {
            SetupGetUserPreferences(_ifa, _userPreferencesWithoutDomain);
            SetupCmpIntegrationEnable(_domain, enable: false);
            var expected = new OkObjectResult(new PreferencesResponse()
            {
                DomainPreferences = new DomainPreferences()
                {
                    Name = _domain,
                    CmpIntegrationEnable = false
                },
                UserPreferences = new UserPreferencesForDomain()
                {
                    Consent = false
                }
            });

            var result = await _controller.GetPreferencesPerDomain(_domain, _ifa);

            AssertExtension.AreEquivalent(expected, result);
        }

        private void SetupGetUserPreferences(string ifa, UserPreferences preferences)
        {
            _userPreferencesRepository.Setup(r => r.GetPreferences(ifa)).Returns(Task.FromResult(preferences));
        }

        private void SetupCmpIntegrationEnable(string domain, bool enable = true)
        {
            _configurationHelperMock.Setup(h => h.CmpIntegrationEnable(domain)).Returns(enable);
        }

        private void VerifyMetricCall(string metric, int count = 1)
        {
            _metricHelperMock.Verify(h => h.SendCounterMetric(metric), Times.Exactly(count));
        }
    }
}
