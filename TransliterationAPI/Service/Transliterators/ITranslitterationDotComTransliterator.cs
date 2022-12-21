using System.Threading.Tasks;

namespace TransliterationAPI.Service.Transliterators
{
    public interface ITranslitterationDotComTransliterator
    {
        Task<string> Transliterate(string text, string language, string scheme);
    }
}
