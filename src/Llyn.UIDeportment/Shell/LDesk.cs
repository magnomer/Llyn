using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Conduct;
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

    private LForay? _lDeskRecording;

    private LForay? _lDeskTranscription;

    private bool _lDeskFilling;

    private bool _lDeskHalted;

    internal LDesk(LDraftPort drafts, string scope, Func<bool> unreadableSeam)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentException.ThrowIfNullOrWhiteSpace(scope);
        ArgumentNullException.ThrowIfNull(unreadableSeam);

        _lDraftPort = drafts;
        _lDeskScope = scope;
        _lDeskUnreadableSeam = unreadableSeam;
        LDeskQuill = new QQuill(this);
        LDeskEasel = new QEasel(this);
    }

    public event Action? LDeskStarted;

    internal event Action<LDraft>? LDeskDraftPrepared;

    public event Action<CDraft>? LDeskDraftChanged;

    public event Action? LDeskStateChanged;

    public event Action<long>? LDeskFinished;

    public event Action<string, Exception>? LDeskFailed;

    public event Action<string>? LDeskRefused;

    public QQuill LDeskQuill { get; }

    public QEasel LDeskEasel { get; }

    public bool LDeskHeld => _lDeskTenure is not null;

    public bool LDeskFilling => _lDeskFilling;

    public long LDeskId => _lDeskTenure?.LTenureId ?? 0;

    public bool LDeskStored => LDeskRead()?.LDraftStored is not null;

    public bool LDeskChanged => _lDeskTenure?.LTenureStateRead() is { LTenureStateChanged: true };

    public bool LDeskStorable =>
        _lDeskTenure?.LTenureStateRead() is { LTenureStateChanged: true, LTenureStateRefusal: null };

    public bool LDeskHalted => _lDeskTenure?.LTenureStateRead() is { LTenureStateHalted: true };

    public bool LDeskRunning => LDeskHeld && !LDeskHalted;

    private bool LDeskStalling => LDeskHalted && !_lDeskHalted;

    internal LTenure? LDeskTenure => _lDeskTenure;

    internal LDraft? LDeskDraft => _lDeskTenure is LTenure held ? held.LTenureRead() : null;

    internal LForay? LDeskRecording => _lDeskRecording;

    internal LForay? LDeskTranscription => _lDeskTranscription;

    internal void LDeskVistaRestore(LVista vista)
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

    public void LDeskStart(string origin, CSubject subject, long? id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(origin);

        LDeskCancel();
        LDeskStartRun(() => _lDraftPort.LEngineTenureStart(origin, LPanel.LPanelSubjectRead(subject), id));
    }

    private void LDeskStartRun(Func<LTenure> start)
    {
        try
        {
            LTenure started = start();
            _lDeskTenure = started;
            _lDeskHalted = false;
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

    public void LDeskObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        LSubject held = LPanel.LPanelSubjectRead(subject);
        _lDeskTenureObservers.Add((held, LDeskBulletinSend));
        _lDeskTenure?.LTenureObserverAttach(held, LDeskBulletinSend);

        void LDeskBulletinSend(LBulletin bulletin)
        {
            observer(new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored));
        }
    }

    public void LDeskDraftAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        LSubject held = LPanel.LPanelSubjectRead(subject);
        _lDeskDraftObservers.Add((held, LDeskBulletinSend));
        _lDeskTenure?.LTenureDraftAttach(held, LDeskBulletinSend);

        void LDeskBulletinSend(LBulletin bulletin)
        {
            observer(new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored));
        }
    }

    public void LDeskEntryAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        LSubject held = LPanel.LPanelSubjectRead(subject);
        _lDeskEntryObservers.Add((held, LDeskBulletinSend));
        _lDeskTenure?.LTenureEntryAttach(held, LDeskBulletinSend);

        void LDeskBulletinSend(LBulletin bulletin)
        {
            observer(new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored));
        }
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

    public bool LDeskRecordingStart(string word, long target, Action<CHarvestStep> sink)
    {
        ArgumentNullException.ThrowIfNull(sink);

        LDeskForayStop(_lDeskRecording);
        _lDeskRecording = LDeskRecordingRun(_lDeskTenure, word, target, LDeskHarvestSend);
        return _lDeskRecording is not null;

        void LDeskHarvestSend(LHarvestStep step)
        {
            sink(new CHarvestStep(
                step.LHarvestStepSource, step.LHarvestStepOrder,
                LDeskRecordingRead(step.LHarvestStepRecording), step.LHarvestStepEnded));
        }
    }

    public bool LDeskTranscriptionStart(string word, long target, string scheme, Action<CLookupStep> sink)
    {
        ArgumentNullException.ThrowIfNull(sink);

        LDeskForayStop(_lDeskTranscription);
        _lDeskTranscription = LDeskTranscriptionRun(_lDeskTenure, word, target, scheme, LDeskLookupSend);
        return _lDeskTranscription is not null;

        void LDeskLookupSend(LLookupStep step)
        {
            sink(new CLookupStep(
                step.LLookupStepSource, step.LLookupStepOrder,
                LDeskCandidateRead(step.LLookupStepCandidate), step.LLookupStepEnded));
        }
    }

    public void LDeskForayCancel()
    {
        LDeskForayStop(_lDeskRecording);
        _lDeskRecording = null;
        LDeskForayStop(_lDeskTranscription);
        _lDeskTranscription = null;
    }

    private static LForay? LDeskRecordingRun(LTenure? tenure, string word, long target, Action<LHarvestStep> sink)
    {
        return tenure?.LTenureRecordingStart(word, target, sink);
    }

    private static LForay? LDeskTranscriptionRun(
        LTenure? tenure, string word, long target, string scheme, Action<LLookupStep> sink)
    {
        return tenure?.LTenureTranscriptionStart(word, target, scheme, sink);
    }

    private static void LDeskForayStop(LForay? foray)
    {
        foray?.LForayCancel();
    }

    internal static CRecording? LDeskRecordingRead(LRecording? recording)
    {
        return recording is null
            ? null
            : new CRecording(
                recording.LRecordingSource, recording.LRecordingAddress, recording.LRecordingOrder,
                recording.LRecordingReached, recording.LRecordingVariety);
    }

    internal static LRecording LDeskRecordingRead(CRecording recording)
    {
        ArgumentNullException.ThrowIfNull(recording);

        return new LRecording(
            recording.CRecordingSource, recording.CRecordingAddress, recording.CRecordingOrder,
            recording.CRecordingReached, recording.CRecordingVariety);
    }

    internal static CCandidate? LDeskCandidateRead(LCandidate? candidate)
    {
        return candidate is null
            ? null
            : new CCandidate(
                candidate.LCandidateSource, candidate.LCandidatePhonetic, candidate.LCandidateOrder,
                candidate.LCandidateReached, candidate.LCandidateVariety, candidate.LCandidateRespelling);
    }

    internal LDraft? LDeskRead()
    {
        if (_lDeskTenure is not LTenure held)
        {
            return null;
        }

        held.LTenurePersist();
        return held.LTenureRead();
    }

    public long? LDeskStoredRead()
    {
        try
        {
            return LDeskRead()?.LDraftStored;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public CMentionDraft? LDeskMentionFind(long cardId, long sentenceId, string text, int start, int length)
    {
        return LDeskMentionRead(
            _lDeskTenure?.LTenureExampleRead(cardId, sentenceId)
                ?.LExampleDraftFind(_lDraftPort.LEngineSpanRead(text, start, length)));
    }

    private static CMentionDraft? LDeskMentionRead(LMentionDraft? found)
    {
        return found is null ? null : LCard.LCardMentionRead(found);
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
            LDeskDraftPrepared?.Invoke(draft);
            LDeskDraftChanged?.Invoke(new CDraft(draft.LDraftAuthorName));
        }
        finally
        {
            _lDeskFilling = false;
        }
    }

    public void LDeskStateUpdate()
    {
        if (LDeskStalling)
        {
            LDeskRefused?.Invoke(_lDeskScope + ".HoldFailed");
        }

        _lDeskHalted = LDeskHalted;
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
        return LDeskFinish(store, id => LDeskFinished?.Invoke(id));
    }

    public bool LDeskFinish(bool store, Action<long> stored)
    {
        ArgumentNullException.ThrowIfNull(stored);

        if (_lDeskTenure is not LTenure held)
        {
            return true;
        }

        long? kept;
        try
        {
            kept = held.LTenureFinish(store, LDeskUnreadableConfirm);
        }
        catch (Exception exception)
        {
            LDeskFailed?.Invoke(_lDeskScope + ".SaveFailed", exception);
            return false;
        }

        _lDeskTenure = null;
        LDeskStateChanged?.Invoke();
        if (kept is long id)
        {
            stored(id);
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
