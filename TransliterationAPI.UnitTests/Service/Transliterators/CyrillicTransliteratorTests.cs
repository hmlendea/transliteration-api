using System.Collections.Generic;
using NUnit.Framework;

using TransliterationAPI.Service.Entities;
using TransliterationAPI.Service.Transliterators;

namespace TransliterationAPI.UnitTests.Service.Transliterators
{
    public class CyrillicTransliteratorTests
    {
        private ITransliterator transliterator;

        [SetUp]
        public void SetUp()
        {
            this.transliterator = new CyrillicTransliterator();
        }

        [Test]
        [TestCase("Асіповічы", "Asipovičy")]
        [TestCase("Астравец", "Astraviec")]
        [TestCase("Ашмяны", "Ašmiany")]
        [TestCase("Бабруйск", "Babrujsk")]
        [TestCase("Баранавічы", "Baranavičy")]
        [TestCase("Барань", "Barań")]
        [TestCase("Барысаў", "Barysaŭ")]
        [TestCase("Белаазёрск", "Bielaaziorsk")]
        [TestCase("Беразіно", "Bierazino")]
        [TestCase("Браслаў", "Braslaŭ")]
        [TestCase("Брэст", "Brest")]
        [TestCase("Буда-Кашалёва", "Buda-Kašaliova")]
        [TestCase("Быхаў", "Bychaŭ")]
        [TestCase("Бялынічы", "Bialyničy")]
        [TestCase("Бяроза", "Biaroza")]
        [TestCase("Бярозаўка", "Biarozaŭka")]
        [TestCase("Валожын", "Valožyn")]
        [TestCase("Ваўкавыск", "Vaŭkavysk")]
        [TestCase("Верхнядзвінск", "Vierchniadzvinsk")]
        [TestCase("Ветка", "Vietka")]
        [TestCase("Вілейка", "Viliejka")]
        [TestCase("Віцебск", "Viciebsk")]
        [TestCase("Высокае", "Vysokaje")]
        [TestCase("Ганцавічы", "Hancavičy")]
        [TestCase("Гарадок", "Haradok")]
        [TestCase("Глыбокае", "Hlybokaje")]
        [TestCase("Гомель", "Homieĺ")]
        [TestCase("Горкі", "Horki")]
        [TestCase("Гродна", "Hrodna")]
        [TestCase("Давыд-Гарадок", "Davyd-Haradok")]
        [TestCase("Дзяржынск", "Dziaržynsk")]
        [TestCase("Дзятлава", "Dziatlava")]
        [TestCase("Добруш", "Dobruš")]
        [TestCase("Докшыцы", "Dokšycy")]
        [TestCase("Драгічын", "Drahičyn")]
        [TestCase("Дуброўна", "Dubroŭna")]
        [TestCase("Ельск", "Jeĺsk")]
        [TestCase("Жабінка", "Žabinka")]
        [TestCase("Жлобін", "Žlobin")]
        [TestCase("Жодзіна", "Žodzina")]
        [TestCase("Жыткавічы", "Žytkavičy")]
        [TestCase("Заслаўе", "Zaslaŭje")]
        [TestCase("Іванава", "Ivanava")]
        [TestCase("Івацэвічы", "Ivacevičy")]
        [TestCase("Іўе", "Iŭje")]
        [TestCase("Калінкавічы", "Kalinkavičy")]
        [TestCase("Камянец", "Kamianiec")]
        [TestCase("Капыль", "Kapyĺ")]
        [TestCase("Касцюковічы", "Kasciukovičy")]
        [TestCase("Кіраўск", "Kiraŭsk")]
        [TestCase("Клецк", "Klieck")]
        [TestCase("Клімавічы", "Klimavičy")]
        [TestCase("Клічаў", "Kličaŭ")]
        [TestCase("Кобрын", "Kobryn")]
        [TestCase("Крупкі", "Krupki")]
        [TestCase("Крычаў", "Kryčaŭ")]
        [TestCase("Лагойск", "Lahojsk")]
        [TestCase("Лепель", "Liepieĺ")]
        [TestCase("Ліда", "Lida")]
        [TestCase("Лунінец", "Luniniec")]
        [TestCase("Любань", "Liubań")]
        [TestCase("Ляхавічы", "Liachavičy")]
        [TestCase("Магілёў", "Mahilioŭ")]
        [TestCase("Мазыр", "Mazyr")]
        [TestCase("Маладзечна", "Maladziečna")]
        [TestCase("Маларыта", "Malaryta")]
        [TestCase("Мар'іна Горка", "Marjina Horka")]
        [TestCase("Масты", "Masty")]
        [TestCase("Міёры", "Mijory")]
        [TestCase("Мікашэвічы", "Mikaševičy")]
        [TestCase("Мінск", "Minsk")]
        [TestCase("Мсціслаў", "Mscislaŭ")]
        [TestCase("Мядзел", "Miadziel")]
        [TestCase("Навагрудак", "Navahrudak")]
        [TestCase("Наваполацк", "Navapolack")]
        [TestCase("Нароўля", "Naroŭlia")]
        [TestCase("Новалукомль", "Novalukomĺ")]
        [TestCase("Нясвіж", "Niasviž")]
        [TestCase("Орша", "Orša")]
        [TestCase("Паставы", "Pastavy")]
        [TestCase("Петрыкаў", "Pietrykaŭ")]
        [TestCase("Пінск", "Pinsk")]
        [TestCase("Полацк", "Polack")]
        [TestCase("Пружаны", "Pružany")]
        [TestCase("Рагачоў", "Rahačoŭ")]
        [TestCase("Рэчыца", "Rečyca")]
        [TestCase("Салігорск", "Salihorsk")]
        [TestCase("Светлагорск", "Svietlahorsk")]
        [TestCase("Свіслач", "Svislač")]
        [TestCase("Скідзель", "Skidzieĺ")]
        [TestCase("Слаўгарад", "Slaŭharad")]
        [TestCase("Слонім", "Slonim")]
        [TestCase("Слуцк", "Sluck")]
        [TestCase("Смалявічы", "Smaliavičy")]
        [TestCase("Смаргонь", "Smarhoń")]
        [TestCase("Старыя Дарогі", "Staryja Darohi")]
        [TestCase("Столін", "Stolin")]
        [TestCase("Стоўбцы", "Stoŭbcy")]
        [TestCase("Сянно", "Sianno")]
        [TestCase("Талачын", "Talačyn")]
        [TestCase("Узда", "Uzda")]
        [TestCase("Фаніпаль", "Fanipaĺ")]
        [TestCase("Хойнікі", "Chojniki")]
        [TestCase("Чавусы", "Čavusy")]
        [TestCase("Чачэрск", "Čačersk")]
        [TestCase("Чашнікі", "Čašniki")]
        [TestCase("Чэрвень", "Červień")]
        [TestCase("Чэрыкаў", "Čerykaŭ")]
        [TestCase("Шаркаўшчына", "Šarkaŭščyna")]
        [TestCase("Шклоў", "Škloŭ")]
        [TestCase("Шчучын", "Ščučyn")]
        public void GivvenATextInBelarussianCyrillicScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string belarussianText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(belarussianText, Language.Belarussian), Is.EqualTo(expectedTransliteratedText));

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
            => Assert.That(transliterator.Transliterate(bulgarianText, Language.Bulgarian), Is.EqualTo(expectedTransliteratedText));

