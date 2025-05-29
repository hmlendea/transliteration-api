using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.Service.Entities
{
    public sealed class Language : IEquatable<Language>
    {
        static IDictionary<string, Language> entries;

        public static Language Abkhaz => new Language("ab", nameof(Abkhaz), nameof(TranslitterationDotComTransliterator));
        public static Language Adyghe => new Language("ady", nameof(Adyghe), nameof(TranslitterationDotComTransliterator));
        public static Language AncientGreek => new Language("grc", $"Ancient {nameof(Greek)}", nameof(GreekTransliterator));
        public static Language AncientGreekDoric => new Language("grc-dor", $"Ancient Doric {nameof(Greek)}", nameof(GreekTransliterator));
        public static Language Arabic => new Language("ar", nameof(Arabic), nameof(ArabicTransliterator));
        public static Language Armenian => new Language("hy", nameof(Armenian), nameof(TranslitterationDotComTransliterator));
        public static Language Bashkir => new Language("ba", nameof(Bashkir), nameof(TranslitterationDotComTransliterator));
        public static Language Belarussian => new Language("be", nameof(Belarussian), nameof(CyrillicTransliterator));
        public static Language Bengali => new Language("bn", nameof(Bengali), nameof(UshuaiaTransliterator));
        public static Language Berber => new Language("ber", nameof(Berber), nameof(TranslitterationDotComTransliterator));
        public static Language Bulgarian => new Language("bg", nameof(Bulgarian), nameof(CyrillicTransliterator));
        public static Language Chinese => new Language("zh", nameof(Chinese), nameof(PinyinTransliterator));
        public static Language Chuvash => new Language("cv", nameof(Chuvash), nameof(CyrillicTransliterator));
        public static Language Coptic => new Language("cop", nameof(Coptic), nameof(CopticTransliterator));
        public static Language EgyptianArabic => new Language("arz", $"Egyptian {nameof(Arabic)}", nameof(ArabicTransliterator));
        public static Language Georgian => new Language("ka", nameof(Georgian), nameof(TranslitterationDotComTransliterator));
        public static Language Greek => new Language("el", nameof(Greek), nameof(GreekTransliterator));
        public static Language Gujarati => new Language("gy", nameof(Gujarati), nameof(GujaratiTransliterator));
        public static Language Hebrew => new Language("he", nameof(Hebrew), nameof(HebrewTransliterator));
        public static Language Hindi => new Language("hi", nameof(Hindi), nameof(UshuaiaTransliterator));
        public static Language Inuttitut => new Language("iu", nameof(Inuttitut), nameof(TranslitterationDotComTransliterator));
        public static Language Japanese => new Language("ja", nameof(Japanese), nameof(JapaneseTransliterator));
        public static Language Kannada => new Language("kn", nameof(Kannada), nameof(UshuaiaTransliterator));
        public static Language Kazakh => new Language("kk", nameof(Kazakh), nameof(CyrillicTransliterator));
        public static Language Korean => new Language("ko", nameof(Korean), nameof(KoreanTransliterator));
        public static Language Kyrgyz => new Language("ky", nameof(Kyrgyz), nameof(TranslitterationDotComTransliterator));
        public static Language MacedonianSlavic => new Language("mk", "Macedonian Slavic", nameof(CyrillicTransliterator));
        public static Language MaghrebiArabic => new Language("ary", $"Maghrebi {nameof(Arabic)}", nameof(ArabicTransliterator));
        public static Language Malayalam => new Language("ml", nameof(Malayalam), nameof(UshuaiaTransliterator));
        public static Language Marathi => new Language("mr", nameof(Marathi), nameof(MarathiTransliterator));
        public static Language Mongol => new Language("mn", nameof(Mongol), nameof(UshuaiaTransliterator));
        public static Language OldChurchSlavonic => new Language("cu", "Old Church Slavonic", nameof(PodolakTransliterator));
        public static Language Ossetic => new Language("os", nameof(Ossetic), nameof(TranslitterationDotComTransliterator));
        public static Language Russian => new Language("ru", nameof(Russian), nameof(CyrillicTransliterator));
        public static Language Sanskrit => new Language("sa", nameof(Sanskrit), nameof(UshuaiaTransliterator));
        public static Language Serbian => new Language("sr", nameof(Serbian), nameof(CyrillicTransliterator));
        public static Language SerbianCyrillic => new Language("sr-ec", nameof(Serbian), nameof(CyrillicTransliterator));
        public static Language SerboCroatian => new Language("sh", "Serbo-Croatian", nameof(CyrillicTransliterator));
        public static Language SimplifiedChinese => new Language("zh-hans", "Simplified Chinese", nameof(PinyinTransliterator));
        public static Language Sinhala => new Language("si", nameof(Sinhala), nameof(UshuaiaTransliterator));
        public static Language TajikiCyrillic => new Language("tg", nameof(TajikiCyrillic), nameof(CyrillicTransliterator));
        public static Language Tamil => new Language("ta", nameof(Tamil), nameof(UshuaiaTransliterator));
        public static Language Telugu => new Language("te", nameof(Telugu), nameof(UshuaiaTransliterator));
        public static Language Udmurt => new Language("udm", nameof(Udmurt), nameof(TranslitterationDotComTransliterator));
        public static Language Ukrainian => new Language("uk", nameof(Ukrainian), nameof(CyrillicTransliterator));
        public static Language WesternArmenian => new Language("hyw", "Western Armenian", nameof(TranslitterationDotComTransliterator));

        static Language()
        {
            entries = new Dictionary<string, Language>();

            Type currentType = typeof(Language);
            PropertyInfo[] properties = currentType.GetProperties(BindingFlags.Static | BindingFlags.Public);

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(Language))
                {
                    Language language = (Language)property.GetValue(null);
                    entries.Add(language.Code, language);
                }
            }
        }

        public string Code { get; }

        public string Name { get; }

        public string Transliterator { get; }

        private Language(string code, string name, string transliterator)
        {
            Code = code;
            Name = name;
            Transliterator = transliterator;
        }

        public static Language FromCode(string code)
        {
            if (!entries.ContainsKey(code))
            {
                throw new ArgumentException($"A {nameof(Language)} with the code \"{code}\" does not exist");
            }

            return entries[code];
        }

        public override string ToString()
            => Code;

        public override int GetHashCode()
            => Code.GetHashCode();

        public bool Equals(Language other)
        {
            if (other is null)
            {
                return false;
            }

            if (!other.Code.Equals(Code))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            return Equals(obj as Language);
        }

        public static bool operator ==(Language me, Language other)
        {
            if (me is null)
            {
                return other is null;
            }

            return me.Equals(other);
        }

        public static bool operator !=(Language me, Language other)
            => !(me == other);

        public static IEnumerable<Language> GetAll()
            => entries.Values.ToList();

        public static implicit operator string(Language me)
            => me.Code;

        public static implicit operator Language(string languageCode)
            => Language.FromCode(languageCode);
    }
}
