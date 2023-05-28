using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NuciDAL.Repositories;

using TransliterationAPI.Configuration;
using TransliterationAPI.Service;
using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI
{
    public static class ServiceCollectionExtensions
    {
        static CacheSettings cacheSettings;

        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            cacheSettings = new CacheSettings();

            configuration.Bind(nameof(CacheSettings), cacheSettings);

            services.AddSingleton(cacheSettings);

            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<IRepository<CachedTransliteration>>(x => new JsonRepository<CachedTransliteration>(cacheSettings.StoreLocation))
                .AddSingleton<IHttpRequestManager, HttpRequestManager>()
                .AddTransliteratorServices()
                .AddSingleton<ITransliteratorFactory, TransliteratorFactory>()
                .AddSingleton<ITransliterationService, TransliterationService>();
        }

        static IServiceCollection AddTransliteratorServices(this IServiceCollection services)
        {
            return services
                .AddScoped<AncientGreekTransliterator>()
                .AddScoped<ArabicTransliterator>()
                .AddScoped<CopticTransliterator>()
                .AddScoped<CyrillicTransliterator>()
                .AddScoped<GujaratiTransliterator>()
                .AddScoped<HebrewTransliterator>()
                .AddScoped<JapaneseTransliterator>()
                .AddScoped<MarathiTransliterator>()
                .AddScoped<PinyinTransliterator>()
                .AddScoped<PodolakTransliterator>()
                .AddScoped<TranslitterationDotComTransliterator>()
                .AddScoped<UshuaiaTransliterator>();
        }
    }
}
