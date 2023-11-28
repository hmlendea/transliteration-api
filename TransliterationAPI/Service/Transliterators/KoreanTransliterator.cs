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
                // Hangul
                { "가", "ga" },
                { "강", "gang" },
                { "거", "geo" },
                { "게", "ge" },
                { "경", "gyeong" },
                { "고", "go" },
                { "광", "gwang" },
                { "구", "gu" },
                { "군", "gun" },
                { "귀", "gwi" },
                { "김", "gim" },
                { "나", "na" },
                { "남", "nam" },
                { "냐", "nya" },
                { "네", "ne" },
                { "니", "ni" },
                { "닝", "ning" },
                { "담", "dam" },
                { "대", "dae" },
                { "동", "dong" },
                { "드", "deu" },
                { "라", "ra" },
                { "랏", "lat" },
                { "레", "re" },
                { "령", "ryeong" },
                { "로", "lo" }, // or ro
                { "롱", "long" },
                { "루", "ru" },
                { "르", "re" },
                { "리", "ri" },
                { "린", "rin" },
                { "명", "myeong" },
                { "모", "mo" },
                { "목", "mok" },
                { "미", "mi" },
                { "바", "ba" },
                { "벨", "bel" },
                { "보", "bo" },
                { "볼", "bol" },
                { "부", "bu" },
                { "비", "bi" },
                { "사", "sa" },
                { "산", "san" },
                { "샤", "sha" },
                { "서", "seo" },
                { "성", "seong" },
                { "소", "so" },
                { "속", "sok" },
                { "수", "su" },
                { "순", "sun" },
                { "스", "seu" },
                { "시", "si" },
                { "신", "sin" },
                { "아", "a" },
                { "안", "an" },
                { "얀", "yan" },
                { "양", "yang" },
                { "에", "e" },
                { "여", "yeo" },
                { "영", "yeong" },
                { "예", "ye" },
                { "오", "o" },
                { "용", "yong" },
                { "우", "u" },
                { "운", "un" },
                { "울", "ul" },
                { "웅", "ung" },
                { "원", "won" },
                { "의", "ui" },
                { "이", "i" },
                { "익", "ik" },
                { "인", "in" },
                { "전", "jeon" },
                { "정", "jeong" },
                { "제", "je" },
                { "주", "ju" },
                { "즈", "jeu" },
                { "지", "ji" },
                { "진", "jin" },
                { "차", "cha" },
                { "창", "chang" },
                { "천", "cheon" },
                { "청", "cheong" },
                { "초", "cho" },
                { "춘", "chun" },
                { "충", "chung" },
                { "츠", "cheu" },
                { "카", "ka" },
                { "케", "ke" },
                { "콘", "kon" },
                { "쿠", "ku" },
                { "크", "keu" },
                { "클", "keul" },
                { "키", "ki" },
                { "타", "ta" },
                { "탄", "tan" },
                { "택", "taek" },
                { "토", "to" },
                { "톨", "tol" },
                { "펜", "pen" },
                { "평", "pyeong" },
                { "포", "po" },
                { "폴", "pol" },
                { "푸", "pu" },
                { "플", "p" },
                { "하", "ha" },
                { "항", "hang" },
                { "해", "hae" },
                { "헤", "he" },
                { "현", "hyeon" },
                { "홍", "hong" },
                { "화", "hwa" },

                // Hanja
                { "룽", "lung" },
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
