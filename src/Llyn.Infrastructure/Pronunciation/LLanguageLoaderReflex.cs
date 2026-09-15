using System;
using System.Collections.Generic;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static partial class LLanguageLoader
{
    private const string LLanguageLoaderReflex = "reflex";

    private static IReadOnlyList<LReflexRule> LLanguageReflexScan(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderReflex, out JsonElement rows) ||
            rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LReflexRule>();
        }

        List<LReflexRule> rules = new();
        foreach (JsonElement row in rows.EnumerateArray())
        {
            LReflexRule? rule = LLanguageReflexRead(row);
            if (rule is not null)
            {
                rules.Add(rule);
            }
        }

        return rules;
    }

    private static LReflexRule? LLanguageReflexRead(JsonElement row)
    {
        if (row.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string language = LLanguageTextRead(row, "language")?.Trim() ?? string.Empty;
        string url = LLanguageTextRead(row, "url")?.Trim() ?? string.Empty;
        string pattern = LLanguageTextRead(row, "match") ?? string.Empty;
        if (language.Length == 0 || url.Length == 0 || pattern.Length == 0)
        {
            return null;
        }

        return new LReflexRule(
            language,
            url,
            LLanguageFormRead(row),
            pattern,
            LLanguageTextRead(row, "format"),
            LLanguageBooleanRead(row, "every"),
            LLanguageTextRead(row, "busy"),
            LLanguageNumberRead(row, "interval"),
            LLanguageHeaderRead(row),
            LLanguageTextRead(row, "epithet"),
            LLanguageTextRead(row, "clip"),
            LLanguageBooleanRead(row, "first"),
            LLanguageRewriteScan(row));
    }
}
