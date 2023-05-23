using System.Collections.Generic;
using System.Text.RegularExpressions;
using NuciExtensions;

namespace TransliterationAPI.Service.Transliterators
{
    public class HebrewTransliterator : IHebrewTransliterator
    {
        Dictionary<char, string> transliterationMap;

        public HebrewTransliterator()
        {
            transliterationMap = new Dictionary<char, string>
            {
                { 'א', "" },
                { 'ב', "b" },
                { 'ג', "g" },
                { 'ד', "d" },
                { 'ה', "h" },
                { 'ו', "v" },
                { 'ז', "z" },
                { 'ח', "ch" },
                { 'ט', "t" },
                { 'י', "y" },
                { 'כ', "k" },
                { 'ך', "k" },
                { 'ל', "l" },
                { 'מ', "m" },
                { 'ם', "m" },
                { 'נ', "n" },
                { 'ן', "n" },
                { 'ס', "s" },
                { 'ע', "" },
                { 'פ', "p" },
                { 'ף', "p" },
                { 'צ', "ts" },
                { 'ץ', "ts" },
                { 'ק', "q" },
                { 'ר', "r" },
                { 'ש', "sh" },
                { 'ת', "t" },

                // Niqqud
                { 'ְ', "" }, // Sheva
                { 'ֱ', "e" }, // Hataf Segol
                { 'ֲ', "a" }, // Hataf Patah
                { 'ֳ', "o" }, // Hataf Qamats
                { 'ִ', "i" }, // Hiriq
                { 'ֵ', "e" }, // Tsere
                { 'ֶ', "e" }, // Segol
                { 'ַ', "a" }, // Patah
                { 'ָ', "a" }, // Qamats
                { 'ֹ', "o" }, // Holam
                { 'ֻ', "u" }, // Qubuts
                { 'ּ', "" }, // Dagesh, Mapiq, Shuruq: used to modify the pronunciation of the consonant, included for completeness but has no direct transliteration
            };
        }

        public string Transliterate(string text)
        {
            string transliteratedText = string.Empty;

            foreach (char character in text)
            {
                if (transliterationMap.ContainsKey(character))
                {
                    transliteratedText += transliterationMap[character];
                }
                else
                {
                    transliteratedText += character;
                }
            }

            transliteratedText = ApplyFixes(transliteratedText);

            return transliteratedText;
        }

        string ApplyFixes(string text)
        {
            string fixedText = text.ToTitleCase();

            fixedText = Regex.Replace(fixedText, "([\\ \\-])byb", "$1Aviv");
            fixedText = Regex.Replace(fixedText, "([hḥ])v ", "$1ō ");
            fixedText = Regex.Replace(fixedText, "([hḥ])v$", "$1ō");
            fixedText = Regex.Replace(fixedText, "am ", "ām ");
            fixedText = Regex.Replace(fixedText, "am$", "ām");
            fixedText = Regex.Replace(fixedText, "ame", "amme");
            fixedText = Regex.Replace(fixedText, "Ash", "ʾAsh");
            fixedText = Regex.Replace(fixedText, "ayim", "áyim");
            fixedText = Regex.Replace(fixedText, "Bab", "Bāv");
            fixedText = Regex.Replace(fixedText, "Ber", "Bəʾēr");
            fixedText = Regex.Replace(fixedText, "bt", "bat");
            fixedText = Regex.Replace(fixedText, "byn", "vin");
            fixedText = Regex.Replace(fixedText, "dvn", "dun");
            fixedText = Regex.Replace(fixedText, "dvo", "dō");
            fixedText = Regex.Replace(fixedText, "eׁba", "evaʿ");
            fixedText = Regex.Replace(fixedText, "eׂq", "eq");
            fixedText = Regex.Replace(fixedText, "Ey", "ʿĒ");
            fixedText = Regex.Replace(fixedText, "lvo", "lō");
            fixedText = Regex.Replace(fixedText, "ly", "láy");
            fixedText = Regex.Replace(fixedText, "Mq", "Maq");
            fixedText = Regex.Replace(fixedText, "Mv", "ʿAmmv");
            fixedText = Regex.Replace(fixedText, "mvn", "mon");
            fixedText = Regex.Replace(fixedText, "Nt", "Net");
            fixedText = Regex.Replace(fixedText, "ny", "nei");
            fixedText = Regex.Replace(fixedText, "ōd ", "ōḏ ");
            fixedText = Regex.Replace(fixedText, "ōd$", "ōḏ");
            fixedText = Regex.Replace(fixedText, "ql", "qəl");
            fixedText = Regex.Replace(fixedText, "Rb", "Rab");
            fixedText = Regex.Replace(fixedText, "rvs", "rus");
            fixedText = Regex.Replace(fixedText, "Sh", "Š");
            fixedText = Regex.Replace(fixedText, "shׁ", "š");
            fixedText = Regex.Replace(fixedText, "sheq", "śeq");
            fixedText = Regex.Replace(fixedText, "shl", "shal");
            fixedText = Regex.Replace(fixedText, "sy", "zi");
            fixedText = Regex.Replace(fixedText, "Tl", "Tel");
            fixedText = Regex.Replace(fixedText, "ts", "ṣ");
            fixedText = Regex.Replace(fixedText, "ych", "īḥ");
            fixedText = Regex.Replace(fixedText, "yh", "yah");
            fixedText = Regex.Replace(fixedText, "ym", "yim");
            fixedText = Regex.Replace(fixedText, "Yr([^iī])", "Yer$1");
            fixedText = Regex.Replace(fixedText, "Yr([iī])", "Yər$1");

            return fixedText;
        }
    }
}