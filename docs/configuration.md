# Transliteration API — Configuration Reference

## Configuration Sources (Priority Order)

1. **appsettings.json** (base)
2. **appsettings.{Environment}.json** (environment-specific)
3. **Environment variables** (`TRANSLITERATION_API__` prefix)
4. **Command line arguments** (highest)

## Settings Classes

### CacheSettings (`TransliterationAPI/Configuration/CacheSettings.cs`)

```csharp
public sealed class CacheSettings
{
    public string ApplicationVersion
        => Assembly.GetEntryAssembly()?.GetName().Version?.ToString();

    public string StoreLocation { get; set; }

    public bool Enabled { get; set; } = true;
}
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ApplicationVersion` | string (read-only) | Assembly version | Computed from entry assembly; used in cache key for automatic invalidation on deploy |
| `StoreLocation` | string | *required* | Relative or absolute path to cache JSON file (e.g., `cache.json` or `/var/lib/transliteration-api/cache.json`) |
| `Enabled` | bool | `true` | Master toggle for cache read/write |

**Environment variables:**
```bash
TRANSLITERATION_API__CACHESETTINGS__STORELOCATION=/var/lib/transliteration-api/cache.json
TRANSLITERATION_API__CACHESETTINGS__ENABLED=true
```

### SecuritySettings (`TransliterationAPI/Configuration/SecuritySettings.cs`)

```csharp
public sealed class SecuritySettings
{
    public string HmacSigningKey { get; set; }
}
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `HmacSigningKey` | string | *required in production* | Base64-encoded 32-byte (256-bit) key for HMAC-SHA256 response signing; 44 chars base64 |

**Environment variables:**
```bash
TRANSLITERATION_API__SECURITYSETTINGS__HMACSIGNINGKEY="$(openssl rand -base64 32)"
```

### NuciLoggerSettings (from NuciLog)

```csharp
public sealed class NuciLoggerSettings
{
    public string LogFilePath { get; set; }
    public bool IsFileOutputEnabled { get; set; }
}
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `LogFilePath` | string | `logfile.log` | Path to log file (relative to working dir or absolute) |
| `IsFileOutputEnabled` | bool | `true` | Enable file logging output |

**Environment variables:**
```bash
TRANSLITERATION_API__NUCILOGGERSETTINGS__LOGFILEPATH=/var/log/transliteration-api/app.log
TRANSLITERATION_API__NUCILOGGERSETTINGS__ISFILEOUTPUTENABLED=true
```

