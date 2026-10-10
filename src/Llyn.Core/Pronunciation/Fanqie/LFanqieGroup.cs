using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LFanqieGroup(
    string LFanqieGroupHeading,
    string LFanqieGroupLabel,
    string LFanqieGroupSource,
    IReadOnlyList<LFanqieRow> LFanqieGroupRows,
    IReadOnlyList<string>? LFanqieGroupStems = null)
{
    public IReadOnlyList<string> LFanqieGroupStems { get; init; } = LFanqieGroupStems ?? [];

    public static string LFanqieReadingFormat(IReadOnlyList<LFanqieGroup> groups, string headword)
    {
        ArgumentNullException.ThrowIfNull(groups);
        ArgumentNullException.ThrowIfNull(headword);

        List<LFanqieRow> rows = [.. groups.SelectMany(static group => group.LFanqieGroupRows)];
        return LFanqieReadingFormat(
            LGlyph.LGlyphScan(headword).Select(character => LFanqieMarkedScan(rows, character)).ToList());
    }

    public static string LFanqieReadingFormat(IReadOnlyList<IReadOnlyList<string>> characters)
    {
        ArgumentNullException.ThrowIfNull(characters);

        List<string> readings = [.. characters
            .Where(static marked => marked.Count > 0)
            .Select(static marked => string.Join(", ", marked))];
        return readings.Count == 0 ? string.Empty : '/' + string.Join(' ', readings) + '/';
    }

    public static IReadOnlyList<string> LFanqieMarkedScan(IReadOnlyList<LFanqieRow> rows, string character)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return rows
            .Where(row => row.LFanqieRowMarked
                && row.LFanqieRowSpoken
                && string.Equals(row.LFanqieRowCharacter, character, StringComparison.Ordinal))
            .OrderBy(static row => row.LFanqieRowRepresentative)
            .Select(static row => row.LFanqieRowReading)
            .ToList();
    }

    public static IReadOnlyList<LFanqieGroup> LFanqieGroupScan(
        IReadOnlyList<LFanqieRow> rows,
        IReadOnlyList<LFanqieBook> books,
        IReadOnlyList<LShengfu>? shengfu = null,
        string separator = "")
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(books);

        IReadOnlyList<string> characters = LFanqieRow.LFanqieCharacterScan(rows);
        List<LFanqieRow> ordered = [.. LFanqieRow.LFanqieRowSort(rows, books)];
        List<LFanqieGroup> groups = [];
        foreach (string character in characters)
        {
            bool first = true;
            string shown = string.Empty;
            List<(LFanqieBook, IReadOnlyList<LFanqieRow>)> held = [];
            foreach (LFanqieBook book in books)
            {
                if (LFanqieRow.LFanqieRowScan(ordered, character, book) is IReadOnlyList<LFanqieRow> found)
                {
                    held.Add((book, found));
                }
            }

            foreach ((LFanqieBook book, IReadOnlyList<LFanqieRow> found) in held.OrderBy(
                         pair => ordered.IndexOf(pair.Item2[0])))
            {
                string heading = first && characters.Count > 1 ? character : string.Empty;
                string label = book.LFanqieBookMatch(shown) ? string.Empty : book.LFanqieBookName;
                IReadOnlyList<string> stems = first && shengfu is not null
                    ? LStem.LStemKeyScan(LShengfu.LShengfuTextFind(shengfu, character), separator)
                    : [];
                groups.Add(new LFanqieGroup(heading, label, book.LFanqieBookSource, found, stems));
                first = false;
                shown = book.LFanqieBookName;
            }
        }

        return groups;
    }
}
