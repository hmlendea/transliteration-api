using NUnit.Framework;

using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    public class CopticTransliteratorTests
    {
        private CopticTransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            transliterator = new();
        }

        [Test]
        [TestCase("ⲁⲗⲉⲝⲁⲛⲇⲣⲓⲁ", "Alexandria")]
        [TestCase("ⲁⲛⲟⲩⲡ", "Anoup")]
        [TestCase("ⲁⲡⲁⲕⲩⲣⲓ", "Apakuri")]
        [TestCase("Ⲉⲃⲱⲧ", "Ebōt")] // Sahidic Coptic
        [TestCase("Ⲉⲗⲓⲟⲩⲓ", "Elioui")]
        [TestCase("Ⲏⲥⲉ", "Ēse")]
        [TestCase("Ⲑⲱⲟⲩⲧ", "Thōout")]
        [TestCase("Ⲗⲓⲟⲩⲓ", "Lioui")]
        [TestCase("ⲙⲉⲙϥⲓ", "Memfi")]
        [TestCase("ⲟⲩⲥⲓⲣⲉ", "Ousire")]
        [TestCase("ⲡⲣ̅ⲣⲟ", "Pǝrro")]
        [TestCase("ⲣⲁⲕⲟϯ", "Rakotī")]
        [TestCase("Ⲥⲓⲛⲁ", "Sina")]
        [TestCase("Ⲥⲟⲩⲁⲛ", "Souan")]
        [TestCase("ⲧϥⲏⲛⲉ", "Tfēne")]
        [TestCase("ⲫⲓⲁⲣⲟ", "Phiaro")]
        [TestCase("ⲫⲓⲟⲙ ⲛ̀ϣⲁⲣⲓ", "Phiom Ǹšari")]
        [TestCase("ⲫⲓⲟⲙ ⲛ̀ϩⲁϩ", "Phiom Ǹhah")]
        [TestCase("ⲭⲁⲓⲣⲟⲛ", "Khairon")]
        [TestCase("ⲱⲛ ⲡⲉⲧ ⲫⲣⲏ", "Ōn Pet Phrē")]
        [TestCase("ϣⲙⲓⲛ", "Šmin")]
        [TestCase("Ϣⲙⲟⲩⲛ", "Šmoun")]
        [TestCase("ϩⲁⲣⲡⲟⲕⲣⲁⲧⲏⲥ", "Harpokratēs")]
        [TestCase("Ϯⲕⲉϣⲣⲱⲙⲓ", "Tikešrōmi")]
        public void GivenATextInCopticScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string copticText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(copticText, Language.Coptic), Is.EqualTo(expectedTransliteratedText));
    }
}
