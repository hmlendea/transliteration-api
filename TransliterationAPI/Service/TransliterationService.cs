using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using TransliterationAPI.Service.Transliterators;
using System.Text;

namespace TransliterationAPI.Service
{
    public class TransliterationService : ITransliterationService
    {
        IDictionary<string, string> cache;

        IAncientGreekTransliterator ancientGreekTransilterator;
        IArabicTransliterator arabicTransliterator;
        ICopticTransliterator copticTransliterator;
        ICyrillicTransliterator cyrillicTransliterator;
        IGujaratiTransliterator gujaratiTransliterator;
        IHebrewTransliterator hebrewTransliterator;
        IMarathiTransliterator marathiTransliterator;
        IPinyinTransliterator pinyinTransliterator;
        IPodolakTransliterator podolakTransliterator;
        IRomajiTransliterator romajiTransliterator;
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
            IRomajiTransliterator romajiTransliterator,
            IThailitTransliterator thailitTransliterator,
            ITranslitterationDotComTransliterator translitterationDotComTransliterator,
            IUshuaiaTransliterator ushuaiaTransliterator)
        {
            this.cache = new Dictionary<string, string>();

            this.ancientGreekTransilterator = ancientGreekTransilterator;
            this.arabicTransliterator = arabicTransliterator;
            this.copticTransliterator = copticTransliterator;
            this.cyrillicTransliterator = cyrillicTransliterator;
            this.gujaratiTransliterator = gujaratiTransliterator;
            this.hebrewTransliterator = hebrewTransliterator;
            this.marathiTransliterator = marathiTransliterator;
            this.pinyinTransliterator = pinyinTransliterator;
            this.podolakTransliterator = podolakTransliterator;
            this.romajiTransliterator = romajiTransliterator;
            this.thailitTransliterator = thailitTransliterator;
            this.translitterationDotComTransliterator = translitterationDotComTransliterator;
            this.ushuaiaTransliterator = ushuaiaTransliterator;
        }

        public async Task<string> Transliterate(string text, string languageCode)
        {
            string normalisedText = NormaliseText(text);
            string cacheKey = GetCacheId(normalisedText, languageCode);

            if (cache.ContainsKey(cacheKey))
            {
                return cache[cacheKey];
            }

            string transliteratedText = await TryGetTransliteratedText(normalisedText, languageCode);

            if (!string.IsNullOrWhiteSpace(transliteratedText))
            {
                cache.Add(cacheKey, transliteratedText);
            }

            return transliteratedText;
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
            switch (languageCode)
            {
                case "ab": // Abkhaz
                    return await translitterationDotComTransliterator.Transliterate(text, "abk", "iso-9");
                case "ady": // Adyghe
                    return await translitterationDotComTransliterator.Transliterate(text, "ady", "iso-9");
                case "ar": // Arabic
                case "ary": // Maghrebi Arabic
                case "arz": // Egyptian Arabic
                    return arabicTransliterator.Transliterate(text, languageCode);
                case "ba": // Bashkir
                    return await translitterationDotComTransliterator.Transliterate(text, "bak", "iso-9");
                case "be": // Belarussian
                    return await translitterationDotComTransliterator.Transliterate(text, "bel", "national");
                case "bg": // Bulgarian
                    return cyrillicTransliterator.Transliterate(text, languageCode);
                case "bn": // Bengali
                    return await ushuaiaTransliterator.Transliterate(text, "bengali_iso_transliterate");
                case "cop": // Coptic
                    return copticTransliterator.Transliterate(text, languageCode);
                case "cu": // Old Church Slavonic
                    return await podolakTransliterator.Transliterate(text, languageCode);
                case "cv": // Chuvash
                    return await translitterationDotComTransliterator.Transliterate(text, "chv", "ala-lc");
                case "el": // Greek
                    return await translitterationDotComTransliterator.Transliterate(text, "gre", "un-elot");
                case "grc": // Ancient Greek
                    return ancientGreekTransilterator.Transliterate(text, languageCode);
                case "grc-dor": // Ancient Greek - Doric
                    return ancientGreekTransilterator.Transliterate(text, languageCode);
                case "gu": // Gujarati
                    return gujaratiTransliterator.Transliterate(text);
                case "he": // Hebrew
                    return hebrewTransliterator.Transliterate(text);
                case "hi": // Hindi
                    return await ushuaiaTransliterator.Transliterate(text, "devanagari_hunt_transcribe");
                case "hy": // Armenian
                    return await translitterationDotComTransliterator.Transliterate(text, "xcl", "iso-9985");
                case "hyw": // Western Armenian
                    return await translitterationDotComTransliterator.Transliterate(text, "hye", "ala-lc");
                case "iu": // Inuttitut
                    return await translitterationDotComTransliterator.Transliterate(text, "iku", "canadian-aboriginal-syllabics");
                case "ja": // Japanese
                    return await romajiTransliterator.Transliterate(text);
                case "ka": // Georgian
                    return await translitterationDotComTransliterator.Transliterate(text, "kat", "national");
                case "kk": // Kazakh
                    return await translitterationDotComTransliterator.Transliterate(text, "kaz", "national");
                case "kn": // Kannada
                    return await ushuaiaTransliterator.Transliterate(text, "kannada_iso_transliterate");
                case "ko": // Korean
                    return await ushuaiaTransliterator.Transliterate(text, "hangul_mr_transcribe");
                case "ky": // Kyrgyz
                    return await translitterationDotComTransliterator.Transliterate(text, "kir", "iso-9");
                case "mk": // Macedonian Slavic
                    return await translitterationDotComTransliterator.Transliterate(text, "mkd", "bgn-pcgn");
                case "ml": // Malayalam
                    return await ushuaiaTransliterator.Transliterate(text, "malayalam_iso_transliterate");
                case "mn": // Mongol
                    return await ushuaiaTransliterator.Transliterate(text, "mongolian_mns_transliterate");
                case "mr": // Marathi
                    return marathiTransliterator.Transliterate(text);
                case "os": // Ossetic
                    return await translitterationDotComTransliterator.Transliterate(text, "oss", "iso-9");
                case "ru": // Russian
                    return cyrillicTransliterator.Transliterate(text, languageCode);
                case "sa": // Sanskrit
                    return await ushuaiaTransliterator.Transliterate(text, "devanagari_iast_transliterate");
                case "si": // Sinhala
                    return await ushuaiaTransliterator.Transliterate(text, "sinhala_iso_transliterate");
                case "sr": // Serbian
                    return await translitterationDotComTransliterator.Transliterate(text, "srp", "national");
                case "ta": // Tamil
                    return await ushuaiaTransliterator.Transliterate(text, "tamil_iso_transliterate");
                case "te": // Telugu
                    return await ushuaiaTransliterator.Transliterate(text, "telugu_iso_transliterate");
                case "th": // Thai
                    return await thailitTransliterator.Transliterate(text);
                case "udm": // Udmurt
                    return await translitterationDotComTransliterator.Transliterate(text, "udm", "bgn-pcgn");
                case "uk": // Ukrainian
                    return await translitterationDotComTransliterator.Transliterate(text, "ukr", "bgn-pcgn");
                case "zh": // Chinese
                case "zh-hans": // Simplified Chinese
                    return pinyinTransliterator.Transliterate(text);
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
            string cacheKey = $"{languageCode}_{textUnicodes}";
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
