using System;
using System.Runtime.ExceptionServices;
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

    private readonly LTenureQueue _lTenureQueue;

    internal LTenure(LEngine engine, LSubject subject, long id)
    {
        _lEngine = engine;
        _lTenureSubject = subject;
        LTenureId = id;
        _lTenureQueue = new(engine, id, _lTenureGate, _lTenureTurn, LTenureStateRaise);
        _lTenureLast = LTenureStateRead();
        _lEngine.LEngineObserverAttach(LTenureBulletinHandle);
    }

    public long LTenureId { get; }

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

    public LTenureState LTenureStateRead()
    {
        long revision;
        lock (_lEngine.LEngineGate)
        {
            revision = _lEngine.LEngineRevision;
        }

        bool halted;
        lock (_lTenureGate)
        {
            if (_lTenureQueue.LTenureQueueEnded)
            {
                return LTenureStateEnded;
            }

            halted = _lTenureQueue.LTenureQueueFault is not null;
            if (!halted && _lTenureState is LTenureState kept && _lTenureStateRevision == revision)
            {
                return kept;
            }
        }

        try
        {
            bool changed = _lEngine.LEngineDraft.LEngineDraftCheck(LTenureId, out string? refusal);
            LTenureState state = new(
                changed,
                refusal,
                !halted && _lEngine.LEngineRequest.LEngineUndoCheck(LTenureId),
                !halted && _lEngine.LEngineRequest.LEngineRedoCheck(LTenureId),
                halted);
            lock (_lTenureGate)
            {
                if (!halted && _lTenureQueue.LTenureQueueLive)
                {
                    _lTenureState = state;
                    _lTenureStateRevision = revision;
                }
            }

            return state;
        }
        catch (Exception exception)
        {
            _lTenureQueue.LTenureQueueSuspend(exception);
            return LTenureStateHalted;
        }
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
        return LTenureReadyCheck(LTenureStateRead());
    }

    public bool LTenureChangeCheck()
    {
        LTenurePersist();
        return LTenureStateRead().LTenureStateChanged;
    }

    public bool LTenureStorable =>
        LTenureStateRead() is { LTenureStateChanged: true } state && LTenureReadyCheck(state);

    private static bool LTenureReadyCheck(LTenureState state) => state.LTenureStateRefusal is null;

    public void LTenureHeadwordSet(string text)
    {
        LTenureRequestDefer(new LRequestHeadword(LTenureId, text));
    }

    public void LTenureNoteSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        LTenureRequestDefer(new LRequestNote(LTenureId, LTenureNoteResolve(text)));
    }

    public static bool LTenureNoteCheck(string text, string note)
    {
        ArgumentNullException.ThrowIfNull(text);

        return string.Equals(LTenureNoteResolve(text), note, StringComparison.Ordinal);
    }

    private static string LTenureNoteResolve(string text) => text.TrimEnd('\r', '\n');

    public void LTenureLanguageSet(string language)
    {
        ArgumentNullException.ThrowIfNull(language);

        if (language.Length > 0)
        {
            LTenureRequestApply(new LRequestLanguage(LTenureId, language));
        }
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
        LTenureStateRaise();
    }

    public void LTenureCancel()
    {
        lock (_lTenureGate)
        {
            LTenureForayStop();
            if (!_lTenureQueue.LTenureQueueClose())
            {
                return;
            }
        }

        LTenureObserverClear();
        _lEngine.LEngineDraft.LEngineDraftCancel(LTenureId);
        LTenureStateRaise();
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
                LTenureForayStop();
                _lTenureQueue.LTenureQueueClose();
            }

            LTenureObserverClear();
            LTenureStateRaise();
            return stored;
        }
    }
}
