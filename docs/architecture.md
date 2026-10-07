# Transliteration API — Architecture Detail

This document provides implementation-level architectural detail that complements the root `ARCHITECTURE.md`. It focuses on component internals, data flows, contracts, and extension points.

## Component Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         Transliteration API Process                         │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────────┐    ┌──────────────────┐    ┌────────────────────────┐   │
│  │   Middleware │───▶│    Controllers   │───▶│  ITransliterationService│   │
│  │  (Pipeline)  │    │  (Transport)     │    │   (Orchestrator)       │   │
│  └──────────────┘    └──────────────────┘    └───────────┬────────────┘   │
│                                                           │                │
│                    ┌──────────────────────────────────────┼──────────┐    │
│                    ▼                                      ▼          ▼    │
│           ┌─────────────────┐              ┌─────────────────────┐  ┌──────────┐
│           │  Language       │              │  ITransliterator    │  │ IFile    │
│           │  Registry       │              │  Factory            │  │ Repository│
│           │  (Language)     │              │                     │  │ (Cache)  │
│           └─────────────────┘              └──────────┬──────────┘  └──────────┘
│                                                        │
│                    ┌───────────────────────────────────┼───────────────┐
│                    ▼                                   ▼               ▼
│           ┌─────────────────┐              ┌─────────────────┐ ┌──────────────┐
│           │ Local           │              │ External        │ │ IHttpRequest │
│           │ Transliterators │              │ Transliterators │ │ Manager      │
│           │ (14 impls)      │              │ (3 impls)       │ │              │
│           └─────────────────┘              └─────────────────┘ └──────────────┘
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Host and Composition

### Program.cs / Startup.cs

**Entry point:** `Program.Main()` → `CreateHostBuilder()` → `WebHostDefaults` → `Startup`

**Startup.ConfigureServices()** registers:
1. Controllers
2. Configuration binding (`CacheSettings`, `SecuritySettings`, `NuciLoggerSettings`)
3. NuciAPI middleware (scanner protection, logging, exception handling)
4. Custom services via `AddCustomServices()`

**Startup.Configure()** builds the middleware pipeline:
1. `UseNuciApiRequestLogging()` — structured request logging
2. `UseNuciApiExceptionHandling()` — maps exceptions to JSON error responses
3. `UseNuciApiScannerProtection()` — blocks scanner/bot probes
4. Developer exception page (development only)
5. Static files + default files
6. Routing + Authorization
7. Endpoint mapping (`MapControllers()`)
8. Cache file creation on startup if missing

### ServiceCollectionExtensions.cs

**AddConfigurations()** — Binds `CacheSettings`, `SecuritySettings`, `NuciLoggerSettings` from `IConfiguration`; registers as singletons.

**AddCustomServices()** — Registers:
- `IFileRepository<CachedTransliteration>` → `JsonRepository` (cache persistence)
- `IHttpRequestManager` → `HttpRequestManager` (external provider HTTP client)
- All transliterator types as singletons (via `AddTransliteratorServices()`)
- `ITransliteratorFactory` → `TransliteratorFactory`
- `ITransliterationService` → `TransliterationService`
- `ILogger` → `NuciLogger`

**AddTransliteratorServices()** — Reflects over `Language.GetAll()`, extracts distinct `TransliteratorType`s, registers each as singleton. Also registers `NuciLogger` as `ILogger`.

## API Transport Layer

### Controllers

Both controllers inherit `NuciApiController` (from `NuciAPI.Controllers`), which provides:
- `ProcessRequest<TRequest, TResponse>(request, handler, authorisation)` — unified request handling
- Automatic model validation, HMAC signing, error mapping

**TransliterationController**
- `GET /Transliteration` with `GetTransliterationRequest` (text, language)
- Authorisation: `NuciApiAuthorisation.None`
- Delegates to `ITransliterationService.Transliterate()`
- Signs response with `SecuritySettings.HmacSigningKey`

