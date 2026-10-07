using System;
using System.Collections.Generic;
using System.Globalization;

namespace Llyn.Core;

public static class LTwin
{
    public static void LTwinNameApply<LTwinRow>(
        IReadOnlyList<LTwinRow> rows,
        Func<LTwinRow, string> read,
        Func<LTwinRow, string> group,
        Action<LTwinRow, string> write)
    {
        ArgumentNullException.ThrowIfNull(rows);

        int[] order = new int[rows.Count];
        for (int index = 0; index < order.Length; index++)
        {
            order[index] = index;
        }

        LTwinNameApply(rows, read, group, write, order);
    }

    public static void LTwinNameApply<LTwinRow>(
        IReadOnlyList<LTwinRow> rows,
        Func<LTwinRow, string> read,
        Func<LTwinRow, string> group,
        Action<LTwinRow, string> write,
        Func<LTwinRow, long> rank)
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

        LTwinNameApply(rows, read, group, write, order);
    }

    private static void LTwinNameApply<LTwinRow>(
        IReadOnlyList<LTwinRow> rows,
        Func<LTwinRow, string> read,
        Func<LTwinRow, string> group,
        Action<LTwinRow, string> write,
        int[] order)
    {
        ArgumentNullException.ThrowIfNull(read);
        ArgumentNullException.ThrowIfNull(group);
        ArgumentNullException.ThrowIfNull(write);

        Dictionary<(string, string), int> shared = [];
        foreach (LTwinRow row in rows)
        {
            (string, string) key = (group(row), read(row));
            shared[key] = shared.TryGetValue(key, out int seen) ? seen + 1 : 1;
        }

        Dictionary<(string, string), int> taken = [];
        foreach (int index in order)
        {
            LTwinRow row = rows[index];
            string name = read(row);
            (string, string) key = (group(row), name);
            if (shared[key] < 2)
            {
                write(row, name);
                continue;
            }

            int place = taken.TryGetValue(key, out int given) ? given + 1 : 1;
            taken[key] = place;
            write(row, name + " (" + place.ToString(CultureInfo.CurrentCulture) + ")");
        }
    }
}
