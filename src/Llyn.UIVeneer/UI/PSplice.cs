using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Llyn.UIVeneer;

internal static class PSplice
{
    internal static void PSpliceApply<PSpliceRow>(
        ObservableCollection<PSpliceRow> held,
        IReadOnlyList<PSpliceRow> fresh,
        Func<PSpliceRow, PSpliceRow, bool> match,
        Action<PSpliceRow, PSpliceRow> mark)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(fresh);
        ArgumentNullException.ThrowIfNull(match);
        ArgumentNullException.ThrowIfNull(mark);

        if (PSpliceMatchCheck(held, fresh, match))
        {
            for (int index = 0; index < held.Count; index++)
            {
                mark(held[index], fresh[index]);
            }

            return;
        }

        held.Clear();
        foreach (PSpliceRow row in fresh)
        {
            held.Add(row);
        }
    }

    private static bool PSpliceMatchCheck<PSpliceRow>(
        ObservableCollection<PSpliceRow> held,
        IReadOnlyList<PSpliceRow> fresh,
        Func<PSpliceRow, PSpliceRow, bool> match)
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
