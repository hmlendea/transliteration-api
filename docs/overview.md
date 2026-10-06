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