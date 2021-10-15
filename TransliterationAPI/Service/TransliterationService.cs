using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.Service
{
    public class TransliterationService : ITransliterationService
    {
        IPodolakTransliterator podolakTransliterator;
        ITransliterationDotCom transliterationDotCom;

        public TransliterationService(
            IPodolakTransliterator podolakTransliterator,
            ITransliterationDotCom transliterationDotCom)
        {
            this.podolakTransliterator = podolakTransliterator;
            this.transliterationDotCom = transliterationDotCom;
        }

        public async Task<string> Transliterate(string text, string language)
        {
            string normalisedText = NormaliseText(text);
            switch (language)
            {
                case "ab":
                    return await transliterationDotCom.Transliterate(normalisedText, "abk", "iso-9");
                case "ady":
                    return await transliterationDotCom.Transliterate(normalisedText, "ady", "iso-9");
                case "ba":
                    return await transliterationDotCom.Transliterate(normalisedText, "bak", "iso-9");
                case "be":
                    return await transliterationDotCom.Transliterate(normalisedText, "bel", "national");
                case "bg":
                    return await transliterationDotCom.Transliterate(normalisedText, "bul", "streamlined");
                case "cu":
                    return await podolakTransliterator.Transliterate(normalisedText, language);
                case "cv":
                    return await transliterationDotCom.Transliterate(normalisedText, "chv", "ala-lc");
                //case "el":
                case "hy":
                    return await transliterationDotCom.Transliterate(normalisedText, "xcl", "iso-9985");
                case "hyw":
                    return await transliterationDotCom.Transliterate(normalisedText, "hye", "ala-lc");
                //case "ja":
                case "ka":
                    return await transliterationDotCom.Transliterate(normalisedText, "kat", "national");
                case "kk":
                    return await transliterationDotCom.Transliterate(normalisedText, "kaz", "national");
                case "ky":
                    return await transliterationDotCom.Transliterate(normalisedText, "kir", "iso-9");
                case "mk":
                    return await transliterationDotCom.Transliterate(normalisedText, "mkd", "bgn-pcgn");
                //case "mn":
                case "os":
                    return await transliterationDotCom.Transliterate(normalisedText, "oss", "iso-9");
                case "ru":
                    return await transliterationDotCom.Transliterate(normalisedText, "rus", "bgn-pcgn");
                case "sr":
                    return await transliterationDotCom.Transliterate(normalisedText, "srp", "national");
                case "udm":
                    return await transliterationDotCom.Transliterate(normalisedText, "udm", "bgn-pcgn");
                case "uk":
                    return await transliterationDotCom.Transliterate(normalisedText, "ukr", "bgn-pcgn");
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
