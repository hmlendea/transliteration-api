using System.Threading.Tasks;

namespace TransliterationAPI.Service.Transliterators
{
    public interface ITransliterateDotComTransliterator
    {
        Task<string> Transliterate(string text, string language);
    }
}
