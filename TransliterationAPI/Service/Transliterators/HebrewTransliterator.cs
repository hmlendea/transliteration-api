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

            fixedText = Regex.Replace(fixedText, "Ash", "ʾAsh");
            fixedText = Regex.Replace(fixedText, "Bab", "Bāv");
            fixedText = Regex.Replace(fixedText, "Ber", "Bəʾēr");
            fixedText = Regex.Replace(fixedText, "Ch", "Ḥ");
            fixedText = Regex.Replace(fixedText, "Ey", "ʿĒ");
            fixedText = Regex.Replace(fixedText, "Ḥe([^b])", "Ḥē$1");
            fixedText = Regex.Replace(fixedText, "Hvo", "Ho");
            fixedText = Regex.Replace(fixedText, "Ḥvo", "Ho");
            fixedText = Regex.Replace(fixedText, "Mq", "Maq");
            fixedText = Regex.Replace(fixedText, "Mv", "ʿAmmv");
            fixedText = Regex.Replace(fixedText, "Na(ṣ|ts)", "Nā$1");
            fixedText = Regex.Replace(fixedText, "Nt", "Net");
            fixedText = Regex.Replace(fixedText, "Pl", "Pal");
            fixedText = Regex.Replace(fixedText, "R(ch|ḥ)", "Re$1");
            fixedText = Regex.Replace(fixedText, "Rb", "Rab");
            fixedText = Regex.Replace(fixedText, "Sh", "Š");
            fixedText = Regex.Replace(fixedText, "Tl", "Tel");
            fixedText = Regex.Replace(fixedText, "Yh", "Yəh");
            fixedText = Regex.Replace(fixedText, "Yi", "Yī");
            fixedText = Regex.Replace(fixedText, "Yr([^iīy])", "Yer$1");
            fixedText = Regex.Replace(fixedText, "Yr([iīy])", "Yər$1");

            fixedText = Regex.Replace(fixedText, "([hḥ])v ", "$1ō ");
            fixedText = Regex.Replace(fixedText, "([hḥ])v$", "$1ō");
            fixedText = Regex.Replace(fixedText, "([Pp])r", "$1ǝr");
            fixedText = Regex.Replace(fixedText, "ā(ṣ|ts)r", "ā$1ər");
            fixedText = Regex.Replace(fixedText, "ae", "āʾē");
            fixedText = Regex.Replace(fixedText, "am ", "ām ");
            fixedText = Regex.Replace(fixedText, "am$", "ām");
            fixedText = Regex.Replace(fixedText, "ame", "amme");
            fixedText = Regex.Replace(fixedText, "at ", "aṯ ");
            fixedText = Regex.Replace(fixedText, "at$", "aṯ");
            fixedText = Regex.Replace(fixedText, "ayim", "áyim");
            fixedText = Regex.Replace(fixedText, "bt", "bat");
            fixedText = Regex.Replace(fixedText, "bvo", "vō");
            fixedText = Regex.Replace(fixedText, "byn", "vin");
            fixedText = Regex.Replace(fixedText, "ch", "ḥ");
            fixedText = Regex.Replace(fixedText, "dh ", "da ");
            fixedText = Regex.Replace(fixedText, "dh$", "da");
            fixedText = Regex.Replace(fixedText, "dvn", "dun");
            fixedText = Regex.Replace(fixedText, "dvo", "dō");
            fixedText = Regex.Replace(fixedText, "eׁba", "evaʿ");
            fixedText = Regex.Replace(fixedText, "ebr", "evr");
            fixedText = Regex.Replace(fixedText, "eׂq", "eq");
            fixedText = Regex.Replace(fixedText, "hvd", "hūd");
            fixedText = Regex.Replace(fixedText, "ḥvo", "ḥō");
            fixedText = Regex.Replace(fixedText, "iba", "iva");
            fixedText = Regex.Replace(fixedText, "lah ", "lā ");
            fixedText = Regex.Replace(fixedText, "lah$", "lā");
            fixedText = Regex.Replace(fixedText, "ls", "les");
            fixedText = Regex.Replace(fixedText, "lvo", "lō");
            fixedText = Regex.Replace(fixedText, "ly", "láy");
            fixedText = Regex.Replace(fixedText, "mvn", "mon");
            fixedText = Regex.Replace(fixedText, "nh ", "na ");
            fixedText = Regex.Replace(fixedText, "nh$", "na");
            fixedText = Regex.Replace(fixedText, "ny", "nei");
            fixedText = Regex.Replace(fixedText, "ōd ", "ōḏ ");
            fixedText = Regex.Replace(fixedText, "ōd$", "ōḏ");
            fixedText = Regex.Replace(fixedText, "oׁm", "ōm");
            fixedText = Regex.Replace(fixedText, "pah", "fā");
            fixedText = Regex.Replace(fixedText, "ql", "qəl");
            fixedText = Regex.Replace(fixedText, "rts", "rz");
            fixedText = Regex.Replace(fixedText, "rvo", "rō");
            fixedText = Regex.Replace(fixedText, "rvs", "rus");
            fixedText = Regex.Replace(fixedText, "shׁ", "š");
            fixedText = Regex.Replace(fixedText, "sheq", "śeq");
            fixedText = Regex.Replace(fixedText, "shl", "shal");
            fixedText = Regex.Replace(fixedText, "shׂr", "sr");
            fixedText = Regex.Replace(fixedText, "sht", "st");
            fixedText = Regex.Replace(fixedText, "sy", "zi");
            fixedText = Regex.Replace(fixedText, "ts", "ṣ");
            fixedText = Regex.Replace(fixedText, "ty", "tī");
            fixedText = Regex.Replace(fixedText, "y(ch|ḥ)", "īḥ");
            fixedText = Regex.Replace(fixedText, "yah ", "ya ");
            fixedText = Regex.Replace(fixedText, "yah$", "ya");
            fixedText = Regex.Replace(fixedText, "yh", "yah");
            fixedText = Regex.Replace(fixedText, "ym", "yim");

            fixedText = Regex.Replace(fixedText, "eiah", "ya");
            fixedText = Regex.Replace(fixedText, "ǝraṯ", "ǝrāṯ");
            fixedText = Regex.Replace(fixedText, "yiyi", "yi");

            return fixedText;
        }
    }
}
