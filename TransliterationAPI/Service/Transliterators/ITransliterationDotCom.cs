using System.Threading.Tasks;

namespace TransliterationAPI.Service.Transliterators
{
    public interface ITransliterationDotCom
    {
        Task<string> Transliterate(string text, string language, string scheme);
    }
}
