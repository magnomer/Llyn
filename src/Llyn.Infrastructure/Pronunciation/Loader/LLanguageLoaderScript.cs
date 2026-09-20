using System;
using System.Collections.Generic;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed partial class LLanguageLoader : LLanguageVault
{
    private const string LLanguageLoaderScript = "script";
    private const string LLanguageLoaderForm = "form";
    private const string LLanguageLoaderRewrite = "rewrite";

    private static IReadOnlyList<LScriptStyle> LLanguageScriptScan(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderScript, out JsonElement rows) ||
            rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LScriptStyle>();
        }

        List<LScriptStyle> styles = new();
        foreach (JsonElement row in rows.EnumerateArray())
        {
            LScriptStyle? style = LLanguageScriptRead(row);
            if (style is not null)
            {
                styles.Add(style);
            }
        }

        return styles;
    }

    private static LScriptStyle? LLanguageScriptRead(JsonElement row)
    {
        if (row.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string name = LLanguageTextRead(row, "name")?.Trim() ?? string.Empty;
        string url = LLanguageTextRead(row, "url")?.Trim() ?? string.Empty;
        string pattern = LLanguageTextRead(row, "match") ?? string.Empty;
        if (name.Length == 0 || url.Length == 0 || pattern.Length == 0)
        {
            return null;
        }

        return new LScriptStyle(
            name,
            url,
            LLanguageFormRead(row),
            pattern,
            LLanguageNumberRead(row, "image"),
            LLanguageNumberRead(row, "caption"),
            LLanguageTextRead(row, "prefix"),
            LLanguageRewriteScan(row),
            LLanguageTextRead(row, "gloss"));
    }

    private static IReadOnlyDictionary<string, string> LLanguageFormRead(JsonElement row)
    {
        Dictionary<string, string> form = new(StringComparer.Ordinal);
        if (!row.TryGetProperty(LLanguageLoaderForm, out JsonElement fields)
            || fields.ValueKind != JsonValueKind.Object)
        {
            return form;
        }

        foreach (JsonProperty field in fields.EnumerateObject())
        {
            if (field.Value.ValueKind == JsonValueKind.String)
            {
                form[field.Name] = field.Value.GetString()!;
            }
        }

        return form;
    }

    private static IReadOnlyList<LRespellingRule> LLanguageRewriteScan(
        JsonElement row, string key = LLanguageLoaderRewrite)
    {
        if (!row.TryGetProperty(key, out JsonElement rows) || rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LRespellingRule>();
        }

        List<LRespellingRule> rules = new();
        foreach (JsonElement rule in rows.EnumerateArray())
        {
            if (rule.ValueKind != JsonValueKind.Array || rule.GetArrayLength() != 2 ||
                rule[0].ValueKind != JsonValueKind.String || rule[1].ValueKind != JsonValueKind.String)
            {
                continue;
            }

            try
            {
                rules.Add(new LRespellingRule(rule[0].GetString()!, rule[1].GetString()!));
            }
            catch (ArgumentException)
            {
            }
        }

        return rules;
    }
}
