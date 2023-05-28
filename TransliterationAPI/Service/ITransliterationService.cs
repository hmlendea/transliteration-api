using System.Threading.Tasks;

namespace TransliterationAPI.Service
{
    public interface ITransliterationService
    {
        Task<string> Transliterate(string text, string languageCode);
    }
}
