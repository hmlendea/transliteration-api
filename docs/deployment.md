# Transliteration API — Deployment Guide

## Requirements

| Component | Version | Notes |
|-----------|---------|-------|
| .NET SDK | 10.0.x | For build; runtime self-contained |
| .NET Runtime | 10.0.x | If not self-contained |
| OS | Linux (tested), Windows, macOS | x64, arm64 |
| Disk | 100 MB + cache | Cache grows with usage (single JSON file) |
| Memory | 100 MB base + cache | Depends on concurrent requests; entire cache loaded in memory |
| Network | Outbound HTTPS | For external providers (transliteration.com, ushuaia.pl, podolak.pl) |

## Build

### Development Build

```bash
# Restore + build (Debug)
dotnet build TransliterationAPI.slnx

# Run directly
dotnet run --project TransliterationAPI/TransliterationAPI.csproj
```

### Production Build (Self-Contained)

```bash
# Linux x64
dotnet publish TransliterationAPI/TransliterationAPI.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -o ./publish/linux-x64

# Linux arm64
dotnet publish TransliterationAPI/TransliterationAPI.csproj \
  -c Release \
  -r linux-arm64 \
  --self-contained true \
  -o ./publish/linux-arm64

# Windows x64
dotnet publish TransliterationAPI/TransliterationAPI.csproj \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -o ./publish/win-x64
```

**Output:** Single executable + dependencies in `publish/{rid}/`

### Trimmed Build (Smaller)

```bash
dotnet publish TransliterationAPI/TransliterationAPI.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -p:PublishTrimmed=true \
  -p:TrimMode=partial \
  -o ./publish/linux-x64-trimmed
```

**Note:** Trimming may remove reflection-used types (Language registry uses `Assembly.GetTypes()`). Test thoroughly.

## Run

### Direct Execution

```bash
# Development
cd TransliterationAPI
dotnet run --urls "http://localhost:5000"

# Production (self-contained)
./publish/linux-x64/TransliterationAPI --urls "http://0.0.0.0:8080"
```

### Environment Variables

```bash
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://0.0.0.0:8080
export TRANSLITERATION_API__CACHESETTINGS__STORELOCATION=/var/lib/transliteration-api/cache.json
export TRANSLITERATION_API__SECURITYSETTINGS__HMACSIGNINGKEY="$(openssl rand -base64 32)"
export TRANSLITERATION_API__NUCILOGGERSETTINGS__LOGFILEPATH=/var/log/transliteration-api/app.log
```

**Note:** Section names match class names exactly (`CacheSettings` → `CACHESETTINGS`, `SecuritySettings` → `SECURITYSETTINGS`, `NuciLoggerSettings` → `NUCILOGGERSETTINGS`).

### systemd Service (Linux)

```ini
# /etc/systemd/system/transliteration-api.service
[Unit]
Description=Transliteration API
After=network.target

[Service]
Type=notify
User=transliteration-api
Group=transliteration-api
WorkingDirectory=/opt/transliteration-api
ExecStart=/opt/transliteration-api/TransliterationAPI
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:8080
Environment=TRANSLITERATION_API__CACHESETTINGS__STORELOCATION=/var/lib/transliteration-api/cache.json
Environment=TRANSLITERATION_API__SECURITYSETTINGS__HMACSIGNINGKEY=your-base64-key-here
Environment=TRANSLITERATION_API__NUCILOGGERSETTINGS__LOGFILEPATH=/var/log/transliteration-api/app.log
Restart=on-failure
RestartSec=5
StandardOutput=journal
StandardError=journal
SyslogIdentifier=transliteration-api

# Security hardening
NoNewPrivileges=true
PrivateTmp=true
ProtectSystem=strict
ProtectHome=true
ReadWritePaths=/var/lib/transliteration-api /var/log/transliteration-api
CapabilityBoundingSet=
AmbientCapabilities=

[Install]
WantedBy=multi-user.target
```

