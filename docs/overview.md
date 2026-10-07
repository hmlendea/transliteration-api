# Transliteration API — Overview

## Purpose

The Transliteration API is a REST service that converts text from 40+ writing systems into the Latin alphabet. It exposes two endpoints:

- `GET /Transliteration` — Transliterates input text for a specific language code
- `GET /Languages` — Returns the authoritative registry of supported languages and their transliterator implementations

## Key Characteristics

| Aspect | Detail |
|--------|--------|
| **Framework** | ASP.NET Core on .NET 10 |
| **Architecture** | Layered, single-process HTTP API |
| **Transliteration strategies** | 14 local (built-in) + 3 external providers |
| **Caching** | File-backed JSON cache with SHA-256 keys |
| **Response integrity** | HMAC-SHA256 signed responses |
| **Security middleware** | Scanner/bot protection, exception mapping |
| **Observability** | Structured logging via NuciLog |
| **Testing** | Unit tests (transliterators, registry) + Integration tests (full HTTP pipeline) |

## Supported Writing Systems

| Category | Languages (examples) |
|----------|---------------------|
| **Cyrillic** | Russian, Ukrainian, Bulgarian, Serbian, Kazakh, Tajik, Tatar, Belarusian, Chuvash, Macedonian, Abkhaz, Adyghe |
| **Greek** | Modern Greek, Ancient Greek, Ancient Doric Greek |
| **Arabic** | Arabic, Egyptian Arabic, Maghrebi Arabic |
| **Indic** | Hindi, Bengali, Marathi, Gujarati, Sanskrit, Tamil, Telugu, Kannada, Malayalam, Sinhala |
| **East Asian** | Japanese (Hiragana/Katakana/Kanji), Korean (Hangul), Chinese (Pinyin) |
| **Other** | Hebrew, Georgian, Armenian, Coptic, Berber, Korean, Mongolian, Old Church Slavonic |

## External Providers

| Provider | Languages | Protocol |
|----------|-----------|----------|
| **translitteration.com** | Abkhaz, Adyghe, Armenian, Bashkir, Georgian, Inuttitut, Kyrgyz, Ossetic, Udmurt, Western Armenian | HTTPS form POST |
| **ushuaia.pl** | Bengali, Hindi, Kannada, Malayalam, Mongol, Sanskrit, Sinhala, Tamil, Telugu | HTTPS cookie + form POST |
| **podolak.net** | Old Church Slavonic | HTTPS form POST |

## Deployment Model

**Self-hosted only.** The project maintainers do not operate a hosted service. Operators deploy the API themselves and control:
- Configuration (cache, logging, HMAC key)
- Local storage (cache file, log file)
- Network exposure and access controls
- External provider usage (via language registry)

## Quick Start

```bash
# Build
dotnet build TransliterationAPI.slnx

# Run (default URLs)
dotnet run --project TransliterationAPI/TransliterationAPI.csproj

# Explicit URL
ASPNETCORE_URLS=http://localhost:5000 dotnet run --project TransliterationAPI/TransliterationAPI.csproj

# Test
dotnet test TransliterationAPI.slnx
```

## Example Usage

```bash
# Transliterate Russian text
curl 'http://localhost:5000/Transliteration?text=%D0%AD%D0%BA%D0%B2%D0%B0%D1%82%D0%BE%D1%80%D0%B8%D0%B0%D0%BB%D1%8C%D0%BD%D0%B0%D1%8F%20%D0%90%D1%84%D1%80%D0%B8%D0%BA%D0%B0&language=ru'
# {"text":"Ekvatorialnaya Afrika","hmac":"..."}

# List supported languages
curl 'http://localhost:5000/Languages'
# {"count":40,"languages":[{"code":"ar","name":"Arabic","transliterator":"ArabicTransliterator"},...]}
```

## Detailed Architecture

### Layered Design

The API follows a clean layered architecture with clear separation of concerns:

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

