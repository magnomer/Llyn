using System;
using System.Collections.Generic;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LDescentLoader
{
    private const string LDescentKey = "tone";

    private const string LDescentClass = "class";

    public static IReadOnlyList<LDescent> LDescentPackRead(string language, JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LDescentKey, out JsonElement section))
        {
            return Array.Empty<LDescent>();
        }

        if (section.ValueKind == JsonValueKind.String)
        {
            if (LPackFile.LPackFileLoad(language, section.GetString()!) is not JsonElement file)
            {
                return Array.Empty<LDescent>();
            }

            if (file.ValueKind == JsonValueKind.Object
                && file.TryGetProperty(LDescentKey, out JsonElement rows))
            {
                file = rows;
            }

            return LDescentRowScan(file);
        }

        return LDescentRowScan(section);
    }

    private static IReadOnlyList<LDescent> LDescentRowScan(JsonElement rows)
    {
        if (rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LDescent>();
        }

        List<LDescent> rules = [];
        foreach (JsonElement row in rows.EnumerateArray())
        {
            LDescent? rule = LDescentRowRead(row);
            if (rule is not null)
            {
                rules.Add(rule);
            }
        }

        return rules;
    }

    private static LDescent? LDescentRowRead(JsonElement row)
    {
        if (row.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        IReadOnlyList<string> languages = LPack.LPackTextScan(row, "language");
        if (languages.Count == 0
            || !row.TryGetProperty(LDescentClass, out JsonElement table)
            || table.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        IReadOnlyDictionary<string, IReadOnlyList<string>> classes = LDescentContourScan(table);
        return classes.Count == 0 ? null : new LDescent(languages, classes);
    }

    private static IReadOnlyDictionary<string, IReadOnlyList<string>> LDescentContourScan(JsonElement table)
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
