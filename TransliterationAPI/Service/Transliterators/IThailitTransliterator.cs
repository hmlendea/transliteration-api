using System.Threading.Tasks;

namespace TransliterationAPI.Service.Transliterators
{
    public interface IThailitTransliterator
    {
        Task<string> Transliterate(string text);
    }
}
