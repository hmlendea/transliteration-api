using System;

using NUnit.Framework;

using NuciLog.Core;

using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;
using TransliterationAPI.UnitTests.Service;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    [TestFixture]
    public class UshuaiaTransliteratorTests
    {
        FakeHttpRequestManager httpRequestManager;
        UshuaiaTransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            httpRequestManager = new();
            httpRequestManager.SetCookiesToReturn("translit=testsession;other=value");
            transliterator = new(httpRequestManager, new NullLogger());
        }

        [Test]
        [TestCase("ঢাকা", "dhaka", "Dhaka")]
        [TestCase("চট্টগ্রাম", "chattagram", "Chattagram")]
        public void GivenABengaliText_WhenTransliteratingIntoLatin_ThenTheTitleCasedResponseIsReturned(
            string inputText,
            string apiResponse,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn(apiResponse);

            string result = transliterator.Transliterate(inputText, Language.Bengali).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        [TestCase("श्रीनगर", "srinagar", "Sarinagar")]
        [TestCase("मुम्बई", "mumbai", "Mumbai")]
        public void GivenAHindiText_WhenTransliteratingIntoLatin_ThenThePostProcessedAndTitleCasedResponseIsReturned(
            string inputText,
            string apiResponse,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn(apiResponse);

            string result = transliterator.Transliterate(inputText, Language.Hindi).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        [TestCase("ಬೆಂಗಳೂರು", "bengaluru", "Bengaluru")]
        [TestCase("ಮೈಸೂರು", "mysuru", "Mysuru")]
        public void GivenAKannadaText_WhenTransliteratingIntoLatin_ThenTheTitleCasedResponseIsReturned(
            string inputText,
            string apiResponse,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn(apiResponse);

            string result = transliterator.Transliterate(inputText, Language.Kannada).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        [TestCase("കൊച്ചി", "kochi", "Kochi")]
        [TestCase("തിരുവനന്തപുരം", "thiruvananthapuram", "Thiruvananthapuram")]
        public void GivenAMalayalamText_WhenTransliteratingIntoLatin_ThenTheTitleCasedResponseIsReturned(
            string inputText,
            string apiResponse,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn(apiResponse);

            string result = transliterator.Transliterate(inputText, Language.Malayalam).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        [TestCase("Улаанбаатар", "ulaanbaatar")]
        [TestCase("Дархан", "darkhan")]
        public void GivenAMongolText_WhenTransliteratingIntoLatin_ThenTheResponseIsReturnedAsIs(
            string inputText,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn(expectedTransliteratedText);

            string result = transliterator.Transliterate(inputText, Language.Mongol).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        [TestCase("वाराणसी", "varanasi", "Varanasi")]
        [TestCase("प्रयागराज", "prayagraj", "Prayagraj")]
        public void GivenASanskritText_WhenTransliteratingIntoLatin_ThenTheTitleCasedResponseIsReturned(
            string inputText,
            string apiResponse,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn(apiResponse);

            string result = transliterator.Transliterate(inputText, Language.Sanskrit).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        [TestCase("කොළඹ", "colombo", "Colombo")]
        [TestCase("ගාල්ල", "galle", "Galle")]
        public void GivenASinhalaText_WhenTransliteratingIntoLatin_ThenTheTitleCasedResponseIsReturned(
            string inputText,
            string apiResponse,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn(apiResponse);

            string result = transliterator.Transliterate(inputText, Language.Sinhala).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        [TestCase("சென்னை", "chennai", "Chennai")]
        [TestCase("மதுரை", "madurai", "Madurai")]
        public void GivenATamilText_WhenTransliteratingIntoLatin_ThenTheTitleCasedResponseIsReturned(
            string inputText,
            string apiResponse,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn(apiResponse);

            string result = transliterator.Transliterate(inputText, Language.Tamil).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        [TestCase("హైదరాబాద్", "hyderabad", "Hyderabad")]
        [TestCase("విజయవాడ", "vijayawada", "Vijayawada")]
        public void GivenATeluguText_WhenTransliteratingIntoLatin_ThenTheTitleCasedResponseIsReturned(
            string inputText,
            string apiResponse,
            string expectedTransliteratedText)
        {
            httpRequestManager.SetPostResponseToReturn(apiResponse);

            string result = transliterator.Transliterate(inputText, Language.Telugu).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        public void GivenAnUnsupportedLanguage_WhenTransliteratingIntoLatin_ThenAnArgumentExceptionIsThrown()
        {
            httpRequestManager.SetPostResponseToReturn(string.Empty);

            Assert.ThrowsAsync<ArgumentException>(
                () => transliterator.Transliterate("מרוצה", Language.Hebrew));
        }
    }
}
