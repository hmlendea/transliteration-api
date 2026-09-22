using System;
using System.IO;
using System.Linq;
using System.Net.Http;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NUnit.Framework;

using TransliterationAPI.Configuration;
using TransliterationAPI.Service;
using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.IntegrationTests.Infrastructure
{
    [TestFixture]
    public sealed class ApplicationStartupTests
    {
        [Test]
        public void GivenNoCacheDirectory_WhenTheApplicationStarts_ThenTheDirectoryAndEmptyStoreAreCreated()
        {
            using TransliterationApiWebApplicationFactory factory = new();
            string? cacheDirectory = Path.GetDirectoryName(factory.CacheStoreLocation);

            Assert.That(cacheDirectory, Is.Not.Null);
            Assert.That(Directory.Exists(cacheDirectory!), Is.False);

            using HttpClient client = factory.CreateClient();

            Assert.Multiple(() =>
            {
                Assert.That(Directory.Exists(cacheDirectory));
                Assert.That(File.Exists(factory.CacheStoreLocation));
                Assert.That(File.ReadAllText(factory.CacheStoreLocation), Is.EqualTo("[]"));
            });
        }

        [Test]
        [TestCase("[]")]
        [TestCase("existing content")]
        [TestCase("{\"username\":\"Angetenar\"}")]
        [TestCase("\r\n")]
        public void GivenAnExistingCacheStore_WhenTheApplicationStarts_ThenItsContentIsPreserved(string existingContent)
        {
            using TransliterationApiWebApplicationFactory factory = new();
            string? cacheDirectory = Path.GetDirectoryName(factory.CacheStoreLocation);
            Directory.CreateDirectory(cacheDirectory!);
            File.WriteAllText(factory.CacheStoreLocation, existingContent);

            using HttpClient client = factory.CreateClient();

            Assert.That(File.ReadAllText(factory.CacheStoreLocation), Is.EqualTo(existingContent));
        }

        [Test]
        public void GivenAStartedApplication_WhenTheFactoryIsDisposed_ThenItsTemporaryCacheDirectoryIsDeleted()
        {
            TransliterationApiWebApplicationFactory factory = new();
            string? cacheDirectory = Path.GetDirectoryName(factory.CacheStoreLocation);
            using HttpClient client = factory.CreateClient();

            Assert.That(Directory.Exists(cacheDirectory));

            factory.Dispose();

            Assert.That(Directory.Exists(cacheDirectory), Is.False);
        }

        [Test]
        public void GivenTheIntegrationHost_WhenResolvingConfiguration_ThenIsolatedValuesAreRegistered()
        {
            using TransliterationApiWebApplicationFactory factory = new();
            using HttpClient client = factory.CreateClient();
            CacheSettings cacheSettings = factory.Services.GetRequiredService<CacheSettings>();
            SecuritySettings securitySettings = factory.Services.GetRequiredService<SecuritySettings>();
            IHostEnvironment environment = factory.Services.GetRequiredService<IHostEnvironment>();

            Assert.Multiple(() =>
            {
                Assert.That(cacheSettings.Enabled);
                Assert.That(cacheSettings.StoreLocation, Is.EqualTo(factory.CacheStoreLocation));
                Assert.That(securitySettings.HmacSigningKey, Is.EqualTo(TransliterationApiWebApplicationFactory.HmacSigningKey));
                Assert.That(environment.EnvironmentName, Is.EqualTo("IntegrationTesting"));
            });
        }

        [Test]
        public void GivenTheIntegrationHost_WhenResolvingApplicationServices_ThenTheirConfiguredLifetimesAreSingleton()
        {
            using TransliterationApiWebApplicationFactory factory = new();
            using HttpClient client = factory.CreateClient();

            Assert.Multiple(() =>
            {
                Assert.That(
                    factory.Services.GetRequiredService<ITransliterationService>(),
                    Is.SameAs(factory.Services.GetRequiredService<ITransliterationService>()));
                Assert.That(
                    factory.Services.GetRequiredService<ITransliteratorFactory>(),
                    Is.SameAs(factory.Services.GetRequiredService<ITransliteratorFactory>()));
                Assert.That(
                    factory.Services.GetRequiredService<IHttpRequestManager>(),
                    Is.SameAs(factory.HttpRequestManager));
            });
        }

        [Test]
        public void GivenTheIntegrationHost_WhenResolvingRegisteredTransliterators_ThenEveryDistinctImplementationIsAvailableAsASingleton()
        {
            using TransliterationApiWebApplicationFactory factory = new();
            using HttpClient client = factory.CreateClient();
            Type[] transliteratorTypes = Language.GetAll()
                .Select(language => language.TransliteratorType)
                .Distinct()
                .ToArray();

            foreach (Type transliteratorType in transliteratorTypes)
            {
                object firstInstance = factory.Services.GetRequiredService(transliteratorType);
                object secondInstance = factory.Services.GetRequiredService(transliteratorType);

                Assert.That(firstInstance, Is.SameAs(secondInstance), transliteratorType.Name);
            }
        }

        [Test]
        public void GivenCachingIsDisabled_WhenTheApplicationStarts_ThenTheDisabledSettingIsRegistered()
        {
            using TransliterationApiWebApplicationFactory factory = new(false);
            using HttpClient client = factory.CreateClient();
            CacheSettings cacheSettings = factory.Services.GetRequiredService<CacheSettings>();

            Assert.That(cacheSettings.Enabled, Is.False);
        }
    }
}