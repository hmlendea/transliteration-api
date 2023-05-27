namespace TransliterationAPI.Service.Transliterators
{
    public interface ITransliterator
    {
        string Transliterate(string text, string languageCode);
    }
}
