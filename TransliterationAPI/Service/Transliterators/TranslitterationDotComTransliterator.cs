using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NuciExtensions;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class TranslitterationDotComTransliterator : IExternalTransliterator
    {
        IHttpRequestManager httpRequestManager;

        public TranslitterationDotComTransliterator(IHttpRequestManager httpRequestManager)
        {
            this.httpRequestManager = httpRequestManager;
        }

        public async Task<string> Transliterate(string text, Language language)
        {
            string transliteratedText = await SendTransliterationRequest(text, language);

            return ApplyFixes(transliteratedText, language);
        }

        private string ApplyFixes(string text, Language language)
        {
            string fixedText = text;

            if (language.Equals(Language.Belarussian))
            {
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])H", "$1h");
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])S", "$1s");
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])T", "$1t");
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])U", "$1u");
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])Z", "$1z");
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])Ž", "$1ž");
            }
            else if (language.Equals(Language.Chuvash))
            {
                fixedText = fixedText.Replace("i͡", "y");
            }
            else if (language.Equals(Language.Inuttitut))
            {
                fixedText = fixedText.Replace("ᐄ", "i");
                fixedText = fixedText.Replace("ᐆ", "u");
            }

            if (language.Equals(Language.Armenian) ||
                language.Equals(Language.Georgian) ||
                language.Equals(Language.Inuttitut) ||
                language.Equals(Language.Kyrgyz) ||
                language.Equals(Language.MacedonianSlavic))
            {
                fixedText = fixedText.ToTitleCase();
            }

            return fixedText;
        }

        private async Task<string> SendTransliterationRequest(string text, string languageCode)
        {
            IDictionary<string, string> formData = new Dictionary<string, string>
            {
                { "text", text},
                { "tlang", string.Empty },
                { "script", "latn" },
                { "scheme", string.Empty }
            };

            if (languageCode.Equals(Language.Abkhaz))
            {
                formData["tlang"] = "abk";
                formData["scheme"] = "iso-9";
            }
            else if (languageCode.Equals(Language.Adyghe))
            {
                formData["tlang"] = "ady";
                formData["scheme"] = "iso-9";
            }
            else if (languageCode.Equals(Language.Armenian))
            {
                formData["tlang"] = "xcl";
                formData["scheme"] = "iso-9985";
            }
            else if (languageCode.Equals(Language.Bashkir))
            {
                formData["tlang"] = "bak";
                formData["scheme"] = "iso-9";
            }
            else if (languageCode.Equals(Language.Belarussian))
            {
                formData["tlang"] = "bel";
                formData["scheme"] = "national";
            }
            else if (languageCode.Equals(Language.Chuvash))
            {
                formData["tlang"] = "chv";
                formData["scheme"] = "ala-lc";
            }
            else if (languageCode.Equals(Language.Georgian))
            {
                formData["tlang"] = "kat";
                formData["scheme"] = "national";
            }
            else if (languageCode.Equals(Language.Inuttitut))
            {
                formData["tlang"] = "iku";
                formData["scheme"] = "canadian-aboriginal-syllabics";
            }
            else if (languageCode.Equals(Language.Kyrgyz))
            {
                formData["tlang"] = "kir";
                formData["scheme"] = "iso-9";
            }
            else if (languageCode.Equals(Language.MacedonianSlavic))
            {
                formData["tlang"] = "mkd";
                formData["scheme"] = "bgn-pcgn";
            }
            else if (languageCode.Equals(Language.Ossetic))
            {
                formData["tlang"] = "oss";
                formData["scheme"] = "iso-9";
            }
            else if (languageCode.Equals(Language.Serbian))
            {
                formData["tlang"] = "srp";
                formData["scheme"] = "national";
            }
            else if (languageCode.Equals(Language.Udmurt))
            {
                formData["tlang"] = "udm";
                formData["scheme"] = "bgn-pcgn";
            }
            else if (languageCode.Equals(Language.WesternArmenian))
            {
                formData["tlang"] = "hye";
                formData["scheme"] = "ala-lc";
            }
            else
            {
                throw new ArgumentException($"The \"{languageCode}\" language is not supported by {nameof(TranslitterationDotComTransliterator)}!");
            }

            string response = await httpRequestManager.Post("https://www.translitteration.com/ajax/en/transliterate", formData);

            return response.Replace("ack:::", "");
        }
    }
}
