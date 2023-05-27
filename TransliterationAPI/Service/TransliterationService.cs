using System;
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
    public class TransliterationService : ITransliterationService
    {
        IRepository<CachedTransliteration> cache;
        CacheSettings cacheSettings;

        IAncientGreekTransliterator ancientGreekTransilterator;
        IArabicTransliterator arabicTransliterator;
        ICopticTransliterator copticTransliterator;
        ICyrillicTransliterator cyrillicTransliterator;
        IGujaratiTransliterator gujaratiTransliterator;
        IHebrewTransliterator hebrewTransliterator;
        IMarathiTransliterator marathiTransliterator;
        IPinyinTransliterator pinyinTransliterator;
        IPodolakTransliterator podolakTransliterator;
        IJapaneseTransliterator japaneseTransliterator;
        IThailitTransliterator thailitTransliterator;
        ITranslitterationDotComTransliterator translitterationDotComTransliterator;
        IUshuaiaTransliterator ushuaiaTransliterator;

        public TransliterationService(
            IAncientGreekTransliterator ancientGreekTransilterator,
            IArabicTransliterator arabicTransliterator,
            ICyrillicTransliterator cyrillicTransliterator,
            IGujaratiTransliterator gujaratiTransliterator,
            ICopticTransliterator copticTransliterator,
            IHebrewTransliterator hebrewTransliterator,
            IMarathiTransliterator marathiTransliterator,
            IPinyinTransliterator pinyinTransliterator,
            IPodolakTransliterator podolakTransliterator,
            IJapaneseTransliterator japaneseTransliterator,
            IThailitTransliterator thailitTransliterator,
            ITranslitterationDotComTransliterator translitterationDotComTransliterator,
            IUshuaiaTransliterator ushuaiaTransliterator,
            IRepository<CachedTransliteration> cache,
            CacheSettings cacheSettings)
        {
            this.cache = cache;
            this.cacheSettings = cacheSettings;

            this.ancientGreekTransilterator = ancientGreekTransilterator;
            this.arabicTransliterator = arabicTransliterator;
            this.copticTransliterator = copticTransliterator;
            this.cyrillicTransliterator = cyrillicTransliterator;
            this.gujaratiTransliterator = gujaratiTransliterator;
            this.hebrewTransliterator = hebrewTransliterator;
            this.marathiTransliterator = marathiTransliterator;
            this.pinyinTransliterator = pinyinTransliterator;
            this.podolakTransliterator = podolakTransliterator;
            this.japaneseTransliterator = japaneseTransliterator;
            this.thailitTransliterator = thailitTransliterator;
            this.translitterationDotComTransliterator = translitterationDotComTransliterator;
            this.ushuaiaTransliterator = ushuaiaTransliterator;
        }

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

            if (language.Transliterator.Equals(nameof(AncientGreekTransliterator)))
            {
                return ancientGreekTransilterator.Transliterate(text, languageCode);
            }
            else if (language.Transliterator.Equals(nameof(ArabicTransliterator)))
            {
                return arabicTransliterator.Transliterate(text, languageCode);
            }
            else if (language.Transliterator.Equals(nameof(CopticTransliterator)))
            {
                return copticTransliterator.Transliterate(text, languageCode);
            }
            else if (language.Transliterator.Equals(nameof(CyrillicTransliterator)))
            {
                return cyrillicTransliterator.Transliterate(text, languageCode);
            }
            else if (language.Transliterator.Equals(nameof(GujaratiTransliterator)))
            {
                return gujaratiTransliterator.Transliterate(text);
            }
            else if (language.Transliterator.Equals(nameof(HebrewTransliterator)))
            {
                return hebrewTransliterator.Transliterate(text);
            }
            else if (language.Transliterator.Equals(nameof(JapaneseTransliterator)))
            {
                return japaneseTransliterator.Transliterate(text);
            }
            else if (language.Transliterator.Equals(nameof(MarathiTransliterator)))
            {
                return marathiTransliterator.Transliterate(text);
            }
            else if (language.Transliterator.Equals(nameof(PinyinTransliterator)))
            {
                return pinyinTransliterator.Transliterate(text);
            }
            else if (language.Transliterator.Equals(nameof(PodolakTransliterator)))
            {
                return await podolakTransliterator.Transliterate(text, languageCode);
            }
            else if (language.Transliterator.Equals(nameof(ThailitTransliterator)))
            {
                return await thailitTransliterator.Transliterate(text);
            }
            else if (language.Transliterator.Equals(nameof(TranslitterationDotComTransliterator)))
            {
                return await translitterationDotComTransliterator.Transliterate(text, languageCode);
            }

            switch (languageCode)
            {
                case "bn": // Bengali
                    return await ushuaiaTransliterator.Transliterate(text, "bengali_iso_transliterate");
                case "hi": // Hindi
                    return await ushuaiaTransliterator.Transliterate(text, "devanagari_hunt_transcribe");
                case "kn": // Kannada
                    return await ushuaiaTransliterator.Transliterate(text, "kannada_iso_transliterate");
                case "ko": // Korean
                    return await ushuaiaTransliterator.Transliterate(text, "hangul_mr_transcribe");
                case "ml": // Malayalam
                    return await ushuaiaTransliterator.Transliterate(text, "malayalam_iso_transliterate");
                case "mn": // Mongol
                    return await ushuaiaTransliterator.Transliterate(text, "mongolian_mns_transliterate");
                case "sa": // Sanskrit
                    return await ushuaiaTransliterator.Transliterate(text, "devanagari_iast_transliterate");
                case "si": // Sinhala
                    return await ushuaiaTransliterator.Transliterate(text, "sinhala_iso_transliterate");
                case "ta": // Tamil
                    return await ushuaiaTransliterator.Transliterate(text, "tamil_iso_transliterate");
                case "te": // Telugu
                    return await ushuaiaTransliterator.Transliterate(text, "telugu_iso_transliterate");
                default:
                    throw new ArgumentException($"The \"{languageCode}\" language is not supported");
            }
        }

        string NormaliseText(string text)
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

        string GetSha256FromString(string strData)
        {
            byte[] message = Encoding.ASCII.GetBytes(strData);
            SHA256 hashString = SHA256.Create();
            string hex = "";

            byte[] hashValue = hashString.ComputeHash(message);

            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }

            return hex;
        }
    }
}