## appsettings.json (Complete Example)

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
  "AllowedHosts": "*"
}
```

**Note:** Section names match class names exactly (`cacheSettings`, `securitySettings`, `nuciLoggerSettings`), not PascalCase.

## Environment-Specific Overrides

### appsettings.Development.json

```json
{
  "cacheSettings": {
    "storeLocation": "cache-dev.json",
    "enabled": "true"
  },
  "nuciLoggerSettings": {
    "logFilePath": "logfile-dev.log",
    "isFileOutputEnabled": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "TransliterationAPI": "Trace"
    }
  }
}
```

### appsettings.Production.json

```json
{
  "cacheSettings": {
    "storeLocation": "/var/lib/transliteration-api/cache.json",
    "enabled": "true"
  },
  "securitySettings": {
    "hmacSigningKey": "[[TRANSLITERATION_API_HMAC_SIGNING_KEY]]"
  },
  "nuciLoggerSettings": {
    "logFilePath": "/var/log/transliteration-api/app.log",
    "isFileOutputEnabled": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## Binding in Startup

```csharp
// TransliterationAPI/ServiceCollectionExtensions.cs
public static IServiceCollection AddConfigurations(
    this IServiceCollection services,
    IConfiguration configuration)
{
    CacheSettings cacheSettings = new();
    SecuritySettings securitySettings = new();

    configuration.Bind(nameof(CacheSettings), cacheSettings);
    configuration.Bind(nameof(SecuritySettings), securitySettings);

    services.AddSingleton(cacheSettings);
    services.AddSingleton(securitySettings);
    services.AddNuciLoggerSettings(configuration);

    return services;
}
```

**Key points:**
- Uses `configuration.Bind(nameof(CacheSettings), cacheSettings)` — binds to section named `cacheSettings` (class name)
- Registers settings as singletons for DI injection
- `AddNuciLoggerSettings` binds `nuciLoggerSettings` section and registers NuciLog

## Configuration Validation

**Startup validation (throws on failure):**
- `CacheSettings.StoreLocation` not null/empty (validated by `JsonRepository` on first use)
- `SecuritySettings.HmacSigningKey` not empty in production (validated by NuciAPI HMAC middleware per request)

**Runtime validation:**
- Cache directory writable (checked on first write in `Startup.CreateCacheStore`)
- HMAC key length verified per request by NuciAPI middleware

## Environment Variable Mapping

| Setting | Environment Variable |
|---------|---------------------|
| `CacheSettings.StoreLocation` | `TRANSLITERATION_API__CACHESETTINGS__STORELOCATION` |
| `CacheSettings.Enabled` | `TRANSLITERATION_API__CACHESETTINGS__ENABLED` |
| `SecuritySettings.HmacSigningKey` | `TRANSLITERATION_API__SECURITYSETTINGS__HMACSIGNINGKEY` |
| `NuciLoggerSettings.LogFilePath` | `TRANSLITERATION_API__NUCILOGGERSETTINGS__LOGFILEPATH` |
| `NuciLoggerSettings.IsFileOutputEnabled` | `TRANSLITERATION_API__NUCILOGGERSETTINGS__ISFILEOUTPUTENABLED` |
| `Logging:LogLevel:Default` | `TRANSLITERATION_API__LOGGING__LOGLEVEL__DEFAULT` |

**Note:** Double underscore `__` = colon `:` in configuration hierarchy. Section names use class name casing (`CacheSettings` → `CACHESETTINGS`).

## Generating HMAC Key

```bash
# 32 bytes = 256 bits = 44 chars base64
openssl rand -base64 32

# Example output: k7V3x9mN2pQ5rT8yU1wZ4aB6cD8eF0gH2jK4lM6nO8=
```

## Docker / Container Configuration

### Dockerfile (relevant section)

```dockerfile
# Cache volume
VOLUME /var/lib/transliteration-api/cache

# Non-root user
RUN adduser --disabled-password --gecos '' appuser
USER appuser

# Working directory
WORKDIR /app

# Config via environment
ENV TRANSLITERATION_API__CACHESETTINGS__STORELOCATION=/var/lib/transliteration-api/cache.json
ENV TRANSLITERATION_API__SECURITYSETTINGS__HMACSIGNINGKEY=""
ENV TRANSLITERATION_API__NUCILOGGERSETTINGS__LOGFILEPATH=/var/log/transliteration-api/app.log
```

### docker-compose.yml

```yaml
services:
  transliteration-api:
    build: .
    environment:
      - TRANSLITERATION_API__CACHESETTINGS__STORELOCATION=/var/lib/transliteration-api/cache.json
      - TRANSLITERATION_API__SECURITYSETTINGS__HMACSIGNINGKEY=${HMAC_KEY}
      - TRANSLITERATION_API__NUCILOGGERSETTINGS__LOGFILEPATH=/var/log/transliteration-api/app.log
      - ASPNETCORE_ENVIRONMENT=Production
    volumes:
      - transliteration-cache:/var/lib/transliteration-api/cache
      - transliteration-logs:/var/log/transliteration-api
    ports:
      - "8080:8080"

volumes:
  transliteration-cache:
  transliteration-logs:
```

### Kubernetes ConfigMap + Secret

```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: transliteration-api-config
data:
  CACHESETTINGS__STORELOCATION: "/var/lib/transliteration-api/cache.json"
  CACHESETTINGS__ENABLED: "true"
  NUCILOGGERSETTINGS__LOGFILEPATH: "/var/log/transliteration-api/app.log"
  NUCILOGGERSETTINGS__ISFILEOUTPUTENABLED: "true"
  LOGGING__LOGLEVEL__DEFAULT: "Information"
  LOGGING__LOGLEVEL__MICROSOFT_ASPNETCORE: "Warning"
---
apiVersion: v1
kind: Secret
metadata:
  name: transliteration-api-secrets
type: Opaque
stringData:
  SECURITYSETTINGS__HMACSIGNINGKEY: "k7V3x9mN2pQ5rT8yU1wZ4aB6cD8eF0gH2jK4lM6nO8="
```

## Configuration Patterns

### Feature Flags (Future)

```json
{
  "Features": {
    "EnableNewTransliterator": false,
    "EnableMetricsEndpoint": false
  }
}
```

### External Provider Overrides

```json
{
  "ExternalProviders": {
    "TransliterationDotCom": {
      "BaseUrl": "https://transliteration.com",
      "Timeout": "00:00:30"
    },
    "Ushuaia": {
      "BaseUrl": "https://ushuaia.pl",
      "CookieTtlMinutes": 5
    },
    "Podolak": {
      "BaseUrl": "https://podolak.pl",
      "Timeout": "00:00:45"
    }
  }
}
```
    services.Configure<SecuritySettings>(configuration.GetSection("Security"));
    services.Configure<HttpRequestSettings>(configuration.GetSection("HttpRequest"));

    // Validation
    services.AddOptions<CacheSettings>()
        .Validate(s => !string.IsNullOrWhiteSpace(s.Directory), "Cache directory required")
        .Validate(s => s.MaxTextLength > 0, "MaxTextLength must be positive")
        .ValidateOnStart();

    services.AddOptions<SecuritySettings>()
        .Validate(s => !string.IsNullOrWhiteSpace(s.HmacKey), "HMAC key required in production")
        .Validate(s => s.HmacKey.Length >= 44, "HMAC key must be 32 bytes (44 chars base64)")
        .ValidateOnStart();

    return services;
}
```

## Configuration Validation

**Startup validation (throws on failure):**
- `Cache.Directory` not empty
- `Cache.MaxTextLength` > 0
- `Security.HmacKey` not empty (production)
- `Security.HmacKey` valid base64, 32 bytes decoded

**Runtime validation:**
- Cache directory writable (checked on first write)
- HMAC key length verified per request

## Environment Variable Mapping

| Setting | Environment Variable |
|---------|---------------------|
| `Cache.Directory` | `TRANSLITERATION_API__CACHE__DIRECTORY` |
| `Cache.MaxTextLength` | `TRANSLITERATION_API__CACHE__MAXTEXTLENGTH` |
| `Security.HmacKey` | `TRANSLITERATION_API__SECURITY__HMACKEY` |
| `Security.AllowedHosts[0]` | `TRANSLITERATION_API__SECURITY__ALLOWEDHOSTS__0` |
| `HttpRequest.ConnectTimeout` | `TRANSLITERATION_API__HTTPREQUEST__CONNECTTIMEOUT` |
| `HttpRequest.ReadTimeout` | `TRANSLITERATION_API__HTTPREQUEST__READTIMEOUT` |
| `HttpRequest.MaxResponseSize` | `TRANSLITERATION_API__HTTPREQUEST__MAXRESPONSESIZE` |
| `HttpRequest.AllowAutoRedirect` | `TRANSLITERATION_API__HTTPREQUEST__ALLOW_AUTO_REDIRECT` |
| `Logging:LogLevel:Default` | `TRANSLITERATION_API__LOGGING__LOGLEVEL__DEFAULT` |

**Note:** Double underscore `__` = colon `:` in configuration hierarchy.

## Generating HMAC Key

```bash
# 32 bytes = 256 bits = 44 chars base64
openssl rand -base64 32

# Example output: k7V3x9mN2pQ5rT8yU1wZ4aB6cD8eF0gH2jK4lM6nO8=
```

## Docker / Container Configuration

### Dockerfile (relevant section)

```dockerfile
# Cache volume
VOLUME /var/lib/transliteration-api/cache

# Non-root user
RUN adduser --disabled-password --gecos '' appuser
USER appuser

# Working directory
WORKDIR /app

# Config via environment
ENV TRANSLITERATION_API__CACHE__DIRECTORY=/var/lib/transliteration-api/cache
ENV TRANSLITERATION_API__SECURITY__HMACKEY=""
```

### docker-compose.yml

```yaml
services:
  transliteration-api:
    build: .
    environment:
      - TRANSLITERATION_API__CACHE__DIRECTORY=/var/lib/transliteration-api/cache
      - TRANSLITERATION_API__SECURITY__HMACKEY=${HMAC_KEY}
      - TRANSLITERATION_API__SECURITY__ALLOWEDHOSTS__0=api.example.com
      - ASPNETCORE_ENVIRONMENT=Production
    volumes:
      - transliteration-cache:/var/lib/transliteration-api/cache
    ports:
      - "8080:8080"

volumes:
  transliteration-cache:
```

### Kubernetes ConfigMap + Secret

```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: transliteration-api-config
data:
  CACHE__DIRECTORY: "/var/lib/transliteration-api/cache"
  CACHE__MAXTEXTLENGTH: "50000"
  SECURITY__ALLOWEDHOSTS__0: "api.example.com"
  HTTPREQUEST__CONNECTTIMEOUT: "00:00:30"
  HTTPREQUEST__READTIMEOUT: "00:01:00"
---
apiVersion: v1
kind: Secret
metadata:
  name: transliteration-api-secrets
type: Opaque
stringData:
  SECURITY__HMACKEY: "k7V3x9mN2pQ5rT8yU1wZ4aB6cD8eF0gH2jK4lM6nO8="
```

## Configuration Patterns

### Feature Flags (Future)

```json
{
  "Features": {
    "EnableNewTransliterator": false,
    "EnableMetricsEndpoint": false
  }
}
```

### External Provider Overrides

```json
{
  "ExternalProviders": {
    "TransliterationDotCom": {
      "BaseUrl": "https://transliteration.com",
      "Timeout": "00:00:30"
    },
    "Ushuaia": {
      "BaseUrl": "https://ushuaia.pl",
      "CookieTtlMinutes": 5
    },
    "Podolak": {
      "BaseUrl": "https://podolak.pl",
      "Timeout": "00:00:45"
    }
  }
}
```

## Troubleshooting Configuration

### Common Issues

| Symptom | Cause | Fix |
|---------|-------|-----|
| "HMAC key required" on startup | `Security.HmacKey` empty in production | Set env var or config |
| "Invalid HMAC key length" | Key not 32 bytes | Regenerate with `openssl rand -base64 32` |
| Cache permission denied | Directory not writable | `chown`/`chmod` cache dir |
| 400 Bad Request on valid input | Text exceeds `MaxTextLength` | Increase limit or truncate input |
| External provider timeout | `HttpRequest.ReadTimeout` too low | Increase to 120s for slow providers |

### Debug Configuration Loading

```bash
# Print effective configuration
dotnet run --environment Development -- --print-config

# Or in code (temporary)
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", true)
    .AddEnvironmentVariables("TRANSLITERATION_API__")
    .Build();

Console.WriteLine(config.GetDebugView());
```

## Configuration Checklist by Environment

### Development
- [ ] `Cache.Directory` = `./cache-dev`
- [ ] `Security.HmacKey` = any non-empty string (e.g., `dev-key-dev-key-dev-key-dev-key==`)
- [ ] `Logging:LogLevel:Default` = `Debug`
- [ ] `AllowedHosts` = `*`

### Staging
- [ ] `Cache.Directory` = `/tmp/transliteration-cache` (or volume)
- [ ] `Security.HmacKey` = dedicated staging key
- [ ] `Security.AllowedHosts` = staging domain(s)
- [ ] `Logging:LogLevel:Default` = `Information`

### Production
- [ ] `Cache.Directory` = persistent volume (`/var/lib/...`)
- [ ] `Security.HmacKey` = strong random key (env var only)
- [ ] `Security.AllowedHosts` = production domain(s)
- [ ] `Cache.MaxTextLength` = appropriate for use case
- [ ] `Logging:LogLevel:Default` = `Information`
- [ ] `Logging:LogLevel:Microsoft.AspNetCore` = `Warning`