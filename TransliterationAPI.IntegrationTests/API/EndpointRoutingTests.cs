using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using NUnit.Framework;

using TransliterationAPI.IntegrationTests.Infrastructure;

namespace TransliterationAPI.IntegrationTests.API
{
    [TestFixture]
    public sealed class EndpointRoutingTests
    {
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
        [TestCase("/languages")]
        [TestCase("/Languages")]
        [TestCase("/LANGUAGES")]
        [TestCase("/LaNgUaGeS")]
        [TestCase("/languages/")]
        [TestCase("/languages?username=Angetenar")]
        [TestCase("/transliteration?text=Hello%2C%20World!&language=ru")]
        [TestCase("/Transliteration?text=Hello%2C%20World!&language=ru")]
        [TestCase("/TRANSLITERATION?text=Hello%2C%20World!&language=ru")]
        [TestCase("/TrAnSlItErAtIoN/?text=Hello%2C%20World!&language=ru")]
        public async Task GivenARegisteredRouteVariant_WhenRequestingIt_ThenTheEndpointIsMatched(string endpoint)
        {
            using HttpResponseMessage response = await client.GetAsync(endpoint);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [TestCase("/?username=Angetenar")]
        [TestCase("/missing")]
        [TestCase("/api/languages")]
        [TestCase("/languages/extra")]
        [TestCase("/transliterations")]
        [TestCase("/favicon.ico")]
        [TestCase("/index.html")]
        public async Task GivenAnUnregisteredSafeRoute_WhenRequestingIt_ThenNotFoundIsReturned(string endpoint)
        {
            using HttpResponseMessage response = await client.GetAsync(endpoint);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [TestCaseSource(nameof(UnsupportedMethodScenarios))]
        public async Task GivenAnUnsupportedMethod_WhenRequestingARegisteredRoute_ThenMethodNotAllowedIsReturned(
            HttpMethod method,
            string endpoint)
        {
            using HttpRequestMessage request = new(method, endpoint);
            using HttpResponseMessage response = await client.SendAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
                Assert.That(response.Content.Headers.Allow, Does.Contain("GET"));
                Assert.That(transliterationService.InvocationCount, Is.Zero);
            });
        }

        [Test]
        [TestCase("application/json")]
        [TestCase("application/*")]
        [TestCase("*/*")]
        [TestCase("text/plain")]
        [TestCase("application/xml")]
        public async Task GivenAnyAcceptHeader_WhenRequestingLanguages_ThenJsonIsReturned(string mediaType)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, "/languages");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

            using HttpResponseMessage response = await client.SendAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
            });
        }

        private static IEnumerable<TestCaseData> UnsupportedMethodScenarios()
        {
            HttpMethod[] methods =
            [
                HttpMethod.Post,
                HttpMethod.Put,
                HttpMethod.Patch,
                HttpMethod.Delete,
                HttpMethod.Head,
                HttpMethod.Options,
                HttpMethod.Trace,
                new HttpMethod("CONNECT")
            ];
            string[] endpoints =
            [
                "/languages",
                "/transliteration?text=Hello%2C%20World!&language=ru"
            ];

            foreach (string endpoint in endpoints)
            {
                foreach (HttpMethod method in methods)
                {
                    yield return new TestCaseData(method, endpoint)
                        .SetName($"Given{method.Method}_WhenRequesting{endpoint.Split('?')[0]}_ThenMethodNotAllowedIsReturned");
                }
            }
        }
    }
}