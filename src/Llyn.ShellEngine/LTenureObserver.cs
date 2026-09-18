using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LTenure : LObserver
{
    private readonly List<(LSubject, LObserver, long?)> _lTenureObservers = [];

    private int _lTenurePreparing;

    public void LTenureObserverAttach(LSubject subject, LObserver observer)
    {
        LTenureObserverInsert(subject, observer, null);
    }

    public void LTenureDraftAttach(LSubject subject, LObserver observer)
    {
        LTenureObserverInsert(subject, observer, LTenureId);
    }

    public void LTenureEntryAttach(LSubject subject, LObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);
        if (LTenureRead()?.LDraftEntryId is > 0 and var id)
        {
            LTenureObserverInsert(subject, observer, id);
        }
    }

    private void LTenureObserverInsert(LSubject subject, LObserver observer, long? id)
    {
        ArgumentNullException.ThrowIfNull(observer);
        lock (_lTenureGate)
        {
            if (!_lTenureEnded)
            {
                _lTenureObservers.Add((subject, observer, id));
            }
        }
    }

    public void LObserverBulletinHandle(LBulletin bulletin)
    {
        ArgumentNullException.ThrowIfNull(bulletin);
        (LSubject, LObserver, long?)[] observers;
        lock (_lTenureGate)
        {
            if (_lTenureEnded)
            {
                return;
            }

            if (_lTenurePreparing > 0 && bulletin.LBulletinSubject == LSubject.LSubjectDraft
                && bulletin.LBulletinId == LTenureId)
            {
                return;
            }

            observers = [.. _lTenureObservers];
        }

        foreach ((LSubject subject, LObserver observer, long? id) in observers)
        {
            if (subject == bulletin.LBulletinSubject && (id is null || id == bulletin.LBulletinId))
            {
                observer.LObserverBulletinHandle(bulletin);
            }
        }
    }

    public LDraft? LTenurePrepare(Action prepare)
    {
        ArgumentNullException.ThrowIfNull(prepare);
        lock (_lTenureTurn)
        {
            lock (_lTenureGate)
            {
                if (_lTenureEnded)
                {
                    return null;
                }

                _lTenurePreparing++;
            }

            try
            {
                prepare();
                return LTenureRead();
            }
            finally
            {
                lock (_lTenureGate)
                {
                    _lTenurePreparing--;
                }
            }
        }
    }

    private void LTenureObserverClear()
    {
        _lEngine.LEngineObserverDetach(this);
        lock (_lTenureGate)
        {
            _lTenureObservers.Clear();
        }
    }
}
