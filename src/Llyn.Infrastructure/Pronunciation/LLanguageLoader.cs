using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

/// <summary>
/// Loads a language pack from <c>languages/&lt;Lang&gt;/source.json</c>, resolved against the
/// application's base directory so packs are drop-in: adding or editing a language needs no
/// recompile. A missing or malformed pack yields a language with no sources rather than throwing,
/// so one bad pack never breaks the app. This is the only place source.json is read; the loaded
/// <see cref="LLanguage"/> carries every language-specific fact onward.
/// </summary>
public static class LLanguageLoader
{
    private const string LLanguageLoaderFolder = "languages";
    private const string LLanguageLoaderFile = "source.json";

    /// <summary>
    /// Scans the <c>languages/</c> folder for available packs, returning the name of every language
    /// that has a <c>source.json</c>. Language-agnostic: the set of languages is whatever is on disk,
    /// discovered at runtime, so adding a pack folder makes it selectable with no code change.
    /// </summary>
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

        names.Sort(StringComparer.Ordinal);
        return names;
    }

    public static LLanguage LLanguageLoaderLoad(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        string folder = Path.Combine(AppContext.BaseDirectory, LLanguageLoaderFolder, language);
        string path = Path.Combine(folder, LLanguageLoaderFile);
        if (!File.Exists(path))
        {
            return new LLanguage(language, null, Array.Empty<LSourceSpec>());
        }

        try
        {
            using FileStream stream = File.OpenRead(path);
            using JsonDocument document = JsonDocument.Parse(stream);
            return LLanguageRead(language, document.RootElement);
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return new LLanguage(language, null, Array.Empty<LSourceSpec>());
        }
    }

    private static LLanguage LLanguageRead(string language, JsonElement root)
    {
        // The flag is declared in the pack as an ISO 3166-1 alpha-2 country code; the image itself is
        // not shipped. The engine downloads and caches it on demand. Absent code means no flag.
        string? flag = root.ValueKind == JsonValueKind.Object ? LLanguageTextRead(root, "flag") : null;

        List<LSourceSpec> specs = new();
        if (root.ValueKind == JsonValueKind.Object &&
            root.TryGetProperty("sources", out JsonElement sources) &&
            sources.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement source in sources.EnumerateArray())
            {
                LSourceSpec? spec = LLanguageSourceRead(source);
                if (spec is not null)
                {
                    specs.Add(spec);
                }
            }
        }

        return new LLanguage(language, flag, specs);
    }

    private static LSourceSpec? LLanguageSourceRead(JsonElement source)
    {
        if (source.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string name = LLanguageTextRead(source, "name") ?? string.Empty;
        string kind = LLanguageTextRead(source, "kind") ?? string.Empty;
        if (name.Length == 0 || kind.Length == 0)
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

        return attempts.Count == 0 ? null : new LSourceSpec(name, kind, attempts);
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

    private static bool LLanguageBooleanRead(JsonElement element, string key)
    {
        return element.TryGetProperty(key, out JsonElement value) && value.ValueKind == JsonValueKind.True;
    }
}
