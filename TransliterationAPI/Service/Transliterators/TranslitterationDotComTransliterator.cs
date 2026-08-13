using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using NuciExtensions;

using NuciLog.Core;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public sealed class TranslitterationDotComTransliterator(
        IHttpRequestManager httpRequestManager,
        ILogger logger)
        : ExternalTransliterator(logger), IExternalTransliterator
    {
        private static readonly string EndpointUrl = "https://www.translitteration.com/ajax/en/transliterate";
        private static readonly string ResponsePrefix = "ack:::";
        private static readonly string Script = "latn";
        private static readonly IDictionary<string, string> TranslitterationDotComTargetLanguageCodes =
            new Dictionary<string, string>()
            {
                { Language.Abkhaz, "abk" },
                { Language.Adyghe, "ady" },
                { Language.Armenian, "xcl" },
                { Language.Bashkir, "bak" },
                { Language.Georgian, "kat" },
                { Language.Inuttitut, "iku" },
                { Language.Kyrgyz, "kir" },
                { Language.Ossetic, "oss" },
                { Language.Udmurt, "udm" },
                { Language.WesternArmenian, "hye" }
            };
        private static readonly IDictionary<string, string> TranslitterationDotComSchemes =
            new Dictionary<string, string>()
            {
                { Language.Abkhaz, "iso-9" },
                { Language.Adyghe, "iso-9" },
                { Language.Armenian, "iso-9985" },
                { Language.Bashkir, "iso-9" },
                { Language.Georgian, "national" },
                { Language.Inuttitut, "canadian-aboriginal-syllabics" },
                { Language.Kyrgyz, "iso-9" },
                { Language.Ossetic, "iso-9" },
                { Language.Udmurt, "bgn-pcgn" },
                { Language.WesternArmenian, "ala-lc" }
            };

        protected override async Task<string> PerformTransliteration(string text, Language language)
        {
            string transliteratedText = await SendTransliterationRequest(text, language);

            return ApplyFixes(transliteratedText, language);
        }

        private static string ApplyFixes(string text, Language language)
        {
            string fixedText = text;

            if (language.Equals(Language.Inuttitut))
            {
                fixedText = fixedText.Replace("ᐄ", "i");
                fixedText = fixedText.Replace("ᐆ", "u");
            }

            if (language.Equals(Language.Armenian) ||
                language.Equals(Language.Georgian) ||
                language.Equals(Language.Inuttitut) ||
                language.Equals(Language.Kyrgyz))
            {
                fixedText = fixedText.ToTitleCase();
            }

            return fixedText;
        }

        private async Task<string> SendTransliterationRequest(string text, string languageCode)
        {
            if (!TranslitterationDotComTargetLanguageCodes.TryGetValue(languageCode, out string targetLanguageCode) ||
                !TranslitterationDotComSchemes.TryGetValue(languageCode, out string transliterationScheme))
            {
                throw new ArgumentException($"The \"{languageCode}\" language is not supported by {nameof(TranslitterationDotComTransliterator)}.");
            }

            Dictionary<string, string> formData = new()
            {
                { "text", text },
                { "tlang", targetLanguageCode },
                { "script", Script },
                { "scheme", transliterationScheme }
            };

            string response = await httpRequestManager.Post(EndpointUrl, formData);

            return response.Replace(ResponsePrefix, "");
        }
    }
}
