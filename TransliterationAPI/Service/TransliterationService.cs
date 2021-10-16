using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.Service
{
    public class TransliterationService : ITransliterationService
    {
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

            switch (language)
            {
                case "ab":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "abk", "iso-9");
                case "ady":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "ady", "iso-9");
                case "ba":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "bak", "iso-9");
                case "be":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "bel", "national");
                case "bg":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "bul", "streamlined");
                case "cu":
                    return await podolakTransliterator.Transliterate(normalisedText, language);
                case "cv":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "chv", "ala-lc");
                case "el":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "chv", "ala-lc");
                case "gu":
                    return gujaratiTransliterator.Transliterate(normalisedText);
                case "hy":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "xcl", "iso-9985");
                case "hyw":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "hye", "ala-lc");
                case "ja":
                    return await romajiTransliterator.Transliterate(normalisedText);
                case "ka":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "kat", "national");
                case "kk":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "kaz", "national");
                case "ky":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "kir", "iso-9");
                case "mk":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "mkd", "bgn-pcgn");
                case "mn":
                    return await ushuaiaTransliterator.Transliterate(normalisedText, "mongolian_mns_transliterate");
                case "os":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "oss", "iso-9");
                case "ru":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "rus", "bgn-pcgn");
                case "sr":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "srp", "national");
                case "udm":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "udm", "bgn-pcgn");
                case "uk":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "ukr", "bgn-pcgn");
                case "zh":
                case "zh-hans":
                    return pinyinTransliterator.Transliterate(normalisedText);
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
