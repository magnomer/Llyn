using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private readonly List<Action<LBulletin>> _lEngineObservers = [];

    public void LEngineObserverAttach(Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (_lEngineGate)
        {
            if (!_lEngineObservers.Contains(observer))
            {
                _lEngineObservers.Add(observer);
            }
        }
    }

    public void LEngineObserverDetach(Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (_lEngineGate)
        {
            _lEngineObservers.Remove(observer);
        }
    }

    internal void LEngineBulletinRaise(LSubject subject, long id)
    {
        Action<LBulletin>[] observers;
        lock (_lEngineGate)
        {
            if (_lEngineObservers.Count == 0)
            {
                return;
            }

            observers = [.. _lEngineObservers];
        }

        LBulletin bulletin = new(subject, id);
        foreach (Action<LBulletin> observer in observers)
        {
            observer(bulletin);
        }
    }
}
