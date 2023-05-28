using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public interface ITransliterator
    {
        string Transliterate(string text, Language language);
    }
}
