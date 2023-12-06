using System.Collections.Generic;
using System.Text.RegularExpressions;

using NuciExtensions;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class ArabicTransliterator : ITransliterator
    {
        Dictionary<string, string> transliterationTable;
        Dictionary<string, string> maghrebiTransliterationTable;

        public ArabicTransliterator()
        {
            transliterationTable = new Dictionary<string, string>()
            {
                { "ا", "ā" },
                { "آ", "\u02beā" },
                { "ء", "\u02be" },
                { "إ", "i" },
                { "أ", "\u02bea" }, // ou u ?
                { "ؤ", "u\u02be" },
                { "ئ", "ʾi" },

                { "ب", "b" },
                { "ت", "t" },
                { "ث", "th" }, // ṯ
                { "ج", "ǧ" },
                { "ح", "ḥ" },
                { "خ", "ẖ" },
                { "د", "d" },
                { "ذ", "ḏ" },
                { "ر", "r" },
                { "ز", "z" },
                { "س", "s" },
                { "ش", "š" },
                { "ص", "ṣ" },
                { "ض", "ḍ" },
                { "ط", "ṭ" },
                { "ظ", "ẓ" },
                { "ع", "ʿ" },
                { "غ", "ġ" },
                { "ف", "f" },
                { "ق", "q" },
                { "ك", "k" },
                { "ل", "l" },
                { "م", "m" },
                { "ن", "n" },
                { "ه", "h" },
                { "ة", "ah" }, // ISO-233: ẗ // ah ou
                { "و", "ū" },  // ISO-233: w
                { "ي", "ī" }, // ISO-233: y

                { "ڭ", "gu" },
            };

            maghrebiTransliterationTable = new Dictionary<string, string>()
            {
                { "گ", "g" },
                { "ڤ", "g" }
            };
        }

        public string Transliterate(string text, Language language)
        {
            string transliteratedText = text;

            if (language.Equals(Language.MaghrebiArabic))
            {
                foreach (string character in maghrebiTransliterationTable.Keys)
                {
                    transliteratedText = Regex.Replace(transliteratedText, character, maghrebiTransliterationTable[character]);
                }
            }

            foreach (string character in transliterationTable.Keys)
            {
                transliteratedText = Regex.Replace(transliteratedText, character, transliterationTable[character]);
            }

            // cas du a initial
            transliteratedText = transliteratedText.Replace(" \u02be", " ");
            // pb rawTransliteratedText = Regex.Replace(rawTransliteratedText, "-\u02be", "-");
            transliteratedText = transliteratedText.Replace(" ā", " a");
            transliteratedText = transliteratedText.Replace(" al", " al-");
            transliteratedText = transliteratedText.Replace("\n\u02be", "\n");
            transliteratedText = transliteratedText.Replace("\nā", "\na");
            transliteratedText = transliteratedText.Replace("\nal", "\nal-");

            // voyelles
            transliteratedText = transliteratedText.Replace("َ", "a"); //1614
            transliteratedText = transliteratedText.Replace("ُ", "u"); //1615
            transliteratedText = transliteratedText.Replace("ِ", "i"); //1616
            transliteratedText = transliteratedText.Replace("ّ", "w"); //1617

            transliteratedText = transliteratedText.Replace("ىْ", "y");
            transliteratedText = transliteratedText.Replace("ى", "i");

            // cas du w et y
            transliteratedText = transliteratedText.Replace("aā", "ā");
            transliteratedText = transliteratedText.Replace("aa", "ā");
            transliteratedText = transliteratedText.Replace("ūa", "wa");
            transliteratedText = transliteratedText.Replace("ūā", "wā");
            transliteratedText = transliteratedText.Replace("ūu", "wu");
            transliteratedText = transliteratedText.Replace("ūū", "wu");
            transliteratedText = transliteratedText.Replace("ūw", "ww");
            transliteratedText = transliteratedText.Replace("ūi", "wi");
            transliteratedText = transliteratedText.Replace("ūī", "wy");
            transliteratedText = transliteratedText.Replace("uu", "ū");
            transliteratedText = transliteratedText.Replace("uw", "ū");
            transliteratedText = transliteratedText.Replace("uū", "ū");
            transliteratedText = transliteratedText.Replace("īī", "yi");
            transliteratedText = transliteratedText.Replace("ii", "ī");
            transliteratedText = transliteratedText.Replace("iī", "ī");
            transliteratedText = transliteratedText.Replace("īy", "ī");

            // double conso
            transliteratedText = transliteratedText.Replace("rً", "rr");
            transliteratedText = transliteratedText.Replace("lً", "ll");
            transliteratedText = transliteratedText.Replace("bً", "bb");
            transliteratedText = transliteratedText.Replace("tً", "tt");
            transliteratedText = transliteratedText.Replace("ṭṭً", "ṭṭ");
            transliteratedText = transliteratedText.Replace("mً", "mm");
            transliteratedText = transliteratedText.Replace("nً", "nn");
            transliteratedText = transliteratedText.Replace("dً", "dd");
            transliteratedText = transliteratedText.Replace("ḍً", "ḍḍ");
            transliteratedText = transliteratedText.Replace("sً", "ss");
            transliteratedText = transliteratedText.Replace("tً", "tt");
            transliteratedText = transliteratedText.Replace("ṣṣً", "ṣṣ");

            transliteratedText = transliteratedText.Replace("،", ",");
            transliteratedText = transliteratedText.Replace("؛", ";");
            transliteratedText = transliteratedText.Replace("؟", "?");
            transliteratedText = transliteratedText.Replace("ـ", ".");

            transliteratedText = transliteratedText.Replace("٠", "0");
            transliteratedText = transliteratedText.Replace("١", "1");
            transliteratedText = transliteratedText.Replace("٢", "2");
            transliteratedText = transliteratedText.Replace("٣", "3");
            transliteratedText = transliteratedText.Replace("٤", "4");
            transliteratedText = transliteratedText.Replace("۴", "4");
            transliteratedText = transliteratedText.Replace("٥", "5");
            transliteratedText = transliteratedText.Replace("۵", "5");
            transliteratedText = transliteratedText.Replace("٦", "6");
            transliteratedText = transliteratedText.Replace("۶", "6");
            transliteratedText = transliteratedText.Replace("٧", "7");
            transliteratedText = transliteratedText.Replace("٨", "8");
            transliteratedText = transliteratedText.Replace("٩", "9");

            return ApplyFixes(transliteratedText);
        }

        string ApplyFixes(string text)
        {
            string fixedText = text;

            fixedText = fixedText.ToTitleCase();

            fixedText = fixedText.Replace(" [Dd]ī[l]* ", " āl-");
            fixedText = fixedText.Replace("ūrfū", "ūrifū");

            fixedText = Regex.Replace(fixedText, @"īْ", "y");
            fixedText = Regex.Replace(fixedText, @"nْ", "n");
            fixedText = Regex.Replace(fixedText, @"sْ", "s");
            fixedText = Regex.Replace(fixedText, @"ṣْ", "ṣ");
            fixedText = Regex.Replace(fixedText, @"yْ", "y");

            fixedText = Regex.Replace(fixedText, @"[ -][Dd][eīo][ls]*[ -]", " al-");
            fixedText = Regex.Replace(fixedText, @"[ -][Ss][uū]r[ -]", " al-");
            fixedText = Regex.Replace(fixedText, @"ٱl", "al-");
            fixedText = Regex.Replace(fixedText, @" Al[ -]", " al-");
            fixedText = Regex.Replace(fixedText, @" Āl[ -]", " āl-");
            fixedText = Regex.Replace(fixedText, @"^Al([ -])*", "al-$1");
            fixedText = Regex.Replace(fixedText, @"^Āl([ -])*", "āl-$1");

            fixedText = Regex.Replace(fixedText, "-(\\p{L})", m => "-" + m.Groups[1].Value.ToUpperInvariant());
            fixedText = Regex.Replace(fixedText, @"al--([A-Z])", " al-$1");

            fixedText = Regex.Replace(fixedText, @"([\\ \\-])ʾ[Aa]r", "$1ʾUr");
            fixedText = Regex.Replace(fixedText, @"([Ǧǧ])b", "$1ib");
            fixedText = Regex.Replace(fixedText, @"([ĠQġq])r", "$1ar");
            fixedText = Regex.Replace(fixedText, @"([Īī])wā", "$1ā");
            fixedText = Regex.Replace(fixedText, @"([Nn])m", "$1im");
            fixedText = Regex.Replace(fixedText, @"([Qq])f", "$1if");
            fixedText = Regex.Replace(fixedText, @"([Tt])n", "$1inn");
            fixedText = Regex.Replace(fixedText, @"([ʿʾ])[Mm]ā", "$1Amā");
            fixedText = Regex.Replace(fixedText, @"([ʿʾ])a", "$1A");
            fixedText = Regex.Replace(fixedText, @"\b[IĪiī]s", "ʾIs");
            fixedText = Regex.Replace(fixedText, @"^(H̱|ẖ|ẖ)", "Kh");
            fixedText = Regex.Replace(fixedText, @"^[IĪiī]s", "ʾIs");
            fixedText = Regex.Replace(fixedText, @"Ā([bf])", "A$1");
            fixedText = Regex.Replace(fixedText, @"Bā", "Ba");
            fixedText = Regex.Replace(fixedText, @"Dm", "Dim");
            fixedText = Regex.Replace(fixedText, @"F([ht])", "Fa$1");
            fixedText = Regex.Replace(fixedText, @"Hr", "Hir");
            fixedText = Regex.Replace(fixedText, @"R([hm])", "Ra$1");
            fixedText = Regex.Replace(fixedText, @"ʾAb", "Ab");
            fixedText = Regex.Replace(fixedText, @"-(H̱|ẖ|ẖ)", "-Kh");

            fixedText = Regex.Replace(fixedText, @"^ǧ", "Ǧ");
            fixedText = Regex.Replace(fixedText, @"([ -])ǧ", "$1Ǧ");
            fixedText = Regex.Replace(fixedText, @"([ -])ẖ", "$1H̱");

            fixedText = Regex.Replace(fixedText, @"([Bb])([nr])", "$1a$2");
            fixedText = Regex.Replace(fixedText, @"([Bb])([t])", "$1i$2");
            fixedText = Regex.Replace(fixedText, @"([Bb])ū", "$1aū");
            fixedText = Regex.Replace(fixedText, @"([Bb])ʾir", "$1iʾr");
            fixedText = Regex.Replace(fixedText, @"([Ff])r", "$1ur");
            fixedText = Regex.Replace(fixedText, @"([ǦJǧj])m", "$1um");
            fixedText = Regex.Replace(fixedText, @"([HḤhḥ])([blmrsṣštṭ])", "$1a$2");
            fixedText = Regex.Replace(fixedText, @"([KQkq])s", "$1as");
            fixedText = Regex.Replace(fixedText, @"([Kk])w", "$1uw");
            fixedText = Regex.Replace(fixedText, @"([LMlm])([ln])", "$1a$2");
            fixedText = Regex.Replace(fixedText, @"([Mm])([sṣ])", "$1i$2");
            fixedText = Regex.Replace(fixedText, @"([Mm])q", "$1uq");
            fixedText = Regex.Replace(fixedText, @"([Rr])w", "$1ū");
            fixedText = Regex.Replace(fixedText, @"([Ss])b", "$1ab");
            fixedText = Regex.Replace(fixedText, @"([Šš])n", "$1in");
            fixedText = Regex.Replace(fixedText, @"([Ṭṭ])l", "$1al");
            fixedText = Regex.Replace(fixedText, @"āhٌ", "ah");
            fixedText = Regex.Replace(fixedText, @"āšr\b", "āšir ");
            fixedText = Regex.Replace(fixedText, @"āūm", "āwam");
            fixedText = Regex.Replace(fixedText, @"bl", "bil");
            fixedText = Regex.Replace(fixedText, @"dn", "dun");
            fixedText = Regex.Replace(fixedText, @"dwd", "dūd");
            fixedText = Regex.Replace(fixedText, @"īā", "iyā");
            fixedText = Regex.Replace(fixedText, @"īah\b", "iyyah");
            fixedText = Regex.Replace(fixedText, @"īī", "ī");
            fixedText = Regex.Replace(fixedText, @"īna\b", "īn");
            fixedText = Regex.Replace(fixedText, @"l--[Ll]", "ll");
            fixedText = Regex.Replace(fixedText, @"lyā", "liyā");
            fixedText = Regex.Replace(fixedText, @"lyl", "līl");
            fixedText = Regex.Replace(fixedText, @"mr", "mir");
            fixedText = Regex.Replace(fixedText, @"mš", "maš");
            fixedText = Regex.Replace(fixedText, @"r([kǧ])", "ra$1");
            fixedText = Regex.Replace(fixedText, @"shׂr", "sr");
            fixedText = Regex.Replace(fixedText, @"sn", "san");
            fixedText = Regex.Replace(fixedText, @"tl", "til");
            fixedText = Regex.Replace(fixedText, @"tsl", "tsil");
            fixedText = Regex.Replace(fixedText, @"un\b", "unn");
            fixedText = Regex.Replace(fixedText, @"w([my])", "wa$1");
            fixedText = Regex.Replace(fixedText, @"w\b", "ū");
            fixedText = Regex.Replace(fixedText, @"wq", "q");
            fixedText = Regex.Replace(fixedText, @"ʿr", "ʿIr");

            fixedText = Regex.Replace(fixedText, @"([Ii])salām", "$1slām");
            fixedText = Regex.Replace(fixedText, @"ahar", "ahr");
            fixedText = Regex.Replace(fixedText, @"al-[Ll]h", "Allāh");
            fixedText = Regex.Replace(fixedText, @"azawah", "azzah");
            fixedText = Regex.Replace(fixedText, @"ẖr", "ẖar");
            fixedText = Regex.Replace(fixedText, @"īrah", "īra");
            fixedText = Regex.Replace(fixedText, @"īwah", "iyya");
            fixedText = Regex.Replace(fixedText, @"myah", "miyyah");
            fixedText = Regex.Replace(fixedText, @"rzn", "rzin");

            fixedText = Regex.Replace(fixedText, @"[aā]l-([BFKM])", "al-$1");
            fixedText = Regex.Replace(fixedText, @"[aā]l-[Nn]", "an-N");
            fixedText = Regex.Replace(fixedText, @"[aā]l-[Rr]", "ar-R");
            fixedText = Regex.Replace(fixedText, @"[Āā]l-ʾ", "Al-ʾ");
            fixedText = Regex.Replace(fixedText, @"[AĀaā]l-[Ii]s", "al-ʾIs");
            fixedText = Regex.Replace(fixedText, @"[AĀaā]l-[Kk]h", "al-Kh");
            fixedText = Regex.Replace(fixedText, @"[AĀaā]l-[Ss]([aā])", "as-S$1");
            fixedText = Regex.Replace(fixedText, @"[AĀaā]l-ʿ[Aa]m", "al-ʿAm");

            fixedText = Regex.Replace(fixedText, @"Ab[a]*ū", "Abu");
            fixedText = Regex.Replace(fixedText, @"Mīdīnā", "Madīnā");

            fixedText = Regex.Replace(fixedText, @"([a-z])([A-Z])", "$1 $2");

            return fixedText;
        }
    }
}
