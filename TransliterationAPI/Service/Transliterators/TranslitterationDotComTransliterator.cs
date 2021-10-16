using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NuciExtensions;

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

            string response = await httpRequestManager.Post(URL, formData);
            string rawTransliteratedText = response.Replace("ack:::", "");

            return ApplyLanguageSpecificFixes(rawTransliteratedText, language);
        }

        private string ApplyLanguageSpecificFixes(string text, string language)
        {
            string fixedText = text;

            if (language == "bel" || language == "bul")
            {
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])H", "$1h");
            }
            else if (language == "chv")
            {
                fixedText = fixedText.Replace("i͡", "y");
            }
            else if (language == "iku")
            {
                fixedText = fixedText.Replace("ᐆ", "u");
            }
            else if (language == "rus")
            {
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])Y", "$1y");
            }

            if (language == "iku" || language == "kat" || language == "kir" || language == "xcl")
            {
                fixedText = fixedText.ToTitleCase();
            }

            return fixedText;
        }
    }
}
