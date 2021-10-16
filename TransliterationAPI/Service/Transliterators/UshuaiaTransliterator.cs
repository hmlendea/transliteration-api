using System.Collections.Generic;
using System.Threading.Tasks;

using NuciExtensions;

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
                { "text", text },
                { "lang", language }
            };
            IDictionary<string, string> headers = new Dictionary<string, string>
            {
                { "Cookie", "translit=6rhvaovg1cfkbt8cb9vhtkbfdh;" }
            };

            string response = await httpRequestManager.Post(URL, formData, headers);
            string transliteratedText = ApplyFixes(response, language);

            return transliteratedText;
        }

        string ApplyFixes(string text, string mode)
        {
            string fixedText = text;

            if (mode.Contains("bengali") || 
                mode.Contains("devanagari") || 
                mode.Contains("hangul_mr") || 
                mode.Contains("kannada") ||
                mode.Contains("malayalam") ||
                mode.Contains("sinhala") ||
                mode.Contains("tamil"))
            {
                fixedText = fixedText.ToTitleCase();
            }
            
            return fixedText;
        }
    }
}
