using System;
using System.Collections.Generic;

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

        List<string> readings = [];
        foreach (string character in LGlyph.LGlyphScan(headword))
        {
            if (LFanqieMarkedFind(groups, character) is { Length: > 0 } reading)
            {
                readings.Add(reading);
            }
        }

        return readings.Count == 0 ? string.Empty : '/' + string.Join(' ', readings) + '/';
    }

    private static string LFanqieMarkedFind(IReadOnlyList<LFanqieGroup> groups, string character)
    {
        List<LFanqieRow> marked = [];
        foreach (LFanqieGroup group in groups)
        {
            foreach (LFanqieRow row in group.LFanqieGroupRows)
            {
                if (row.LFanqieRowMarked
                    && row.LFanqieRowSpoken
                    && string.Equals(row.LFanqieRowCharacter, character, StringComparison.Ordinal))
                {
                    int place = 0;
                    while (place < marked.Count
                           && marked[place].LFanqieRowRepresentative <= row.LFanqieRowRepresentative)
                    {
                        place++;
                    }

                    marked.Insert(place, row);
                }
            }
        }

        List<string> readings = new(marked.Count);
        foreach (LFanqieRow row in marked)
        {
            readings.Add(row.LFanqieRowReading);
        }

        return string.Join(", ", readings);
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
        List<LFanqieGroup> groups = [];
        foreach (string character in characters)
        {
            bool first = true;
            string shown = string.Empty;
            foreach (LFanqieBook book in books)
            {
                if (LFanqieRow.LFanqieRowScan(rows, character, book) is not IReadOnlyList<LFanqieRow> found)
                {
                    continue;
                }

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
