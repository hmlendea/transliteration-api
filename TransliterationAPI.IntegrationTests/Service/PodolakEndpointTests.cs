using System;
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
    public sealed class PodolakEndpointTests
    {
        private static string LanguageCode => "cu";
        private static string ProviderEndpoint => "https://podolak.net/en/transliteration/old-church-slavonic";
        private static string SourceText => "слово";

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
        public async Task GivenAnOldChurchSlavonicRequest_WhenRequestingTransliteration_ThenTheExpectedProviderRequestIsSent()
        {
            factory.HttpRequestManager.ResponseToReturn = "<textarea id=\"ausgabe\">slovo</textarea>";

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(SourceText));
            using JsonDocument responseBody = await ReadResponseBody(response);
            RecordingHttpRequestManager manager = factory.HttpRequestManager;

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseBody.RootElement.GetProperty("text").GetString(), Is.EqualTo("slovo"));
                Assert.That(manager.PostInvocationCount, Is.EqualTo(1));
                Assert.That(manager.PostWithHeadersInvocationCount, Is.Zero);
                Assert.That(manager.RetrieveCookiesInvocationCount, Is.Zero);
                Assert.That(manager.LastPostUrl, Is.EqualTo(ProviderEndpoint));
                Assert.That(manager.LastFormData, Has.Count.EqualTo(6));
                Assert.That(manager.LastFormData!["quelltext"], Is.EqualTo(LanguageCode));
                Assert.That(manager.LastFormData["zieltext"], Is.EqualTo("isor9"));
                Assert.That(manager.LastFormData["startabfrage"], Is.EqualTo("1"));
                Assert.That(manager.LastFormData["text"], Is.EqualTo(SourceText));
                Assert.That(manager.LastFormData["transliteration"], Is.EqualTo("Transliteration"));
                Assert.That(manager.LastFormData["cu_isor9_jer"], Is.EqualTo("3"));
                Assert.That(manager.LastHeaders, Is.Null);
            });
        }

        [Test]
        [TestCase("<textarea id=\"ausgabe\">slovo</textarea>", "slovo")]
        [TestCase("header\n<textarea id=\"ausgabe\">izgnanica</textarea>\nfooter", "izgnanica")]
        [TestCase("header\r\n<textarea id=\"ausgabe\">slovo</textarea>\r\nfooter", "slovo")]
        [TestCase("<div><textarea id=\"ausgabe\">slovo</textarea></div>", "slovo")]
        [TestCase("<textarea id=\"ausgabe\">&amp;&lt;&gt;</textarea>", "&amp;&lt;&gt;")]
        [TestCase("<textarea id=\"ausgabe\">Crăciun Fericit!</textarea>", "Crăciun Fericit!")]
        [TestCase("<textarea id=\"ausgabe\">😀 🌞 🚀</textarea>", "😀 🌞 🚀")]
        public async Task GivenAValidProviderResponse_WhenRequestingTransliteration_ThenItsResultIsExtracted(
            string providerResponse,
            string expectedText)
        {
            factory.HttpRequestManager.ResponseToReturn = providerResponse;

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(SourceText));
            using JsonDocument responseBody = await ReadResponseBody(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseBody.RootElement.GetProperty("text").GetString(), Is.EqualTo(expectedText));
                Assert.That(ReadCacheEntryCount(), Is.EqualTo(1));
            });
        }

        [Test]
        public async Task GivenMultipleProviderResults_WhenRequestingTransliteration_ThenTheFirstResultIsExtracted()
        {
            factory.HttpRequestManager.ResponseToReturn =
                "<textarea id=\"ausgabe\">first</textarea>\n<textarea id=\"ausgabe\">second</textarea>";

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(SourceText));
            using JsonDocument responseBody = await ReadResponseBody(response);

            Assert.That(responseBody.RootElement.GetProperty("text").GetString(), Is.EqualTo("first"));
        }

        [Test]
        public async Task GivenAnEmptyProviderResult_WhenRequestingTransliterationRepeatedly_ThenItIsNotCached()
        {
            factory.HttpRequestManager.ResponseToReturn = "<textarea id=\"ausgabe\"></textarea>";

            using HttpResponseMessage firstResponse = await client.GetAsync(BuildEndpoint(SourceText));
            using HttpResponseMessage secondResponse = await client.GetAsync(BuildEndpoint(SourceText));
            using JsonDocument secondBody = await ReadResponseBody(secondResponse);

            Assert.Multiple(() =>
            {
                Assert.That(firstResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(secondBody.RootElement.GetProperty("text").GetString(), Is.Empty);
                Assert.That(factory.HttpRequestManager.PostInvocationCount, Is.EqualTo(2));
                Assert.That(ReadCacheEntryCount(), Is.Zero);
            });
        }

        [Test]
        [TestCase("<textarea id=\"ausgabe\">\nslovo\n</textarea>")]
        [TestCase("<textarea id=\"ausgabe\">\r\nslovo\r\n</textarea>")]
        public async Task GivenAMultilineProviderResult_WhenRequestingTransliteration_ThenNullIsReturnedWithoutCaching(
            string providerResponse)
        {
            factory.HttpRequestManager.ResponseToReturn = providerResponse;

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(SourceText));
            using JsonDocument responseBody = await ReadResponseBody(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseBody.RootElement.GetProperty("text").ValueKind, Is.EqualTo(JsonValueKind.Null));
                Assert.That(ReadCacheEntryCount(), Is.Zero);
            });
        }

        [Test]
        [TestCaseSource(nameof(ProviderFailureScenarios))]
        public async Task GivenAProviderException_WhenRequestingTransliteration_ThenNullIsReturnedWithoutCaching(
            Exception exception)
        {
            factory.HttpRequestManager.ExceptionToThrow = exception;

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(SourceText));
            using JsonDocument responseBody = await ReadResponseBody(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseBody.RootElement.GetProperty("text").ValueKind, Is.EqualTo(JsonValueKind.Null));
                Assert.That(ReadCacheEntryCount(), Is.Zero);
            });
        }

        private int ReadCacheEntryCount()
        {
            using JsonDocument cache = JsonDocument.Parse(File.ReadAllText(factory.CacheStoreLocation));

            return cache.RootElement.GetArrayLength();
        }

        private static Exception[] ProviderFailureScenarios()
            =>
            [
                new HttpRequestException("Service unavailable."),
                new TaskCanceledException("Request timeout."),
                new TimeoutException("Request timeout."),
                new InvalidOperationException("Malformed response.")
            ];

        private static string BuildEndpoint(string text)
            => $"/transliteration?text={Uri.EscapeDataString(text)}&language={LanguageCode}";

        private static async Task<JsonDocument> ReadResponseBody(HttpResponseMessage response)
        {
            string responseBody = await response.Content.ReadAsStringAsync();

            return JsonDocument.Parse(responseBody);
        }
    }
}