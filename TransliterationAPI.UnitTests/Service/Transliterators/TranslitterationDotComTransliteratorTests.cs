using System;

using NUnit.Framework;

using NuciLog.Core;

using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;
using TransliterationAPI.UnitTests.Service;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    public class TranslitterationDotComTransliteratorTests
    {
        private FakeHttpRequestManager httpRequestManager;
        private TranslitterationDotComTransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            httpRequestManager = new();
            transliterator = new(httpRequestManager, new NullLogger());
        }

        [Test]
        [TestCase("Аҟәа", "sukhumi")]
        [TestCase("Гагра", "gagra")]
        public void GivenAnAbkhazText_WhenTransliteratingIntoLatin_ThenTheResponseIsReturnedAsIs(
            string inputText,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn($"ack:::{expectedTransliteratedText}");

            string result = transliterator.Transliterate(inputText, Language.Abkhaz).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        [TestCase("Налщик", "nalshchik")]
        [TestCase("Мэйкъуапэ", "maykuape")]
        public void GivenAnAdygheText_WhenTransliteratingIntoLatin_ThenTheResponseIsReturnedAsIs(
            string inputText,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn($"ack:::{expectedTransliteratedText}");

            string result = transliterator.Transliterate(inputText, Language.Adyghe).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        [TestCase("Երևան", "yerevan", "Yerevan")]
        [TestCase("Անի", "ani", "Ani")]
        public void GivenAnArmenianText_WhenTransliteratingIntoLatin_ThenTheTitleCasedResponseIsReturned(
            string inputText,
            string apiResponse,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn($"ack:::{apiResponse}");

            string result = transliterator.Transliterate(inputText, Language.Armenian).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        [TestCase("Башкортостан", "baškortostan")]
        [TestCase("Өфө", "ofo")]
        public void GivenABashkirText_WhenTransliteratingIntoLatin_ThenTheResponseIsReturnedAsIs(
            string inputText,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn($"ack:::{expectedTransliteratedText}");

            string result = transliterator.Transliterate(inputText, Language.Bashkir).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        [TestCase("თბილისი", "tbilisi", "Tbilisi")]
        [TestCase("ბათუმი", "batumi", "Batumi")]
        public void GivenAGeorgianText_WhenTransliteratingIntoLatin_ThenTheTitleCasedResponseIsReturned(
            string inputText,
            string apiResponse,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn($"ack:::{apiResponse}");

            string result = transliterator.Transliterate(inputText, Language.Georgian).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        public void GivenAnInuttitutTextWithUnconvertedCharacters_WhenTransliteratingIntoLatin_ThenTheCharactersAreReplacedAndTitleCaseIsApplied()
        {
            httpRequestManager.SetPostResponseToReturn("ack:::ᐄᐆnunavut");

            string result = transliterator.Transliterate("ᐄᐆᓄᓇᕗᑦ", Language.Inuttitut).Result;

            Assert.That(result, Is.EqualTo("Iununavut"));
        }

        [Test]
        [TestCase("Бишкек", "bishkek", "Bishkek")]
        [TestCase("Ош", "osh", "Osh")]
        public void GivenAKyrgyzText_WhenTransliteratingIntoLatin_ThenTheTitleCasedResponseIsReturned(
            string inputText,
            string apiResponse,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn($"ack:::{apiResponse}");

            string result = transliterator.Transliterate(inputText, Language.Kyrgyz).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        [TestCase("Осетия", "osetia")]
        [TestCase("Цхинвали", "tskhinvali")]
        public void GivenAnOsseticText_WhenTransliteratingIntoLatin_ThenTheResponseIsReturnedAsIs(
            string inputText,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn($"ack:::{expectedTransliteratedText}");

            string result = transliterator.Transliterate(inputText, Language.Ossetic).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        [TestCase("Удмуртия", "udmurtia")]
        [TestCase("Ижевск", "izhevsk")]
        public void GivenAnUdmurtText_WhenTransliteratingIntoLatin_ThenTheResponseIsReturnedAsIs(
            string inputText,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn($"ack:::{expectedTransliteratedText}");

            string result = transliterator.Transliterate(inputText, Language.Udmurt).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        [TestCase("Մասիս", "masis")]
        [TestCase("Վան", "van")]
        public void GivenAWesternArmenianText_WhenTransliteratingIntoLatin_ThenTheResponseIsReturnedAsIs(
            string inputText,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn($"ack:::{expectedTransliteratedText}");

            string result = transliterator.Transliterate(inputText, Language.WesternArmenian).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        public void GivenAnUnsupportedLanguage_WhenTransliteratingIntoLatin_ThenAnArgumentExceptionIsThrown()
        {
            httpRequestManager.SetPostResponseToReturn(string.Empty);

            Assert.ThrowsAsync<ArgumentException>(
                () => transliterator.Transliterate("مرحبا", Language.Arabic));
        }
    }
}
