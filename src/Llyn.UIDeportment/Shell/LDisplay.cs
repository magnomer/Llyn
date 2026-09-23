using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LDisplay
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPhonologyPort _lPhonologyPort;

    private readonly LSettingsPort _lSettingsPort;

    private LVista? _lDisplayVista;

    private bool _lDisplayOpened;

    public LDisplay(LEntryPort entries, LPhonologyPort phonology, LSettingsPort settings)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(settings);

        _lEntryPort = entries;
        _lPhonologyPort = phonology;
        _lSettingsPort = settings;
    }

    public bool LDisplayFoldOpened => _lDisplayOpened;

    public long? LDisplayChosen => _lDisplayVista?.LVistaChosen;

    public void LDisplayVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lDisplayVista = vista;
    }

    public void LDisplayChosenAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lDisplayVista?.LVistaChosenAttach(subject, observer);
    }

    public void LDisplayObserverAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lDisplayVista?.LVistaObserverAttach(subject, observer);
    }

    public LEntryDraft? LDisplayDraftLoad()
    {
        return _lDisplayVista?.LVistaLoad()?.LDraftContent;
    }

    public void LDisplayFoldSet(bool opened)
    {
        _lDisplayOpened = opened;
    }

    public bool LDisplayFanqieCheck(long? id)
    {
        return LDisplayPendingRead(_lPhonologyPort.LEngineFanqieCheck, id);
    }

    public bool LDisplayScriptCheck(long? id)
    {
        return LDisplayPendingRead(_lPhonologyPort.LEngineScriptCheck, id);
    }

    public bool LDisplayParadigmCheck(long? id)
    {
        return LDisplayPendingRead(_lPhonologyPort.LEngineInflectionCheck, id);
    }

    public LEntry? LDisplayEntryRead(long id)
    {
        return _lEntryPort.LEngineEntryRead(id);
    }

    public LEntryDraft? LDisplayEntryLoad(long id)
    {
        return _lEntryPort.LEngineEntryLoad(id);
    }

    public bool LDisplayFavoriteCheck(long id)
    {
        return _lEntryPort.LEngineFavoriteCheck(id);
    }

    public void LDisplayFavoriteSave(long id)
    {
        _lEntryPort.LEngineFavoriteSave(id);
    }

    public void LDisplayFavoriteDelete(long id)
    {
        _lEntryPort.LEngineFavoriteDelete(id);
    }

    public int LDisplayGraspStep => _lEntryPort.LEngineGraspStep;

    public IReadOnlyList<string> LDisplayNameResolve(IReadOnlyList<string> labels)
    {
        return _lEntryPort.LEngineNameResolve(labels);
    }

    public string LDisplayGraspFormat(int step)
    {
        return LDisplayChosen is null
            ? string.Empty
            : _lEntryPort.LEngineGraspFormat(step);
    }

    public int LDisplayGraspRead(long id)
    {
        return _lEntryPort.LEngineGraspRead(id);
    }

    public void LDisplayGraspSave(long id, int grasp)
    {
        _lEntryPort.LEngineGraspSave(id, grasp);
    }

    public IReadOnlyList<LFrequency> LDisplayFrequencyRead(long id)
    {
        return _lEntryPort.LEngineFrequencyRead(id);
    }

    public string LDisplayEpithetRead(long id)
    {
        return _lEntryPort.LEngineEpithetRead(id);
    }

    public IReadOnlyList<LUsage> LDisplayIncomingRead(long id)
    {
        return _lEntryPort.LEngineIncomingRead(id);
    }

    public IReadOnlyList<LTranslationTarget> LDisplayTargetRead(LEntryDraft draft)
    {
        return _lEntryPort.LEngineTargetRead(draft);
    }

    public IReadOnlyList<LTranslationTarget> LDisplayEtymonRead(LEntryDraft draft)
    {
        try
        {
            return _lEntryPort.LEngineEtymonRead(draft);
        }
        catch (Exception)
        {
            return [];
        }
    }

    public static bool LDisplayNarrativeCheck(bool editable, string text)
    {
        return !editable && text.Trim().Length > 0;
    }

    public static bool LDisplayEtymonCheck(bool editable, int count)
    {
        return editable || count > 0;
    }

    public static bool LDisplayEtymologyCheck(string text, int count)
    {
        return text.Trim().Length > 0 || count > 0;
    }

    public IReadOnlyDictionary<long, string> LDisplayCitationRead()
    {
        return _lEntryPort.LEngineCitationRead();
    }

    public LGlyph? LDisplayGlyphRead(string language)
    {
        return _lEntryPort.LEngineGlyphRead(language);
    }

    public LMentionResult LDisplayMentionFind(
        string text, string language, int offset, IReadOnlyList<LMention> mentions)
    {
        return _lEntryPort.LEngineMentionFind(text, language, offset, mentions);
    }

    public bool LDisplayTonalCheck(string language)
    {
        return _lPhonologyPort.LEngineTonalCheck(language);
    }

    public bool LDisplayFlaggedCheck(LEntryDraft draft)
    {
        return _lPhonologyPort.LEngineFlaggedCheck(draft);
    }

    public void LDisplayFanqieStart(long id)
    {
        _lPhonologyPort.LEngineFanqieStart(id);
    }

    public IReadOnlyList<LFanqieGroup> LDisplayFanqieDivide(long id)
    {
        return _lPhonologyPort.LEngineFanqieDivide(id);
    }

    public string LDisplayReadingRead(long id, string headword)
    {
        return LFanqieGroup.LFanqieReadingFormat(_lPhonologyPort.LEngineFanqieDivide(id), headword);
    }

    public void LDisplayFanqieSet(long id, long fanqieId, int rank)
    {
        _lPhonologyPort.LEngineFanqieSet(id, fanqieId, rank);
    }

    public void LDisplayScriptStart(long id)
    {
        _lPhonologyPort.LEngineScriptStart(id);
    }

    public IReadOnlyList<LScriptGroup> LDisplayScriptDivide(long id)
    {
        return _lPhonologyPort.LEngineScriptDivide(id);
    }

    public void LDisplayReflexStart(long id)
    {
        _lPhonologyPort.LEngineReflexStart(id);
    }

    public bool LDisplayReflexCheck(long id)
    {
        return _lPhonologyPort.LEngineReflexCheck(id);
    }

    public void LDisplayReflexRebuild(long id)
    {
        _lPhonologyPort.LEngineReflexRebuild(id);
    }

    public IReadOnlyList<LParadigmSlot> LDisplayParadigmShow(long id)
    {
        return _lPhonologyPort.LEngineParadigmShow(id);
    }

    public void LDisplayInflectionStart(long id)
    {
        _lPhonologyPort.LEngineInflectionStart(id);
    }

    public bool LDisplayInflectionCheck(long id)
    {
        return _lPhonologyPort.LEngineInflectionCheck(id);
    }

    public bool LDisplayMorphologyRead()
    {
        return _lSettingsPort.LEngineSettingsRead().LSettingsMorphology;
    }

    private static bool LDisplayPendingRead(Func<long, bool> check, long? id)
    {
        if (id is not long shown)
        {
            return false;
        }

        try
        {
            return check(shown);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
