using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using NUnit.Framework;

using TransliterationAPI.IntegrationTests.Infrastructure;

namespace TransliterationAPI.IntegrationTests.Service
{
    [TestFixture]
    public sealed class TranslitterationDotComEndpointTests
    {
        private static string ProviderEndpoint => "https://www.translitteration.com/ajax/en/transliterate";

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
        [TestCaseSource(nameof(LanguageRequestScenarios))]
        public async Task GivenATranslitterationDotComLanguage_WhenRequestingTransliteration_ThenTheExpectedProviderRequestIsSent(
            string languageCode,
            string text,
            string providerResponse,
            string expectedText,
            string expectedTargetLanguageCode,
            string expectedScheme)
        {
            factory.HttpRequestManager.ResponseToReturn = providerResponse;

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(text, languageCode));
            using JsonDocument responseBody = await ReadResponseBody(response);
            RecordingHttpRequestManager manager = factory.HttpRequestManager;

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseBody.RootElement.GetProperty("text").GetString(), Is.EqualTo(expectedText));
                Assert.That(manager.PostInvocationCount, Is.EqualTo(1));
                Assert.That(manager.PostWithHeadersInvocationCount, Is.Zero);
                Assert.That(manager.RetrieveCookiesInvocationCount, Is.Zero);
                Assert.That(manager.LastPostUrl, Is.EqualTo(ProviderEndpoint));
                Assert.That(manager.LastFormData, Has.Count.EqualTo(4));
                Assert.That(manager.LastFormData!["text"], Is.EqualTo(text));
                Assert.That(manager.LastFormData["tlang"], Is.EqualTo(expectedTargetLanguageCode));
                Assert.That(manager.LastFormData["script"], Is.EqualTo("latn"));
                Assert.That(manager.LastFormData["scheme"], Is.EqualTo(expectedScheme));
                Assert.That(manager.LastHeaders, Is.Null);
            });
        }

        [Test]
        [TestCase("ack:::maykuape", "maykuape")]
        [TestCase("maykuape", "maykuape")]
        [TestCase("ack:::ack:::maykuape", "maykuape")]
        [TestCase("prefix ack:::maykuape", "prefix maykuape")]
        [TestCase("ack:::", "")]
        [TestCase("", "")]
        public async Task GivenAnyProviderPrefixArrangement_WhenRequestingTransliteration_ThenEveryPrefixIsRemoved(
            string providerResponse,
            string expectedText)
        {
            factory.HttpRequestManager.ResponseToReturn = providerResponse;

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint("Мэйкъуапэ", "ady"));
            using JsonDocument responseBody = await ReadResponseBody(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseBody.RootElement.GetProperty("text").GetString(), Is.EqualTo(expectedText));
            });
        }

        [Test]
        [TestCase("Hello, World!")]
        [TestCase("Crăciun Fericit!")]
        [TestCase("C'est la vie")]
        [TestCase("Անի")]
        [TestCase("A&B = C+D? #1/2")]
        [TestCase("Line one\r\nLine two\tTabbed")]
        [TestCase("😀 🌞 🚀")]
        public async Task GivenDiverseText_WhenRequestingTransliteration_ThenTheProviderFormPreservesIt(string text)
        {
            factory.HttpRequestManager.ResponseToReturn = "ack:::result";

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(text, "ady"));

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(factory.HttpRequestManager.LastFormData!["text"], Is.EqualTo(text));
            });
        }

        [Test]
        [TestCaseSource(nameof(ProviderFailureScenarios))]
        public async Task GivenAProviderException_WhenRequestingTransliteration_ThenNullIsReturnedWithoutCaching(
            Exception exception)
        {
            factory.HttpRequestManager.ExceptionToThrow = exception;

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint("Мэйкъуапэ", "ady"));
            using JsonDocument responseBody = await ReadResponseBody(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseBody.RootElement.GetProperty("text").ValueKind, Is.EqualTo(JsonValueKind.Null));
                Assert.That(ReadCacheEntryCount(), Is.Zero);
            });
        }

        [Test]
        public async Task GivenAProviderThatRecovers_WhenRetryingTransliteration_ThenTheSuccessfulResultIsReturnedAndCached()
        {
            factory.HttpRequestManager.ExceptionToThrow = new HttpRequestException("Service unavailable.");

            using HttpResponseMessage failedResponse = await client.GetAsync(BuildEndpoint("Мэйкъуапэ", "ady"));
            factory.HttpRequestManager.ExceptionToThrow = null;
            factory.HttpRequestManager.ResponseToReturn = "ack:::maykuape";

            using HttpResponseMessage successfulResponse = await client.GetAsync(BuildEndpoint("Мэйкъуапэ", "ady"));
            using JsonDocument successfulBody = await ReadResponseBody(successfulResponse);

            Assert.Multiple(() =>
            {
                Assert.That(failedResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(successfulResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(successfulBody.RootElement.GetProperty("text").GetString(), Is.EqualTo("maykuape"));
                Assert.That(factory.HttpRequestManager.PostInvocationCount, Is.EqualTo(2));
                Assert.That(ReadCacheEntryCount(), Is.EqualTo(1));
            });
        }

        private int ReadCacheEntryCount()
        {
            using JsonDocument cache = JsonDocument.Parse(File.ReadAllText(factory.CacheStoreLocation));

            return cache.RootElement.GetArrayLength();
        }

        private static IEnumerable<TestCaseData> LanguageRequestScenarios()
        {
            yield return BuildLanguageTestCase("ady", "Мэйкъуапэ", "ack:::maykuape", "maykuape", "ady", "iso-9");
            yield return BuildLanguageTestCase("hy", "Անի", "ack:::ani", "Ani", "xcl", "iso-9985");
            yield return BuildLanguageTestCase("ba", "Өфө", "ack:::ofo", "ofo", "bak", "iso-9");
            yield return BuildLanguageTestCase("ka", "ბათუმი", "ack:::batumi", "Batumi", "kat", "national");
            yield return BuildLanguageTestCase("iu", "ᐄᐆᓄᓇᕗᑦ", "ack:::ᐄᐆnunavut", "Iununavut", "iku", "canadian-aboriginal-syllabics");
            yield return BuildLanguageTestCase("ky", "Ош", "ack:::osh", "Osh", "kir", "iso-9");
            yield return BuildLanguageTestCase("os", "Цхинвали", "ack:::tskhinvali", "tskhinvali", "oss", "iso-9");
            yield return BuildLanguageTestCase("udm", "Ижевск", "ack:::izhevsk", "izhevsk", "udm", "bgn-pcgn");
            yield return BuildLanguageTestCase("hyw", "Վան", "ack:::van", "van", "hye", "ala-lc");
        }

        private static IEnumerable<Exception> ProviderFailureScenarios()
        {
            yield return new HttpRequestException("Service unavailable.");
            yield return new TaskCanceledException("Request timeout.");
            yield return new TimeoutException("Request timeout.");
            yield return new InvalidOperationException("Malformed response.");
            yield return new ArgumentException("Invalid provider request.");
        }

        private static TestCaseData BuildLanguageTestCase(
            string languageCode,
            string text,
            string providerResponse,
            string expectedText,
            string expectedTargetLanguageCode,
            string expectedScheme)
            => new TestCaseData(
                languageCode,
                text,
                providerResponse,
                expectedText,
                expectedTargetLanguageCode,
                expectedScheme)
                .SetName($"GivenThe{languageCode}Language_WhenRequestingTransliteration_ThenTheExpectedTranslitterationDotComRequestIsSent");

        private static string BuildEndpoint(string text, string languageCode)
            => $"/transliteration?text={Uri.EscapeDataString(text)}&language={Uri.EscapeDataString(languageCode)}";

        private static async Task<JsonDocument> ReadResponseBody(HttpResponseMessage response)
        {
            string responseBody = await response.Content.ReadAsStringAsync();

            return JsonDocument.Parse(responseBody);
        }
    }
}