**LanguagesController**
- `GET /Languages` with empty `GetLanguagesRequest`
- Authorisation: `NuciApiAuthorisation.None`
- Returns `Language.GetAll()` ordered by code
- Signs response with `SecuritySettings.HmacSigningKey`

### Request/Response Models

| Model | Purpose | Key Fields |
|-------|---------|------------|
| `GetTransliterationRequest` | Transliteration input | `Text` (max 256 chars), `Language` |
| `GetLanguagesRequest` | Language discovery input | (empty) |
| `GetTransliterationResponse` | Transliteration output | `Text` (transliterated) |
| `GetLanguagesResponse` | Language discovery output | `Language[] Languages`, `int Count` |

Both responses inherit `NuciApiSuccessResponse` which includes `Hmac` property set by `SignHMAC()`.

## Application and Data Layer

### TransliterationService (ITransliterationService)

**Primary method:** `Task<string> Transliterate(string text, string languageCode)`

**Flow:**
1. Log start (`MyOperation.Transliteration`, `OperationStatus.Started`)
2. Validate language exists in registry → if not, return original text
3. Normalise text (trim whitespace)
4. If cache disabled → `TryGetTransliteratedText()` directly
5. If cache enabled:
   - Compute cache ID: `SHA256(languageCode + "_" + unicodeCodePoints(text) + "_" + appVersion)`
   - Check cache → return if hit
   - Otherwise `TryGetTransliteratedText()`, store result if non-null, save cache
6. Log success/failure
7. Return transliterated text (or null on external provider failure)

**Cache ID generation:**
```csharp
string textUnicodes = string.Join('-', text.Select(c => (int)c));
string cacheKey = $"{languageCode}_{textUnicodes}_{cacheSettings.ApplicationVersion}";
return SHA256(cacheKey);
```

**TryGetTransliteratedText()** — wraps `GetTransliteratedText()` in try/catch, returns null on any exception (swallows external provider failures).

**GetTransliteratedText()** — Dispatches to factory:
- If `language.UsesExternalTransliterator` → `factory.GetExternalTransliterator(language).Transliterate()`
- Else → `factory.GetTransliterator(language).Transliterate()`

### Language Registry (Language.cs)

**Static registry** — 40+ `Language` instances defined as static properties, collected via reflection in static constructor.

**Each Language has:**
- `Code` — ISO-like code (e.g., "ru", "grc-dor", "zh-hans")
- `Name` — Display name
- `TransliteratorType` — Concrete `Type` implementing `ITransliterator` or `IExternalTransliterator`
- `Transliterator` — Property returning `TransliteratorType.Name`
- `UsesExternalTransliterator` — `typeof(IExternalTransliterator).IsAssignableFrom(TransliteratorType)`

**Key methods:**
- `GetAll()` → `IEnumerable<Language>` (all registered)
- `FromCode(string)` → `Language` (throws if not found)
- `Equals()` / `GetHashCode()` based on `Code`

### TransliteratorFactory (ITransliteratorFactory)

**Resolves transliterators from DI container:**
- `GetTransliterator(Language)` → `serviceProvider.GetRequiredService(language.TransliteratorType)` cast to `ITransliterator`
- `GetExternalTransliterator(Language)` → same but cast to `IExternalTransliterator`

All transliterator types are registered as singletons at startup.

## Transliteration Strategies

### Base Classes

**Transliterator (abstract, implements ITransliterator)**
- Synchronous `Transliterate(string, Language)`
- Logs start/success/failure with `MyOperation.TransliteratorExecution`
- Calls abstract `PerformTransliteration(string, Language)`

**ExternalTransliterator (abstract, implements IExternalTransliterator)**
- Async `Transliterate(string, Language)`
- Same logging pattern
- Calls abstract `PerformTransliteration(string, Language)` returning `Task<string>`

### Local Transliterators (14 implementations)

All inherit `Transliterator`, implement `PerformTransliteration` using character/pattern replacement tables:

