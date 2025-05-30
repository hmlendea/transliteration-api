using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NuciExtensions;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class UshuaiaTransliterator(IHttpRequestManager httpRequestManager) : IExternalTransliterator
    {
        string sessionCookieValue;
        DateTime cookieDate;

        public async Task<string> Transliterate(string text, Language language)
        {
            string transliteratedText = await SendTransliterationRequest(text, language);

            return ApplyFixes(transliteratedText, language);
        }

        static string ApplyFixes(string text, Language language)
        {
            string fixedText = text;

            if (language.Equals(Language.Bengali) ||
                language.Equals(Language.Hindi) ||
                language.Equals(Language.Kannada) ||
                language.Equals(Language.Malayalam) ||
                language.Equals(Language.Sanskrit) ||
                language.Equals(Language.Sinhala) ||
                language.Equals(Language.Tamil) ||
                language.Equals(Language.Telugu))
            {
                fixedText = fixedText.ToTitleCase();
            }

            return fixedText;
        }

        private async Task<string> SendTransliterationRequest(string text, Language language)
        {
            Dictionary<string, string> formData = new()
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

            if (language.Equals(Language.Bengali))
            {
                formData["lang"] = "bengali_iso_transliterate";
            }
            else if (language.Equals(Language.Hindi))
            {
                formData["lang"] = "devanagari_hunt_transcribe";
            }
            else if (language.Equals(Language.Kannada))
            {
                formData["lang"] = "kannada_iso_transliterate";
            }
            else if (language.Equals(Language.Malayalam))
            {
                formData["lang"] = "malayalam_iso_transliterate";
            }
            else if (language.Equals(Language.Mongol))
            {
                formData["lang"] = "mongolian_mns_transliterate";
            }
            else if (language.Equals(Language.Sanskrit))
            {
                formData["lang"] = "devanagari_iast_transliterate";
            }
            else if (language.Equals(Language.Sinhala))
            {
                formData["lang"] = "sinhala_iso_transliterate";
            }
            else if (language.Equals(Language.Tamil))
            {
                formData["lang"] = "tamil_iso_transliterate";
            }
            else if (language.Equals(Language.Telugu))
            {
                formData["lang"] = "telugu_iso_transliterate";
            }
            else
            {
                throw new ArgumentException($"The \"{language}\" language is not supported by {nameof(UshuaiaTransliterator)}!");
            }

            Dictionary<string, string> headers = new()
            {
                { "Cookie", $"translit={sessionCookieValue};lastlang={formData["lang"]}" }
            };

            string transliteratedText = await httpRequestManager.Post("https://www.ushuaia.pl/transliterate/transliterate.php", formData, headers);

            if (language.Equals(Language.Hindi))
            {
                transliteratedText = ApplyHindiPostProcessing(transliteratedText);
            }

            return transliteratedText;
        }

        static string ApplyHindiPostProcessing(string text)
        {
            string fixedText = text;

            // *a*
            fixedText = Regex.Replace(fixedText, @"([Ss])([r])", "$1a$2");

            return fixedText;
        }
    }
}
