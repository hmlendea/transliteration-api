# Transliteration API — Testing Architecture

## Overview

Two test projects provide comprehensive coverage:
- **Unit Tests** (`TransliterationAPI.UnitTests`) — Fast, isolated, deterministic
- **Integration Tests** (`TransliterationAPI.IntegrationTests`) — Full HTTP pipeline, external provider simulation

## Unit Tests

### Project: `TransliterationAPI.UnitTests`

**Framework:** NUnit 4.6.1, NuciLog.Core (NullLogger)

**Test Classes:**

| Class | Coverage |
|-------|----------|
| `LanguageRegistryTests` | Registry completeness, DI resolvability |
| `*TransliteratorTests` (14) | Each local transliterator with verified pairs |

### LanguageRegistryTests

```csharp
[Test]
public void GivenTheLanguageRegistry_WhenEnumeratingLanguages_ThenEachLanguageHasAConcreteTransliteratorType()
{
    Assert.That(Language.GetAll(), Is.Not.Empty);
    Assert.That(Language.GetAll().All(l => l.TransliteratorType != null), Is.True);
    Assert.That(Language.GetAll().All(l =>
        typeof(IExternalTransliterator).IsAssignableFrom(l.TransliteratorType) == l.UsesExternalTransliterator), Is.True);
}

[Test]
public void GivenTheServiceCollection_WhenRegisteringTransliterators_ThenEveryConfiguredTransliteratorIsResolvable()
{
    // Builds full DI container with test config
    // Verifies every distinct TransliteratorType resolves
}
```

### Transliterator Tests (Pattern)

Each of the 14 local transliterators has a dedicated test class:

```csharp
[TestFixture]
public class CyrillicTransliteratorTests
{
    CyrillicTransliterator transliterator;

    [SetUp]
    public void SetUp() => transliterator = new(new NullLogger());

    [Test]
    [TestCase("Аҟәа", "Ak̄a̋a")]
    [TestCase("Гагра", "Gagra")]
    // ... 50+ test cases per transliterator
    public void GivenText_WhenTransliterating_ThenCorrectOutput(string input, string expected)
        => Assert.That(transliterator.Transliterate(input, Language.Abkhaz), Is.EqualTo(expected));
}
```

**Test data source:** Verified transliteration pairs from authoritative standards (ISO, BGN/PCGN, ALA-LC, etc.)

**External transliterator unit tests:** Use `FakeHttpRequestManager` to simulate provider responses.

### Running Unit Tests

```bash
dotnet test TransliterationAPI.UnitTests/TransliterationAPI.UnitTests.csproj
```

## Integration Tests

### Project: `TransliterationAPI.IntegrationTests`

**Framework:** NUnit 4.6.1, Microsoft.AspNetCore.Mvc.Testing 10.0.12

**Architecture:** Full ASP.NET Core host in memory via `WebApplicationFactory<Program>`

### Test Infrastructure

#### TransliterationApiWebApplicationFactory

```csharp
internal sealed class TransliterationApiWebApplicationFactory : WebApplicationFactory<Program>
{
    // Configures test environment:
    // - Unique temp cache directory per factory instance
    // - RecordingHttpRequestManager (captures HTTP calls)
    // - RecordingTransliterationService (captures service calls)
    // - NullLogger (silences logs)
    // - Fixed HMAC key: "NucileRullz!"
    // - LoopbackRemoteIpAddressStartupFilter (sets remote IP)
}
```

#### RecordingHttpRequestManager

Captures all external HTTP interactions:

```csharp
internal sealed class RecordingHttpRequestManager : IHttpRequestManager
{
    // Tracks:
    // - PostInvocationCount, PostWithHeadersInvocationCount, RetrieveCookiesInvocationCount
    // - LastPostUrl, LastFormData, LastHeaders, LastCookieRetrievalUrl
    // - Configurable: ResponseToReturn, CookiesToReturn, ExceptionToThrow
}
```

#### RecordingTransliterationService

Captures service-layer calls:

```csharp
internal sealed class RecordingTransliterationService : ITransliterationService
{
    // Tracks: InvocationCount, LastLanguageCode, LastText
    // Configurable: ResultToReturn, ExceptionToThrow
}
```

### Test Categories

#### 1. EndpointRoutingTests
- Route matching (case insensitivity, trailing slashes)
- 404 for unregistered routes
- 405 for unsupported methods (POST, PUT, etc.)
- Accept header handling

#### 2. LanguagesEndpointTests
- 200 OK with JSON
- All languages returned in code order
- Each language has `code`, `name`, `transliterator` properties
- HMAC signature present

#### 3. RegisteredLanguageEndpointTests
- **Every registry language tested exactly once**
- Verifies correct transliterator dispatch (local vs external)
- Local: no HTTP calls; External: 1 HTTP call
- Cache entry created

