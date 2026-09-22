using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

using NUnit.Framework;

using TransliterationAPI.API.Responses;
using TransliterationAPI.IntegrationTests.Infrastructure;

namespace TransliterationAPI.IntegrationTests.API
{
    [TestFixture]
    public sealed class TransliterationEndpointTests
    {
        private static readonly JsonSerializerOptions SerialisationOptions = new(JsonSerializerDefaults.Web);

        private static string DefaultLanguageCode => "ru";
        private static string DefaultResult => "Solaire of Astora";
        private static string DefaultText => "Hello, World!";
        private static string TransliterationEndpoint => "/transliteration";

        private HttpClient client = null!;
        private TransliterationApiWebApplicationFactory factory = null!;
        private RecordingTransliterationService transliterationService = null!;

        [SetUp]
        public void SetUp()
        {
            transliterationService = new RecordingTransliterationService
            {
                ResultToReturn = DefaultResult
            };
            factory = new TransliterationApiWebApplicationFactory(transliterationService);
            client = factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            client.Dispose();
            factory.Dispose();
        }

        [Test]
        [TestCase("Hello, World!", "ru")]
        [TestCase("Crăciun Fericit!", "ro")]
        [TestCase("C'est la vie", "fr")]
        [TestCase("e=mc²", "el")]
        [TestCase("Аҟәа", "ab")]
        [TestCase("Անի", "hy")]
        [TestCase("أبيدوس", "ar")]
        [TestCase("תל-אביב", "he")]
        [TestCase("ⲁⲗⲉⲝⲁⲛⲇⲣⲓⲁ", "cop")]
        [TestCase("京都", "ja")]
        [TestCase("거제", "ko")]
        [TestCase("A&B = C+D? #1/2", "unknown/value")]
        [TestCase("Line one\r\nLine two\tTabbed", "ru")]
        [TestCase("😀 🌞 🚀", "emoji")]
        public async Task GivenDiverseQueryValues_WhenRequestingTransliteration_ThenTheDecodedValuesReachTheService(
            string text,
            string languageCode)
        {
            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(text, languageCode));

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(transliterationService.InvocationCount, Is.EqualTo(1));
                Assert.That(transliterationService.LastText, Is.EqualTo(text));
                Assert.That(transliterationService.LastLanguageCode, Is.EqualTo(languageCode));
            });
        }

        [Test]
        [TestCase(1)]
        [TestCase(255)]
        [TestCase(256)]
        public async Task GivenTextAtOrBelowTheMaximumLength_WhenRequestingTransliteration_ThenTheRequestIsAccepted(
            int textLength)
        {
            string text = new('a', textLength);

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(text, DefaultLanguageCode));

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(transliterationService.InvocationCount, Is.EqualTo(1));
                Assert.That(transliterationService.LastText, Has.Length.EqualTo(textLength));
            });
        }

        [Test]
        [TestCase(257)]
        [TestCase(512)]
        [TestCase(1024)]
        public async Task GivenTextAboveTheMaximumLength_WhenRequestingTransliteration_ThenAValidationProblemIsReturned(
            int textLength)
        {
            string text = new('a', textLength);

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(text, DefaultLanguageCode));
            using JsonDocument responseBody = await ReadResponseBody(response);
            JsonElement errors = responseBody.RootElement.GetProperty("errors");

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/problem+json"));
                Assert.That(responseBody.RootElement.GetProperty("status").GetInt32(), Is.EqualTo(400));
                Assert.That(errors.GetProperty("Text")[0].GetString(), Is.EqualTo("The text cannot exceed 256 characters."));
                Assert.That(transliterationService.InvocationCount, Is.Zero);
            });
        }

        [Test]
        [TestCase(127, HttpStatusCode.OK)]
        [TestCase(128, HttpStatusCode.OK)]
        [TestCase(129, HttpStatusCode.BadRequest)]
        public async Task GivenSupplementaryUnicodeTextNearTheUtf16Limit_WhenRequestingTransliteration_ThenTheCodeUnitLimitIsApplied(
            int symbolCount,
            HttpStatusCode expectedStatusCode)
        {
            string text = string.Concat(Enumerable.Repeat("😀", symbolCount));

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(text, DefaultLanguageCode));

            Assert.That(response.StatusCode, Is.EqualTo(expectedStatusCode));
        }

        [Test]
        [TestCase("/transliteration", null, null)]
        [TestCase("/transliteration?text=", null, null)]
        [TestCase("/transliteration?language=", null, null)]
        [TestCase("/transliteration?text=Hello%2C%20World!", "Hello, World!", null)]
        [TestCase("/transliteration?language=ru", null, "ru")]
        public async Task GivenAbsentOrEmptyQueryValues_WhenRequestingTransliteration_ThenTheBoundValuesReachTheService(
            string endpoint,
            string? expectedText,
            string? expectedLanguageCode)
        {
            using HttpResponseMessage response = await client.GetAsync(endpoint);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(transliterationService.InvocationCount, Is.EqualTo(1));
                Assert.That(transliterationService.LastText, Is.EqualTo(expectedText));
                Assert.That(transliterationService.LastLanguageCode, Is.EqualTo(expectedLanguageCode));
            });
        }

        [Test]
        [TestCase("TEXT", "LANGUAGE")]
        [TestCase("Text", "Language")]
        [TestCase("tExT", "lAnGuAgE")]
        [TestCase("text", "language")]
        public async Task GivenQueryKeysWithAnyCasing_WhenRequestingTransliteration_ThenTheValuesAreBound(
            string textKey,
            string languageKey)
        {
            string endpoint = $"{TransliterationEndpoint}?{textKey}=Hello%2C%20World!&{languageKey}=ru";

            using HttpResponseMessage response = await client.GetAsync(endpoint);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(transliterationService.LastText, Is.EqualTo(DefaultText));
                Assert.That(transliterationService.LastLanguageCode, Is.EqualTo(DefaultLanguageCode));
            });
        }

        [Test]
        public async Task GivenDuplicateQueryKeys_WhenRequestingTransliteration_ThenTheFirstValuesAreBound()
        {
            string endpoint = $"{TransliterationEndpoint}?text=Hello%2C%20World!&text=Alt%C4%83%20%C3%AEntrebare&language=ru&language=uk";

            using HttpResponseMessage response = await client.GetAsync(endpoint);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(transliterationService.LastText, Is.EqualTo(DefaultText));
                Assert.That(transliterationService.LastLanguageCode, Is.EqualTo(DefaultLanguageCode));
            });
        }

        [Test]
        public async Task GivenUnrelatedQueryValues_WhenRequestingTransliteration_ThenTheyAreIgnored()
        {
            string endpoint = $"{BuildEndpoint(DefaultText, DefaultLanguageCode)}&username=Angetenar&country=Nucilandia";

            using HttpResponseMessage response = await client.GetAsync(endpoint);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(transliterationService.InvocationCount, Is.EqualTo(1));
                Assert.That(transliterationService.LastText, Is.EqualTo(DefaultText));
                Assert.That(transliterationService.LastLanguageCode, Is.EqualTo(DefaultLanguageCode));
            });
        }

        [Test]
        [TestCase("")]
        [TestCase("Hello, World!")]
        [TestCase("Crăciun Fericit!")]
        [TestCase("Αθήνα")]
        [TestCase("京都")]
        [TestCase("😀")]
        [TestCase("\"quoted\"\\path\r\nline")]
        [TestCase(null)]
        public async Task GivenAnyServiceResult_WhenRequestingTransliteration_ThenTheResultIsSerialisedExactly(
            string? expectedResult)
        {
            transliterationService.ResultToReturn = expectedResult;

            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(DefaultText, DefaultLanguageCode));
            using JsonDocument responseBody = await ReadResponseBody(response);
            JsonElement text = responseBody.RootElement.GetProperty("text");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            if (expectedResult is null)
            {
                Assert.That(text.ValueKind, Is.EqualTo(JsonValueKind.Null));

                return;
            }

            Assert.That(text.GetString(), Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task GivenASuccessfulTransliteration_WhenReadingTheResponse_ThenTheSuccessEnvelopeIsComplete()
        {
            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(DefaultText, DefaultLanguageCode));
            using JsonDocument responseBody = await ReadResponseBody(response);
            JsonElement root = responseBody.RootElement;

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
                Assert.That(root.GetProperty("success").GetBoolean());
                Assert.That(root.GetProperty("message").GetString(), Is.EqualTo("Operation completed successfully."));
                Assert.That(root.GetProperty("code").GetString(), Is.EqualTo("SUCCESS"));
                Assert.That(root.GetProperty("hmac").GetString(), Is.Not.Null.And.Not.Empty);
                Assert.That(root.GetProperty("text").GetString(), Is.EqualTo(DefaultResult));
            });
        }

        [Test]
        public async Task GivenIdenticalResponses_WhenRequestingTransliterationRepeatedly_ThenTheHmacIsDeterministic()
        {
            using HttpResponseMessage firstResponse = await client.GetAsync(BuildEndpoint(DefaultText, DefaultLanguageCode));
            using HttpResponseMessage secondResponse = await client.GetAsync(BuildEndpoint(DefaultText, DefaultLanguageCode));
            using JsonDocument firstBody = await ReadResponseBody(firstResponse);
            using JsonDocument secondBody = await ReadResponseBody(secondResponse);

            string? firstHmac = firstBody.RootElement.GetProperty("hmac").GetString();
            string? secondHmac = secondBody.RootElement.GetProperty("hmac").GetString();

            Assert.Multiple(() =>
            {
                Assert.That(firstHmac, Is.Not.Null.And.Not.Empty);
                Assert.That(secondHmac, Is.EqualTo(firstHmac));
                Assert.That(transliterationService.InvocationCount, Is.EqualTo(2));
            });
        }

        [Test]
        public async Task GivenDifferentResponses_WhenRequestingTransliteration_ThenTheHmacChanges()
        {
            using HttpResponseMessage firstResponse = await client.GetAsync(BuildEndpoint(DefaultText, DefaultLanguageCode));
            using JsonDocument firstBody = await ReadResponseBody(firstResponse);
            transliterationService.ResultToReturn = "Praise the Sun!";

            using HttpResponseMessage secondResponse = await client.GetAsync(BuildEndpoint(DefaultText, DefaultLanguageCode));
            using JsonDocument secondBody = await ReadResponseBody(secondResponse);

            Assert.That(
                secondBody.RootElement.GetProperty("hmac").GetString(),
                Is.Not.EqualTo(firstBody.RootElement.GetProperty("hmac").GetString()));
        }

        [Test]
        public async Task GivenASuccessfulTransliteration_WhenValidatingItsHmac_ThenTheCorrectKeyIsAccepted()
        {
            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(DefaultText, DefaultLanguageCode));
            string responseBody = await response.Content.ReadAsStringAsync();
            GetTransliterationResponse? responseForValidation = JsonSerializer.Deserialize<GetTransliterationResponse>(
                responseBody,
                SerialisationOptions);

            Assert.That(responseForValidation, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(responseForValidation!.HasValidHMAC(TransliterationApiWebApplicationFactory.HmacSigningKey));
                Assert.That(responseForValidation.HasValidHMAC("P@ssw0rd!"), Is.False);
            });
        }

        [Test]
        public async Task GivenAValidTransliterationHmac_WhenTheTextIsModified_ThenTheHmacIsInvalid()
        {
            using HttpResponseMessage response = await client.GetAsync(BuildEndpoint(DefaultText, DefaultLanguageCode));
            string responseBody = await response.Content.ReadAsStringAsync();
            GetTransliterationResponse? responseForValidation = JsonSerializer.Deserialize<GetTransliterationResponse>(
                responseBody,
                SerialisationOptions);

            Assert.That(responseForValidation, Is.Not.Null);
            responseForValidation!.Text = "Praise the Sun!";

            Assert.That(
                responseForValidation.HasValidHMAC(TransliterationApiWebApplicationFactory.HmacSigningKey),
                Is.False);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Bearer Test1234!")]
        [TestCase("Basic VGVzdDEyMzQh")]
        [TestCase("malformed")]
        public async Task GivenAnyAuthorisationHeader_WhenRequestingTransliteration_ThenAnonymousAccessIsPermitted(
            string? authorisationHeader)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, BuildEndpoint(DefaultText, DefaultLanguageCode));

            if (authorisationHeader is not null)
            {
                request.Headers.TryAddWithoutValidation("Authorization", authorisationHeader);
            }

            using HttpResponseMessage response = await client.SendAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(transliterationService.InvocationCount, Is.EqualTo(1));
            });
        }

        [Test]
        [TestCase("")]
        [TestCase("NucileRullz!")]
        [TestCase("%2B%2F%3D")]
        [TestCase("malformed hmac")]
        public async Task GivenAnyRequestHmacHeader_WhenRequestingTransliteration_ThenTheUnsignedEndpointAcceptsIt(
            string hmacHeader)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, BuildEndpoint(DefaultText, DefaultLanguageCode));
            request.Headers.TryAddWithoutValidation("X-HMAC", hmacHeader);

            using HttpResponseMessage response = await client.SendAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(transliterationService.InvocationCount, Is.EqualTo(1));
            });
        }

        private static string BuildEndpoint(string text, string languageCode)
            => $"{TransliterationEndpoint}?text={Uri.EscapeDataString(text)}&language={Uri.EscapeDataString(languageCode)}";

        private static async Task<JsonDocument> ReadResponseBody(HttpResponseMessage response)
        {
            string responseBody = await response.Content.ReadAsStringAsync();

            return JsonDocument.Parse(responseBody);
        }
    }
}