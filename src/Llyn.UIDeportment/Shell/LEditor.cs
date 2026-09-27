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

    private readonly LEntryPort _lEntryPort;

    private readonly LPhonologyPort _lPhonologyPort;

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
        _lEntryPort = entries;
        _lPhonologyPort = phonology;
        _lSettingsPort = settings;
        LEditorDesk = new LDesk(drafts, "Input", unreadableSeam);
        LEditorDisplay = new LDisplay(entries, phonology, settings, media);
        LEditorCard = new LCard(LEditorDesk, drafts, entries, phonology);
        LEditorClip = new LClip(LEditorDesk);
        LEditorNotation = new LNotation(LEditorDesk);
        LEditorSounding = new LSounding(phonology, drafts);
        LEditorSounding.LSoundingFailed += LEditorFailureShow;
        LEditorDesk.LDeskStateChanged += LEditorStateUpdate;
        LEditorDesk.LDeskFinished += LEditorStoredShow;
        LEditorDesk.LDeskDraftPrepared += LEditorDraftShow;
    }

    public event Action? LEditorStateChanged;

    public event Action<CEntryDraft>? LEditorDraftChanged;

    public event Action? LEditorStopped;

    public event Action? LEditorFavoriteChanged;

    public event Action? LEditorGraspChanged;

    public event Action<string, Exception>? LEditorFailed;

    public LDesk LEditorDesk { get; }

    public LDisplay LEditorDisplay { get; }

    public LCard LEditorCard { get; }

    public LClip LEditorClip { get; }

    public LNotation LEditorNotation { get; }

    public LSounding LEditorSounding { get; }

    public bool LEditorOwned => _lEditorVista?.LVistaInput ?? false;

    public string LEditorOrigin => _lEditorVista?.LVistaTab ?? string.Empty;

    public bool LEditorHeld => LEditorDesk.LDeskHeld;

    public bool LEditorChanged => LEditorDesk.LDeskChanged;

    public bool LEditorStorable => LEditorDesk.LDeskStorable;

    public bool LEditorRunning => LEditorHeld && !LEditorDesk.LDeskHalted;

    public bool LEditorFresh => _lEditorFresh;

    public bool LEditorHalted => _lEditorHalted;

    public long? LEditorEntry
    {
        get
        {
            try
            {
                return LEditorDesk.LDeskRead()?.LDraftStored;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public string LEditorLanguage => LEditorDesk.LDeskTenure?.LTenureLanguageRead() ?? string.Empty;

    public bool LEditorFlagged => LEditorDesk.LDeskTenure?.LTenureFlaggedCheck() ?? false;

    public bool LEditorReflexShown => LEditorDesk.LDeskTenure?.LTenureReflexCheck() ?? false;

    public IReadOnlyList<string> LEditorVarietyNames => LEditorDesk.LDeskTenure?.LTenureVarietyNames ?? [];

    public bool LEditorTonal => _lPhonologyPort.LEngineTonalCheck(LEditorLanguage);

    public bool LEditorSilent => _lPhonologyPort.LEngineSilentCheck(LEditorLanguage);

    public bool LEditorSpoken => !LEditorSilent;

    public bool LEditorRespelled => _lPhonologyPort.LEngineRespellingCheck(LEditorLanguage);

    public bool LEditorPhonemic => LEditorRespelled && _lPhonologyPort.LEnginePhonemicCheck(LEditorLanguage);

    public bool LEditorFanqieRebuildable =>
        LEditorEntry is not null && _lPhonologyPort.LEngineBookCheck(LEditorLanguage);

    public bool LEditorScriptRebuildable =>
        LEditorEntry is not null && _lPhonologyPort.LEngineStyleCheck(LEditorLanguage);

    public bool LEditorMorphology => _lSettingsPort.LEngineSettingsRead().LSettingsMorphology;

    public bool LEditorFavorite => LEditorEntry is long id && LEditorFavoriteRead(id);

    public int LEditorGrasp => LEditorEntry is long id ? LEditorGraspRead(id) : 0;

    public int LEditorGraspStep => _lEntryPort.LEngineGraspStep;

    public bool LEditorFanqiePending => LEditorDisplay.LDisplayFanqieCheck(LEditorEntry);

    public bool LEditorScriptPending => LEditorDisplay.LDisplayScriptCheck(LEditorEntry);

    public bool LEditorParadigmPending => LEditorDisplay.LDisplayParadigmCheck(LEditorEntry);

    public string LEditorParadigmLanguage =>
        LParadigm.LParadigmLanguageRead(LEditorSounding.LSoundingParadigmFind(LEditorEntry));

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
            window.LWindowVistaStart("input", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
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
        return LEditorDesk.LDeskRecordingStart(word, target, sink);
    }

    public bool LEditorNotationStart(string word, long target, string scheme, Action<CLookupStep> sink)
    {
        return LEditorDesk.LDeskTranscriptionStart(word, target, scheme, sink);
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
        return LEditorContent?.LEntryDraftPronunciation?.LPronunciationDraftRead(LEditorRespelled) ?? string.Empty;
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
            LEditorRespelled
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

    public void LEditorFavoriteSet(bool marked)
    {
        if (LEditorEntry is not long id)
        {
            LEditorFavoriteChanged?.Invoke();
            return;
        }

        try
        {
            LEditorFavoriteSave(id, marked);
        }
        catch (Exception exception)
        {
            LEditorFailed?.Invoke("Favorite.MarkFailed", exception);
            LEditorFavoriteChanged?.Invoke();
        }
    }

    private void LEditorFavoriteSave(long id, bool marked)
    {
        if (marked)
        {
            _lEntryPort.LEngineFavoriteSave(id);
            return;
        }

        _lEntryPort.LEngineFavoriteDelete(id);
    }

    private bool LEditorFavoriteRead(long id)
    {
        try
        {
            return _lEntryPort.LEngineFavoriteCheck(id);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void LEditorGraspSet(int step)
    {
        if (LEditorEntry is not long id)
        {
            LEditorGraspChanged?.Invoke();
            return;
        }

        try
        {
            _lEntryPort.LEngineGraspSave(id, step);
        }
        catch (Exception exception)
        {
            LEditorFailed?.Invoke("Grasp.MarkFailed", exception);
            LEditorGraspChanged?.Invoke();
        }
    }

    private int LEditorGraspRead(long id)
    {
        try
        {
            return _lEntryPort.LEngineGraspRead(id);
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public string LEditorGraspFormat(int step)
    {
        return LEditorEntry is null
            ? string.Empty
            : _lEntryPort.LEngineGraspFormat(step);
    }

    public CFrequency? LEditorFrequencyRead(string once)
    {
        if (LEditorEntry is not long id)
        {
            return null;
        }

        try
        {
            return LSounding.LSoundingFrequencyRead(_lEntryPort.LEngineFrequencyRead(id), once);
        }
        catch (Exception)
        {
            return null;
        }
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

    public IReadOnlyList<CFanqieGroup> LEditorFanqieRead()
    {
        return LEditorSounding.LSoundingFanqieRead(LEditorEntry);
    }

    public string LEditorReadingRead(string headword)
    {
        return LEditorSounding.LSoundingReadingRead(LEditorEntry, headword);
    }

    public bool LEditorAnchorCheck(string headword)
    {
        return LEditorSounding.LSoundingAnchorCheck(LEditorSounding.LSoundingAnchorRead(LEditorEntry), headword);
    }

    public string LEditorAnchorFormat(IReadOnlyList<long> anchors, string headword, string separator)
    {
        return LEditorSounding.LSoundingAnchorFormat(
            LEditorSounding.LSoundingAnchorRead(LEditorEntry), anchors, headword, separator);
    }

    public IReadOnlyList<CAnchorRow> LEditorAnchorScan(IReadOnlyList<long> anchors, string reflex, string tone)
    {
        return LEditorSounding.LSoundingAnchorScan(
            LEditorSounding.LSoundingAnchorRead(LEditorEntry), anchors, LEditorLanguage, reflex, tone);
    }

    public void LEditorFanqieRebuild()
    {
        LEditorSounding.LSoundingFanqieRebuild(LEditorEntry);
    }

    public void LEditorFanqieSet(long fanqieId, int rank)
    {
        LEditorSounding.LSoundingFanqieSet(LEditorEntry, fanqieId, rank);
    }

    public IReadOnlyList<CScriptGroup> LEditorScriptRead()
    {
        return LEditorSounding.LSoundingScriptRead(LEditorEntry);
    }

    public void LEditorScriptRebuild()
    {
        LEditorSounding.LSoundingScriptRebuild(LEditorEntry);
    }

    public IReadOnlyList<CParadigmSlot> LEditorParadigmRead()
    {
        return LEditorSounding.LSoundingParadigmRead(LEditorEntry);
    }

    private void LEditorFailureShow(string key, Exception exception)
    {
        LEditorFailed?.Invoke(key, exception);
    }
}
