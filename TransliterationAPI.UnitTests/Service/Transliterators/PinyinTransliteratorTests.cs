using NUnit.Framework;
using NuciLog.Core;
using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    public class PinyinTransliteratorTests
    {
        private PinyinTransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            transliterator = new(new NullLogger());
        }

        [Test]
        [TestCase("凯奇凯梅特", "Kǎijīkǎiméitè")]
        [TestCase("凱代尼艾", "Kǎidàiníài")]
        [TestCase("基思", "Jīsāi")]
        public void GivenATextInChineseScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string chineseText,
            string expectedTransliteratedText)
            => Assert.That(
                transliterator.Transliterate(chineseText, Language.Chinese),
                Is.EqualTo(expectedTransliteratedText));
    }
}
