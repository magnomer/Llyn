using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LEditor
{
    private readonly LDraftPort _lDraftPort;

    private readonly LSettingsPort _lSettingsPort;

    private LVista? _lEditorVista;

    private bool _lEditorFresh;

    private bool _lEditorHalted;

    internal LEditor(
        LDraftPort drafts,
        LEntryPort entries,
        LPhonologyPort phonology,
        LSettingsPort settings,
        LMediaPort media,
        Func<bool> unreadableSeam)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(media);

        _lDraftPort = drafts;
        _lSettingsPort = settings;
        LEditorDesk = new LDesk(drafts, "Input", unreadableSeam);
        LEditorDisplay = new LDisplay(entries, phonology, settings, media);
        LEditorCard = new LCard(LEditorDesk, drafts, entries, phonology);
        LEditorClip = new LClip(LEditorDesk);
        LEditorNotation = new LNotation(LEditorDesk);
        LEditorSounding = new LSounding(phonology, drafts);
        LEditorSounding.LSoundingFailed += LEditorFailureShow;
        LEditorDisplay.LDisplayFailed += LEditorFailureShow;
        LEditorEsteem = new QEsteem(LEditorDesk, LEditorDisplay);
        LEditorTimbre = new QTimbre(this, phonology, LEditorDisplay, LEditorSounding);
        LEditorDesk.LDeskStateChanged += LEditorStateUpdate;
        LEditorDesk.LDeskFinished += LEditorStoredShow;
        LEditorDesk.LDeskDraftPrepared += LEditorDraftShow;
    }

    public event Action? LEditorStateChanged;

    public event Action<CEntryDraft>? LEditorDraftChanged;

    public event Action? LEditorStopped;

    public event Action<string, Exception>? LEditorFailed;

    public LDesk LEditorDesk { get; }

    public LDisplay LEditorDisplay { get; }

    public LCard LEditorCard { get; }

    public LClip LEditorClip { get; }

    public LNotation LEditorNotation { get; }

    public LSounding LEditorSounding { get; }

    public QEsteem LEditorEsteem { get; }

    public QTimbre LEditorTimbre { get; }

    public bool LEditorOwned => _lEditorVista?.LVistaInput ?? false;

    public string LEditorOrigin => _lEditorVista?.LVistaTab ?? string.Empty;

    public bool LEditorHeld => LEditorDesk.LDeskHeld;

    public bool LEditorChanged => LEditorDesk.LDeskChanged;

    public bool LEditorStorable => LEditorDesk.LDeskStorable;

    public bool LEditorRunning => LEditorHeld && !LEditorDesk.LDeskHalted;

    public bool LEditorFresh => _lEditorFresh;

    public bool LEditorHalted => _lEditorHalted;

    public long? LEditorEntry => LEditorDesk.LDeskStoredRead();

    public string LEditorLanguage => LEditorDesk.LDeskTenure?.LTenureLanguageRead() ?? string.Empty;

    public bool LEditorFlagged => LEditorDesk.LDeskTenure?.LTenureFlaggedCheck() ?? false;

    public bool LEditorReflexShown => LEditorDesk.LDeskTenure?.LTenureReflexCheck() ?? false;

    public IReadOnlyList<string> LEditorVarietyNames => LEditorDesk.LDeskTenure?.LTenureVarietyNames ?? [];

    public bool LEditorMorphology => _lSettingsPort.LEngineSettingsRead().LSettingsMorphology;

    private LEntryDraft? LEditorContent => LEditorDesk.LDeskDraft?.LDraftContent;

    private bool LEditorRestarting => LEditorFresh && LEditorOwned;

    private bool LEditorStalling => LEditorDesk.LDeskHalted && !LEditorHalted;

    internal void LEditorVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lEditorVista = vista;
        LEditorDesk.LDeskVistaRestore(vista);
        LEditorDisplay.LDisplayVistaRestore(vista);
    }

    internal void LEditorVistaRestore(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        LEditorVistaRestore(
            window.LWindowAtelier.CAtelierVistaStart(
                "input", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword));
    }

    public void LEditorOpen(long? id)
    {
        LEditorDesk.LDeskStart(id);
        if (LEditorMissCheck(id))
        {
            LEditorDesk.LDeskStart(null);
        }
    }

    private bool LEditorMissCheck(long? id)
    {
        return id is not null && !LEditorHeld;
    }

    public void LEditorClose()
    {
        LEditorDesk.LDeskCancel();
    }

    public bool LEditorClipStart(string word, long target, Action<CHarvestStep> sink)
    {
        return LEditorDesk.LDeskErrand.QErrandRecordingStart(word, target, sink);
    }

    public bool LEditorNotationStart(string word, long target, string scheme, Action<CLookupStep> sink)
    {
        return LEditorDesk.LDeskErrand.QErrandTranscriptionStart(word, target, scheme, sink);
    }

    private void LEditorDraftShow(LDraft draft)
    {
        LEditorDraftChanged?.Invoke(LCard.LCardEntryRead(draft.LDraftContent));
    }

    public CEntryDraft? LEditorDraftRead()
    {
        return LEditorContent is LEntryDraft draft ? LCard.LCardEntryRead(draft) : null;
    }

    public string LEditorPronunciationRead()
    {
        return LEditorContent?.LEntryDraftPronunciation?.LPronunciationDraftRead(LEditorTimbre.QTimbreRespelled)
            ?? string.Empty;
    }

    public IReadOnlyList<CTranslationTarget> LEditorEtymonRead()
    {
        return LEditorContent is LEntryDraft draft
            ? LCard.LCardTargetRead(LEditorDisplay.LDisplayEtymonRead(draft))
            : [];
    }

    public void LEditorHeadwordSet(string text)
    {
        LEditorDesk.LDeskDefer(new LRequestHeadword(LEditorDesk.LDeskId, text));
    }

    public void LEditorPronunciationSet(string text)
    {
        LEditorDesk.LDeskDefer(
            LEditorTimbre.QTimbreRespelled
                ? new LRequestRespelling(LEditorDesk.LDeskId, text)
                : new LRequestIpa(LEditorDesk.LDeskId, text));
    }

    public void LEditorNoteSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        LEditorDesk.LDeskDefer(new LRequestNote(LEditorDesk.LDeskId, text.TrimEnd('\r', '\n')));
    }

    public void LEditorLanguageSet(string language)
    {
        ArgumentNullException.ThrowIfNull(language);

        if (language.Length == 0)
        {
            return;
        }

        LEditorDesk.LDeskSend(new LRequestLanguage(LEditorDesk.LDeskId, language));
    }

    public void LEditorTagAdd(long id)
    {
        LEditorDesk.LDeskSend(new LRequestTagPick(LEditorDesk.LDeskId, 0, id, 0));
    }

    public void LEditorRegisterAdd(long id)
    {
        LEditorDesk.LDeskSend(new LRequestRegisterPick(LEditorDesk.LDeskId, 0, id, 0));
    }

    public void LEditorSituationAdd(long id)
    {
        LEditorDesk.LDeskSend(new LRequestSituationPick(LEditorDesk.LDeskId, 0, id, 0));
    }

    public void LEditorExampleAdd(long id)
    {
        LEditorDesk.LDeskSend(new LRequestSentenceExample(LEditorDesk.LDeskId, 0, 0, id));
    }

    public void LEditorReferenceAdd(long id)
    {
        LEditorDesk.LDeskSend(new LRequestSentenceReference(LEditorDesk.LDeskId, 0, 0, id));
    }

    public void LEditorPersist()
    {
        LEditorDesk.LDeskPersist();
    }

    public void LEditorSave()
    {
        if (!LEditorDesk.LDeskChangeCheck())
        {
            return;
        }

        _lEditorFresh = LEditorEntry is null;
        LEditorDesk.LDeskFinish(true);
    }

    public bool LEditorFinish(bool store)
    {
        _lEditorFresh = LEditorEntry is null;
        return LEditorDesk.LDeskFinish(store);
    }

    private void LEditorStoredShow(long id)
    {
        if (LEditorRestarting)
        {
            LEditorOpen(null);
            return;
        }

        LEditorOpen(id);
    }

    public void LEditorReset()
    {
        LEditorOpen(LEditorEntry);
    }

    private void LEditorStateUpdate()
    {
        if (LEditorStalling)
        {
            LEditorStopped?.Invoke();
        }

        _lEditorHalted = LEditorDesk.LDeskHalted;
        LEditorStateChanged?.Invoke();
    }

    public IReadOnlyDictionary<long, CTranslationTarget> LEditorTargetRead()
    {
        try
        {
            return _lDraftPort.LEngineTargetFind(LEditorDesk.LDeskId)
                .ToDictionary(pair => pair.Key, pair => LCard.LCardTargetRead([pair.Value])[0]);
        }
        catch (Exception)
        {
            return new Dictionary<long, CTranslationTarget>();
        }
    }

    private void LEditorFailureShow(string key, Exception exception)
    {
        LEditorFailed?.Invoke(key, exception);
    }
}
