# Transliteration API — Transliterators Reference

## Overview

The API supports 40+ languages via 14 local (built-in) transliterators and 3 external provider adapters. Each language in the registry maps to exactly one transliterator type.

## Language Registry

Defined in `TransliterationAPI/Service/Entities/Language.cs` — static properties collected via reflection.

**Each entry specifies:**
- `Code` — Language identifier (used in API `language` parameter)
- `Name` — Display name
- `TransliteratorType` — Concrete implementation type
- `UsesExternalTransliterator` — Whether it uses `IExternalTransliterator` (async, HTTP)

## Local Transliterators (Synchronous)

All inherit `Transliterator` base class, implement `ITransliterator`.

### CyrillicTransliterator
**Languages:** `ab`, `be`, `bg`, `cv`, `kk`, `mk`, `ru`, `sr`, `sr-ec`, `sh`, `tg`, `tg-cyrl`, `tt`, `tt-cyrl`, `uk` (15 languages)

**Schemes:**
- ALA-LC (default for Abkhaz, Adyghe)
- BGN/PCGN (default for most)
- ISO-9 (selective)
- Language-specific overrides (Belarusian, Bulgarian, Chuvash, Kazakh, Macedonian, Serbian, Tatar, Tajik, Ukrainian)

**Algorithm:** Multi-table character replacement with digraph handling (e.g., `Ц` → `Ts`, `Щ` → `Shch`). Post-processing for specific languages.

### GreekTransliterator
**Languages:** `el`, `grc`, `grc-dor` (3 languages)

**Tables:** Modern Greek (ISO 843), Ancient Greek, Ancient Doric Greek

**Special handling:** Digraphs (`ευ` → `ev`, `αυ` → `au`, `ου` → `ou`) with accent preservation. Separate tables per variant.

### ArabicTransliterator
**Languages:** `ar`, `arz`, `ary` (3 languages)

**Tables:** Main Arabic + Maghrebi Arabic extensions

**Features:**
- Diacritic handling (fatha, damma, kasra, shadda)
- Maghrebi-specific characters (`گ`, `ڤ` → `g`)
- Post-processing fixes: article handling (`al-`), initial hamza, title casing

### HebrewTransliterator
**Language:** `he` (1 language)

**Tables:** Consonants + niqqud (vowel points)

**Post-processing:** Extensive regex fixes for proper nouns (e.g., `byb` → `Aviv`, `Ash` → `ʾAsh`, `Ch` → `Ḥ`)

### JapaneseTransliterator
**Language:** `ja` (1 language)

**Map:** Char→string for Hiragana, Katakana, selected Kanji (toponyms), and some CJK unified ideographs

**Coverage:** Basic syllabaries + common place name Kanji (Tokyo, Kyoto, Osaka, etc.)

### KoreanTransliterator
**Language:** `ko` (1 language)

**Map:** Syllable→string for common Hangul blocks

**Coverage:** Frequently used syllables; not exhaustive

### PinyinTransliterator
**Languages:** `zh`, `zh-hans` (2 languages)

**Backend:** `Microsoft.International.Converters.PinYinConverter` (CHSPinYinConv)

**Algorithm:**
1. Convert each Chinese char to numerical Pinyin (e.g., `kǎi` → `kai3`)
2. Convert tone numbers to diacritics (ā, á, ǎ, à)
3. Handle `ü` (u:)
4. Title-case output

### GujaratiTransliterator
**Language:** `gy` (1 language)

**Algorithm:** Multi-pass replacement:
1. Consonant + vowel sign + visarga/anusvara
2. Consonant + vowel sign + anusvara
3. Vowel + anusvara
4. Consonant + vowel sign
5. Halant consonants, vowel signs, vowels, consonants, numerals
6. Post-processing for consonant clusters

### MarathiTransliterator
**Language:** `mr` (1 language)

**Table:** Devanagari with Marathi-specific additions (`क़`, `ख़`, `ग़`, `ज़`, `ड़`, `ढ़`, `फ़`, `य़`, `क्ष`, `ज्ञ`, `श्र`)

**Output:** Title case

### CopticTransliterator
**Language:** `cop` (1 language)

**Map:** Direct Coptic character → Latin equivalent (including special characters: `Ϣ`→`Š`, `Ⲱ`→`Ō`, etc.)

### BerberTransliterator
**Language:** `ber` (1 language)

**Map:** Tifinagh script → Latin

**Output:** Title case

## External Transliterators (Asynchronous)

All inherit `ExternalTransliterator` base class, implement `IExternalTransliterator`.

### TranslitterationDotComTransliterator
**Provider:** `https://www.translitteration.com/`
**Languages:** `ady`, `hy`, `ba`, `ka`, `iu`, `ky`, `os`, `udm`, `hyw` (9 languages)

**Protocol:**
```
POST https://www.translitteration.com/ajax/en/transliterate
Content-Type: application/x-www-form-urlencoded

text=<source>&tlang=<mapped>&script=latn&scheme=<mapped>
```

