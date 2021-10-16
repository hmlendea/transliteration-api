using System.Text.RegularExpressions;

using NuciExtensions;

namespace TransliterationAPI.Service.Transliterators
{
    public class MarathiTransliterator : IMarathiTransliterator
    {
        IHttpRequestManager httpRequestManager;

        public MarathiTransliterator(IHttpRequestManager httpRequestManager)
        {
            this.httpRequestManager = httpRequestManager;
        }

        public string Transliterate(string text)
        {
            string transliteratedText = text;
            
            transliteratedText = transliteratedText.Replace("क", "k");
            transliteratedText = transliteratedText.Replace("ख", "kh");
            transliteratedText = transliteratedText.Replace("ग", "ga");
            transliteratedText = transliteratedText.Replace("घ", "gh");
            transliteratedText = transliteratedText.Replace("ङ", "ng");
            transliteratedText = transliteratedText.Replace("च", "k");
            transliteratedText = transliteratedText.Replace("छ", "ch");
            transliteratedText = transliteratedText.Replace("ज", "j");
            transliteratedText = transliteratedText.Replace("झ", "jh");
            transliteratedText = transliteratedText.Replace("ञ", "ny");
            transliteratedText = transliteratedText.Replace("ट", "t");
            transliteratedText = transliteratedText.Replace("ठ", "th");
            transliteratedText = transliteratedText.Replace("ड", "d");
            transliteratedText = transliteratedText.Replace("ढ", "dh");
            transliteratedText = transliteratedText.Replace("ण", "n");
            transliteratedText = transliteratedText.Replace("त", "t");
            transliteratedText = transliteratedText.Replace("थ", "th");
            transliteratedText = transliteratedText.Replace("द", "d");
            transliteratedText = transliteratedText.Replace("ध", "dh");
            transliteratedText = transliteratedText.Replace("न", "n");
            transliteratedText = transliteratedText.Replace("प", "p");
            transliteratedText = transliteratedText.Replace("फ", "f");
            transliteratedText = transliteratedText.Replace("ब", "b");
            transliteratedText = transliteratedText.Replace("भ", "bh");
            transliteratedText = transliteratedText.Replace("म", "m");
            transliteratedText = transliteratedText.Replace("य", "y");
            transliteratedText = transliteratedText.Replace("र", "r");
            transliteratedText = transliteratedText.Replace("ल", "l");
            transliteratedText = transliteratedText.Replace("व", "v");
            transliteratedText = transliteratedText.Replace("श", "sh");
            transliteratedText = transliteratedText.Replace("ष", "s");
            transliteratedText = transliteratedText.Replace("स", "s");
            transliteratedText = transliteratedText.Replace("ह", "h");
            transliteratedText = transliteratedText.Replace("क़", "k");
            transliteratedText = transliteratedText.Replace("ख़", "kh");
            transliteratedText = transliteratedText.Replace("ग़", "g");
            transliteratedText = transliteratedText.Replace("ऩ", "n");
            transliteratedText = transliteratedText.Replace("ड़", "d");
            transliteratedText = transliteratedText.Replace("ढ", "dh");
            transliteratedText = transliteratedText.Replace("ढ़", "rh");
            transliteratedText = transliteratedText.Replace("ऱ", "r");
            transliteratedText = transliteratedText.Replace("य़", "ye");
            transliteratedText = transliteratedText.Replace("ळ", "l");
            transliteratedText = transliteratedText.Replace("ऴ", "ll");
            transliteratedText = transliteratedText.Replace("फ़", "f");
            transliteratedText = transliteratedText.Replace("ज़", "z");
            transliteratedText = transliteratedText.Replace("ऋ", "ri");
            transliteratedText = transliteratedText.Replace("ा", "aa");
            transliteratedText = transliteratedText.Replace("ि", "i");
            transliteratedText = transliteratedText.Replace("ी", "i");
            transliteratedText = transliteratedText.Replace("ु", "u");
            transliteratedText = transliteratedText.Replace("ू", "u");
            transliteratedText = transliteratedText.Replace("ॅ", "e");
            transliteratedText = transliteratedText.Replace("ॆ", "e");
            transliteratedText = transliteratedText.Replace("े", "e");
            transliteratedText = transliteratedText.Replace("ै", "ai");
            transliteratedText = transliteratedText.Replace("ॉ", "o");
            transliteratedText = transliteratedText.Replace("ॊ", "o");
            transliteratedText = transliteratedText.Replace("ो", "o");
            transliteratedText = transliteratedText.Replace("ौ", "au");
            transliteratedText = transliteratedText.Replace("अ", "a");
            transliteratedText = transliteratedText.Replace("आ", "aa");
            transliteratedText = transliteratedText.Replace("इ", "i");
            transliteratedText = transliteratedText.Replace("ई", "i");
            transliteratedText = transliteratedText.Replace("उ", "u");
            transliteratedText = transliteratedText.Replace("ऊ", "oo");
            transliteratedText = transliteratedText.Replace("ए", "e");
            transliteratedText = transliteratedText.Replace("ऐ", "ai");
            transliteratedText = transliteratedText.Replace("ऑ", "au");
            transliteratedText = transliteratedText.Replace("ओ", "o");
            transliteratedText = transliteratedText.Replace("औ", "au");
            transliteratedText = transliteratedText.Replace("ँ", "n");
            transliteratedText = transliteratedText.Replace("ं", "n");
            transliteratedText = transliteratedText.Replace("ः", "ah");
            transliteratedText = transliteratedText.Replace("़", "e");
            transliteratedText = transliteratedText.Replace("्", "");
            transliteratedText = transliteratedText.Replace("०", "0");
            transliteratedText = transliteratedText.Replace("१", "1");
            transliteratedText = transliteratedText.Replace("२", "2");
            transliteratedText = transliteratedText.Replace("३", "3");
            transliteratedText = transliteratedText.Replace("४", "4");
            transliteratedText = transliteratedText.Replace("५", "5");
            transliteratedText = transliteratedText.Replace("६", "6");
            transliteratedText = transliteratedText.Replace("७", "7");
            transliteratedText = transliteratedText.Replace("८", "8");
            transliteratedText = transliteratedText.Replace("९", "9");
            transliteratedText = transliteratedText.Replace("।", ".");
            transliteratedText = transliteratedText.Replace("ऍ", "e");
            transliteratedText = transliteratedText.Replace("ृ", "ri");
            transliteratedText = transliteratedText.Replace("ॄ", "rr");
            transliteratedText = transliteratedText.Replace("ॠ", "ṝ");
            transliteratedText = transliteratedText.Replace("ऌ", "ḷ");
            transliteratedText = transliteratedText.Replace("ॣ", "l");
            transliteratedText = transliteratedText.Replace("ॢ", "l");
            transliteratedText = transliteratedText.Replace("ॡ", "l");
            transliteratedText = transliteratedText.Replace("ॿ", "b");
            transliteratedText = transliteratedText.Replace("ॾ", "d");
            transliteratedText = transliteratedText.Replace("ॽ", "");
            transliteratedText = transliteratedText.Replace("ॼ", "j");
            transliteratedText = transliteratedText.Replace("ॻ", "g");
            transliteratedText = transliteratedText.Replace("ॐ", "om");
            transliteratedText = transliteratedText.Replace("ऽ", "'");
            transliteratedText = Regex.Replace(transliteratedText, "e.a", "a");

            return transliteratedText.ToTitleCase();
        }
    }
}