| Transliterator | Languages | Approach |
|----------------|-----------|----------|
| `CyrillicTransliterator` | 13 languages | Multiple scheme tables (ALA-LC, BGN/PCGN, ISO-9, language-specific) |
| `GreekTransliterator` | 3 languages | Modern + Ancient + Doric tables with digraph handling |
| `ArabicTransliterator` | 3 languages | Main + Maghrebi tables; diacritic handling; title-case fixes |
| `HebrewTransliterator` | 1 language | Consonant + niqqud tables; extensive post-processing fixes |
| `JapaneseTransliterator` | 1 language | Char→string map for Hiragana/Katakana + selected Kanji |
| `KoreanTransliterator` | 1 language | Syllable→string map for common Hangul |
| `PinyinTransliterator` | 2 languages | `Microsoft.International.Converters.PinYinConverter` + tone mark conversion |
| `GujaratiTransliterator` | 1 language | Multi-pass replacement: consonants+vowel signs, halants, vowels, numerals |
| `MarathiTransliterator` | 1 language | Devanagari table with additional chars; title-case output |
| `CopticTransliterator` | 1 language | Direct char→string map |
| `BerberTransliterator` | 1 language | Tifinagh char→string map; title-case output |
| `CyrillicTransliterator` (shared) | — | See above |

**Common pattern:** Each maintains `Dictionary<string,string>` tables; `PerformTransliteration` iterates keys and applies `Regex.Replace`. Some apply post-processing fixes (title casing, specific replacements).

### External Transliterators (3 implementations)

All inherit `ExternalTransliterator`, implement async `PerformTransliteration` via HTTP:

| Transliterator | Provider | Protocol Details |
|----------------|----------|------------------|
| `TranslitterationDotComTransliterator` | translitteration.com | POST form: `text`, `tlang` (mapped), `script=latn`, `scheme` (mapped). Strips `ack:::` prefix. |
| `UshuaiaTransliterator` | ushuaia.pl | 1) GET cookies from `/transliterate/`; 2) POST form: `text`, `lang` (mapped) with `Cookie: translit=...;lastlang=...`. Session cookie cached 5 min. |
| `PodolakTransliterator` | podolak.net | POST form with 6 fixed fields. Extracts result from `<textarea id="ausgabe">` via regex. |

**Error handling:** All external transliterators let exceptions propagate; `TransliterationService.TryGetTransliteratedText()` catches and returns null (caller returns original text).

## Caching

### CachedTransliteration (Entity)

```csharp
public class CachedTransliteration : EntityBase
{
    public string TransliteratedText { get; set; }
    // Id inherited from EntityBase (string)
}
```

### JsonRepository (NuciDAL)

- File-backed JSON array persistence
- `TryGet(id)` → entity or null
- `Add(entity)` → tracks in memory
- `SaveChanges()` → serializes entire collection to file

### Cache Behavior

- **Key:** SHA-256 of `languageCode_unicodeCodePoints_applicationVersion`
- **Scope:** Per-language, per-text, per-app-version (version change invalidates cache)
- **Persistence:** `cacheSettings.storeLocation` (default `cache.json`)
- **Enabled:** `cacheSettings.enabled` (default true)
- **Initialization:** Startup creates empty `[]` file if missing
- **Eviction:** None (manual file deletion or disable cache)

## Cross-Cutting Concerns

### Logging (NuciLog)

**Configuration:** `NuciLoggerSettings` → `logFilePath`, `isFileOutputEnabled`

**Log entries** include:
- Operation: `MyOperation.Transliteration` or `MyOperation.TransliteratorExecution`
- Status: `Started`, `Success`, `Failure`
- Context: `MyLogInfoKey.Text`, `LanguageCode`, `LanguageName`, `Transliterator`, `TransliteratedText`

### Exception Handling (NuciAPI.Middleware.ExceptionHandling)

