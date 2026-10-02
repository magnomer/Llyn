using System;
using System.Collections.Generic;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LRespellingLoader
{
    public const string LRespellingCleanup = "cleanup";
    public const string LRespellingKey = "respelling";
    private const string LRespellingSpelling = "spelling";
    private const string LRespellingRewrite = "rewrite";

    public static IReadOnlyList<LRespelling> LRespellingPackScan(JsonElement root, string key, bool scoped)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(key, out JsonElement groups) ||
            groups.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LRespelling>();
        }

        List<LRespelling> respellings = new();
        foreach (JsonElement group in groups.EnumerateArray())
        {
            LRespelling? respelling = LRespellingRowRead(group);
            if (respelling is not null && (!scoped || respelling.LRespellingVarieties.Count > 0))
            {
                respellings.Add(respelling);
            }
        }

        return respellings;
    }

    private static LRespelling? LRespellingRowRead(JsonElement group)
    {
        if (group.ValueKind != JsonValueKind.Object ||
            !group.TryGetProperty("rules", out JsonElement rows) ||
            rows.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        List<string> varieties = new();
        if (group.TryGetProperty("varieties", out JsonElement names) &&
            names.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement variety in names.EnumerateArray())
            {
                string tag = variety.ValueKind == JsonValueKind.String ? variety.GetString()!.Trim() : string.Empty;
                if (tag.Length > 0)
                {
                    varieties.Add(tag);
                }
            }
        }

        IReadOnlyList<LRespellingRule> rules = LRespellingRuleScan(rows);
        return rules.Count == 0 ? null : new LRespelling(varieties, rules);
    }

    public static IReadOnlyList<LRespellingRule> LRespellingSpellingScan(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LRespellingSpelling, out JsonElement rows) ||
            rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LRespellingRule>();
        }

        return LRespellingRuleScan(rows);
    }

    public static IReadOnlyList<LRespellingRule> LRespellingRuleScan(JsonElement rows)
    {
        try
        {
            List<LRespellingRule> rules = new();
            foreach (JsonElement row in rows.EnumerateArray())
            {
                if (row.ValueKind == JsonValueKind.Array && row.GetArrayLength() == 2 &&
                    row[0].ValueKind == JsonValueKind.String && row[1].ValueKind == JsonValueKind.String)
                {
                    rules.Add(new LRespellingRule(row[0].GetString()!, row[1].GetString()!));
                }
            }

            return rules;
        }
        catch (ArgumentException)
        {
            return Array.Empty<LRespellingRule>();
        }
    }

    public static IReadOnlyList<LRespellingRule> LRespellingRewriteScan(
        JsonElement row, string key = LRespellingRewrite)
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
