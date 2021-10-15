using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TransliterationAPI.Service.Transliterators
{
    public class TransliterateDotComTransliterator : ITransliterateDotComTransliterator
    {
        private const string URL = "https://transliterate.com/Home/Transliterate";

        IHttpRequestManager httpRequestManager;

        public TransliterateDotComTransliterator(IHttpRequestManager httpRequestManager)
        {
            this.httpRequestManager = httpRequestManager;
        }

        public async Task<string> Transliterate(string text, string language)
        {
            IDictionary<string, string> formData = new Dictionary<string, string>
            {
                { "input", text}
            };

            string response = await httpRequestManager.Post(URL, formData);
            string rawTransliteratedText = ExtractResultFromHtml(response);

            return ApplyLanguageSpecificFixes(rawTransliteratedText, language).Replace("ack:::", "");
        }

        private string ApplyLanguageSpecificFixes(string text, string language)
        {
            string fixedText = text;

            if (language == "el")
            {
                fixedText = Regex.Replace(fixedText, "^Mp", "B");
                fixedText = Regex.Replace(fixedText, "^Nk", "G");
                fixedText = Regex.Replace(fixedText, "^Nt", "D");
                fixedText = Regex.Replace(fixedText, "mp([ao])", "b");
                fixedText = Regex.Replace(fixedText, "nknt", "gd");
                fixedText = Regex.Replace(fixedText, "ntm", "dm");
                fixedText = Regex.Replace(fixedText, "rnk", "rk");
                fixedText = Regex.Replace(fixedText, "snt", "sht");
            }

            return fixedText;
        }

        private string ExtractResultFromHtml(string html)
        {
            return Regex.Replace(html, ".*\"latin\":\"([^\"]*).*", "$1");
        }
    }
}
