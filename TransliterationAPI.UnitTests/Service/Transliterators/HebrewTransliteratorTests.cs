using NUnit.Framework;

using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    public class HebrewTransliteratorTests
    {
        private HebrewTransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            transliterator = new();
        }

        [Test]
        [TestCase("אַשְׁדּוֹד", "ʾAšdōḏ")]
        [TestCase("אַשְׁקְלוֹן", "ʾAšqəlōn")]
        [TestCase("בְּאֵר שֶׁבַע", "Bəʾēr Ševaʿ")]
        [TestCase("בָּבֶל", "Bāvel")]
        [TestCase("גִּבְעָתַיִים", "Givatayim")]
        [TestCase("דַּמֶּשֶׂק", "Dammeśeq")]
        [TestCase("הַיָּם הָאָדְוֹם", "Hayyām Hāʾāḏōm")]
        [TestCase("הֶרְצְלִיָּה", "Herzliya")]
        [TestCase("חֶבְרוֹן", "Ḥevrōn")]
        [TestCase("חֵיפָה", "Ḥēyfā")]
        [TestCase("יהודה", "Yəhūda")]
        [TestCase("יַם-סוּף", "Yam-sūf")]
        [TestCase("ירושלים", "Yerushaláyim")]
        [TestCase("יריחו", "Yərīḥō")]
        [TestCase("יִשְׂרָאֵל", "Yīsrāʾēl")]
        [TestCase("מִצְרַיִם", "Miṣráyim")]
        [TestCase("נַהֲרִיָּה", "Nahariya")]
        [TestCase("נציבין", "Netzivin")]
        [TestCase("נָצְרַת", "Nāṣəraṯ")]
        [TestCase("נְתַנְיָה", "Netanya")]
        [TestCase("עֵילָם", "ʿĒlām")]
        [TestCase("פלשתינה", "Palestīna")]
        [TestCase("פַּרְעֹה", "Parʿō")]
        [TestCase("פְּרָת", "Pǝrāṯ")]
        [TestCase("רבת בני עמון", "Rabat Bnei ʿAmmon")]
        [TestCase("רְחוֹבוֹת", "Reḥōvōt")]
        [TestCase("רַמְלָה", "Ramlā")]
        [TestCase("שֹׁמְרוֹן", "Šōmrōn")]
        [TestCase("תל-אביב", "Tel-Aviv")]
        public void GivenATextInHebrewScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string hebrewText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(hebrewText, Language.Hebrew), Is.EqualTo(expectedTransliteratedText));
    }
}
