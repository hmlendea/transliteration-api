using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class CyrillicTransliterator : ITransliterator
    {
        Dictionary<string, string> bgnPcgnTransliterationTable;

        Dictionary<string, string> bulgarianTransliterationTable;
        Dictionary<string, string> russianTransliterationTable;
        Dictionary<string, string> ukrainianTransliterationTable;

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

            ukrainianTransliterationTable = new Dictionary<string, string>
            {
                { @"ія\b", "ia" },
                { @"\b([Сс])и", "$1i" },

                { "Г", "H" },
                { "Є", "Ye" },
                { "И", "Y" },
                { "І", "I" },
                { "Ї", "I" },
                { "Й", "I" },
                { "О́", "Ó" },
                { "Я", "Ya" },

                { "г", "h" },
                { "є", "ie" },
                { "и", "y" },
                { "і", "i" },
                { "ї", "i" },
                { "й", "i" },
                { "о́", "ó" },
                { "я", "ia" },

                { "'", "" },
            };

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

                if (!ukrainianTransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    ukrainianTransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
                }
            }
        }

        public string Transliterate(string text, Language language)
        {
            IDictionary<string, string> transliterationTable;

            if (language.Equals(Language.Bulgarian))
            {
                transliterationTable = bulgarianTransliterationTable;
            }
            else if (language.Equals(Language.Russian))
            {
                transliterationTable = russianTransliterationTable;
            }
            else if (language.Equals(Language.Ukrainian))
            {
                transliterationTable = ukrainianTransliterationTable;
            }
            else
            {
                throw new ArgumentException($"The {language.Name} language is not supported by the {nameof(CyrillicTransliterator)}");
            }

            string transliteratedText = text;

            foreach (string character in transliterationTable.Keys)
            {
                transliteratedText = Regex.Replace(transliteratedText, character, transliterationTable[character]);
            }

            if (language.Equals(Language.Russian))
            {
                transliteratedText = ApplyRussianFixes(transliteratedText);
            }

            return transliteratedText;
        }

        string ApplyRussianFixes(string text)
        {
            string fixedText = text;

            fixedText = Regex.Replace(fixedText, "Eka", "Yeka");
            fixedText = Regex.Replace(fixedText, "([eo])e", "$1ye");

            fixedText = Regex.Replace(fixedText, @"([\ \-])syur([\ \-])(.*)\b", "$1na$1$3e");

            return fixedText;
        }
    }
}
