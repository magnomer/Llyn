using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PFanqieItem
{
    private PFanqieItem(string character, string book, string source, IReadOnlyList<PFanqieLine> lines)
    {
        PFanqieItemCharacter = character;
        PFanqieItemBook = book;
        PFanqieItemSource = source;
        PFanqieItemLines = lines;
    }

    public string PFanqieItemCharacter { get; }

    public string PFanqieItemBook { get; }

    public string PFanqieItemSource { get; }

    public IReadOnlyList<PFanqieLine> PFanqieItemLines { get; }

    internal static IReadOnlyList<PFanqieItem> PFanqieItemScan(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<LFanqieBook> books)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(books);

        List<string> characters = [];
        Dictionary<(string, string, string), List<PFanqieLine>> groups = [];
        foreach (LFanqieRow row in rows)
        {
            if (!characters.Contains(row.LFanqieRowCharacter))
            {
                characters.Add(row.LFanqieRowCharacter);
            }

            (string, string, string) key = (row.LFanqieRowCharacter, row.LFanqieRowBook, row.LFanqieRowSource);
            if (!groups.TryGetValue(key, out List<PFanqieLine>? group))
            {
                group = [];
                groups[key] = group;
            }

            group.Add(PFanqieLine.PFanqieLineCreate(row));
        }

        List<PFanqieItem> items = [];
        foreach (string character in characters)
        {
            bool first = true;
            string shown = string.Empty;
            foreach (LFanqieBook book in books)
            {
                (string, string, string) key = (character, book.LFanqieBookName, book.LFanqieBookSource);
                if (!groups.TryGetValue(key, out List<PFanqieLine>? group))
                {
                    continue;
                }

                string heading = first && characters.Count > 1 ? character : string.Empty;
                string label = book.LFanqieBookName == shown ? string.Empty : book.LFanqieBookName;
                items.Add(new PFanqieItem(heading, label, book.LFanqieBookSource, group));
                first = false;
                shown = book.LFanqieBookName;
            }
        }

        return items;
    }
}
