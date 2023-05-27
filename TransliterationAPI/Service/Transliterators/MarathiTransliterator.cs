using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using NuciExtensions;

namespace TransliterationAPI.Service.Transliterators
{
    public class MarathiTransliterator : ITransliterator
    {
        Dictionary<string, string> transliterationTable;

        public MarathiTransliterator()
        {
            transliterationTable = new Dictionary<string, string>
            {
                // Additional characters
                {"क़", "q"},
                {"ख़", "x"},
                {"ग़", "ġ"},
                {"ज़", "z"},
                {"ड़", "ṛ"},
                {"ढ़", "ṛh"},
                {"फ़", "f"},
                {"य़", "y"},
                {"क्ष", "kṣ"},
                {"ज्ञ", "gy"},
                {"श्र", "śr"},

                // Main
                { "।", "." },
                { "ः", "ah" },
                { "़", "e" },
                { "ँ", "n" },
                { "ं", "n" },
                { "०", "0" },
                { "१", "1" },
                { "२", "2" },
                { "३", "3" },
                { "४", "4" },
                { "५", "5" },
                { "६", "6" },
                { "७", "7" },
                { "८", "8" },
                { "९", "9" },
                { "ॐ", "om" },
                { "अ", "a" },
                { "आ", "ā" }, // aa
                { "इ", "i" },
                { "ई", "ī" },
                { "उ", "u" },
                { "ऊ", "ū" },
                { "ऋ", "ṛ" }, // ri
                { "ॠ", "ṝ" },
                { "ऌ", "ḷ" },
                { "ॡ", "l" },
                { "ऍ", "e" },
                { "ए", "e" },
                { "ऐ", "ai" },
                { "ऑ", "au" },
                { "ओ", "o" },
                { "औ", "au" },
                { "क", "k" },
                { "क़", "k" },
                { "ख", "kh" },
                { "ख़", "kh" },
                { "ग़", "g" },
                { "ग", "ga" },
                { "ॻ", "g" },
                { "घ", "gh" },
                { "ङ", "ṅ" }, //ng
                { "च", "k" },
                { "छ", "ch" },
                { "ज", "j" },
                { "ज़", "z" },
                { "ॼ", "j" },
                { "झ", "jh" },
                { "ञ", "ñ" }, // ny
                { "ट", "ṭ" },
                { "ठ", "ṭh" },
                { "ड", "ḍ" },
                { "ड़", "d" },
                { "ॾ", "d" },
                { "ढ", "ḍh" },
                { "ढ़", "rh" },
                { "ण", "ṇ" },
                { "त", "t" },
                { "थ", "th" },
                { "द", "d" },
                { "ध", "dh" },
                { "न", "n" },
                { "ऩ", "n" },
                { "प", "p" },
                { "फ", "f" },
                { "फ़", "f" },
                { "ब", "b" },
                { "ॿ", "b" },
                { "भ", "bh" },
                { "म", "m" },
                { "य", "y" },
                { "य़", "ye" },
                { "र", "r" },
                { "ऱ", "r" },
                { "ल", "l" },
                { "ळ", "ḷ" },
                { "ऴ", "ll" },
                { "व", "v" },
                { "श", "ś" }, // sh
                { "ष", "ṣ" },
                { "स", "s" },
                { "ह", "h" },
                { "ऽ", "'" },
                { "ॽ", "" },
                { "ा", "aa" },
                { "ि", "i" },
                { "ी", "i" },
                { "ु", "u" },
                { "ू", "u" },
                { "ृ", "ri" },
                { "ॄ", "rr" },
                { "ॢ", "l" },
                { "ॣ", "l" },
                { "ॅ", "e" },
                { "ॆ", "e" },
                { "े", "e" },
                { "ै", "ai" },
                { "ॉ", "o" },
                { "ॊ", "o" },
                { "ो", "o" },
                { "ौ", "au" },
                { "्", "" },
            };
        }

