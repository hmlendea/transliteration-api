using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NuciExtensions;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class TranslitterationDotComTransliterator : ITranslitterationDotComTransliterator
    {
        IHttpRequestManager httpRequestManager;

        public TranslitterationDotComTransliterator(IHttpRequestManager httpRequestManager)
        {
            this.httpRequestManager = httpRequestManager;
        }

        public async Task<string> Transliterate(string text, string languageCode)
        {
            string transliteratedText = await SendTransliterationRequest(text, languageCode);

            return ApplyLanguageSpecificFixes(transliteratedText, languageCode);
        }

        private string ApplyLanguageSpecificFixes(string text, string languageCode)
        {
            string fixedText = text;

            if (languageCode.Equals(Language.Belarussian))
            {
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])H", "$1h");
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])S", "$1s");
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])T", "$1t");
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])U", "$1u");
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])Z", "$1z");
                fixedText = Regex.Replace(fixedText, "([a-zA-Z])Ž", "$1ž");
            }
            else if (languageCode.Equals(Language.Chuvash))
            {
                fixedText = fixedText.Replace("i͡", "y");
            }
            else if (languageCode.Equals(Language.Greek)) // Modern Greek
            {
                fixedText = Regex.Replace(fixedText, "Mή[lt]", "Mí$1");
                fixedText = Regex.Replace(fixedText, "Tή[m]", "Tí$1");
                fixedText = Regex.Replace(fixedText, "ήn([iíί])$", "íni");

                fixedText = Regex.Replace(fixedText, "[m]ή[d]", "$1é$2");
                fixedText = Regex.Replace(fixedText, "ήr([iíί])$", "éri");

                fixedText = Regex.Replace(fixedText, "([a-zA-Z])H", "$1h");

                fixedText = Regex.Replace(fixedText, "[Ά]", "Á");
                fixedText = Regex.Replace(fixedText, "[Έ]", "É");
                fixedText = Regex.Replace(fixedText, "[Ό]", "Ó");
                fixedText = Regex.Replace(fixedText, "[Ύ]", "Ý");

                fixedText = Regex.Replace(fixedText, "ᾶ", "a");
                fixedText = Regex.Replace(fixedText, "ά", "á");
                fixedText = Regex.Replace(fixedText, "[έ]", "é");
                fixedText = Regex.Replace(fixedText, "[ίή]", "í");
                fixedText = Regex.Replace(fixedText, "[ϊ]", "ï");
                fixedText = Regex.Replace(fixedText, "[όώ]", "ó");
                fixedText = Regex.Replace(fixedText, "ς", "s");
                fixedText = Regex.Replace(fixedText, "ύ", "ú");

                fixedText = fixedText.Replace("Mp", "B");
                fixedText = fixedText.Replace("Nt", "D");

                fixedText = Regex.Replace(fixedText, "([r])nt", "$1d");
                fixedText = Regex.Replace(fixedText, "([nrs])mp", "$1b");
            }
            else if (languageCode.Equals(Language.Inuttitut))
            {
                fixedText = fixedText.Replace("ᐄ", "i");
                fixedText = fixedText.Replace("ᐆ", "u");
            }
            else if (languageCode.Equals(Language.Kazakh))
            {
                fixedText = fixedText.Replace("Ц", "C");
                fixedText = fixedText.Replace("Э", "E");
                fixedText = fixedText.Replace("Я", "Ia");
                fixedText = fixedText.Replace("Ю", "Iu");
                fixedText = fixedText.Replace("ь", "’");
                fixedText = fixedText.Replace("ц", "c");
                fixedText = fixedText.Replace("э", "e");
                fixedText = fixedText.Replace("я", "ia");
                fixedText = fixedText.Replace("ю", "iu");
            }

            if (languageCode.Equals(Language.Armenian) ||
                languageCode.Equals(Language.Georgian) ||
                languageCode.Equals(Language.Inuttitut) ||
                languageCode.Equals(Language.Kyrgyz) ||
                languageCode.Equals(Language.MacedonianSlavic))
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
            else if (languageCode.Equals(Language.Greek))
            {
                formData["tlang"] = "gre";
                formData["scheme"] = "un-elot";
            }
            else if (languageCode.Equals(Language.Inuttitut))
            {
                formData["tlang"] = "iku";
                formData["scheme"] = "canadian-aboriginal-syllabics";
            }
            else if (languageCode.Equals(Language.Kazakh))
            {
                formData["tlang"] = "kaz";
                formData["scheme"] = "national";
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