**Enable:**
```bash
sudo systemctl daemon-reload
sudo systemctl enable --now transliteration-api
sudo systemctl status transliteration-api
```

### Docker

#### Dockerfile

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY TransliterationAPI.slnx .
COPY TransliterationAPI/TransliterationAPI.csproj TransliterationAPI/
RUN dotnet restore TransliterationAPI/TransliterationAPI.csproj
COPY . .
RUN dotnet publish TransliterationAPI/TransliterationAPI.csproj -c Release -r linux-x64 --self-contained true -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-noble-chiseled AS final
WORKDIR /app
COPY --from=build /app/publish .
USER app
VOLUME /var/lib/transliteration-api
VOLUME /var/log/transliteration-api
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV TRANSLITERATION_API__CACHESETTINGS__STORELOCATION=/var/lib/transliteration-api/cache.json
ENV TRANSLITERATION_API__NUCILOGGERSETTINGS__LOGFILEPATH=/var/log/transliteration-api/app.log
EXPOSE 8080
ENTRYPOINT ["./TransliterationAPI"]
```

#### Build & Run

```bash
# Build
docker build -t transliteration-api:latest .

# Run
docker run -d \
  --name transliteration-api \
  -p 8080:8080 \
  -e TRANSLITERATION_API__SECURITYSETTINGS__HMACSIGNINGKEY="$(openssl rand -base64 32)" \
  -e TRANSLITERATION_API__SECURITYSETTINGS__ALLOWEDHOSTS__0=api.example.com \
  -v transliteration-cache:/var/lib/transliteration-api \
  -v transliteration-logs:/var/log/transliteration-api \
  transliteration-api:latest
```

### Kubernetes

#### Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: transliteration-api
  labels:
    app: transliteration-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: transliteration-api
  template:
    metadata:
      labels:
        app: transliteration-api
    spec:
      containers:
      - name: transliteration-api
        image: transliteration-api:latest
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ASPNETCORE_URLS
          value: "http://0.0.0.0:8080"
        - name: TRANSLITERATION_API__CACHESETTINGS__STORELOCATION
          value: "/var/lib/transliteration-api/cache.json"
        - name: TRANSLITERATION_API__SECURITYSETTINGS__HMACSIGNINGKEY
          valueFrom:
            secretKeyRef:
              name: transliteration-api-secrets
              key: HMAC_KEY
        - name: TRANSLITERATION_API__NUCILOGGERSETTINGS__LOGFILEPATH
          value: "/var/log/transliteration-api/app.log"
        volumeMounts:
        - name: cache
          mountPath: /var/lib/transliteration-api
        - name: logs
          mountPath: /var/log/transliteration-api
        resources:
          requests:
            memory: "128Mi"
            cpu: "100m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 10
          periodSeconds: 30
        readinessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 5
          periodSeconds: 10
      volumes:
      - name: cache
        persistentVolumeClaim:
          claimName: transliteration-api-cache
      - name: logs
        persistentVolumeClaim:
          claimName: transliteration-api-logs
---
apiVersion: v1
kind: Service
metadata:
  name: transliteration-api
spec:
  selector:
    app: transliteration-api
  ports:
  - port: 80
    targetPort: 8080
  type: ClusterIP
---
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: transliteration-api-cache
spec:
  accessModes:
  - ReadWriteOnce
  resources:
    requests:
      storage: 1Gi
---
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: transliteration-api-logs
spec:
  accessModes:
  - ReadWriteOnce
  resources:
    requests:
      storage: 500Mi
```

## Reverse Proxy (Required for Production)

### Nginx

