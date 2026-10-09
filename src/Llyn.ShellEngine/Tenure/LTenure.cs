using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LTenure
{
    private readonly object _lTenureGate = new();

    private readonly object _lTenureTurn = new();

    private readonly LEngine _lEngine;

    private readonly LSubject _lTenureSubject;

    private readonly LTenureQueue _lTenureQueue;

    internal LTenure(LEngine engine, LSubject subject, long id)
    {
        _lEngine = engine;
        _lTenureSubject = subject;
        LTenureId = id;
        _lTenureQueue = new(engine, id, _lTenureGate, _lTenureTurn, () => LTenureGauge?.LTenureGaugeRaise());
        LTenureGauge = new(engine, id, _lTenureGate, _lTenureQueue);
        LTenureErrand = new(engine, this, _lTenureGate);
        LTenureHerald = new(engine.LEngineHearth, engine.LEngineDraft, LTenureId, _lTenureGate, _lTenureQueue);
    }

    public long LTenureId { get; }

    public LTenureGauge LTenureGauge { get; }

    public LErrand LTenureErrand { get; }

    internal LTenureHerald LTenureHerald { get; }

    internal LEngine LTenureEngine => _lEngine;

    public LDraft? LTenureRead()
    {
        lock (_lTenureGate)
        {
            if (_lTenureQueue.LTenureQueueEnded)
            {
                return null;
            }
        }

        return _lEngine.LEngineDraft.LEngineDraftRead(LTenureId);
    }

    public string LTenureLanguageRead()
    {
        return LTenureRead()?.LDraftContent.LEntryDraftLanguage ?? string.Empty;
    }

    public void LTenureRequestDefer(LRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        int delay = _lEngine.LEngineTenure.LEngineTenureDelay;
        lock (_lTenureGate)
        {
            if (!_lTenureQueue.LTenureQueueLive)
            {
                return;
            }

            _ = _lTenureQueue.LTenureQueueStart(request, delay);
            if (delay > 0)
            {
                return;
            }
        }

        LTenurePersist();
    }

    public void LTenureRequestApply(LRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        lock (_lTenureTurn)
        {
            _lTenureQueue.LTenureQueueDispatch(null);
            _lTenureQueue.LTenureQueueApply([request]);
        }
    }

    public void LTenurePersist()
    {
        lock (_lTenureTurn)
        {
            _lTenureQueue.LTenureQueueDispatch(null);
        }
    }

    public bool LTenureReadyCheck()
    {
        LTenurePersist();
        return LTenureGauge.LTenureGaugeCheck(LTenureGauge.LTenureGaugeRead());
    }

    public bool LTenureChangeCheck()
    {
        LTenurePersist();
        return LTenureGauge.LTenureGaugeRead().LTenureStateChanged;
    }

    public LDraft? LTenureUndo()
    {
        return _lTenureQueue.LTenureQueueRestore(_lEngine.LEngineRequest.LEngineChronicleUndo);
    }

    public LDraft? LTenureRedo()
    {
        return _lTenureQueue.LTenureQueueRestore(_lEngine.LEngineRequest.LEngineChronicleRedo);
    }

    public void LTenureSweep()
    {
        lock (_lTenureGate)
        {
            if (_lTenureQueue.LTenureQueueEnded)
            {
                return;
            }
        }

        _lEngine.LEngineDraft.LEngineDraftSweep(LTenureId);
        LTenureGauge.LTenureGaugeRaise();
    }

    public void LTenureCancel()
    {
        lock (_lTenureGate)
        {
            LTenureErrand.LErrandStop();
            if (!_lTenureQueue.LTenureQueueClose())
            {
                return;
            }
        }

        try
        {
            LTenureHerald.LTenureObserverClear();
            _lEngine.LEngineDraft.LEngineDraftCancel(LTenureId);
        }
        catch (Exception exception) when (LWorkspaceClerk.LWorkspaceRefusedCheck(exception))
        {
        }
        finally
        {
            LTenureGauge.LTenureGaugeRaise();
        }
    }

    public long? LTenureFinish(bool store, Func<bool> unreadableSeam)
    {
        ArgumentNullException.ThrowIfNull(unreadableSeam);

        lock (_lTenureTurn)
        {
            lock (_lTenureGate)
            {
                if (_lTenureQueue.LTenureQueueEnded)
                {
                    return null;
                }
            }

            if (!store)
            {
                LTenureCancel();
                return null;
            }

            _lTenureQueue.LTenureQueueDispatch(null);

            Exception? fault = _lTenureQueue.LTenureQueueFault;
            if (fault is not null)
            {
                ExceptionDispatchInfo.Throw(fault);
            }

            if (!_lEngine.LEngineDraft.LEngineDraftCheck(LTenureId))
            {
                LTenureCancel();
                return null;
            }

            long stored;
            try
            {
                stored = _lEngine.LEngineTenure.LEngineTenureCommit(_lTenureSubject, LTenureId);
            }
            catch (Exception exception) when (LWorkspaceClerk.LWorkspaceIllegibleCheck(exception) && unreadableSeam())
            {
                LTenureSweep();
                return LTenureFinish(store, static () => false);
            }
            lock (_lTenureGate)
            {
                LTenureErrand.LErrandStop();
                _lTenureQueue.LTenureQueueClose();
            }

            LTenureHerald.LTenureObserverClear();
            LTenureGauge.LTenureGaugeRaise();
            return stored;
        }
    }

    public void LTenureObserverAttach(LSubject subject, Action<LBulletin> observer)
    {
        LTenureHerald.LTenureObserverInsert(subject, observer, null);
    }

    public void LTenureDraftAttach(LSubject subject, Action<LBulletin> observer)
    {
        LTenureHerald.LTenureObserverInsert(subject, observer, LTenureId);
    }

    public void LTenureEntryAttach(LSubject subject, Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);
        if (LTenureRead()?.LDraftStored is long id)
        {
            LTenureHerald.LTenureObserverInsert(subject, observer, id);
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
        IReadOnlyList<LRequest> requests = _lEngine.LEngineDraft.LEngineDraftPrepare(LTenureId);
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
                if (_lTenureQueue.LTenureQueueEnded)
                {
                    return null;
                }

                LTenureHerald.LTenurePrepareStart();
            }

            try
            {
                prepare();
                return LTenureRead();
            }
            finally
            {
                LTenureHerald.LTenurePrepareFinish();
            }
        }
    }

    public long? LTenureStoredRead()
    {
        try
        {
            return LTenureRead()?.LDraftStored;
        }
        catch (Exception exception) when (LWorkspaceClerk.LWorkspaceRefusedCheck(exception))
        {
            return null;
        }
    }

}
