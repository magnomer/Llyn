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
        List<PCardItem> shown = [];
        bool[] taken = new bool[rows.Count];
        foreach (PCardDraft draft in drafts)
        {
            long wanted = id(draft);
            int found = -1;
            for (int index = 0; index < rows.Count && found < 0; index++)
            {
                if (!taken[index] && key(rows[index]) == wanted)
                {
                    found = index;
                }
            }

            if (found < 0)
            {
                shown.Add(create(draft));
                continue;
            }

            taken[found] = true;
            shown.Add(update(rows[found], draft));
        }

        List<PCardItem> standing = [.. rows];
        int next = 0;
        foreach (PCardItem row in standing)
        {
            if (key(row) is null)
            {
                continue;
            }

            if (next < shown.Count && ReferenceEquals(row, shown[next]))
            {
                next++;
                continue;
            }

            rows.Remove(row);
        }

        for (; next < shown.Count; next++)
        {
            rows.Add(shown[next]);
        }
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
