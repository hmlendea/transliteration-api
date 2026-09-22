using System;
using Microsoft.Extensions.DependencyInjection;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class TransliteratorFactory(IServiceProvider serviceProvider) : ITransliteratorFactory
    {
        public IExternalTransliterator GetExternalTransliterator(Language language)
            => (IExternalTransliterator)serviceProvider.GetRequiredService(language.TransliteratorType);

        public ITransliterator GetTransliterator(Language language)
            => (ITransliterator)serviceProvider.GetRequiredService(language.TransliteratorType);
    }
}
