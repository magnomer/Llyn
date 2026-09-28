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
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(media);

        _lDraftPort = drafts;
        _lSettingsPort = settings;
        LEditorDesk = new CDesk(drafts, "Input", envoy);
        LEditorDisplay = new LDisplay(drafts, entries, phonology, settings, media);
        LEditorCard = new CCard(LEditorDesk, drafts, entries);
        LEditorSentence = new CSentence(LEditorDesk, phonology);
        LEditorClip = new LClip(LEditorDesk);
        LEditorNotation = new LNotation(LEditorDesk);
        LEditorSounding = new LSounding(phonology, drafts);
        LEditorSounding.LSoundingFailed += LEditorFailureShow;
        LEditorDisplay.LDisplayFailed += LEditorFailureShow;
        LEditorEsteem = new QEsteem(LEditorDesk, LEditorDisplay);
        LEditorTimbre = new QTimbre(this, phonology, LEditorDisplay, LEditorSounding);
        LEditorDesk.CDeskStateChanged += LEditorStateUpdate;
        LEditorDesk.CDeskFinished += LEditorStoredShow;
        LEditorDesk.CDeskDraftPrepared += LEditorDraftShow;
    }

    public event Action? LEditorStateChanged;

    public event Action<CEntryDraft>? LEditorDraftChanged;

    public event Action? LEditorStopped;

    public event Action<string, Exception>? LEditorFailed;

    public CDesk LEditorDesk { get; }

    public LDisplay LEditorDisplay { get; }

    public CCard LEditorCard { get; }

    public CSentence LEditorSentence { get; }

    public LClip LEditorClip { get; }

    public LNotation LEditorNotation { get; }

    public LSounding LEditorSounding { get; }

    public QEsteem LEditorEsteem { get; }

    public QTimbre LEditorTimbre { get; }

    public bool LEditorOwned => _lEditorVista?.LVistaInput ?? false;

    public string LEditorOrigin => _lEditorVista?.LVistaTab ?? string.Empty;

    public bool LEditorHeld => LEditorDesk.CDeskHeld;

    public bool LEditorChanged => LEditorDesk.CDeskChanged;

    public bool LEditorStorable => LEditorDesk.CDeskStorable;

    public bool LEditorRunning => LEditorHeld && !LEditorDesk.CDeskHalted;

    public bool LEditorFresh => _lEditorFresh;

    public bool LEditorHalted => _lEditorHalted;

    public long? LEditorEntry => LEditorDesk.CDeskStoredRead();

    public string LEditorLanguage => LEditorDesk.CDeskTenure?.LTenureLanguageRead() ?? string.Empty;

    public bool LEditorFlagged => LEditorDesk.CDeskTenure?.LTenureFlaggedCheck() ?? false;

    public bool LEditorReflexShown => LEditorDesk.CDeskTenure?.LTenureReflexCheck() ?? false;

    public IReadOnlyList<string> LEditorVarietyNames => LEditorDesk.CDeskTenure?.LTenureVarietyNames ?? [];

    public bool LEditorMorphology => _lSettingsPort.LEngineSettingsRead().LSettingsMorphology;

    private LEntryDraft? LEditorContent => LEditorDesk.CDeskDraft?.LDraftContent;

    internal LTenure? LEditorTenure => LEditorDesk.CDeskFilling ? null : LEditorDesk.CDeskTenure;

    private bool LEditorRestarting => LEditorFresh && LEditorOwned;

    private bool LEditorStalling => LEditorDesk.CDeskHalted && !LEditorHalted;

    internal void LEditorVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lEditorVista = vista;
        LEditorDesk.CDeskVistaRestore(vista);
        LEditorDisplay.LDisplayVistaRestore(vista);
    }

    internal void LEditorVistaRestore(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        LEditorVistaRestore(
            atelier.CAtelierVistaStart(
                "input", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword));
    }

    public void LEditorOpen(long? id)
    {
        LEditorDesk.CDeskStart(id);
        if (LEditorMissCheck(id))
        {
            LEditorDesk.CDeskStart(null);
        }
    }

    private bool LEditorMissCheck(long? id)
    {
        return id is not null && !LEditorHeld;
    }

    public void LEditorClose()
    {
        LEditorDesk.CDeskCancel();
    }

    public bool LEditorClipStart(string word, long target, Action<CHarvestStep> sink)
    {
        return LEditorDesk.CDeskErrand.CErrandRecordingStart(word, target, sink);
    }

    public bool LEditorNotationStart(string word, long target, string scheme, Action<CLookupStep> sink)
    {
        return LEditorDesk.CDeskErrand.CErrandTranscriptionStart(word, target, scheme, sink);
    }

    private void LEditorDraftShow(LDraft draft)
    {
        LEditorDraftChanged?.Invoke(CFolio.CFolioEntryRead(draft.LDraftContent));
    }

    public CEntryDraft? LEditorDraftRead()
    {
        return LEditorContent is LEntryDraft draft ? CFolio.CFolioEntryRead(draft) : null;
    }

    public string LEditorPronunciationRead()
    {
        return LEditorContent?.LEntryDraftPronunciation?.LPronunciationDraftRead(LEditorTimbre.QTimbreRespelled)
            ?? string.Empty;
    }

    public IReadOnlyList<CTranslationTarget> LEditorEtymonRead()
    {
        return LEditorContent is LEntryDraft draft
            ? CFolio.CFolioTargetRead(LEditorDisplay.LDisplayEtymonRead(draft))
            : [];
    }

    public void LEditorHeadwordSet(string text)
    {
        LEditorTenure?.LTenureHeadwordSet(text);
    }

    public void LEditorPronunciationSet(string text)
    {
        if (LEditorTimbre.QTimbreRespelled)
        {
            LEditorTenure?.LTenureRespellingSet(text);
            return;
        }

        LEditorTenure?.LTenureIpaSet(text);
    }

    public void LEditorNoteSet(string text)
    {
        LEditorTenure?.LTenureNoteSet(text);
    }

    public void LEditorLanguageSet(string language)
    {
        LEditorTenure?.LTenureLanguageSet(language);
    }

    public void LEditorTagAdd(long id)
    {
        LEditorDesk.CDeskSend(new LRequestTagPick(LEditorDesk.CDeskId, 0, id, 0));
    }

    public void LEditorRegisterAdd(long id)
    {
        LEditorDesk.CDeskSend(new LRequestRegisterPick(LEditorDesk.CDeskId, 0, id, 0));
    }

    public void LEditorSituationAdd(long id)
    {
        LEditorDesk.CDeskSend(new LRequestSituationPick(LEditorDesk.CDeskId, 0, id, 0));
    }

    public void LEditorExampleAdd(long id)
    {
        LEditorDesk.CDeskSend(new LRequestSentenceExample(LEditorDesk.CDeskId, 0, 0, id));
    }

    public void LEditorReferenceAdd(long id)
    {
        LEditorDesk.CDeskSend(new LRequestSentenceReference(LEditorDesk.CDeskId, 0, 0, id));
    }

    public void LEditorPersist()
    {
        LEditorDesk.CDeskPersist();
    }

    public void LEditorSave()
    {
        if (!LEditorDesk.CDeskChangeCheck())
        {
            return;
        }

        _lEditorFresh = LEditorEntry is null;
        LEditorDesk.CDeskFinish(true);
    }

    public bool LEditorFinish(bool store)
    {
        _lEditorFresh = LEditorEntry is null;
        return LEditorDesk.CDeskFinish(store);
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

        _lEditorHalted = LEditorDesk.CDeskHalted;
        LEditorStateChanged?.Invoke();
    }

    public IReadOnlyDictionary<long, CTranslationTarget> LEditorTargetRead()
    {
        try
        {
            return _lDraftPort.LEngineTargetFind(LEditorDesk.CDeskId)
                .ToDictionary(pair => pair.Key, pair => CFolio.CFolioTargetRead([pair.Value])[0]);
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
