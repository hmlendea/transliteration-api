using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NuciExtensions;
using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class BerberTransliterator : ITransliterator
    {
        Dictionary<string, string> transliterationTable;
        public BerberTransliterator()
        {
            transliterationTable = new Dictionary<string, string>
            {
                { "ⴰ", "a" },
                { "ⴳ", "g" },
                { "ⴼ", "f" },
                { "ⵉ", "i" },
                { "ⵍ", "l" },
                { "ⵎ", "m" },
                { "ⵏ", "n" },
                { "ⵓ", "u" },
                { "ⵙ", "s" },
            };
        }

        public string Transliterate(string text, Language language)
        {
            if (!language.Equals(Language.Berber))
            {
                throw new ArgumentException($"The {language.Name} language is not supported by the {nameof(BerberTransliterator)}");
            }

            string transliteratedText = text;

            foreach (string character in transliterationTable.Keys)
            {
                transliteratedText = Regex.Replace(transliteratedText, character, transliterationTable[character]);
            }

            return transliteratedText.ToTitleCase();
        }
    }
}
