using System;
using System.Collections.Generic;
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
            LLanguageSourceScan(root, LLanguageLoaderHarvest));
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

        return new LFont(string.IsNullOrWhiteSpace(family) ? null : family, size);
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

        string strategy = LLanguageTextRead(row, "strategy") ?? string.Empty;
        if (urls.Count == 0 || strategy.Length == 0)
        {
            return null;
        }

        return new LSourceAttempt(
            urls,
            strategy,
            LLanguageTextRead(row, "match"),
            LLanguageNumberRead(row, "group"),
            LLanguageTextRead(row, "path"),
            LLanguageTextRead(row, "confirm"),
            LLanguageBooleanRead(row, "normalize"),
            LLanguageHeaderRead(row),
            LLanguageTextRead(row, "prefix"));
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
