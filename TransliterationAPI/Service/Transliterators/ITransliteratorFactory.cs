using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public interface ITransliteratorFactory
    {
        IExternalTransliterator GetExternalTransliterator(Language language);

        ITransliterator GetTransliterator(Language language);
    }
}
