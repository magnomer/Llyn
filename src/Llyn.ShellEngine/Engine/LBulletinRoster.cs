using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LBulletinRoster
{
    private readonly Func<LBulletin, bool>? _lBulletinRosterFilter;

    private readonly List<(LSubject?, Action<LBulletin>, long?)> _lBulletinRosterObservers = [];

    internal LBulletinRoster(Func<LBulletin, bool>? filter)
    {
        _lBulletinRosterFilter = filter;
    }

    internal void LBulletinRosterAttach(LSubject? subject, Action<LBulletin> observer, long? id)
    {
        _lBulletinRosterObservers.Add((subject, observer, id));
    }

    internal bool LBulletinRosterCheck(Action<LBulletin> observer)
    {
        return _lBulletinRosterObservers.Exists(entry => entry.Item2 == observer);
    }

    internal void LBulletinRosterDetach(Action<LBulletin> observer)
    {
        int index = _lBulletinRosterObservers.FindIndex(entry => entry.Item2 == observer);
        if (index >= 0)
        {
            _lBulletinRosterObservers.RemoveAt(index);
        }
    }

    internal void LBulletinRosterClear()
    {
        _lBulletinRosterObservers.Clear();
    }

    internal (LSubject?, Action<LBulletin>, long?)[] LBulletinRosterRead()
    {
        return [.. _lBulletinRosterObservers];
    }

    internal void LBulletinRosterDispatch(LBulletin bulletin, (LSubject?, Action<LBulletin>, long?)[] snapshot)
    {
        foreach ((LSubject? subject, Action<LBulletin> observer, long? id) in snapshot)
        {
            if ((subject is null || subject == bulletin.LBulletinSubject)
                && (id is null || id == bulletin.LBulletinId)
                && (_lBulletinRosterFilter is null || _lBulletinRosterFilter(bulletin)))
            {
                observer(bulletin);
            }
        }
    }
}
