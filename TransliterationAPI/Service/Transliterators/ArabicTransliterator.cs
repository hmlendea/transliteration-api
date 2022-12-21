using System.Text.RegularExpressions;

using NuciExtensions;

namespace TransliterationAPI.Service.Transliterators
{
    public class ArabicTransliterator : IArabicTransliterator
    {
        public string Transliterate(string text)
        {
            string rawTransliteratedText = text;

            // marbouta
            rawTransliteratedText = rawTransliteratedText.Replace("ة", "ẗ"); // ah ou
            rawTransliteratedText = rawTransliteratedText.Replace("ا", "ā");
            rawTransliteratedText = rawTransliteratedText.Replace("آ", "\u02beā");
            rawTransliteratedText = rawTransliteratedText.Replace("ء", "\u02be");
            rawTransliteratedText = rawTransliteratedText.Replace("إ", "i");
            rawTransliteratedText = rawTransliteratedText.Replace("أ", "\u02bea"); // ou u ?
            rawTransliteratedText = rawTransliteratedText.Replace("ؤ", "u\u02be");
            rawTransliteratedText = rawTransliteratedText.Replace("ئ", "ʾi");

            rawTransliteratedText = rawTransliteratedText.Replace("ب", "b");
            rawTransliteratedText = rawTransliteratedText.Replace("ت", "t");
            rawTransliteratedText = rawTransliteratedText.Replace("ث", "ṯ");
            rawTransliteratedText = rawTransliteratedText.Replace("ج", "ǧ");
            rawTransliteratedText = rawTransliteratedText.Replace("ح", "ḥ");
            rawTransliteratedText = rawTransliteratedText.Replace("خ", "ẖ");
            rawTransliteratedText = rawTransliteratedText.Replace("د", "d");
            rawTransliteratedText = rawTransliteratedText.Replace("ذ", "ḏ");
            rawTransliteratedText = rawTransliteratedText.Replace("ر", "r");
            rawTransliteratedText = rawTransliteratedText.Replace("ز", "z");
            rawTransliteratedText = rawTransliteratedText.Replace("س", "s");
            rawTransliteratedText = rawTransliteratedText.Replace("ش", "š");
            rawTransliteratedText = rawTransliteratedText.Replace("ص", "ṣ");
            rawTransliteratedText = rawTransliteratedText.Replace("ض", "ḍ");
            rawTransliteratedText = rawTransliteratedText.Replace("ط", "ṭ");
            rawTransliteratedText = rawTransliteratedText.Replace("ظ", "ẓ");
            rawTransliteratedText = rawTransliteratedText.Replace("ع", "ʿ");
            rawTransliteratedText = rawTransliteratedText.Replace("غ", "ġ");
            rawTransliteratedText = rawTransliteratedText.Replace("ف", "f");
            rawTransliteratedText = rawTransliteratedText.Replace("ق", "q");
            rawTransliteratedText = rawTransliteratedText.Replace("ك", "k");
            rawTransliteratedText = rawTransliteratedText.Replace("ل", "l");
            rawTransliteratedText = rawTransliteratedText.Replace("م", "m");
            rawTransliteratedText = rawTransliteratedText.Replace("ن", "n");
            rawTransliteratedText = rawTransliteratedText.Replace("ه", "h");
            rawTransliteratedText = rawTransliteratedText.Replace("و", "ū");
            rawTransliteratedText = rawTransliteratedText.Replace("ي", "ī");

            // cas du a initial
            rawTransliteratedText = rawTransliteratedText.Replace(" \u02be", " ");
            // pb rawTransliteratedText = Regex.Replace(rawTransliteratedText, "-\u02be", "-");
            rawTransliteratedText = rawTransliteratedText.Replace(" ā", " a");
            rawTransliteratedText = rawTransliteratedText.Replace(" al", " al-");
            rawTransliteratedText = rawTransliteratedText.Replace("\n\u02be", "\n");
            rawTransliteratedText = rawTransliteratedText.Replace("\nā", "\na");
            rawTransliteratedText = rawTransliteratedText.Replace("\nal", "\nal-");

            // voyelles
            rawTransliteratedText = rawTransliteratedText.Replace("َ", "a"); //1614
            rawTransliteratedText = rawTransliteratedText.Replace("ُ", "u"); //1615
            rawTransliteratedText = rawTransliteratedText.Replace("ِ", "i"); //1616
            rawTransliteratedText = rawTransliteratedText.Replace("ّ", "w"); //1617

            rawTransliteratedText = rawTransliteratedText.Replace("ىْ", "y");
            rawTransliteratedText = rawTransliteratedText.Replace("ى", "i");

            // cas du w et y
            rawTransliteratedText = rawTransliteratedText.Replace("aā", "ā");
            rawTransliteratedText = rawTransliteratedText.Replace("aa", "ā");
            rawTransliteratedText = rawTransliteratedText.Replace("ūa", "wa");
            rawTransliteratedText = rawTransliteratedText.Replace("ūā", "wā");
            rawTransliteratedText = rawTransliteratedText.Replace("ūu", "wu");
            rawTransliteratedText = rawTransliteratedText.Replace("ūū", "wu");
            rawTransliteratedText = rawTransliteratedText.Replace("ūw", "ww");
            rawTransliteratedText = rawTransliteratedText.Replace("ūi", "wi");
            rawTransliteratedText = rawTransliteratedText.Replace("ūī", "wy");
            rawTransliteratedText = rawTransliteratedText.Replace("uu", "ū");
            rawTransliteratedText = rawTransliteratedText.Replace("uū", "ū");
            rawTransliteratedText = rawTransliteratedText.Replace("īī", "yi");
            rawTransliteratedText = rawTransliteratedText.Replace("ii", "ī");
            rawTransliteratedText = rawTransliteratedText.Replace("iī", "ī");

            // double conso
            rawTransliteratedText = rawTransliteratedText.Replace("rً", "rr");
            rawTransliteratedText = rawTransliteratedText.Replace("lً", "ll");
            rawTransliteratedText = rawTransliteratedText.Replace("bً", "bb");
            rawTransliteratedText = rawTransliteratedText.Replace("tً", "tt");
            rawTransliteratedText = rawTransliteratedText.Replace("ṭṭً", "ṭṭ");
            rawTransliteratedText = rawTransliteratedText.Replace("mً", "mm");
            rawTransliteratedText = rawTransliteratedText.Replace("nً", "nn");
            rawTransliteratedText = rawTransliteratedText.Replace("dً", "dd");
            rawTransliteratedText = rawTransliteratedText.Replace("ḍً", "ḍḍ");
            rawTransliteratedText = rawTransliteratedText.Replace("sً", "ss");
            rawTransliteratedText = rawTransliteratedText.Replace("tً", "tt");
            rawTransliteratedText = rawTransliteratedText.Replace("ṣṣً", "ṣṣ");

            rawTransliteratedText = rawTransliteratedText.Replace("،", ",");
            rawTransliteratedText = rawTransliteratedText.Replace("؛", ";");
            rawTransliteratedText = rawTransliteratedText.Replace("؟", "?");
            rawTransliteratedText = rawTransliteratedText.Replace("ـ", ".");

            rawTransliteratedText = rawTransliteratedText.Replace("٠", "0");
            rawTransliteratedText = rawTransliteratedText.Replace("١", "1");
            rawTransliteratedText = rawTransliteratedText.Replace("٢", "2");
            rawTransliteratedText = rawTransliteratedText.Replace("٣", "3");
            rawTransliteratedText = rawTransliteratedText.Replace("٤", "4");
            rawTransliteratedText = rawTransliteratedText.Replace("۴", "4");
            rawTransliteratedText = rawTransliteratedText.Replace("٥", "5");
            rawTransliteratedText = rawTransliteratedText.Replace("۵", "5");
            rawTransliteratedText = rawTransliteratedText.Replace("٦", "6");
            rawTransliteratedText = rawTransliteratedText.Replace("۶", "6");
            rawTransliteratedText = rawTransliteratedText.Replace("٧", "7");
            rawTransliteratedText = rawTransliteratedText.Replace("٨", "8");
            rawTransliteratedText = rawTransliteratedText.Replace("٩", "9");

            // maghreb
            rawTransliteratedText = rawTransliteratedText.Replace("گ", "g");
            rawTransliteratedText = rawTransliteratedText.Replace("ڤ", "g");

            return ApplyFixes(rawTransliteratedText);
        }

