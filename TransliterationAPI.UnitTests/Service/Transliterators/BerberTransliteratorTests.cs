using NUnit.Framework;

using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    public class BerberTransliteratorTests
    {
        private BerberTransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            transliterator = new();
        }

        [Test]
        [TestCase("ⴳⵓⵍⵎⵉⵎ ⴰⵙⵉⴼ ⵏⵓⵏ", "Gulmim Asif Nun")]
        public void GivenATextInBerberScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string arabicText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(arabicText, Language.Berber), Is.EqualTo(expectedTransliteratedText));
    }
}