Maps exception types to HTTP responses:
| Exception | Status | Code | Message |
|-----------|--------|------|---------|
| `BadHttpRequestException` | 400 | BAD_REQUEST | Original message |
| `FormatException` | 400 | BAD_REQUEST | Original message |
| `ArgumentException` | 400 | BAD_REQUEST | Original message |
| `ValidationException` | 400 | BAD_REQUEST | Original message |
| `SecurityException` | 403 | UNAUTHORISED | Generic message |
| `AuthenticationException` | 401 | UNAUTHENTICATED | Generic message |
| `UnauthorizedAccessException` | 403 | FORBIDDEN | Generic message |
| `NotImplementedException` | 501 | NOT_IMPLEMENTED | Generic message |
| `TimeoutException` | 504 | GATEWAY_TIMEOUT | Generic message |
| `HttpRequestException` | 502 | BAD_GATEWAY | Generic message |
| Other | 500 | INTERNAL_ERROR | Generic message |

Responses: `{ success: false, message, code, hmac: null }` (no `text` property)

### Scanner Protection (NuciAPI.Middleware.Security)

Blocks requests matching:
- **Paths:** `/.env`, `/wp-admin`, `/phpmyadmin`, `/.git`, `/config`, `/backup`, etc.
- **Query probes:** `page=gravitysmtp-settings`, `rest_route=/wp/v2/users`, `XDEBUG_SESSION_START`, `app_vl`
- **Headers:** `User-Agent` containing scanner/bot signatures, `From` header with OAI/OpenAI
- **Empty root requests:** `GET /`, `POST /`, etc. with/without content

Returns 403 Forbidden with empty body; does not invoke application handlers.

### Response Signing (NuciSecurity.HMAC)

- `NuciApiSuccessResponse.SignHMAC(key)` computes HMAC-SHA256 of response JSON (excluding `hmac` field)
- Key: `SecuritySettings.HmacSigningKey` (from config, placeholder in appsettings.json)
- Both endpoints sign responses
- Not authentication; integrity verification only

## Configuration

### appsettings.json Structure

```json
{
  "cacheSettings": {
    "storeLocation": "cache.json",
    "enabled": "true"
  },
  "securitySettings": {
    "hmacSigningKey": "[[TRANSLITERATION_API_HMAC_SIGNING_KEY]]"
  },
  "nuciLoggerSettings": {
    "logFilePath": "logfile.log",
    "isFileOutputEnabled": true
  }
}
```

### Settings Classes

| Class | Properties | Source |
|-------|------------|--------|
| `CacheSettings` | `StoreLocation`, `Enabled`, `ApplicationVersion` (computed) | `cacheSettings` section |
| `SecuritySettings` | `HmacSigningKey` | `securitySettings` section |
| `NuciLoggerSettings` | `LogFilePath`, `IsFileOutputEnabled` | `nuciLoggerSettings` section |

All bound in `AddConfigurations()` and registered as singletons.

## External Dependencies

### NuGet Packages (TransliterationAPI)

| Package | Version | Purpose |
|---------|---------|---------|
| `CHSPinYinConv` | 1.0.0 | Chinese Pinyin conversion |
| `NuciAPI` | 3.6.1 | Base API infrastructure |
| `NuciAPI.Controllers` | 2.3.1 | Controller base class, request/response models |
| `NuciAPI.Middleware` | 2.0.3 | Middleware pipeline |
| `NuciAPI.Middleware.ExceptionHandling` | 1.0.2 | Exception→response mapping |
| `NuciAPI.Middleware.Logging` | 1.0.1 | Request logging |
| `NuciAPI.Middleware.Security` | 1.0.6 | Scanner protection |
| `NuciDAL` | 3.2.1 | JSON repository (cache) |
| `NuciExtensions` | 5.3.2 | String extensions (ToTitleCase, etc.) |
| `NuciLog` | 1.2.1 | Logging implementation |
| `NuciLog.Core` | 3.1.0 | Logging abstractions |
| `NuciSecurity.HMAC` | 4.1.3 | HMAC response signing |

### External HTTP Endpoints