        public string Transliterate(string text, string languageCode)
        {
            string transliteratedText = text;

            foreach (string marathiCharacter in transliterationTable.Keys)
            {
                transliteratedText = transliteratedText.Replace(marathiCharacter, transliterationTable[marathiCharacter]);
            }

            transliteratedText = ApplyFixes(transliteratedText);

            return transliteratedText;
        }

        string ApplyFixes(string text)
        {
            string fixedText = text;
            fixedText = Regex.Replace(fixedText, " kn", " chan");
            fixedText = Regex.Replace(fixedText, "^kn", "chan");

            fixedText = Regex.Replace(fixedText, "([kd])ol", "$1ōl");
            fixedText = Regex.Replace(fixedText, "([lḷ])g", "$1ag");
            fixedText = Regex.Replace(fixedText, "([pt])ur", "$1ūr");
            fixedText = Regex.Replace(fixedText, "aa[a]*", "ā");
            fixedText = Regex.Replace(fixedText, "ānj", "āṅj");
            fixedText = Regex.Replace(fixedText, "ḍh(ch|k)", "da$1");
            fixedText = Regex.Replace(fixedText, "dng", "dnag");
            fixedText = Regex.Replace(fixedText, "e.a", "a");
            fixedText = Regex.Replace(fixedText, "ga([il])", "g$1");
            fixedText = Regex.Replace(fixedText, "gao", "gō");
            fixedText = Regex.Replace(fixedText, "gap", "gāp");
            fixedText = Regex.Replace(fixedText, "h([ṇr])", "ha$1");
            fixedText = Regex.Replace(fixedText, "hn", "hān");
            fixedText = Regex.Replace(fixedText, "jḷ", "jaḷ");
            fixedText = Regex.Replace(fixedText, "ki", "chi");
            fixedText = Regex.Replace(fixedText, "lḍ", "ḷḍ");
            fixedText = Regex.Replace(fixedText, "ḷeg", "ḷēg");
            fixedText = Regex.Replace(fixedText, "md", "mad");
            fixedText = Regex.Replace(fixedText, "mir", "mīr");
            fixedText = Regex.Replace(fixedText, "nḍ", "ṇḍ");
            fixedText = Regex.Replace(fixedText, "ng", "ṅg");
            fixedText = Regex.Replace(fixedText, "p([ṇr])", "pa$1");
            fixedText = Regex.Replace(fixedText, "r([bt])", "ra$1");
            fixedText = Regex.Replace(fixedText, "r([p])", "rā$1");
            fixedText = Regex.Replace(fixedText, "r[nṅ]", "ran");
            fixedText = Regex.Replace(fixedText, "s([ṅr])", "sa$1");
            fixedText = Regex.Replace(fixedText, "śh", "śah");
            fixedText = Regex.Replace(fixedText, "t([lm])", "ta$1");
            fixedText = Regex.Replace(fixedText, "v([lt])", "va$1");
            fixedText = Regex.Replace(fixedText, "vel", "vēl");
            fixedText = Regex.Replace(fixedText, "yev", "yēv");
            fixedText = Regex.Replace(fixedText, "yv", "yav");

            fixedText = Regex.Replace(fixedText, "osm", "osam");
            fixedText = Regex.Replace(fixedText, "rāpūr", "rāpur");

            fixedText = Regex.Replace(fixedText, "([ḍlṇrt])i ", "$1ī ");
            fixedText = Regex.Replace(fixedText, "([ḍlṇrt])i$", "$1ī");
            fixedText = Regex.Replace(fixedText, "eḍ ", "ēd ");
            fixedText = Regex.Replace(fixedText, "eḍ$", "ēd");
            fixedText = Regex.Replace(fixedText, "ḷi ", "lī ");
            fixedText = Regex.Replace(fixedText, "ḷi$", "lī");
            fixedText = Regex.Replace(fixedText, "ṇe ", "ṇē ");
            fixedText = Regex.Replace(fixedText, "ṇe$", "ṇē");
            fixedText = Regex.Replace(fixedText, "sī ", "sai ");
            fixedText = Regex.Replace(fixedText, "sī$", "sai");

            fixedText = Regex.Replace(fixedText, "valī", "vlī");

            return fixedText.ToTitleCase();
        }
    }
}
