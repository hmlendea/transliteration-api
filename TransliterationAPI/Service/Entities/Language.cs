using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TransliterationAPI.Service.Entities
{
    public sealed class Language : IEquatable<Language>
    {
        static IDictionary<string, Language> entries;

        public static Language Abkhaz => new Language("ab", "Abkhaz");
        public static Language Adyghe => new Language("ady", "Adyghe");
        public static Language Arabic => new Language("ar", "Arabic");
        public static Language MaghrebiArabic => new Language("ary", "Maghrebi Arabic");
        public static Language EgyptianArabic => new Language("arz", "Egyptian Arabic");
        public static Language Bashkir => new Language("ba", "Bashkir");
        public static Language Belarussian => new Language("be", "Belarussian");
        public static Language Bulgarian => new Language("bg", "Bulgarian");
        public static Language Bengali => new Language("bn", "Bengali");
        public static Language Coptic => new Language("cop", "Coptic");
        public static Language OldChurchSlavonic => new Language("cu", "Old Church Slavonic");
        public static Language Chuvash => new Language("cv", "Chuvash");
        public static Language Greek => new Language("el", "Greek");
        public static Language AncientGreek => new Language("grc", "Ancient Greek");
        public static Language AncientGreekDoric => new Language("grc-dor", "Ancient Doric Greek");
        public static Language Gujarati => new Language("gy", "Gujarati");
        public static Language Hebrew => new Language("he", "Hebrew");
        public static Language Hindi => new Language("hi", "Hindi");
        public static Language Armenian => new Language("hy", "Armenian");
        public static Language WesternArmenian => new Language("hyw", "Western Armenian");
        public static Language Inuttitut => new Language("iu", "Inuttitut");
        public static Language Japanese => new Language("ja", "Japanese");
        public static Language Georgian => new Language("ka", "Georgian");
        public static Language Kazakh => new Language("kk", "Kazakh");
        public static Language Kannada => new Language("kn", "Kannada");
        public static Language Korean => new Language("ko", "Korean");
        public static Language Kyrgyz => new Language("ky", "Kyrgyz");
        public static Language MacedonianSlavic => new Language("mk", "Macedonian Slavic");
        public static Language Malayalam => new Language("ml", "Malayalam");
        public static Language Mongol => new Language("mn", "Mongol");
        public static Language Marathi => new Language("mr", "Marathi");
        public static Language Ossetic => new Language("os", "Ossetic");
        public static Language Russian => new Language("ru", "Russian");
        public static Language Sanskrit => new Language("sa", "Sanskrit");
        public static Language Sinhala => new Language("si", "Sinhala");
        public static Language Serbian => new Language("sr", "Serbian");
        public static Language Tamil => new Language("ta", "Tamil");
        public static Language Telugu => new Language("te", "Telugu");
        public static Language Thai => new Language("th", "Thai");
        public static Language Udmurt => new Language("udm", "Udmurt");
        public static Language Ukrainian => new Language("uk", "Ukrainian");
        public static Language Chinese => new Language("zh", "Chinese");
        public static Language SimplifiedChinese => new Language("zh-hans", "Simplified Chinese");

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

        private Language(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public static Language FromId(string code)
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
            => me.Name;

        public static implicit operator Language(string languageCode)
            => Language.FromId(languageCode);
    }
}
