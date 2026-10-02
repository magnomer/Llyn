using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LSourceLoader
{
    public const string LSourceSources = "sources";
    private const string LSourceReadings = "readings";
    private const string LSourceFollow = "follow";
    private const string LSourceBands = "bands";
    private const string LSourceOnce = "once";
    private const string LSourceDecode = "decode";
    private const string LSourceUnit = "unit";

    public static IReadOnlyList<LSourceSpec> LSourceSpecScan(
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
            LSourceSpec? spec = LSourceSpecRead(source, spelling);
            if (spec is not null)
            {
                specs.Add(spec);
            }
        }

        return specs;
    }

    private static LSourceSpec? LSourceSpecRead(JsonElement source, IReadOnlyList<LRespellingRule> spelling)
    {
        if (source.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string name = LPack.LPackTextRead(source, "name") ?? string.Empty;
        if (name.Length == 0)
        {
            return null;
        }

        List<LSourceAttempt> attempts = new();
        if (source.TryGetProperty("attempts", out JsonElement rows) && rows.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement row in rows.EnumerateArray())
            {
                LSourceAttempt? attempt = LSourceAttemptRead(row);
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

        JsonElement once = source.TryGetProperty(LSourceOnce, out JsonElement rule) ? rule : default;
        return new LSourceSpec(
            name,
            attempts,
            spelling,
            LSourceBandScan(source),
            LSourceFigureRead(once, "total"),
            LSourceFigureRead(once, "factor"),
            LSourceFigureRead(once, "base"),
            LPack.LPackTextRead(source, LSourceUnit)?.Trim());
    }

    private static double? LSourceFigureRead(JsonElement rule, string key)
    {
        if (rule.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        double figure = LPack.LPackMeasureRead(rule, key);
        return figure > 0 ? figure : null;
    }

    private static IReadOnlyList<LBand> LSourceBandScan(JsonElement source)
    {
        if (!source.TryGetProperty(LSourceBands, out JsonElement rows) ||
            rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LBand>();
        }

        List<LBand> bands = new();
        foreach (JsonElement row in rows.EnumerateArray())
        {
            LBand? band = LSourceBandRead(row);
            if (band is not null)
            {
                bands.Add(band);
            }
        }

        return bands;
    }

    private static LBand? LSourceBandRead(JsonElement row)
    {
        if (row.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string name = LPack.LPackTextRead(row, "name")?.Trim() ?? string.Empty;
        if (name.Length == 0)
        {
            return null;
        }

        string? pattern = LPack.LPackTextRead(row, "match");
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

    private static LSourceAttempt? LSourceAttemptRead(JsonElement row)
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

        IReadOnlyList<LSourceReading> readings = LSourceReadingScan(row);
        if (urls.Count == 0 || readings.Count == 0)
        {
            return null;
        }

        return new LSourceAttempt(
            urls,
            readings,
            LPack.LPackTextRead(row, "confirm"),
            LEnvelope.LEnvelopeHeaderRead(row),
            LPack.LPackTextRead(row, "prefix"),
            LSourceFollowRead(row),
            LPack.LPackBooleanRead(row, LSourceDecode));
    }

    private static LSourceReading? LSourceFollowRead(JsonElement row)
    {
        if (!row.TryGetProperty(LSourceFollow, out JsonElement follow))
        {
            return null;
        }

        LSourceReading? reading = LSourceReadingRead(follow);
        return reading is null ? null : reading with { LSourceReadingPhonetic = false };
    }

    private static IReadOnlyList<LSourceReading> LSourceReadingScan(JsonElement row)
    {
        if (!row.TryGetProperty(LSourceReadings, out JsonElement rows) || rows.ValueKind != JsonValueKind.Array)
        {
            LSourceReading? flat = LSourceReadingRead(row);
            return flat is null ? Array.Empty<LSourceReading>() : [flat];
        }

        List<LSourceReading> readings = new();
        foreach (JsonElement element in rows.EnumerateArray())
        {
            LSourceReading? reading = LSourceReadingRead(element);
            if (reading is not null)
            {
                readings.Add(reading);
            }
        }

        return readings;
    }

    private static LSourceReading? LSourceReadingRead(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string strategy = LPack.LPackTextRead(element, "strategy") ?? string.Empty;
        if (strategy.Length == 0)
        {
            return null;
        }

        return new LSourceReading(
            LPack.LPackTextRead(element, "variety")?.Trim() ?? string.Empty,
            strategy,
            LPack.LPackTextRead(element, "match"),
            LPack.LPackNumberRead(element, "group"),
            LPack.LPackTextRead(element, "path"),
            LPack.LPackBooleanRead(element, "normalize"),
            LPack.LPackNumberRead(element, "skip"),
            LPack.LPackBooleanRead(element, "every"));
    }
}
