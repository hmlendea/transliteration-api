using System.Threading.Tasks;

namespace TransliterationAPI.Service.Transliterators
{
    public interface IRomajiTransliterator
    {
        Task<string> Transliterate(string text);
    }
}
