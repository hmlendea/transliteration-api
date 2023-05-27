using System.Threading.Tasks;

namespace TransliterationAPI.Service.Transliterators
{
    public interface IExternalTransliterator
    {
        Task<string> Transliterate(string text, string languageCode);
    }
}
