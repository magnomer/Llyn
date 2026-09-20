using System;
using System.Collections.Generic;
using Llyn.Application;
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

    public LEditor(
        LDraftPort drafts,
        LEntryPort entries,
        LPhonologyPort phonology,
        LSettingsPort settings,
        Func<bool> unreadableSeam)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(settings);

        _lDraftPort = drafts;
        _lEntryPort = entries;
        _lPhonologyPort = phonology;
        _lSettingsPort = settings;
        LEditorDesk = new LDesk(drafts, "Input", unreadableSeam);
        LEditorDisplay = new LDisplay(entries, phonology, settings);
        LEditorCard = new LCard(drafts, entries, phonology);
        LEditorClip = new LClip();
        LEditorNotation = new LNotation();
        LEditorDesk.LDeskStateChanged += LEditorStateUpdate;
        LEditorDesk.LDeskFinished += LEditorStoredShow;
    }

    public event Action? LEditorStateChanged;

    public event Action? LEditorStopped;

    public event Action? LEditorFavoriteChanged;

    public event Action? LEditorGraspChanged;

    public event Action? LEditorFanqieChanged;

    public event Action<string, Exception>? LEditorFailed;

    public LDesk LEditorDesk { get; }

    public LDisplay LEditorDisplay { get; }

    public LCard LEditorCard { get; }

    public LClip LEditorClip { get; }

    public LNotation LEditorNotation { get; }

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

    public bool LEditorRebuildable => LEditorEntry is not null && _lPhonologyPort.LEngineBookCheck(LEditorLanguage);

    public bool LEditorMorphology => _lSettingsPort.LEngineSettingsRead().LSettingsMorphology;

    public bool LEditorFavorite => LEditorEntry is long id && LEditorFavoriteRead(id);

    public int LEditorGrasp => LEditorEntry is long id ? LEditorGraspRead(id) : 0;

    public bool LEditorFanqiePending => LEditorDisplay.LDisplayFanqieCheck(LEditorEntry);

    public bool LEditorScriptPending => LEditorDisplay.LDisplayScriptCheck(LEditorEntry);

    public bool LEditorParadigmPending => LEditorDisplay.LDisplayParadigmCheck(LEditorEntry);

    public string LEditorParadigmLanguage => LParadigm.LParadigmLanguageRead(LEditorParadigmRead());

    private bool LEditorRestarting => LEditorFresh && LEditorOwned;

    private bool LEditorStalling => LEditorDesk.LDeskHalted && !LEditorHalted;

    public void LEditorVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lEditorVista = vista;
        LEditorDesk.LDeskVistaRestore(vista);
        LEditorDisplay.LDisplayVistaRestore(vista);
    }

    public void LEditorVistaRestore(LPosture posture)
    {
        ArgumentNullException.ThrowIfNull(posture);

        LEditorVistaRestore(
            posture.LPostureVistaStart("input", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
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

    public void LEditorClipStart(string word, long target, LListener listener)
    {
        LEditorClip.LClipForaySet(LEditorDesk.LDeskRecordingStart(word, target, listener));
    }

    public void LEditorNotationStart(string word, long target, string scheme, LReceiver receiver)
    {
        LEditorNotation.LNotationForaySet(LEditorDesk.LDeskTranscriptionStart(word, target, scheme, receiver));
    }

    public LEntryDraft? LEditorDraftRead()
    {
        return LEditorDesk.LDeskTenure?.LTenureRead()?.LDraftContent;
    }

    public string LEditorPronunciationRead()
    {
        return LEditorDraftRead()?.LEntryDraftPronunciation?.LPronunciationDraftRead(LEditorRespelled)
            ?? string.Empty;
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
            : LLocalization.LLocalizationTextRead(LGrasp.LGraspKeyRead(step));
    }

    public IReadOnlyList<LFrequency> LEditorFrequencyRead()
    {
        if (LEditorEntry is not long id)
        {
            return [];
        }

        try
        {
            return _lEntryPort.LEngineFrequencyRead(id);
        }
        catch (Exception)
        {
            return [];
        }
    }

    public IReadOnlyDictionary<long, LTranslationTarget> LEditorTargetRead()
    {
        try
        {
            return _lDraftPort.LEngineTargetFind(LEditorDesk.LDeskId);
        }
        catch (Exception)
        {
            return new Dictionary<long, LTranslationTarget>();
        }
    }

    public IReadOnlyList<LFanqieGroup> LEditorFanqieRead()
    {
        if (LEditorEntry is not long id)
        {
            return [];
        }

        try
        {
            _lPhonologyPort.LEngineFanqieStart(id);
            return _lPhonologyPort.LEngineFanqieDivide(id);
        }
        catch (Exception)
        {
            return [];
        }
    }

    public IReadOnlyList<LFanqieRow> LEditorAnchorRead()
    {
        List<LFanqieRow> rows = [];
        foreach (LFanqieGroup group in LEditorFanqieRead())
        {
            rows.AddRange(group.LFanqieGroupRows);
        }

        return rows;
    }

    public void LEditorFanqieRebuild()
    {
        if (LEditorEntry is not long id)
        {
            return;
        }

        try
        {
            _lPhonologyPort.LEngineFanqieRebuild(id);
        }
        catch (Exception exception)
        {
            LEditorFailed?.Invoke("Display.FanqieRebuildFailed", exception);
            return;
        }

        LEditorFanqieChanged?.Invoke();
    }

    public IReadOnlyList<LScriptGroup> LEditorScriptRead()
    {
        if (LEditorEntry is not long id)
        {
            return [];
        }

        try
        {
            _lPhonologyPort.LEngineScriptStart(id);
            return _lPhonologyPort.LEngineScriptDivide(id);
        }
        catch (Exception)
        {
            return [];
        }
    }

    public IReadOnlyList<LParadigmSlot> LEditorParadigmRead()
    {
        if (LEditorEntry is not long id)
        {
            return [];
        }

        try
        {
            return _lPhonologyPort.LEngineParadigmShow(id);
        }
        catch (Exception)
        {
            return [];
        }
    }
}
