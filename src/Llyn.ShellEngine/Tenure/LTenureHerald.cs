using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LTenureHerald
{
    private readonly LEngineHearth _lTenureHeraldHearth;

    private readonly LDraftFacade _lTenureHeraldDraft;

    private readonly long _lTenureHeraldId;

    private readonly object _lTenureHeraldGate;

    private readonly LTenureQueue _lTenureHeraldQueue;

    private readonly LBulletinRoster _lTenureHeraldRoster = new(null);

    private int _lTenureHeraldPreparing;

    private LDraft? _lTenureHeraldKept;

    private int _lTenureHeraldRound;

    internal LTenureHerald(LEngineHearth hearth, LDraftFacade draft, long id, object gate, LTenureQueue queue)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(queue);
        _lTenureHeraldHearth = hearth;
        _lTenureHeraldDraft = draft;
        _lTenureHeraldId = id;
        _lTenureHeraldGate = gate;
        _lTenureHeraldQueue = queue;
        _lTenureHeraldHearth.LEngineObserverAttach(LTenureBulletinHandle);
    }

    internal void LTenureObserverInsert(LSubject subject, Action<LBulletin> observer, long? id)
    {
        ArgumentNullException.ThrowIfNull(observer);
        lock (_lTenureHeraldGate)
        {
            if (!_lTenureHeraldQueue.LTenureQueueEnded)
            {
                _lTenureHeraldRoster.LBulletinRosterAttach(subject, observer, id);
            }
        }
    }

    private void LTenureBulletinHandle(LBulletin bulletin)
    {
        (LSubject?, Action<LBulletin>, long?)[] snapshot;
        lock (_lTenureHeraldGate)
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectDraft)
            {
                LTenureKeptClear();
            }

            if (_lTenureHeraldQueue.LTenureQueueEnded)
            {
                return;
            }

            if (_lTenureHeraldPreparing > 0 && bulletin.LBulletinSubject == LSubject.LSubjectDraft
                && bulletin.LBulletinId == _lTenureHeraldId)
            {
                return;
            }

            snapshot = _lTenureHeraldRoster.LBulletinRosterRead();
        }

        _lTenureHeraldRoster.LBulletinRosterDispatch(bulletin, snapshot);
    }

    internal void LTenurePrepareStart()
    {
        lock (_lTenureHeraldGate)
        {
            _lTenureHeraldPreparing++;
        }
    }

    internal void LTenurePrepareFinish()
    {
        lock (_lTenureHeraldGate)
        {
            _lTenureHeraldPreparing--;
            LTenureKeptClear();
        }
    }

    internal LDraft? LTenureKeptRead()
    {
        int round;
        lock (_lTenureHeraldGate)
        {
            if (_lTenureHeraldQueue.LTenureQueueEnded)
            {
                return null;
            }

            if (_lTenureHeraldKept is LDraft kept)
            {
                return kept;
            }

            round = _lTenureHeraldRound;
        }

        LDraft? read;
        try
        {
            read = _lTenureHeraldDraft.LEngineDraftRead(_lTenureHeraldId);
        }
        catch (Exception exception) when (LWorkspaceClerk.LWorkspaceRefusedCheck(exception))
        {
            return null;
        }

        lock (_lTenureHeraldGate)
        {
            if (round == _lTenureHeraldRound)
            {
                _lTenureHeraldKept = read;
            }
        }

        return read;
    }

    private void LTenureKeptClear()
    {
        _lTenureHeraldKept = null;
        _lTenureHeraldRound++;
    }

    internal void LTenureObserverClear()
    {
        _lTenureHeraldHearth.LEngineObserverDetach(LTenureBulletinHandle);
        lock (_lTenureHeraldGate)
        {
            _lTenureHeraldRoster.LBulletinRosterClear();
        }
    }
}
