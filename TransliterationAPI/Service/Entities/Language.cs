using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.Service.Entities
{
    public sealed class Language : IEquatable<Language>
    {
        static readonly IDictionary<string, Language> entries;

        public static Language Abkhaz => new("ab", nameof(Abkhaz), nameof(TranslitterationDotComTransliterator));
        public static Language Adyghe => new("ady", nameof(Adyghe), nameof(TranslitterationDotComTransliterator));
        public static Language AncientGreek => new("grc", $"Ancient {nameof(Greek)}", nameof(GreekTransliterator));
        public static Language AncientGreekDoric => new("grc-dor", $"Ancient Doric {nameof(Greek)}", nameof(GreekTransliterator));
        public static Language Arabic => new("ar", nameof(Arabic), nameof(ArabicTransliterator));
        public static Language Armenian => new("hy", nameof(Armenian), nameof(TranslitterationDotComTransliterator));
        public static Language Bashkir => new("ba", nameof(Bashkir), nameof(TranslitterationDotComTransliterator));
        public static Language Belarussian => new("be", nameof(Belarussian), nameof(CyrillicTransliterator));
        public static Language Bengali => new("bn", nameof(Bengali), nameof(UshuaiaTransliterator));
        public static Language Berber => new("ber", nameof(Berber), nameof(TranslitterationDotComTransliterator));
        public static Language Bulgarian => new("bg", nameof(Bulgarian), nameof(CyrillicTransliterator));
        public static Language Chinese => new("zh", nameof(Chinese), nameof(PinyinTransliterator));
        public static Language Chuvash => new("cv", nameof(Chuvash), nameof(CyrillicTransliterator));
        public static Language Coptic => new("cop", nameof(Coptic), nameof(CopticTransliterator));
        public static Language EgyptianArabic => new("arz", $"Egyptian {nameof(Arabic)}", nameof(ArabicTransliterator));
        public static Language Georgian => new("ka", nameof(Georgian), nameof(TranslitterationDotComTransliterator));
        public static Language Greek => new("el", nameof(Greek), nameof(GreekTransliterator));
        public static Language Gujarati => new("gy", nameof(Gujarati), nameof(GujaratiTransliterator));
        public static Language Hebrew => new("he", nameof(Hebrew), nameof(HebrewTransliterator));
        public static Language Hindi => new("hi", nameof(Hindi), nameof(UshuaiaTransliterator));
        public static Language Inuttitut => new("iu", nameof(Inuttitut), nameof(TranslitterationDotComTransliterator));
        public static Language Japanese => new("ja", nameof(Japanese), nameof(JapaneseTransliterator));
        public static Language Kannada => new("kn", nameof(Kannada), nameof(UshuaiaTransliterator));
        public static Language Kazakh => new("kk", nameof(Kazakh), nameof(CyrillicTransliterator));
        public static Language Korean => new("ko", nameof(Korean), nameof(KoreanTransliterator));
        public static Language Kyrgyz => new("ky", nameof(Kyrgyz), nameof(TranslitterationDotComTransliterator));
        public static Language MacedonianSlavic => new("mk", "Macedonian Slavic", nameof(CyrillicTransliterator));
        public static Language MaghrebiArabic => new("ary", $"Maghrebi {nameof(Arabic)}", nameof(ArabicTransliterator));
        public static Language Malayalam => new("ml", nameof(Malayalam), nameof(UshuaiaTransliterator));
        public static Language Marathi => new("mr", nameof(Marathi), nameof(MarathiTransliterator));
        public static Language Mongol => new("mn", nameof(Mongol), nameof(UshuaiaTransliterator));
        public static Language OldChurchSlavonic => new("cu", "Old Church Slavonic", nameof(PodolakTransliterator));
        public static Language Ossetic => new("os", nameof(Ossetic), nameof(TranslitterationDotComTransliterator));
        public static Language Russian => new("ru", nameof(Russian), nameof(CyrillicTransliterator));
        public static Language Sanskrit => new("sa", nameof(Sanskrit), nameof(UshuaiaTransliterator));
        public static Language Serbian => new("sr", nameof(Serbian), nameof(CyrillicTransliterator));
        public static Language SerbianCyrillic => new("sr-ec", nameof(Serbian), nameof(CyrillicTransliterator));
        public static Language SerboCroatian => new("sh", "Serbo-Croatian", nameof(CyrillicTransliterator));
        public static Language SimplifiedChinese => new("zh-hans", "Simplified Chinese", nameof(PinyinTransliterator));
        public static Language Sinhala => new("si", nameof(Sinhala), nameof(UshuaiaTransliterator));
        public static Language Tajik => new("tg", nameof(Tajik), nameof(CyrillicTransliterator));
        public static Language TajikCyrillic => new("tg-cyrl", nameof(Tajik), nameof(CyrillicTransliterator));
        public static Language Tamil => new("ta", nameof(Tamil), nameof(UshuaiaTransliterator));
        public static Language Telugu => new("te", nameof(Telugu), nameof(UshuaiaTransliterator));
        public static Language Udmurt => new("udm", nameof(Udmurt), nameof(TranslitterationDotComTransliterator));
        public static Language Ukrainian => new("uk", nameof(Ukrainian), nameof(CyrillicTransliterator));
        public static Language WesternArmenian => new("hyw", "Western Armenian", nameof(TranslitterationDotComTransliterator));

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
            if (!entries.TryGetValue(code, out Language value))
            {
                throw new ArgumentException($"A {nameof(Language)} with the code \"{code}\" does not exist");
            }

            return value;
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
            => FromCode(languageCode);
    }
}
