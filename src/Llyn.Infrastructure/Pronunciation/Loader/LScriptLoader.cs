using System;
using System.Collections.Generic;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LScriptLoader
{
    private const string LScriptKey = "script";
    private const string LScriptEpochKey = "epoch";

    public static IReadOnlyList<LScriptStyle> LScriptPackScan(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LScriptKey, out JsonElement rows) ||
            rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LScriptStyle>();
        }

        IReadOnlyList<LEpoch> shared = LScriptEpochScan(root);
        List<LScriptStyle> styles = new();
        foreach (JsonElement row in rows.EnumerateArray())
        {
            LScriptStyle? style = LScriptRowRead(row, shared);
            if (style is not null)
            {
                styles.Add(style);
            }
        }

        return styles;
    }

    private static LScriptStyle? LScriptRowRead(JsonElement row, IReadOnlyList<LEpoch> shared)
    {
        if (row.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        string name = LPack.LPackTextRead(row, "name")?.Trim() ?? string.Empty;
        string url = LPack.LPackTextRead(row, "url")?.Trim() ?? string.Empty;
        string pattern = LPack.LPackTextRead(row, "match") ?? string.Empty;
        if (name.Length == 0 || url.Length == 0 || pattern.Length == 0)
        {
            return null;
        }

        return new LScriptStyle(
            name,
            url,
            LEnvelope.LEnvelopePayloadRead(row),
            pattern,
            LPack.LPackNumberRead(row, "image"),
            LPack.LPackNumberRead(row, "caption"),
            LPack.LPackTextRead(row, "prefix"),
            LRespellingLoader.LRespellingRewriteScan(row),
            LPack.LPackTextRead(row, "gloss"),
            LScriptEpochScan(row) is { Count: > 0 } epochs ? epochs : shared);
    }

    private static IReadOnlyList<LEpoch> LScriptEpochScan(JsonElement row)
    {
        if (row.ValueKind != JsonValueKind.Object
            || !row.TryGetProperty(LScriptEpochKey, out JsonElement rows)
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
}
