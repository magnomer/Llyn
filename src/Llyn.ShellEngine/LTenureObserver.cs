using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LTenure
{
    private readonly List<(LSubject, Action<LBulletin>, long?)> _lTenureObservers = [];

    private int _lTenurePreparing;

    public void LTenureObserverAttach(LSubject subject, Action<LBulletin> observer)
    {
        LTenureObserverInsert(subject, observer, null);
    }

    public void LTenureDraftAttach(LSubject subject, Action<LBulletin> observer)
    {
        LTenureObserverInsert(subject, observer, LTenureId);
    }

    public void LTenureEntryAttach(LSubject subject, Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);
        if (LTenureRead()?.LDraftEntryId is > 0 and var id)
        {
            LTenureObserverInsert(subject, observer, id);
        }
    }

    private void LTenureObserverInsert(LSubject subject, Action<LBulletin> observer, long? id)
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

    private void LTenureBulletinHandle(LBulletin bulletin)
    {
        (LSubject, Action<LBulletin>, long?)[] observers;
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

        foreach ((LSubject subject, Action<LBulletin> observer, long? id) in observers)
        {
            if (subject == bulletin.LBulletinSubject && (id is null || id == bulletin.LBulletinId))
            {
                observer(bulletin);
            }
        }
    }

    private const int LTenurePrepareRounds = 3;

    public LDraft? LTenurePrepare()
    {
        return LTenurePrepare(LTenureDraftPrepare);
    }

    private void LTenureDraftPrepare()
    {
        if (_lTenureSubject != LSubject.LSubjectEntry)
        {
            return;
        }

        int rounds = 0;
        while (rounds++ < LTenurePrepareRounds && LTenureDraftApply())
        {
        }
    }

    private bool LTenureDraftApply()
    {
        IReadOnlyList<LRequest> requests = _lEngine.LEngineDraftPrepare(LTenureId);
        foreach (LRequest request in requests)
        {
            LTenureRequestApply(request);
        }

        return requests.Count > 0;
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
        _lEngine.LEngineObserverDetach(LTenureBulletinHandle);
        lock (_lTenureGate)
        {
            _lTenureObservers.Clear();
        }
    }
}
