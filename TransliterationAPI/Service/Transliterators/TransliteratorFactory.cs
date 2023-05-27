using System;
using Microsoft.Extensions.DependencyInjection;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class TransliteratorFactory : ITransliteratorFactory
    {
        public IExternalTransliterator GetExternalTransliterator(Language language)
            => language.Transliterator switch
            {
                nameof(PodolakTransliterator) => Startup.ServiceProvider.GetService<PodolakTransliterator>(),
                nameof(ThailitTransliterator) => Startup.ServiceProvider.GetService<ThailitTransliterator>(),
                nameof(TranslitterationDotComTransliterator) => Startup.ServiceProvider.GetService<TranslitterationDotComTransliterator>(),
                nameof(UshuaiaTransliterator) => Startup.ServiceProvider.GetService<UshuaiaTransliterator>(),
                _ => throw new ArgumentException($"The \"{language.Transliterator}\" external transliterator is not registered!")
            };

        public ITransliterator GetTransliterator(Language language)
            => language.Transliterator switch
            {
                nameof(AncientGreekTransliterator) => Startup.ServiceProvider.GetService<AncientGreekTransliterator>(),
                nameof(ArabicTransliterator) => Startup.ServiceProvider.GetService<ArabicTransliterator>(),
                nameof(CopticTransliterator) => Startup.ServiceProvider.GetService<CopticTransliterator>(),
                nameof(CyrillicTransliterator) => Startup.ServiceProvider.GetService<CyrillicTransliterator>(),
                nameof(GujaratiTransliterator) => Startup.ServiceProvider.GetService<GujaratiTransliterator>(),
                nameof(HebrewTransliterator) => Startup.ServiceProvider.GetService<HebrewTransliterator>(),
                nameof(JapaneseTransliterator) => Startup.ServiceProvider.GetService<JapaneseTransliterator>(),
                nameof(MarathiTransliterator) => Startup.ServiceProvider.GetService<MarathiTransliterator>(),
                nameof(PinyinTransliterator) => Startup.ServiceProvider.GetService<PinyinTransliterator>(),
                _ => throw new ArgumentException($"The \"{language.Transliterator}\" internal transliterator is not registered!")
            };
    }
}
