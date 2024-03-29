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
                { "간", "gan" },
                { "강", "gang" },
                { "거", "geo" },
                { "게", "ge" },
                { "겐", "gen" },
                { "겔", "gel" },
                { "경", "gyeong" },
                { "고", "go" },
                { "관", "gwan" },
                { "광", "gwang" },
                { "구", "gu" },
                { "군", "gun" },
                { "귀", "gwi" },
                { "그", "geu" },
                { "기", "gi" },
                { "길", "gil" },
                { "김", "gim" },
                { "나", "na" },
                { "남", "nam" },
                { "냐", "nya" },
                { "네", "ne" },
                { "노", "no" },
                { "누", "nu" },
                { "니", "ni" },
                { "닝", "ning" },
                { "다", "da" },
                { "달", "dal" },
                { "담", "dam" },
                { "대", "dae" },
                { "데", "de" },
                { "도", "do" },
                { "동", "dong" },
                { "두", "du" },
                { "드", "deu" },
                { "디", "di" },
                { "라", "ra" },
                { "락", "nak" }, // Or rak
                { "란", "ran" },
                { "랏", "lat" },
                { "랑", "rang" },
                { "러", "reo" },
                { "레", "re" },
                { "령", "ryeong" },
                { "로", "lo" }, // or ro
                { "롱", "long" },
                { "루", "ru" },
                { "르", "re" },
                { "리", "ri" },
                { "린", "rin" },
                { "마", "ma" },
                { "만", "man" },
                { "말", "mal" },
                { "망", "mang" },
                { "먼", "meon" },
                { "메", "me" },
                { "멘", "men" },
                { "명", "myeong" },
                { "모", "mo" },
                { "목", "mok" },
                { "무", "mu" },
                { "물", "mul" },
                { "미", "mi" },
                { "바", "ba" },
                { "밧", "bat" },
                { "베", "be" },
                { "벨", "bel" },
                { "보", "bo" },
                { "볼", "bol" },
                { "부", "bu" },
                { "불", "bul" },
                { "브", "beu" },
                { "블", "beul" },
                { "비", "bi" },
                { "사", "sa" },
                { "산", "san" },
                { "샤", "sha" },
                { "서", "seo" },
                { "선", "seon" },
                { "성", "seong" },
                { "세", "se" },
                { "센", "sen" },
                { "소", "so" },
                { "속", "sok" },
                { "수", "su" },
                { "순", "sun" },
                { "슈", "shu" },
                { "스", "seu" },
                { "슬", "seul" },
                { "시", "si" },
                { "신", "sin" },
                { "싱", "sing" },
                { "아", "a" },
                { "안", "an" },
                { "야", "ya" },
                { "얀", "yan" },
                { "얌", "yam" },
                { "양", "yang" },
                { "에", "e" },
                { "엔", "en" },
                { "엘", "el" },
                { "여", "yeo" },
                { "영", "yeong" },
                { "예", "ye" },
                { "오", "o" },
                { "외", "oe" },
                { "용", "yong" },
                { "우", "u" },
                { "운", "un" },
                { "울", "ul" },
                { "웅", "ung" },
                { "원", "won" },
                { "웨", "we" },
                { "위", "wi" },
                { "은", "eun" },
                { "의", "ui" },
                { "이", "i" },
                { "익", "ik" },
                { "인", "in" },
                { "일", "il" },
                { "자", "ja" },
                { "잠", "jam" },
                { "저", "jeo" },
                { "전", "jeon" },
                { "정", "jeong" },
                { "제", "je" },
                { "젤", "jel" },
                { "주", "ju" },
                { "즈", "jeu" },
                { "지", "ji" },
                { "진", "jin" },
                { "차", "cha" },
                { "창", "chang" },
                { "천", "cheon" },
                { "청", "cheong" },
                { "체", "che" },
                { "초", "cho" },
                { "춘", "chun" },
                { "충", "chung" },
                { "츠", "cheu" },
                { "치", "chi" },
                { "칭", "ching" },
                { "카", "ka" },
                { "칸", "kan" },
                { "칼", "kal" },
                { "캉", "kaeng" },
                { "케", "ke" },
                { "코", "ko" },
                { "콘", "kon" },
                { "콧", "kot" },
                { "쿠", "ku" },
                { "쿤", "kun" },
                { "크", "keu" },
                { "클", "keul" },
                { "키", "ki" },
                { "타", "ta" },
                { "탄", "tan" },
                { "탕", "tang" },
                { "택", "taek" },
                { "토", "to" },
                { "톨", "tol" },
                { "톰", "tom" },
                { "투", "tu" },
                { "트", "teu" },
                { "티", "ti" },
                { "틴", "tin" },
                { "파", "pa" },
                { "펜", "pen" },
                { "평", "pyeong" },
                { "포", "po" },
                { "폴", "pol" },
                { "푸", "pu" },
                { "프", "peu" },
                { "플", "p" },
                { "하", "ha" },
                { "할", "hal" },
                { "항", "hang" },
                { "해", "hae" },
                { "헐", "heol" },
                { "헤", "he" },
                { "현", "hyeon" },
                { "홍", "hong" },
                { "화", "hwa" },
                { "후", "hu" },
                { "흥", "heung" },

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
