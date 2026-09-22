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
using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.IntegrationTests.Service
{
    [TestFixture]
    public sealed class RegisteredLanguageEndpointTests
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
        [TestCaseSource(nameof(RegisteredLanguageScenarios))]
        public async Task GivenARegisteredLanguage_WhenRequestingTransliteration_ThenItsConfiguredTransliteratorIsExecuted(
            string languageCode,
            string text,
            string providerResponse,
            string expectedText)
        {
            factory.HttpRequestManager.ResponseToReturn = providerResponse;

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(text, languageCode));
            using JsonDocument responseBody = await ReadResponseBody(response);
            Language language = Language.FromCode(languageCode);
            int expectedProviderInvocations = 0;

            if (language.UsesExternalTransliterator)
            {
                expectedProviderInvocations = 1;
            }

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseBody.RootElement.GetProperty("text").GetString(), Is.EqualTo(expectedText));
                Assert.That(factory.HttpRequestManager.TotalPostInvocationCount, Is.EqualTo(expectedProviderInvocations));
                Assert.That(ReadCacheEntryCount(), Is.EqualTo(1));
            });
        }

        [Test]
        public void GivenTheRegisteredLanguageMatrix_WhenComparingItWithTheRegistry_ThenEveryLanguageOccursExactlyOnce()
        {
            string[] registeredCodes = Language.GetAll()
                .Select(language => language.Code)
                .OrderBy(languageCode => languageCode)
                .ToArray();
            string[] testedCodes = RegisteredLanguageScenarios()
                .Select(testCase => testCase.Arguments[0] as string)
                .OrderBy(languageCode => languageCode)
                .ToArray()!;

            Assert.Multiple(() =>
            {
                Assert.That(testedCodes, Has.Length.EqualTo(registeredCodes.Length));
                Assert.That(testedCodes, Is.EqualTo(registeredCodes));
                Assert.That(testedCodes, Is.Unique);
            });
        }

        private int ReadCacheEntryCount()
        {
            using JsonDocument cache = JsonDocument.Parse(File.ReadAllText(factory.CacheStoreLocation));

            return cache.RootElement.GetArrayLength();
        }

        private static IEnumerable<TestCaseData> RegisteredLanguageScenarios()
            => LocalLanguageScenarios()
                .Concat(TranslitterationDotComLanguageScenarios())
                .Concat(UshuaiaLanguageScenarios())
                .Concat(PodolakLanguageScenarios());

        private static IEnumerable<TestCaseData> LocalLanguageScenarios()
        {
            yield return BuildTestCase("ab", "Аҟәа", string.Empty, "Ak̄a̋a");
            yield return BuildTestCase("grc", "Ἀθῆναι", string.Empty, "Athênai");
            yield return BuildTestCase("grc-dor", "Ᾰθῆναι", string.Empty, "Athênai");
            yield return BuildTestCase("ar", "أبيدوس", string.Empty, "Abīdūs");
            yield return BuildTestCase("be", "А", string.Empty, "A");
            yield return BuildTestCase("ber", "ⴳⵓⵍⵎⵉⵎ ⴰⵙⵉⴼ ⵏⵓⵏ", string.Empty, "Gulmim Asif Nun");
            yield return BuildTestCase("bg", "А", string.Empty, "A");
            yield return BuildTestCase("zh", "凯奇凯梅特", string.Empty, "Kǎijīkǎiméitè");
            yield return BuildTestCase("cv", "А", string.Empty, "A");
            yield return BuildTestCase("cop", "ⲁⲗⲉⲝⲁⲛⲇⲣⲓⲁ", string.Empty, "Alexandria");
            yield return BuildTestCase("arz", "أبيدوس", string.Empty, "Abīdūs");
            yield return BuildTestCase("el", "Χαλάνδρι", string.Empty, "Chalándri");
            yield return BuildTestCase("gy", "ભારત", string.Empty, "Bhārata");
            yield return BuildTestCase("he", "תל-אביב", string.Empty, "Tel-Aviv");
            yield return BuildTestCase("ja", "さいたま", string.Empty, "Saitama");
            yield return BuildTestCase("kk", "А", string.Empty, "A");
            yield return BuildTestCase("ko", "거제", string.Empty, "Geoje");
            yield return BuildTestCase("mk", "А", string.Empty, "A");
            yield return BuildTestCase("ary", "أبيدوس", string.Empty, "Abīdūs");
            yield return BuildTestCase("mr", "अमरावती", string.Empty, "Amrāvatī");
            yield return BuildTestCase("ru", "А", string.Empty, "A");
            yield return BuildTestCase("sr", "А", string.Empty, "A");
            yield return BuildTestCase("sr-ec", "А", string.Empty, "A");
            yield return BuildTestCase("sh", "А", string.Empty, "A");
            yield return BuildTestCase("zh-hans", "凯奇凯梅特", string.Empty, "Kǎijīkǎiméitè");
            yield return BuildTestCase("tg", "А", string.Empty, "A");
            yield return BuildTestCase("tg-cyrl", "А", string.Empty, "A");
            yield return BuildTestCase("tt", "А", string.Empty, "A");
            yield return BuildTestCase("tt-cyrl", "А", string.Empty, "A");
            yield return BuildTestCase("uk", "А", string.Empty, "A");
        }

        private static IEnumerable<TestCaseData> TranslitterationDotComLanguageScenarios()
        {
            yield return BuildTestCase("ady", "Мэйкъуапэ", "ack:::maykuape", "maykuape");
            yield return BuildTestCase("hy", "Անի", "ack:::ani", "Ani");
            yield return BuildTestCase("ba", "Өфө", "ack:::ofo", "ofo");
            yield return BuildTestCase("ka", "ბათუმი", "ack:::batumi", "Batumi");
            yield return BuildTestCase("iu", "ᐄᐆᓄᓇᕗᑦ", "ack:::ᐄᐆnunavut", "Iununavut");
            yield return BuildTestCase("ky", "Ош", "ack:::osh", "Osh");
            yield return BuildTestCase("os", "Цхинвали", "ack:::tskhinvali", "tskhinvali");
            yield return BuildTestCase("udm", "Ижевск", "ack:::izhevsk", "izhevsk");
            yield return BuildTestCase("hyw", "Վան", "ack:::van", "van");
        }

        private static IEnumerable<TestCaseData> UshuaiaLanguageScenarios()
        {
            yield return BuildTestCase("bn", "চট্টগ্রাম", "chattagram", "Chattagram");
            yield return BuildTestCase("hi", "श्रीनगर", "srinagar", "Sarinagar");
            yield return BuildTestCase("kn", "ಮೈಸೂರು", "mysuru", "Mysuru");
            yield return BuildTestCase("ml", "തിരുവനന്തപുരം", "thiruvananthapuram", "Thiruvananthapuram");
            yield return BuildTestCase("mn", "Дархан", "darkhan", "darkhan");
            yield return BuildTestCase("sa", "प्रयागराज", "prayagraj", "Prayagraj");
            yield return BuildTestCase("si", "ගාල්ල", "galle", "Galle");
            yield return BuildTestCase("ta", "மதுரை", "madurai", "Madurai");
            yield return BuildTestCase("te", "విజయవాడ", "vijayawada", "Vijayawada");
        }

        private static IEnumerable<TestCaseData> PodolakLanguageScenarios()
        {
            yield return BuildTestCase(
                "cu",
                "слово",
                "<textarea id=\"ausgabe\">slovo</textarea>",
                "slovo");
        }

        private static TestCaseData BuildTestCase(
            string languageCode,
            string text,
            string providerResponse,
            string expectedText)
            => new TestCaseData(languageCode, text, providerResponse, expectedText)
                .SetName($"GivenThe{Language.FromCode(languageCode).Name}Language_WhenRequestingTransliteration_ThenItsConfiguredTransliteratorIsExecuted");

        private static string BuildEndpoint(string text, string languageCode)
            => $"/transliteration?text={Uri.EscapeDataString(text)}&language={Uri.EscapeDataString(languageCode)}";

        private static async Task<JsonDocument> ReadResponseBody(HttpResponseMessage response)
        {
            string responseBody = await response.Content.ReadAsStringAsync();

            return JsonDocument.Parse(responseBody);
        }
    }
}