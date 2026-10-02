using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PCard
{
    internal static void PCardRowShow<PCardItem, PCardDraft>(
        ObservableCollection<PCardItem> rows,
        IReadOnlyList<PCardDraft> drafts,
        Func<PCardItem, long?> key,
        Func<PCardDraft, long> id,
        Func<PCardDraft, PCardItem> create,
        Func<PCardItem, PCardDraft, PCardItem> update)
    {
        HashSet<long> wanted = [];
        foreach (PCardDraft draft in drafts)
        {
            wanted.Add(id(draft));
        }

        for (int index = rows.Count - 1; index >= 0; index--)
        {
            if (key(rows[index]) is long held && !wanted.Remove(held))
            {
                rows.RemoveAt(index);
            }
        }

        int slot = -1;
        foreach (PCardDraft draft in drafts)
        {
            int found = PCardRowFind(rows, key, id(draft));
            if (found < 0)
            {
                slot++;
                rows.Insert(slot, create(draft));
                continue;
            }

            PCardItem shown = update(rows[found], draft);
            if (!ReferenceEquals(shown, rows[found]))
            {
                rows[found] = shown;
            }

            if (found <= slot)
            {
                rows.Move(found, slot);
            }
            else
            {
                slot = found;
            }
        }
    }

    internal static int PCardRowFind<PCardItem>(IReadOnlyList<PCardItem> rows, Func<PCardItem, long?> key, long id)
    {
        for (int index = 0; index < rows.Count; index++)
        {
            if (key(rows[index]) == id)
            {
                return index;
            }
        }

        return -1;
    }

    private static int PCardCaretFind<PCardItem>(IReadOnlyList<PCardItem> rows, PCardItem? anchor)
        where PCardItem : class
    {
        for (int index = 0; index < rows.Count; index++)
        {
            if (ReferenceEquals(rows[index], anchor))
            {
                return index;
            }
        }

        return rows.Count;
    }

    private static List<PCardItem> PCardCaretRead<PCardItem>(IReadOnlyList<PCardItem> rows, PCardItem? anchor)
        where PCardItem : class
    {
        List<PCardItem> trail = [];
        for (int index = PCardCaretFind(rows, anchor); index < rows.Count; index++)
        {
            trail.Add(rows[index]);
        }

        return trail;
    }

    private static PCardItem? PCardCaretResolve<PCardItem>(
        IReadOnlyList<PCardItem> rows,
        IReadOnlyList<PCardItem> trail)
        where PCardItem : class
    {
        foreach (PCardItem chip in trail)
        {
            for (int index = 0; index < rows.Count; index++)
            {
                if (ReferenceEquals(rows[index], chip))
                {
                    return chip;
                }
            }
        }

        return null;
    }
}
