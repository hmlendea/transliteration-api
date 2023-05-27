using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

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
            => language.Transliterator switch
            {
                nameof(PodolakTransliterator) => Startup.ServiceProvider.GetService<PodolakTransliterator>(),
                nameof(ThailitTransliterator) => Startup.ServiceProvider.GetService<ThailitTransliterator>(),
                nameof(TranslitterationDotComTransliterator) => Startup.ServiceProvider.GetService<TranslitterationDotComTransliterator>(),
                nameof(UshuaiaTransliterator) => Startup.ServiceProvider.GetService<UshuaiaTransliterator>(),
                _ => throw new ArgumentException($"The \"{language.Transliterator}\" external transliterator is not registered!")
            };

        public ITransliterator GetTransliterator(Language language)
            => GetTransliteratorInstance<ITransliterator>(language.Transliterator);

        private TTransliteratorInterface GetTransliteratorInstance<TTransliteratorInterface>(string transliteratorClassName)
        {
            Type transliteratorType = GetTransliteratorType(transliteratorClassName);

            try
            {
                return (TTransliteratorInterface)Activator.CreateInstance(transliteratorType);
            }
            catch
            {
                throw new ArgumentException($"The {transliteratorClassName} transliterator is not registered!");
            }
        }

        private Type GetTransliteratorType(string transliteratorClassName)
            => assembly.GetType($"{nameof(TransliterationAPI)}.{nameof(Service)}.{nameof(Transliterators)}.{transliteratorClassName}");
    }
}