### Transport Layer (Controllers)

Both controllers inherit from `NuciApiController` (NuciAPI.Controllers package), which provides:
- Unified request processing via `ProcessRequest<TRequest, TResponse>()`
- Automatic model validation
- HMAC response signing
- Standardized error mapping

**TransliterationController** (`GET /Transliteration`):
- Accepts `text` (max 256 chars) and `language` query parameters
- Delegates to `ITransliterationService.Transliterate()`
- Returns `GetTransliterationResponse` with `text` and `hmac` fields

**LanguagesController** (`GET /Languages`):
- No input parameters
- Returns `Language.GetAll()` ordered by code
- Returns `GetLanguagesResponse` with `count`, `languages[]`, and `hmac`

### Application Layer (TransliterationService)

The `TransliterationService` orchestrates the transliteration flow:

1. **Language validation** — Looks up language code in registry; returns original text if unknown
2. **Text normalization** — Trims leading/trailing whitespace
3. **Cache lookup** (if enabled) — Computes SHA-256 key from `languageCode + unicodeCodePoints + appVersion`
4. **Strategy dispatch** — Uses `ITransliteratorFactory` to get appropriate transliterator
5. **Execution** — Calls transliterator's `Transliterate()` method
6. **Cache persistence** — Stores non-null, non-whitespace results
7. **Response** — Returns transliterated text (or null on external provider failure)

### Strategy Layer (Transliterators)

**Local Transliterators** (14 implementations, synchronous):
- All inherit `Transliterator` base class implementing `ITransliterator`
- Use character/pattern replacement tables (dictionaries)
- Apply post-processing fixes (title casing, specific replacements)
- Thread-safe singletons registered in DI

**External Transliterators** (3 implementations, asynchronous):
- All inherit `ExternalTransliterator` base class implementing `IExternalTransliterator`
- Use `IHttpRequestManager` for HTTP communication
- Handle provider-specific protocols (form POST, cookie management, HTML parsing)
- Let exceptions propagate; caught by `TryGetTransliteratedText()` returning null

### Data Layer (Caching)

- **Entity**: `CachedTransliteration` (inherits `EntityBase` with `Id` + `TransliteratedText`)
- **Repository**: `JsonRepository<CachedTransliteration>` from NuciDAL
- **Storage**: JSON array in file at `cacheSettings.storeLocation`
- **Key**: SHA-256 of `languageCode_unicodeCodePoints_applicationVersion`
- **Scope**: Per-language, per-text, per-app-version (version change invalidates all)

### Cross-Cutting Concerns

| Concern | Implementation |
|---------|----------------|
| **Logging** | NuciLog with `MyOperation` and `MyLogInfoKey` vocabulary |
| **Exception Handling** | NuciAPI.Middleware.ExceptionHandling maps exceptions to JSON responses |
| **Scanner Protection** | NuciAPI.Middleware.Security blocks known scanner/bot patterns |
| **Response Signing** | NuciSecurity.HMAC signs all successful responses |
| **Configuration** | Strongly-typed settings classes bound from `appsettings.json` |

## Complete Language Registry

The authoritative list of supported languages is defined in `Language.cs` as static properties. Each entry specifies:
- `Code` — ISO-like identifier used in API `language` parameter
- `Name` — Display name
- `TransliteratorType` — Concrete implementation type
- `UsesExternalTransliterator` — Whether it uses `IExternalTransliterator` (async, HTTP)

### Local Transliterators (14 implementations, 31 languages)

