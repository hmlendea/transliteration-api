using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class TransliteratorFactory(IServiceProvider serviceProvider) : ITransliteratorFactory
    {
        private readonly IServiceProvider serviceProvider = serviceProvider;
        private readonly Assembly assembly = Assembly.GetExecutingAssembly();

        public IExternalTransliterator GetExternalTransliterator(Language language)
            => (IExternalTransliterator)serviceProvider.GetRequiredService(GetTransliteratorType(language));

        public ITransliterator GetTransliterator(Language language)
            => (ITransliterator)serviceProvider.GetRequiredService(GetTransliteratorType(language));

        Type GetTransliteratorType(Language language)
            => assembly.GetType(GetTransliteratorTypeName(language))
                ?? throw new InvalidOperationException($"Transliterator '{language.Transliterator}' not found.");

        static string GetTransliteratorTypeName(Language language)
            => $"{nameof(TransliterationAPI)}." +
               $"{nameof(Service)}." +
               $"{nameof(Transliterators)}." +
               $"{language.Transliterator}";
    }
}
