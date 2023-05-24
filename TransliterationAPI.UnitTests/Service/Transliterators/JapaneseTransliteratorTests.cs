using NUnit.Framework;
using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    public class JapaneseTransliteratorTests
    {
        private IJapaneseTransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            this.transliterator = new JapaneseTransliterator();
        }

        [Test]
        [TestCase("京都", "Kyōto")]
        [TestCase("仙台", "Sendai")]
        [TestCase("北海道", "Hokkaidō")]
        [TestCase("名古屋", "Nagoya")]
        [TestCase("和歌山", "Wakayama")]
        [TestCase("大阪", "Ōsaka")]
        [TestCase("奈良", "Nara")]
        [TestCase("宮崎", "Miyazaki")]
        [TestCase("富士山", "Fujisan")]
        [TestCase("山口", "Yamaguchi")]
        [TestCase("山形", "Yamagata")]
        [TestCase("岐阜", "Gifu")]
        [TestCase("岡山", "Okayama")]
        [TestCase("島根", "Shimane")]
        [TestCase("広島", "Hiroshima")]
        [TestCase("愛媛", "Ehime")]
        [TestCase("新潟", "Niigata")]
        [TestCase("札幌", "Sapporo")]
        [TestCase("東京", "Tōkyō")]
        [TestCase("横浜", "Yokohama")]
        [TestCase("横須賀", "Yokosuka")]
        [TestCase("沖縄", "Okinawa")]
        [TestCase("滋賀", "Shiga")]
        [TestCase("熊本", "Kumamoto")]
        [TestCase("石川", "Ishikawa")]
        [TestCase("福井", "Fukui")]
        [TestCase("福岡", "Fukuoka")]
        [TestCase("福島", "Fukushima")]
        [TestCase("群馬", "Gunma")]
        [TestCase("茨城", "Ibaraki")]
        [TestCase("金沢", "Kanazawa")]
        [TestCase("鎌倉", "Kamakura")]
        [TestCase("長崎", "Nagasaki")]
        [TestCase("長野", "Nagano")]
        [TestCase("青森", "Aomori")]
        [TestCase("静岡", "Shizuoka")]
        [TestCase("高松", "Takamatsu")]
        [TestCase("高知", "Kōchi")]
        [TestCase("鳥取", "Tottori")]
        [TestCase("鹿児島", "Kagoshima")]
        public void GivenATextInJapaneseScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string japaneseText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(japaneseText), Is.EqualTo(expectedTransliteratedText));
    }
}
