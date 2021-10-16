namespace TransliterationAPI.Service.Transliterators
{
    public interface IPinyinTransliterator
    {
        string Transliterate(string text);
    }
}
