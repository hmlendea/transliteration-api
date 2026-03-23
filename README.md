[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/fund.html) [![Latest GitHub release](https://img.shields.io/github/v/release/hmlendea/transliteration-api)](https://github.com/hmlendea/transliteration-api/releases/latest)

# About

REST API for transliterating text from multiple writing systems into the Latin alphabet.

The application is built with ASP.NET Core and currently targets .NET 10. It exposes a small HTTP API for:

- transliterating input text for a specific language code
- listing the supported languages
- caching transliteration results on disk

# Features

- Support for 40+ languages and variants
- Multiple transliteration strategies, including built-in and external providers
- File-based cache for repeated requests
- HMAC-signed API responses
- Unit tests for transliterators

# Requirements

- .NET SDK 10.0 or newer

Check the installed SDK version with:

```sh
dotnet --version
```

# Project Structure

- `TransliterationAPI/` - main ASP.NET Core API project
- `TransliterationAPI/API/Controllers/` - HTTP endpoints
- `TransliterationAPI/Service/` - transliteration logic, cache access, HTTP integrations
- `TransliterationAPI/Service/Transliterators/` - language-specific transliteration implementations
- `TransliterationAPI.UnitTests/` - NUnit test project

# Getting Started

## Restore dependencies

```sh
dotnet restore
```

## Build

```sh
dotnet build TransliterationAPI.sln
```

## Run the API

```sh
dotnet run --project TransliterationAPI/TransliterationAPI.csproj
```

By default, ASP.NET Core will bind to the standard development URLs configured by your local environment. If no custom environment variables or launch profile are applied, a common local URL is:

```text
http://localhost:5000
```

You can also set an explicit URL when starting the service:

```sh
ASPNETCORE_URLS=http://localhost:5000 dotnet run --project TransliterationAPI/TransliterationAPI.csproj
```

# Configuration

The application reads configuration from `TransliterationAPI/appsettings.json`.

Current settings include:

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

## Cache settings

- `storeLocation` - path to the JSON file used for cached transliteration results
- `enabled` - flag intended to control cache usage

If the cache file does not exist, the application creates it automatically on startup.

## Security settings

- `hmacSigningKey` - secret used to sign API responses

## NuciLogger settings

The standard `NuciLog` configuration

# API Endpoints

## `GET /Transliteration`

Transliterates input text for a specific language.

Query parameters:

- `text` - input text to transliterate
- `language` - supported language code

Validation and behavior:

- `text` is limited to 256 characters
- leading and trailing whitespace is trimmed before processing
- if the language code is not supported, the original text is returned unchanged
- successful results may be stored in the JSON cache

Example request:

```sh
curl --request GET \
	--location 'http://localhost:5000/Transliteration?text=%D0%AD%D0%BA%D0%B2%D0%B0%D1%82%D0%BE%D1%80%D0%B8%D0%B0%D0%BB%D1%8C%D0%BD%D0%B0%D1%8F%20%D0%90%D1%84%D1%80%D0%B8%D0%BA%D0%B0&language=ru'
```

Example payload field returned on success:

```json
{
	"text": "Ekvatorialnaya Afrika"
}
```

Note: the concrete response body also includes additional fields inherited from the shared API response model and an HMAC signature.

## `GET /Languages`

Returns the list of supported languages.

Example request:

```sh
curl --request GET --location 'http://localhost:5000/Languages'
```

Example payload fields returned on success:

```json
{
	"count": 1,
	"languages": [
		{
			"code": "ar",
			"name": "Arabic",
			"transliterator": "ArabicTransliterator"
		}
	]
}
```

Note: as with the transliteration endpoint, the concrete response body includes shared response metadata and an HMAC signature.

# Supported Languages

The API currently defines support for the following language codes:

| Code | Language |
| --- | --- |
| `ab` | Abkhaz |
| `ady` | Adyghe |
| `ar` | Arabic |
| `ary` | Maghrebi Arabic |
| `arz` | Egyptian Arabic |
| `ba` | Bashkir |
| `be` | Belarussian |
| `ber` | Berber |
| `bg` | Bulgarian |
| `bn` | Bengali |
| `cop` | Coptic |
| `cu` | Old Church Slavonic |
| `cv` | Chuvash |
| `el` | Greek |
| `grc` | Ancient Greek |
| `grc-dor` | Ancient Doric Greek |
| `gy` | Gujarati |
| `he` | Hebrew |
| `hi` | Hindi |
| `hy` | Armenian |
| `hyw` | Western Armenian |
| `iu` | Inuttitut |
| `ja` | Japanese |
| `ka` | Georgian |
| `kk` | Kazakh |
| `kn` | Kannada |
| `ko` | Korean |
| `ky` | Kyrgyz |
| `mk` | Macedonian Slavic |
| `ml` | Malayalam |
| `mn` | Mongol |
| `mr` | Marathi |
| `os` | Ossetic |
| `ru` | Russian |
| `sa` | Sanskrit |
| `sh` | Serbo-Croatian |
| `si` | Sinhala |
| `sr` | Serbian |
| `sr-ec` | Serbian |
| `ta` | Tamil |
| `te` | Telugu |
| `tg` | Tajik |
| `tg-cyrl` | Tajik |
| `tt` | Tatar |
| `tt-cyrl` | Tatar |
| `udm` | Udmurt |
| `uk` | Ukrainian |
| `zh` | Chinese |
| `zh-hans` | Simplified Chinese |

You can retrieve the authoritative list at runtime from `GET /Languages`.

# How Transliteration Is Implemented

The service chooses a transliteration strategy based on the requested language:

- built-in transliterators are used for several scripts such as Cyrillic, Greek, Hebrew, Arabic, Japanese, Korean, Gujarati, Marathi, Coptic, and Chinese Pinyin
- selected languages use external transliteration providers
- the chosen transliterator is resolved through a factory at runtime

Before storing a result in cache, the service:

1. trims leading and trailing whitespace
2. combines the normalized text, language code, and application version
3. hashes that combination with SHA-256
4. stores the transliterated result in the JSON cache file

# Running Tests

Run the unit tests with:

```sh
dotnet test TransliterationAPI.UnitTests/TransliterationAPI.UnitTests.csproj
```

# Development Notes

- The API uses controllers and conventional routing with endpoint names derived from controller names.
- Static files and default files are enabled in the ASP.NET Core pipeline.
- The cache store is created automatically when the application starts.
- Logging and exception handling are wired through the Nuci API middleware packages.

# Example Workflow

1. Start the API locally.
2. Call `GET /Languages` to discover valid language codes.
3. Call `GET /Transliteration?text=...&language=...` with one of those codes.
4. Repeated requests for the same normalized text and language can be served from cache.

# License

Licensed under GNU GPL v3. See `LICENSE` for details.