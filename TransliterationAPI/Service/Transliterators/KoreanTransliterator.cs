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
                { "가", "ga" },
                { "강", "gang" },
                { "거", "geo" },
                { "경", "gyeong" },
                { "고", "go" },
                { "광", "gwang" },
                { "구", "gu" },
                { "군", "gun" },
                { "귀", "gwi" },
                { "김", "gim" },
                { "나", "na" },
                { "남", "nam" },
                { "닝", "ning" },
                { "대", "dae" },
                { "동", "dong" },
                { "랏", "lat" },
                { "령", "ryeong" },
                { "르", "re" },
                { "리", "ri" },
                { "명", "myeong" },
                { "목", "mok" },
                { "미", "mi" },
                { "보", "bo" },
                { "부", "bu" },
                { "비", "bi" },
                { "산", "san" },
                { "서", "seo" },
                { "성", "seong" },
                { "속", "sok" },
                { "수", "su" },
                { "순", "sun" },
                { "시", "si" },
                { "안", "an" },
                { "양", "yang" },
                { "여", "yeo" },
                { "영", "yeong" },
                { "오", "o" },
                { "용", "yong" },
                { "울", "ul" },
                { "원", "won" },
                { "의", "ui" },
                { "이", "i" },
                { "익", "ik" },
                { "인", "in" },
                { "전", "jeon" },
                { "정", "jeong" },
                { "제", "je" },
                { "주", "ju" },
                { "진", "jin" },
                { "창", "chang" },
                { "천", "cheon" },
                { "청", "cheong" },
                { "초", "cho" },
                { "춘", "chun" },
                { "충", "chung" },
                { "케", "ke" },
                { "택", "taek" },
                { "평", "pyeong" },
                { "포", "po" },
                { "푸", "pu" },
                { "하", "ha" },
                { "항", "hang" },
                { "해", "hae" },
                { "현", "hyeon" },
                { "홍", "hong" },
                { "화", "hwa" },
                { "흥", "heung" },
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
