using NUnit.Framework;

using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    public class ArabicTransliteratorTests
    {
        private ITransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            this.transliterator = new ArabicTransliterator();
        }

        [Test]
        [TestCase("أبيدوس", "Abīdūs")]
        [TestCase("أريحا", "ʾArīḥā")]
        [TestCase("ٱلنَّقَب", "an-Naqab")]
        [TestCase("إسدود", "ʾIsdūd")]
        [TestCase("إِسْرَائِيل", "ʾIsrāʾīl")]
        [TestCase("ابو قير", "Abu Qīr")]
        [TestCase("اسلو", "Āslū")]
        [TestCase("افود", "Afūd")]
        [TestCase("الأردن", "Al-ʾUrdunn")]
        [TestCase("البيرة", "al-Bīra")]
        [TestCase("الخليل", "al-Khalīl")]
        [TestCase("السامرة", "as-Sāmirah")]
        [TestCase("العمارنة", "al-ʿAmārnah")]
        [TestCase("الفرات", "al-Furāt")]
        [TestCase("الكويت", "al-Kuwayt")]
        [TestCase("المندائيّة", "al-Mandāʾiyya")]
        [TestCase("النيل", "an-Nīl")]
        [TestCase("بئر السبع", "Biʾr as-Sabʿ")]
        [TestCase("بابل", "Babil")]
        [TestCase("تشيشيناو", "Tšīšīnāū")]
        [TestCase("تنيس", "Tinnīs")]
        [TestCase("حَدِيثَةٌ", "Ḥadīthah")]
        [TestCase("حركة المقاومة الإسلامية", "Ḥarakah al-Muqāwamah al-ʾIslāmiyyah")]
        [TestCase("حسن أباد", "Ḥasan Abād")]
        [TestCase("حصوين", "Ḥaṣwayn")]
        [TestCase("حماس", "Ḥamās")]
        [TestCase("حَيْفَا", "Ḥayfā")]
        [TestCase("دمشق", "Dimašq")]
        [TestCase("رام الله", "Rām Allāh")]
        [TestCase("روما", "Rūmā")]
        [TestCase("رومان سور ديزير", "Rūmān al-Dīzīr")]
        [TestCase("سُورِيَا", "Sūriyā")]
        [TestCase("عَسْقَلَان", "ʿAsqalān")]
        [TestCase("غَزَّة", "Ġazzah")]
        [TestCase("فتح", "Fatḥ")]
        [TestCase("فِلَسْطِينَ", "Filasṭīn")]
        [TestCase("فهرج", "Fahraǧ")]
        [TestCase("قرنين", "Qarnīn")]
        [TestCase("قَصْر الرُوم", "Qaṣr ar-Rūm")]
        [TestCase("مَنْف", "Manf")]
        [TestCase("مَنْف", "Manf")]
        [TestCase("هرتسليا", "Hirtsiliyā")]
        [TestCase("هشتغرد", "Haštġard")]
        [TestCase("يوانينا", "Īānīnā")]
        public void GivenATextInArabicScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string arabicText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(arabicText, Language.Arabic), Is.EqualTo(expectedTransliteratedText));
    }
}