```nginx
# /etc/nginx/sites-available/transliteration-api
upstream transliteration_api {
    server 127.0.0.1:8080;
    keepalive 32;
}

server {
    listen 443 ssl http2;
    server_name api.example.com;

    ssl_certificate /etc/letsencrypt/live/api.example.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/api.example.com/privkey.pem;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;

    # Security headers
    add_header X-Frame-Options DENY;
    add_header X-Content-Type-Options nosniff;
    add_header Referrer-Policy strict-origin-when-cross-origin;

    # Rate limiting
    limit_req_zone $binary_remote_addr zone=api:10m rate=100r/m;
    limit_req zone=api burst=20 nodelay;

    # Request size
    client_max_body_size 1M;

    location / {
        proxy_pass http://transliteration_api;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
        proxy_read_timeout 60s;
        proxy_send_timeout 60s;
    }

    # Health check (no rate limit)
    location /health {
        proxy_pass http://transliteration_api;
        access_log off;
    }
}

# HTTP redirect
server {
    listen 80;
    server_name api.example.com;
    return 301 https://$server_name$request_uri;
}
```

### Caddy (Simpler)

```caddyfile
# /etc/caddy/Caddyfile
api.example.com {
    reverse_proxy localhost:8080 {
        header_up X-Real-IP {remote_host}
        header_up X-Forwarded-For {remote_host}
        header_up X-Forwarded-Proto {scheme}
    }
    rate_limit {
        zone api 100r/m
        key {remote_host}
    }
    request_body {
        max_size 1MB
    }
}
```

## Health Checks

**Endpoint:** `GET /health`

**Response:** `200 OK` with `{"status":"Healthy"}`

**Kubernetes probe:** Use `/health` for both liveness and readiness.

**Note:** No external dependencies checked (cache is local, providers are optional).

## Cache Management

### Directory Structure

```
/var/lib/transliteration-api/cache/
├── a1b2c3d4e5f6...json  # SHA-256 key
├── f7e8d9c0b1a2...json
└── ...
```

### Cache Entry Format

```json
{
  "languageCode": "abk",
  "originalText": "Аҟәа",
  "transliteratedText": "Ak̄a̋a",
  "timestamp": "2025-01-15T10:30:00Z",
  "appVersion": "1.2.3"
}
```

### Operations

```bash
# Size
du -sh /var/lib/transliteration-api/cache

# Count
ls /var/lib/transliteration-api/cache | wc -l

# Clear (service must be stopped)
rm -rf /var/lib/transliteration-api/cache/*

# Or via API (if admin endpoint added in future)
# DELETE /admin/cache
```

### Monitoring

```bash
# Cache hit rate (from logs)
grep "Cache hit" /var/log/transliteration-api.log | wc -l
grep "Cache miss" /var/log/transliteration-api.log | wc -l

# Disk usage alert
df -h /var/lib/transliteration-api/cache
```

## Release Process

### Versioning

**Scheme:** Semantic Versioning (MAJOR.MINOR.PATCH)

**Location:** `TransliterationAPI/TransliterationAPI.csproj`

```xml
<PropertyGroup>
  <Version>1.2.3</Version>
  <AssemblyVersion>1.2.3.0</AssemblyVersion>
  <FileVersion>1.2.3.0</FileVersion>
  <InformationalVersion>1.2.3</InformationalVersion>
</PropertyGroup>
```

### Release Script

```bash
# release.sh (in repo root)
#!/bin/bash
set -e

VERSION=$1
if [ -z "$VERSION" ]; then
  echo "Usage: ./release.sh <version>"
  exit 1
fi

# Update version
sed -i "s/<Version>.*<\/Version>/<Version>$VERSION<\/Version>/" TransliterationAPI/TransliterationAPI.csproj

# Build
dotnet build TransliterationAPI.slnx -c Release

# Test
dotnet test TransliterationAPI.slnx -c Release --no-build

# Publish
dotnet publish TransliterationAPI/TransliterationAPI.csproj -c Release -r linux-x64 --self-contained true -o ./publish

# Tag
git add TransliterationAPI/TransliterationAPI.csproj
git commit -m "chore: release v$VERSION"
git tag -a "v$VERSION" -m "Release v$VERSION"

echo "Release v$VERSION ready. Push with: git push origin master --tags"
```

### GitHub Actions Release

