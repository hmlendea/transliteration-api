using System.Collections.Generic;
using System.Threading.Tasks;

namespace TransliterationAPI.Service.Transliterators
{
    public class TransliterationDotCom : ITransliterationDotCom
    {
        private const string URL = "https://www.translitteration.com/ajax/en/transliterate";

        IHttpRequestManager httpRequestManager;

        public TransliterationDotCom(IHttpRequestManager httpRequestManager)
        {
            this.httpRequestManager = httpRequestManager;
        }

        public async Task<string> Transliterate(string text, string language, string scheme)
        {
            IDictionary<string, string> formData = new Dictionary<string, string>
            {
                { "text", text},
                { "tlang", language },
                { "script", "latn" },
                { "scheme", scheme }
            };

            string rawTransliteratedText = await httpRequestManager.Post(URL, formData);

            return rawTransliteratedText.Replace("ack:::", "");
        }
    }
}
