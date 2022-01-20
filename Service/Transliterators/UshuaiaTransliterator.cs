using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NuciExtensions;

namespace TransliterationAPI.Service.Transliterators
{
    public class UshuaiaTransliterator : IUshuaiaTransliterator
    {
        IHttpRequestManager httpRequestManager;

        string sessionCookieValue;
        DateTime cookieDate;

        public UshuaiaTransliterator(IHttpRequestManager httpRequestManager)
        {
            this.httpRequestManager = httpRequestManager;
        }

        public async Task<string> Transliterate(string text, string mode)
        {
            IDictionary<string, string> formData = new Dictionary<string, string>
            {
                { "text", text },
                { "lang", mode }
            };

            if ((DateTime.Now - cookieDate).TotalMinutes > 5)
            {
                string cookies = await httpRequestManager.RetrieveCookies("https://www.ushuaia.pl/transliterate/");
                sessionCookieValue = Regex.Replace(cookies, "translit=([^;]*).*", "$1");
                cookieDate = DateTime.Now;
            }

            IDictionary<string, string> headers = new Dictionary<string, string>
            {
                { "Cookie", $"translit={sessionCookieValue};lastlang={mode}" }
            };

            string response = await httpRequestManager.Post("https://www.ushuaia.pl/transliterate/transliterate.php", formData, headers);
            string transliteratedText = ApplyFixes(response, mode);

            return transliteratedText;
        }

        string ApplyFixes(string text, string mode)
        {
            string fixedText = text;

            if (mode.Contains("bengali") || 
                mode.Contains("devanagari") || 
                mode.Contains("hangul") || 
                mode.Contains("kannada") ||
                mode.Contains("malayalam") ||
                mode.Contains("sinhala") ||
                mode.Contains("tamil") ||
                mode.Contains("telugu"))
            {
                fixedText = fixedText.ToTitleCase();
            }

            if (mode.Contains("hangul"))
            {
                fixedText = fixedText.Replace("ǒ", "ŏ");
            }
            
            return fixedText;
        }
    }
}
