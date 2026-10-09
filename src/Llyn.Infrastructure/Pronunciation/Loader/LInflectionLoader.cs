using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LInflectionLoader
{
    private const string LInflectionKey = "inflection";

    public static LInflectionBook? LInflectionPackRead(string language, JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty(LInflectionKey, out JsonElement section))
        {
            return null;
        }

        JsonElement book = section;
        if (section.ValueKind == JsonValueKind.String)
        {
            if (LPackFile.LPackFileLoad(language, section.GetString()!) is not JsonElement file)
            {
                return null;
            }

            book = file;
        }

        string stamp = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(book.GetRawText())));
        if (book.ValueKind == JsonValueKind.Object
            && book.TryGetProperty(LInflectionKey, out JsonElement inner)
            && inner.ValueKind == JsonValueKind.Object)
        {
            book = inner;
        }

        if (book.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        IReadOnlyList<LInflectionKind> kinds = LInflectionKindScan(book);
        IReadOnlyList<IReadOnlyList<long>> columns = LSpeechLoader.LSpeechCellsRead(book, "columns") ?? [];
        IReadOnlyList<LInflectionStem> stems = LInflectionStemScan(book, columns);
        if (kinds.Count == 0 || stems.Count == 0)
        {
            return null;
        }

        return new LInflectionBook(
            kinds,
            stems,
            LInflectionRuleScan(book, "rules"),
            LInflectionRuleScan(book, "folds"),
            LInflectionLayoutRead(book),
            stamp);
    }

    private static IReadOnlyList<LInflectionKind> LInflectionKindScan(JsonElement root)
    {
        if (!root.TryGetProperty("kinds", out JsonElement rows) || rows.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        List<LInflectionKind> kinds = [];
        foreach (JsonElement row in rows.EnumerateArray())
        {
            if (row.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            string name = LPack.LPackTextRead(row, "name")?.Trim() ?? string.Empty;
            string match = LPack.LPackTextRead(row, "match") ?? string.Empty;
            if (name.Length == 0 || match.Length == 0)
            {
                continue;
            }

            try
            {
                kinds.Add(new LInflectionKind(name, match));
            }
            catch (ArgumentException)
            {
            }
        }

        return kinds;
    }

    private static IReadOnlyList<LInflectionStem> LInflectionStemScan(
        JsonElement root, IReadOnlyList<IReadOnlyList<long>> columns)
    {
        if (!root.TryGetProperty("stems", out JsonElement rows) || rows.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        List<LInflectionStem> stems = [];
        foreach (JsonElement row in rows.EnumerateArray())
        {
            if (row.ValueKind != JsonValueKind.Object
                || LSpeechLoader.LSpeechNumbersRead(row, "values") is not { Count: > 0 } values
                || !row.TryGetProperty("templates", out JsonElement block)
                || block.ValueKind != JsonValueKind.Object
                || !row.TryGetProperty("endings", out JsonElement endings)
                || endings.ValueKind != JsonValueKind.Array
                || endings.GetArrayLength() != columns.Count)
            {
                continue;
            }

            Dictionary<string, string> templates = new(StringComparer.Ordinal);
            foreach (JsonProperty template in block.EnumerateObject())
            {
                if (template.Value.ValueKind == JsonValueKind.String)
                {
                    templates[template.Name] = template.Value.GetString()!;
                }
            }

            List<LInflectionEnding> zipped = [];
            int column = 0;
            foreach (JsonElement ending in endings.EnumerateArray())
            {
                if (ending.ValueKind == JsonValueKind.String)
                {
                    zipped.Add(new LInflectionEnding(columns[column], ending.GetString()!));
                }

                column++;
            }

            if (templates.Count > 0 && zipped.Count > 0)
            {
                stems.Add(new LInflectionStem(values, templates, zipped));
            }
        }

        return stems;
    }

    private static IReadOnlyList<LInflectionRule> LInflectionRuleScan(JsonElement root, string key)
    {
        if (!root.TryGetProperty(key, out JsonElement rows) || rows.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        List<LInflectionRule> rules = [];
        foreach (JsonElement row in rows.EnumerateArray())
        {
            if (row.ValueKind != JsonValueKind.Array
                || row.GetArrayLength() is < 2 or > 3
                || row.EnumerateArray().Any(static part => part.ValueKind != JsonValueKind.String)
                || string.IsNullOrEmpty(row[0].GetString()))
            {
                continue;
            }

            string? kind = row.GetArrayLength() == 3 ? row[2].GetString() : null;
            try
            {
                rules.Add(new LInflectionRule(row[0].GetString()!, row[1].GetString()!, kind));
            }
            catch (ArgumentException)
            {
            }
        }

        return rules;
    }

    private static LInflectionLayout? LInflectionLayoutRead(JsonElement root)
    {
        if (!root.TryGetProperty("layout", out JsonElement layout)
            || layout.ValueKind != JsonValueKind.Object
            || !layout.TryGetProperty("part", out JsonElement part)
            || part.ValueKind != JsonValueKind.Number
            || !part.TryGetInt64(out long code)
            || code <= 0
            || !layout.TryGetProperty("collapsed", out JsonElement collapsed)
            || collapsed.ValueKind != JsonValueKind.Object
            || !layout.TryGetProperty("expanded", out JsonElement expanded)
            || expanded.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        LInflectionLayout plain = new(code, LInflectionSheetRead(collapsed), LInflectionSheetRead(expanded));
        return plain with { LInflectionLayoutCustom = LInflectionCustomRead(layout, code) ?? plain };
    }

    private static LInflectionLayout? LInflectionCustomRead(JsonElement layout, long code)
    {
        if (!layout.TryGetProperty("custom", out JsonElement custom)
            || custom.ValueKind != JsonValueKind.Object
            || !custom.TryGetProperty("collapsed", out JsonElement collapsed)
            || collapsed.ValueKind != JsonValueKind.Object
            || !custom.TryGetProperty("expanded", out JsonElement expanded)
            || expanded.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        return new LInflectionLayout(code, LInflectionSheetRead(collapsed), LInflectionSheetRead(expanded));
    }

    private static LInflectionSheet LInflectionSheetRead(JsonElement sheet)
    {
        IReadOnlyList<IReadOnlyList<long>> columns = LSpeechLoader.LSpeechCellsRead(sheet, "columns") ?? [];
        IReadOnlyList<LInflectionLine> lines =
            sheet.TryGetProperty("groups", out JsonElement groups) && groups.ValueKind == JsonValueKind.Array
                ? LInflectionLineScan(groups, columns)
                : [];
        List<string> headers = [];
        if (sheet.TryGetProperty("headers", out JsonElement row) && row.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement header in row.EnumerateArray())
            {
                headers.Add(header.ValueKind == JsonValueKind.String ? header.GetString()! : string.Empty);
            }
        }

        return new LInflectionSheet(headers, lines);
    }

    private static IReadOnlyList<LInflectionLine> LInflectionLineScan(
        JsonElement groups, IReadOnlyList<IReadOnlyList<long>> columns)
    {
        List<LInflectionLine> lines = [];
        foreach (JsonElement group in groups.EnumerateArray())
        {
            if (group.ValueKind != JsonValueKind.Object
                || !group.TryGetProperty("lines", out JsonElement rows)
                || rows.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            string label = LPack.LPackTextRead(group, "label") ?? string.Empty;
            foreach (JsonElement row in rows.EnumerateArray())
            {
                if (row.ValueKind != JsonValueKind.Object
                    || LSpeechLoader.LSpeechNumbersRead(row, "values") is not IReadOnlyList<long> values)
                {
                    continue;
                }

                IReadOnlyList<IReadOnlyList<long>> cells = LSpeechLoader.LSpeechCellsRead(row, "cells") ?? columns;
                lines.Add(new LInflectionLine(
                    label,
                    LPack.LPackTextRead(row, "label") ?? string.Empty,
                    [.. cells.Select(cell => (IReadOnlyList<long>)[.. values.Union(cell).Order()])]));
                label = string.Empty;
            }
        }

        return lines;
    }
}
