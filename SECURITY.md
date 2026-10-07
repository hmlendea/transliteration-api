# Security Policy

This security policy outlines how vulnerabilities are reported, investigated, and disclosed for the Transliteration API project. We maintain security fixes for the latest stable release and encourage responsible disclosure of any discovered vulnerabilities.

## 📑 Table of Contents

- [Supported Versions](#-supported-versions)
- [Reporting a Vulnerability](#-reporting-a-vulnerability)
- [Scope](#-scope)
- [Disclosure Policy](#-disclosure-policy)
- [Safe Harbour](#-safe-harbour)
- [Recognition](#-recognition)
- [Security Architecture](#-security-architecture)
- [Threat Model](#-threat-model)
- [Security Controls](#-security-controls)

## 🛡️ Supported Versions

Use this table to indicate which project versions currently receive security maintenance.

| Version | Distribution Channel | Supported |
|---------|----------------------|-----------|
| Latest version | GitHub Releases | ✅ |
| Preceding versions | Any distribution channel | ❌ |

## 🚨 Reporting a Vulnerability

Please do not disclose suspected vulnerabilities publicly before maintainers have had an opportunity to validate and remediate them.

To report a vulnerability:
- [GitHub Security Advisories](https://github.com/hmlendea/transliteration-api/security/advisories) — preferred channel for coordinated disclosure
- Contact the maintainers directly via GitHub issues or email

When reporting, please include:
- Description of the vulnerability and its impact
- Steps to reproduce or proof-of-concept
- Affected component(s) and version(s)
- Any suggested mitigation or fix

## 📌 Scope

The subsequent report categories are in scope for this repository:
- API endpoint vulnerabilities (authentication, authorisation, input validation)
- Dependency vulnerabilities (third-party library security issues)
- Cryptographic or HMAC signing implementation flaws
- Cache security and data exposure issues
- Configuration or environment mismanagement leading to security exposure
- HTTP header injection or response manipulation
- Scanner protection bypasses
- Logging injection or sensitive data exposure in logs

The subsequent categories are out of scope unless explicitly stated to the contrary:
- Vulnerabilities in third-party services or external transliteration providers
- User account security (this is an API without user accounts)
- Social engineering or phishing attacks
- Denial-of-service attacks without proof of concept
- Documentation or informational disclosure

## 📢 Disclosure Policy

This project follows coordinated disclosure:
1. Vulnerabilities are investigated privately.
2. A remediation plan is prepared and validated.
3. Public disclosure is published after a fix, mitigation, or agreed risk decision is available.
4. Credit is attributed in accordance with reporter preference and project policy.

## 🧾 Safe Harbour

If your research is conducted in good faith, confined to authorised scope, and disclosed responsibly, the maintainers will not pursue action for policy-compliant activity.

## 🙏 Recognition

We appreciate responsible disclosure. Reporters who desire public attribution may be acknowledged in release notes, advisories, or a dedicated acknowledgements section.

## 🏗️ Security Architecture

### Authentication and Authorisation

- **No authentication**: Both endpoints (`GET /Transliteration`, `GET /Languages`) use `NuciApiAuthorisation.None`
- **No authorisation**: No role-based access control or scopes are implemented
- **Operator responsibility**: Deploy behind appropriate network controls (reverse proxy, firewall, VPN) if access restriction is required

### Response Integrity (HMAC)

- **Algorithm**: HMAC-SHA256 via `NuciSecurity.HMAC` package
- **Key**: Configured via `securitySettings.hmacSigningKey` in `appsettings.json`
- **Scope**: Signs the entire JSON response envelope (excluding the `hmac` field itself)
- **Purpose**: Response integrity verification for clients possessing the shared key
- **Limitations**: Does not provide request authentication, confidentiality, or replay prevention

### Input Validation

- **Text length**: Maximum 256 UTF-16 code units (enforced by `[StringLength(256)]` on request DTO)
- **Language code**: Exact case-sensitive match against static registry; unknown codes return original text
- **Model validation**: ASP.NET Core model binding validates before controller execution
- **Whitespace handling**: Leading/trailing whitespace trimmed before processing

### Scanner Protection

Middleware (`NuciAPI.Middleware.Security`) executes before routing and blocks:
- **Paths**: `/.env`, `/wp-admin`, `/phpmyadmin`, `/.git`, `/config`, `/backup`, etc.
- **Query probes**: `page=gravitysmtp-settings`, `rest_route=/wp/v2/users`, `XDEBUG_SESSION_START`, `app_vl`
- **Headers**: `User-Agent` containing scanner/bot signatures, `From` header with OAI/OpenAI
- **Empty root requests**: `GET /`, `POST /`, etc. with/without content
- **Response**: 403 Forbidden with empty body; does not invoke application handlers

### Exception Handling

Middleware (`NuciAPI.Middleware.ExceptionHandling`) maps exceptions to standard HTTP responses:
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

Error responses: `{ success: false, message, code, hmac: null }` (no `text` property)

### Cache Security

- **Storage**: JSON file at configured `cacheSettings.storeLocation` (default `cache.json`)
- **Key derivation**: SHA-256 of `languageCode_unicodeCodePoints_applicationVersion`
- **Data minimisation**: Raw source text never stored; only transliterated output persisted
- **No expiry/eviction**: Cache entries persist until manual deletion or cache disable
- **File permissions**: Operator must secure filesystem access to cache file

### External Provider Communication

- **Transport**: All external provider calls use HTTPS
- **Timeout**: 3-second `HttpClient` timeout (configured in `HttpRequestManager`)
- **No retry**: Provider failures degrade to `text: null` in signed 200 response
- **Providers**: translitteration.com, ushuaia.pl, podolak.net

### Logging and Sensitive Data

- **NuciLog** structured logging captures: source text, language code, transliterated text, operation status
- **Log file**: Configured via `nuciLoggerSettings.logFilePath` (default `logfile.log`)
- **Risk**: User-supplied text enters logs; operator must implement log rotation, retention, and access controls
- **No telemetry**: No automatic crash reports, update checks, or telemetry sent to maintainers

## ⚔️ Threat Model

### Assets

1. **HMAC signing key** — Confidentiality critical; compromise allows response forgery
2. **Cache file** — Integrity important; corruption can cause service degradation
3. **Log files** — Confidentiality important; contain user-supplied text
4. **External provider communication** — Integrity and confidentiality via HTTPS

### Threat Actors

1. **External attackers** — Network-based, targeting API endpoints
2. **Malicious clients** — Authenticated or unauthenticated API consumers
3. **Compromised operators** — Insider threat with filesystem access
4. **External providers** — Third-party services with undocumented behaviour

### Attack Vectors and Mitigations

| Vector | Mitigation |
|--------|------------|
| HMAC key exposure | Operator must inject secret at runtime; placeholder in repository |
| Request smuggling | ASP.NET Core Kestrel hardening; no custom body parsing |
| Input injection | Model validation, exact language matching, no dynamic code execution |
| Cache poisoning | SHA-256 key derivation; no user control over cache keys |
| Log injection | Structured logging; operator controls log destination and access |
| Scanner/probe traffic | Pre-routing middleware blocks known patterns |
| Provider MITM | HTTPS enforced for all external calls |
| DoS via external calls | 3-second timeout; no retry; singleton HttpClient |
| Configuration leakage | Secrets in configuration; operator manages via environment |

## 🛡️ Security Controls

### Implemented Controls

- **Transport security**: HTTPS for all external provider communication
- **Response integrity**: HMAC-SHA256 signing on all successful responses
- **Input validation**: Length limits, exact matching, model binding validation
- **Scanner protection**: Pre-routing middleware with path/query/header rules
- **Exception sanitisation**: Generic error messages for 5xx responses
- **Cache key derivation**: Cryptographic hash prevents direct text recovery
- **Dependency pinning**: Exact package versions in `.csproj`
- **No secrets in repo**: Placeholder HMAC key; operator injects at runtime

### Missing Controls (Known Gaps)

- **No authentication/authorisation**: Endpoints are public
- **No rate limiting**: No per-client or global request throttling
- **No health/readiness endpoints**: No operational visibility for orchestration
- **No metrics/tracing**: No distributed tracing or Prometheus metrics
- **No cache integrity verification**: No checksums or signatures on cache file
- **No configuration validation**: No startup validation of required settings
- **No secret rotation support**: HMAC key change requires restart
- **No audit logging**: No security-relevant event log separate from application logs

### Operator Responsibilities

For self-hosted deployments, the operator is responsible for:
- Securing the `HmacSigningKey` and configuration (use environment variables or secret managers)
- Managing filesystem permissions for cache and log files
- Configuring log rotation and retention policies
- Network exposure and reverse proxy configuration (TLS termination, access controls)
- Applying .NET and dependency security updates promptly
- Monitoring logs for anomalous activity
- Implementing rate limiting at infrastructure layer if needed
