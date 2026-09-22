using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using NUnit.Framework;

using TransliterationAPI.API.Responses;
using TransliterationAPI.IntegrationTests.Infrastructure;
using TransliterationAPI.Service.Entities;

namespace TransliterationAPI.IntegrationTests.API
{
    [TestFixture]
    public sealed class LanguagesEndpointTests
    {
        private static string LanguagesEndpoint => "/languages";

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
        public async Task GivenTheLanguagesEndpoint_WhenRequestingIt_ThenAnOkJsonResponseIsReturned()
        {
            using HttpResponseMessage response = await client.GetAsync(LanguagesEndpoint);
            string responseBody = await response.Content.ReadAsStringAsync();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), responseBody);
                Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
            });
        }

        [Test]
        public async Task GivenTheLanguagesEndpoint_WhenRequestingIt_ThenEveryLanguageIsReturnedInCodeOrder()
        {
            using HttpResponseMessage response = await client.GetAsync(LanguagesEndpoint);
            using JsonDocument responseBody = await ReadResponseBody(response);

            JsonElement languages = responseBody.RootElement.GetProperty("languages");
            string[] expectedCodes = Language.GetAll()
                .OrderBy(language => language.Code)
                .Select(language => language.Code)
                .ToArray();
            string?[] actualCodes = languages
                .EnumerateArray()
                .Select(language => language.GetProperty("code").GetString())
                .ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(actualCodes, Is.EqualTo(expectedCodes));
                Assert.That(responseBody.RootElement.GetProperty("count").GetInt32(), Is.EqualTo(expectedCodes.Length));
            });
        }

        [Test]
        public async Task GivenTheLanguagesEndpoint_WhenRequestingIt_ThenEveryLanguageContainsItsPublicContract()
        {
            using HttpResponseMessage response = await client.GetAsync(LanguagesEndpoint);
            using JsonDocument responseBody = await ReadResponseBody(response);

            JsonElement[] actualLanguages = responseBody.RootElement
                .GetProperty("languages")
                .EnumerateArray()
                .ToArray();
            Language[] expectedLanguages = Language.GetAll()
                .OrderBy(language => language.Code)
                .ToArray();

            Assert.That(actualLanguages, Has.Length.EqualTo(expectedLanguages.Length));

            for (int languageIndex = 0; languageIndex < expectedLanguages.Length; languageIndex += 1)
            {
                JsonElement actualLanguage = actualLanguages[languageIndex];
                Language expectedLanguage = expectedLanguages[languageIndex];
                string[] propertyNames = actualLanguage
                    .EnumerateObject()
                    .Select(property => property.Name)
                    .ToArray();
                string[] expectedPropertyNames = ["code", "name", "transliterator"];

                Assert.Multiple(() =>
                {
                    Assert.That(propertyNames, Is.EquivalentTo(expectedPropertyNames));
                    Assert.That(actualLanguage.GetProperty("code").GetString(), Is.EqualTo(expectedLanguage.Code));
                    Assert.That(actualLanguage.GetProperty("name").GetString(), Is.EqualTo(expectedLanguage.Name));
                    Assert.That(actualLanguage.GetProperty("transliterator").GetString(), Is.EqualTo(expectedLanguage.Transliterator));
                });
            }
        }

        [Test]
        public async Task GivenTheLanguagesEndpoint_WhenRequestingIt_ThenTheSuccessEnvelopeAndHmacAreValid()
        {
            using HttpResponseMessage response = await client.GetAsync(LanguagesEndpoint);
            using JsonDocument responseBody = await ReadResponseBody(response);
            JsonElement root = responseBody.RootElement;
            GetLanguagesResponse responseForValidation = new()
            {
                Languages = [.. Language.GetAll().OrderBy(language => language.Code)],
                HmacToken = root.GetProperty("hmac").GetString()!
            };

            Assert.Multiple(() =>
            {
                Assert.That(root.GetProperty("success").GetBoolean());
                Assert.That(root.GetProperty("message").GetString(), Is.EqualTo("Operation completed successfully."));
                Assert.That(root.GetProperty("code").GetString(), Is.EqualTo("SUCCESS"));
                Assert.That(responseForValidation.HasValidHMAC(TransliterationApiWebApplicationFactory.HmacSigningKey));
                Assert.That(responseForValidation.HasValidHMAC("P@ssw0rd!"), Is.False);
            });
        }

        [Test]
        public async Task GivenAValidLanguagesHmac_WhenThePayloadIsReordered_ThenTheHmacIsInvalid()
        {
            using HttpResponseMessage response = await client.GetAsync(LanguagesEndpoint);
            using JsonDocument responseBody = await ReadResponseBody(response);
            GetLanguagesResponse responseForValidation = new()
            {
                Languages = [.. Language.GetAll().OrderByDescending(language => language.Code)],
                HmacToken = responseBody.RootElement.GetProperty("hmac").GetString()!
            };

            Assert.That(
                responseForValidation.HasValidHMAC(TransliterationApiWebApplicationFactory.HmacSigningKey),
                Is.False);
        }

        [Test]
        public async Task GivenTheLanguagesEndpoint_WhenRequestingItRepeatedly_ThenThePayloadIsDeterministic()
        {
            using HttpResponseMessage firstResponse = await client.GetAsync(LanguagesEndpoint);
            using HttpResponseMessage secondResponse = await client.GetAsync(LanguagesEndpoint);
            string firstBody = await firstResponse.Content.ReadAsStringAsync();
            string secondBody = await secondResponse.Content.ReadAsStringAsync();

            Assert.That(secondBody, Is.EqualTo(firstBody));
        }

        [Test]
        public async Task GivenTheLanguagesEndpoint_WhenRequestingIt_ThenCodesAreUniqueAndVariantNamesAreExplicit()
        {
            using HttpResponseMessage response = await client.GetAsync(LanguagesEndpoint);
            using JsonDocument responseBody = await ReadResponseBody(response);
            JsonElement[] languages = responseBody.RootElement.GetProperty("languages").EnumerateArray().ToArray();
            string?[] codes = languages.Select(language => language.GetProperty("code").GetString()).ToArray();
            string?[] names = languages.Select(language => language.GetProperty("name").GetString()).ToArray();
            string?[] duplicateNames = names
                .GroupBy(name => name)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .OrderBy(name => name)
                .ToArray();
            string[] expectedDuplicateNames = ["Serbian", "Tajik", "Tatar"];

            Assert.Multiple(() =>
            {
                Assert.That(codes, Is.Unique);
                Assert.That(codes, Has.None.Null.Or.Empty);
                Assert.That(names, Has.None.Null.Or.Empty);
                Assert.That(duplicateNames, Is.EqualTo(expectedDuplicateNames));
            });
        }

        private static async Task<JsonDocument> ReadResponseBody(HttpResponseMessage response)
        {
            string responseBody = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), responseBody);

            return JsonDocument.Parse(responseBody);
        }
    }
}