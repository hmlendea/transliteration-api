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
                .AddSingleton<IGujaratiTransliterator, GujaratiTransliterator>()
                .AddSingleton<IPodolakTransliterator, PodolakTransliterator>()
                .AddSingleton<IRomajiTransliterator, RomajiTransliterator>()
                .AddSingleton<ITransliterateDotComTransliterator, TransliterateDotComTransliterator>()
                .AddSingleton<ITranslitterationDotComTransliterator, TranslitterationDotComTransliterator>()
                .AddSingleton<IUshuaiaTransliterator, UshuaiaTransliterator>()
                .AddSingleton<ITransliterationService, TransliterationService>();
        }
    }
}
