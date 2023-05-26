namespace TransliterationAPI.Service.Transliterators
{
    public interface ICyrillicTransliterator
    {
        string Transliterate(string text, string variant);
    }
}
