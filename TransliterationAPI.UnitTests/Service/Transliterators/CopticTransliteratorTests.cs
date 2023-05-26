using NUnit.Framework;

using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    public class CopticTransliteratorTests
    {
        private ICopticTransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            this.transliterator = new CopticTransliterator();
        }

        [Test]
        [TestCase("ⲱⲛ ⲡⲉⲧ ⲫⲣⲏ", "on pet phre")]
        [TestCase("ϣⲙⲓⲛ", "shmin")]
        public void GivenATextInCopticScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string copticText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(copticText, "cop"), Is.EqualTo(expectedTransliteratedText));
    }
}
