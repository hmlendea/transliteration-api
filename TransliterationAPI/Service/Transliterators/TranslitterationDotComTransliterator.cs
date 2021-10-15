using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TransliterationAPI.Service.Transliterators
{
    public class TranslitterationDotComTransliterator : ITranslitterationDotComTransliterator
    {
        private const string URL = "https://www.translitteration.com/ajax/en/transliterate";

        IHttpRequestManager httpRequestManager;

        public TranslitterationDotComTransliterator(IHttpRequestManager httpRequestManager)
        {
            this.httpRequestManager = httpRequestManager;
        }

        public async Task<string> Transliterate(string text, string language, string scheme)
        {
            IDictionary<string, string> formData = new Dictionary<string, string>
            {
                { "text", text},
                { "tlang", language },
                { "script", "latn" },
                { "scheme", scheme }
            };

            string rawTransliteratedText = await httpRequestManager.Post(URL, formData);

            return ApplyLanguageSpecificFixes(rawTransliteratedText, language).Replace("ack:::", "");
        }

        private string ApplyLanguageSpecificFixes(string text, string language)
        {
            if (language == "bel" || language == "bul")
            {
                return Regex.Replace(text, "([a-zA-Z])H", "$1h");
            }
            else if (language == "chv")
            {
                return text.Replace("i͡", "y");
            }
            else if (language == "rus")
            {
                return Regex.Replace(text, "([a-zA-Z])Y", "$1y");
            }

            return text;
        }
    }
}
