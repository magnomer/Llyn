using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LFanqieGroup(
    string LFanqieGroupHeading,
    string LFanqieGroupLabel,
    string LFanqieGroupSource,
    IReadOnlyList<LFanqieRow> LFanqieGroupRows)
{
    public static IReadOnlyList<LFanqieGroup> LFanqieGroupScan(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<LFanqieBook> books)
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
                groups.Add(new LFanqieGroup(heading, label, book.LFanqieBookSource, found));
                first = false;
                shown = book.LFanqieBookName;
            }
        }

        return groups;
    }
}
