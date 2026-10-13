using System;
using System.Collections.Generic;

namespace Llyn.Conduct;

internal sealed class CVoyage
{
    private const int LVoyageCap = 50;

    private readonly LinkedList<(string LVoyageTab, long LVoyageId)> _cVoyagePast = new();

    private readonly LinkedList<(string LVoyageTab, long LVoyageId)> _cVoyageFuture = new();

    internal CVoyageState LVoyageRead()
    {
        return new CVoyageState(_cVoyagePast.Count > 0, _cVoyageFuture.Count > 0);
    }

    internal void LVoyageStationAdd(string tab, long id)
    {
        ArgumentNullException.ThrowIfNull(tab);

        if (id == 0 || _cVoyagePast.Last?.Value == (tab, id))
        {
            return;
        }

        LVoyageTrailAdd(_cVoyagePast, (tab, id));
        _cVoyageFuture.Clear();
    }

    internal bool LVoyageUndo(string tab, long id, Func<string, long, bool> show)
    {
        return LVoyageRun(_cVoyagePast, _cVoyageFuture, (tab, id), show);
    }

    internal bool LVoyageRedo(string tab, long id, Func<string, long, bool> show)
    {
        return LVoyageRun(_cVoyageFuture, _cVoyagePast, (tab, id), show);
    }

    private static void LVoyageTrailAdd(
        LinkedList<(string LVoyageTab, long LVoyageId)> trail, (string LVoyageTab, long LVoyageId) station)
    {
        if (station.LVoyageId == 0)
        {
            return;
        }

        if (trail.Count >= LVoyageCap)
        {
            trail.RemoveFirst();
        }

        trail.AddLast(station);
    }

    private static bool LVoyageRun(
        LinkedList<(string LVoyageTab, long LVoyageId)> source,
        LinkedList<(string LVoyageTab, long LVoyageId)> target,
        (string LVoyageTab, long LVoyageId) current,
        Func<string, long, bool> show)
    {
        ArgumentNullException.ThrowIfNull(show);

        if (source.Last is null)
        {
            return false;
        }

        (string LVoyageTab, long LVoyageId) next = source.Last.Value;
        if (!show(next.LVoyageTab, next.LVoyageId))
        {
            return false;
        }

        source.RemoveLast();
        LVoyageTrailAdd(target, current);
        return true;
    }
}
