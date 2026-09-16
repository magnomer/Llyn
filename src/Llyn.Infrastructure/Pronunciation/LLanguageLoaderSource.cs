using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static partial class LLanguageLoader
{
    private const string LLanguageLoaderBands = "bands";
    private const string LLanguageLoaderOnce = "once";
    private const string LLanguageLoaderDecode = "decode";
    private const string LLanguageLoaderUnit = "unit";

    private static IReadOnlyList<LSourceSpec> LLanguageSourceScan(
        JsonElement root, string key, IReadOnlyList<LRespellingRule> spelling)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(key, out JsonElement sources) ||
            sources.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LSourceSpec>();
        }

        List<LSourceSpec> specs = new();
        foreach (JsonElement source in sources.EnumerateArray())
        {
            LSourceSpec? spec = LLanguageSourceRead(source, spelling);
            if (spec is not null)
            {
                specs.Add(spec);
            }
        }

        return specs;
    }

    private static LSourceSpec? LLanguageSourceRead(JsonElement source, IReadOnlyList<LRespellingRule> spelling)
    {
        if (source.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string name = LLanguageTextRead(source, "name") ?? string.Empty;
        if (name.Length == 0)
        {
            return null;
        }

        List<LSourceAttempt> attempts = new();
        if (source.TryGetProperty("attempts", out JsonElement rows) && rows.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement row in rows.EnumerateArray())
            {
                LSourceAttempt? attempt = LLanguageAttemptRead(row);
                if (attempt is not null)
                {
                    attempts.Add(attempt);
                }
            }
        }

        if (attempts.Count == 0)
        {
            return null;
        }

        JsonElement once = source.TryGetProperty(LLanguageLoaderOnce, out JsonElement rule) ? rule : default;
        return new LSourceSpec(
            name,
            attempts,
            spelling,
            LLanguageBandScan(source),
            LLanguageFigureRead(once, "total"),
            LLanguageFigureRead(once, "factor"),
            LLanguageFigureRead(once, "base"),
            LLanguageTextRead(source, LLanguageLoaderUnit)?.Trim());
    }

    private static double? LLanguageFigureRead(JsonElement rule, string key)
    {
        if (rule.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        double figure = LLanguageMeasureRead(rule, key);
        return figure > 0 ? figure : null;
    }

    private static IReadOnlyList<LBand> LLanguageBandScan(JsonElement source)
    {
        if (!source.TryGetProperty(LLanguageLoaderBands, out JsonElement rows) ||
            rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LBand>();
        }

        List<LBand> bands = new();
        foreach (JsonElement row in rows.EnumerateArray())
        {
            LBand? band = LLanguageBandRead(row);
            if (band is not null)
            {
                bands.Add(band);
            }
        }

        return bands;
    }

    private static LBand? LLanguageBandRead(JsonElement row)
    {
        if (row.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string name = LLanguageTextRead(row, "name")?.Trim() ?? string.Empty;
        if (name.Length == 0)
        {
            return null;
        }

        string? pattern = LLanguageTextRead(row, "match");
        if (string.IsNullOrEmpty(pattern))
        {
            return null;
        }

        try
        {
            _ = new Regex(pattern);
        }
        catch (ArgumentException)
        {
            return null;
        }

        return new LBand(name, pattern);
    }

    private static LSourceAttempt? LLanguageAttemptRead(JsonElement row)
    {
        if (row.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        List<string> urls = new();
        if (row.TryGetProperty("urls", out JsonElement addresses) && addresses.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement address in addresses.EnumerateArray())
            {
                if (address.ValueKind == JsonValueKind.String)
                {
                    urls.Add(address.GetString()!);
                }
            }
        }

        IReadOnlyList<LSourceReading> readings = LLanguageReadingScan(row);
        if (urls.Count == 0 || readings.Count == 0)
        {
            return null;
        }

        return new LSourceAttempt(
            urls,
            readings,
            LLanguageTextRead(row, "confirm"),
            LLanguageHeaderRead(row),
            LLanguageTextRead(row, "prefix"),
            LLanguageFollowRead(row),
            LLanguageBooleanRead(row, LLanguageLoaderDecode));
    }

    private static LSourceReading? LLanguageFollowRead(JsonElement row)
    {
        if (!row.TryGetProperty(LLanguageLoaderFollow, out JsonElement follow))
        {
            return null;
        }

        LSourceReading? reading = LLanguageReadingRead(follow);
        return reading is null ? null : reading with { LSourceReadingPhonetic = false };
    }

    private static IReadOnlyList<LSourceReading> LLanguageReadingScan(JsonElement row)
    {
        if (!row.TryGetProperty(LLanguageLoaderReadings, out JsonElement rows) || rows.ValueKind != JsonValueKind.Array)
        {
            LSourceReading? flat = LLanguageReadingRead(row);
            return flat is null ? Array.Empty<LSourceReading>() : [flat];
        }

        List<LSourceReading> readings = new();
        foreach (JsonElement element in rows.EnumerateArray())
        {
            LSourceReading? reading = LLanguageReadingRead(element);
            if (reading is not null)
            {
                readings.Add(reading);
            }
        }

        return readings;
    }

    private static LSourceReading? LLanguageReadingRead(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string strategy = LLanguageTextRead(element, "strategy") ?? string.Empty;
        if (strategy.Length == 0)
        {
            return null;
        }

        return new LSourceReading(
            LLanguageTextRead(element, "variety")?.Trim() ?? string.Empty,
            strategy,
            LLanguageTextRead(element, "match"),
            LLanguageNumberRead(element, "group"),
            LLanguageTextRead(element, "path"),
            LLanguageBooleanRead(element, "normalize"),
            LLanguageNumberRead(element, "skip"),
            LLanguageBooleanRead(element, "every"));
    }

    private static IReadOnlyDictionary<string, string>? LLanguageHeaderRead(JsonElement row)
    {
        if (!row.TryGetProperty("headers", out JsonElement headers) || headers.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        Dictionary<string, string> map = new(StringComparer.Ordinal);
        foreach (JsonProperty header in headers.EnumerateObject())
        {
            if (header.Value.ValueKind == JsonValueKind.String)
            {
                map[header.Name] = header.Value.GetString()!;
            }
        }

        return map.Count == 0 ? null : map;
    }

    private static LGlyph? LLanguageGlyphRead(JsonElement root, IReadOnlyList<LRespellingRule> spelling)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderGlyph, out JsonElement glyph) ||
            glyph.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string name = LLanguageTextRead(glyph, "name")?.Trim() ?? string.Empty;
        string language = LLanguageTextRead(glyph, "language")?.Trim() ?? string.Empty;
        if (name.Length == 0 || language.Length == 0)
        {
            return null;
        }

        LFont font = LLanguageFontRead(glyph, LLanguageLoaderFont);
        return new LGlyph(
            name,
            language,
            LLanguageSourceScan(glyph, LLanguageLoaderSources, spelling),
            font.LFontFamily is null && font.LFontSize <= 0 ? null : font);
    }
}