        [Test]
        [TestCase("Çĕмĕрле", "Śĕmĕrle")]
        [TestCase("Çӗрпӳ", "Śĕrpü")]
        [TestCase("Вăйлă", "Văjlă")]
        [TestCase("Вăрнар", "Vărnar")]
        [TestCase("Виҫҫĕ", "Viśśĕ")]
        [TestCase("Ёмкăҫ", "Jomkăś")]
        [TestCase("ёршик", "joršik")]
        [TestCase("Етӗрне", "Jetĕrne")]
        [TestCase("Жираф", "Žiraf")]
        [TestCase("Жук", "Žuk")]
        [TestCase("Жюри", "Žjuri")]
        [TestCase("Канаш", "Kanaš")]
        [TestCase("Кӳкеç", "Kükeś")]
        [TestCase("Куславкка", "Kuslavkka")]
        [TestCase("Ҫăкăр", "Śăkăr")]
        [TestCase("Ҫĕмĕрт", "Śĕmĕrt")]
        [TestCase("Ҫӗнӗ Шупашкар", "Śĕnĕ Šupaškar")]
        [TestCase("Ӳкерчĕк", "Ükerčĕk")]
        [TestCase("Улатӑр", "Ulatăr")]
        [TestCase("Ӳпĕте", "Üpĕte")]
        [TestCase("Чĕкĕнтĕр", "Čĕkĕntĕr")]
        [TestCase("Чӑваш Ен", "Čăvaš Jen")]
        [TestCase("Чӑвашла", "Čăvašla")]
        [TestCase("Чул", "Čul")]
        [TestCase("Чӳрече", "Čüreče")]
        [TestCase("Шăл", "Šăl")]
        [TestCase("Шапа", "Šapa")]
        [TestCase("Шупашкар", "Šupaškar")]
        [TestCase("Шурă", "Šură")]
        [TestCase("Энерги", "Energi")]
        [TestCase("Эпĕр", "Epĕr")]
        [TestCase("Этем", "Etem")]
        [TestCase("Юман", "Juman")]
        [TestCase("Юр", "Jur")]
        [TestCase("Ялав", "Jalav")]
        [TestCase("Япала", "Japala")]
        public void GivenATextInChuvashCyrillicScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string chuvashText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(chuvashText, Language.Chuvash), Is.EqualTo(expectedTransliteratedText));

        [Test]
        [TestCase("Ақтау", "Aqtau")]
        [TestCase("Ақтөбе", "Aqtöbe")]
        [TestCase("Алматы", "Almaty")]
        [TestCase("Арқалық", "Arqalyq")]
        [TestCase("Астана", "Astana")]
        [TestCase("Атырау", "Atyrau")]
        [TestCase("Байқоңыр", "Baiqoñyr")]
        [TestCase("Балқаш", "Balqaş")]
        [TestCase("Екібастұз", "Ekıbastūz")]
        [TestCase("Жаңаөзен", "Jañaözen")]
        [TestCase("Жезқазған", "Jezqazğan")]
        [TestCase("Кентау", "Kentau")]
        [TestCase("Көкшетау", "Kökşetau")]
        [TestCase("Қарағанды", "Qarağandy")]
        [TestCase("Қостанай", "Qostanai")]
        [TestCase("Қызылорда", "Qyzylorda")]
        [TestCase("Орал", "Oral")]
        [TestCase("Өскемен", "Öskemen")]
        [TestCase("Павлодар", "Pavlodar")]
        [TestCase("Петропавл", "Petropavl")]
        [TestCase("Риддер", "Ridder")]
        [TestCase("Саран", "Saran")]
        [TestCase("Сәтбаев", "Sätbaev")]
        [TestCase("Семей", "Semei")]
        [TestCase("Степногорск", "Stepnogorsk")]
        [TestCase("Талдықорған", "Taldyqorğan")]
        [TestCase("Тараз", "Taraz")]
        [TestCase("Теміртау", "Temırtau")]
        [TestCase("Түркістан", "Türkıstan")]
        [TestCase("Шахтинск", "Şahtinsk")]
        [TestCase("Шымкент", "Şymkent")]
        [TestCase("Щучинск", "Ştşutşinsk")]
        [TestCase("Барлық адамдар тумысынан азат және қадір-қасиеті мен құқықтары тең болып дүниеге келеді. Адамдарға ақыл-парасат, ар-ождан берілген, сондықтан олар бір-бірімен туыстық, бауырмалдық қарым-қатынас жасаулары тиіс.", "Barlyq adamdar tumysynan azat jäne qadır-qasietı men qūqyqtary teñ bolyp düniege keledı. Adamdarğa aqyl-parasat, ar-ojdan berılgen, sondyqtan olar bır-bırımen tuystyq, bauyrmaldyq qarym-qatynas jasaulary tiıs.")]
        public void GivenATextInKazakhCyrillicScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string kazakhText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(kazakhText, Language.Kazakh), Is.EqualTo(expectedTransliteratedText));

        [Test]
        [TestCase("Берово", "Berovo")]
        [TestCase("Битола", "Bitola")]
        [TestCase("Богданци", "Bogdanci")]
        [TestCase("Валандово", "Valandovo")]
        [TestCase("Велес", "Veles")]
        [TestCase("Виница", "Vinica")]
        [TestCase("Гевгелија", "Gevgelija")]
        [TestCase("Гостивар", "Gostivar")]
        [TestCase("Дебар", "Debar")]
        [TestCase("Делчево", "Delčevo")]
        [TestCase("Демир Капија", "Demir Kapija")]
        [TestCase("Демир Хисар", "Demir Hisar")]
        [TestCase("Дукља", "Duklja")]
        [TestCase("Кавадарци", "Kavadarci")]
        [TestCase("Кичево", "Kičevo")]
        [TestCase("Коњух", "Konjuh")]
        [TestCase("Кочани", "Kočani")]
        [TestCase("Коџинци", "Kodžinci")]
        [TestCase("Кратово", "Kratovo")]
        [TestCase("Крива Паланка", "Kriva Palanka")]
        [TestCase("Крушево", "Kruševo")]
        [TestCase("Куманово", "Kumanovo")]
        [TestCase("Летење", "Letenje")]
        [TestCase("Македонска Каменица", "Makedonska Kamenica")]
        [TestCase("Македонски Брод", "Makedonski Brod")]
        [TestCase("Неготино", "Negotino")]
        [TestCase("Охрид", "Ohrid")]
        [TestCase("Пехчево", "Pehčevo")]
        [TestCase("Прилеп", "Prilep")]
        [TestCase("Пробиштип", "Probištip")]
        [TestCase("Радовиш", "Radoviš")]
        [TestCase("Ресен", "Resen")]
        [TestCase("Свети Николе", "Sveti Nikole")]
        [TestCase("Скопје", "Skopje")]
        [TestCase("Струга", "Struga")]
        [TestCase("Струмица", "Strumica")]
        [TestCase("Тетово", "Tetovo")]
        [TestCase("Џума", "Džuma")]
        [TestCase("Штип", "Štip")]
        [TestCase("Оче наш, кој си на небесата, да се свети името Твое, да дојде царството Твое, да биде волјата Твоја, како на небото, така и на земјата; лебот наш насушен дај ни го денес и прости ни ги долговите наши како и ние што им ги проштеваме на нашите должници; И не нѐ воведувај во искушение, но избави нѐ од лукавиот. Зашто Твое е Царството и Силата и Славата, во вечни векови. Амин!", "Oče naš, koj si na nebesata, da se sveti imeto Tvoe, da dojde carstvoto Tvoe, da bide voljata Tvoja, kako na neboto, taka i na zemjata; lebot naš nasušen daj ni go denes i prosti ni gi dolgovite naši kako i nie što im gi proštevame na našite dolžnici; I ne nè voveduvaj vo iskušenie, no izbavi nè od lukaviot. Zašto Tvoe e Carstvoto i Silata i Slavata, vo večni vekovi. Amin!")]
        public void GivenATextInMacedonianCyrillicScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string macedonianText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(macedonianText, Language.MacedonianSlavic), Is.EqualTo(expectedTransliteratedText));

        [Test]
        [TestCase("Ала́тырь", "Alátyr")]
        [TestCase("Барнау́л", "Barnaúl")]
        [TestCase("Белгород", "Belgorod")]
        [TestCase("Владивосток", "Vladivostok")]
        [TestCase("Волгоград", "Volgograd")]
        [TestCase("Воронеж", "Voronezh")]
        [TestCase("Вурнары", "Vurnary")]
        [TestCase("Екатеринбург", "Yekaterinburg")]
        [TestCase("Иже́вск", "Izhévsk")]
        [TestCase("Иркутск", "Irkutsk")]
        [TestCase("Казань", "Kazan")]
        [TestCase("Кана́ш", "Kanásh")]
        [TestCase("Кемерово", "Kemerovo")]
        [TestCase("Козло́вка", "Kozlóvka")]
        [TestCase("Краснодар", "Krasnodar")]
        [TestCase("Красноярск", "Krasnoyarsk")]
        [TestCase("Кугеси", "Kugesi")]
        [TestCase("Махачкала", "Makhachkala")]
        [TestCase("Молотов", "Molotov")]
        [TestCase("Москва", "Moskva")]
        [TestCase("Набережные Челны", "Naberezhnye Chelny")]
        [TestCase("Нижний Новгород", "Nizhniy Novgorod")]
        [TestCase("Новосиби́рск", "Novosibírsk")]
        [TestCase("Новочебокса́рск", "Novocheboksársk")]
        [TestCase("Новошареево", "Novoshareyevo")]
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
        [TestCase("Усти́нов", "Ustínov")]
        [TestCase("Уфа", "Ufa")]
        [TestCase("Хабаровск", "Khabarovsk")]
        [TestCase("Циви́льск", "Tsivílsk")]
        [TestCase("Чебокса́ры", "Cheboksáry")]
        [TestCase("Челя́бинск", "Chelyábinsk")]
        [TestCase("Чувашия", "Chuvashiya")]
        [TestCase("Шу́мерля", "Shúmerlya")]
        [TestCase("Экваториальная Африка", "Ekvatorialnaya Afrika")]
        [TestCase("Ягошиха", "Yagoshikha")]
        [TestCase("Я́дрин", "Yádrin")]
        [TestCase("Ярославль", "Yaroslavl")]
        public void GivenATextInRussianCyrillicScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string russianText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(russianText, Language.Russian), Is.EqualTo(expectedTransliteratedText));

        [Test]
        [TestCase("Алексинац", "Aleksinac")]
        [TestCase("Апатин", "Apatin")]
        [TestCase("Аранђеловац", "Aranđelovac")]
        [TestCase("Бачка Паланка", "Bačka Palanka")]
        [TestCase("Београд", "Beograd")]
        [TestCase("Бечеј", "Bečej")]
        [TestCase("Бор", "Bor")]
        [TestCase("Ваљево", "Valjevo")]
        [TestCase("Велика Плана", "Velika Plana")]
        [TestCase("Власотинце", "Vlasotince")]
        [TestCase("Врање", "Vranje")]
        [TestCase("Врбас", "Vrbas")]
        [TestCase("Вршац", "Vršac")]
        [TestCase("Горњи Милановац", "Gornji Milanovac")]
        [TestCase("Зајечар", "Zaječar")]
        [TestCase("Зрењанин", "Zrenjanin")]
        [TestCase("Инђија", "Inđija")]
        [TestCase("Јагодина", "Jagodina")]
        [TestCase("Кикинда", "Kikinda")]
        [TestCase("Књажевац", "Knjaževac")]
        [TestCase("Крагујевац", "Kragujevac")]
        [TestCase("Краљево", "Kraljevo")]
        [TestCase("Крушевац", "Kruševac")]
        [TestCase("Кула", "Kula")]
        [TestCase("Лесковац", "Leskovac")]
        [TestCase("Лозница", "Loznica")]
        [TestCase("Неготин", "Negotin")]
        [TestCase("Ниш", "Niš")]
        [TestCase("Нови Пазар", "Novi Pazar")]
        [TestCase("Нови Сад", "Novi Sad")]
        [TestCase("Панчево", "Pančevo")]
        [TestCase("Параћин", "Paraćin")]
        [TestCase("Пирот", "Pirot")]
        [TestCase("Пожаревац", "Požarevac")]
        [TestCase("Прокупље", "Prokuplje")]
        [TestCase("Рума", "Ruma")]
        [TestCase("Сента", "Senta")]
        [TestCase("Смедерево", "Smederevo")]
        [TestCase("Смедеревска Паланка", "Smederevska Palanka")]
        [TestCase("Сомбор", "Sombor")]
        [TestCase("Сремска Митровица", "Sremska Mitrovica")]
        [TestCase("Стара Пазова", "Stara Pazova")]
        [TestCase("Суботица", "Subotica")]
        [TestCase("Темерин", "Temerin")]
        [TestCase("Ћуприја", "Ćuprija")]
        [TestCase("Ужице", "Užice")]
        [TestCase("Чачак", "Čačak")]
        public void GivenATextInSerbianCyrillicScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string serbianText,
            string expectedTransliteratedText)
        {
            foreach (Language language in new List<Language> { Language.Serbian, Language.SerbianCyrillic, Language.SerboCroatian })
            {
                Assert.That(transliterator.Transliterate(serbianText, language), Is.EqualTo(expectedTransliteratedText));
            }
        }

        [Test]
        [TestCase("Київ", "Kyiv")]
        [TestCase("Харків", "Kharkiv")]
        [TestCase("Одеса", "Odesa")]
        [TestCase("Дніпро", "Dnipro")]
        [TestCase("Донецьк", "Donetsk")]
        [TestCase("Запоріжжя", "Zaporizhzhia")]
        [TestCase("Львів", "Lviv")]
        [TestCase("Кривий Ріг", "Kryvyi Rih")]
        [TestCase("Миколаїв", "Mykolaiv")]
        [TestCase("Севастополь", "Sevastopol")]
        [TestCase("Маріуполь", "Mariupol")]
        [TestCase("Луганськ", "Luhansk")]
        [TestCase("Вінниця", "Vinnytsia")]
        [TestCase("Макіївка", "Makiivka")]
        [TestCase("Симферополь", "Simferopol")]
        [TestCase("Чернігів", "Chernihiv")]
        [TestCase("Херсон", "Kherson")]
        [TestCase("Полтава", "Poltava")]
        [TestCase("Хмельницький", "Khmelnytskyi")]
        [TestCase("Черкаси", "Cherkasy")]
        [TestCase("Чернівці", "Chernivtsi")]
        [TestCase("Житомир", "Zhytomyr")]
        [TestCase("Суми", "Sumy")]
        [TestCase("Рівне", "Rivne")]
        [TestCase("Горлівка", "Horlivka")]
        [TestCase("Івано-Франківськ", "Ivano-Frankivsk")]
        [TestCase("Кам'янське", "Kamianske")]
        [TestCase("Тернопіль", "Ternopil")]
        [TestCase("Кропивницький", "Kropyvnytskyi")]
        [TestCase("Кременчук", "Kremenchuk")]
        [TestCase("Луцьк", "Lutsk")]
        [TestCase("Біла Церква", "Bila Tserkva")]
        [TestCase("Керч", "Kerch")]
        [TestCase("Меліто́поль", "Melitópol")]
        [TestCase("Краматорськ", "Kramatorsk")]
        [TestCase("Ужгород", "Uzhhorod")]
        [TestCase("Бровари", "Brovary")]
        [TestCase("Євпаторія", "Yevpatoria")]
        [TestCase("Бердянськ", "Berdiansk")]
        [TestCase("Нікополь", "Nikopol")]
        [TestCase("Слов'янськ", "Sloviansk")]
        [TestCase("Алчевськ", "Alchevsk")]
        [TestCase("Павлоград", "Pavlohrad")]
        [TestCase("Сєвєродонецьк", "Sievierodonetsk")]
        [TestCase("Кам'янець-Подільський", "Kamianets-Podilskyi")]
        [TestCase("Лисичанськ", "Lysychansk")]
        [TestCase("Мукачево", "Mukachevo")]
        [TestCase("Конотоп", "Konotop")]
        [TestCase("Умань", "Uman")]
        [TestCase("Хрустальний", "Khrustalnyi")]
        [TestCase("Ялта", "Yalta")]
        public void GivenATextInUkrainianCyrillicScript_WhenTransliteratingIntoLatin_ThenTheCorrectTextIsReturned(
            string ukrainianText,
            string expectedTransliteratedText)
            => Assert.That(transliterator.Transliterate(ukrainianText, Language.Ukrainian), Is.EqualTo(expectedTransliteratedText));
    }
}
