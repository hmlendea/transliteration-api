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
        [TestCase("روما", "Rūmā")]
        [TestCase("تشيشيناو", "Tšīšīnāū")]
        [TestCase("اسلو", "Āslū")]
        public void GivenATextInArabicScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string arabicText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(arabicText), Is.EqualTo(expectedTransliteratedText));
    }
}
