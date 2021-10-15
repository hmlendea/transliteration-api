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
                case "ru":
                    return await transliterationDotCom.Transliterate(text, "rus", "iso-9");
                default:
                    throw new ArgumentException("The specified language is not supported");
            }
        }
    }
}
