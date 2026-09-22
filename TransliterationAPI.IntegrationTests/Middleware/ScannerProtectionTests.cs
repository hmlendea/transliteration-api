using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using NUnit.Framework;

using TransliterationAPI.IntegrationTests.Infrastructure;

namespace TransliterationAPI.IntegrationTests.Middleware
{
    [TestFixture]
    public sealed class ScannerProtectionTests
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
        [TestCaseSource(nameof(ForbiddenResourcePaths))]
        public async Task GivenAForbiddenResourcePath_WhenRequestingIt_ThenTheClientIsForbidden(string path)
        {
            using HttpResponseMessage response = await client.GetAsync(path);
            string responseBody = await response.Content.ReadAsStringAsync();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
                Assert.That(responseBody, Is.Empty);
                Assert.That(transliterationService.InvocationCount, Is.Zero);
            });
        }

        [Test]
        [TestCase("/transliteration?page=gravitysmtp-settings")]
        [TestCase("/transliteration?PAGE=GRAVITYSMTP-SETTINGS")]
        [TestCase("/transliteration?rest_route=/wp/v2/users")]
        [TestCase("/transliteration?rest_route=/wp/v2/users/")]
        [TestCase("/transliteration?XDEBUG_SESSION_START=phpstorm")]
        [TestCase("/transliteration?xdebug_session_start=PHPSTORM")]
        [TestCase("/transliteration?app_vl=")]
        [TestCase("/transliteration?app_vl=Test1234!")]
        public async Task GivenAForbiddenQueryProbe_WhenRequestingIt_ThenTheClientIsForbidden(string endpoint)
        {
            using HttpResponseMessage response = await client.GetAsync(endpoint);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
                Assert.That(transliterationService.InvocationCount, Is.Zero);
            });
        }

        [Test]
        [TestCase("User-Agent", "Chrome/143.0.0.0")]
        [TestCase("User-Agent", "Mozilla SecurityScanner Client")]
        [TestCase("User-Agent", "internetmeasurement")]
        [TestCase("User-Agent", "OAI-SearchBot/1.0")]
        [TestCase("sec-ch-ua", "Chromium; .Not/A)Brand")]
        [TestCase("From", "oai-searchbot(at)openai.com")]
        [TestCase("From", "OAI-SEARCHBOT(AT)OPENAI.COM")]
        public async Task GivenAForbiddenScannerHeader_WhenRequestingLanguages_ThenTheClientIsForbidden(
            string headerName,
            string headerValue)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, "/languages");
            request.Headers.TryAddWithoutValidation(headerName, headerValue);

            using HttpResponseMessage response = await client.SendAsync(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        [TestCaseSource(nameof(EmptyRootRequestScenarios))]
        public async Task GivenAnEmptyRootRequest_WhenSendingIt_ThenTheClientIsForbidden(HttpMethod method)
        {
            using HttpRequestMessage request = new(method, "/");
            using HttpResponseMessage response = await client.SendAsync(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        [TestCaseSource(nameof(RootRequestsWithContentScenarios))]
        public async Task GivenARootRequestWithContent_WhenSendingIt_ThenTheScannerPermitsRouting(HttpMethod method)
        {
            using HttpRequestMessage request = new(method, "/")
            {
                Content = new StringContent("Hello, World!")
            };

            using HttpResponseMessage response = await client.SendAsync(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [TestCase("/?username=Angetenar")]
        [TestCase("/?country=Nucilandia&city=Hokazuro")]
        public async Task GivenARootGetRequestWithAQuery_WhenSendingIt_ThenTheScannerPermitsRouting(string endpoint)
        {
            using HttpResponseMessage response = await client.GetAsync(endpoint);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GivenAClientThatRequestedAForbiddenResource_WhenRequestingASafeResource_ThenTheIpBanPersists()
        {
            using HttpResponseMessage forbiddenResponse = await client.GetAsync("/.env");
            using HttpResponseMessage subsequentResponse = await client.GetAsync("/languages");

            Assert.Multiple(() =>
            {
                Assert.That(forbiddenResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
                Assert.That(subsequentResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            });
        }

        [Test]
        public async Task GivenABanInAnotherApplicationHost_WhenRequestingASafeResource_ThenTheBanIsIsolated()
        {
            using HttpResponseMessage forbiddenResponse = await client.GetAsync("/.env");
            using TransliterationApiWebApplicationFactory isolatedFactory = new();
            using HttpClient isolatedClient = isolatedFactory.CreateClient();

            using HttpResponseMessage isolatedResponse = await isolatedClient.GetAsync("/languages");

            Assert.Multiple(() =>
            {
                Assert.That(forbiddenResponse.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
                Assert.That(isolatedResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            });
        }

        [Test]
        [TestCase("/configuration")]
        [TestCase("/environment")]
        [TestCase("/appsettings")]
        [TestCase("/wp-admin")]
        [TestCase("/robot.txt")]
        [TestCase("/sitemap")]
        [TestCase("/index.phps")]
        [TestCase("/.environment")]
        [TestCase("/database.txt")]
        [TestCase("/debug.log")]
        [TestCase("/logs/404.txt")]
        public async Task GivenALegitimateNearMissPath_WhenRequestingIt_ThenTheScannerDoesNotForbidIt(string path)
        {
            using HttpResponseMessage response = await client.GetAsync(path);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        private static IEnumerable<string> ForbiddenResourcePaths()
        {
            yield return "/.env";
            yield return "/.env.production";
            yield return "/.env.production.local";
            yield return "/.git/config";
            yield return "/.aws/credentials";
            yield return "/.DS_Store";
            yield return "/.htpasswd";
            yield return "/.npmrc";
            yield return "/.well-known/security.txt";
            yield return "/appsettings.json";
            yield return "/appsettings.Development.json";
            yield return "/appsettings.Production.json";
            yield return "/application-production.yml";
            yield return "/application-prod.json";
            yield return "/application.properties";
            yield return "/application.yml";
            yield return "/backup.sql";
            yield return "/backup.sql.gz";
            yield return "/database.json";
            yield return "/db.zip";
            yield return "/1ebug.log";
            yield return "/error.log";
            yield return "/secrets.yml";
            yield return "/settings.py";
            yield return "/config.json";
            yield return "/config/local.json";
            yield return "/config/production.json";
            yield return "/config/master.key";
            yield return "/docker-compose.yml";
            yield return "/docker-compose.production.yaml";
            yield return "/index.php";
            yield return "/index.php~";
            yield return "/index.php.bak";
            yield return "/schema.sql";
            yield return "/terraform.tfvars";
            yield return "/terraform.tfstate";
            yield return "/wp-admin/";
            yield return "/wp-content/plugins/index.php";
            yield return "/wp-json/users";
            yield return "/_profiler";
            yield return "/_profiler/requests";
            yield return "/profiler/requests";
            yield return "/actuator/health";
            yield return "/console";
            yield return "/console/login";
            yield return "/logs/error.log";
            yield return "/storage/logs/application.log";
            yield return "/var/log/system.log";
            yield return "/api/graphql";
            yield return "/graphql";
            yield return "/v2/_catalog";
            yield return "/package.json";
            yield return "/web.config";
            yield return "/foo/.git/config";
            yield return "/foo/*";
            yield return "/robots.txt";
            yield return "/security.txt";
            yield return "/sitemap.xml";
            yield return "/sitemap_index.xml";
            yield return "/aws.json";
            yield return "/phpinfo";
            yield return "/serverless.yml";
            yield return "/credentials.json";
            yield return "/connectionstrings.config";
            yield return "/local_settings.py";
            yield return "/trace.axd";
            yield return "/geoserver/web/";
            yield return "/owa/auth/logon.aspx";
            yield return "/telescope/requests";
            yield return "/@vite/env";
        }

        private static IEnumerable<HttpMethod> EmptyRootRequestScenarios()
        {
            yield return HttpMethod.Get;
            yield return HttpMethod.Post;
            yield return HttpMethod.Put;
            yield return HttpMethod.Patch;
            yield return HttpMethod.Delete;
            yield return HttpMethod.Head;
            yield return HttpMethod.Options;
            yield return HttpMethod.Trace;
            yield return new HttpMethod("CONNECT");
        }

        private static IEnumerable<HttpMethod> RootRequestsWithContentScenarios()
        {
            yield return HttpMethod.Post;
            yield return HttpMethod.Put;
            yield return HttpMethod.Patch;
            yield return HttpMethod.Delete;
        }
    }
}