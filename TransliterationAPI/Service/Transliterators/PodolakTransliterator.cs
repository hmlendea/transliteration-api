using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NuciLog.Core;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class PodolakTransliterator(
        IHttpRequestManager httpRequestManager,
        ILogger logger)
        : ExternalTransliterator(logger), IExternalTransliterator
    {
        private static string OldChurchSlavonicTransliterationUrl
            => "https://podolak.net/en/transliteration/old-church-slavonic";

        protected override async Task<string> PerformTransliteration(string text, Language language)
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
            if (!language.Equals(Language.OldChurchSlavonic))
            {
                throw new ArgumentException($"Unsupported language \"{language}\"");
            }

            return OldChurchSlavonicTransliterationUrl;
        }

        private static string ExtractResultFromResponse(string response)
        {
            string line = response
                .Split(['\r', '\n'])
                .First(responseLine => responseLine.Contains("ausgabe"));
            Match resultMatch = Regex.Match(line, "[^>]*>([^<]*)</textarea>.*");

            if (!resultMatch.Success)
            {
                throw new FormatException("The Podolak response does not contain a valid transliteration result.");
            }

            return resultMatch.Groups[1].Value;
        }
    }
}
