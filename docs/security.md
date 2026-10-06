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

**Location:** `NuciAPI.Middleware.Security` package

**Blocks:**
- **Paths:** `.env`, `.git`, `wp-admin`, `wp-login.php`, `xmlrpc.php`, `phpmyadmin`, `adminer.php`, `config.php`, `backup`, `dump.sql`, `database.sql`, `shell.php`, `cmd.php`, `eval.php`, `test.php`, `info.php`, `phpinfo.php`, `server-status`, `server-info`, `actuator`, `metrics`, `health`, `swagger`, `api-docs`, `openapi`
- **Query parameters:** `gravitysmtp`, `wp/v2/users`, `XDEBUG`, `app_vl`, `debug`, `test`, `shell`, `cmd`, `exec`, `eval`
- **Headers:** `User-Agent` containing `sqlmap`, `nikto`, `nessus`, `openvas`, `acunetix`, `burp`, `zap`, `w3af`, `skipfish`, `wpscan`, `dirb`, `gobuster`, `ffuf`, `feroxbuster`; `From` header containing `openai`, `gpt`, `bot`, `crawler`, `spider`

**Response:** 403 Forensic (no body, no HMAC, logged)

### 2. ExceptionHandlingMiddleware

**Location:** `NuciAPI.Middleware.ExceptionHandling` package

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

### 3. HMAC Response Signing

**Implementation:** `NuciSecurity.HMAC` 4.1.3

**Algorithm:** HMAC-SHA256

**Key:** `SecuritySettings.HmacKey` (from `appsettings.json` or env var)

**Scope:** All successful JSON responses (2xx)

**Header:** `X-HMAC-SHA256: <base64>`

**Verification (client):**
```csharp
var hmac = new HMACSHA256(key);
var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(responseBody));
var received = Convert.FromBase64String(responseHeaders["X-HMAC-SHA256"]);
Assert.That(computed, Is.EqualTo(received));
```

**Key rotation:** Update `appsettings.json` and restart; old signatures invalid immediately.

### 4. Input Validation

**Transliteration endpoint:**
- `languageCode`: Required, must exist in registry (validated by `Language.GetByCode()`)
- `text`: Required, max 10,000 chars (configurable via `CacheSettings.MaxTextLength`)
- Whitespace trimmed before processing
- Empty text → 400 Bad Request

**Languages endpoint:** No input parameters

### 5. Output Encoding

- All JSON responses: UTF-8, no BOM
- HTML from external providers: Parsed via `HtmlAgilityPack`, only text content extracted
- No raw HTML ever returned to client
- HMAC covers exact response bytes

### 6. Cache Security

**Key generation:** SHA-256(`languageCode` + `|` + `normalizedText` + `|` + `appVersion`)

**Properties:**
- Keys are opaque hashes (no PII in filenames)
- Cache directory: configurable, default `./cache/`
- File permissions: OS default (operator must restrict)
- No encryption at rest (operator responsibility)
- App version in key = automatic invalidation on deploy

### 7. External Provider Communication

| Provider | Protocol | Security |
|----------|----------|----------|
| transliteration.com | HTTP POST (form) | No auth, response prefix validated |
| Ushuaia | HTTP GET (cookies) → POST (form) | Session cookies, Referer header |
| Podolak | HTTP POST (form) | No auth, HTML parsing only |

**Protections:**
- Timeouts: 30s connect, 60s read (configurable)
- No redirects followed
- Response size limit: 1MB
- TLS verification: enabled (operator must ensure valid certs)
- No credentials stored (all public endpoints)

### 8. Logging Security

**Framework:** `NuciLog` 1.2.1 + `NuciLog.Core` 3.1.0

**Masked fields (automatic):**
- `Authorization` headers
- `Cookie` headers
- `X-HMAC-SHA256` headers
- Query parameters: `password`, `token`, `secret`, `key`, `api_key`, `apikey`

**Logged per request:**
- Correlation ID
- HTTP method + path
- Remote IP (from `X-Forwarded-For` or socket)
- Response status + duration
- HMAC present/absent (not value)

**Not logged:**
- Request body (text content)
- Response body
- Cache keys/values

### 9. Authentication / Authorization

**Current state:** `NuciApiAuthorisation.None` (no auth required)

**Rationale:** Public transliteration API, no user accounts

**Operator options:**
- Deploy behind API gateway with auth
- Add `NuciAPI.Middleware.Authorization` package
- Implement custom middleware before `UseTransliterationApi()`

### 10. Dependency Security

**NuGet packages (pinned versions):**
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

**Update policy:** Monthly `dotnet list package --vulnerable --include-transitive`

**Supply chain:** All packages from nuget.org, signed, verified checksums

## Configuration Security

### appsettings.json (Production Template)

```json
{
  "Security": {
    "HmacKey": "REPLACE_WITH_32_BYTE_BASE64_KEY",
    "AllowedHosts": "api.example.com"
  },
  "Cache": {
    "Directory": "/var/lib/transliteration-api/cache",
    "MaxTextLength": 10000
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Environment Variables (Preferred for Secrets)

```bash
export TRANSLITERATION_API__SECURITY__HMACKEY="$(openssl rand -base64 32)"
export TRANSLITERATION_API__CACHE__DIRECTORY="/var/lib/transliteration-api/cache"
```

### File Permissions (Operator Responsibility)

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