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
        [TestCase("إسدود", "ʾIsdūd")]
        [TestCase("اسلو", "Āslū")]
        [TestCase("الرملة", "āl-Ramlaḧ")] // Or ar-Ramleh
        [TestCase("بابل", "Babil")]
        [TestCase("تشيشيناو", "Tšīšīnāū")]
        [TestCase("حَيْفَا", "Ḥayfā")]
        [TestCase("دمشق", "Dimašq")]
        [TestCase("روما", "Rūmā")]
        [TestCase("عَسْقَلَان", "ʿAsqalān")]
        [TestCase("فِلَسْطِينَ", "Filasṭīn")]
        [TestCase("هرتسليا", "Hirtsiliyā")]
        public void GivenATextInArabicScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string arabicText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(arabicText), Is.EqualTo(expectedTransliteratedText));
    }
}
