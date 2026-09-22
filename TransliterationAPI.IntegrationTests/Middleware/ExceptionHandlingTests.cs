using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using NuciAPI.Middleware.Security;

using NuciDAL.Repositories;

using NUnit.Framework;

using TransliterationAPI.IntegrationTests.Infrastructure;

namespace TransliterationAPI.IntegrationTests.Middleware
{
    [TestFixture]
    public sealed class ExceptionHandlingTests
    {
        private static string Endpoint => "/transliteration?text=Hello%2C%20World!&language=ru";

        private HttpClient client = null!;
        private TransliterationApiWebApplicationFactory factory = null!;
        private RecordingTransliterationService transliterationService = null!;

        [SetUp]
        public void SetUp()
        {
            transliterationService = new RecordingTransliterationService();
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
        [TestCaseSource(nameof(ErrorScenarios))]
        public async Task GivenAServiceException_WhenRequestingTransliteration_ThenTheMappedErrorResponseIsReturned(
            Exception exception,
            int expectedStatusCode,
            string expectedCode,
            string expectedMessage)
        {
            transliterationService.ExceptionToThrow = exception;

            using HttpResponseMessage response = await client.GetAsync(Endpoint);
            using JsonDocument responseBody = await ReadResponseBody(response);
            JsonElement root = responseBody.RootElement;

            Assert.Multiple(() =>
            {
                Assert.That((int)response.StatusCode, Is.EqualTo(expectedStatusCode));
                Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
                Assert.That(root.GetProperty("success").GetBoolean(), Is.False);
                Assert.That(root.GetProperty("message").GetString(), Is.EqualTo(expectedMessage));
                Assert.That(root.GetProperty("code").GetString(), Is.EqualTo(expectedCode));
                Assert.That(root.GetProperty("hmac").ValueKind, Is.EqualTo(JsonValueKind.Null));
                Assert.That(root.TryGetProperty("text", out JsonElement _), Is.False);
                Assert.That(transliterationService.InvocationCount, Is.EqualTo(1));
            });
        }

        private static IEnumerable<TestCaseData> ErrorScenarios()
        {
            yield return BuildErrorScenario(
                new BadHttpRequestException("The request body is invalid."),
                400,
                "BAD_REQUEST",
                "The request body is invalid.");
            yield return BuildErrorScenario(
                new FormatException("The language format is invalid."),
                400,
                "BAD_REQUEST",
                "The language format is invalid.");
            yield return BuildErrorScenario(
                new ArgumentException("The language is invalid."),
                400,
                "BAD_REQUEST",
                "The language is invalid.");
            yield return BuildErrorScenario(
                new ValidationException("The text is invalid."),
                400,
                "BAD_REQUEST",
                "The text is invalid.");
            yield return BuildErrorScenario(
                new SecurityException("Sensitive detail."),
                403,
                "UNAUTHORISED",
                "You do not have the required permission to perform this action.");
            yield return BuildErrorScenario(
                new UnauthorizedAccessException("Sensitive detail."),
                403,
                "UNAUTHORISED",
                "You do not have the required permission to perform this action.");
            yield return BuildErrorScenario(
                new HttpRequestException("Sensitive dependency detail."),
                503,
                "SERVICE_DEPENDENCY_UNAVAILABLE",
                "A service dependency is currently unavailable.");
            yield return BuildErrorScenario(
                new TaskCanceledException("Sensitive dependency detail."),
                503,
                "SERVICE_DEPENDENCY_UNAVAILABLE",
                "A service dependency is currently unavailable.");
            yield return BuildErrorScenario(
                new TimeoutException("Sensitive dependency detail."),
                503,
                "SERVICE_DEPENDENCY_UNAVAILABLE",
                "A service dependency is currently unavailable.");
            yield return BuildErrorScenario(
                new AuthenticationException("Sensitive authentication detail."),
                401,
                "AUTHENTICATION_FAILURE",
                "The authentication has failed.");
            yield return BuildErrorScenario(
                new EntityNotFoundException("42", "CachedTransliteration"),
                404,
                "NOT_FOUND",
                "The requested resource was not found.");
            yield return BuildErrorScenario(
                new KeyNotFoundException("Sensitive entity detail."),
                404,
                "NOT_FOUND",
                "The requested resource was not found.");
            yield return BuildErrorScenario(
                new NotImplementedException("The requested alphabet is not implemented."),
                501,
                "NOT_IMPLEMENTED",
                "The requested alphabet is not implemented.");
            yield return BuildErrorScenario(
                new OperationCanceledException("Sensitive cancellation detail."),
                499,
                "CLIENT_CLOSED_THE_REQUEST",
                "The client has closed the request.");
            yield return BuildErrorScenario(
                new InvalidOperationException("Sensitive internal detail."),
                500,
                "INTERNAL_SERVER_ERROR",
                "An internal server error has occurred.");
            yield return BuildErrorScenario(
                new EntityAlreadyExistsException("42", "CachedTransliteration"),
                409,
                "ALREADY_EXISTS",
                "The requested resource already exists.");
            yield return BuildErrorScenario(
                new RequestAlreadyProcessedException("42"),
                409,
                "ALREADY_PROCESSED",
                "The request has already been processed.");
        }

        private static TestCaseData BuildErrorScenario(
            Exception exception,
            int expectedStatusCode,
            string expectedCode,
            string expectedMessage)
            => new TestCaseData(exception, expectedStatusCode, expectedCode, expectedMessage)
                .SetName($"GivenA{exception.GetType().Name}_WhenRequestingTransliteration_ThenTheMappedErrorResponseIsReturned");

        private static async Task<JsonDocument> ReadResponseBody(HttpResponseMessage response)
        {
            string responseBody = await response.Content.ReadAsStringAsync();

            return JsonDocument.Parse(responseBody);
        }
    }
}