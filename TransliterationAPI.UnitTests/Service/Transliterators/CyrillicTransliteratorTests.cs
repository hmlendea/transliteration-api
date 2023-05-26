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
        [TestCase("Айтос", "Aytos")]
        [TestCase("Асеновград", "Asenovgrad")]
        [TestCase("Берковица", "Berkovitsa")]
        [TestCase("Благоевград", "Blagoevgrad")]
        [TestCase("Ботевград", "Botevgrad")]
        [TestCase("Бургас", "Burgas")]
        [TestCase("Варна", "Varna")]
        [TestCase("Велико Търново", "Veliko Tărnovo")]
        [TestCase("Велинград", "Velingrad")]
        [TestCase("Видин", "Vidin")]
        [TestCase("Враца", "Vratsa")]
        [TestCase("Габрово", "Gabrovo")]
        [TestCase("Горна Оряховица", "Gorna Oryahovitsa")]
        [TestCase("Гоце Делчев", "Gotse Delchev")]
        [TestCase("Димитровград", "Dimitrovgrad")]
        [TestCase("Добрич", "Dobrich")]
        [TestCase("Дупница", "Dupnitsa")]
        [TestCase("Казанлък", "Kazanlăk")]
        [TestCase("Карлово", "Karlovo")]
        [TestCase("Карнобат", "Karnobat")]
        [TestCase("Козлодуй", "Kozloduy")]
        [TestCase("Кърджали", "Kărdzhali")]
        [TestCase("Кюстендил", "Kyustendil")]
        [TestCase("Ловеч", "Lovech")]
        [TestCase("Лом", "Lom")]
        [TestCase("Монтана", "Montana")]
        [TestCase("Неврокоп", "Nevrokop")]
        [TestCase("Нова Загора", "Nova Zagora")]
        [TestCase("Нови Искър", "Novi Iskăr")]
        [TestCase("Пазарджик", "Pazardzhik")]
        [TestCase("Панагюрище", "Panagyurishte")]
        [TestCase("Перник", "Pernik")]
        [TestCase("Петрич", "Petrich")]
        [TestCase("Пещера", "Peshtera")]
        [TestCase("Плевен", "Pleven")]
        [TestCase("Пловдив", "Plovdiv")]
        [TestCase("Поморие", "Pomorie")]
        [TestCase("Попово", "Popovo")]
        [TestCase("Първомай", "Părvomay")]
        [TestCase("Радомир", "Radomir")]
        [TestCase("Разград", "Razgrad")]
        [TestCase("Раковски", "Rakovski")]
        [TestCase("Република България", "Republika Bălgaria")]
        [TestCase("Русе", "Ruse")]
        [TestCase("Самоков", "Samokov")]
        [TestCase("Самунджиево", "Samundzhievo")]
        [TestCase("Сандански", "Sandanski")]
        [TestCase("Свети Врач", "Sveti Vrach")]
        [TestCase("Свиленград", "Svilengrad")]
        [TestCase("Свищов", "Svishtov")]
        [TestCase("Севлиево", "Sevlievo")]
        [TestCase("Силистра", "Silistra")]
        [TestCase("Сливен", "Sliven")]
        [TestCase("Смолян", "Smolyan")]
        [TestCase("София", "Sofia")]
        [TestCase("Стара Загора", "Stara Zagora")]
        [TestCase("Троян", "Troyan")]
        [TestCase("Търговище", "Tărgovishte")]
        [TestCase("Харманли", "Harmanli")]
        [TestCase("Хасково", "Haskovo")]
        [TestCase("Червен бряг", "Cherven bryag")]
        [TestCase("Чирпан", "Chirpan")]
        [TestCase("Шумен", "Shumen")]
        [TestCase("Ямбол", "Yambol")]
        public void GivenATextInBulgarianCyrillicScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string bulgarianText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(bulgarianText, "bg"), Is.EqualTo(expectedTransliteratedText));

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
            string russianText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(russianText, "ru"), Is.EqualTo(expectedTransliteratedText));
    }
}
