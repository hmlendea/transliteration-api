using System;
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
    public sealed class TransliterationServiceIntegrationTests
    {
        private static string ExternalLanguageCode => "ady";
        private static string ExternalText => "Налщик";
        private static string ProviderResponse => "ack:::nalshchik";

        private HttpClient client = null!;
        private TransliterationApiWebApplicationFactory factory = null!;

        [SetUp]
        public void SetUp()
        {
            factory = new TransliterationApiWebApplicationFactory();
            factory.HttpRequestManager.ResponseToReturn = ProviderResponse;
            client = factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            client.Dispose();
            factory.Dispose();
        }

        [Test]
        [TestCase("unknown")]
        [TestCase("xx")]
        [TestCase("zz-ZZ")]
        [TestCase("RU")]
        [TestCase("Ru")]
        [TestCase("ru-RU")]
        [TestCase(" ru")]
        [TestCase("ru ")]
        [TestCase("123")]
        [TestCase("-")]
        [TestCase("_")]
        [TestCase("日本語")]
        [TestCase("😀")]
        public async Task GivenAnUnknownLanguageCode_WhenRequestingTransliteration_ThenTheOriginalTextIsReturned(
            string languageCode)
        {
            string expectedText = "Crăciun Fericit!";

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(expectedText, languageCode));
            using JsonDocument responseBody = await ReadResponseBody(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseBody.RootElement.GetProperty("text").GetString(), Is.EqualTo(expectedText));
                Assert.That(factory.HttpRequestManager.TotalPostInvocationCount, Is.Zero);
                Assert.That(ReadCacheEntryCount(), Is.Zero);
            });
        }

        [Test]
        public async Task GivenNoLanguageCode_WhenRequestingTransliteration_ThenTheOriginalTextIsReturned()
        {
            using HttpResponseMessage response = await client.GetAsync("/transliteration?text=Hello%2C%20World!");
            using JsonDocument responseBody = await ReadResponseBody(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseBody.RootElement.GetProperty("text").GetString(), Is.EqualTo("Hello, World!"));
                Assert.That(factory.HttpRequestManager.TotalPostInvocationCount, Is.Zero);
            });
        }

        [Test]
        public async Task GivenNoTextForAnUnknownLanguage_WhenRequestingTransliteration_ThenNullTextIsReturned()
        {
            using HttpResponseMessage response = await client.GetAsync("/transliteration?language=unknown");
            using JsonDocument responseBody = await ReadResponseBody(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseBody.RootElement.GetProperty("text").ValueKind, Is.EqualTo(JsonValueKind.Null));
                Assert.That(factory.HttpRequestManager.TotalPostInvocationCount, Is.Zero);
            });
        }

        [Test]
        [TestCase("/transliteration?language=ru")]
        [TestCase("/transliteration?text=&language=ru")]
        public async Task GivenNoTextForASupportedLanguage_WhenRequestingTransliteration_ThenBadRequestIsReturned(
            string endpoint)
        {
            using HttpResponseMessage response = await client.GetAsync(endpoint);
            using JsonDocument responseBody = await ReadResponseBody(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(responseBody.RootElement.GetProperty("code").GetString(), Is.EqualTo("BAD_REQUEST"));
                Assert.That(factory.HttpRequestManager.TotalPostInvocationCount, Is.Zero);
            });
        }

        [Test]
        [TestCase(" Налщик")]
        [TestCase("Налщик ")]
        [TestCase("  Налщик  ")]
        [TestCase("\tНалщик\t")]
        [TestCase("\rНалщик\r")]
        [TestCase("\nНалщик\n")]
        [TestCase("\r\nНалщик\r\n")]
        [TestCase(" \t\r\nНалщик\n\r\t ")]
        public async Task GivenTextWithSurroundingWhitespace_WhenRequestingTransliteration_ThenTheProviderReceivesTrimmedText(
            string text)
        {
            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(text, ExternalLanguageCode));
            using JsonDocument responseBody = await ReadResponseBody(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseBody.RootElement.GetProperty("text").GetString(), Is.EqualTo("nalshchik"));
                Assert.That(factory.HttpRequestManager.LastFormData, Contains.Key("text"));
                Assert.That(factory.HttpRequestManager.LastFormData!["text"], Is.EqualTo(ExternalText));
            });
        }

        [Test]
        public void GivenANewApplicationHost_WhenItStarts_ThenAnEmptyCacheStoreIsCreated()
        {
            Assert.Multiple(() =>
            {
                Assert.That(File.Exists(factory.CacheStoreLocation));
                Assert.That(File.ReadAllText(factory.CacheStoreLocation), Is.EqualTo("[]"));
            });
        }

        [Test]
        public async Task GivenANovelSuccessfulTransliteration_WhenRequestingIt_ThenTheResultIsPersisted()
        {
            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(ExternalText, ExternalLanguageCode));
            using JsonDocument cache = JsonDocument.Parse(File.ReadAllText(factory.CacheStoreLocation));
            JsonElement cacheEntry = cache.RootElement[0];

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(cache.RootElement.GetArrayLength(), Is.EqualTo(1));
                Assert.That(cacheEntry.GetProperty("id").GetString(), Does.Match("^[0-9a-f]{64}$"));
                Assert.That(cacheEntry.GetProperty("transliteratedText").GetString(), Is.EqualTo("nalshchik"));
            });
        }

        [Test]
        public async Task GivenACachedTransliteration_WhenRequestingItAgain_ThenTheProviderIsNotInvokedAgain()
        {
            using HttpResponseMessage firstResponse = await client.GetAsync(BuildEndpoint(ExternalText, ExternalLanguageCode));
            factory.HttpRequestManager.ResponseToReturn = "ack:::maykuape";

            using HttpResponseMessage secondResponse = await client.GetAsync(BuildEndpoint(ExternalText, ExternalLanguageCode));
            using JsonDocument secondBody = await ReadResponseBody(secondResponse);

            Assert.Multiple(() =>
            {
                Assert.That(firstResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(secondResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(secondBody.RootElement.GetProperty("text").GetString(), Is.EqualTo("nalshchik"));
                Assert.That(factory.HttpRequestManager.TotalPostInvocationCount, Is.EqualTo(1));
                Assert.That(ReadCacheEntryCount(), Is.EqualTo(1));
            });
        }

        [Test]
        [TestCase(" Налщик ")]
        [TestCase("\tНалщик\r\n")]
        [TestCase("\n\r Налщик \t")]
        public async Task GivenEquivalentNormalisedText_WhenRequestingItAgain_ThenTheCachedResultIsReturned(
            string firstText)
        {
            using HttpResponseMessage firstResponse = await client.GetAsync(BuildEndpoint(firstText, ExternalLanguageCode));
            factory.HttpRequestManager.ResponseToReturn = "ack:::maykuape";

            using HttpResponseMessage secondResponse = await client.GetAsync(BuildEndpoint(ExternalText, ExternalLanguageCode));
            using JsonDocument secondBody = await ReadResponseBody(secondResponse);

            Assert.Multiple(() =>
            {
                Assert.That(firstResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(secondBody.RootElement.GetProperty("text").GetString(), Is.EqualTo("nalshchik"));
                Assert.That(factory.HttpRequestManager.TotalPostInvocationCount, Is.EqualTo(1));
                Assert.That(ReadCacheEntryCount(), Is.EqualTo(1));
            });
        }

        [Test]
        public async Task GivenTheSameTextForDifferentLanguages_WhenRequestingBoth_ThenSeparateCacheEntriesArePersisted()
        {
            using HttpResponseMessage adygheResponse = await client.GetAsync(BuildEndpoint(ExternalText, "ady"));
            factory.HttpRequestManager.ResponseToReturn = "ack:::baškortostan";

            using HttpResponseMessage bashkirResponse = await client.GetAsync(BuildEndpoint(ExternalText, "ba"));

            Assert.Multiple(() =>
            {
                Assert.That(adygheResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(bashkirResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(factory.HttpRequestManager.TotalPostInvocationCount, Is.EqualTo(2));
                Assert.That(ReadCacheEntryCount(), Is.EqualTo(2));
            });
        }

        [Test]
        public async Task GivenTextThatDiffersOnlyByCase_WhenRequestingBothValues_ThenSeparateCacheEntriesArePersisted()
        {
            using HttpResponseMessage firstResponse = await client.GetAsync(BuildEndpoint("Налщик", ExternalLanguageCode));
            factory.HttpRequestManager.ResponseToReturn = "ack:::NALSHCHIK";

            using HttpResponseMessage secondResponse = await client.GetAsync(BuildEndpoint("налщик", ExternalLanguageCode));

            Assert.Multiple(() =>
            {
                Assert.That(firstResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(secondResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(factory.HttpRequestManager.TotalPostInvocationCount, Is.EqualTo(2));
                Assert.That(ReadCacheEntryCount(), Is.EqualTo(2));
            });
        }

        [Test]
        [TestCase("ack:::")]
        [TestCase("ack:::   ")]
        public async Task GivenAnEmptyOrWhitespaceProviderResult_WhenRequestingItRepeatedly_ThenTheResultIsNotCached(
            string providerResponse)
        {
            factory.HttpRequestManager.ResponseToReturn = providerResponse;

            using HttpResponseMessage firstResponse = await client.GetAsync(BuildEndpoint(ExternalText, ExternalLanguageCode));
            using HttpResponseMessage secondResponse = await client.GetAsync(BuildEndpoint(ExternalText, ExternalLanguageCode));

            Assert.Multiple(() =>
            {
                Assert.That(firstResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(secondResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(factory.HttpRequestManager.TotalPostInvocationCount, Is.EqualTo(2));
                Assert.That(ReadCacheEntryCount(), Is.Zero);
            });
        }

        [Test]
        public async Task GivenAProviderFailure_WhenRequestingTransliterationRepeatedly_ThenNullIsReturnedAndNotCached()
        {
            factory.HttpRequestManager.ExceptionToThrow = new HttpRequestException("Service unavailable.");

            using HttpResponseMessage firstResponse = await client.GetAsync(BuildEndpoint(ExternalText, ExternalLanguageCode));
            using HttpResponseMessage secondResponse = await client.GetAsync(BuildEndpoint(ExternalText, ExternalLanguageCode));
            using JsonDocument secondBody = await ReadResponseBody(secondResponse);

            Assert.Multiple(() =>
            {
                Assert.That(firstResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(secondResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(secondBody.RootElement.GetProperty("text").ValueKind, Is.EqualTo(JsonValueKind.Null));
                Assert.That(factory.HttpRequestManager.TotalPostInvocationCount, Is.EqualTo(2));
                Assert.That(ReadCacheEntryCount(), Is.Zero);
            });
        }

        [Test]
        [TestCase("")]
        [TestCase("<html></html>")]
        [TestCase("<textarea>slovo</textarea>")]
        [TestCase("ausgabe without a textarea")]
        public async Task GivenAMalformedPodolakResponse_WhenRequestingTransliteration_ThenNullIsReturnedAndNotCached(
            string providerResponse)
        {
            factory.HttpRequestManager.ResponseToReturn = providerResponse;

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint("слово", "cu"));
            using JsonDocument responseBody = await ReadResponseBody(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseBody.RootElement.GetProperty("text").ValueKind, Is.EqualTo(JsonValueKind.Null));
                Assert.That(ReadCacheEntryCount(), Is.Zero);
            });
        }

        [Test]
        public async Task GivenCachingIsDisabled_WhenRequestingTheSameTransliterationTwice_ThenTheProviderIsInvokedTwice()
        {
            client.Dispose();
            factory.Dispose();
            factory = new TransliterationApiWebApplicationFactory(false);
            factory.HttpRequestManager.ResponseToReturn = ProviderResponse;
            client = factory.CreateClient();

            using HttpResponseMessage firstResponse = await client.GetAsync(BuildEndpoint(ExternalText, ExternalLanguageCode));
            using HttpResponseMessage secondResponse = await client.GetAsync(BuildEndpoint(ExternalText, ExternalLanguageCode));

            Assert.Multiple(() =>
            {
                Assert.That(firstResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(secondResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(factory.HttpRequestManager.TotalPostInvocationCount, Is.EqualTo(2));
                Assert.That(ReadCacheEntryCount(), Is.Zero);
            });
        }

        [Test]
        public async Task GivenACorruptCacheStore_WhenRequestingTransliteration_ThenAnInternalServerErrorIsReturned()
        {
            File.WriteAllText(factory.CacheStoreLocation, "not valid JSON");

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(ExternalText, ExternalLanguageCode));
            using JsonDocument responseBody = await ReadResponseBody(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
                Assert.That(responseBody.RootElement.GetProperty("code").GetString(), Is.EqualTo("INTERNAL_SERVER_ERROR"));
                Assert.That(factory.HttpRequestManager.TotalPostInvocationCount, Is.Zero);
            });
        }

        [Test]
        public async Task GivenAPersistedCacheFromAPriorHost_WhenRequestingTheSameTransliteration_ThenThePersistedResultIsReturned()
        {
            using HttpResponseMessage firstResponse = await client.GetAsync(BuildEndpoint(ExternalText, ExternalLanguageCode));
            string persistedCache = File.ReadAllText(factory.CacheStoreLocation);
            client.Dispose();
            factory.Dispose();

            factory = new TransliterationApiWebApplicationFactory();
            string? cacheDirectory = Path.GetDirectoryName(factory.CacheStoreLocation);
            Directory.CreateDirectory(cacheDirectory!);
            File.WriteAllText(factory.CacheStoreLocation, persistedCache);
            factory.HttpRequestManager.ResponseToReturn = "ack:::maykuape";
            client = factory.CreateClient();

            using HttpResponseMessage secondResponse = await client.GetAsync(BuildEndpoint(ExternalText, ExternalLanguageCode));
            using JsonDocument secondBody = await ReadResponseBody(secondResponse);

            Assert.Multiple(() =>
            {
                Assert.That(firstResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(secondResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(secondBody.RootElement.GetProperty("text").GetString(), Is.EqualTo("nalshchik"));
                Assert.That(factory.HttpRequestManager.TotalPostInvocationCount, Is.Zero);
            });
        }

        [Test]
        public async Task GivenCachingIsDisabledAndTheStoreIsCorrupt_WhenRequestingTransliteration_ThenTheStoreIsBypassed()
        {
            client.Dispose();
            factory.Dispose();
            factory = new TransliterationApiWebApplicationFactory(false);
            factory.HttpRequestManager.ResponseToReturn = ProviderResponse;
            client = factory.CreateClient();
            File.WriteAllText(factory.CacheStoreLocation, "not valid JSON");

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(ExternalText, ExternalLanguageCode));
            using JsonDocument responseBody = await ReadResponseBody(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseBody.RootElement.GetProperty("text").GetString(), Is.EqualTo("nalshchik"));
                Assert.That(factory.HttpRequestManager.TotalPostInvocationCount, Is.EqualTo(1));
                Assert.That(File.ReadAllText(factory.CacheStoreLocation), Is.EqualTo("not valid JSON"));
            });
        }

        [Test]
        public async Task GivenDifferentTextWithTheSameResult_WhenBothAreTransliterated_ThenDistinctCacheIdentifiersArePersisted()
        {
            using HttpResponseMessage firstResponse = await client.GetAsync(BuildEndpoint("Налщик", ExternalLanguageCode));
            using HttpResponseMessage secondResponse = await client.GetAsync(BuildEndpoint("Мэйкъуапэ", ExternalLanguageCode));
            using JsonDocument cache = JsonDocument.Parse(File.ReadAllText(factory.CacheStoreLocation));
            string?[] identifiers = cache.RootElement
                .EnumerateArray()
                .Select(cacheEntry => cacheEntry.GetProperty("id").GetString())
                .ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(firstResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(secondResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(identifiers, Has.Length.EqualTo(2));
                Assert.That(identifiers, Is.Unique);
            });
        }

        [Test]
        public async Task GivenASuccessfulTransliteration_WhenInspectingTheCache_ThenRawRequestValuesAreNotPersisted()
        {
            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(ExternalText, ExternalLanguageCode));
            string cacheContent = File.ReadAllText(factory.CacheStoreLocation);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(cacheContent, Does.Not.Contain(ExternalText));
                Assert.That(cacheContent, Does.Not.Contain(ExternalLanguageCode));
                Assert.That(cacheContent, Does.Contain("nalshchik"));
            });
        }

        private int ReadCacheEntryCount()
        {
            using JsonDocument cache = JsonDocument.Parse(File.ReadAllText(factory.CacheStoreLocation));

            return cache.RootElement.GetArrayLength();
        }

        private static string BuildEndpoint(string text, string languageCode)
            => $"/transliteration?text={Uri.EscapeDataString(text)}&language={Uri.EscapeDataString(languageCode)}";

        private static async Task<JsonDocument> ReadResponseBody(HttpResponseMessage response)
        {
            string responseBody = await response.Content.ReadAsStringAsync();

            return JsonDocument.Parse(responseBody);
        }
    }
}