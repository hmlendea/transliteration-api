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
                .AddSingleton<IPodolakTransliterator, PodolakTransliterator>()
                .AddSingleton<ITransliterateDotComTransliterator, TransliterateDotComTransliterator>()
                .AddSingleton<ITranslitterationDotComTransliterator, TranslitterationDotComTransliterator>()
                .AddSingleton<ITransliterationService, TransliterationService>();
        }
    }
}