| Provider | Base URL | Purpose |
|----------|----------|---------|
| translitteration.com | `https://www.translitteration.com/ajax/en/transliterate` | 10 languages |
| ushuaia.pl | `https://www.ushuaia.pl/transliterate/` (cookies), `.../transliterate.php` (POST) | 9 languages |
| podolak.net | `https://podolak.net/en/transliteration/old-church-slavonic` | 1 language |

All use HTTPS. Timeout: 3 seconds (HttpClient default in `HttpRequestManager`).

## Deployment and Operations

### Build
```bash
dotnet build TransliterationAPI.slnx
```

### Run
```bash
dotnet run --project TransliterationAPI/TransliterationAPI.csproj
# Or with explicit URL:
ASPNETCORE_URLS=http://localhost:5000 dotnet run --project TransliterationAPI/TransliterationAPI.csproj
```

### Test
```bash
# All tests
dotnet test TransliterationAPI.slnx

# Unit only
dotnet test TransliterationAPI.UnitTests/TransliterationAPI.UnitTests.csproj

# Integration only
dotnet test TransliterationAPI.IntegrationTests/TransliterationAPI.IntegrationTests.csproj
```

### Release
```bash
bash ./release.sh <version>
# Downloads and executes external release script
```

### Filesystem Requirements

| Path | Purpose | Permissions |
|------|---------|-------------|
| `cacheSettings.storeLocation` | JSON cache file | Read/write |
| `nuciLoggerSettings.logFilePath` | Log file | Read/write (append) |
| Cache directory | Parent of cache file | Create/write |

### Environment Variables

| Variable | Purpose |
|----------|---------|
| `ASPNETCORE_URLS` | Binding URLs (default: Kestrel defaults) |
| `ASPNETCORE_ENVIRONMENT` | `Development` enables dev exception page |

## Extension Points

### Add a Local Transliterator

1. Create `NewTransliterator : Transliterator, ITransliterator`
2. Implement `PerformTransliteration(string, Language)`
3. Add static property to `Language` class: `public static Language NewCode => new("code", "Name", typeof(NewTransliterator));`
4. Register in DI (automatic via reflection in `AddTransliteratorServices()`)
5. Add unit tests in `TransliterationAPI.UnitTests/Service/Transliterators/`

### Add an External Provider

1. Create `NewExternalTransliterator : ExternalTransliterator, IExternalTransliterator`
2. Implement `PerformTransliteration(string, Language)` using `IHttpRequestManager`
3. Add static property to `Language` with `typeof(NewExternalTransliterator)`
4. Register in DI (automatic)
5. Add integration tests with `RecordingHttpRequestManager`

### Replace Cache Persistence

1. Implement `IFileRepository<CachedTransliteration>` (or replace `IFileRepository<T>` registration)
2. Register alternative in `AddCustomServices()` replacing `JsonRepository`
3. `EntityBase` provides `Id` property; `CachedTransliteration` adds `TransliteratedText`

## Key Flows

### Transliteration Request (Cache Miss, Local)

```
GET /Transliteration?text=...&language=ru
    │
    ▼
Middleware: Logging → ExceptionHandling → ScannerProtection
    │
    ▼
TransliterationController.Get()
    │
    ▼
ProcessRequest() → validation → TransliterationService.Transliterate()
    │
    ▼
Language.FromCode("ru") → Language.Russian (CyrillicTransliterator)
    │
    ▼
Normalise text (trim)
    │
    ▼
Cache disabled or miss → GetTransliteratedText()
    │
    ▼
TransliteratorFactory.GetTransliterator() → CyrillicTransliterator
    │
    ▼
CyrillicTransliterator.Transliterate() → logging → PerformTransliteration()
    │
    ▼
Character replacement via tables → result
    │
    ▼
Cache store (if enabled) → SaveChanges()
    │
    ▼
Response: { text, hmac }
```

### Transliteration Request (External Provider)