#### 4. LocalTransliterationEndpointTests
- End-to-end local transliteration via HTTP
- Verifies exact output matches unit test expectations
- HMAC present, no external HTTP calls

#### 5. TranslitterationDotComEndpointTests
- Provider request format verification (URL, form fields, headers)
- Response prefix stripping (`ack:::`)
- Edge cases: empty, double prefix, prefixed

#### 6. UshuaiaEndpointTests
- Two-step protocol: cookie GET → form POST
- Cookie parsing and header construction
- Session reuse (5-min cache)
- Various cookie response formats

#### 7. PodolakEndpointTests
- Fixed form field verification
- HTML extraction from `<textarea id="ausgabe">`
- Multiple results → first extracted
- Special characters preserved

#### 8. TransliterationServiceIntegrationTests
- Unknown language → original text
- No language code → original text
- No text + supported language → 400 Bad Request
- Whitespace trimming before provider call
- Cache persistence on success
- Cache hit → no provider call
- Normalised text equivalence (whitespace variants hit same cache)

#### 9. ExceptionHandlingTests
- Exception → HTTP status/code/message mapping
- All configured exception types verified
- HMAC null on error responses
- Service not invoked on middleware rejection

#### 10. ScannerProtectionTests
- Forbidden paths (`.env`, `wp-admin`, `.git`, etc.)
- Forbidden query probes (`gravitysmtp`, `wp/v2/users`, `XDEBUG`, `app_vl`)
- Forbidden headers (`User-Agent` scanners, `From` OAI)
- Empty root requests (GET, POST, etc.)

### Running Integration Tests

```bash
# All integration tests
dotnet test TransliterationAPI.IntegrationTests/TransliterationAPI.IntegrationTests.csproj

# Specific class
dotnet test --filter "FullyQualifiedName~TranslitterationDotComEndpointTests"
```

## Test Data Strategy

### Unit Test Data
- **Source:** Authoritative transliteration standards
- **Format:** `[TestCase]` attributes with input/expected pairs
- **Volume:** 50-200 cases per transliterator
- **Maintenance:** Update when transliterator logic changes

### Integration Test Data
- **Source:** Same verified pairs as unit tests (shared via `Language` registry)
- **External provider responses:** Mocked via `RecordingHttpRequestManager.ResponseToReturn`
- **Coverage:** Every language in registry has at least one integration test case

### Registry Completeness Test
```csharp
[Test]
public void GivenTheRegisteredLanguageMatrix_WhenComparingItWithTheRegistry_ThenEveryLanguageOccursExactlyOnce()
{
    // Compares Language.GetAll() codes vs test scenario codes
    // Fails if language added to registry without test case
}
```

## Coverage Summary

| Layer | Coverage | Method |
|-------|----------|--------|
| Transliterators (local) | 100% of languages | Unit + Integration |
| Transliterators (external) | 100% of languages | Integration (mocked HTTP) |
| Cache behavior | Full | Integration |
| Middleware (exception, scanner, logging) | Full | Integration |
| Routing | Full | Integration |
| Language registry | Full | Unit + Integration |
| DI composition | Full | Unit (registry test) |

## Continuous Integration

**GitHub Actions:** `.github/workflows/dotnet.yml`

```yaml
- name: Build
  run: dotnet build TransliterationAPI.slnx

- name: Test
  run: dotnet test TransliterationAPI.slnx --no-build
```

Runs on every push/PR to master.

## Adding Tests

### New Local Transliterator
1. Add `NewTransliteratorTests.cs` in `TransliterationAPI.UnitTests/Service/Transliterators/`
2. Follow existing pattern with `TestCase` attributes
3. Add integration scenario to `RegisteredLanguageEndpointTests.LocalLanguageScenarios()`

### New External Provider
1. Add `NewProviderEndpointTests.cs` in `TransliterationAPI.IntegrationTests/Service/`
2. Use `RecordingHttpRequestManager` to verify request format
3. Add scenarios to `RegisteredLanguageEndpointTests` (appropriate provider section)

### New Middleware Behavior
1. Add test class in `TransliterationAPI.IntegrationTests/Middleware/`
2. Use `RecordingTransliterationService` to verify service not invoked
3. Test both positive and negative cases

## Test Isolation

- Each test gets fresh `TransliterationApiWebApplicationFactory`
- Unique temp cache directory per factory
- `RecordingHttpRequestManager` state reset per test
- `TearDown` disposes client and factory (cleans temp directory)

## Determinism

- No real HTTP calls (all mocked)
- No real time dependencies (cookie expiry mocked via test control)
- Fixed HMAC key
- NullLogger (no log output)
- In-memory TestServer (no network)