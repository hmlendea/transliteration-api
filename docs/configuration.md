# Transliteration API — Configuration Reference

## Configuration Sources (Priority Order)

1. **appsettings.json** (base)
2. **appsettings.{Environment}.json** (environment-specific)
3. **Environment variables** (`TRANSLITERATION_API__` prefix)
4. **Command line arguments** (highest)

## Settings Classes

### CacheSettings (`Configuration/CacheSettings.cs`)

```csharp
public sealed class CacheSettings
{
    public string Directory { get; set; } = "./cache";
    public int MaxTextLength { get; set; } = 10000;
}
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Directory` | string | `./cache` | Cache file storage path (relative to working dir or absolute) |
| `MaxTextLength` | int | `10000` | Maximum input text length; longer returns 400 |

**Environment variables:**
```bash
TRANSLITERATION_API__CACHE__DIRECTORY=/var/lib/transliteration-api/cache
TRANSLITERATION_API__CACHE__MAXTEXTLENGTH=50000
```

### SecuritySettings (`Configuration/SecuritySettings.cs`)

```csharp
public sealed class SecuritySettings
{
    public string HmacKey { get; set; } = "";
    public string[] AllowedHosts { get; set; } = [];
}
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `HmacKey` | string | `""` | Base64-encoded 32-byte key for HMAC-SHA256 response signing |
| `AllowedHosts` | string[] | `[]` | Host header validation (empty = allow all) |

**Environment variables:**
```bash
TRANSLITERATION_API__SECURITY__HMACKEY="$(openssl rand -base64 32)"
TRANSLITERATION_API__SECURITY__ALLOWEDHOSTS__0=api.example.com
TRANSLITERATION_API__SECURITY__ALLOWEDHOSTS__1=api-staging.example.com
```

### HttpRequestSettings (from NuciAPI)

```csharp
public sealed class HttpRequestSettings
{
    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan ReadTimeout { get; set; } = TimeSpan.FromSeconds(60);
    public long MaxResponseSize { get; set; } = 1_048_576; // 1MB
    public bool AllowAutoRedirect { get; set; } = false;
}
```

**Environment variables:**
```bash
TRANSLITERATION_API__HTTPREQUEST__CONNECTTIMEOUT=00:00:30
TRANSLITERATION_API__HTTPREQUEST__READTIMEOUT=00:01:00
TRANSLITERATION_API__HTTPREQUEST__MAXRESPONSESIZE=2097152
TRANSLITERATION_API__HTTPREQUEST__ALLOW_AUTO_REDIRECT=false
```

## appsettings.json (Complete Example)

```json
{
  "Cache": {
    "Directory": "./cache",
    "MaxTextLength": 10000
  },
  "Security": {
    "HmacKey": "",
    "AllowedHosts": []
  },
  "HttpRequest": {
    "ConnectTimeout": "00:00:30",
    "ReadTimeout": "00:01:00",
    "MaxResponseSize": 1048576,
    "AllowAutoRedirect": false
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

## Environment-Specific Overrides

### appsettings.Development.json

```json
{
  "Cache": {
    "Directory": "./cache-dev"
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
  "Cache": {
    "Directory": "/var/lib/transliteration-api/cache",
    "MaxTextLength": 50000
  },
  "Security": {
    "AllowedHosts": ["api.example.com", "api.internal"]
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
// ServiceCollectionExtensions.cs
public static IServiceCollection AddTransliterationApi(this IServiceCollection services, IConfiguration configuration)
{
    services.Configure<CacheSettings>(configuration.GetSection("Cache"));
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