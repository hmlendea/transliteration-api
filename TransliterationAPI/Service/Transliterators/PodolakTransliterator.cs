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
            string html = await httpRequestManager.Post(url, formData);

            return ExtractResultFromHtml(html);
        }

        private string GetUrl(string language)
        {
            const string url = "https://podolak.net/en/transliteration";

            if (language == "cu")
            {
                return $"{url}/old-church-slavonic";
            }
            else
            {
                throw new ArgumentException($"Unsupported language \"{language}\"");
            }
        }

        private string ExtractResultFromHtml(string html)
        {
            string line = html
                .Split(new [] { '\r', '\n' })
                .First(x => x.Contains("ausgabe"));

            return Regex.Replace(line, "[^>]*>([^<]*)</textarea>.*", "$1");
        }
    }
}
