using System.Threading.Tasks;

namespace TransliterationAPI.Service.Transliterators
{
    public interface IPodolakTransliterator
    {
        Task<string> Transliterate(string text, string language);
    }
}
