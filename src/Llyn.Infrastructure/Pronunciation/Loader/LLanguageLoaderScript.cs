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
    private const string LLanguageLoaderEpoch = "epoch";

    private static IReadOnlyList<LScriptStyle> LLanguageScriptScan(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderScript, out JsonElement rows) ||
            rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LScriptStyle>();
        }

        IReadOnlyList<LEpoch> shared = LLanguageEpochScan(root);
        List<LScriptStyle> styles = new();
        foreach (JsonElement row in rows.EnumerateArray())
        {
            LScriptStyle? style = LLanguageScriptRead(row, shared);
            if (style is not null)
            {
                styles.Add(style);
            }
        }

        return styles;
    }

    private static LScriptStyle? LLanguageScriptRead(JsonElement row, IReadOnlyList<LEpoch> shared)
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
            LLanguageTextRead(row, "gloss"),
            LLanguageEpochScan(row) is { Count: > 0 } epochs ? epochs : shared);
    }

    private static IReadOnlyList<LEpoch> LLanguageEpochScan(JsonElement row)
    {
        if (row.ValueKind != JsonValueKind.Object
            || !row.TryGetProperty(LLanguageLoaderEpoch, out JsonElement rows)
            || rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LEpoch>();
        }

        List<LEpoch> epochs = new();
        foreach (JsonElement pair in rows.EnumerateArray())
        {
            if (pair.ValueKind != JsonValueKind.Array || pair.GetArrayLength() != 2 ||
                pair[0].ValueKind != JsonValueKind.String || pair[1].ValueKind != JsonValueKind.String)
            {
                continue;
            }

            string label = pair[0].GetString()!.Trim();
            string code = pair[1].GetString()!.Trim();
            if (label.Length > 0 && code.Length > 0)
            {
                epochs.Add(new LEpoch(label, code));
            }
        }

        return epochs;
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
