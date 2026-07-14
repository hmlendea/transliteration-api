using NUnit.Framework;

using NuciLog.Core;

using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    public class GujaratiTransliteratorTests
    {
        private GujaratiTransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            transliterator = new(new NullLogger());
        }

        [Test]
        [TestCase("ભારત", "Bhārata")]
        [TestCase("ગઢડા", "Gaḍhaḍā")]
        [TestCase("ગાંધીનગર", "Gāndhīnagara")]
        [TestCase("ગુજરાત", "Gujarāta")]
        [TestCase("ખેડા", "Kheḍā")]
        [TestCase("ગ", "Ga")]
        [TestCase("ઝ", "Za")]
        [TestCase("ટ", "Ṭa")]
        [TestCase("ઢ", "Ḍha")]
        [TestCase("ક", "Ka")]
        [TestCase("ભ", "Bha")]
        [TestCase("ગંગા", "Gangā")]
        [TestCase("નદી", "Nadī")]
        [TestCase("ઉત્તર", "Uttara")]
        [TestCase("અમદાવાદ", "Amadāvāda")]
        [TestCase("ભાવનગર", "Bhāvanagara")]
        [TestCase("રાજકોટ", "Rājakoṭa")]
        [TestCase("સુરત", "Surata")]
        [TestCase("દક્ષિણ", "Dakṣhiṇa")]
        [TestCase("વડોદરા", "Vaḍodarā")]
        [TestCase("૧", "1")]
        [TestCase("૨", "2")]
        [TestCase("૩", "3")]
        public void GivenATextInGujaratiScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string gujaratiText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(gujaratiText, Language.Gujarati), Is.EqualTo(expectedTransliteratedText));
    }
}
