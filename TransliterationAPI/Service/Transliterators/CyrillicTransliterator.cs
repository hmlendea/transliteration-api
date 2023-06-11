using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class CyrillicTransliterator : ITransliterator
    {
        Dictionary<string, string> bgnPcgnTransliterationTable;

        Dictionary<string, string> belarussianTransliterationTable;
        Dictionary<string, string> bulgarianTransliterationTable;
        Dictionary<string, string> kazakhTransliterationTable;
        Dictionary<string, string> russianTransliterationTable;
        Dictionary<string, string> macedonianTransliterationTable;
        Dictionary<string, string> serbianTransliterationTable;
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

            belarussianTransliterationTable = new Dictionary<string, string>
            {
                // Uppeercase exceptions
                { "Ль", "Ĺ" },
                { "Нь", "Ń" },
                { @"\bЕ", "Je" },
                { @"\bЁ", "Jo" },
                { @"\bЯ", "Ja" },
                { @"\bЮ", "Ju" },

                // Lowercase exceptions
                { "іё", "іjo" },
                { "ль", "ĺ" },
                { "ля", "лia" },
                { "нь", "ń" },
                { @"е\b", "je" },
                { @"ё\b", "jo" },
                { @"ю\b", "ju" },
                { @"я\b", "ja" },

                // Uppeercase characters
                { "Г", "H" },
                { "Д", "D" },
                { "Е", "Ie" }, // Also Je
                { "Ё", "Jo" }, // Also Io
                { "Ж", "Ž" },
                { "І", "I" },
                { "Й", "J" },
                { "Ў", "Ŭ" },
                { "Х", "Ch" },
                { "Ц", "C" },
                { "Ч", "Č" },
                { "Ш", "Š" },
                { "Ю", "Iu" }, // Also Ju
                { "Я", "Ia" }, // Also Ja

                // Lowercase characters
                { "г", "h" },
                { "д", "d" },
                { "е", "ie" },
                { "ё", "io" },
                { "ж", "ž" },
                { "і", "i" },
                { "й", "j" },
                { "ў", "ŭ" },
                { "х", "ch" },
                { "ц", "c" },
                { "ч", "č" },
                { "ш", "š" },
                { "ю", "iu" },
                { "я", "ia" },
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

            kazakhTransliterationTable = new Dictionary<string, string>
            {
                { "Ә", "Ä" },
                { "Ғ", "Ğ" },
                { "Ё", "İo" },
                { "Ж", "J" },
                { "И", "İ" },
                { "І", "I" },
                { "Й", "İ" },
                { "Қ", "Q" },
                { "Ң", "Ñ" },
                { "Ө", "Ö" },
                { "У", "U" },
                { "Ү", "Ü" },
                { "Ұ", "Ū" },
                { "Х", "H" },
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
                { "ж", "j" },
                { "и", "i" },
                { "і", "ı" },
                { "й", "i" },
                { "қ", "q" },
                { "ң", "ñ" },
                { "ө", "ö" },
                { "у", "u" },
                { "ү", "ü" },
                { "ұ", "ū" },
                { "х", "h" },
                { "ц", "ts" },
                { "ч", "tş" },
                { "ш", "ş" },
                { "щ", "ştş" },
                { "э", "e" },
                { "ю", "iu" },
                { "я", "ia" },
            };

            russianTransliterationTable = new Dictionary<string, string>();

            macedonianTransliterationTable = new Dictionary<string, string>
            {
                // Uppercase letters
                { "Ѓ", "Ǵ" },
                { "Ѐ", "È" }, // technically not a separate letter, but used to differentiate in words with homographs
                { "Ж", "Ž" },
                { "Ѝ", "Ì" },
                { "Ј", "J" },
                { "Х", "H" },
                { "Ц", "C" },
                { "Ч", "Č" },
                { "Ш", "Š" },

                // Lowercase letters
                { "ѓ", "ǵ" },
                { "ѐ", "è" }, // technically not a separate letter, but used to differentiate in words with homographs
                { "ж", "ž" },
                { "ѝ", "ì" },
                { "ј", "j" },
                { "х", "h" },
                { "ц", "c" },
                { "ч", "č" },
                { "ш", "š" },
            };

            serbianTransliterationTable = new Dictionary<string, string>
            {
                // Uppercase letters
                { "Ђ", "Đ" },
                { "Ж", "Ž" },
                { "Ј", "J" },
                { "Љ", "Lj" },
                { "Ћ", "Ć" },
                { "Ц", "C" },
                { "Ч", "Č" },
                { "Џ", "Dž" },
                { "Ш", "Š" },

                // Lowercase letters
                { "ђ", "đ" },
                { "ж", "ž" },
                { "ј", "j" },
                { "љ", "lj" },
                { "њ", "nj" },
                { "ћ", "ć" },
                { "ц", "c" },
                { "ч", "č" },
                { "џ", "dž" },
                { "ш", "š" },
            };

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
                if (!belarussianTransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    belarussianTransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
                }

                if (!bulgarianTransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    bulgarianTransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
                }

                if (!kazakhTransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    kazakhTransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
                }

                if (!macedonianTransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    macedonianTransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
                }

                if (!russianTransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    russianTransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
                }

                if (!serbianTransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    serbianTransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
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

            if (language.Equals(Language.Belarussian))
            {
                transliterationTable = belarussianTransliterationTable;
            }
            else if (language.Equals(Language.Bulgarian))
            {
                transliterationTable = bulgarianTransliterationTable;
            }
            else if (language.Equals(Language.Kazakh))
            {
                transliterationTable = kazakhTransliterationTable;
            }
            else if (language.Equals(Language.MacedonianSlavic))
            {
                transliterationTable = macedonianTransliterationTable;
            }
            else if (language.Equals(Language.Russian))
            {
                transliterationTable = russianTransliterationTable;
            }
            else if (language.Equals(Language.Serbian) ||
                     language.Equals(Language.SerbianCyrillic) ||
                     language.Equals(Language.SerboCroatian))
            {
                transliterationTable = serbianTransliterationTable;
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

            if (language.Equals(Language.Belarussian))
            {
                transliteratedText = ApplyBelarussianFixes(transliteratedText);
            }
            else if (language.Equals(Language.Russian))
            {
                transliteratedText = ApplyRussianFixes(transliteratedText);
            }

            return transliteratedText;
        }

        string ApplyBelarussianFixes(string text)
        {
            string fixedText = text;

            fixedText = Regex.Replace(fixedText, "'i", "ji");

            return fixedText;
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
