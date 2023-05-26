using NUnit.Framework;

using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    public class CyrillicTransliteratorTests
    {
        private ICyrillicTransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            this.transliterator = new CyrillicTransliterator();
        }

        [Test]
        [TestCase("Барнау́л", "Barnaúl")]
        [TestCase("Белгород", "Belgorod")]
        [TestCase("Владивосток", "Vladivostok")]
        [TestCase("Волгоград", "Volgograd")]
        [TestCase("Воронеж", "Voronezh")]
        [TestCase("Екатеринбург", "Yekaterinburg")]
        [TestCase("Иже́вск", "Izhévsk")]
        [TestCase("Иркутск", "Irkutsk")]
        [TestCase("Казань", "Kazan")]
        [TestCase("Кемерово", "Kemerovo")]
        [TestCase("Краснодар", "Krasnodar")]
        [TestCase("Красноярск", "Krasnoyarsk")]
        [TestCase("Махачкала", "Makhachkala")]
        [TestCase("Молотов", "Molotov")]
        [TestCase("Москва", "Moskva")]
        [TestCase("Набережные Челны", "Naberezhnye Chelny")]
        [TestCase("Нижний Новгород", "Nizhniy Novgorod")]
        [TestCase("Новосиби́рск", "Novosibírsk")]
        [TestCase("Омск", "Omsk")]
        [TestCase("Пермь", "Perm")]
        [TestCase("Петровск-Порт", "Petrovsk-Port")]
        [TestCase("Петровское", "Petrovskoye")]
        [TestCase("Роман-сюр-Изер", "Roman-na-Izere")]
        [TestCase("Ростов-на-Дону", "Rostov-na-Donu")]
        [TestCase("Самара", "Samara")]
        [TestCase("Санкт-Петербург", "Sankt-Peterburg")]
        [TestCase("Сарабури", "Saraburi")]
        [TestCase("Саратов", "Saratov")]
        [TestCase("Толья́тти", "Tolyátti")]
        [TestCase("Томск", "Tomsk")]
        [TestCase("Тюмень", "Tyumen")]
        [TestCase("Ульяновск", "Ulyanovsk")]
        [TestCase("Ульяновск", "Ulyanovsk")]
        [TestCase("Усти́нов", "Ustínov")]
        [TestCase("Уфа", "Ufa")]
        [TestCase("Хабаровск", "Khabarovsk")]
        [TestCase("Челя́бинск", "Chelyábinsk")]
        [TestCase("Экваториальная Африка", "Ekvatorialnaya Afrika")]
        [TestCase("Ягошиха", "Yagoshikha")]
        [TestCase("Ярославль", "Yaroslavl")]
        public void GivenATextInRussianCyrillicScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string cyrillicText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(cyrillicText, "ru"), Is.EqualTo(expectedTransliteratedText));
    }
}
