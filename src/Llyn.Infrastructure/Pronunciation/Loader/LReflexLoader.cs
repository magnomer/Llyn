using System;
using System.Collections.Generic;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LReflexLoader
{
    private const string LReflexKey = "reflex";
    private const string LReflexRecast = "recast";
    private const string LReflexOrderKey = "order";

    public static IReadOnlyList<LReflexRule> LReflexPackScan(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LReflexKey, out JsonElement rows) ||
            rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LReflexRule>();
        }

        List<LReflexRule> rules = new();
        foreach (JsonElement row in rows.EnumerateArray())
        {
            LReflexRule? rule = LReflexClauseRead(row);
            if (rule is not null)
            {
                rules.Add(rule);
            }
        }

        return rules;
    }

    public static LReflexOrder LReflexOrderRead(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LReflexOrderKey, out JsonElement order) ||
            order.ValueKind != JsonValueKind.Object)
        {
            return LReflexOrder.LReflexOrderEmpty;
        }

        Dictionary<string, IReadOnlyList<string>> kinds = new(StringComparer.Ordinal);
        if (order.TryGetProperty("kind", out JsonElement declared) && declared.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty language in declared.EnumerateObject())
            {
                kinds[language.Name.Trim()] = LPack.LPackTextScan(declared, language.Name);
            }
        }

        return new LReflexOrder(LPack.LPackTextScan(order, "language"), kinds);
    }

    private static LReflexRule? LReflexClauseRead(JsonElement row)
    {
        if (row.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string language = LPack.LPackTextRead(row, "language")?.Trim() ?? string.Empty;
        string url = LPack.LPackTextRead(row, "url")?.Trim() ?? string.Empty;
        string pattern = LPack.LPackTextRead(row, "match") ?? string.Empty;
        if (language.Length == 0 || url.Length == 0 || pattern.Length == 0)
        {
            return null;
        }

        return new LReflexRule(
            language,
            url,
            LEnvelope.LEnvelopePayloadRead(row),
            pattern,
            LPack.LPackTextRead(row, "format"),
            LPack.LPackBooleanRead(row, "every"),
            LPack.LPackTextRead(row, "busy"),
            LPack.LPackNumberRead(row, "interval"),
            LEnvelope.LEnvelopeHeaderRead(row),
            LPack.LPackTextRead(row, "epithet"),
            LPack.LPackTextRead(row, "clip"),
            LPack.LPackBooleanRead(row, "first"),
            LRespellingLoader.LRespellingRewriteScan(row),
            LPack.LPackTextRead(row, "region")?.Trim(),
            LPack.LPackTextRead(row, "split"),
            LPack.LPackTextRead(row, "gloss"),
            LPack.LPackTextRead(row, "until"),
            LPack.LPackBooleanRead(row, "folded"),
            LRespellingLoader.LRespellingRewriteScan(row, LReflexRecast),
            LPack.LPackBooleanRead(row, "superscript"));
    }
}
