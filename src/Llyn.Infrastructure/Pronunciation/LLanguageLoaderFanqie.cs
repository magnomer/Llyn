using System;
using System.Collections.Generic;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed partial class LLanguageLoader : LLanguageVault
{
    private const string LLanguageLoaderFanqie = "fanqie";

    private static IReadOnlyList<LFanqieBook> LLanguageFanqieScan(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LLanguageLoaderFanqie, out JsonElement rows) ||
            rows.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<LFanqieBook>();
        }

        List<LFanqieBook> books = new();
        foreach (JsonElement row in rows.EnumerateArray())
        {
            LFanqieBook? book = LLanguageFanqieRead(row);
            if (book is not null)
            {
                books.Add(book);
            }
        }

        return books;
    }

    private static LFanqieBook? LLanguageFanqieRead(JsonElement row)
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

        return new LFanqieBook(
            name,
            url,
            LLanguageFormRead(row),
            pattern,
            LLanguageTextRead(row, "busy"),
            LLanguageNumberRead(row, "interval"),
            LLanguageTextRead(row, "split"),
            LLanguageTextRead(row, "head"),
            LLanguageTextRead(row, "column"),
            LLanguageTextRead(row, "rounded"),
            LLanguageTextRead(row, "source")?.Trim(),
            LLanguageTextRead(row, "line"),
            LLanguageTextRead(row, "spelling"));
    }
}
