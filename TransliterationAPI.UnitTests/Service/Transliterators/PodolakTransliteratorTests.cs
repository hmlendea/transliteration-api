using System;

using NUnit.Framework;

using NuciLog.Core;

using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;
using TransliterationAPI.UnitTests.Service;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    [TestFixture]
    public class PodolakTransliteratorTests
    {
        FakeHttpRequestManager httpRequestManager;
        PodolakTransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            httpRequestManager = new();
            transliterator = new(httpRequestManager, new NullLogger());
        }

        [Test]
        [TestCase("ꙇзгнаница", "izgnanica")]
        [TestCase("добрость", "dobrosti")]
        [TestCase("слово", "slovo")]
        public void GivenAnOldChurchSlavonicText_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string inputText,
            string expectedTransliteratedText)
        {
            string fakeResponse = BuildFakeTransliterationResponse(expectedTransliteratedText);
            httpRequestManager.SetPostResponseToReturn(fakeResponse);

            string result = transliterator.Transliterate(inputText, Language.OldChurchSlavonic).Result;

            Assert.That(result, Is.EqualTo(expectedTransliteratedText));
        }

        [Test]
        public void GivenAnUnsupportedLanguage_WhenTransliteratingIntoLatin_ThenAnArgumentExceptionIsThrown()
        {
            httpRequestManager.SetPostResponseToReturn(string.Empty);

            Assert.ThrowsAsync<ArgumentException>(
                () => transliterator.Transliterate("текст", Language.Russian));
        }

        private static string BuildFakeTransliterationResponse(string transliteratedText)
            => $"<!DOCTYPE html>\n<html>\n<textarea id=\"ausgabe\" name=\"ausgabe\">{transliteratedText}</textarea>\n</html>";
    }
}
