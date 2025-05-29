using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;

using NuciDAL.Repositories;

using TransliterationAPI.Configuration;
using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.Service
{
    public class TransliterationService(
        ITransliteratorFactory transliteratorFactory,
        IRepository<CachedTransliteration> cache,
        CacheSettings cacheSettings) : ITransliterationService
    {
        public async Task<string> Transliterate(string text, string languageCode)
        {
            if (Language.GetAll().All(language => !language.Code.Equals(languageCode)))
            {
                return text;
            }

            string normalisedText = NormaliseText(text);
            string cacheId = GetCacheId(normalisedText, languageCode);

            CachedTransliteration transliteration = cache.TryGet(cacheId);

            if (transliteration is not null)
            {
                return transliteration.TransliteratedText;
            }

            transliteration = new CachedTransliteration()
            {
                Id = cacheId,
                TransliteratedText = await TryGetTransliteratedText(normalisedText, languageCode)
            };

            if (!string.IsNullOrWhiteSpace(transliteration.TransliteratedText))
            {
                cache.Add(transliteration);
                cache.ApplyChanges();
            }

            return transliteration.TransliteratedText;
        }

        async Task<string> TryGetTransliteratedText(string text, string languageCode)
        {
            try
            {
                return await GetTransliteratedText(text, languageCode);
            }
            catch
            {
                return null;
            }
        }

        async Task<string> GetTransliteratedText(string text, string languageCode)
        {
            Language language = Language.FromCode(languageCode);

            if (language.Transliterator.Equals(nameof(PodolakTransliterator)) ||
                language.Transliterator.Equals(nameof(TranslitterationDotComTransliterator)) ||
                language.Transliterator.Equals(nameof(UshuaiaTransliterator)))
            {
                return await transliteratorFactory
                    .GetExternalTransliterator(language)
                    .Transliterate(text, languageCode);
            }

            return transliteratorFactory
                .GetTransliterator(language)
                .Transliterate(text, languageCode);
        }

        static string NormaliseText(string text)
        {
            string normalisedText = text;

            normalisedText = Regex.Replace(normalisedText, "^[\\s\r\n]*", "");
            normalisedText = Regex.Replace(normalisedText, "[\\s\r\n]*$", "");

            return normalisedText;
        }

        string GetCacheId(string text, string languageCode)
        {
            string textUnicodes = string.Join('-', text.Select(c => (int)c));
            string cacheKey = $"{languageCode}_{textUnicodes}_{cacheSettings.ApplicationVersion}";
            cacheKey = GetSha256FromString(cacheKey);

            return cacheKey;
        }

        static string GetSha256FromString(string strData)
        {
            byte[] message = Encoding.ASCII.GetBytes(strData);
            string hex = "";

            byte[] hashValue = SHA256.HashData(message);

            foreach (byte x in hashValue)
            {
                hex += string.Format("{0:x2}", x);
            }

            return hex;
        }
    }
}
