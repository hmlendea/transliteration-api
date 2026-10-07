# Transliteration API — Security Architecture

## Threat Model

| Asset | Threat | Mitigation |
|-------|--------|------------|
| User input text | Injection, XSS, oversized payload | Input validation, size limits, output encoding |
| External provider responses | Malformed HTML, script injection | Strict parsing, no eval, sanitization |
| Cache files | Path traversal, corruption | SHA-256 keys, directory isolation |
| HMAC signing key | Disclosure, tampering | Environment variable, rotation |
| Network traffic | Eavesdropping, MITM | HTTPS in production (operator responsibility) |
| Scanner probes | Reconnaissance, exploitation | ScannerProtectionMiddleware blocks known patterns |

## Security Layers

### 1. ScannerProtectionMiddleware (First Line)

**Location:** `NuciAPI.Middleware.Security` package (registered via `AddNuciApiScannerProtection()`)

**Blocks:**
- **Paths:** `.env`, `.git`, `wp-admin`, `wp-login.php`, `xmlrpc.php`, `phpmyadmin`, `adminer.php`, `config.php`, `backup`, `dump.sql`, `database.sql`, `shell.php`, `cmd.php`, `eval.php`, `test.php`, `info.php`, `phpinfo.php`, `server-status`, `server-info`, `actuator`, `metrics`, `health`, `swagger`, `api-docs`, `openapi`
- **Query parameters:** `gravitysmtp`, `wp/v2/users`, `XDEBUG`, `app_vl`, `debug`, `test`, `shell`, `cmd`, `exec`, `eval`
- **Headers:** `User-Agent` containing `sqlmap`, `nikto`, `nessus`, `openvas`, `acunetix`, `burp`, `zap`, `w3af`, `skipfish`, `wpscan`, `dirb`, `gobuster`, `ffuf`, `feroxbuster`; `From` header containing `openai`, `gpt`, `bot`, `crawler`, `spider`

**Response:** 403 Forensic (no body, no HMAC, logged)

**Pipeline position:** First middleware after `UseNuciApiRequestLogging()` in `Startup.Configure()`

### 2. ExceptionHandlingMiddleware

**Location:** `NuciAPI.Middleware.ExceptionHandling` package (registered via `AddNuciApiExceptionHandling()`)

**Maps exceptions to HTTP responses:**

| Exception | Status | Code | Message |
|-----------|--------|------|---------|
| `ArgumentNullException` | 400 | `ArgumentNull` | Parameter cannot be null |
| `ArgumentException` | 400 | `Argument` | Invalid argument |
| `KeyNotFoundException` | 404 | `NotFound` | Resource not found |
| `UnauthorizedAccessException` | 401 | `Unauthorized` | Unauthorized |
| `NotSupportedException` | 405 | `NotSupported` | Operation not supported |
| `TimeoutException` | 504 | `Timeout` | Operation timed out |
| `HttpRequestException` | 502 | `BadGateway` | Upstream service unavailable |
| `Exception` (fallback) | 500 | `InternalError` | Internal server error |

**Security properties:**
- No stack traces in responses
- No internal details leaked
- HMAC signature = null on error responses
- Structured logging with correlation ID

**Pipeline position:** Second middleware after ScannerProtection

### 3. HMAC Response Signing

**Implementation:** `NuciSecurity.HMAC` 4.1.3 (applied via `NuciApiController` base class)

**Algorithm:** HMAC-SHA256

**Key:** `SecuritySettings.HmacSigningKey` (from `appsettings.json` or env var `TRANSLITERATION_API__SECURITYSETTINGS__HMACSIGNINGKEY`)

**Scope:** All successful JSON responses (2xx) from controllers inheriting `NuciApiController`

**Header:** `X-HMAC-SHA256: <base64>`

**Verification (client):**
```csharp
var hmac = new HMACSHA256(key);
var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(responseBody));
var received = Convert.FromBase64String(responseHeaders["X-HMAC-SHA256"]);
Assert.That(computed, Is.EqualTo(received));
```

