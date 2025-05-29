using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class CyrillicTransliterator : ITransliterator
    {
        readonly Dictionary<string, string> alaLcTransliterationTable;
        readonly Dictionary<string, string> bgnPcgnTransliterationTable;
        readonly Dictionary<string, string> iso9TransliterationTable;

        readonly Dictionary<string, string> abkhazianTransliterationTable;
        readonly Dictionary<string, string> belarussianTransliterationTable;
        readonly Dictionary<string, string> bulgarianTransliterationTable;
        readonly Dictionary<string, string> chuvashTransliterationTable;
        readonly Dictionary<string, string> kazakhTransliterationTable;
        readonly Dictionary<string, string> russianTransliterationTable;
        readonly Dictionary<string, string> macedonianTransliterationTable;
        readonly Dictionary<string, string> serbianTransliterationTable;
        readonly Dictionary<string, string> tatarTransliterationTable;
        readonly Dictionary<string, string> tajikTransliterationTable;
        readonly Dictionary<string, string> ukrainianTransliterationTable;

        public CyrillicTransliterator()
        {
            alaLcTransliterationTable = new()
            {
                // Uppercase letters
                { "Ё", "Ë" },
                { "Й", "Ĭ" },
                { "Ц", "T͡s" },
                { "Э", "Ė" },
                { "Ю", "I͡u" },
                { "Я", "I͡a" },

                // Lowercase letters
                { "ё", "ë" },
                { "й", "ĭ" },
                { "ц", "t͡s" },
                { "э", "ė" },
                { "ю", "i͡u" },
                { "я", "i͡a" },
            };

            bgnPcgnTransliterationTable = new()
            {
                { "А", "A" },
                { "Б", "B" },
                { "В", "V" },
                { "Г", "G" },
                { "Ґ", "G" },
                { "Д", "D" },
                { "Е", "E" },
                { "Ё", "Yo" },
                { "Ж", "Zh" },
                { "З", "Z" },
                { "И", "I" },
                { "Й", "Y" },
                { "Є", "Ye" },
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
                { "Ў", "V" },
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
                { "ґ", "g" },
                { "д", "d" },
                { "е", "e" },
                { "э", "e" },
                { "з", "z" },
                { "и", "i" },
                { "є", "ie" },
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
                { "ў", "v" },
                { "ф", "f" },
                { "х", "kh" },
                { "ц", "ts" },
                { "ч", "ch" },
                { "ш", "sh" },
                { "щ", "shch" },
                { "ъ", "\""},
                { "ы", "y" },
                { "ь", "" }, // '
                { "ё", "yo" },
                { "ю", "yu" },
                { "я", "ya" },
                { "ж", "zh" }
            };

            iso9TransliterationTable = new()
            {
                // Special characters
                { "Ъ", "ʺ" },
                { "Ӏ", "‡" },
                { "ʼ", "ʼ" },
                { "ˮ", "ˮ" },

                // Uppercase characters
                { "А", "A" },
                { "Ӓ", "Ä" },
                { "Ӓ̄", "Ạ̈" },
                { "Ӑ", "Ă" },
                { "А̄", "Ā" },
                { "Ӕ", "Æ" },
                { "А́", "Á" },
                { "А̊", "Å" },
                { "Б", "B" },
                { "В", "V" },
                { "Г", "G" },
                { "Ґ", "G̀" },
                { "Ѓ", "Ǵ" },
                { "Ғ", "Ġ" },
                { "Ҕ", "Ğ" },
                { "Һ", "Ḥ" },
                { "Д", "D" },
                { "Ђ", "Đ" },
                { "Е", "E" },
                { "Ӗ", "Ĕ" },
                { "Ё", "Ë" },
                { "Є", "Ê" },
                { "Ж", "Ž" },
                { "Җ", "Ž̦" }, // Or Ž̧
                { "Ӝ", "Z̄" },
                { "Ӂ", "Z̆" },
                { "З", "Z" },
                { "Ӟ", "Z̈" },
                { "Ӡ", "Ź" },
                { "Ѕ", "Ẑ" },
                { "И", "I" },
                { "Ӣ", "Ī" },
                { "И́", "Í" },
                { "Ӥ", "Î" },
                { "Й", "J" },
                { "І", "Ì" },
                { "Ї", "Ï" },
                { "І̄", "Ǐ" },
                { "Ј", "J̌" },
                { "Ј̵", "J́" },
                { "К", "K" },
                { "Ќ", "Ḱ" },
                { "Ӄ", "Ḳ" },
                { "Ҝ", "K̂" },
                { "Ҡ", "Ǩ" },
                { "Ҟ", "K̄" },
                { "Қ", "K̦" }, // Or Ķ
                { "К̨", "K̀" },
                { "Ԛ", "Q" },
                { "Л", "L" },
                { "Љ", "L̂" },
                { "Ԡ", "L̦" }, // Or Ļ
                { "М", "M" },
                { "Н", "N" },
                { "Њ", "N̂" },
                { "Ң", "N̦" }, // Or Ņ
                { "Ӊ", "Ṇ" },
                { "Ҥ", "Ṅ" },
                { "Ԋ", "Ǹ" },
                { "Ԣ", "Ń" },
                { "Ӈ", "Ň" },
                { "Н̄", "N̄" },
                { "О", "O" },
                { "Ӧ", "Ö" },
                { "Ө", "Ô" },
                { "Ӫ", "Ő" },
                { "Ӧ̄", "Ọ̈" },
                { "Ҩ", "Ò" },
                { "О́", "Ó" },
                { "О̄", "Ō" },
                { "Œ", "Œ" },
                { "П", "P" },
                { "Ҧ", "Ṕ" },
                { "Ԥ", "P̀" },
                { "Р", "R" },
                { "С", "S" },
                { "Ҫ", "Ș" }, // Or Ş
                { "С̀", "S̀" },
                { "Т", "T" },
                { "Ћ", "Ć" },
                { "Ԏ", "T̀" },
                { "Т̌", "Ť" },
                { "Ҭ", "Ț" }, // Or Ţ
                { "У", "U" },
                { "Ӱ", "Ü" },
                { "Ӯ", "Ū" },
                { "Ў", "Ŭ" },
                { "Ӳ", "Ű" },
                { "У́", "Ú" },
                { "Ӱ̄", "Ụ̈" }, // Or Ụ̄
                { "Ү", "Ù" },
                { "Ұ", "U̇" },
                { "Ԝ", "W" },
                { "Ф", "F" },
                { "Х", "H" },
                { "Ҳ", "H̦" }, // Or Ḩ
                { "Ц", "C" },
                { "Ҵ", "C̄" },
                { "Џ", "D̂" },
                { "Ч", "Č" },
                { "Ҷ", "C̦" }, // Or Ç
                { "Ӌ", "C̣" },
                { "Ӵ", "C̈" },
                { "Ҹ", "Ĉ" },
                { "Ч̀", "C̀" },
                { "Ҽ", "C̆" },
                { "Ҿ", "C̨̆" },
                { "Ш", "Š" },
                { "Щ", "Ŝ" },
                { "Ы", "Y" },
                { "Ӹ", "Ÿ" },
                { "Ы̄", "Ȳ" },
                { "Ь", "ʹ" },
                { "Э", "È" },
                { "Ә", "A̋" },
                { "Ӛ", "À" },
                { "Ю", "Û" },
                { "Ю̄", "Û̄" },
                { "Я", "Â" },
                { "Ѣ", "Ě" },
                { "Ѫ", "Ǎ" },
                { "Ѳ", "F̀" },
                { "Ѵ", "Ỳ" },

                // Uppercase characters
                { "а", "a" },
                { "ӓ", "ä" },
                { "ӓ̄", "ạ̈" },
                { "ӑ", "ă" },
                { "а̄", "ā" },
                { "ӕ", "æ" },
                { "а́", "á" },
                { "а̊", "å" },
                { "б", "b" },
                { "в", "v" },
                { "г", "g" },
                { "ґ", "g̀" },
                { "ѓ", "ǵ" },
                { "ғ", "ġ" },
                { "ҕ", "ğ" },
                { "һ", "ḥ" },
                { "д", "d" },
                { "ђ", "đ" },
                { "е", "e" },
                { "ӗ", "ĕ" },
                { "ё", "ë" },
                { "є", "ê" },
                { "ж", "ž" },
                { "җ", "ž̦" }, // Or ž̧
                { "ӝ", "z̄" },
                { "ӂ", "z̆" },
                { "з", "z" },
                { "ӟ", "z̈" },
                { "ӡ", "ź" },
                { "ѕ", "ẑ" },
                { "и", "i" },
                { "ӣ", "ī" },
                { "и́", "í" },
                { "ӥ", "î" },
                { "й", "j" },
                { "і", "ì" },
                { "ї", "ï" },
                { "і̄", "ǐ" },
                { "ј", "ǰ" },
                { "ј̵", "j́" },
                { "к", "k" },
                { "ќ", "ḱ" },
                { "ӄ", "ḳ" },
                { "ҝ", "k̂" },
                { "ҡ", "ǩ" },
                { "ҟ", "k̄" },
                { "қ", "k̦" }, // Or ķ
                { "к̨", "k̀" },
                { "ԛ", "q" },
                { "л", "l" },
                { "љ", "l̂" },
                { "ԡ", "l̦" }, // Or ļ
                { "м", "m" },
                { "н", "n" },
                { "њ", "n̂" },
                { "ң", "n̦" }, // Or ņ
                { "ӊ", "ṇ" },
                { "ҥ", "ṅ" },
                { "ԋ", "ǹ" },
                { "ԣ", "ń" },
                { "ӈ", "ň" },
                { "н̄", "n̄" },
                { "о", "o" },
                { "ӧ", "ö" },
                { "ө", "ô" },
                { "ӫ", "ő" },
                { "ӧ̄", "ọ̈" },
                { "ҩ", "ò" },
                { "о́", "ó" },
                { "о̄", "ō" },
                { "œ", "œ" },
                { "п", "p" },
                { "ҧ", "ṕ" },
                { "ԥ", "p̀" },
                { "р", "r" },
                { "с", "s" },
                { "ҫ", "ș" }, // Or ş
                { "с̀", "s̀" },
                { "т", "t" },
                { "ћ", "ć" },
                { "ԏ", "t̀" },
                { "т̌", "ť" },
                { "ҭ", "ț" }, // Or ţ
                { "у", "u" },
                { "ӱ", "ü" },
                { "ӯ", "ū" },
                { "ў", "ŭ" },
                { "ӳ", "ű" },
                { "у́", "ú" },
                { "ӱ̄", "ụ̈" }, // Or ụ̄
                { "ү", "ù" },
                { "ұ", "u̇" },
                { "ԝ", "w" },
                { "ф", "f" },
                { "х", "h" },
                { "ҳ", "h̦" }, // Or ḩ
                { "ц", "c" },
                { "ҵ", "c̄" },
                { "џ", "d̂" },
                { "ч", "č" },
                { "ҷ", "c̦" }, // Or ç
                { "ӌ", "c̣" },
                { "ӵ", "c̈" },
                { "ҹ", "ĉ" },
                { "ч̀", "c̀" },
                { "ҽ", "c̆" },
                { "ҿ", "c̨̆" },
                { "ш", "š" },
                { "щ", "ŝ" },
                { "ы", "y" },
                { "ӹ", "ÿ" },
                { "ы̄", "ȳ" },
                { "ь", "ʹ" },
                { "э", "è" },
                { "ә", "a̋" },
                { "ӛ", "à" },
                { "ю", "û" },
                { "ю̄", "û̄" },
                { "я", "â" },
                { "ѣ", "ě" },
                { "ѫ", "ǎ" },
                { "ѳ", "f̀" },
                { "ѵ", "ỳ" }
            };

            abkhazianTransliterationTable = new() // ISO-9
            {
                // Uppercase exceptions
                { "Гь", "Gʹ" },
                { "Гә", "Ga̋" },
                { "Гу", "Ga̋" },
                { "Ӷь", "Ğʹ" },
                { "Ҕь", "Ğʹ" },
                { "Ӷә", "Ğa̋" },
                { "Ҕу", "Ğa̋" },
                { "Дә", "Da̋" },
                { "Жь", "Žʹ" },
                { "Жә", "Ža̋" },
                { "Ӡә", "Źa̋" },
                { "Кь", "Kʹ" },
                { "Кә", "Ka̋" },
                { "Ку", "Ka̋" },
                { "Қь", "Ķʹ" },
                { "Қә", "Ķa̋" },
                { "Ӄу", "Ķa̋" },
                { "Ҟь", "K̄ʹ" },
                { "Ҟә", "K̄a̋" },
                { "Ҟу", "K̄a̋" },
                { "Тә", "Ta̋" },
                { "Ҭә", "Ţa̋" },
                { "Хь", "Hʹ" },
                { "Хә", "Ha̋" },
                { "Ху", "Ha̋" },
                { "Ҳә", "H̦a̋" },
                { "Цә", "Ca̋" },
                { "Ҵә", "C̄a̋" },
                { "Шь", "Šʹ" },
                { "Шә", "Ša̋" },
                { "Џь", "D̂" },

                // Uppercase characters
                { "А", "A" },
                { "Б", "B" },
                { "В", "V" },
                { "Г", "G" },
                { "Ҕ", "Ğ" },
                { "Д", "D" },
                { "Е", "E" },
                { "Ж", "Ž" },
                { "З", "Z" },
                { "Ӡ", "Ź" },
                { "И", "I" },
                { "К", "K" },
                { "Қ", "Ķ" },
                { "Ҟ", "K̄" },
                { "Л", "L" },
                { "М", "M" },
                { "Н", "N" },
                { "О", "O" },
                { "П", "P" },
                { "Ԥ", "Ṕ" },
                { "Ҧ", "Ṕ" },
                { "Р", "R" },
                { "С", "S" },
                { "Т", "T" },
                { "Ҭ", "Ţ" },
                { "У", "U" },
                { "Ф", "F" },
                { "Х", "H" },
                { "Ҳ", "H̦" },
                { "Ц", "C" },
                { "Ҵ", "C̄" },
                { "Ч", "Č" },
                { "Ҷ", "C̦" },
                { "Ҽ", "C̆" },
                { "Ҿ", "̦C̆" },
                { "Ш", "Š" },
                { "Ы", "Y" },
                { "Ҩ", "Ò" },
                { "Џ", "D̂" },
                { "Ь", "ʹ" },
                { "Ә", "A̋" },

                // Lowercase characters
                { "а", "a" },
                { "б", "b" },
                { "в", "v" },
                { "г", "g" },
                { "ҕ", "ğ" },
                { "д", "d" },
                { "е", "e" },
                { "ж", "ž" },
                { "з", "z" },
                { "ӡ", "ź" },
                { "и", "i" },
                { "к", "k" },
                { "қ", "ķ" },
                { "ҟ", "k̄" },
                { "л", "l" },
                { "м", "m" },
                { "н", "n" },
                { "о", "o" },
                { "п", "p" },
                { "ԥ", "ṕ" },
                { "ҧ", "ṕ" },
                { "р", "r" },
                { "с", "s" },
                { "т", "t" },
                { "ҭ", "ţ" },
                { "у", "u" },
                { "ф", "f" },
                { "х", "h" },
                { "ҳ", "h̦" },
                { "ц", "c" },
                { "ҵ", "c̄" },
                { "ч", "č" },
                { "ҷ", "c̦" },
                { "ҽ", "c̆" },
                { "ҿ", "̦c̆" },
                { "ш", "š" },
                { "ы", "y" },
                { "ҩ", "ò" },
                { "џ", "d̂" },
                { "ь", "ʹ" },
                { "ә", "a̋" }
            };

            belarussianTransliterationTable = new()
            {
                // Uppercase exceptions
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

                // Uppercase characters
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

            bulgarianTransliterationTable = new()
            {
                { @"ия\b", "ia" },

                { "Ъ", "Ă" }, // A officially, since the 2002 proposal did not pass
                { "Х", "H" },
                { "Щ", "Sht" },
                { "ъ", "ă" }, // a officially, since the 2002 proposal did not pass
                { "х", "h" },
                { "щ", "sht" },
            };

            chuvashTransliterationTable = new()
            {
                // Uppercase letters
                { "[ҪÇ]", "Ś" },
                { "Ӑ", "Ă" },
                { "Ӗ", "Ĕ" },
                { "Е", "Je" },
                { "Ё", "Jo" },
                { "Ж", "Ž" },
                { "Й", "J" },
                { "Ӳ", "Ü" },
                { "Ч", "Č" },
                { "Ш", "Š" },
                { "Э", "E" },
                { "Ю", "Ju" },
                { "Я", "Ja" },

                // Lowercase letters
                { @"\bе", "je" },
                { "[ҫç]", "ś" },
                { "ӑ", "ă" },
                { "ӗ", "ĕ" },
                { "ё", "jo" },
                { "ж", "ž" },
                { "й", "j" },
                { "ӳ", "ü" },
                { "ч", "č" },
                { "ш", "š" },
                { "э", "e" },
                { "ю", "ju" },
                { "я", "ja" },
            };

            kazakhTransliterationTable = new()
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

            russianTransliterationTable = new();

            macedonianTransliterationTable = new()
            {
                // Uppercase letters
                { "Ѕ", "Dz" },
                { "Ѓ", "Ǵ" },
                { "Ѐ", "È" }, // technically not a separate letter, but used to differentiate in words with homographs
                { "Ж", "Ž" },
                { "Ѝ", "Ì" },
                { "Ј", "J" },
                { "Љ", "Lj" },
                { "Њ", "Nj" },
                { "Х", "H" },
                { "Ц", "C" },
                { "Ч", "Č" },
                { "Џ", "Dž" },
                { "Ш", "Š" },

                // Lowercase letters
                { "ѕ", "dz" },
                { "ѓ", "ǵ" },
                { "ѐ", "è" }, // technically not a separate letter, but used to differentiate in words with homographs
                { "ж", "ž" },
                { "ѝ", "ì" },
                { "ј", "j" },
                { "љ", "lj" },
                { "њ", "nj" },
                { "х", "h" },
                { "ц", "c" },
                { "ч", "č" },
                { "џ", "dž" },
                { "ш", "š" },
            };

            serbianTransliterationTable = new()
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
                { "Я", "Ja" },

                // Lowercase letters
                { "ђ", "đ" },
                { "ж", "ž" },
                { "й", "j" },
                { "ј", "j" },
                { "љ", "lj" },
                { "њ", "nj" },
                { "ћ", "ć" },
                { "ц", "c" },
                { "ч", "č" },
                { "џ", "dž" },
                { "ш", "š" },
                { "я", "ja" },
            };

            tajikTransliterationTable = new()
            {
                { "[Ъъ]", "'" },

                // Uppercase letters
                { "Ғ", "Ğ" },
                { "Ё", "Jo" },
                { "Ж", "Ƶ" },
                { "Ӣ", "Ī" },
                { "Й", "J" },
                { "Қ", "Q" },
                { "Ӯ", "Ū" },
                { "Х", "X" },
                { "Ҳ", "H" },
                { "Ч", "C" },
                { "Ҷ", "Ç" },
                { "Ш", "Ș" },
                { "Ю", "Ju" },
                { "Я", "Ja" },

                // Lowercase letters
                { "ғ", "ğ" },
                { "ё", "jo" },
                { "ж", "ƶ" },
                { "ӣ", "ī" },
                { "й", "j" },
                { "қ", "q" },
                { "ӯ", "ū" },
                { "х", "x" },
                { "ҳ", "h" },
                { "ч", "c" },
                { "ҷ", "ç" },
                { "ш", "ș" },
                { "ю", "ju" },
                { "я", "ja" },
            };

            tatarTransliterationTable = new()
            {
                // Front vowels: [ÄäEeİiÖöÜüӘәЕеИиӨөҮү]
                // Back vowels:  [AaIıOoUuАаЫыОоУу]

                // Uppercase vowel harmony
                { "([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])Г", "$1G" },
                { "([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])Е", "$1Ye" },
                { "([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])К", "$1K" },
                { "([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])Ю", "$1Yü" },
                { "([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])Я", "$1Yä" },
                { "([AaIıOoUuАаЫыОоУу])Г", "$1Ğ" },
                { "([AaIıOoUuАаЫыОоУу])Е", "$1Yı" },
                { "([AaIıOoUuАаЫыОоУу])К", "$1Q" },
                { "([AaIıOoUuАаЫыОоУу])Ю", "$1Yu" },
                { "([AaIıOoUuАаЫыОоУу])Я", "$1Ya" },
                { "Г([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])", "G$1" },
                { "Г([AaIıOoUuАаЫыОоУу])", "Ğ$1" },
                { "К([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])", "K$1" },
                { "К([AaIıOoUuАаЫыОоУу])", "Q$1" },
                { "Ю([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])", "Yü$1" },
                { "Ю([AaIıOoUuАаЫыОоУу])", "Yu$1" },
                { "Я([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])", "Yä$1" },
                { "Я([AaIıOoUuАаЫыОоУу])", "Ya$1" },

                // Lowercase vowel harmony
                { "([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])г", "$1g" },
                { "([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])е", "$1ye" },
                { "([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])к", "$1k" },
                { "([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])ю", "$1yü" },
                { "([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])я", "$1yä" },
                { "([AaIıOoUuАаЫыОоУу])г", "$1ğ" },
                { "([AaIıOoUuАаЫыОоУу])е", "$1yı" },
                { "([AaIıOoUuАаЫыОоУу])к", "$1q" },
                { "([AaIıOoUuАаЫыОоУу])ю", "$1yu" },
                { "([AaIıOoUuАаЫыОоУу])я", "$1ya" },
                { "г([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])", "g$1" },
                { "г([AaIıOoUuАаЫыОоУу])", "ğ$1" },
                { "к([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])", "k$1" },
                { "к([AaIıOoUuАаЫыОоУу])", "q$1" },
                { "ю([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])", "yü$1" },
                { "ю([AaIıOoUuАаЫыОоУу])", "yu$1" },
                { "я([ÄäEeİiÖöÜüӘәЕеИиӨөҮү])", "yä$1" },
                { "я([AaIıOoUuАаЫыОоУу])", "ya$1" },

                //// Uppercase exceptions
                //{ "Аъ", "Ä" },
                //{ "Ӓ", "Ä" },
                //{ "Оъ", "Ö" },
                //{ "Ӧ", "Ö" },
                //{ "Уъ", "Ü" },
                //{ "Ӱ", "Ü" },
                //{ "Жъ", "C" },
                //{ "Нъ", "Ñ" },
                //{ "Ҥ", "Ñ" },
                //{ "Хъ", "H" },

                // Uppercase letters
                { "А", "A" },
                { "Б", "B" },
                { "В", "W" }, // Or V in Russian words
                { "Д", "D" },
                { "Е", "E" },
                { "Ж", "J" },
                { "З", "Z" },
                { "И", "İ" },
                { "Й", "Y" },
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
                { "Х", "X" },
                { "Ч", "Ç" },
                { "Ш", "Ş" },
                { "Ы", "I" },
                { "Ә", "Ä" },
                { "Ө", "Ö" },
                { "Ү", "Ü" },
                { "Җ", "C" },
                { "Ң", "Ñ" },
                { "Һ", "H" },

                // Uppercase letters - Russian
                { "Ё", "Yo" },
                { "Ц", "Ts" },
                { "Щ", "Şç" },

                //// Lowercase exceptions
                //{ "аъ", "ä" },
                //{ "ӓ", "ä" },
                //{ "оъ", "ö" },
                //{ "ӧ", "ö" },
                //{ "уъ", "ü" },
                //{ "ӱ", "ü" },
                //{ "жъ", "c" },
                //{ "нъ", "ñ" },
                //{ "ҥ", "ñ" },
                //{ "хъ", "h" },

                // Lowercase letters
                { "а", "a" },
                { "б", "b" },
                { "в", "w" }, // Or V in Russian words
                { "д", "d" },
                { "е", "e" },
                { "ж", "j" },
                { "з", "z" },
                { "и", "i" },
                { "й", "y" },
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
                { "х", "x" },
                { "ч", "ç" },
                { "ш", "ş" },
                { "ы", "ı" },
                { "ә", "ä" },
                { "ө", "ö" },
                { "ү", "ü" },
                { "җ", "c" },
                { "ң", "ñ" },
                { "һ", "h" },

                // Lowercase letters - Russian
                { "ё", "yo" },
                { "ц", "ts" },
                { "щ", "şç" },

                // Special characters
                { "Ъ", "" },
                { "Ь", "" },
                { "ъ", "" },
                { "ь", "" },
            };

            ukrainianTransliterationTable = new()
            {
                { @"ія\b", "ia" },
                { @"\b([Сс])и", "$1i" },

                { "Ґ", "G" },
                { "Г", "H" },
                { "Є", "Ye" },
                { "И", "Y" },
                { "І", "I" },
                { "Ї", "I" },
                { "Й", "I" },
                { "О́", "Ó" },
                { "Я", "Ya" },

                { "ґ", "g" },
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
                if (!alaLcTransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    alaLcTransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
                }

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

                if (!tatarTransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    tatarTransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
                }

                if (!tajikTransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    tajikTransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
                }

                if (!ukrainianTransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    ukrainianTransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
                }
            }

            foreach (var characterTransliteration in alaLcTransliterationTable)
            {
                if (!chuvashTransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    chuvashTransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
                }
            }

            foreach (var characterTransliteration in iso9TransliterationTable)
            {
                if (!abkhazianTransliterationTable.ContainsKey(characterTransliteration.Key))
                {
                    abkhazianTransliterationTable.Add(characterTransliteration.Key, characterTransliteration.Value);
                }
            }
        }

        public string Transliterate(string text, Language language)
        {
            IDictionary<string, string> transliterationTable;

            if (language.Equals(Language.Abkhaz))
            {
                transliterationTable = abkhazianTransliterationTable;
            }
            else if (language.Equals(Language.Belarussian))
            {
                transliterationTable = belarussianTransliterationTable;
            }
            else if (language.Equals(Language.Bulgarian))
            {
                transliterationTable = bulgarianTransliterationTable;
            }
            else if (language.Equals(Language.Chuvash))
            {
                transliterationTable = chuvashTransliterationTable;
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
            else if (language.Equals(Language.Tajik) ||
                     language.Equals(Language.TajikCyrillic))
            {
                transliterationTable = tajikTransliterationTable;
            }
            else if (language.Equals(Language.Tatar) ||
                     language.Equals(Language.TatarCyrillic))
            {
                transliterationTable = tatarTransliterationTable;
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

        static string ApplyBelarussianFixes(string text)
        {
            string fixedText = text;

            fixedText = Regex.Replace(fixedText, "'i", "ji");

            return fixedText;
        }

        static string ApplyRussianFixes(string text)
        {
            string fixedText = text;

            fixedText = Regex.Replace(fixedText, "Eka", "Yeka");
            fixedText = Regex.Replace(fixedText, "([eo])e", "$1ye");

            fixedText = Regex.Replace(fixedText, @"([\ \-])syur([\ \-])(.*)\b", "$1na$1$3e");

            return fixedText;
        }
    }
}
