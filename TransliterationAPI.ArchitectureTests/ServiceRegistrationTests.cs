using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using NuciDAL.Repositories;

using NuciLog.Core;

using NUnit.Framework;

using TransliterationAPI.Service;
using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.ArchitectureTests
{
    [TestFixture]
    public sealed class ServiceRegistrationTests
    {
        [TestCase(typeof(ITransliterationService), typeof(TransliterationService))]
        [TestCase(typeof(ITransliteratorFactory), typeof(TransliteratorFactory))]
        [TestCase(typeof(IHttpRequestManager), typeof(HttpRequestManager))]
        public void GivenApplicationPorts_WhenInspectingRegistrations_ThenEachHasOneSingletonImplementation(
            Type serviceType,
            Type implementationType)
        {
            ServiceCollection services = new();
            services.AddCustomServices();
            ServiceDescriptor[] registrations = services
                .Where(descriptor => Equals(descriptor.ServiceType, serviceType))
                .ToArray();

            Assert.That(registrations, Has.Length.EqualTo(1), $"Register {serviceType.Name} exactly once.");
            Assert.That(registrations[0].Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
            Assert.That(registrations[0].ImplementationType, Is.EqualTo(implementationType));
        }

        [TestCase(typeof(IFileRepository<CachedTransliteration>))]
        [TestCase(typeof(ILogger))]
        public void GivenInfrastructurePorts_WhenInspectingRegistrations_ThenEachHasOneSingleton(
            Type serviceType)
        {
            ServiceCollection services = new();
            services.AddCustomServices();
            ServiceDescriptor[] registrations = services
                .Where(descriptor => Equals(descriptor.ServiceType, serviceType))
                .ToArray();

            Assert.That(registrations, Has.Length.EqualTo(1), $"Register {serviceType.Name} exactly once.");
            Assert.That(registrations[0].Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
        }

        [Test]
        public void GivenRegistryStrategies_WhenInspectingRegistrations_ThenEachHasOneSelfBoundSingleton()
        {
            ServiceCollection services = new();
            services.AddCustomServices();
            Type[] strategyTypes = Language.GetAll()
                .Select(language => language.TransliteratorType)
                .Distinct()
                .ToArray();

            Assert.That(strategyTypes, Is.Not.Empty, "The registry must select production strategies.");

            foreach (Type strategyType in strategyTypes)
            {
                ServiceDescriptor[] registrations = services
                    .Where(descriptor => Equals(descriptor.ServiceType, strategyType))
                    .ToArray();

                Assert.That(registrations, Has.Length.EqualTo(1), $"Register {strategyType.Name} exactly once.");
                Assert.That(registrations[0].Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
                Assert.That(registrations[0].ImplementationType, Is.EqualTo(strategyType));
            }
        }

        [Test]
        public void GivenRegistryStrategies_WhenInspectingTypes_ThenTheyAreClosedConcreteProductionClasses()
        {
            Type[] strategyTypes = Language.GetAll()
                .Select(language => language.TransliteratorType)
                .Distinct()
                .ToArray();

            Assert.That(strategyTypes, Is.Not.Empty, "The registry must select production strategies.");

            foreach (Type strategyType in strategyTypes)
            {
                Assert.That(strategyType.IsClass, Is.True, strategyType.FullName);
                Assert.That(strategyType.IsAbstract, Is.False, strategyType.FullName);
                Assert.That(strategyType.ContainsGenericParameters, Is.False, strategyType.FullName);
                Assert.That(strategyType.Assembly, Is.EqualTo(ProductionArchitecture.Assembly));
                Assert.That(
                    typeof(ITransliterator).IsAssignableFrom(strategyType) ^
                        typeof(IExternalTransliterator).IsAssignableFrom(strategyType),
                    Is.True,
                    $"{strategyType.Name} must implement exactly one supported dispatch contract.");
                Assert.That(
                    strategyType.Namespace,
                    Is.EqualTo(typeof(ITransliterator).Namespace),
                    $"{strategyType.Name} must belong to the strategy boundary.");
            }
        }

        [TestCase(typeof(ITransliterator), typeof(IExternalTransliterator), typeof(Transliterator))]
        [TestCase(typeof(IExternalTransliterator), typeof(ITransliterator), typeof(ExternalTransliterator))]
        public void GivenRegistryStrategies_WhenInspectingContracts_ThenDispatchAndLoggedExecutionAreUnambiguous(
            Type strategyContract,
            Type otherContract,
            Type loggedBaseType)
        {
            Type[] strategyTypes = Language.GetAll()
                .Select(language => language.TransliteratorType)
                .Distinct()
                .Where(strategyContract.IsAssignableFrom)
                .ToArray();

            Assert.That(strategyTypes, Is.Not.Empty, $"The registry must contain {strategyContract.Name} types.");

            foreach (Type strategyType in strategyTypes)
            {
                Assert.That(
                    otherContract.IsAssignableFrom(strategyType),
                    Is.False,
                    $"{strategyType.Name} must implement only one dispatch contract.");
                Assert.That(
                    loggedBaseType.IsAssignableFrom(strategyType),
                    Is.True,
                    $"{strategyType.Name} must inherit {loggedBaseType.Name} for logged execution.");
            }
        }
    }
}
