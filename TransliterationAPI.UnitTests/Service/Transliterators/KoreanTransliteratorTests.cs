using NUnit.Framework;

using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    public class KoreanTransliteratorTests
    {
        private ITransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            this.transliterator = new KoreanTransliterator();
        }

        [Test]
        [TestCase("거제", "Geoje")]
        [TestCase("경산", "Gyeongsan")]
        [TestCase("경주", "Gyeongju")]
        [TestCase("고성", "Goseong")]
        [TestCase("고양", "Goyang")]
        [TestCase("광명", "Gwangmyeong")]
        [TestCase("광양", "Gwangyang")]
        [TestCase("광주", "Gwangju")]
        [TestCase("구리", "Guri")]
        [TestCase("군산", "Gunsan")]
        [TestCase("김포", "Gimpo")]
        [TestCase("김해", "Gimhae")]
        [TestCase("남원", "Namwon")]
        [TestCase("대구", "Daegu")]
        [TestCase("대전", "Daejeon")]
        [TestCase("동해", "Donghae")]
        [TestCase("목포", "Mokpo")]
        [TestCase("보령", "Boryeong")]
        [TestCase("부산", "Busan")]
        [TestCase("부천", "Bucheon")]
        [TestCase("서귀포", "Seogwipo")]
        [TestCase("서울", "Seoul")]
        [TestCase("서천", "Seocheon")]
        [TestCase("성남", "Seongnam")]
        [TestCase("속초", "Sokcho")]
        [TestCase("수원", "Suwon")]
        [TestCase("순천", "Suncheon")]
        [TestCase("시흥", "Siheung")]
        [TestCase("안산", "Ansan")]
        [TestCase("안성", "Anseong")]
        [TestCase("안양", "Anyang")]
        [TestCase("양양", "Yangyang")]
        [TestCase("양주", "Yangju")]
        [TestCase("양평", "Yangpyeong")]
        [TestCase("여수", "Yeosu")]
        [TestCase("여천", "Yeocheon")]
        [TestCase("영주", "Yeongju")]
        [TestCase("오산", "Osan")]
        [TestCase("용인", "Yongin")]
        [TestCase("울산", "Ulsan")]
        [TestCase("원주", "Wonju")]
        [TestCase("의정부", "Uijeongbu")]
        [TestCase("이천", "Icheon")]
        [TestCase("익산", "Iksan")]
        [TestCase("인천", "Incheon")]
        [TestCase("전주", "Jeonju")]
        [TestCase("제주", "Jeju")]
        [TestCase("진주", "Jinju")]
        [TestCase("창원", "Changwon")]
        [TestCase("청주", "Cheongju")]
        [TestCase("춘천", "Chuncheon")]
        [TestCase("충주", "Chungju")]
        [TestCase("평택", "Pyeongtaek")]
        [TestCase("포항", "Pohang")]
        [TestCase("하남", "Hanam")]
        [TestCase("홍천", "Hongcheon")]
        [TestCase("화성", "Hwaseong")]
        public void GivenATextInKoreanScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string koreanText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(koreanText, Language.Korean), Is.EqualTo(expectedTransliteratedText));
    }
}
