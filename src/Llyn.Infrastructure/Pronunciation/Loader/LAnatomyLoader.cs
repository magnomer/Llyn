using System;
using System.Collections.Generic;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LAnatomyLoader
{
    private const string LAnatomyKey = "anatomy";

    public static IReadOnlyList<LAnatomyRule> LAnatomyPackRead(string language, JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LAnatomyKey, out JsonElement section))
        {
            return Array.Empty<LAnatomyRule>();
        }

        if (section.ValueKind == JsonValueKind.String)
        {
            if (LPackFile.LPackFileLoad(language, section.GetString()!) is not JsonElement file)
            {
                return Array.Empty<LAnatomyRule>();
            }

            if (file.ValueKind == JsonValueKind.Object
                && file.TryGetProperty(LAnatomyKey, out JsonElement rows))
            {
                file = rows;
            }

            return LAnatomyRowScan(file);
        }

        return LAnatomyRowScan(section);
    }

    private static IReadOnlyList<LAnatomyRule> LAnatomyRowScan(JsonElement rows)
    {
        if (rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LAnatomyRule>();
        }

        List<LAnatomyRule> rules = [];
        foreach (JsonElement row in rows.EnumerateArray())
        {
            LAnatomyRule? rule = LAnatomyBlockRead(row);
            if (rule is not null)
            {
                rules.Add(rule);
            }
        }

        return rules;
    }

    private static LAnatomyRule? LAnatomyBlockRead(JsonElement row)
    {
        if (row.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        IReadOnlyList<string> languages = LPack.LPackTextScan(row, "language");
        if (languages.Count == 0)
        {
            return null;
        }

        LAnatomyPattern? ipa = LAnatomyPatternRead(row, "ipa");
        LAnatomyPattern? respelling = LAnatomyPatternRead(row, "respelling") ?? ipa;
        if (ipa is null || respelling is null)
        {
            return null;
        }

        return new LAnatomyRule(languages, ipa, respelling);
    }

    private static LAnatomyPattern? LAnatomyPatternRead(JsonElement row, string key)
    {
        if (!row.TryGetProperty(key, out JsonElement block) || block.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string match = LPack.LPackTextRead(block, "match")?.Trim() ?? string.Empty;
        if (match.Length == 0)
        {
            return null;
        }

        IReadOnlyList<LRespellingRule> rewrites =
            block.TryGetProperty("rewrite", out JsonElement rows) && rows.ValueKind == JsonValueKind.Array
                ? LRespellingLoader.LRespellingRuleScan(rows)
                : Array.Empty<LRespellingRule>();

        try
        {
            return new LAnatomyPattern(match, rewrites, LPack.LPackBooleanRead(block, "decompose"));
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
