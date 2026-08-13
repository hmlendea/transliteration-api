using System;
using System.IO;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using NuciLog.Core;
using NUnit.Framework;

using TransliterationAPI.Configuration;
using TransliterationAPI.Service;
using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.UnitTests.Service
{
    [TestFixture]
    public class LanguageRegistryTests
    {
        [Test]
        public void GivenTheLanguageRegistry_WhenEnumeratingLanguages_ThenEachLanguageHasAConcreteTransliteratorType()
        {
            Assert.That(Language.GetAll(), Is.Not.Empty);
            Assert.That(
                Language.GetAll().All(language => language.TransliteratorType is not null),
                Is.True);
            Assert.That(
                Language.GetAll().All(language =>
                    typeof(IExternalTransliterator).IsAssignableFrom(language.TransliteratorType)
                    == language.UsesExternalTransliterator),
                Is.True);
        }

        [Test]
        public void GivenTheServiceCollection_WhenRegisteringTransliterators_ThenEveryConfiguredTransliteratorIsResolvable()
        {
            string cacheDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(cacheDirectory);

            string cacheStoreLocation = Path.Combine(cacheDirectory, "cache.json");
            File.WriteAllText(cacheStoreLocation, "[]");

            ServiceCollection services = new();
            services.AddSingleton(new CacheSettings { StoreLocation = cacheStoreLocation });
            services.AddSingleton(new SecuritySettings { HmacSigningKey = "test-key" });
            services.AddSingleton<IHttpRequestManager, FakeHttpRequestManager>();
            services.AddCustomServices();
            services.AddSingleton<ILogger, NullLogger>();

            using ServiceProvider provider = services.BuildServiceProvider();

            foreach (Type transliteratorType in Language.GetAll()
                         .Select(language => language.TransliteratorType)
                         .Distinct())
            {
                Assert.That(provider.GetService(transliteratorType), Is.Not.Null, transliteratorType.Name);
            }
        }
    }
}