**Key rotation:** Update environment variable and restart; old signatures invalid immediately.

**Controllers using HMAC:** `LanguagesController`, `TransliterationController` (both inherit `NuciApiController`)

### 4. Input Validation

**Transliteration endpoint (`GET /Transliteration`):**
- `languageCode`: Required, must exist in registry (validated by `Language.FromCode()` which throws `ArgumentException` if not found)
- `text`: Required, max 256 chars (hardcoded in `TransliterationRequest` model binding)
- Whitespace trimmed before processing (`NormaliseText()` in `TransliterationService`)
- Empty text after trim → returns original text (no transliteration needed)

**Languages endpoint (`GET /Languages`):** No input parameters

**Model binding:** `TransliterationRequest` with `[FromQuery]` attributes

### 5. Output Encoding

- All JSON responses: UTF-8, no BOM (ASP.NET Core default)
- HTML from external providers: Parsed via `HtmlAgilityPack`, only text content extracted via `InnerText`/`SelectNodes`
- No raw HTML ever returned to client
- HMAC covers exact response bytes as serialized by `System.Text.Json`

### 6. Cache Security

**Key generation (in `TransliterationService.GetCacheId()`):**
```csharp
string textUnicodes = string.Join('-', text.Select(c => (int)c));
string cacheKey = $"{languageCode}_{textUnicodes}_{cacheSettings.ApplicationVersion}";
return SHA256.HashData(Encoding.ASCII.GetBytes(cacheKey)).ToHexString();
```

**Properties:**
- Keys are opaque SHA-256 hashes (no PII in filenames or keys)
- Cache file: single JSON array at `CacheSettings.StoreLocation` (default `cache.json`)
- File permissions: OS default (operator must restrict to service account)
- No encryption at rest (operator responsibility)
- App version in key = automatic invalidation on deploy (computed from `Assembly.GetEntryAssembly().GetName().Version`)

**Cache entity (`CachedTransliteration`):**
```csharp
public class CachedTransliteration : EntityBase
{
    public string TransliteratedText { get; set; }
    // Id inherited from EntityBase (string) — stores the SHA-256 cache key
}
```

### 7. External Provider Communication

| Provider | Protocol | Security |
|----------|----------|----------|
| transliteration.com | HTTP POST (form) | No auth, response prefix validated (`"result":`) |
| Ushuaia | HTTP GET (cookies) → POST (form) | Session cookies, Referer header, cookie TTL 5 min |
| Podolak | HTTP POST (form) | No auth, HTML parsing only |

**Shared HTTP client:** `HttpRequestManager` (singleton, `HttpClient` with `HttpClientHandler`)

**Protections (configured in `HttpRequestManager`):**
- Timeouts: 30s connect, 60s read (configurable via `HttpRequestSettings`)
- No redirects followed (`AllowAutoRedirect = false`)
- Response size limit: 1MB (`MaxResponseSize`)
- TLS verification: enabled (uses system trust store)
- No credentials stored (all public endpoints)

**Provider-specific details:**

**TranslitterationDotComTransliterator:**
- POST to `https://transliteration.com/transliterate`
- Form data: `text`, `lang`
- Response: JSON with `result` field
- Validates response starts with `"result":`

**UshuaiaTransliterator:**
- GET `https://ushuaia.pl` to establish session cookies
- POST to `https://ushuaia.pl/transliterate` with cookies
- Form data: `text`, `lang`
- Cookie TTL: 5 minutes (cached in memory)
- Referer header set to `https://ushuaia.pl`

**PodolakTransliterator:**
- POST to `https://podolak.pl/transliterate`
- Form data: `text`, `lang`
- Response: HTML table, parsed via `HtmlAgilityPack`

### 8. Logging Security

**Framework:** `NuciLog` 1.2.1 + `NuciLog.Core` 3.1.0

**Masked fields (automatic via NuciLog):**
- `Authorization` headers
- `Cookie` headers
- `X-HMAC-SHA256` headers
- Query parameters: `password`, `token`, `secret`, `key`, `api_key`, `apikey`

