using NUnit.Framework;

using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    public class HebrewTransliteratorTests
    {
        private IHebrewTransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            this.transliterator = new HebrewTransliterator();
        }

        [Test]
        [TestCase("אַשְׁדּוֹד", "ʾAšdōḏ")]
        [TestCase("אַשְׁקְלוֹן", "ʾAšqəlōn")]
        [TestCase("בְּאֵר שֶׁבַע", "Bəʾēr Ševaʿ")]
        [TestCase("בָּבֶל", "Bāvel")]
        [TestCase("גִּבְעָתַיִים", "Givatayim")]
        [TestCase("דַּמֶּשֶׂק", "Dammeśeq")]
        [TestCase("הֶרְצְלִיָּה", "Herzliya")]
        [TestCase("חֶבְרוֹן", "Ḥevrōn")]
        [TestCase("חֵיפָה", "Ḥēyfā")]
        [TestCase("ירושלים", "Yerushaláyim")]
        [TestCase("יריחו", "Yərīḥō")]
        [TestCase("מִצְרַיִם", "Miṣráyim")]
        [TestCase("נַהֲרִיָּה", "Nahariya")]
        [TestCase("נציבין", "Netzivin")]
        [TestCase("נָצְרַת", "Nāṣəraṯ")]
        [TestCase("נְתַנְיָה", "Netanya")]
        [TestCase("עֵילָם", "ʿĒlām")]
        [TestCase("רבת בני עמון", "Rabat Bnei ʿAmmon")]
        [TestCase("רְחוֹבוֹת", "Reḥōvōt")]
        [TestCase("רַמְלָה", "Ramlā")]
        [TestCase("תל-אביב", "Tel-Aviv")]
        public void GivenATextInHebrewScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string hebrewText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(hebrewText), Is.EqualTo(expectedTransliteratedText));
    }
}
