using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private readonly List<LObserver> _lEngineObservers = [];

    public void LEngineObserverAttach(LObserver observer)
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

    public void LEngineObserverDetach(LObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (_lEngineGate)
        {
            _lEngineObservers.Remove(observer);
        }
    }

    private void LEngineBulletinRaise(LSubject subject, string id)
    {
        LObserver[] observers;
        lock (_lEngineGate)
        {
            if (_lEngineObservers.Count == 0)
            {
                return;
            }

            observers = [.. _lEngineObservers];
        }

        LBulletin bulletin = new(subject, id ?? string.Empty);
        foreach (LObserver observer in observers)
        {
            observer.LObserverBulletinHandle(bulletin);
        }
    }
}
