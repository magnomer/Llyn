using System;
using System.Collections.Generic;

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
}