**Logged per request (via `NuciApiRequestLoggingMiddleware`):**
- Correlation ID (`X-Correlation-ID` header or generated)
- HTTP method + path
- Remote IP (from `X-Forwarded-For` or socket)
- Response status + duration
- HMAC present/absent (not value)

**Logged per transliteration (via `TransliterationService`):**
- Operation: `MyOperation.Transliteration`
- Status: Started/Success/Failure
- Language code
- Text length (not content)
- Transliterated text (on success)
- Duration

**Not logged:**
- Request body (text content)
- Response body
- Cache keys/values

**Log output:** File (`NuciLoggerSettings.LogFilePath`) + console (development)

### 9. Authentication / Authorization

**Current state:** `NuciApiAuthorisation.None` (no auth required)

**Rationale:** Public transliteration API, no user accounts

**Operator options:**
- Deploy behind API gateway with auth (OAuth2, API keys, etc.)
- Add `NuciAPI.Middleware.Authorization` package
- Implement custom middleware before `UseEndpoints()`

**Authorization header:** Not validated by application; passed through to external providers if needed

### 10. Dependency Security

**NuGet packages (pinned versions from `TransliterationAPI.csproj`):**
```
NuciAPI 3.6.1
NuciAPI.Controllers 2.3.1
NuciAPI.Middleware 2.0.3
NuciAPI.Middleware.ExceptionHandling 1.0.2
NuciAPI.Middleware.Logging 1.0.1
NuciAPI.Middleware.Security 1.0.6
NuciDAL 3.2.1
NuciLog 1.2.1
NuciLog.Core 3.1.0
NuciSecurity.HMAC 4.1.3
NuciExtensions 5.3.2
CHSPinYinConv 1.0.0
```

**Update policy:** Monthly `dotnet list package --vulnerable --include-transitive` in CI

**Supply chain:** All packages from nuget.org, signed, verified checksums

**Transitive dependencies:** Monitored via `dotnet list package --include-transitive`

## Configuration Security

### appsettings.json (Production Template)

```json
{
  "securitySettings": {
    "hmacSigningKey": "[[TRANSLITERATION_API_HMAC_SIGNING_KEY]]"
  },
  "cacheSettings": {
    "storeLocation": "/var/lib/transliteration-api/cache.json",
    "enabled": "true"
  },
  "nuciLoggerSettings": {
    "logFilePath": "/var/log/transliteration-api/app.log",
    "isFileOutputEnabled": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.AspNetCore.Hosting": "Warning",
      "Microsoft.AspNetCore.Mvc": "Warning",
      "TransliterationAPI": "Debug"
    }
  },
  "AllowedHosts": "api.example.com"
}
```

**Note:** Section names match class names exactly (`securitySettings`, `cacheSettings`, `nuciLoggerSettings`).

### Environment Variables (Preferred for Secrets)

```bash
export TRANSLITERATION_API__SECURITYSETTINGS__HMACSIGNINGKEY="$(openssl rand -base64 32)"
export TRANSLITERATION_API__CACHESETTINGS__STORELOCATION="/var/lib/transliteration-api/cache.json"
export TRANSLITERATION_API__NUCILOGGERSETTINGS__LOGFILEPATH="/var/log/transliteration-api/app.log"
```

### File Permissions (Operator Responsibility)

```bash
# Cache directory
chown transliteration-api:transliteration-api /var/lib/transliteration-api
chmod 700 /var/lib/transliteration-api

# Log directory
chown transliteration-api:transliteration-api /var/log/transliteration-api
chmod 750 /var/log/transliteration-api

# Config file (if file-based)
chown root:transliteration-api /etc/transliteration-api/appsettings.json
chmod 640 /etc/transliteration-api/appsettings.json
```

## Deployment Security Checklist

