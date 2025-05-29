using System.Collections.Generic;
using System.Text.RegularExpressions;

using NuciExtensions;

using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.Service.Transliterators
{
    public class JapaneseTransliterator : ITransliterator
    {
        Dictionary<char, string> transliterationMap;

        public JapaneseTransliterator()
        {
            transliterationMap = new Dictionary<char, string>()
            {
                // Hiragana
                {'あ', "a"}, {'い', "i"}, {'う', "u"}, {'え', "e"}, {'お', "o"},
                {'か', "ka"}, {'き', "ki"}, {'く', "ku"}, {'け', "ke"}, {'こ', "ko"},
                {'さ', "sa"}, {'し', "shi"}, {'す', "su"}, {'せ', "se"}, {'そ', "so"},
                {'た', "ta"}, {'ち', "chi"}, {'つ', "tsu"}, {'て', "te"}, {'と', "to"},
                {'な', "na"}, {'に', "ni"}, {'ぬ', "nu"}, {'ね', "ne"}, {'の', "no"},
                {'は', "ha"}, {'ひ', "hi"}, {'ふ', "fu"}, {'へ', "he"}, {'ほ', "ho"},
                {'ま', "ma"}, {'み', "mi"}, {'む', "mu"}, {'め', "me"}, {'も', "mo"},
                {'や', "ya"}, {'ゆ', "yu"}, {'よ', "yo"},
                {'ら', "ra"}, {'り', "ri"}, {'る', "ru"}, {'れ', "re"}, {'ろ', "ro"},
                {'わ', "wa"}, {'を', "wo"}, {'ん', "n"},

                // Katakana
                {'ア', "a"}, {'イ', "i"}, {'ウ', "u"}, {'エ', "e"}, {'オ', "o"},
                {'カ', "ka"}, {'キ', "ki"}, {'ク', "ku"}, {'ケ', "ke"}, {'コ', "ko"},
                {'サ', "sa"}, {'シ', "shi"}, {'ス', "su"}, {'セ', "se"}, {'ソ', "so"},
                {'タ', "ta"}, {'チ', "chi"}, {'ツ', "tsu"}, {'テ', "te"}, {'ト', "to"},
                {'ナ', "na"}, {'ニ', "ni"}, {'ヌ', "nu"}, {'ネ', "ne"}, {'ノ', "no"},
                {'ハ', "ha"}, {'ヒ', "hi"}, {'フ', "fu"}, {'ヘ', "he"}, {'ホ', "ho"},
                {'マ', "ma"}, {'ミ', "mi"}, {'ム', "mu"}, {'メ', "me"}, {'モ', "mo"},
                {'ヤ', "ya"}, {'ユ', "yu"}, {'ヨ', "yo"},
                {'ラ', "ra"}, {'リ', "ri"}, {'ル', "ru"}, {'レ', "re"}, {'ロ', "ro"},
                {'ワ', "wa"}, {'ヲ', "wo"}, {'ン', "n"},
                {'ィ', "i"}, {'デ', "de"}, {'プ', "pu"}, {'グ', "gu"}, {'ズ', "zu"}, {'ジ', "ji"}, {'ガ', "ga"}, {'ベ', "be"}, {'ェ', "e"},
                {'バ', "ba"}, {'ゲ', "ge"}, {'ド', "do"}, {'ゥ', "ū"}, {'ビ', "bi"}, {'ボ', "bo"}, {'ザ', "za"}, {'ざ', "za"},
                {'・', " "}, {'ー', ""}, {'ブ', "bu"}, {'ヴ', "vu"}, {'ァ', "a"}, {'パ', "pa"}, {'ポ', "po"}, {'ピ', "pi"}, {'ォ', "o"},
                {'ギ', "gi"}, {'ペ', "pe"}, {'ゼ', "ze"},

                // Special Characters
                {'ゃ', "ya"}, {'ゅ', "yu"}, {'ょ', "yo"}, // small ya, yu, yo
                {'ャ', "ya"}, {'ュ', "yu"}, {'ョ', "yo"}, // small YA, YU, YO
                {'っ', "tsu"}, // small tsu
                {'ッ', "tsu"},  // small TSU

                // Kanji for toponyms
                { '東', "tō" },     // East, as in 東京 (Tōkyō)
                { '京', "kyō" },    // Capital, as in 京都 (Kyōto)
                { '大', "ō" },      // Big, as in 大阪 (Ōsaka)
                { '阪', "saka" },   // Hill, slope, as in 大阪 (Ōsaka)
                { '北', "hoku" },   // North, as in 北海道 (Hokkaidō)
                { '海', "kai" },    // Sea, as in 北海道 (Hokkaidō)
                { '道', "dō" },     // Road, path, as in 北海道 (Hokkaidō)
                { '名', "na" },     // Name, as in 名古屋 (Nagoya)
                { '古', "ko" },     // Old, as in 名古屋 (Nagoya)
                { '屋', "ya" },     // Shop, house, as in 名古屋 (Nagoya)
                { '神', "kami" },   // God, as in 神戸 (Kōbe)
                { '戸', "ko" },     // Door, gate, as in 神戸 (Kōbe)
                { '横', "yoko" },   // Horizontal, as in 横浜 (Yokohama)
                { '浜', "hama" },   // Beach, as in 横浜 (Yokohama)
                { '仙', "sen" },    // Hermit, wizard, as in 仙台 (Sendai)
                { '台', "dai" },    // Stand, support, as in 仙台 (Sendai)

                // Chinese
                { '九', "kyū" },
                { '寧', "nei" },
                { '歙', "shī" },
                { '玉', "gyoku" },
                { '西', "sei" },
                { '郡', "gun" },

                // Korean
                { '基', "ki" },

                // NonAsian
                { '델', "del" },

                // Kanji
                { 'ゴ', "go" },
                { 'ゾ', "zo" },
                { 'ダ', "da" },
                { '井', "i" },
                { '修', "shū" },
                { '倉', "kura" },
                { '児', "go" },
                { '入', "nyū" },
                { '半', "han" },
                { '南', "nam" },
                { '南', "nan" },
                { '取', "tori" },
                { '口', "guchi" },
                { '和', "wa" },
                { '地', "chi" },
                { '坪', "tsubo" },
                { '城', "baraki" },
                { '塩', "shio" },
                { '士', "ji" },
                { '奈', "na" },
                { '媛', "hime" },
                { '宇', "u" },
                { '宮', "miya" },
                { '富', "fu" },
                { '小', "shō" },
                { '山', "yama" },
                { '岐', "gi" },
                { '岡', "oka" },
                { '島', "shima" },
                { '崎', "saki" },
                { '川', "kawa" },
                { '州', "shū" },
                { '市', "shi" },
                { '幌', "poro" },
                { '広', "hiro" },
                { '形', "gata" },
                { '愛', "e" },
                { '新', "nii" }, // Also as: shin
                { '方', "hō" },
                { '木', "ki" },
                { '本', "moto" },
                { '札', "satsu" },
                { '松', "matsu" },
                { '根', "ne" },
                { '森', "mori" },
                { '歌', "ka" },
                { '殿', "dono" },
                { '沖', "oki" },
                { '沢', "zawa" },
                { '津', "tsu" },
                { '滋', "shi" },
                { '潟', "gata" },
                { '熊', "kuma" },
                { '町', "machi" },
                { '直', "choku" },
                { '知', "chi" },
                { '石', "ishi" },
                { '福', "fuku" },
                { '竜', "ryū" },
                { '紅', "kō" },
                { '終', "shū" },
                { '縄', "nawa" },
                { '群', "gun" },
                { '良', "ra" },
                { '茨', "i" },
                { '谷', "tani" },
                { '賀', "ka" },
                { '部', "bu" },
                { '都', "to" },
                { '野', "no" },
                { '金', "kana" },
                { '鎌', "kama" },
                { '鎮', "chin" },
                { '長', "naga" },
                { '門', "mon" },
                { '関', "kan" },
                { '阜', "fu" },
                { '院', "in" },
                { '青', "ao" },
                { '静', "shizu" },
                { '須', "su" },
                { '馬', "ma" },
                { '高', "taka" },
                { '鳥', "tori" },
                { '鹿', "ka" },

                { '県', "Todōfuken" },
            };

        }

        public string Transliterate(string text, Language language)
        {
            string transliteratedText = string.Empty;

            foreach (char character in text)
            {
                if (transliterationMap.ContainsKey(character))
                {
                    transliteratedText += transliterationMap[character];
                }
                else
                {
                    transliteratedText += character;
                }
            }

            transliteratedText = ApplyFixes(transliteratedText);

            return transliteratedText;
        }

        string ApplyFixes(string text)
        {
            string fixedText = text.ToTitleCase();

            fixedText = Regex.Replace(fixedText, "aー", "ā");
            fixedText = Regex.Replace(fixedText, "eー", "ē");
            fixedText = Regex.Replace(fixedText, "iー", "ī");
            fixedText = Regex.Replace(fixedText, "oー", "ō");
            fixedText = Regex.Replace(fixedText, "uー", "ū");

            fixedText = Regex.Replace(fixedText, "([Tt])orit", "$1ott");
            fixedText = Regex.Replace(fixedText, "Takac", "Kōc");

            fixedText = Regex.Replace(fixedText, "Oo", "Ō");
            fixedText = Regex.Replace(fixedText, "oo", "ō");

            fixedText = Regex.Replace(fixedText, "akoy", "agoy");
            fixedText = Regex.Replace(fixedText, "ika ", "iga ");
            fixedText = Regex.Replace(fixedText, "ika$", "iga");
            fixedText = Regex.Replace(fixedText, "iyama", "isan");
            fixedText = Regex.Replace(fixedText, "kuk", "kk");
            fixedText = Regex.Replace(fixedText, "tsup", "pp");
            fixedText = Regex.Replace(fixedText, "yasa", "yaza");

            return fixedText;
        }
    }
}
