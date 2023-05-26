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
        [TestCase("ⲁⲗⲉⲝⲁⲛⲇⲣⲓⲁ", "Alexandria")]
        [TestCase("ⲣⲁⲕⲟϯ", "Rakotī")]
        [TestCase("ⲱⲛ ⲡⲉⲧ ⲫⲣⲏ", "On Pet Phre")]
        [TestCase("ϣⲙⲓⲛ", "Shmin")]
        [TestCase("Ⲥⲟⲩⲁⲛ", "Souan")]
        public void GivenATextInCopticScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string copticText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(copticText, "cop"), Is.EqualTo(expectedTransliteratedText));
    }
}
