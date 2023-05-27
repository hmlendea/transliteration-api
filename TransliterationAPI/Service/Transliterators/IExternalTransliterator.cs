using System.Threading.Tasks;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public interface IExternalTransliterator
    {
        Task<string> Transliterate(string text, Language language);
    }
}
