using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.Service
{
    public class TransliterationService : ITransliterationService
    {
        IPodolakTransliterator podolakTransliterator;
        ITransliterateDotComTransliterator transliterateDotComTransliterator;
        ITranslitterationDotComTransliterator translitterationDotComTransliterator;

        public TransliterationService(
            IPodolakTransliterator podolakTransliterator,
            ITransliterateDotComTransliterator transliterateDotComTransliterator,
            ITranslitterationDotComTransliterator translitterationDotComTransliterator)
        {
            this.podolakTransliterator = podolakTransliterator;
            this.transliterateDotComTransliterator = transliterateDotComTransliterator;
            this.translitterationDotComTransliterator = translitterationDotComTransliterator;
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
                    return await transliterateDotComTransliterator.Transliterate(normalisedText, "el");
                case "hy":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "xcl", "iso-9985");
                case "hyw":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "hye", "ala-lc");
                //case "ja":
                case "ka":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "kat", "national");
                case "kk":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "kaz", "national");
                case "ky":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "kir", "iso-9");
                case "mk":
                    return await translitterationDotComTransliterator.Transliterate(normalisedText, "mkd", "bgn-pcgn");
                //case "mn":
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
