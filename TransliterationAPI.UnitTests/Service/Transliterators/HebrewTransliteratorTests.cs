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
        [TestCase("יריחו", "Yərīḥō")]
        [TestCase("עֵילָם", "ʿĒlām")]
        [TestCase("מִצְרַיִם", "Miṣráyim")]
        [TestCase("אַשְׁדּוֹד", "ʾAšdōḏ")]
        [TestCase("אַשְׁקְלוֹן", "ʾAšqəlōn")]
        [TestCase("בְּאֵר שֶׁבַע", "Bəʾēr Ševaʿ")]
        [TestCase("בָּבֶל", "Bāvel")]
        [TestCase("דַּמֶּשֶׂק", "Dammeśeq")]
        [TestCase("ירושלים", "Yerushaláyim")]
        [TestCase("נציבין", "Netzivin")]
        [TestCase("רבת בני עמון", "Rabat Bnei ʿAmmon")]
        [TestCase("תל-אביב", "Tel-Aviv")]
        public void GivenATextInHebrewScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string hebrewText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(hebrewText), Is.EqualTo(expectedTransliteratedText));
    }
}
