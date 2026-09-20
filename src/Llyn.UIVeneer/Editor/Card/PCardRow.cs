using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Llyn.UIVeneer;

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

    internal static int PCardRowResolve<PCardItem>(IReadOnlyList<PCardItem> rows, Func<PCardItem, long?> key, int limit)
    {
        int count = 0;
        for (int index = 0; index < limit && index < rows.Count; index++)
        {
            if (key(rows[index]) is not null)
            {
                count++;
            }
        }

        return count;
    }
}
