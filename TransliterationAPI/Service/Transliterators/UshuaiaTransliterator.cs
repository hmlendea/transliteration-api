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
                { "text", text},
                { "lang", language }
            };
            IDictionary<string, string> headers = new Dictionary<string, string>
            {
                { "Cookie", $"translit=86ta9rrnl43mdrf4qpq33h77ii; lastlang={language}; PHPSESSID=2r3ig8h0o534m09o9gogc6vood"}
            };

            string response = await httpRequestManager.Post(URL, formData, headers);

            return ApplyFixes(response, language);
        }

        string ApplyFixes(string text, string language)
        {
            string fixedText = text;

            if (language.Contains("bengali") || 
                language.Contains("devanagari"))
            {
                fixedText = fixedText.ToTitleCase();
            }
            
            return fixedText;
        }
    }
}
