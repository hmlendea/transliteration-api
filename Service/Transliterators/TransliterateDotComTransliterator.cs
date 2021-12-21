using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NuciExtensions;

namespace TransliterationAPI.Service.Transliterators
{
    public class TransliterateDotComTransliterator : ITransliterateDotComTransliterator
    {
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

            string response = await httpRequestManager.Post("https://transliterate.com/Home/Transliterate", formData);
            string result = ExtractResultFromResponse(response);

            return ApplyFixes(result, language);
        }

        private string ApplyFixes(string text, string language)
        {
            string fixedText = text.Replace("ack:::", "");

            if (language == "el")
            {
                fixedText = Regex.Replace(fixedText, "^Mp", "B");
                fixedText = Regex.Replace(fixedText, " Mp", "B");
                fixedText = Regex.Replace(fixedText, "^Nk", "G");
                fixedText = Regex.Replace(fixedText, " Nk", "G");
                fixedText = Regex.Replace(fixedText, "^Nt", "D");
                fixedText = Regex.Replace(fixedText, " Nt", "D");
                fixedText = Regex.Replace(fixedText, "mp([aáioó])", "b$1");
                fixedText = Regex.Replace(fixedText, "nknt", "gd");
                fixedText = Regex.Replace(fixedText, "ntm", "dm");
                fixedText = Regex.Replace(fixedText, "[rs]mp", "$1b");
                fixedText = Regex.Replace(fixedText, "rnk", "rk");
                fixedText = Regex.Replace(fixedText, "rnt", "rd");
                fixedText = Regex.Replace(fixedText, "snt", "sht");
                fixedText = Regex.Replace(fixedText, "([A-Za-z])'([A-Za-z])", "$1$2");
                fixedText = Regex.Replace(fixedText, "([Rr])(a|ṓ)", "$1h$2");
            }
            else if (language == "he")
            {
                // Add vowels
                fixedText = Regex.Replace(fixedText, "qh", "qah");
                fixedText = Regex.Replace(fixedText, "e(ae|\\s)", "$1", RegexOptions.IgnoreCase);
                fixedText = Regex.Replace(fixedText, "([b-df-hj-np-tv-z])([b-df-gj-np-tv-z])", "$1e$2", RegexOptions.IgnoreCase);

                // Add diacritics
                //fixedText = fixedText.Replace("sh", "š");

                // Fix casing
                fixedText = fixedText.ToTitleCase();
                fixedText = fixedText.Replace(" Dh ", " dh ");
                fixedText = Regex.Replace(fixedText, " '([a-z])", m => " '" + m.Groups[1].Value.ToUpperInvariant());
                fixedText = Regex.Replace(fixedText, "^'([a-z])", m => "'" + m.Groups[1].Value.ToUpperInvariant());
            }

            return fixedText;
        }

        private string ExtractResultFromResponse(string response)
        {
            if (!response.Contains("\"latin\""))
            {
                return null;
            }
            
            Regex regexDecoder = new Regex(@"\\u(?<Value>[a-zA-Z0-9]{4})", RegexOptions.Compiled);
            string latinText = Regex.Replace(response, ".*\"latin\":\"([^\"]*).*", "$1");

            return regexDecoder.Replace(
                latinText,
                m => ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString()
            );
        }
    }
}