```
... same until GetTransliteratedText() ...
    │
    ▼
Language.UsesExternalTransliterator == true
    │
    ▼
TransliteratorFactory.GetExternalTransliterator() → e.g. TranslitterationDotComTransliterator
    │
    ▼
ExternalTransliterator.Transliterate() → logging → PerformTransliteration()
    │
    ▼
HttpRequestManager.Post(url, formData) → HTTPS POST
    │
    ▼
Provider response → strip prefix/extract → result
    │
    ▼
Cache store → SaveChanges()
    │
    ▼
Response: { text, hmac }
```

### Language Discovery

```
GET /Languages
    │
    ▼
Middleware pipeline
    │
    ▼
LanguagesController.Get()
    │
    ▼
ProcessRequest() → Language.GetAll().OrderBy(l => l.Code)
    │
    ▼
GetLanguagesResponse { Languages[], Count }
    │
    ▼
SignHMAC() → Response: { count, languages[], hmac }
```

## Testing Architecture

### Unit Tests (TransliterationAPI.UnitTests)

| Test Class | Coverage |
|------------|----------|
| `LanguageRegistryTests` | Registry completeness, DI resolvability |
| `*TransliteratorTests` (14) | Each local transliterator with verified input/output pairs |

**Test pattern:** Instantiate transliterator with `NullLogger`, call `Transliterate(text, Language.X)`, assert exact output.

### Integration Tests (TransliterationAPI.IntegrationTests)

**Infrastructure:**
- `TransliterationApiWebApplicationFactory` — `WebApplicationFactory<Program>` with test configuration
- `RecordingHttpRequestManager` — Captures HTTP calls, returns configured responses
- `RecordingTransliterationService` — Captures service calls, returns configured results
- `LoopbackRemoteIpAddressStartupFilter` — Sets loopback remote IP for testing

**Test Categories:**

| Test Class | Focus |
|------------|-------|
| `EndpointRoutingTests` | Route matching, case insensitivity, 404s, method not allowed, Accept headers |
| `LanguagesEndpointTests` | Response structure, ordering, contract completeness |
| `RegisteredLanguageEndpointTests` | Every registry language tested once; local vs external dispatch |
| `LocalTransliterationEndpointTests` | End-to-end local transliteration via HTTP |
| `TranslitterationDotComEndpointTests` | Provider request format, response parsing, prefix stripping |
| `UshuaiaEndpointTests` | Cookie handling, session reuse, header construction |
| `PodolakEndpointTests` | Form fields, HTML extraction, edge cases |
| `TransliterationServiceIntegrationTests` | Cache behavior, whitespace normalisation, unknown languages, empty text |
| `ExceptionHandlingTests` | Exception→response mapping for all configured types |
| `ScannerProtectionTests` | Path/query/header blocking, empty root requests |

**Test Data:** Scenarios defined as `TestCaseSource` methods returning verified input/output pairs matching unit test expectations.

## Invariants and Contracts

1. **Language registry is authoritative** — `Language.GetAll()` is the single source of truth for supported languages, transliterator mapping, and external vs local classification.

2. **Cache key includes app version** — Version change invalidates all cached entries (prevents stale results after transliterator updates).

3. **External provider failures are silent** — `TryGetTransliteratedText()` catches all exceptions, returns null; caller returns original text. No error propagated to client.

4. **Whitespace normalisation before cache lookup** — Leading/trailing whitespace trimmed; cache keyed on normalised text.

5. **Response signing is mandatory** — Both endpoints sign every successful response; `hmac` field always present on success.

6. **Scanner protection runs before application logic** — Blocked requests never reach controllers or services.

7. **Transliterators are stateless** — All implementations are thread-safe singletons; no instance state.

8. **Language code matching is exact** — `Language.FromCode()` uses case-sensitive dictionary lookup; no aliasing or fuzzy matching.

9. **External provider timeouts are hard** — `HttpClient.Timeout = 3 seconds`; timeout → `HttpRequestException` → 502 Bad Gateway via exception middleware.

10. **Cache is per-instance** — File-backed; no distributed cache support. Multiple instances = separate caches.