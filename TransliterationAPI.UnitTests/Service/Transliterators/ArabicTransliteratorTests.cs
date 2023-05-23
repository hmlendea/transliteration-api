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
        [TestCase("بابل", "Babil")]
        [TestCase("تشيشيناو", "Tšīšīnāū")]
        [TestCase("دمشق", "Dimašq")]
        [TestCase("روما", "Rūmā")]
        [TestCase("عَسْقَلَان", "ʿAsqalān")]
        public void GivenATextInArabicScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string arabicText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(arabicText), Is.EqualTo(expectedTransliteratedText));
    }
}
