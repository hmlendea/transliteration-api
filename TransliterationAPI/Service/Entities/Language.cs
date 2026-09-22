using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.Service.Entities
{
    public sealed class Language : IEquatable<Language>
    {
        static readonly IDictionary<string, Language> entries;

        public static Language Abkhaz => new("ab", nameof(Abkhaz), typeof(CyrillicTransliterator));
        public static Language Adyghe => new("ady", nameof(Adyghe), typeof(TranslitterationDotComTransliterator));
        public static Language AncientGreek => new("grc", $"Ancient {nameof(Greek)}", typeof(GreekTransliterator));
        public static Language AncientGreekDoric => new("grc-dor", $"Ancient Doric {nameof(Greek)}", typeof(GreekTransliterator));
        public static Language Arabic => new("ar", nameof(Arabic), typeof(ArabicTransliterator));
        public static Language Armenian => new("hy", nameof(Armenian), typeof(TranslitterationDotComTransliterator));
        public static Language Bashkir => new("ba", nameof(Bashkir), typeof(TranslitterationDotComTransliterator));
        public static Language Belarussian => new("be", nameof(Belarussian), typeof(CyrillicTransliterator));
        public static Language Bengali => new("bn", nameof(Bengali), typeof(UshuaiaTransliterator));
        public static Language Berber => new("ber", nameof(Berber), typeof(BerberTransliterator));
        public static Language Bulgarian => new("bg", nameof(Bulgarian), typeof(CyrillicTransliterator));
        public static Language Chinese => new("zh", nameof(Chinese), typeof(PinyinTransliterator));
        public static Language Chuvash => new("cv", nameof(Chuvash), typeof(CyrillicTransliterator));
        public static Language Coptic => new("cop", nameof(Coptic), typeof(CopticTransliterator));
        public static Language EgyptianArabic => new("arz", $"Egyptian {nameof(Arabic)}", typeof(ArabicTransliterator));
        public static Language Georgian => new("ka", nameof(Georgian), typeof(TranslitterationDotComTransliterator));
        public static Language Greek => new("el", nameof(Greek), typeof(GreekTransliterator));
        public static Language Gujarati => new("gy", nameof(Gujarati), typeof(GujaratiTransliterator));
        public static Language Hebrew => new("he", nameof(Hebrew), typeof(HebrewTransliterator));
        public static Language Hindi => new("hi", nameof(Hindi), typeof(UshuaiaTransliterator));
        public static Language Inuttitut => new("iu", nameof(Inuttitut), typeof(TranslitterationDotComTransliterator));
        public static Language Japanese => new("ja", nameof(Japanese), typeof(JapaneseTransliterator));
        public static Language Kannada => new("kn", nameof(Kannada), typeof(UshuaiaTransliterator));
        public static Language Kazakh => new("kk", nameof(Kazakh), typeof(CyrillicTransliterator));
        public static Language Korean => new("ko", nameof(Korean), typeof(KoreanTransliterator));
        public static Language Kyrgyz => new("ky", nameof(Kyrgyz), typeof(TranslitterationDotComTransliterator));
        public static Language MacedonianSlavic => new("mk", "Macedonian Slavic", typeof(CyrillicTransliterator));
        public static Language MaghrebiArabic => new("ary", $"Maghrebi {nameof(Arabic)}", typeof(ArabicTransliterator));
        public static Language Malayalam => new("ml", nameof(Malayalam), typeof(UshuaiaTransliterator));
        public static Language Marathi => new("mr", nameof(Marathi), typeof(MarathiTransliterator));
        public static Language Mongol => new("mn", nameof(Mongol), typeof(UshuaiaTransliterator));
        public static Language OldChurchSlavonic => new("cu", "Old Church Slavonic", typeof(PodolakTransliterator));
        public static Language Ossetic => new("os", nameof(Ossetic), typeof(TranslitterationDotComTransliterator));
        public static Language Russian => new("ru", nameof(Russian), typeof(CyrillicTransliterator));
        public static Language Sanskrit => new("sa", nameof(Sanskrit), typeof(UshuaiaTransliterator));
        public static Language Serbian => new("sr", nameof(Serbian), typeof(CyrillicTransliterator));
        public static Language SerbianCyrillic => new("sr-ec", nameof(Serbian), typeof(CyrillicTransliterator));
        public static Language SerboCroatian => new("sh", "Serbo-Croatian", typeof(CyrillicTransliterator));
        public static Language SimplifiedChinese => new("zh-hans", "Simplified Chinese", typeof(PinyinTransliterator));
        public static Language Sinhala => new("si", nameof(Sinhala), typeof(UshuaiaTransliterator));
        public static Language Tajik => new("tg", nameof(Tajik), typeof(CyrillicTransliterator));
        public static Language TajikCyrillic => new("tg-cyrl", nameof(Tajik), typeof(CyrillicTransliterator));
        public static Language Tamil => new("ta", nameof(Tamil), typeof(UshuaiaTransliterator));
        public static Language Tatar => new("tt", nameof(Tatar), typeof(CyrillicTransliterator));
        public static Language TatarCyrillic => new("tt-cyrl", nameof(Tatar), typeof(CyrillicTransliterator));
        public static Language Telugu => new("te", nameof(Telugu), typeof(UshuaiaTransliterator));
        public static Language Udmurt => new("udm", nameof(Udmurt), typeof(TranslitterationDotComTransliterator));
        public static Language Ukrainian => new("uk", nameof(Ukrainian), typeof(CyrillicTransliterator));
        public static Language WesternArmenian => new("hyw", "Western Armenian", typeof(TranslitterationDotComTransliterator));

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

        public string Transliterator => TransliteratorType.Name;

        [JsonIgnore]
        public Type TransliteratorType { get; }

        [JsonIgnore]
        public bool UsesExternalTransliterator
            => typeof(IExternalTransliterator).IsAssignableFrom(TransliteratorType);

        private Language(string code, string name, Type transliteratorType)
        {
            Code = code;
            Name = name;
            TransliteratorType = transliteratorType;
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
