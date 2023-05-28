using System;
using System.Reflection;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class TransliteratorFactory : ITransliteratorFactory
    {
        readonly Assembly assembly;

        public TransliteratorFactory()
        {
            assembly = Assembly.GetExecutingAssembly();
        }

        public IExternalTransliterator GetExternalTransliterator(Language language)
        {
            Type transliteratorType = assembly.GetType($"{nameof(TransliterationAPI)}.{nameof(Service)}.{nameof(Transliterators)}.{language.Transliterator}");
            return (IExternalTransliterator)Startup.ServiceProvider.GetService(transliteratorType);
        }

        public ITransliterator GetTransliterator(Language language)
        {
            Type transliteratorType = assembly.GetType($"{nameof(TransliterationAPI)}.{nameof(Service)}.{nameof(Transliterators)}.{language.Transliterator}");
            return (ITransliterator)Startup.ServiceProvider.GetService(transliteratorType);
        }
    }
}