- [ ] HMAC key generated via `openssl rand -base64 32`
- [ ] HMAC key stored in environment variable (not config file)
- [ ] Cache directory on dedicated volume, 700 permissions
- [ ] Log directory on dedicated volume, 750 permissions
- [ ] HTTPS termination (nginx, Caddy, Traefik, cloud LB)
- [ ] Rate limiting at gateway (e.g., 100 req/min/IP)
- [ ] Request size limit at gateway (e.g., 1MB)
- [ ] ScannerProtectionMiddleware enabled (default)
- [ ] ExceptionHandlingMiddleware enabled (default)
- [ ] Logging to secure destination (not world-readable)
- [ ] Regular `dotnet list package --vulnerable` in CI
- [ ] Dependency updates tested in staging before production
- [ ] `AllowedHosts` configured to expected hostname(s)

## Incident Response

### HMAC Key Compromise
1. Generate new key: `openssl rand -base64 32`
2. Update environment variable
3. Restart service (zero-downtime: rolling restart)
4. All existing signatures immediately invalid
5. Clients must retry (401 on signature mismatch)

### Cache Poisoning
1. Stop service
2. Delete cache file: `rm -f /var/lib/transliteration-api/cache.json`
3. Restart service
4. Investigate log for anomalous requests

### External Provider Compromise
1. Disable affected language in `LanguageRegistry` (code change)
2. Deploy hotfix
3. Monitor for anomalous responses
4. Re-enable after provider confirms fix

## Compliance Notes

- **GDPR:** No personal data processed (text is transient, not stored; cache stores only transliterated output)
- **CCPA:** No personal data sold or shared
- **SOC 2:** Operator responsible for infrastructure controls
- **PCI DSS:** Not applicable (no payment data)
- **HIPAA:** Not applicable (no health data)

## Security Contacts

- **Repository:** https://github.com/hmlendea/transliteration-api
- **Issues:** https://github.com/hmlendea/transliteration-api/issues
- **Security:** Use GitHub Security Advisories (private reporting)

```bash
# Cache directory
chown transliteration-api:transliteration-api /var/lib/transliteration-api/cache
chmod 700 /var/lib/transliteration-api/cache

# Config file
chown root:transliteration-api /etc/transliteration-api/appsettings.json
chmod 640 /etc/transliteration-api/appsettings.json
```

## Deployment Security Checklist

- [ ] HMAC key generated via `openssl rand -base64 32`
- [ ] HMAC key stored in environment variable (not config file)
- [ ] Cache directory on dedicated volume, 700 permissions
- [ ] HTTPS termination (nginx, Caddy, Traefik, cloud LB)
- [ ] Rate limiting at gateway (e.g., 100 req/min/IP)
- [ ] Request size limit at gateway (e.g., 1MB)
- [ ] ScannerProtectionMiddleware enabled (default)
- [ ] ExceptionHandlingMiddleware enabled (default)
- [ ] Logging to secure destination (not world-readable)
- [ ] Regular `dotnet list package --vulnerable` in CI
- [ ] Dependency updates tested in staging before production

## Incident Response

### HMAC Key Compromise
1. Generate new key: `openssl rand -base64 32`
2. Update environment variable
3. Restart service (zero-downtime: rolling restart)
4. All existing signatures immediately invalid
5. Clients must retry (401 on signature mismatch)

### Cache Poisoning
1. Stop service
2. Delete cache directory: `rm -rf /var/lib/transliteration-api/cache/*`
3. Restart service
4. Investigate log for anomalous requests

### External Provider Compromise
1. Disable affected language in `LanguageRegistry` (code change)
2. Deploy hotfix
3. Monitor for anomalous responses
4. Re-enable after provider confirms fix

## Compliance Notes

- **GDPR:** No personal data processed (text is transient, not stored)
- **CCPA:** No personal data sold or shared
- **SOC 2:** Operator responsible for infrastructure controls
- **PCI DSS:** Not applicable (no payment data)
- **HIPAA:** Not applicable (no health data)

## Security Contacts

- **Repository:** https://github.com/hmlendea/transliteration-api
- **Issues:** https://github.com/hmlendea/transliteration-api/issues
- **Security:** Use GitHub Security Advisories (private reporting)