| Transliterator | Languages | Script/Standard |
|----------------|-----------|-----------------|
| `CyrillicTransliterator` | ab, be, bg, cv, kk, mk, ru, sr, sr-ec, sh, tg, tg-cyrl, tt, tt-cyrl, uk (15) | ALA-LC, BGN/PCGN, ISO-9, language-specific |
| `GreekTransliterator` | el, grc, grc-dor (3) | ISO 843, Ancient, Doric |
| `ArabicTransliterator` | ar, arz, ary (3) | Main + Maghrebi |
| `HebrewTransliterator` | he (1) | Consonants + niqqud |
| `JapaneseTransliterator` | ja (1) | Hiragana/Katakana/Kanji |
| `KoreanTransliterator` | ko (1) | Hangul syllables |
| `PinyinTransliterator` | zh, zh-hans (2) | Microsoft PinYinConverter |
| `GujaratiTransliterator` | gy (1) | Multi-pass Devanagari |
| `MarathiTransliterator` | mr (1) | Devanagari + Marathi chars |
| `CopticTransliterator` | cop (1) | Coptic alphabet |
| `BerberTransliterator` | ber (1) | Tifinagh |

### External Transliterators (3 implementations, 19 languages)

| Transliterator | Provider | Languages |
|----------------|----------|-----------|
| `TranslitterationDotComTransliterator` | translitteration.com | ady, hy, ba, ka, iu, ky, os, udm, hyw (9) |
| `UshuaiaTransliterator` | ushuaia.pl | bn, hi, kn, ml, mn, sa, si, ta, te (9) |
| `PodolakTransliterator` | podolak.net | cu (1) |

**Total: 40+ languages and variants**

## Request/Response Flow

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

## Configuration

The application reads configuration from `TransliterationAPI/appsettings.json` with support for environment-specific overrides and environment variables.

### Settings Sections

| Section | Class | Key Properties |
|---------|-------|----------------|
| `cacheSettings` | `CacheSettings` | `StoreLocation`, `Enabled`, `ApplicationVersion` (computed) |
| `securitySettings` | `SecuritySettings` | `HmacSigningKey` |
| `nuciLoggerSettings` | `NuciLoggerSettings` | `LogFilePath`, `IsFileOutputEnabled` |

### Example Configuration

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

Environment variables use double-underscore notation: `TRANSLITERATION_API__CACHE__STORELOCATION=/var/cache/cache.json`

## Testing

Two test projects provide comprehensive coverage:

| Project | Framework | Focus |
|---------|-----------|-------|
| `TransliterationAPI.UnitTests` | NUnit | 14 transliterator test classes + registry tests |
| `TransliterationAPI.IntegrationTests` | NUnit + WebApplicationFactory | Full HTTP pipeline, 10 test classes |

Run all tests: `dotnet test TransliterationAPI.slnx`

## Extension Points

### Add a Local Transliterator

1. Create `NewTransliterator : Transliterator, ITransliterator`
2. Implement `PerformTransliteration(string text, Language language)`
3. Add static property to `Language` class
4. No DI registration needed (automatic via reflection)

### Add an External Provider

1. Create `NewExternalTransliterator : ExternalTransliterator, IExternalTransliterator`
2. Implement `PerformTransliteration(string text, Language language)` using `IHttpRequestManager`
3. Add static property to `Language` class
4. No DI registration needed

### Replace Cache Persistence

1. Implement `IFileRepository<CachedTransliteration>` or replace `IFileRepository<T>` registration
2. Register alternative in `AddCustomServices()`

## Key Invariants

1. **Language registry is authoritative** — `Language.GetAll()` is the single source of truth
2. **Cache key includes app version** — Version change invalidates all cached entries
3. **External provider failures are silent** — Returns original text, no error propagated
4. **Whitespace normalisation before cache lookup** — Trimmed text used for cache key
5. **Response signing is mandatory** — Both endpoints sign every successful response
6. **Scanner protection runs first** — Blocked requests never reach application logic
7. **Transliterators are stateless** — Thread-safe singletons, no instance state
8. **Language code matching is exact** — Case-sensitive dictionary lookup, no aliasing
9. **External provider timeouts are hard** — 3 seconds → 502 Bad Gateway
10. **Cache is per-instance** — File-backed, no distributed cache support