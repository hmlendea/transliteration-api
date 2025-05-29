using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class PodolakTransliterator(IHttpRequestManager httpRequestManager) : IExternalTransliterator
    {
        public async Task<string> Transliterate(string text, Language language)
        {
            Dictionary<string, string> formData = new()
            {
                { "quelltext", language.Code },
                { "zieltext", "isor9" },
                { "startabfrage", "1" },
                { "text", text },
                { "transliteration", "Transliteration" },
                { "cu_isor9_jer", "3" }
            };

            string url = GetUrl(language);
            string response = await httpRequestManager.Post(url, formData);

            return ExtractResultFromResponse(response);
        }

        private static string GetUrl(Language language)
        {
            const string url = "https://podolak.net/en/transliteration";

            if (!language.Equals(Language.OldChurchSlavonic))
            {
                throw new ArgumentException($"Unsupported language \"{language}\"");
            }

            return $"{url}/old-church-slavonic";
        }

        private static string ExtractResultFromResponse(string response)
        {
            string line = response
                .Split(['\r', '\n'])
                .First(x => x.Contains("ausgabe"));

            return Regex.Replace(line, "[^>]*>([^<]*)</textarea>.*", "$1");
        }
    }
}
