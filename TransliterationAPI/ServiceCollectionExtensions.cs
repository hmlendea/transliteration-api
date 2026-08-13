using System;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NuciDAL.Repositories;

using NuciLog;
using NuciLog.Configuration;
using NuciLog.Core;

using TransliterationAPI.Configuration;
using TransliterationAPI.Service;
using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurations(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            CacheSettings cacheSettings = new();
            SecuritySettings securitySettings = new();

            configuration.Bind(nameof(CacheSettings), cacheSettings);
            configuration.Bind(nameof(SecuritySettings), securitySettings);

            services.AddSingleton(cacheSettings);
            services.AddSingleton(securitySettings);
            services.AddNuciLoggerSettings(configuration);

            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<IFileRepository<CachedTransliteration>>(provider
                    => new JsonRepository<CachedTransliteration>(
                        provider.GetRequiredService<CacheSettings>().StoreLocation))
                .AddSingleton<IHttpRequestManager, HttpRequestManager>()
                .AddTransliteratorServices()
                .AddSingleton<ITransliteratorFactory, TransliteratorFactory>()
                .AddSingleton<ITransliterationService, TransliterationService>();
        }

        private static IServiceCollection AddTransliteratorServices(this IServiceCollection services)
        {
            foreach (Type transliteratorType in Language.GetAll()
                         .Select(language => language.TransliteratorType)
                         .Distinct())
            {
                services.AddSingleton(transliteratorType);
            }

            return services
                .AddSingleton<ILogger, NuciLogger>();
        }
    }
}