        string ApplyFixes(string text)
        {
            string fixedText = text;

            fixedText = fixedText.ToTitleCase();

            fixedText = fixedText.Replace("Ftḥ", "Fatḥ");
            fixedText = fixedText.Replace("ūrfū", "ūrifū");
            fixedText = fixedText.Replace(" [Dd]ī[l]* ", " āl-");

            fixedText = Regex.Replace(fixedText, "^ǧ", "Ǧ");
            fixedText = Regex.Replace(fixedText, "^ẖ", "H̱");
            fixedText = Regex.Replace(fixedText, "([ -])ǧ", "$1Ǧ");
            fixedText = Regex.Replace(fixedText, "([ -])ẖ", "$1H̱");

            fixedText = Regex.Replace(fixedText, "[ -][Dd][eīo][ls]*[ -]", " al-");
            fixedText = Regex.Replace(fixedText, "^Al([ -])*", "al-$1");
            fixedText = Regex.Replace(fixedText, "^Āl([ -])*", "āl-$1");
            fixedText = Regex.Replace(fixedText, "ʾ(\\p{L})", m => "ʾ" + m.Groups[1].Value.ToUpperInvariant());
            fixedText = Regex.Replace(fixedText, "āšr$", "āšir");
            fixedText = Regex.Replace(fixedText, "lnd$", "land");
            fixedText = Regex.Replace(fixedText, "rzn", "rzin");

            fixedText = Regex.Replace(fixedText, "rzn", "rzin");
            fixedText = Regex.Replace(fixedText, "([Ǧǧ])b", "$1ib");
            fixedText = Regex.Replace(fixedText, "([Nn])m", "$1im");
            fixedText = Regex.Replace(fixedText, "([Qq])f", "$1if");
            fixedText = fixedText.Replace("ẖr", "ẖar");

            fixedText = fixedText.Replace("Mīdīnā", "Madīnā");

            return fixedText;
        }
    }
}
