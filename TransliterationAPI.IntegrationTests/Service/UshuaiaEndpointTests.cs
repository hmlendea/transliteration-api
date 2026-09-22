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
    public sealed class UshuaiaEndpointTests
    {
        private static string CookieEndpoint => "https://www.ushuaia.pl/transliterate/";
        private static string ProviderEndpoint => "https://www.ushuaia.pl/transliterate/transliterate.php";

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
        public async Task GivenAnUshuaiaLanguage_WhenRequestingTransliteration_ThenTheExpectedProviderRequestIsSent(
            string languageCode,
            string text,
            string providerResponse,
            string expectedText,
            string expectedProviderLanguage)
        {
            factory.HttpRequestManager.ResponseToReturn = providerResponse;

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(text, languageCode));
            using JsonDocument responseBody = await ReadResponseBody(response);
            RecordingHttpRequestManager manager = factory.HttpRequestManager;

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseBody.RootElement.GetProperty("text").GetString(), Is.EqualTo(expectedText));
                Assert.That(manager.PostInvocationCount, Is.Zero);
                Assert.That(manager.PostWithHeadersInvocationCount, Is.EqualTo(1));
                Assert.That(manager.RetrieveCookiesInvocationCount, Is.EqualTo(1));
                Assert.That(manager.LastCookieRetrievalUrl, Is.EqualTo(CookieEndpoint));
                Assert.That(manager.LastPostUrl, Is.EqualTo(ProviderEndpoint));
                Assert.That(manager.LastFormData, Has.Count.EqualTo(2));
                Assert.That(manager.LastFormData!["text"], Is.EqualTo(text));
                Assert.That(manager.LastFormData["lang"], Is.EqualTo(expectedProviderLanguage));
                Assert.That(manager.LastHeaders, Has.Count.EqualTo(1));
                Assert.That(
                    manager.LastHeaders!["Cookie"],
                    Is.EqualTo($"translit=Test1234!;lastlang={expectedProviderLanguage}"));
            });
        }

        [Test]
        [TestCase("translit=Test1234!;other=value", "translit=Test1234!;lastlang=bengali_iso_transliterate")]
        [TestCase("translit=NucileRullz!", "translit=NucileRullz!;lastlang=bengali_iso_transliterate")]
        [TestCase("translit=;other=value", "translit=;lastlang=bengali_iso_transliterate")]
        [TestCase("", "translit=;lastlang=bengali_iso_transliterate")]
        [TestCase("other=value", "translit=other=value;lastlang=bengali_iso_transliterate")]
        [TestCase("other=value;translit=Silver56;path=/", "translit=other=value;Silver56;lastlang=bengali_iso_transliterate")]
        public async Task GivenAnyCookieResponse_WhenRequestingTransliteration_ThenItsParsedValueIsSent(
            string cookieResponse,
            string expectedCookieHeader)
        {
            factory.HttpRequestManager.CookiesToReturn = cookieResponse;
            factory.HttpRequestManager.ResponseToReturn = "dhaka";

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint("ঢাকা", "bn"));

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(factory.HttpRequestManager.LastHeaders!["Cookie"], Is.EqualTo(expectedCookieHeader));
            });
        }

        [Test]
        public async Task GivenAnExistingProviderSession_WhenRequestingDifferentText_ThenTheCookieIsReused()
        {
            factory.HttpRequestManager.ResponseToReturn = "dhaka";

            using HttpResponseMessage firstResponse = await client.GetAsync(BuildEndpoint("ঢাকা", "bn"));
            factory.HttpRequestManager.ResponseToReturn = "chattagram";

            using HttpResponseMessage secondResponse = await client.GetAsync(BuildEndpoint("চট্টগ্রাম", "bn"));
            using JsonDocument secondBody = await ReadResponseBody(secondResponse);

            Assert.Multiple(() =>
            {
                Assert.That(firstResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(secondResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(secondBody.RootElement.GetProperty("text").GetString(), Is.EqualTo("Chattagram"));
                Assert.That(factory.HttpRequestManager.RetrieveCookiesInvocationCount, Is.EqualTo(1));
                Assert.That(factory.HttpRequestManager.PostWithHeadersInvocationCount, Is.EqualTo(2));
                Assert.That(ReadCacheEntryCount(), Is.EqualTo(2));
            });
        }

        [Test]
        public async Task GivenCookieRetrievalFails_WhenRequestingTransliteration_ThenNullIsReturnedWithoutPostingOrCaching()
        {
            factory.HttpRequestManager.ExceptionToThrow = new HttpRequestException("Cookie service unavailable.");

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint("ঢাকা", "bn"));
            using JsonDocument responseBody = await ReadResponseBody(response);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseBody.RootElement.GetProperty("text").ValueKind, Is.EqualTo(JsonValueKind.Null));
                Assert.That(factory.HttpRequestManager.RetrieveCookiesInvocationCount, Is.EqualTo(1));
                Assert.That(factory.HttpRequestManager.PostWithHeadersInvocationCount, Is.Zero);
                Assert.That(ReadCacheEntryCount(), Is.Zero);
            });
        }

        [Test]
        public async Task GivenProviderPostingFailsAfterSessionAcquisition_WhenRequestingNewText_ThenNullIsReturnedWithoutCachingIt()
        {
            factory.HttpRequestManager.ResponseToReturn = "dhaka";

            using HttpResponseMessage successfulResponse = await client.GetAsync(BuildEndpoint("ঢাকা", "bn"));
            factory.HttpRequestManager.ExceptionToThrow = new HttpRequestException("Provider unavailable.");

            using HttpResponseMessage failedResponse = await client.GetAsync(BuildEndpoint("চট্টগ্রাম", "bn"));
            using JsonDocument failedBody = await ReadResponseBody(failedResponse);

            Assert.Multiple(() =>
            {
                Assert.That(successfulResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(failedResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(failedBody.RootElement.GetProperty("text").ValueKind, Is.EqualTo(JsonValueKind.Null));
                Assert.That(factory.HttpRequestManager.RetrieveCookiesInvocationCount, Is.EqualTo(1));
                Assert.That(factory.HttpRequestManager.PostWithHeadersInvocationCount, Is.EqualTo(2));
                Assert.That(ReadCacheEntryCount(), Is.EqualTo(1));
            });
        }

        [Test]
        [TestCase("srinagar", "Sarinagar")]
        [TestCase("Srinagar", "Sarinagar")]
        [TestCase("wn", "Wan")]
        [TestCase("Wn", "Wan")]
        [TestCase("sr wn", "Sar Wan")]
        [TestCase("plain", "Plain")]
        public async Task GivenAHindiProviderResult_WhenRequestingTransliteration_ThenHindiPostProcessingIsApplied(
            string providerResponse,
            string expectedText)
        {
            factory.HttpRequestManager.ResponseToReturn = providerResponse;

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint("श्रीनगर", "hi"));
            using JsonDocument responseBody = await ReadResponseBody(response);

            Assert.That(responseBody.RootElement.GetProperty("text").GetString(), Is.EqualTo(expectedText));
        }

        private int ReadCacheEntryCount()
        {
            using JsonDocument cache = JsonDocument.Parse(File.ReadAllText(factory.CacheStoreLocation));

            return cache.RootElement.GetArrayLength();
        }

        private static IEnumerable<TestCaseData> LanguageRequestScenarios()
        {
            yield return BuildLanguageTestCase("bn", "চট্টগ্রাম", "chattagram", "Chattagram", "bengali_iso_transliterate");
            yield return BuildLanguageTestCase("hi", "श्रीनगर", "srinagar", "Sarinagar", "devanagari_hunt_transcribe");
            yield return BuildLanguageTestCase("kn", "ಮೈಸೂರು", "mysuru", "Mysuru", "kannada_iso_transliterate");
            yield return BuildLanguageTestCase("ml", "തിരുവനന്തപുരം", "thiruvananthapuram", "Thiruvananthapuram", "malayalam_iso_transliterate");
            yield return BuildLanguageTestCase("mn", "Дархан", "darkhan", "darkhan", "mongolian_mns_transliterate");
            yield return BuildLanguageTestCase("sa", "प्रयागराज", "prayagraj", "Prayagraj", "devanagari_iast_transliterate");
            yield return BuildLanguageTestCase("si", "ගාල්ල", "galle", "Galle", "sinhala_iso_transliterate");
            yield return BuildLanguageTestCase("ta", "மதுரை", "madurai", "Madurai", "tamil_iso_transliterate");
            yield return BuildLanguageTestCase("te", "విజయవాడ", "vijayawada", "Vijayawada", "telugu_iso_transliterate");
        }

        private static TestCaseData BuildLanguageTestCase(
            string languageCode,
            string text,
            string providerResponse,
            string expectedText,
            string expectedProviderLanguage)
            => new TestCaseData(languageCode, text, providerResponse, expectedText, expectedProviderLanguage)
                .SetName($"GivenThe{languageCode}Language_WhenRequestingTransliteration_ThenTheExpectedUshuaiaRequestIsSent");

        private static string BuildEndpoint(string text, string languageCode)
            => $"/transliteration?text={Uri.EscapeDataString(text)}&language={Uri.EscapeDataString(languageCode)}";

        private static async Task<JsonDocument> ReadResponseBody(HttpResponseMessage response)
        {
            string responseBody = await response.Content.ReadAsStringAsync();

            return JsonDocument.Parse(responseBody);
        }
    }
}