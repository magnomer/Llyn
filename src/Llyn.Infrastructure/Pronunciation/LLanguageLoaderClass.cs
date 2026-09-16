using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static partial class LLanguageLoader
{
    private const string LLanguageLoaderTone = "tone";

    private const string LLanguageLoaderClass = "class";

    private static IReadOnlyList<LAnatomyTone> LLanguageClassRead(string language, JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderTone, out JsonElement section))
        {
            return Array.Empty<LAnatomyTone>();
        }

        if (section.ValueKind == JsonValueKind.String)
        {
            return LLanguageClassLoad(language, section.GetString()!);
        }

        return LLanguageClassScan(section);
    }

    private static IReadOnlyList<LAnatomyTone> LLanguageClassLoad(string language, string file)
    {
        string name = file.Trim();
        if (name.Length == 0 || name != Path.GetFileName(name))
        {
            return Array.Empty<LAnatomyTone>();
        }

        string path = Path.Combine(AppContext.BaseDirectory, LLanguageLoaderFolder, language, name);
        if (!File.Exists(path))
        {
            return Array.Empty<LAnatomyTone>();
        }

        try
        {
            using FileStream stream = File.OpenRead(path);
            using JsonDocument document = JsonDocument.Parse(stream);
            JsonElement root = document.RootElement;
            if (root.ValueKind == JsonValueKind.Object
                && root.TryGetProperty(LLanguageLoaderTone, out JsonElement rows))
            {
                root = rows;
            }

            return LLanguageClassScan(root);
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return Array.Empty<LAnatomyTone>();
        }
    }

    private static IReadOnlyList<LAnatomyTone> LLanguageClassScan(JsonElement rows)
    {
        if (rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LAnatomyTone>();
        }

        List<LAnatomyTone> rules = [];
        foreach (JsonElement row in rows.EnumerateArray())
        {
            LAnatomyTone? rule = LLanguageEstimateRead(row);
            if (rule is not null)
            {
                rules.Add(rule);
            }
        }

        return rules;
    }

    private static LAnatomyTone? LLanguageEstimateRead(JsonElement row)
    {
        if (row.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        IReadOnlyList<string> languages = LLanguageNameScan(row);
        if (languages.Count == 0
            || !row.TryGetProperty(LLanguageLoaderClass, out JsonElement table)
            || table.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        IReadOnlyDictionary<string, IReadOnlyList<string>> classes = LLanguageContourScan(table);
        return classes.Count == 0 ? null : new LAnatomyTone(languages, classes);
    }

    private static IReadOnlyDictionary<string, IReadOnlyList<string>> LLanguageContourScan(JsonElement table)
    {
        Dictionary<string, IReadOnlyList<string>> classes = new(StringComparer.Ordinal);
        foreach (JsonProperty row in table.EnumerateObject())
        {
            string contour = row.Name.Trim();
            if (contour.Length == 0)
            {
                continue;
            }

            IEnumerable<JsonElement> elements = row.Value.ValueKind switch
            {
                JsonValueKind.String => [row.Value],
                JsonValueKind.Array => row.Value.EnumerateArray(),
                _ => [],
            };
            List<string> names = [];
            foreach (JsonElement element in elements)
            {
                string label = element.ValueKind == JsonValueKind.String ? element.GetString()!.Trim() : string.Empty;
                if (label.Length > 0 && !names.Contains(label))
                {
                    names.Add(label);
                }
            }

            if (names.Count > 0)
            {
                classes[contour] = names;
            }
        }

        return classes;
    }
}
