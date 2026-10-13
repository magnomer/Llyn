using System;
using System.Collections.Generic;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LFanqieLoader
{
    private const string LFanqieKey = "fanqie";

    public static IReadOnlyList<LFanqieBook> LFanqiePackScan(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LFanqieKey, out JsonElement rows) ||
            rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LFanqieBook>();
        }

        List<LFanqieBook> books = new();
        foreach (JsonElement row in rows.EnumerateArray())
        {
            LFanqieBook? book = LFanqieRowRead(row);
            if (book is not null)
            {
                books.Add(book);
            }
        }

        return books;
    }

    private static LFanqieBook? LFanqieRowRead(JsonElement row)
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

        return new LFanqieBook(
            name,
            url,
            LEnvelope.LEnvelopePayloadRead(row),
            pattern,
            LPack.LPackTextRead(row, "busy"),
            LPack.LPackNumberRead(row, "interval"),
            LPack.LPackTextRead(row, "split"),
            LPack.LPackTextRead(row, "head"),
            LPack.LPackTextRead(row, "column"),
            LPack.LPackTextRead(row, "rounded"),
            LPack.LPackTextRead(row, "source")?.Trim(),
            LPack.LPackTextRead(row, "line"),
            LPack.LPackTextRead(row, "spelling"));
    }
}
