using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NuciExtensions;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class UshuaiaTransliterator : IExternalTransliterator
    {
        IHttpRequestManager httpRequestManager;

        string sessionCookieValue;
        DateTime cookieDate;

        public UshuaiaTransliterator(IHttpRequestManager httpRequestManager)
        {
            this.httpRequestManager = httpRequestManager;
        }

        public async Task<string> Transliterate(string text, string languageCode)
        {
            string transliteratedText = await SendTransliterationRequest(text, languageCode);

            return ApplyFixes(transliteratedText, languageCode);
        }

        string ApplyFixes(string text, string languageCode)
        {
            string fixedText = text;

            if (languageCode.Equals(Language.Bengali) ||
                languageCode.Equals(Language.Hindi) ||
                languageCode.Equals(Language.Kannada) ||
                languageCode.Equals(Language.Korean) ||
                languageCode.Equals(Language.Malayalam) ||
                languageCode.Equals(Language.Sanskrit) ||
                languageCode.Equals(Language.Sinhala) ||
                languageCode.Equals(Language.Tamil) ||
                languageCode.Equals(Language.Telugu))
            {
                fixedText = fixedText.ToTitleCase();
            }

            if (languageCode.Equals(Language.Korean))
            {
                fixedText = fixedText
                    .Replace("ǒ", "ŏ")
                    .Replace("’", "");

                fixedText = Regex.Replace(fixedText, "^\"(.*)\"$", "$1");
            }

            return fixedText;
        }

        private async Task<string> SendTransliterationRequest(string text, string languageCode)
        {
            IDictionary<string, string> formData = new Dictionary<string, string>
            {
                { "text", text},
                { "lang", string.Empty }
            };

            if ((DateTime.Now - cookieDate).TotalMinutes > 5)
            {
                string cookies = await httpRequestManager.RetrieveCookies("https://www.ushuaia.pl/transliterate/");
                sessionCookieValue = Regex.Replace(cookies, "translit=([^;]*).*", "$1");
                cookieDate = DateTime.Now;
            }

            if (languageCode.Equals(Language.Bengali))
            {
                formData["lang"] = "bengali_iso_transliterate";
            }
            else if (languageCode.Equals(Language.Hindi))
            {
                formData["lang"] = "devanagari_hunt_transcribe";
            }
            else if (languageCode.Equals(Language.Kannada))
            {
                formData["lang"] = "kannada_iso_transliterate";
            }
            else if (languageCode.Equals(Language.Korean))
            {
                formData["lang"] = "hangul_mr_transcribe";
            }
            else if (languageCode.Equals(Language.Malayalam))
            {
                formData["lang"] = "malayalam_iso_transliterate";
            }
            else if (languageCode.Equals(Language.Mongol))
            {
                formData["lang"] = "mongolian_mns_transliterate";
            }
            else if (languageCode.Equals(Language.Sanskrit))
            {
                formData["lang"] = "devanagari_iast_transliterate";
            }
            else if (languageCode.Equals(Language.Sinhala))
            {
                formData["lang"] = "sinhala_iso_transliterate";
            }
            else if (languageCode.Equals(Language.Tamil))
            {
                formData["lang"] = "tamil_iso_transliterate";
            }
            else if (languageCode.Equals(Language.Telugu))
            {
                formData["lang"] = "telugu_iso_transliterate";
            }
            else
            {
                throw new ArgumentException($"The \"{languageCode}\" language is not supported by {nameof(UshuaiaTransliterator)}!");
            }

            IDictionary<string, string> headers = new Dictionary<string, string>
            {
                { "Cookie", $"translit={sessionCookieValue};lastlang={formData["lang"]}" }
            };

            return await httpRequestManager.Post("https://www.ushuaia.pl/transliterate/transliterate.php", formData, headers);
        }
    }
}
