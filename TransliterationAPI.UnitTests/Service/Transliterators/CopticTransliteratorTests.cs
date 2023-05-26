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
        [TestCase("ⲁⲛⲟⲩⲡ", "Anoup")]
        [TestCase("Ⲉⲃⲱⲧ", "Ebōt")] // Sahidic Coptic
        [TestCase("Ⲉⲗⲓⲟⲩⲓ", "Elioui")]
        [TestCase("Ⲗⲓⲟⲩⲓ", "Lioui")]
        [TestCase("ⲣⲁⲕⲟϯ", "Rakotī")]
        [TestCase("Ⲥⲟⲩⲁⲛ", "Souan")]
        [TestCase("ⲭⲁⲓⲣⲟⲛ", "Khairon")]
        [TestCase("ⲭⲁⲓⲣⲟⲛ", "Khairon")]
        [TestCase("ⲱⲛ ⲡⲉⲧ ⲫⲣⲏ", "Ōn Pet Phre")]
        [TestCase("ϣⲙⲓⲛ", "Šmin")]
        [TestCase("Ϯⲕⲉϣⲣⲱⲙⲓ", "Tikešrōmi")]
        public void GivenATextInCopticScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string copticText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(copticText, "cop"), Is.EqualTo(expectedTransliteratedText));
    }
}
