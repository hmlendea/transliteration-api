using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.Service
{
    public class TransliterationService : ITransliterationService
    {
        IDictionary<string, string> cache;

        IGujaratiTransliterator gujaratiTransliterator;
        IPinyinTransliterator pinyinTransliterator;
        IPodolakTransliterator podolakTransliterator;
        IRomajiTransliterator romajiTransliterator;
        ITransliterateDotComTransliterator transliterateDotComTransliterator;
        ITranslitterationDotComTransliterator translitterationDotComTransliterator;
        IUshuaiaTransliterator ushuaiaTransliterator;

        public TransliterationService(
            IGujaratiTransliterator gujaratiTransliterator,
            IPinyinTransliterator pinyinTransliterator,
            IPodolakTransliterator podolakTransliterator,
            IRomajiTransliterator romajiTransliterator,
            ITransliterateDotComTransliterator transliterateDotComTransliterator,
            ITranslitterationDotComTransliterator translitterationDotComTransliterator,
            IUshuaiaTransliterator ushuaiaTransliterator)
        {
            this.cache = new Dictionary<string, string>();
            
            this.gujaratiTransliterator = gujaratiTransliterator;
            this.pinyinTransliterator = pinyinTransliterator;
            this.podolakTransliterator = podolakTransliterator;
            this.romajiTransliterator = romajiTransliterator;
            this.transliterateDotComTransliterator = transliterateDotComTransliterator;
            this.translitterationDotComTransliterator = translitterationDotComTransliterator;
            this.ushuaiaTransliterator = ushuaiaTransliterator;
        }

        public async Task<string> Transliterate(string text, string language)
        {
            string normalisedText = NormaliseText(text);

            string cacheKey = $"{normalisedText}_${language}";

            if (cache.ContainsKey(cacheKey))
            {
                return cache[cacheKey];
            }

            string transliteratedText = await GetTransliteratedText(normalisedText, language);

            cache.Add(cacheKey, transliteratedText);

            return transliteratedText;
        }

        async Task<string> GetTransliteratedText(string text, string language)
        {
            switch (language)
            {
                case "ab":
                    return await translitterationDotComTransliterator.Transliterate(text, "abk", "iso-9");
                case "ady":
                    return await translitterationDotComTransliterator.Transliterate(text, "ady", "iso-9");
                case "ba":
                    return await translitterationDotComTransliterator.Transliterate(text, "bak", "iso-9");
                case "be":
                    return await translitterationDotComTransliterator.Transliterate(text, "bel", "national");
                case "bg":
                    return await translitterationDotComTransliterator.Transliterate(text, "bul", "streamlined");
                case "cu":
                    return await podolakTransliterator.Transliterate(text, language);
                case "cv":
                    return await translitterationDotComTransliterator.Transliterate(text, "chv", "ala-lc");
                case "el":
                    return await translitterationDotComTransliterator.Transliterate(text, "chv", "ala-lc");
                case "gu":
                    return gujaratiTransliterator.Transliterate(text);
                case "hy":
                    return await translitterationDotComTransliterator.Transliterate(text, "xcl", "iso-9985");
                case "hyw":
                    return await translitterationDotComTransliterator.Transliterate(text, "hye", "ala-lc");
                case "ja":
                    return await romajiTransliterator.Transliterate(text);
                case "ka":
                    return await translitterationDotComTransliterator.Transliterate(text, "kat", "national");
                case "kk":
                    return await translitterationDotComTransliterator.Transliterate(text, "kaz", "national");
                case "ky":
                    return await translitterationDotComTransliterator.Transliterate(text, "kir", "iso-9");
                case "mk":
                    return await translitterationDotComTransliterator.Transliterate(text, "mkd", "bgn-pcgn");
                case "mn":
                    return await ushuaiaTransliterator.Transliterate(text, "mongolian_mns_transliterate");
                case "os":
                    return await translitterationDotComTransliterator.Transliterate(text, "oss", "iso-9");
                case "ru":
                    return await translitterationDotComTransliterator.Transliterate(text, "rus", "bgn-pcgn");
                case "sr":
                    return await translitterationDotComTransliterator.Transliterate(text, "srp", "national");
                case "udm":
                    return await translitterationDotComTransliterator.Transliterate(text, "udm", "bgn-pcgn");
                case "uk":
                    return await translitterationDotComTransliterator.Transliterate(text, "ukr", "bgn-pcgn");
                case "zh":
                case "zh-hans":
                    return pinyinTransliterator.Transliterate(text);
                default:
                    throw new ArgumentException($"The \"{language}\" language is not supported");
            }
        }

        string NormaliseText(string text)
        {
            string normalisedText = text;
            normalisedText = Regex.Replace(normalisedText, "^[\\s\r\n]*", "");
            normalisedText = Regex.Replace(normalisedText, "[\\s\r\n]*$", "");

            return normalisedText;
        }
    }
}
