namespace TransliterationAPI.Service.Transliterators
{
    public interface IAncientGreekTransliterator
    {
        string Transliterate(string text);

        string Transliterate(string text, string variant);
    }
}