```yaml
# .github/workflows/github-release.yml
name: GitHub Release
on:
  push:
    tags: ['v*']
jobs:
  release:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      - name: Build
        run: dotnet build TransliterationAPI.slnx -c Release
      - name: Test
        run: dotnet test TransliterationAPI.slnx -c Release --no-build
      - name: Publish
        run: dotnet publish TransliterationAPI/TransliterationAPI.csproj -c Release -r linux-x64 --self-contained true -o ./publish
      - name: Create Release
        uses: softprops/action-gh-release@v1
        with:
          files: ./publish/TransliterationAPI
          generate_release_notes: true
```

## Monitoring & Observability

### Logging

**Structured JSON logs** (via NuciLog):

```json
{
  "timestamp": "2025-01-15T10:30:00.123Z",
  "level": "Information",
  "correlationId": "abc-123",
  "message": "Transliteration completed",
  "languageCode": "abk",
  "textLength": 4,
  "cacheHit": true,
  "durationMs": 2
}
```

**Log aggregation:** Ship to Loki, Elasticsearch, or cloud provider.

### Metrics (Future)

**Planned endpoints:**
- `GET /metrics` (Prometheus format)
- Cache hit/miss counters
- Request latency histogram
- External provider latency
- Error rates by type

### Distributed Tracing

**Correlation ID:** Passed via `X-Correlation-ID` header, generated if absent.

**Propagation:** Included in all external HTTP calls.

## Backup & Recovery

### Cache Backup

```bash
# Daily cron
0 2 * * * tar -czf /backup/transliteration-cache-$(date +\%F).tar.gz /var/lib/transliteration-api/cache
```

### Config Backup

```bash
# Version control appsettings.json (without secrets)
# Secrets in vault/secret manager
```

### Disaster Recovery

1. Provision new server/container
2. Deploy same version
3. Restore cache from backup (optional - cold start works)
4. Update DNS/load balancer
5. Verify `/health` and test transliteration

## Scaling

### Horizontal

- Stateless (cache is local filesystem)
- **Option A:** Shared cache volume (NFS, Ceph, EFS) - simple, potential contention
- **Option B:** Redis cache (requires code change) - better performance
- **Option C:** No shared cache - each instance independent, more external calls

### Vertical

- Increase CPU/memory for concurrent requests
- Cache is memory-mapped by OS (benefits from RAM)

### External Provider Limits

| Provider | Rate Limit | Notes |
|----------|------------|-------|
| transliteration.com | Unknown | Public, no auth |
| Ushuaia | Unknown | Session-based |
| Podolak | Unknown | Public, no auth |

**Mitigation:** Cache aggressively; monitor external call failures.

## Troubleshooting

### Service Won't Start

```bash
# Check logs
journalctl -u transliteration-api -f

# Common causes:
# - Port in use: ss -tlnp | grep 8080
# - Config validation: check HMAC key, cache dir permissions
# - Missing runtime: dotnet --list-runtimes
```

### High Latency

```bash
# Check external provider calls
grep "External call" /var/log/transliteration-api.log

# Cache hit rate
grep -c "Cache hit" /var/log/transliteration-api.log
grep -c "Cache miss" /var/log/transliteration-api.log
```

### 502 Bad Gateway

```bash
# Upstream (API) not running
systemctl status transliteration-api

# Upstream timeout
# Increase proxy_read_timeout in nginx
```

### HMAC Verification Fails

```bash
# Key mismatch between client and server
# Check TRANSLITERATION_API__SECURITY__HMACKEY matches
# Restart after key change
```

## File Checklist for New Deployment

- [ ] .NET 10 runtime installed (or self-contained)
- [ ] Cache directory created, correct permissions
- [ ] HMAC key generated and configured
- [ ] appsettings.json / env vars configured
- [ ] Reverse proxy configured (HTTPS, rate limit, size limit)
- [ ] systemd service / Docker / K8s manifests deployed
- [ ] Health check endpoint accessible
- [ ] Logs shipping to aggregation
- [ ] Backup schedule for cache
- [ ] Monitoring alerts (disk, latency, errors)
- [ ] Load test completed