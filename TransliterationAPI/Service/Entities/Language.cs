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

        public static Language Abkhaz => new Language("ab", "Abkhaz", nameof(TranslitterationDotComTransliterator));
        public static Language Adyghe => new Language("ady", "Adyghe", nameof(TranslitterationDotComTransliterator));
        public static Language AncientGreek => new Language("grc", "Ancient Greek", nameof(GreekTransliterator));
        public static Language AncientGreekDoric => new Language("grc-dor", "Ancient Doric Greek", nameof(GreekTransliterator));
        public static Language Arabic => new Language("ar", "Arabic", nameof(ArabicTransliterator));
        public static Language Armenian => new Language("hy", "Armenian", nameof(TranslitterationDotComTransliterator));
        public static Language Bashkir => new Language("ba", "Bashkir", nameof(TranslitterationDotComTransliterator));
        public static Language Belarussian => new Language("be", "Belarussian", nameof(TranslitterationDotComTransliterator));
        public static Language Bengali => new Language("bn", "Bengali", nameof(UshuaiaTransliterator));
        public static Language Bulgarian => new Language("bg", "Bulgarian", nameof(CyrillicTransliterator));
        public static Language Chinese => new Language("zh", "Chinese", nameof(PinyinTransliterator));
        public static Language Chuvash => new Language("cv", "Chuvash", nameof(TranslitterationDotComTransliterator));
        public static Language Coptic => new Language("cop", "Coptic", nameof(CopticTransliterator));
        public static Language EgyptianArabic => new Language("arz", "Egyptian Arabic", nameof(ArabicTransliterator));
        public static Language Georgian => new Language("ka", "Georgian", nameof(TranslitterationDotComTransliterator));
        public static Language Greek => new Language("el", "Greek", nameof(GreekTransliterator));
        public static Language Gujarati => new Language("gy", "Gujarati", nameof(GujaratiTransliterator));
        public static Language Hebrew => new Language("he", "Hebrew", nameof(HebrewTransliterator));
        public static Language Hindi => new Language("hi", "Hindi", nameof(UshuaiaTransliterator));
        public static Language Inuttitut => new Language("iu", "Inuttitut", nameof(TranslitterationDotComTransliterator));
        public static Language Japanese => new Language("ja", "Japanese", nameof(JapaneseTransliterator));
        public static Language Kannada => new Language("kn", "Kannada", nameof(UshuaiaTransliterator));
        public static Language Kazakh => new Language("kk", "Kazakh", nameof(CyrillicTransliterator));
        public static Language Korean => new Language("ko", "Korean", nameof(UshuaiaTransliterator));
        public static Language Kyrgyz => new Language("ky", "Kyrgyz", nameof(TranslitterationDotComTransliterator));
        public static Language MacedonianSlavic => new Language("mk", "Macedonian Slavic", nameof(TranslitterationDotComTransliterator));
        public static Language MaghrebiArabic => new Language("ary", "Maghrebi Arabic", nameof(ArabicTransliterator));
        public static Language Malayalam => new Language("ml", "Malayalam", nameof(UshuaiaTransliterator));
        public static Language Marathi => new Language("mr", "Marathi", nameof(MarathiTransliterator));
        public static Language Mongol => new Language("mn", "Mongol", nameof(UshuaiaTransliterator));
        public static Language OldChurchSlavonic => new Language("cu", "Old Church Slavonic", nameof(PodolakTransliterator));
        public static Language Ossetic => new Language("os", "Ossetic", nameof(TranslitterationDotComTransliterator));
        public static Language Russian => new Language("ru", "Russian", nameof(CyrillicTransliterator));
        public static Language Sanskrit => new Language("sa", "Sanskrit", nameof(UshuaiaTransliterator));
        public static Language Serbian => new Language("sr", "Serbian", nameof(TranslitterationDotComTransliterator));
        public static Language SimplifiedChinese => new Language("zh-hans", "Simplified Chinese", nameof(PinyinTransliterator));
        public static Language Sinhala => new Language("si", "Sinhala", nameof(UshuaiaTransliterator));
        public static Language Tamil => new Language("ta", "Tamil", nameof(UshuaiaTransliterator));
        public static Language Telugu => new Language("te", "Telugu", nameof(UshuaiaTransliterator));
        public static Language Udmurt => new Language("udm", "Udmurt", nameof(TranslitterationDotComTransliterator));
        public static Language Ukrainian => new Language("uk", "Ukrainian", nameof(CyrillicTransliterator));
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
