using System.Collections.Generic;
using System.Threading.Tasks;

namespace TransliterationAPI.Service.Transliterators
{
    public class UshuaiaTransliterator : IUshuaiaTransliterator
    {
        const string URL = "https://www.ushuaia.pl/transliterate/transliterate.php";

        IHttpRequestManager httpRequestManager;

        public UshuaiaTransliterator(IHttpRequestManager httpRequestManager)
        {
            this.httpRequestManager = httpRequestManager;
        }

        public async Task<string> Transliterate(string text, string language)
        {
            IDictionary<string, string> formData = new Dictionary<string, string>
            {
                { "text", text},
                { "lang", language }
            };
            IDictionary<string, string> headers = new Dictionary<string, string>
            {
                { "Cookie", $"translit=4iuo753l2j3dej19i8rg7vkbkt; lastlang={language}; PHPSESSID=46akf4fi4bsq67otho7pg8qa34"}
            };

            return await httpRequestManager.Post(URL, formData, headers);
        }
    }
}
