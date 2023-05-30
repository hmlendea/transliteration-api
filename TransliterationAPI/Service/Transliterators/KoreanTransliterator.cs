using System.Collections.Generic;
using System.Text.RegularExpressions;

using NuciExtensions;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class KoreanTransliterator : ITransliterator
    {
        Dictionary<string, string> transliterationTable;

        public KoreanTransliterator()
        {
            transliterationTable = new Dictionary<string, string>()
            {
                { "강", "gang" },
                { "경", "gyeong" },
                { "고", "go" },
                { "광", "gwang" },
                { "구", "gu" },
                { "군", "gun" },
                { "귀", "gwi" },
                { "김", "gim" },
                { "남", "nam" },
                { "대", "dae" },
                { "동", "dong" },
                { "령", "ryeong" },
                { "목", "mok" },
                { "보", "bo" },
                { "부", "bu" },
                { "산", "san" },
                { "서", "seo" },
                { "성", "seong" },
                { "속", "sok" },
                { "수", "su" },
                { "순", "sun" },
                { "안", "an" },
                { "양", "yang" },
                { "여", "yeo" },
                { "용", "yong" },
                { "울", "ul" },
                { "원", "won" },
                { "익", "ik" },
                { "인", "in" },
                { "전", "jeon" },
                { "제", "je" },
                { "주", "ju" },
                { "진", "jin" },
                { "창", "chang" },
                { "천", "cheon" },
                { "청", "cheong" },
                { "초", "cho" },
                { "춘", "chun" },
                { "택", "taek" },
                { "평", "pyeong" },
                { "포", "po" },
                { "항", "hang" },
                { "해", "hae" },
                { "홍", "hong" },
            };
        }

        public string Transliterate(string text, Language language)
        {
            string transliteratedText = text;

            foreach (string character in transliterationTable.Keys)
            {
                transliteratedText = Regex.Replace(transliteratedText, character, transliterationTable[character]);
            }

            return ApplyFixes(transliteratedText);
        }

        string ApplyFixes(string text)
        {
            string fixedText = text;

            fixedText = fixedText.ToTitleCase();

            return fixedText;
        }
    }
}
