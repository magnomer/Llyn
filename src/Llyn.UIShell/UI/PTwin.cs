using System;
using System.Collections.Generic;
using System.Globalization;

namespace Llyn.UIShell;

internal static class PTwin
{
    internal static void PTwinNameApply<PTwinRow>(
        IReadOnlyList<PTwinRow> rows, Func<PTwinRow, string> read, Action<PTwinRow, string> write)
    {
        ArgumentNullException.ThrowIfNull(rows);

        int[] order = new int[rows.Count];
        for (int index = 0; index < order.Length; index++)
        {
            order[index] = index;
        }

        PTwinNameApply(rows, read, write, order);
    }

    internal static void PTwinNameApply<PTwinRow>(
        IReadOnlyList<PTwinRow> rows,
        Func<PTwinRow, string> read,
        Action<PTwinRow, string> write,
        Func<PTwinRow, long> rank)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(rank);

        int[] order = new int[rows.Count];
        for (int index = 0; index < order.Length; index++)
        {
            order[index] = index;
        }

        Array.Sort(order, (one, other) =>
        {
            int result = rank(rows[one]).CompareTo(rank(rows[other]));
            return result != 0 ? result : one.CompareTo(other);
        });

        PTwinNameApply(rows, read, write, order);
    }

    private static void PTwinNameApply<PTwinRow>(
        IReadOnlyList<PTwinRow> rows,
        Func<PTwinRow, string> read,
        Action<PTwinRow, string> write,
        int[] order)
    {
        ArgumentNullException.ThrowIfNull(read);
        ArgumentNullException.ThrowIfNull(write);

        Dictionary<string, int> shared = new(StringComparer.Ordinal);
        foreach (PTwinRow row in rows)
        {
            string name = read(row);
            shared[name] = shared.TryGetValue(name, out int seen) ? seen + 1 : 1;
        }

        Dictionary<string, int> taken = new(StringComparer.Ordinal);
        foreach (int index in order)
        {
            PTwinRow row = rows[index];
            string name = read(row);
            if (shared[name] < 2)
            {
                write(row, name);
                continue;
            }

            int place = taken.TryGetValue(name, out int given) ? given + 1 : 1;
            taken[name] = place;
            write(row, name + " (" + place.ToString(CultureInfo.CurrentCulture) + ")");
        }
    }
}
