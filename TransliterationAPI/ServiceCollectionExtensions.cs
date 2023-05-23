using Microsoft.Extensions.DependencyInjection;

using TransliterationAPI.Service;
using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<IHttpRequestManager, HttpRequestManager>()
                .AddSingleton<IAncientGreekTransliterator, AncientGreekTransliterator>()
                .AddSingleton<IArabicTransliterator, ArabicTransliterator>()
                .AddSingleton<IGujaratiTransliterator, GujaratiTransliterator>()
                .AddSingleton<IMarathiTransliterator, MarathiTransliterator>()
                .AddSingleton<IPinyinTransliterator, PinyinTransliterator>()
                .AddSingleton<IPodolakTransliterator, PodolakTransliterator>()
                .AddSingleton<IRomajiTransliterator, RomajiTransliterator>()
                .AddSingleton<IThailitTransliterator, ThailitTransliterator>()
                .AddSingleton<ITransliterateDotComTransliterator, TransliterateDotComTransliterator>()
                .AddSingleton<ITranslitterationDotComTransliterator, TranslitterationDotComTransliterator>()
                .AddSingleton<IUshuaiaTransliterator, UshuaiaTransliterator>()
                .AddSingleton<ITransliterationService, TransliterationService>();
        }
    }
}
