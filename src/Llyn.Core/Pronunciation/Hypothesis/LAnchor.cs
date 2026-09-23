using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public static class LAnchor
{
    public static IReadOnlyList<long> LAnchorNormalize(IReadOnlyList<long>? anchors)
    {
        if (anchors is null || anchors.Count == 0)
        {
            return [];
        }

        SortedSet<long> kept = [];
        foreach (long id in anchors)
        {
            if (id > 0)
            {
                kept.Add(id);
            }
        }

        return [.. kept];
    }

    public static IReadOnlyList<long> LAnchorToggle(IReadOnlyList<long> anchors, long fanqieId, bool anchored)
    {
        ArgumentNullException.ThrowIfNull(anchors);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(fanqieId);

        SortedSet<long> kept = [.. LAnchorNormalize(anchors)];
        if (anchored)
        {
            kept.Add(fanqieId);
        }
        else
        {
            kept.Remove(fanqieId);
        }

        return [.. kept];
    }

    public static bool LAnchorMatch(IReadOnlyList<long> one, IReadOnlyList<long> other)
    {
        ArgumentNullException.ThrowIfNull(one);
        ArgumentNullException.ThrowIfNull(other);

        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index] != other[index])
            {
                return false;
            }
        }

        return true;
    }

    public static IReadOnlyList<LAnchorRow> LAnchorRowScan(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, IReadOnlyList<string> classes)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(anchors);
        ArgumentNullException.ThrowIfNull(classes);

        List<LAnchorRow> marked = new(rows.Count);
        foreach (LFanqieRow row in rows)
        {
            if (!row.LFanqieRowStored)
            {
                continue;
            }

            bool estimated = row.LFanqieRowClassed && classes.Contains(row.LFanqieRowClass);
            marked.Add(new LAnchorRow(row, anchors.Contains(row.LFanqieRowId), estimated));
        }

        return marked;
    }

    public static bool LAnchorCheck(IReadOnlyList<LFanqieRow> rows, string headword)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return rows.Count > 0 && LGlyph.LGlyphSingleCheck(headword);
    }

    public static string LAnchorTextFormat(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string headword, string separator)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(anchors);

        if (!LAnchorCheck(rows, headword))
        {
            return string.Empty;
        }

        List<string> readings = [];
        foreach (LFanqieRow row in rows)
        {
            if (!row.LFanqieRowStored)
            {
                continue;
            }

            if (!anchors.Contains(row.LFanqieRowId))
            {
                continue;
            }

            string reading = row.LFanqieRowSpoken ? row.LFanqieRowSlashed : row.LFanqieRowSummary;
            if (!readings.Contains(reading))
            {
                readings.Add(reading);
            }
        }

        return string.Join(separator, readings);
    }
}
