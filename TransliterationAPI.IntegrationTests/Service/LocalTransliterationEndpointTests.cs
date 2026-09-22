using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using NUnit.Framework;

using TransliterationAPI.IntegrationTests.Infrastructure;

namespace TransliterationAPI.IntegrationTests.Service
{
    [TestFixture]
    public sealed class LocalTransliterationEndpointTests
    {
        private HttpClient client = null!;
        private TransliterationApiWebApplicationFactory factory = null!;

        [SetUp]
        public void SetUp()
        {
            factory = new TransliterationApiWebApplicationFactory();
            client = factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            client.Dispose();
            factory.Dispose();
        }

        [Test]
        [TestCaseSource(nameof(LocalTransliterationScenarios))]
        public async Task GivenVerifiedLocalScriptText_WhenRequestingTransliteration_ThenTheExactLatinTextIsReturned(
            string languageCode,
            string sourceText,
            string expectedText)
        {
            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(sourceText, languageCode));
            using JsonDocument responseBody = await ReadResponseBody(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseBody.RootElement.GetProperty("success").GetBoolean());
                Assert.That(responseBody.RootElement.GetProperty("text").GetString(), Is.EqualTo(expectedText));
                Assert.That(responseBody.RootElement.GetProperty("hmac").GetString(), Is.Not.Null.And.Not.Empty);
                Assert.That(factory.HttpRequestManager.TotalPostInvocationCount, Is.Zero);
                Assert.That(ReadCacheEntryCount(), Is.EqualTo(1));
            });
        }

        private int ReadCacheEntryCount()
        {
            using JsonDocument cache = JsonDocument.Parse(File.ReadAllText(factory.CacheStoreLocation));

            return cache.RootElement.GetArrayLength();
        }

        private static IEnumerable<TestCaseData> LocalTransliterationScenarios()
            => ArabicScenarios()
                .Concat(BerberScenarios())
                .Concat(CopticScenarios())
                .Concat(CyrillicScenarios())
                .Concat(GreekScenarios())
                .Concat(GujaratiScenarios())
                .Concat(HebrewScenarios())
                .Concat(JapaneseScenarios())
                .Concat(KoreanScenarios())
                .Concat(MarathiScenarios())
                .Concat(PinyinScenarios());

        private static IEnumerable<TestCaseData> ArabicScenarios()
        {
            yield return BuildTestCase("ar", "أبيدوس", "Abīdūs");
            yield return BuildTestCase("ar", "أريحا", "ʾArīḥā");
            yield return BuildTestCase("ar", "ٱلنَّقَب", "an-Naqab");
            yield return BuildTestCase("ar", "إِسْرَائِيل", "ʾIsrāʾīl");
            yield return BuildTestCase("ar", "الأردن", "Al-ʾUrdunn");
            yield return BuildTestCase("ar", "الخليل", "al-Khalīl");
            yield return BuildTestCase("ar", "السامرة", "as-Sāmirah");
            yield return BuildTestCase("ar", "حركة المقاومة الإسلامية", "Ḥarakah al-Muqāwamah al-ʾIslāmiyyah");
            yield return BuildTestCase("ar", "قَصْر الرُوم", "Qaṣr ar-Rūm");
            yield return BuildTestCase("ar", "ڭلميم-وادي نون", "Gulmīm-Wādī Nūn");
            yield return BuildTestCase("arz", "أبيدوس", "Abīdūs");
            yield return BuildTestCase("arz", "الأردن", "Al-ʾUrdunn");
            yield return BuildTestCase("ary", "أبيدوس", "Abīdūs");
            yield return BuildTestCase("ary", "ڭلميم-وادي نون", "Gulmīm-Wādī Nūn");
        }

        private static IEnumerable<TestCaseData> BerberScenarios()
        {
            yield return BuildTestCase("ber", "ⴳⵓⵍⵎⵉⵎ ⴰⵙⵉⴼ ⵏⵓⵏ", "Gulmim Asif Nun");
        }

        private static IEnumerable<TestCaseData> CopticScenarios()
        {
            yield return BuildTestCase("cop", "ⲁⲗⲉⲝⲁⲛⲇⲣⲓⲁ", "Alexandria");
            yield return BuildTestCase("cop", "ⲁⲛⲟⲩⲡ", "Anoup");
            yield return BuildTestCase("cop", "Ⲉⲃⲱⲧ", "Ebōt");
            yield return BuildTestCase("cop", "Ⲏⲥⲉ", "Ēse");
            yield return BuildTestCase("cop", "ⲡⲣ̅ⲣⲟ", "Pǝrro");
            yield return BuildTestCase("cop", "ⲫⲓⲟⲙ ⲛ̀ϣⲁⲣⲓ", "Phiom Ǹšari");
            yield return BuildTestCase("cop", "Ϣⲙⲟⲩⲛ", "Šmoun");
            yield return BuildTestCase("cop", "Ϯⲕⲉϣⲣⲱⲙⲓ", "Tikešrōmi");
        }

        private static IEnumerable<TestCaseData> CyrillicScenarios()
        {
            yield return BuildTestCase("ab", "Аҟәа", "Ak̄a̋a");
            yield return BuildTestCase("ab", "Гагра", "Gagra");
            yield return BuildTestCase("ab", "Очамчыра", "Očamčyra");
            yield return BuildTestCase("be", "Асіповічы", "Asipovičy");
            yield return BuildTestCase("be", "Брэст", "Brest");
            yield return BuildTestCase("be", "Мінск", "Minsk");
            yield return BuildTestCase("bg", "Айтос", "Aytos");
            yield return BuildTestCase("bg", "Варна", "Varna");
            yield return BuildTestCase("bg", "София", "Sofia");
            yield return BuildTestCase("cv", "Çĕмĕрле", "Śĕmĕrle");
            yield return BuildTestCase("cv", "Вăйлă", "Văjlă");
            yield return BuildTestCase("cv", "Чӑвашла", "Čăvašla");
            yield return BuildTestCase("kk", "Ақтау", "Aqtau");
            yield return BuildTestCase("kk", "Алматы", "Almaty");
            yield return BuildTestCase("kk", "Астана", "Astana");
            yield return BuildTestCase("mk", "Берово", "Berovo");
            yield return BuildTestCase("mk", "Битола", "Bitola");
            yield return BuildTestCase("mk", "Скопје", "Skopje");
            yield return BuildTestCase("ru", "Москва", "Moskva");
            yield return BuildTestCase("ru", "Санкт-Петербург", "Sankt-Peterburg");
            yield return BuildTestCase("ru", "Экваториальная Африка", "Ekvatorialnaya Afrika");

            string[] serbianLanguageCodes = ["sr", "sr-ec", "sh"];

            foreach (string languageCode in serbianLanguageCodes)
            {
                yield return BuildTestCase(languageCode, "Београд", "Beograd");
                yield return BuildTestCase(languageCode, "Нови Сад", "Novi Sad");
                yield return BuildTestCase(languageCode, "Ниш", "Niš");
            }

            string[] tajikLanguageCodes = ["tg", "tg-cyrl"];

            foreach (string languageCode in tajikLanguageCodes)
            {
                yield return BuildTestCase(languageCode, "Балх", "Balx");
                yield return BuildTestCase(languageCode, "Тоҷикистон", "Toçikiston");
            }

            string[] tatarLanguageCodes = ["tt", "tt-cyrl"];

            foreach (string languageCode in tatarLanguageCodes)
            {
                yield return BuildTestCase(languageCode, "Казань", "Qazan");
                yield return BuildTestCase(languageCode, "Әлмәт", "Älmät");
                yield return BuildTestCase(languageCode, "Яшел Үзән", "Yäşel Üzän");
            }

            yield return BuildTestCase("uk", "Київ", "Kyiv");
            yield return BuildTestCase("uk", "Львів", "Lviv");
            yield return BuildTestCase("uk", "Запоріжжя", "Zaporizhzhia");
        }

        private static IEnumerable<TestCaseData> GreekScenarios()
        {
            yield return BuildTestCase("grc", "Kαταoνία", "Kataonía");
            yield return BuildTestCase("grc", "Ἀθῆναι", "Athênai");
            yield return BuildTestCase("grc", "Ἀκρόπολις τῶν Ἀθηνῶν", "Akrópolis tôn Athēnôn");
            yield return BuildTestCase("grc", "Βυζάντιον", "Byzántion");
            yield return BuildTestCase("grc", "Ῥώμη", "Rhṓmē");
            yield return BuildTestCase("grc", "Χαλκιδική", "Khalkidikḗ");
            yield return BuildTestCase("grc-dor", "Ᾰθῆναι", "Athênai");
            yield return BuildTestCase("grc-dor", "Αττική", "Attiká");
            yield return BuildTestCase("grc-dor", "Δᾶλος", "Dâlos");
            yield return BuildTestCase("grc-dor", "Ἐτεόκρητη", "Eteókrēta");
            yield return BuildTestCase("grc-dor", "Κερύνεια", "Karýneia");
            yield return BuildTestCase("grc-dor", "Χαλκιδική", "Khalkidiká");
            yield return BuildTestCase("el", "Αγία Παρασκευή", "Agía Paraskevī́");
            yield return BuildTestCase("el", "Αθήνα", "Athī́na");
            yield return BuildTestCase("el", "Εύοσμος", "Ev́osmos");
            yield return BuildTestCase("el", "Θεσσαλονίκη", "Thessaloníkī");
            yield return BuildTestCase("el", "Πειραιάς", "Peiraiás");
            yield return BuildTestCase("el", "Χαλάνδρι", "Chalándri");
        }

        private static IEnumerable<TestCaseData> GujaratiScenarios()
        {
            yield return BuildTestCase("gy", "ભારત", "Bhārata");
            yield return BuildTestCase("gy", "ગાંધીનગર", "Gāndhīnagara");
            yield return BuildTestCase("gy", "ગુજરાત", "Gujarāta");
            yield return BuildTestCase("gy", "ગંગા", "Gangā");
            yield return BuildTestCase("gy", "અમદાવાદ", "Amadāvāda");
            yield return BuildTestCase("gy", "દક્ષિણ", "Dakṣhiṇa");
            yield return BuildTestCase("gy", "૧", "1");
            yield return BuildTestCase("gy", "૩", "3");
        }

        private static IEnumerable<TestCaseData> HebrewScenarios()
        {
            yield return BuildTestCase("he", "אַשְׁדּוֹד", "ʾAšdōḏ");
            yield return BuildTestCase("he", "בְּאֵר שֶׁבַע", "Bəʾēr Ševaʿ");
            yield return BuildTestCase("he", "הַיָּם הָאָדְוֹם", "Hayyām Hāʾāḏōm");
            yield return BuildTestCase("he", "יַם-סוּף", "Yam-sūf");
            yield return BuildTestCase("he", "ירושלים", "Yerushaláyim");
            yield return BuildTestCase("he", "יִשְׂרָאֵל", "Yīsrāʾēl");
            yield return BuildTestCase("he", "פַּרְעֹה", "Parʿō");
            yield return BuildTestCase("he", "תל-אביב", "Tel-Aviv");
        }

        private static IEnumerable<TestCaseData> JapaneseScenarios()
        {
            yield return BuildTestCase("ja", "さいたま", "Saitama");
            yield return BuildTestCase("ja", "京都", "Kyōto");
            yield return BuildTestCase("ja", "北海道", "Hokkaidō");
            yield return BuildTestCase("ja", "大阪", "Ōsaka");
            yield return BuildTestCase("ja", "富士山", "Fujisan");
            yield return BuildTestCase("ja", "東京", "Tōkyō");
            yield return BuildTestCase("ja", "横浜", "Yokohama");
            yield return BuildTestCase("ja", "沖縄", "Okinawa");
            yield return BuildTestCase("ja", "高知", "Kōchi");
            yield return BuildTestCase("ja", "鹿児島", "Kagoshima");
        }

        private static IEnumerable<TestCaseData> KoreanScenarios()
        {
            yield return BuildTestCase("ko", "거제", "Geoje");
            yield return BuildTestCase("ko", "경주", "Gyeongju");
            yield return BuildTestCase("ko", "광주", "Gwangju");
            yield return BuildTestCase("ko", "대구", "Daegu");
            yield return BuildTestCase("ko", "부산", "Busan");
            yield return BuildTestCase("ko", "서울", "Seoul");
            yield return BuildTestCase("ko", "수원", "Suwon");
            yield return BuildTestCase("ko", "인천", "Incheon");
            yield return BuildTestCase("ko", "제주", "Jeju");
            yield return BuildTestCase("ko", "화성", "Hwaseong");
        }

        private static IEnumerable<TestCaseData> MarathiScenarios()
        {
            yield return BuildTestCase("mr", "अमरावती", "Amrāvatī");
            yield return BuildTestCase("mr", "अहमदनगर", "Ahmadnagar");
            yield return BuildTestCase("mr", "इंदापूर", "Indāpūr");
            yield return BuildTestCase("mr", "औरंगाबाद", "Aurangābād");
            yield return BuildTestCase("mr", "कोल्हापूर", "Kōlhāpūr");
            yield return BuildTestCase("mr", "नागपूर", "Nāgāpūr");
            yield return BuildTestCase("mr", "मीराज", "Mīrāj");
            yield return BuildTestCase("mr", "रत्नागिरी", "Ratnāgirī");
            yield return BuildTestCase("mr", "सोलापूर", "Solāpūr");
            yield return BuildTestCase("mr", "हिंगोली", "Hiṅgōlī");
        }

        private static IEnumerable<TestCaseData> PinyinScenarios()
        {
            string[] chineseLanguageCodes = ["zh", "zh-hans"];

            foreach (string languageCode in chineseLanguageCodes)
            {
                yield return BuildTestCase(languageCode, "凯奇凯梅特", "Kǎijīkǎiméitè");
                yield return BuildTestCase(languageCode, "凱代尼艾", "Kǎidàiníài");
                yield return BuildTestCase(languageCode, "基思", "Jīsāi");
            }
        }

        private static TestCaseData BuildTestCase(string languageCode, string sourceText, string expectedText)
            => new(languageCode, sourceText, expectedText);

        private static string BuildEndpoint(string text, string languageCode)
            => $"/transliteration?text={Uri.EscapeDataString(text)}&language={Uri.EscapeDataString(languageCode)}";

        private static async Task<JsonDocument> ReadResponseBody(HttpResponseMessage response)
        {
            string responseBody = await response.Content.ReadAsStringAsync();

            return JsonDocument.Parse(responseBody);
        }
    }
}