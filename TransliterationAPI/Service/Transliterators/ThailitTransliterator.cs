using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NuciExtensions;

namespace TransliterationAPI.Service.Transliterators
{
    public class ThailitTransliterator : IThailitTransliterator
    {
        IHttpRequestManager httpRequestManager;

        public ThailitTransliterator(IHttpRequestManager httpRequestManager)
        {
            this.httpRequestManager = httpRequestManager;
        }

        public async Task<string> Transliterate(string text)
        {
            IDictionary<string, string> formData = new Dictionary<string, string>
            {
                { "s", text}
            };

            string response = await httpRequestManager.Post("https://thailit.com/transliterate.php", formData);

            return ExtractResultFromResponse(response).ToTitleCase();
        }

        private string ApplyFixes(string text)
        {
            string fixedText = text.ToTitleCase();

            return fixedText;
        }

        private string ExtractResultFromResponse(string response)
        {
            string line = response
                .Split(new [] { '\r', '\n' })
                .First(x => x.Contains("div style=\"border:solid 1px gray;background:rgba(255,255,255,0.75);padding:15px\""));

            return Regex.Replace(line, ".*<div[^>]*>([^<]*).*", "$1");
        }
    }
}
