# Privacy and Personal Data

This document describes how the Transliteration API handles personal data. It covers the application behaviour and verified integrations for self-hosted deployments. The instance operator controls their instance's configuration, local storage, logs, backups, access controls, retention, and request handling.

**Information reviewed:** 2026-10-06

## 📑 Table of Contents

- [What This Document Covers](#-what-this-document-covers)
- [Self-Hosted Deployments](#-self-hosted-deployments)
- [Data We Handle](#-data-we-handle)
- [Processing and Use](#-processing-and-use)
- [Storage, Retention, and Deletion](#-storage-retention-and-deletion)
- [External Processing and Integrations](#-external-processing-and-integrations)
- [Data Protection and Security](#-data-protection-and-security)
- [Data Subject Rights](#-data-subject-rights)
- [Data Locations](#-data-locations)
- [Telemetry Controls](#-telemetry-controls)
- [Document Changes](#-document-changes)
- [Contact](#-contact)

## 🔎 What This Document Covers

This document describes how Transliteration API at https://github.com/hmlendea/transliteration-api handles personal data. It covers the application behaviour and verified integrations described below. Where the software is self-hosted, the instance operator may have separate responsibilities described below.

## 🏠 Self-Hosted Deployments

This document covers the Transliteration API application behaviour. The instance operator controls their instance's configuration, local storage, logs, backups, access controls, retention, and request handling. The project maintainers do not operate a hosted service and do not receive data from self-hosted instances.

The application sends normalised source text to three verified external transliteration providers when the requested language is assigned to an external transliterator:
- **translitteration.com** — HTTPS form POST with text, target language code, script, and scheme
- **ushuaia.pl** — HTTPS cookie retrieval and form POST with text and language identifier
- **podolak.net** — HTTPS form POST with text and transliteration parameters

No telemetry, update checks, crash reports, or other data are sent to project maintainers. Operators can disable external provider use by configuring only languages that use local transliterators, or by modifying the language registry.

## 📥 Data We Handle

### Data Provided to the Application

- **Source text** — Unicode text submitted by the client in the `text` query parameter for transliteration (maximum 256 UTF-16 code units)
- **Language code** — Language identifier submitted by the client in the `language` query parameter to select the transliteration strategy (case-sensitive, exact match against registry)

No other personal data is requested or collected from users, administrators, or connected systems. The API has no user accounts, authentication, sessions, cookies, or tracking identifiers.

### Data Generated or Collected by the Application

- **Transliterated text** — Latin-alphabet output produced by the selected transliterator
- **Cache entries** — SHA-256 hash of normalised text, language code, and application version as key; transliterated text as value
- **Structured logs** — Request text, language code, transliterated text, operation status, and timestamps written to the configured log destination
- **HMAC signatures** — Response signatures computed using the configured `HmacSigningKey`

No telemetry, crash reports, update checks, or other automatic data collection occurs.

### Data Received from Integrations

- **Transliteration results** — Latin-alphabet text returned by external providers (translitteration.com, ushuaia.pl, podolak.net) for languages assigned to external transliterators

No other personal data is received from integrations or third parties.

## 🧭 Processing and Use

The application processes the data described above for these verified functions:
- **Transliteration** — Source text and language code
- **Language discovery** — No personal data (returns static language registry)
- **Cache lookup and storage** — Normalised text, language code, application version hash, and transliterated text
- **Response signing** — Transliterated text and HMAC signing key
- **Structured logging** — Source text, language code, transliterated text, operation status

Legal basis for processing (where applicable): legitimate interest of the operator in providing the transliteration service; no consent mechanism is implemented as the API has no user accounts.

## 🗄️ Storage, Retention, and Deletion

| Data category | Storage location | Retention and deletion |
|---------------|------------------|------------------------|
| Cache entries | JSON file at configured `cacheSettings.storeLocation` (default: `cache.json`) | Persisted until operator deletes the file or disables caching via `cacheSettings.enabled: false`. No automatic expiry. |
| Log entries | File at configured `nuciLoggerSettings.logFilePath` (default: `logfile.log`) | Controlled by operator's log rotation and retention policies. |
| HMAC signing key | Configuration (`securitySettings.hmacSigningKey`) | Stored in configuration; operator manages secret rotation. |
| In-memory request data | Process memory | Discarded after each request completes. |
| Ushuaia session cookie | In-memory (singleton `UshuaiaTransliterator`) | Refreshed after 5 minutes; discarded on process restart. |
| External provider responses | In-memory (request scope) | Discarded after response construction. |

For self-hosted deployments, the instance operator controls all local storage, deletion, and backups. The project does not operate a centralised service and does not retain any data from self-hosted instances.

## 🔗 External Processing and Integrations

| Service or integration | Purpose | Data involved | Configuration or documentation |
|-----------------------|---------|---------------|--------------------------------|
| translitteration.com | Transliteration for Abkhaz, Adyghe, Armenian, Bashkir, Georgian, Inuttitut, Kyrgyz, Ossetic, Udmurt, Western Armenian | Source text, target language code, script (latn), transliteration scheme | https://www.translitteration.com/ |
| ushuaia.pl | Transliteration for Bengali, Hindi, Kannada, Malayalam, Mongol, Sanskrit, Sinhala, Tamil, Telugu | Source text, language identifier, session cookie | https://www.ushuaia.pl/transliterate/ |
| podolak.net | Transliteration for Old Church Slavonic | Source text, fixed form parameters (ISO-9 scheme) | https://podolak.net/en/transliteration/old-church-slavonic |

The application has no other built-in external data transfers. External provider use is determined by the language registry; operators can restrict to local-only transliterators by modifying the registry.

### Data Flow to External Providers

1. Client requests transliteration for a language mapped to an external provider
2. Service normalises text (trims whitespace)
3. Service computes cache key; checks cache (if enabled)
4. On cache miss: external transliterator constructs HTTP request
5. Request sent via HTTPS to provider endpoint with form-encoded body
6. Provider response parsed; result returned to service
7. Service caches result (if enabled and non-whitespace)
8. Service returns signed response to client

**Data minimisation**: Only the normalised source text and mapped language code are transmitted. No client IP, headers, or identifiers are forwarded.

## 🛡️ Data Protection and Security

- **Transport security:** All external provider communication uses HTTPS.
- **Response integrity:** Successful responses are HMAC-signed using the configured `HmacSigningKey`. The key is not an authentication mechanism or encryption key for request data.
- **No authentication:** Endpoints use `NuciApiAuthorisation.None`; operators should deploy behind appropriate network controls if access restriction is required.
- **Input handling:** Source text is trimmed of leading/trailing whitespace before processing.
- **Cache keys:** SHA-256 hashes prevent direct text recovery from the cache file.
- **Operator responsibilities:** For self-hosted deployments, the operator is responsible for:
  - Securing the `HmacSigningKey` and configuration
  - Managing filesystem permissions for cache and log files
  - Configuring log rotation and retention
  - Network exposure and reverse proxy configuration
  - Applying .NET and dependency security updates

No absolute security is promised.

## 👤 Data Subject Rights

As the API has no user accounts or persistent identifiers, traditional data subject rights (access, rectification, erasure, portability, restriction, objection) apply as follows:

| Right | Applicability | Mechanism |
|-------|---------------|-----------|
| Access | Cache entries contain only hashed keys and transliterated output; source text not stored | Operator can inspect cache file directly |
| Rectification | No persistent personal data linked to identifiers | N/A |
| Erasure | Delete cache file; rotate logs | Operator-controlled |
| Portability | Cache is JSON; logs are text | Operator can export files |
| Restriction | Disable caching via config; adjust log level | Configuration changes |
| Objection | Block client at network layer | Infrastructure controls |

Operators of self-hosted instances should implement their own processes for handling data subject requests.

## 📍 Data Locations

| Platform or Scope | Location | Contents |
|-------------------|----------|----------|
| Cache file | `cacheSettings.storeLocation` (default: `./cache.json`) | JSON array of `{ "id": "sha256...", "transliteratedText": "..." }` |
| Log file | `nuciLoggerSettings.logFilePath` (default: `./logfile.log`) | Structured log lines with timestamp, operation, status, text, language, transliterator |
| Configuration | `TransliterationAPI/appsettings.json` | HMAC key (placeholder), cache/log paths, feature flags |
| In-memory | Process heap | Request-scoped strings; singleton Ushuaia session cookie |

## 📊 Telemetry Controls

The application has **no built-in telemetry, analytics, diagnostics, or crash reporting**. The following controls are available to operators:

| Control | Mechanism |
|---------|-----------|
| Disable file logging | Set `nuciLoggerSettings.isFileOutputEnabled: false` |
| Adjust log destination | Modify `nuciLoggerSettings.logFilePath` or configure NuciLog programmatically |
| Disable caching | Set `cacheSettings.enabled: false` |
| Restrict to local transliterators | Remove external language entries from `Language.cs` registry |
| Network egress control | Firewall rules blocking outbound HTTPS to provider domains |

No opt-out is required because no telemetry exists.

## 🔄 Document Changes

Update this document when application data flows, storage, integrations, or deployment responsibilities change. The current version is published at https://github.com/hmlendea/transliteration-api/blob/main/PRIVACY.md.

## 📬 Contact

For questions about application data handling, contact the project maintainers via GitHub issues at https://github.com/hmlendea/transliteration-api/issues. For a self-hosted instance, contact the instance operator. Include the deployment URL and language code if relevant; do not send passwords, access tokens, or other secrets.