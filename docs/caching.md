# Transliteration API — Caching Architecture

## Overview

The caching layer provides file-backed persistence for transliteration results to avoid recomputation and external provider calls. It uses NuciDAL's `JsonRepository` for JSON array storage.

## Cache Key Generation

**Algorithm:** SHA-256 hash of composite string

```csharp
string textUnicodes = string.Join('-', text.Select(c => (int)c));
string cacheKey = $"{languageCode}_{textUnicodes}_{cacheSettings.ApplicationVersion}";
return SHA256(cacheKey);
```

**Components:**
| Component | Source | Purpose |
|-----------|--------|---------|
| `languageCode` | `Language.Code` | Isolates per-language |
| `textUnicodes` | Unicode code points of input text | Exact text match (case-sensitive, whitespace-sensitive after trim) |
| `ApplicationVersion` | `Assembly.GetEntryAssembly().GetName().Version` | Automatic invalidation on deploy |

**Example:** Russian "привет" → `ru_1087-1088-1080-1074-1077-1090_1.2.3.4` → SHA-256 → `a1b2c3...`

## Cache Entity

```csharp
public class CachedTransliteration : EntityBase
{
    public string TransliteratedText { get; set; }
    // Id inherited from EntityBase (string) — stores the SHA-256 cache key
}
```

## JsonRepository (NuciDAL 3.2.1)

**File format:** JSON array of `CachedTransliteration` objects

```json
[
  {
    "id": "a1b2c3...",
    "transliteratedText": "privet"
  },
  {
    "id": "d4e5f6...",
    "transliteratedText": "hello"
  }
]
```

**Operations:**
| Method | Behavior |
|--------|----------|
| `TryGet(id)` | Returns entity or null; no exception on missing |
| `Add(entity)` | Tracks in memory; does not persist immediately |
| `SaveChanges()` | Serializes entire tracked collection to file (overwrites) |

**Thread safety:** Not thread-safe; single-process assumption. Concurrent access requires external synchronization (not implemented).

## Cache Lifecycle

### Initialization (Startup.cs)
```csharp
if (!File.Exists(cacheSettings.StoreLocation))
{
    File.WriteAllText(cacheSettings.StoreLocation, "[]");
}
```
Creates empty JSON array if file missing.

### Read Path (TransliterationService.Transliterate)
```csharp
if (cacheSettings.Enabled)
{
    var cacheId = GenerateCacheId(languageCode, text);
    var cached = await _repository.TryGet(cacheId);
    if (cached != null) return cached.TransliteratedText;
}
```

### Write Path (TransliterationService.Transliterate)
```csharp
var result = GetTransliteratedText(text, language);
if (result != null && cacheSettings.Enabled)
{
    var cacheId = GenerateCacheId(languageCode, text);
    _repository.Add(new CachedTransliteration { Id = cacheId, TransliteratedText = result });
    _repository.SaveChanges();
}
```

## Configuration

**CacheSettings** (bound from `cacheSettings` section):

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `StoreLocation` | string | `"cache.json"` | Relative or absolute path to cache file |
| `Enabled` | bool | `true` | Master toggle |
| `ApplicationVersion` | string | Computed | Assembly version (read-only) |

**appsettings.json:**
```json
{
  "cacheSettings": {
    "storeLocation": "cache.json",
    "enabled": "true"
  }
}
```

## Behavior Characteristics

| Aspect | Behavior |
|--------|----------|
| **Scope** | Per-process, per-file |
| **Eviction** | None (manual file deletion or disable) |
| **Expiration** | None (version change invalidates) |
| **Concurrency** | Single-writer assumed; no locking |
| **Failure mode** | Write failures logged, request continues |
| **Size limits** | None (bounded by disk) |

## Cache Invalidation

**Automatic:** Application version change (new deployment) → all keys change → cache miss → recompute.

**Manual:**
- Delete `cache.json` file
- Set `cacheSettings.enabled = false` in config
- Restart application

## Performance Considerations

- **Read:** Single file read + JSON deserialize on first access (repository loads on first `TryGet`)
- **Write:** Full file rewrite on every `SaveChanges()` (not incremental)
- **Memory:** Entire cache held in memory after first load
- **Suitable for:** Low-to-moderate volume (< 100k entries)
- **Not suitable for:** High-volume, multi-instance, or distributed deployments

## Testing

**Unit tests:** None directly (tested via integration)

**Integration tests** (`TransliterationServiceIntegrationTests`):
- Cache hit returns cached value
- Cache miss computes and stores
- Disabled cache bypasses read/write
- Version change invalidates
- Whitespace normalisation before cache key

## Extension: Replace Cache Backend

Replace `JsonRepository` registration in `ServiceCollectionExtensions.AddCustomServices()`:

```csharp
services.AddSingleton<IFileRepository<CachedTransliteration>, CustomCacheRepository>();
```

**Required interface:**
```csharp
public interface IFileRepository<T> where T : EntityBase
{
    Task<T> TryGet(string id);
    void Add(T entity);
    void SaveChanges();
}
```

**EntityBase contract:**
```csharp
public abstract class EntityBase
{
    public string Id { get; set; }
}
```