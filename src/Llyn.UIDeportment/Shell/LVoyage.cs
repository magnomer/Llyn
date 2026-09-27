using System;
using System.Collections.Generic;

namespace Llyn.UIDeportment;

public sealed class LVoyage
{
    private const int LVoyageCap = 50;

    private readonly LinkedList<(string LVoyageTab, long LVoyageId)> _lVoyagePast = new();

    private readonly LinkedList<(string LVoyageTab, long LVoyageId)> _lVoyageFuture = new();

    public bool LVoyagePastCheck()
    {
        return _lVoyagePast.Count > 0;
    }

    public bool LVoyageFutureCheck()
    {
        return _lVoyageFuture.Count > 0;
    }

    public void LVoyageRecord(string tab, long id)
    {
        ArgumentNullException.ThrowIfNull(tab);

        if (id == 0 || _lVoyagePast.Last?.Value == (tab, id))
        {
            return;
        }

        LVoyageStationAdd(_lVoyagePast, (tab, id));
        _lVoyageFuture.Clear();
    }

    public bool LVoyageRetreat(string tab, long id, Func<string, long, bool> show)
    {
        return LVoyageRun(_lVoyagePast, _lVoyageFuture, (tab, id), show);
    }

    public bool LVoyageAdvance(string tab, long id, Func<string, long, bool> show)
    {
        return LVoyageRun(_lVoyageFuture, _lVoyagePast, (tab, id), show);
    }

    private static void LVoyageStationAdd(
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
        LVoyageStationAdd(target, current);
        return true;
    }
}
