using System;
using System.Threading.Tasks;

using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.Service
{
    public class TransliterationService : ITransliterationService
    {
        ITransliterationDotCom transliterationDotCom;

        public TransliterationService(ITransliterationDotCom transliterationDotCom)
        {
            this.transliterationDotCom = transliterationDotCom;
        }

        public async Task<string> Transliterate(string text, string language)
        {
            switch (language)
            {
                case "ab":
                    return await transliterationDotCom.Transliterate(text, "abk", "iso-9");
                case "ady":
                    return await transliterationDotCom.Transliterate(text, "ady", "iso-9");
                case "ba":
                    return await transliterationDotCom.Transliterate(text, "bak", "iso-9");
                case "be":
                    return await transliterationDotCom.Transliterate(text, "bel", "national");
                case "bg":
                    return await transliterationDotCom.Transliterate(text, "bul", "streamlined");
                //case "cu":
                case "cv":
                    return await transliterationDotCom.Transliterate(text, "chv", "ala-lc");
                //case "el":
                case "hy":
                    return await transliterationDotCom.Transliterate(text, "hye", "ala-lc");
                //case "ja":
                case "ka":
                    return await transliterationDotCom.Transliterate(text, "kat", "national");
                case "kk":
                    return await transliterationDotCom.Transliterate(text, "kaz", "national");
                case "ky":
                    return await transliterationDotCom.Transliterate(text, "kir", "iso-9");
                case "mk":
                    return await transliterationDotCom.Transliterate(text, "mkd", "bgn-pcgn");
                //case "mn":
                case "os":
                    return await transliterationDotCom.Transliterate(text, "oss", "iso-9");
                case "ru":
                    return await transliterationDotCom.Transliterate(text, "rus", "bgn-pcgn");
                case "sr":
                    return await transliterationDotCom.Transliterate(text, "srp", "national");
                case "udm":
                    return await transliterationDotCom.Transliterate(text, "udm", "bgn-pcgn");
                case "uk":
                    return await transliterationDotCom.Transliterate(text, "ukr", "bgn-pcgn");
                default:
                    throw new ArgumentException("The specified language is not supported");
            }
        }
    }
}
