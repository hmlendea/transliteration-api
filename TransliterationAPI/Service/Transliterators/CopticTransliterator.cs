using System.Collections.Generic;
using System.Text.RegularExpressions;

using NuciExtensions;

namespace TransliterationAPI.Service.Transliterators
{
    public class CopticTransliterator : ICopticTransliterator
    {
        Dictionary<string, string> transliterationTable;

        public CopticTransliterator()
        {
            transliterationTable = new Dictionary<string, string>
            {
                { "Ⲁ", "A" },
                { "ⲁ", "a" },
                { "Ⲃ", "B" },
                { "ⲃ", "b" },
                { "Ⲅ", "G" },
                { "ⲅ", "g" },
                { "Ⲇ", "D" },
                { "ⲇ", "d" },
                { "Ⲉ", "E" },
                { "ⲉ", "e" },
                { "Ϣ", "Š" }, // Sh
                { "ϣ", "š" }, // sh
                { "Ⲋ", "Z" },
                { "ⲋ", "z" },
                { "Ⲍ", "Y" },
                { "ⲍ", "y" },
                { "Ⲏ", "Ē" }, // E
                { "ⲏ", "ē" }, // e
                { "Ⲑ", "Th" },
                { "ⲑ", "th" },
                { "Ⲓ", "I" },
                { "ⲓ", "i" },
                { "Ⲕ", "K" },
                { "ⲕ", "k" },
                { "Ⲗ", "L" },
                { "ⲗ", "l" },
                { "Ⲙ", "M" },
                { "ⲙ", "m" },
                { "Ⲛ̀", "Ǹ" }, // En
                { "Ⲛ", "N" },
                { "ⲛ̀", "ǹ" }, // en
                { "ⲛ", "n" },
                { "Ⲝ", "X" },
                { "ⲝ", "x" },
                { "Ⲟ", "O" },
                { "ⲟ", "o" },
                { "Ⲡ", "P" },
                { "ⲡ", "p" },
                { "Ϥ", "F" },
                { "ϥ", "f" },
                { "Ⲣ", "R" },
                { "ⲣ̅", "ǝr" },
                { "ⲣ", "r" },
                { "Ⲥ", "S" },
                { "ⲥ", "s" },
                { "Ⲧ", "T" },
                { "ⲧ", "t" },
                { "Ⲩ", "U" },
                { "ⲩ", "u" },
                { "Ⲫ", "Ph" },
                { "ⲫ", "ph" },
                { "Ⲭ", "Kh" },
                { "ⲭ", "kh" },
                { "Ⲯ", "Ps" },
                { "ⲯ", "ps" },
                { "Ⲱ", "Ō" }, // O
                { "ⲱ", "ō" }, // o
                { "Ⳁ", "Sh" },
                { "ⳁ", "sh" },
                { "Ⳃ", "F" },
                { "ⳃ", "f" },
                { "Ⳅ", "Kh" },
                { "ⳅ", "kh" },
                { "Ⳇ", "H" },
                { "ⳇ", "h" },
                { "Ⳉ", "j" },
                { "ⳉ", "j" },
                { "Ϧ", "Ch" },
                { "ϧ", "ch" },
                { "Ϩ", "H" },
                { "ϩ", "h" },
                { "Ϫ", "J" },
                { "ϫ", "j" },
                { "Ϭ", "G" },
                { "ϭ", "g" },
                { "Ϯ", "Ti" }, // T
                { "ϯ", "tī" }, // t
            };
        }

        public string Transliterate(string text)
            => Transliterate(text, null);

        public string Transliterate(string text, string variant)
        {
            string transliteratedText = text;

            foreach (string character in transliterationTable.Keys)
            {
                transliteratedText = Regex.Replace(transliteratedText, character, transliterationTable[character]);
            }

            transliteratedText = ApplyFixes(transliteratedText);

            return transliteratedText;
        }

        string ApplyFixes(string text)
        {
            string fixedText = text;

            return fixedText.ToTitleCase();
        }
    }
}
