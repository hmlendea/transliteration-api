using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NuciExtensions;

namespace TransliterationAPI.Service.Transliterators
{
    public class TranslitterationDotComTransliterator : ITranslitterationDotComTransliterator
    {
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

            string response = await httpRequestManager.Post("https://www.translitteration.com/ajax/en/transliterate", formData);
            string rawTransliteratedText = response.Replace("ack:::", "");

            return ApplyLanguageSpecificFixes(rawTransliteratedText, language);
        }

        private string ApplyLanguageSpecificFixes(string text, string language)
        {
            string fixedText = text;

            if (language == "bel" || language == "bul")
            {
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])H", "$1h");
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])S", "$1s");
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])T", "$1t");
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])U", "$1u");
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])Z", "$1z");
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])Ž", "$1ž");
            }
            else if (language == "chv")
            {
                fixedText = fixedText.Replace("i͡", "y");
            }
            else if (language == "gre") // Modern Greek
            {
                fixedText = Regex.Replace(fixedText, "Mή[lt]", "Mí$1");
                fixedText = Regex.Replace(fixedText, "Tή[m]", "Tí$1");
                fixedText = Regex.Replace(fixedText, "ήn([iíί])$", "íni");
                
                fixedText = Regex.Replace(fixedText, "[m]ή[d]", "$1é$2");
                fixedText = Regex.Replace(fixedText, "ήr([iíί])$", "éri");

                fixedText = Regex.Replace(fixedText, "([a-zA-Z])H", "$1h");
                fixedText = Regex.Replace(fixedText, "ᾶ", "a");
                fixedText = Regex.Replace(fixedText, "ά", "á");
                fixedText = Regex.Replace(fixedText, "[έ]", "é");
                fixedText = Regex.Replace(fixedText, "[ίή]", "í");
                fixedText = Regex.Replace(fixedText, "ώ", "ó");
                fixedText = Regex.Replace(fixedText, "ς", "s");
                fixedText = Regex.Replace(fixedText, "ύ", "ú");

                fixedText = fixedText.Replace("Mp", "B");
                fixedText = Regex.Replace(fixedText, "([r])nt", "$1d");
                fixedText = Regex.Replace(fixedText, "([nrs])mp", "$1b");
            }
            else if (language == "iku")
            {
                fixedText = fixedText.Replace("ᐆ", "u");
            }
            else if (language == "kaz")
            {
                fixedText = fixedText.Replace("Ц", "C");
                fixedText = fixedText.Replace("Э", "E");
                fixedText = fixedText.Replace("Я", "Ia");
                fixedText = fixedText.Replace("Ю", "Iu");
                fixedText = fixedText.Replace("ь", "’");
                fixedText = fixedText.Replace("ц", "c");
                fixedText = fixedText.Replace("э", "e");
                fixedText = fixedText.Replace("я", "ia");
                fixedText = fixedText.Replace("ю", "iu");
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
