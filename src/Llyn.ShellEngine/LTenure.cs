using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LTenure
{
    private static readonly LTenureState LTenureStateHalted = new(false, null, false, false, true);

    private static readonly LTenureState LTenureStateEnded = new(false, null, false, false, false);

    private readonly object _lTenureGate = new();

    private readonly object _lTenureTurn = new();

    private readonly LEngine _lEngine;

    private readonly LSubject _lTenureSubject;

    private readonly List<LRequest> _lTenureQueue = [];

    private CancellationTokenSource? _lTenurePending;

    private Exception? _lTenureFault;

    private bool _lTenureEnded;

    private LTenureState _lTenureLast;

    internal LTenure(LEngine engine, LSubject subject, long id)
    {
        _lEngine = engine;
        _lTenureSubject = subject;
        LTenureId = id;
        _lTenureLast = LTenureStateRead();
    }

    public long LTenureId { get; }

    public LDraft? LTenureRead()
    {
        lock (_lTenureGate)
        {
            if (_lTenureEnded)
            {
                return null;
            }
        }

        return _lEngine.LEngineDraftRead(LTenureId);
    }

    public string LTenureLanguageRead()
    {
        return LTenureRead()?.LDraftContent.LEntryDraftLanguage ?? string.Empty;
    }

    public LTenureState LTenureStateRead()
    {
        bool halted;
        lock (_lTenureGate)
        {
            if (_lTenureEnded)
            {
                return LTenureStateEnded;
            }

            halted = _lTenureFault is not null;
        }

        try
        {
            bool changed = _lEngine.LEngineDraftCheck(LTenureId, out string? refusal);
            return new LTenureState(
                changed,
                refusal,
                !halted && _lEngine.LEngineUndoCheck(LTenureId),
                !halted && _lEngine.LEngineRedoCheck(LTenureId),
                halted);
        }
        catch (Exception exception)
        {
            LTenureSuspend(exception);
            return LTenureStateHalted;
        }
    }

    public void LTenureRequestDefer(LRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        int delay = _lEngine.LEngineTenureDelay;
        lock (_lTenureGate)
        {
            if (_lTenureEnded || _lTenureFault is not null)
            {
                return;
            }

            LTenureRequestInsert(request);
            if (delay > 0)
            {
                LTenureStop();
                CancellationTokenSource pending = new();
                _lTenurePending = pending;
                _ = LTenureRun(pending, delay);
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
            LTenureDispatch(null);
            LTenureApply([request]);
        }
    }

    public void LTenurePersist()
    {
        lock (_lTenureTurn)
        {
            LTenureDispatch(null);
        }
    }

    public LDraft? LTenureUndo()
    {
        return LTenureRestore(_lEngine.LEngineChronicleUndo);
    }

    public LDraft? LTenureRedo()
    {
        return LTenureRestore(_lEngine.LEngineChronicleRedo);
    }

    public void LTenureSweep()
    {
        lock (_lTenureGate)
        {
            if (_lTenureEnded)
            {
                return;
            }
        }

        _lEngine.LEngineDraftSweep(LTenureId);
        LTenureStateRaise();
    }

    public void LTenureCancel()
    {
        lock (_lTenureGate)
        {
            LTenureStop();
            LTenureForayStop();
            _lTenureQueue.Clear();
            if (_lTenureEnded)
            {
                return;
            }

            _lTenureEnded = true;
        }

        try
        {
            LTenureObserverClear();
            _lEngine.LEngineDraftCancel(LTenureId);
        }
        catch (Exception)
        {
        }

        LTenureStateRaise();
    }

    public long? LTenureFinish(bool store)
    {
        lock (_lTenureTurn)
        {
            lock (_lTenureGate)
            {
                if (_lTenureEnded)
                {
                    return null;
                }
            }

            if (!store)
            {
                LTenureCancel();
                return null;
            }

            LTenureDispatch(null);

            Exception? fault;
            lock (_lTenureGate)
            {
                fault = _lTenureFault;
            }

            if (fault is not null)
            {
                ExceptionDispatchInfo.Throw(fault);
            }

            if (!_lEngine.LEngineDraftCheck(LTenureId))
            {
                LTenureCancel();
                return null;
            }

            long stored = LTenureCommit();
            lock (_lTenureGate)
            {
                LTenureForayStop();
                _lTenureEnded = true;
            }

            LTenureObserverClear();
            LTenureStateRaise();
            return stored;
        }
    }

    private long LTenureCommit()
    {
        return _lTenureSubject switch
        {
            LSubject.LSubjectEntry => _lEngine.LEngineDraftCommit(LTenureId).LOutcomeEntry.LEntryId,
            LSubject.LSubjectExample => _lEngine.LEngineExampleCommit(LTenureId).LExampleId,
            LSubject.LSubjectSituation => _lEngine.LEngineSituationCommit(LTenureId).LSituationId,
            LSubject.LSubjectReference => _lEngine.LEngineReferenceCommit(LTenureId).LReferenceId,
            LSubject.LSubjectAuthor => _lEngine.LEngineAuthorCommit(LTenureId).LAuthorId,
            _ => throw new InvalidOperationException(
                "A tenure holds only an entry, example, situation, reference or author."),
        };
    }

    private LDraft? LTenureRestore(Func<long, LDraft?> step)
    {
        LDraft? restored;
        lock (_lTenureTurn)
        {
            LTenureDispatch(null);

            lock (_lTenureGate)
            {
                if (_lTenureEnded || _lTenureFault is not null)
                {
                    return null;
                }
            }

            restored = step(LTenureId);
        }

        LTenureStateRaise();
        return restored;
    }

    private void LTenureRequestInsert(LRequest request)
    {
        string key = request.LRequestKey;
        for (int index = 0; index < _lTenureQueue.Count; index++)
        {
            if (string.Equals(_lTenureQueue[index].LRequestKey, key, StringComparison.Ordinal))
            {
                _lTenureQueue[index] = request;
                return;
            }
        }

        _lTenureQueue.Add(request);
    }

    private async Task LTenureRun(CancellationTokenSource pending, int delay)
    {
        try
        {
            await Task.Delay(delay, pending.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        lock (_lTenureTurn)
        {
            LTenureDispatch(pending);
        }
    }

    private void LTenureDispatch(CancellationTokenSource? pending)
    {
        LRequest[] taken;
        lock (_lTenureGate)
        {
            if (pending is not null && !ReferenceEquals(_lTenurePending, pending))
            {
                return;
            }

            LTenureStop();
            if (_lTenureEnded || _lTenureFault is not null || _lTenureQueue.Count == 0)
            {
                return;
            }

            taken = [.. _lTenureQueue];
            _lTenureQueue.Clear();
        }

        LTenureApply(taken);
    }

    private void LTenureStop()
    {
        CancellationTokenSource? pending = _lTenurePending;
        _lTenurePending = null;

        if (pending is null)
        {
            return;
        }

        pending.Cancel();
        pending.Dispose();
    }

    private void LTenureApply(IReadOnlyList<LRequest> requests)
    {
        bool refused = false;
        foreach (LRequest request in requests)
        {
            lock (_lTenureGate)
            {
                if (_lTenureEnded || _lTenureFault is not null)
                {
                    return;
                }
            }

            try
            {
                _lEngine.LEngineRequestApply(request);
            }
            catch (LRefusal refusal) when (refusal.LRefusalReason != LRefusal.LRefusalDraft)
            {
                refused = true;
            }
            catch (Exception exception)
            {
                LTenureSuspend(exception);
                return;
            }
        }

        if (refused)
        {
            _lEngine.LEngineBulletinRaise(LSubject.LSubjectDraft, LTenureId);
        }

        LTenureStateRaise();
    }

    private void LTenureSuspend(Exception exception)
    {
        lock (_lTenureGate)
        {
            if (_lTenureFault is not null)
            {
                return;
            }

            _lTenureFault = exception;
            LTenureStop();
            _lTenureQueue.Clear();
        }

        LTenureStateRaise();
    }

    private void LTenureStateRaise()
    {
        LTenureState state = LTenureStateRead();
        lock (_lTenureGate)
        {
            if (state == _lTenureLast)
            {
                return;
            }

            _lTenureLast = state;
        }

        _lEngine.LEngineBulletinRaise(LSubject.LSubjectTenure, LTenureId);
    }
}
