using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NuciExtensions;

namespace TransliterationAPI.Service.Transliterators
{
    public class RomajiTransliterator : IRomajiTransliterator
    {
        const string URL = "http://romaji.me/romaji.cgi";

        IHttpRequestManager httpRequestManager;

        public RomajiTransliterator(IHttpRequestManager httpRequestManager)
        {
            this.httpRequestManager = httpRequestManager;
        }

        public async Task<string> Transliterate(string text)
        {
            IDictionary<string, string> formData = new Dictionary<string, string>
            {
                { "mode", "2"},
                { "text", text }
            };

            string html = await httpRequestManager.Post(URL, formData);

            return ExtractResultFromHtml(html);
        }

        private string ExtractResultFromHtml(string response)
        {
            string line = response;
            
            line = Regex.Replace(line, "<rb>[^<]*</rb>*", "");
            line = Regex.Replace(line, "<rt>", "<rt> ");
            line = Regex.Replace(line, "<[a-z/]*>", "");
            line = Regex.Replace(line, "^\\s*", "");
            line = Regex.Replace(line, "\\s*$", "");

            return line.ToTitleCase();
        }
    }
}
