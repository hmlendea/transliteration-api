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
                .AddSingleton<IAncientGreekTransliterator, AncientGreekTransliterator>()
                .AddSingleton<IArabicTransliterator, ArabicTransliterator>()
                .AddSingleton<ICopticTransliterator, CopticTransliterator>()
                .AddSingleton<ICyrillicTransliterator, CyrillicTransliterator>()
                .AddSingleton<IGujaratiTransliterator, GujaratiTransliterator>()
                .AddSingleton<IHebrewTransliterator, HebrewTransliterator>()
                .AddSingleton<IMarathiTransliterator, MarathiTransliterator>()
                .AddSingleton<IPinyinTransliterator, PinyinTransliterator>()
                .AddSingleton<IPodolakTransliterator, PodolakTransliterator>()
                .AddSingleton<IJapaneseTransliterator, JapaneseTransliterator>()
                .AddSingleton<IThailitTransliterator, ThailitTransliterator>()
                .AddSingleton<ITranslitterationDotComTransliterator, TranslitterationDotComTransliterator>()
                .AddSingleton<IUshuaiaTransliterator, UshuaiaTransliterator>()
                .AddSingleton<ITransliterationService, TransliterationService>();
        }
    }
}
