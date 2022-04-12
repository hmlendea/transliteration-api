using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TransliterationAPI.Service.Transliterators
{
    public class PodolakTransliterator : IPodolakTransliterator
    {
        IHttpRequestManager httpRequestManager;

        public PodolakTransliterator(IHttpRequestManager httpRequestManager)
        {
            this.httpRequestManager = httpRequestManager;
        }

        public async Task<string> Transliterate(string text, string language)
        {
            IDictionary<string, string> formData = new Dictionary<string, string>
            {
                { "quelltext", language},
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

        private string GetUrl(string language)
        {
            const string url = "https://podolak.net/en/transliteration";

            if (!language.Equals("cu"))
            {
                throw new ArgumentException($"Unsupported language \"{language}\"");
            }
            
            return $"{url}/old-church-slavonic";
        }

        private string ExtractResultFromResponse(string response)
        {
            string line = response
                .Split(new [] { '\r', '\n' })
                .First(x => x.Contains("ausgabe"));

            return Regex.Replace(line, "[^>]*>([^<]*)</textarea>.*", "$1");
        }
    }
}
