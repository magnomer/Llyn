using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LDesk
{
    private readonly LDraftPort _lDraftPort;

    private readonly string _lDeskScope;

    private readonly Func<bool> _lDeskUnreadableSeam;

    private LVista? _lDeskVista;

    private LTenure? _lDeskTenure;

    private readonly List<(LSubject LDeskSubject, Action<LBulletin> LDeskObserver)> _lDeskTenureObservers = [];

    private readonly List<(LSubject LDeskSubject, Action<LBulletin> LDeskObserver)> _lDeskDraftObservers = [];

    private readonly List<(LSubject LDeskSubject, Action<LBulletin> LDeskObserver)> _lDeskEntryObservers = [];

    private bool _lDeskFilling;

    public LDesk(LDraftPort drafts, string scope, Func<bool> unreadableSeam)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentException.ThrowIfNullOrWhiteSpace(scope);
        ArgumentNullException.ThrowIfNull(unreadableSeam);

        _lDraftPort = drafts;
        _lDeskScope = scope;
        _lDeskUnreadableSeam = unreadableSeam;
    }

    public event Action? LDeskStarted;

    public event Action<LDraft>? LDeskDraftChanged;

    public event Action? LDeskStateChanged;

    public event Action<long>? LDeskFinished;

    public event Action<string, Exception>? LDeskFailed;

    public bool LDeskHeld => _lDeskTenure is not null;

    public bool LDeskFilling => _lDeskFilling;

    public long LDeskId => _lDeskTenure?.LTenureId ?? 0;

    public bool LDeskStored => LDeskRead()?.LDraftStored is not null;

    public bool LDeskChanged => _lDeskTenure?.LTenureStateRead() is { LTenureStateChanged: true };

    public bool LDeskStorable =>
        _lDeskTenure?.LTenureStateRead() is { LTenureStateChanged: true, LTenureStateRefusal: null };

    public bool LDeskHalted => _lDeskTenure?.LTenureStateRead() is { LTenureStateHalted: true };

    internal LTenure? LDeskTenure => _lDeskTenure;

    public void LDeskVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lDeskVista = vista;
    }

    public void LDeskStart(long? id)
    {
        LDeskCancel();
        if (_lDeskVista is not LVista vista)
        {
            return;
        }

        LDeskStartRun(() => _lDraftPort.LEngineTenureStart(vista, id));
    }

    public void LDeskStart(string origin, LSubject subject, long? id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(origin);

        LDeskCancel();
        LDeskStartRun(() => _lDraftPort.LEngineTenureStart(origin, subject, id));
    }

    private void LDeskStartRun(Func<LTenure> start)
    {
        try
        {
            LTenure started = start();
            _lDeskTenure = started;
            LDeskObserverApply(started);
            LDeskStarted?.Invoke();
        }
        catch (Exception exception)
        {
            LDeskCancel();
            LDeskFailed?.Invoke(_lDeskScope + ".LoadFailed", exception);
            return;
        }

        LDeskDraftUpdate();
        LDeskStateChanged?.Invoke();
    }

    public void LDeskObserverAttach(LSubject subject, Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        _lDeskTenureObservers.Add((subject, observer));
        _lDeskTenure?.LTenureObserverAttach(subject, observer);
    }

    public void LDeskDraftAttach(LSubject subject, Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        _lDeskDraftObservers.Add((subject, observer));
        _lDeskTenure?.LTenureDraftAttach(subject, observer);
    }

    public void LDeskEntryAttach(LSubject subject, Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        _lDeskEntryObservers.Add((subject, observer));
        _lDeskTenure?.LTenureEntryAttach(subject, observer);
    }

    private void LDeskObserverApply(LTenure started)
    {
        foreach ((LSubject subject, Action<LBulletin> observer) in _lDeskTenureObservers)
        {
            started.LTenureObserverAttach(subject, observer);
        }

        foreach ((LSubject subject, Action<LBulletin> observer) in _lDeskDraftObservers)
        {
            started.LTenureDraftAttach(subject, observer);
        }

        foreach ((LSubject subject, Action<LBulletin> observer) in _lDeskEntryObservers)
        {
            started.LTenureEntryAttach(subject, observer);
        }
    }

    public LForay? LDeskRecordingStart(string word, long target, Action<LHarvestStep> sink)
    {
        return _lDeskTenure?.LTenureRecordingStart(word, target, sink);
    }

    public LForay? LDeskTranscriptionStart(string word, long target, string scheme, Action<LLookupStep> sink)
    {
        return _lDeskTenure?.LTenureTranscriptionStart(word, target, scheme, sink);
    }

    public LDraft? LDeskRead()
    {
        if (_lDeskTenure is not LTenure held)
        {
            return null;
        }

        held.LTenurePersist();
        return held.LTenureRead();
    }

    public LMentionDraft? LDeskMentionFind(long cardId, long sentenceId, string text, int start, int length)
    {
        return _lDeskTenure?.LTenureExampleRead(cardId, sentenceId)
            ?.LExampleDraftFind(_lDraftPort.LEngineSpanRead(text, start, length));
    }

    public void LDeskDraftUpdate()
    {
        if (LDeskFilling)
        {
            return;
        }

        try
        {
            LDeskDraftShow(LDeskPrepare());
        }
        catch (Exception exception)
        {
            LDeskFailed?.Invoke(_lDeskScope + ".LoadFailed", exception);
        }
    }

    private LDraft? LDeskPrepare()
    {
        if (_lDeskTenure is not LTenure held)
        {
            return null;
        }

        held.LTenurePersist();
        return held.LTenurePrepare();
    }

    private void LDeskDraftShow(LDraft? draft)
    {
        if (draft is null)
        {
            return;
        }

        _lDeskFilling = true;
        try
        {
            LDeskDraftChanged?.Invoke(draft);
        }
        finally
        {
            _lDeskFilling = false;
        }
    }

    public void LDeskStateUpdate()
    {
        LDeskStateChanged?.Invoke();
    }

    public void LDeskPersist()
    {
        if (LDeskFilling)
        {
            return;
        }

        _lDeskTenure?.LTenurePersist();
    }

    public void LDeskDefer(LRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (LDeskFilling)
        {
            return;
        }

        _lDeskTenure?.LTenureRequestDefer(request);
    }

    public void LDeskSend(LRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (LDeskFilling)
        {
            return;
        }

        _lDeskTenure?.LTenureRequestApply(request);
    }

    public bool LDeskChangeCheck()
    {
        if (_lDeskTenure is not LTenure held)
        {
            return false;
        }

        held.LTenurePersist();
        return held.LTenureStateRead().LTenureStateChanged;
    }

    public (bool LDeskBackward, bool LDeskForward) LDeskChronicleRead()
    {
        if (_lDeskTenure is not LTenure held)
        {
            return (false, false);
        }

        return LDeskChronicleRead(held.LTenureStateRead());
    }

    private static (bool LDeskBackward, bool LDeskForward) LDeskChronicleRead(LTenureState state)
    {
        return (state.LTenureStateBackward, state.LTenureStateForward);
    }

    public void LDeskUndo()
    {
        _lDeskTenure?.LTenureUndo();
        LDeskStateChanged?.Invoke();
    }

    public void LDeskRedo()
    {
        _lDeskTenure?.LTenureRedo();
        LDeskStateChanged?.Invoke();
    }

    public bool LDeskFinish(bool store)
    {
        if (_lDeskTenure is not LTenure held)
        {
            return true;
        }

        long? stored;
        try
        {
            stored = held.LTenureFinish(store, LDeskUnreadableConfirm);
        }
        catch (Exception exception)
        {
            LDeskFailed?.Invoke(_lDeskScope + ".SaveFailed", exception);
            return false;
        }

        _lDeskTenure = null;
        LDeskStateChanged?.Invoke();
        if (stored is long id)
        {
            LDeskFinished?.Invoke(id);
        }

        return true;
    }

    private bool LDeskUnreadableConfirm()
    {
        return _lDeskUnreadableSeam();
    }

    public void LDeskCancel()
    {
        if (_lDeskTenure is not LTenure held)
        {
            return;
        }

        _lDeskTenure = null;
        held.LTenureCancel();
        LDeskStateChanged?.Invoke();
    }
}
