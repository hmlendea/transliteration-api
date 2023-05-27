using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NuciExtensions;

namespace TransliterationAPI.Service.Transliterators
{
    public class ThailitTransliterator : IExternalTransliterator
    {
        IHttpRequestManager httpRequestManager;

        public ThailitTransliterator(IHttpRequestManager httpRequestManager)
        {
            this.httpRequestManager = httpRequestManager;
        }

        public async Task<string> Transliterate(string text, string languageCode)
        {
            IDictionary<string, string> formData = new Dictionary<string, string>
            {
                { "s", text}
            };

            string response = await httpRequestManager.Post("https://thailit.com/transliterate.php", formData);
            string result = ExtractResultFromResponse(response);

            return ApplyFixes(result);
        }

        private string ApplyFixes(string text)
        {
            string fixedText = text;

            fixedText = fixedText.Replace("- ", "-");
            fixedText = Regex.Replace(fixedText, "\\s\\s*", " ");
            fixedText = fixedText.ToTitleCase();

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
