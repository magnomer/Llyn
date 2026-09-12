using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LLanguageLoader
{
    private const string LLanguageLoaderFolder = "languages";
    private const string LLanguageLoaderFile = "source.json";
    private const string LLanguageLoaderPrimary = "English";
    private const string LLanguageLoaderLookup = "pronunciation";
    private const string LLanguageLoaderHarvest = "audio";
    private const string LLanguageLoaderFont = "font";
    private const string LLanguageLoaderExample = "example";
    private const string LLanguageLoaderGloss = "gloss";
    private const string LLanguageLoaderScheme = "transcription";
    private const string LLanguageLoaderSeparator = "separator";
    private const string LLanguageLoaderVarieties = "varieties";
    private const string LLanguageLoaderReadings = "readings";

    public static IReadOnlyList<string> LLanguageLoaderScan()
    {
        string root = Path.Combine(AppContext.BaseDirectory, LLanguageLoaderFolder);
        if (!Directory.Exists(root))
        {
            return Array.Empty<string>();
        }

        List<string> names = new();
        foreach (string directory in Directory.EnumerateDirectories(root))
        {
            if (File.Exists(Path.Combine(directory, LLanguageLoaderFile)))
            {
                names.Add(Path.GetFileName(directory));
            }
        }

        LLanguageSort(names);
        return names;
    }

    private static void LLanguageSort(List<string> names)
    {
        names.Sort(static (left, right) =>
        {
            bool leftPrimary = string.Equals(left, LLanguageLoaderPrimary, StringComparison.OrdinalIgnoreCase);
            bool rightPrimary = string.Equals(right, LLanguageLoaderPrimary, StringComparison.OrdinalIgnoreCase);
            if (leftPrimary != rightPrimary)
            {
                return leftPrimary ? -1 : 1;
            }

            return string.CompareOrdinal(left, right);
        });
    }

    public static LLanguage LLanguageLoaderLoad(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        string folder = Path.Combine(AppContext.BaseDirectory, LLanguageLoaderFolder, language);
        string path = Path.Combine(folder, LLanguageLoaderFile);
        if (!File.Exists(path))
        {
            return LLanguageBlankCreate(language);
        }

        try
        {
            using FileStream stream = File.OpenRead(path);
            using JsonDocument document = JsonDocument.Parse(stream);
            return LLanguageRead(language, document.RootElement);
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return LLanguageBlankCreate(language);
        }
    }

    private static LLanguage LLanguageBlankCreate(string language)
    {
        return new LLanguage(
            language,
            null,
            LLanguageFontBlank,
            LLanguageFontBlank,
            Array.Empty<LSourceSpec>(),
            Array.Empty<LSourceSpec>());
    }

    private static LLanguage LLanguageRead(string language, JsonElement root)
    {
        string? flag = root.ValueKind == JsonValueKind.Object ? LLanguageTextRead(root, "flag") : null;

        return new LLanguage(
            language,
            flag,
            LLanguageFontRead(root, LLanguageLoaderFont),
            LLanguageFontRead(root, LLanguageLoaderExample),
            LLanguageSourceScan(root, LLanguageLoaderLookup),
            LLanguageSourceScan(root, LLanguageLoaderHarvest),
            LLanguageSchemeScan(root),
            LLanguageSeparatorRead(root),
            LLanguageVarietyScan(root),
            LLanguageFlaggedCheck(root),
            LLanguageFontRead(root, LLanguageLoaderGloss));
    }

    private static bool LLanguageSeparatorRead(JsonElement root)
    {
        string? separator = root.ValueKind == JsonValueKind.Object
            ? LLanguageTextRead(root, LLanguageLoaderSeparator)
            : null;
        return !string.Equals(separator?.Trim(), "none", StringComparison.OrdinalIgnoreCase);
    }

    private static IReadOnlyList<string> LLanguageSchemeScan(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderScheme, out JsonElement schemes) ||
            schemes.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<string>();
        }

        List<string> names = new();
        foreach (JsonElement scheme in schemes.EnumerateArray())
        {
            string name = scheme.ValueKind == JsonValueKind.String ? scheme.GetString()!.Trim() : string.Empty;
            if (name.Length > 0 && !names.Contains(name, StringComparer.Ordinal))
            {
                names.Add(name);
            }
        }

        return names;
    }

    private static IReadOnlyList<LSourceSpec> LLanguageSourceScan(JsonElement root, string key)
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
            LSourceSpec? spec = LLanguageSourceRead(source);
            if (spec is not null)
            {
                specs.Add(spec);
            }
        }

        return specs;
    }

    private static LFont LLanguageFontBlank => new(null, 0);

    private static LFont LLanguageFontRead(JsonElement root, string key)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(key, out JsonElement font) ||
            font.ValueKind != JsonValueKind.Object)
        {
            return LLanguageFontBlank;
        }

        string? family = LLanguageTextRead(font, "family");
        double size = LLanguageMeasureRead(font, "size");
        string? style = LLanguageTextRead(font, "style");

        return new LFont(
            string.IsNullOrWhiteSpace(family) ? null : family,
            size,
            string.IsNullOrWhiteSpace(style) ? null : style.Trim());
    }

    private static LSourceSpec? LLanguageSourceRead(JsonElement source)
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

        return attempts.Count == 0 ? null : new LSourceSpec(name, attempts);
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
            LLanguageTextRead(row, "prefix"));
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
            LLanguageNumberRead(element, "skip"));
    }

    private static IReadOnlyList<LVariety> LLanguageVarietyScan(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderVarieties, out JsonElement block) ||
            block.ValueKind != JsonValueKind.Object ||
            !block.TryGetProperty("list", out JsonElement rows) ||
            rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LVariety>();
        }

        List<LVariety> varieties = new();
        foreach (JsonElement row in rows.EnumerateArray())
        {
            LVariety? variety = LLanguageVarietyRead(row);
            if (variety is not null && varieties.All(known => !string.Equals(known.LVarietyName, variety.LVarietyName, StringComparison.Ordinal)))
            {
                varieties.Add(variety);
            }
        }

        return varieties;
    }

    private static LVariety? LLanguageVarietyRead(JsonElement row)
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

        string? flag = LLanguageTextRead(row, "flag")?.Trim();
        return new LVariety(name, string.IsNullOrEmpty(flag) ? null : flag);
    }

    private static bool LLanguageFlaggedCheck(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderVarieties, out JsonElement block) ||
            block.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        string? shown = LLanguageTextRead(block, "shown");
        return string.Equals(shown?.Trim(), "flag", StringComparison.OrdinalIgnoreCase);
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

    private static string? LLanguageTextRead(JsonElement element, string key)
    {
        return element.TryGetProperty(key, out JsonElement value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }

    private static int LLanguageNumberRead(JsonElement element, string key)
    {
        return element.TryGetProperty(key, out JsonElement value)
            && value.ValueKind == JsonValueKind.Number
            && value.TryGetInt32(out int number)
            ? number
            : 0;
    }

    private static double LLanguageMeasureRead(JsonElement element, string key)
    {
        return element.TryGetProperty(key, out JsonElement value)
            && value.ValueKind == JsonValueKind.Number
            && value.TryGetDouble(out double measure)
            && measure > 0
            ? measure
            : 0;
    }

    private static bool LLanguageBooleanRead(JsonElement element, string key)
    {
        return element.TryGetProperty(key, out JsonElement value) && value.ValueKind == JsonValueKind.True;
    }
}
