using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public static class LEntryClerkTwin
{
    public static string[] LTwinRead(IReadOnlyList<LEntry> entries)
    {
        return LTwinRead(entries, entry => entry.LEntryHeadword, entry => entry.LEntryId);
    }

    public static string[] LTwinRead<LTwinRow>(
        IReadOnlyList<LTwinRow> rows, Func<LTwinRow, string> name, Func<LTwinRow, long> id)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(id);

        string[] names = new string[rows.Count];
        int[] places = new int[rows.Count];
        for (int index = 0; index < places.Length; index++)
        {
            places[index] = index;
        }

        LTwin.LTwinNameApply(places, place => name(rows[place]),
            (place, twinned) => names[place] = twinned, place => id(rows[place]));
        return names;
    }

    public static IReadOnlyList<string> LTwinNameResolve(IReadOnlyList<string> labels)
    {
        ArgumentNullException.ThrowIfNull(labels);

        string[] names = new string[labels.Count];
        int[] places = new int[labels.Count];
        for (int index = 0; index < places.Length; index++)
        {
            places[index] = index;
        }

        LTwin.LTwinNameApply(places, place => labels[place], (place, name) => names[place] = name);
        return names;
    }

    public static string LTwinNameRead(LStateValue value, string unknown, string fallback)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.LStateValueState == LState.LStateUnknown
            ? unknown
            : value.LStateValueShow() is { Length: > 0 } text ? text : fallback;
    }
}
