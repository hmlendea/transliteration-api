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
        Dictionary<string, string> kazakh2018TransliterationTable;
        Dictionary<string, string> kazakh2021TransliterationTable;
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

            kazakh2018TransliterationTable = new Dictionary<string, string>
            {
                { "Ә", "Á" },
                { "Ғ", "Ǵ" },
                { "Ё", "Io" },
                { "Ж", "J" },
                { "И", "I" },
                { "І", "I" },
                { "Й", "I" },
                { "Қ", "Q" },
                { "Ң", "Ń" },
                { "Ө", "Ó" },
                { "У", "Ý" },
                { "Ү", "Ú" },
                { "Ұ", "U" },
                { "Х", "H" },
                { "Ц", "S" },
                { "Ч", "Ch" },
                { "Ш", "Sh" },
                { "Щ", "Shch" },
                { "Ы", "Y" },
                { "Э", "E" },
                { "Ю", "Iý" },
                { "Я", "Ia" },

                { "ә", "á" },
                { "ғ", "ǵ" },
                { "ё", "io" },
                { "ж", "j" },
                { "и", "ı" },
                { "і", "i" },
                { "й", "ı" },
                { "қ", "q" },
                { "ң", "ń" },
                { "ө", "ó" },
                { "у", "ý" },
                { "ү", "ú" },
                { "ұ", "u" },
                { "х", "h" },
                { "ц", "s" },
                { "ч", "ch" },
                { "ш", "sh" },
                { "щ", "shch" },
                { "ы", "y" },
                { "э", "e" },
                { "ю", "iý" },
                { "я", "ia" },
            };

            kazakh2021TransliterationTable = new Dictionary<string, string>
            {
                { "Ә", "Ä" },
                { "Ғ", "Ğ" },
                { "Ё", "İo" },
                { "И", "İ" },
                { "І", "I" },
                { "Й", "İ" },
                { "Ң", "Ñ" },
                { "Ө", "Ö" },
                { "У", "U" },
                { "Ү", "Ü" },
                { "Ұ", "Ū" },
                { "Ц", "ts" },
                { "Ч", "Tş" },
                { "Ш", "Ş" },
                { "Щ", "Ştş" },
                { "Э", "E" },
                { "Ю", "İu" },
                { "Я", "İa" },

                { "ә", "ä" },
                { "ғ", "ğ" },
                { "ё", "io" },
                { "и", "i" },
                { "і", "ı" },
                { "й", "i" },
                { "ң", "ñ" },
                { "ө", "ö" },
                { "у", "u" },
                { "ү", "ü" },
                { "ұ", "ū" },
                { "ц", "ts" },
                { "ч", "tş" },
                { "ш", "ş" },
                { "щ", "ştş" },
                { "э", "e" },
                { "ю", "iu" },
                { "я", "ia" },
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

            foreach (var characterTransliteration in kazakh2018TransliterationTable)
            {
                if (!kazakh2021TransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    kazakh2021TransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
                }
            }

            foreach (var characterTransliteration in bgnPcgnTransliterationTable)
            {
                if (!bulgarianTransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    bulgarianTransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
                }

                if (!kazakh2018TransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    kazakh2018TransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
                }

                if (!kazakh2021TransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    kazakh2021TransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
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
            else if (language.Equals(Language.Kazakh) ||
                     language.Equals(Language.Kazakh2021))
            {
                transliterationTable = kazakh2021TransliterationTable;
            }
            else if (language.Equals(Language.Kazakh2018))
            {
                transliterationTable = kazakh2018TransliterationTable;
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
            fixedText = Regex.Replace(fixedText, "oe", "oye");

            fixedText = Regex.Replace(fixedText, @"([\ \-])syur([\ \-])(.*)\b", "$1na$1$3e");

            return fixedText;
        }
    }
}
