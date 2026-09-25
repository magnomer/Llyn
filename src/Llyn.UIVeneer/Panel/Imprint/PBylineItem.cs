using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PBylineItem
{
    internal PBylineItem(long id, string name, string word)
    {
        PBylineItemId = id;

        int found = -1;
        int size = 0;
        if (word.Length != 0)
        {
            found = CultureInfo.CurrentCulture.CompareInfo.IndexOf(
                name, word, CompareOptions.IgnoreCase, out size);
        }

        if (found < 0)
        {
            PBylineItemLead = name;
            PBylineItemMark = string.Empty;
            PBylineItemTail = string.Empty;
            return;
        }

        PBylineItemLead = name[..found];
        PBylineItemMark = name.Substring(found, size);
        PBylineItemTail = name[(found + size)..];
    }

    internal long PBylineItemId { get; }

    public string PBylineItemLead { get; }

    public string PBylineItemMark { get; }

    public string PBylineItemTail { get; }

    internal static long? PBylineItemRead(object? chosen)
    {
        return (chosen as PBylineItem)?.PBylineItemId;
    }

    internal static IReadOnlyList<PBylineItem> PBylineItemBuild(IReadOnlyList<LAuthor> rows, string word)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(word);

        List<PBylineItem> built = new(rows.Count);
        foreach (LAuthor author in rows)
        {
            built.Add(new PBylineItem(author.LAuthorId, author.LAuthorName, word));
        }

        return built;
    }
}
