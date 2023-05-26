namespace TransliterationAPI.Service.Transliterators
{
    public interface ICopticTransliterator
    {
        string Transliterate(string text, string languageCode);
    }
}