**Mappings:**
| Language | tlang | scheme |
|----------|-------|--------|
| `ady` | `ady` | `iso-9` |
| `hy` | `xcl` | `iso-9985` |
| `ba` | `bak` | `iso-9` |
| `ka` | `kat` | `national` |
| `iu` | `iku` | `canadian-aboriginal-syllabics` |
| `ky` | `kir` | `iso-9` |
| `os` | `oss` | `iso-9` |
| `udm` | `udm` | `bgn-pcgn` |
| `hyw` | `hye` | `ala-lc` |

**Response:** `ack:::<result>` → strip prefix

**Post-processing:**
- Inuttitut: `ᐄ`→`i`, `ᐆ`→`u`
- Armenian/Georgian/Inuttitut/Kyrgyz: Title case

### UshuaiaTransliterator
**Provider:** `https://www.ushuaia.pl/`
**Languages:** `bn`, `hi`, `kn`, `ml`, `mn`, `sa`, `si`, `ta`, `te` (9 languages)

**Protocol (two-step):**
1. `GET https://www.ushuaia.pl/transliterate/` → extract `translit` cookie
2. `POST https://www.ushuaia.pl/transliterate/transliterate.php` with cookie

```
POST .../transliterate.php
Cookie: translit=<value>;lastlang=<mapped>
Content-Type: application/x-www-form-urlencoded

text=<source>&lang=<mapped>
```

**Language mappings:**
| Language | lang parameter |
|----------|----------------|
| `bn` | `bengali_iso_transliterate` |
| `hi` | `devanagari_hunt_transcribe` |
| `kn` | `kannada_iso_transliterate` |
| `ml` | `malayalam_iso_transliterate` |
| `mn` | `mongolian_mns_transliterate` |
| `sa` | `devanagari_iast_transliterate` |
| `si` | `sinhala_iso_transliterate` |
| `ta` | `tamil_iso_transliterate` |
| `te` | `telugu_iso_transliterate` |

**Session management:** Cookie cached for 5 minutes; reused across requests.

**Post-processing:** Bengali, Hindi, Kannada, Malayalam, Sanskrit, Sinhala, Tamil, Telugu → Title case

### PodolakTransliterator
**Provider:** `https://podolak.net/`
**Language:** `cu` (Old Church Slavonic) (1 language)

**Protocol:**
```
POST https://podolak.net/en/transliteration/old-church-slavonic
Content-Type: application/x-www-form-urlencoded

quelltext=cu&zieltext=isor9&startabfrage=1&text=<source>&transliteration=Transliteration&cu_isor9_jer=3
```

**Response parsing:** Extract first `<textarea id="ausgabe">...</textarea>` content via regex.

## Transliterator Factory

`TransliteratorFactory` resolves instances from DI container:

```csharp
public ITransliterator GetTransliterator(Language language)
    => (ITransliterator)serviceProvider.GetRequiredService(language.TransliteratorType);

public IExternalTransliterator GetExternalTransliterator(Language language)
    => (IExternalTransliterator)serviceProvider.GetRequiredService(language.TransliteratorType);
```

All transliterator types registered as singletons at startup via reflection over `Language.GetAll()`.

## Adding a New Transliterator

### Local
1. Create `NewTransliterator : Transliterator, ITransliterator`
2. Implement `PerformTransliteration(string text, Language language)`
3. Add to `Language` class:
   ```csharp
   public static Language NewCode => new("code", "Name", typeof(NewTransliterator));
   ```
4. No DI registration needed (automatic via reflection)

### External
1. Create `NewExternalTransliterator : ExternalTransliterator, IExternalTransliterator`
2. Implement `PerformTransliteration(string text, Language language)` using `IHttpRequestManager`
3. Add to `Language` class (same as above)
4. No DI registration needed

## Testing

**Unit tests:** Each transliterator has dedicated test class in `TransliterationAPI.UnitTests/Service/Transliterators/` with verified input/output pairs.

**Integration tests:** `RegisteredLanguageEndpointTests` verifies every registry language executes its configured transliterator via HTTP.

## Error Handling

| Scenario | Behavior |
|----------|----------|
| Unknown language code | Returns original text unchanged |
| External provider timeout (3s) | `HttpRequestException` → 502 via exception middleware |
| External provider HTTP error | Exception caught → returns original text |
| External provider malformed response | Parsing exception → returns original text |
| Local transliterator exception | Logged, rethrown → 500 via exception middleware |

## Performance

| Type | Latency | Throughput |
|------|---------|------------|
| Local | < 1ms | High (CPU-bound) |
| External | 100-2000ms | Limited by provider (network-bound) |
| Cached | < 1ms | High (disk I/O) |

## Provider Reliability

- **No SLA** — External providers are third-party, best-effort
- **No fallback** — If provider fails, original text returned
- **No retry** — Single attempt per request
- **Operator control** — Can restrict to local-only by modifying registry