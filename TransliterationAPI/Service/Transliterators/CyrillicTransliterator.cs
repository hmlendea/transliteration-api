using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TransliterationAPI.Service.Transliterators
{
    public class CyrillicTransliterator : ICyrillicTransliterator
    {
        Dictionary<string, string> bgnPcgnTransliterationTable;

        Dictionary<string, string> bulgarianTransliterationTable;
        Dictionary<string, string> russianTransliterationTable;

        public CyrillicTransliterator()
        {
            bgnPcgnTransliterationTable = new Dictionary<string, string>
            {
                { "А", "A" },
                { "Б", "B" },
                { "В", "V" },
                { "Г", "G" },
                { "Д", "D" },
                { "Е", "E" },
                { "Ё", "Yo" },
                { "Ж", "Zh" },
                { "З", "Z" },
                { "И", "I" },
                { "Й", "Y" },
                { "К", "K" },
                { "Л", "L" },
                { "М", "M" },
                { "Н", "N" },
                { "О", "O" },
                { "П", "P" },
                { "Р", "R" },
                { "С", "S" },
                { "Т", "T" },
                { "У", "U" },
                { "Ф", "F" },
                { "Х", "Kh" }, // Kk
                { "Ц", "Ts" },
                { "Ч", "Ch" },
                { "Ш", "Sh" },
                { "Щ", "Shch" },
                { "Ъ", "\""},
                { "Ы", "Y" },
                { "Ь", "'" },
                { "Э", "E" },
                { "Ю", "Yu" },
                { "Я", "Ya" },
                { "а", "a" },
                { "б", "b" },
                { "в", "v" },
                { "г", "g" },
                { "д", "d" },
                { "е", "e" },
                { "ё", "yo" },
                { "ж", "zh" },
                { "з", "z" },
                { "и", "i" },
                { "й", "y" },
                { "к", "k" },
                { "л", "l" },
                { "м", "m" },
                { "н", "n" },
                { "о", "o" },
                { "п", "p" },
                { "р", "r" },
                { "с", "s" },
                { "т", "t" },
                { "у", "u" },
                { "ф", "f" },
                { "х", "kh" },
                { "ц", "ts" },
                { "ч", "ch" },
                { "ш", "sh" },
                { "щ", "shch" },
                { "ъ", "\""},
                { "ы", "y" },
                { "ь", "" }, // '
                { "э", "e" },
                { "ю", "yu" },
                { "я", "ya" }
            };

            bulgarianTransliterationTable = new Dictionary<string, string>
            {
                { @"ия\b", "ia" },

                { "Ъ", "Ă" }, // A officially, since the 2002 proposal did not pass
                { "Х", "H" },
                { "Щ", "Sht" },
                { "ъ", "ă" }, // a officially, since the 2002 proposal did not pass
                { "х", "h" },
                { "щ", "sht" },
            };

            russianTransliterationTable = new Dictionary<string, string>();

            foreach (var characterTransliteration in bgnPcgnTransliterationTable)
            {
                if (!bulgarianTransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    bulgarianTransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
                }

                if (!russianTransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    russianTransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
                }
            }
        }

        public string Transliterate(string text, string languageCode)
        {
            IDictionary<string, string> transliterationTable;

            if (languageCode.Equals("bg"))
            {
                transliterationTable = bulgarianTransliterationTable;
            }
            else if (languageCode.Equals("ru"))
            {
                transliterationTable = russianTransliterationTable;
            }
            else
            {
                throw new ArgumentException($"The '{languageCode}' language is not supported in the {nameof(CyrillicTransliterator)}");
            }

            string transliteratedText = text;

            foreach (string character in transliterationTable.Keys)
            {
                transliteratedText = Regex.Replace(transliteratedText, character, transliterationTable[character]);
            }

            //if (languageCode.Equals("bg"))
            //{
            //    transliteratedText = ApplyBulgarianFixes(transliteratedText);
            //}
            //else
            if (languageCode.Equals("ru"))
            {
                transliteratedText = ApplyRussianFixes(transliteratedText);
            }

            return transliteratedText;
        }

        string ApplyRussianFixes(string text)
        {
            string fixedText = text;

            fixedText = Regex.Replace(fixedText, "Eka", "Yeka");
            fixedText = Regex.Replace(fixedText, "oe", "oye");

            fixedText = Regex.Replace(fixedText, @"([\ \-])syur([\ \-])(.*)\b", "$1na$1$3e");

            return fixedText;
        }
    }
}
