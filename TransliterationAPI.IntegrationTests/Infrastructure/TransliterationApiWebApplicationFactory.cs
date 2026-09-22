using System;
using System.IO;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NuciLog.Core;
using NuciLogger = NuciLog.Core.ILogger;

using TransliterationAPI.Configuration;
using TransliterationAPI.Service;

namespace TransliterationAPI.IntegrationTests.Infrastructure
{
    internal sealed class TransliterationApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        private static string EnvironmentName => "IntegrationTesting";

        private readonly string cacheDirectoryLocation;
        private readonly bool isCacheEnabled;
        private readonly ITransliterationService? transliterationService;

        internal static string HmacSigningKey => "NucileRullz!";
        internal string CacheStoreLocation { get; }
        internal RecordingHttpRequestManager HttpRequestManager { get; }

        internal TransliterationApiWebApplicationFactory() : this(true, null)
        {
        }

        internal TransliterationApiWebApplicationFactory(bool isCacheEnabled) : this(isCacheEnabled, null)
        {
        }

        internal TransliterationApiWebApplicationFactory(ITransliterationService transliterationService)
            : this(true, transliterationService)
        {
        }

        private TransliterationApiWebApplicationFactory(
            bool isCacheEnabled,
            ITransliterationService? transliterationService)
        {
            cacheDirectoryLocation = Path.Combine(
                Path.GetTempPath(),
                nameof(TransliterationApiWebApplicationFactory),
                Guid.NewGuid().ToString("N"));
            CacheStoreLocation = Path.Combine(cacheDirectoryLocation, "cache.json");
            HttpRequestManager = new RecordingHttpRequestManager();
            this.isCacheEnabled = isCacheEnabled;
            this.transliterationService = transliterationService;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(EnvironmentName);
            builder.ConfigureLogging(logging => logging.ClearProviders());
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<CacheSettings>();
                services.RemoveAll<SecuritySettings>();
                services.RemoveAll<NuciLogger>();
                services.RemoveAll<IHttpRequestManager>();

                services.AddSingleton<IStartupFilter, LoopbackRemoteIpAddressStartupFilter>();
                services.AddSingleton(new CacheSettings
                {
                    Enabled = isCacheEnabled,
                    StoreLocation = CacheStoreLocation
                });
                services.AddSingleton(new SecuritySettings
                {
                    HmacSigningKey = HmacSigningKey
                });
                services.AddSingleton<NuciLogger, NullLogger>();
                services.AddSingleton<IHttpRequestManager>(HttpRequestManager);

                if (transliterationService is not null)
                {
                    services.RemoveAll<ITransliterationService>();
                    services.AddSingleton(transliterationService);
                }
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (Directory.Exists(cacheDirectoryLocation))
            {
                Directory.Delete(cacheDirectoryLocation, true);
            }
        }
    }
}