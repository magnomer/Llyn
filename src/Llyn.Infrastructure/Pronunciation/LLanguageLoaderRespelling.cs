using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static partial class LLanguageLoader
{
    private static IReadOnlyList<LRespelling> LLanguageRespellingScan(JsonElement root, string key, bool scoped)
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
            LRespelling? respelling = LLanguageRespellingRead(group);
            if (respelling is not null && (!scoped || respelling.LRespellingVarieties.Count > 0))
            {
                respellings.Add(respelling);
            }
        }

        return respellings;
    }

    private static LRespelling? LLanguageRespellingRead(JsonElement group)
    {
        if (group.ValueKind != JsonValueKind.Object ||
            !group.TryGetProperty("rules", out JsonElement rows) ||
            rows.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        string name = LLanguageTextRead(group, "name")?.Trim() ?? string.Empty;
        List<string> varieties = new();
        if (group.TryGetProperty(LLanguageLoaderVarieties, out JsonElement names) &&
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

        IReadOnlyList<LRespellingRule> rules = LLanguageRuleScan(rows);
        return rules.Count == 0 ? null : new LRespelling(name, varieties, rules);
    }

    private static IReadOnlyList<LRespellingRule> LLanguageSpellingScan(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderSpelling, out JsonElement rows) ||
            rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LRespellingRule>();
        }

        return LLanguageRuleScan(rows);
    }

    private static IReadOnlyList<LRespellingRule> LLanguageRuleScan(JsonElement rows)
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

    private static bool LLanguageSeparatorRead(JsonElement root)
    {
        string? separator = root.ValueKind == JsonValueKind.Object
            ? LLanguageTextRead(root, LLanguageLoaderSeparator)
            : null;
        return !string.Equals(separator?.Trim(), "none", StringComparison.OrdinalIgnoreCase);
    }

    private static IReadOnlyList<LScheme> LLanguageSchemeScan(
        JsonElement root, IReadOnlyList<LRespellingRule> spelling)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderScheme, out JsonElement schemes) ||
            schemes.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LScheme>();
        }

        List<LScheme> declared = new();
        foreach (JsonElement scheme in schemes.EnumerateArray())
        {
            LScheme? read = LLanguageSchemeRead(scheme, spelling);
            if (read is not null && declared.All(known =>
                    !string.Equals(known.LSchemeName, read.LSchemeName, StringComparison.Ordinal)))
            {
                declared.Add(read);
            }
        }

        return declared;
    }

    private static LScheme? LLanguageSchemeRead(JsonElement scheme, IReadOnlyList<LRespellingRule> spelling)
    {
        if (scheme.ValueKind == JsonValueKind.String)
        {
            string bare = scheme.GetString()!.Trim();
            return bare.Length == 0 ? null : new LScheme(bare, Array.Empty<LSourceSpec>());
        }

        if (scheme.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string name = LLanguageTextRead(scheme, "name")?.Trim() ?? string.Empty;
        return name.Length == 0
            ? null
            : new LScheme(name, LLanguageSourceScan(scheme, LLanguageLoaderSources, spelling));
    }
}
