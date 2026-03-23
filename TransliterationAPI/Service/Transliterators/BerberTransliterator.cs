using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NuciExtensions;
using NuciLog.Core;
using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class BerberTransliterator : Transliterator, ITransliterator
    {
        readonly Dictionary<string, string> transliterationTable;

        public BerberTransliterator(ILogger logger) : base(logger)
        {
            transliterationTable = new()
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

        protected override string PerformTransliteration(string text, Language language)
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
