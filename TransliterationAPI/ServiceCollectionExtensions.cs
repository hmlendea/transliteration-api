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
        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            CacheSettings cacheSettings = new();
            SecuritySettings securitySettings = new();
            NuciLoggerSettings loggerSettings = new();

            configuration.Bind(nameof(CacheSettings), cacheSettings);
            configuration.Bind(nameof(SecuritySettings), securitySettings);
            configuration.Bind(nameof(NuciLoggerSettings), loggerSettings);

            services.AddSingleton(cacheSettings);
            services.AddSingleton(securitySettings);
            services.AddSingleton(loggerSettings);

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
                .AddScoped<ITransliteratorFactory, TransliteratorFactory>()
                .AddSingleton<ITransliterationService, TransliterationService>();
        }

        static IServiceCollection AddTransliteratorServices(this IServiceCollection services)
        {
            return services
                .AddTransient<GreekTransliterator>()
                .AddTransient<ArabicTransliterator>()
                .AddTransient<CopticTransliterator>()
                .AddTransient<CyrillicTransliterator>()
                .AddTransient<GujaratiTransliterator>()
                .AddTransient<HebrewTransliterator>()
                .AddTransient<JapaneseTransliterator>()
                .AddTransient<KoreanTransliterator>()
                .AddTransient<MarathiTransliterator>()
                .AddTransient<PinyinTransliterator>()
                .AddTransient<PodolakTransliterator>()
                .AddTransient<TranslitterationDotComTransliterator>()
                .AddTransient<UshuaiaTransliterator>()
                .AddTransient<ILogger, NuciLogger>();
        }
    }
}
