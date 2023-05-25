using NUnit.Framework;

using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    public class ArabicTransliteratorTests
    {
        private IArabicTransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            this.transliterator = new ArabicTransliterator();
        }

        [Test]
        [TestCase("أريحا", "ʾArīḥā")]
        [TestCase("ٱلنَّقَب", "an-Naqab")]
        [TestCase("إسدود", "ʾIsdūd")]
        [TestCase("إِسْرَائِيل", "ʾIsrāʾīl")]
        [TestCase("اسلو", "Āslū")]
        [TestCase("الأردن", "Al-ʾUrdunn")]
        [TestCase("الخليل", "al-Khalīl")]
        [TestCase("السامرة", "as-Sāmirah")]
        [TestCase("العمارنة", "al-ʿAmārnah")]
        [TestCase("الفرات", "al-Furāt")]
        [TestCase("الكويت", "al-Kuwayt")]
        [TestCase("المندائيّة", "al-Mandāʾiyya")]
        [TestCase("بئر السبع", "Biʾr as-Sabʿ")]
        [TestCase("بابل", "Babil")]
        [TestCase("تشيشيناو", "Tšīšīnāū")]
        [TestCase("حركة المقاومة الإسلامية", "Ḥarakah al-Muqāwamah al-ʾIslāmiyyah")]
        [TestCase("حماس", "Ḥamās")]
        [TestCase("حَيْفَا", "Ḥayfā")]
        [TestCase("دمشق", "Dimašq")]
        [TestCase("رام الله", "Rām Allāh")]
        [TestCase("روما", "Rūmā")]
        [TestCase("سُورِيَا", "Sūriyā")]
        [TestCase("عَسْقَلَان", "ʿAsqalān")]
        [TestCase("غَزَّة", "Ġazzah")]
        [TestCase("فتح", "Fatḥ")]
        [TestCase("فِلَسْطِينَ", "Filasṭīn")]
        [TestCase("فهرج", "Fahraǧ")]
        [TestCase("مَنْف", "Manf")]
        [TestCase("هرتسليا", "Hirtsiliyā")]
        [TestCase("يوانينا", "Īānīnā")]
        public void GivenATextInArabicScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string arabicText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(arabicText, "ar"), Is.EqualTo(expectedTransliteratedText));
    }
}
