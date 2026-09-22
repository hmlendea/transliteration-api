[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/transliteration-api)](https://github.com/hmlendea/transliteration-api/releases/latest)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://gnu.org/licenses/gpl-3.0)
[![Build Status](https://github.com/hmlendea/transliteration-api/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/transliteration-api/actions/workflows/dotnet.yml)

# Transliteration API

REST API for transliterating text from multiple writing systems into the Latin alphabet.

The application is built with ASP.NET Core and targets .NET 10. It exposes an HTTP API for transliterating input text for specific language codes, listing supported languages, and caching transliteration results on disk.

## 📑 Table of Contents

- [Capabilities](#capabilities)
- [Usage](#usage)
- [System Requirements](#system-requirements)
- [Development](#development)
- [Configuration](#configuration)
- [Project Structure](#project-structure)
- [API Reference](#api-reference)
- [Supported Languages](#supported-languages)
- [Architecture](#architecture)
- [Contributing](#contributing)
- [Security](#security)
- [Supporting the Project](#supporting-the-project)
- [License](#license)

## ✨ Capabilities

- Support for 40+ languages and variants
- Multiple transliteration strategies, including built-in and external providers
- File-based cache for repeated requests
- HMAC-signed API responses
- Unit tests for transliterators and full-pipeline HTTP integration tests

## 🚀 Usage

The API provides two main endpoints for transliteration operations and language discovery.

### Transliteration Endpoint

```bash
curl --request GET \
	--location 'http://localhost:5000/Transliteration?text=%D0%AD%D0%BA%D0%B2%D0%B0%D1%82%D0%BE%D1%80%D0%B8%D0%B0%D0%BB%D1%8C%D0%BD%D0%B0%D1%8F%20%D0%90%D1%84%D1%80%D0%B8%D0%BA%D0%B0&language=ru'
```

Response payload on success:

```json
{
	"text": "Ekvatorialnaya Afrika"
}
```

### Language Discovery

```bash
curl --request GET --location 'http://localhost:5000/Languages'
```

Response payload on success:

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

## 🖥️ System Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10)

Verify the installed SDK version:

```bash
dotnet --version
```

## 🛠️ Development

### Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10)

### Setup

All NuGet dependencies are restored automatically by `dotnet restore`.

### Build

```bash
dotnet build TransliterationAPI.slnx
```

### Run

```bash
dotnet run --project TransliterationAPI/TransliterationAPI.csproj
```

By default, ASP.NET Core binds to the development URLs configured by your local environment. For explicit URL configuration:

```bash
ASPNETCORE_URLS=http://localhost:5000 dotnet run --project TransliterationAPI/TransliterationAPI.csproj
```

### Test

The solution command executes both the unit and integration test projects:

```bash
dotnet test TransliterationAPI.slnx
```

To execute only the HTTP integration tests:

```bash
dotnet test TransliterationAPI.IntegrationTests/TransliterationAPI.IntegrationTests.csproj
```

### Release

```bash
bash ./release.sh [[LATEST_RELEASE_VERSION_WITHOUT_V_PREFIX]]
```

The script downloads and executes an external release helper from `https://raw.githubusercontent.com/hmlendea/deployment-scripts/master/release/dotnet/10.sh`.

**Note:** Piping into `bash` is an intensely controversial topic. Please review any external scripts before running them in your environment!

## ⚙️ Configuration

The application reads configuration from `TransliterationAPI/appsettings.json`.

| Section | Key | Description |
|---------|-----|-------------|
| `cacheSettings` | `storeLocation` | Path to the JSON file used for cached transliteration results |
| `cacheSettings` | `enabled` | Flag to control cache usage |
| `securitySettings` | `hmacSigningKey` | Secret used to sign API responses |
| `nuciLoggerSettings` | `logFilePath` | Path to the log file |
| `nuciLoggerSettings` | `isFileOutputEnabled` | Flag to enable file logging |

The cache file is created automatically on startup if it does not exist.

### Configuration Example

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

## 🗂️ Project Structure

The solution contains the following projects:

- `TransliterationAPI`: Main ASP.NET Core API project
- `TransliterationAPI.IntegrationTests`: NUnit full-pipeline HTTP integration test project
- `TransliterationAPI.UnitTests`: NUnit test project

The key directories inside `TransliterationAPI/` are:

| Directory | Purpose |
|-----------|---------|
| `API/Controllers/` | HTTP endpoints |
| `API/Requests/` | Request models |
| `API/Responses/` | Response models |
| `Configuration/` | Configuration models |
| `Logging/` | Logging utilities |
| `Service/` | Business logic, cache access, HTTP integrations |
| `Service/Transliterators/` | Language-specific transliteration implementations |

## 🌐 API Reference

### GET /Transliteration

Transliterates input text for a specific language.

**Query parameters:**

- `text` - input text to transliterate (limited to 256 characters)
- `language` - supported language code

**Behaviour:**

- Leading and trailing whitespace is trimmed before processing
- If the language code is not supported, the original text is returned unchanged
- Successful results may be stored in the JSON cache
- The response includes an HMAC signature

### GET /Languages

Returns the list of supported languages and their transliterator implementations.

**Response includes:**

- `count` - number of supported languages
- `languages` - array of language objects with `code`, `name`, and `transliterator` fields
- HMAC signature for response verification

## 📋 Supported Languages

The API currently supports 40+ languages and variants. You can retrieve the authoritative list at runtime from `GET /Languages`.

Common supported languages include (but are not limited to):

| Code | Language |
| --- | --- |
| `ar` | Arabic |
| `be` | Belarussian |
| `bg` | Bulgarian |
| `el` | Greek |
| `grc` | Ancient Greek |
| `he` | Hebrew |
| `ja` | Japanese |
| `ka` | Georgian |
| `ko` | Korean |
| `ru` | Russian |
| `uk` | Ukrainian |
| `zh` | Chinese |

## 🏗️ Architecture

See the [architecture documentation](./ARCHITECTURE.md) for verified system boundaries, runtime flows, dependencies, constraints, and extension points.

### Transliteration Implementation

The service chooses a transliteration strategy based on the requested language:

- Built-in transliterators are used for Cyrillic, Greek, Hebrew, Arabic, Japanese, Korean, Gujarati, Marathi, Coptic, and Chinese Pinyin scripts
- Selected languages use external transliteration providers
- The appropriate transliterator is resolved through a factory at runtime

### Caching Strategy

Before storing a result in cache, the service performs the following:

1. Trims leading and trailing whitespace
2. Combines the normalised text, language code, and application version
3. Hashes the combination with SHA-256
4. Stores the transliterated result in the JSON cache file

### Development Notes

- The API uses controllers and conventional routing with endpoint names derived from controller names
- Static files and default files are enabled in the ASP.NET Core pipeline
- The cache store is created automatically on application startup
- Logging and exception handling are wired through the Nuci API middleware packages

## 🤝 Contributing

You are welcome to submit any suggestion, feedback, or modification to this project.

When doing so, please:

- Maintain cross-platform compatibility
- Maintain the existing public contract intact unless a breaking change is intentional
- Maintain the pull requests as focused and consistent with the existing code style
- Maintain your branch up-to-date with `master`
- Revise the documentation when behaviour changes
- Properly test all changes, including edge cases and error conditions
- Add unit tests for any new or changed functionality

## 🔒 Security

For information on reporting security vulnerabilities, see [SECURITY.md](./SECURITY.md).

## 💝 Supporting the Project

Discovered a problem or have a suggestion? [Open an issue](https://github.com/hmlendea/transliteration-api/issues)!

If you find this project useful, consider [funding it](https://hmlendea.go.ro/funding) or starring ⭐️ it on GitHub!

[![Donate](https://raw.githubusercontent.com/hmlendea/readme-assets/master/donate_generic.png)](https://hmlendea.go.ro/funding)

## 📄 License

This project is being distributed under the `GNU General Public License v3.0 or later`.
See [LICENSE](./LICENSE) for further information.
