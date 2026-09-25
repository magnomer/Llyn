using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Llyn.UIDeportment;

public static class LSplice
{
    public static void LSpliceApply<LSpliceRow>(
        ObservableCollection<LSpliceRow> held,
        IReadOnlyList<LSpliceRow> fresh,
        Func<LSpliceRow, LSpliceRow, bool> match,
        Action<LSpliceRow, LSpliceRow> mark)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(fresh);
        ArgumentNullException.ThrowIfNull(match);
        ArgumentNullException.ThrowIfNull(mark);

        if (LSpliceMatchCheck(held, fresh, match))
        {
            for (int index = 0; index < held.Count; index++)
            {
                mark(held[index], fresh[index]);
            }

            return;
        }

        held.Clear();
        foreach (LSpliceRow row in fresh)
        {
            held.Add(row);
        }
    }

    public static IReadOnlyList<LSpliceItem> LSpliceBuild<LSpliceRow, LSpliceItem>(
        IReadOnlyList<LSpliceRow> rows, Func<LSpliceRow, LSpliceItem> make)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(make);

        List<LSpliceItem> built = new(rows.Count);
        foreach (LSpliceRow row in rows)
        {
            built.Add(make(row));
        }

        return built;
    }

    private static bool LSpliceMatchCheck<LSpliceRow>(
        ObservableCollection<LSpliceRow> held,
        IReadOnlyList<LSpliceRow> fresh,
        Func<LSpliceRow, LSpliceRow, bool> match)
    {
        if (held.Count != fresh.Count)
        {
            return false;
        }

        for (int index = 0; index < held.Count; index++)
        {
            if (!match(held[index], fresh[index]))
            {
                return false;
            }
        }

        return true;
    }
}
