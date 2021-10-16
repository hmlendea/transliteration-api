using System.Threading.Tasks;

namespace TransliterationAPI.Service.Transliterators
{
    public interface IUshuaiaTransliterator
    {
        Task<string> Transliterate(string text, string mode);
    }
}
