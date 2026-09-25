using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LLectern
{
    private readonly LDisplay _lLecternDisplay;

    public LLectern(LDisplay display)
    {
        ArgumentNullException.ThrowIfNull(display);

        _lLecternDisplay = display;
    }

    public bool LLecternFoldOpened => _lLecternDisplay.LDisplayFoldOpened;

    public long? LLecternChosen => _lLecternDisplay.LDisplayChosen;

    public int LLecternGraspStep => _lLecternDisplay.LDisplayGraspStep;

    public void LLecternVistaRestore(LVista vista) => _lLecternDisplay.LDisplayVistaRestore(vista);

    public void LLecternChosenAttach(LSubject subject, Action<LBulletin> observer) =>
        _lLecternDisplay.LDisplayChosenAttach(subject, observer);

    public void LLecternObserverAttach(LSubject subject, Action<LBulletin> observer) =>
        _lLecternDisplay.LDisplayObserverAttach(subject, observer);

    public LEntryDraft? LLecternDraftLoad() => _lLecternDisplay.LDisplayDraftLoad();

    public void LLecternFoldSet(bool opened) => _lLecternDisplay.LDisplayFoldSet(opened);

    public bool LLecternFanqieCheck(long? id) => _lLecternDisplay.LDisplayFanqieCheck(id);

    public bool LLecternScriptCheck(long? id) => _lLecternDisplay.LDisplayScriptCheck(id);

    public bool LLecternParadigmCheck(long? id) => _lLecternDisplay.LDisplayParadigmCheck(id);

    public LEntry? LLecternEntryRead(long id) => _lLecternDisplay.LDisplayEntryRead(id);

    public LEntryDraft? LLecternEntryLoad(long id) => _lLecternDisplay.LDisplayEntryLoad(id);

    public bool LLecternFavoriteCheck(long id) => _lLecternDisplay.LDisplayFavoriteCheck(id);

    public void LLecternFavoriteSave(long id) => _lLecternDisplay.LDisplayFavoriteSave(id);

    public void LLecternFavoriteDelete(long id) => _lLecternDisplay.LDisplayFavoriteDelete(id);

    public IReadOnlyList<string> LLecternNameResolve(IReadOnlyList<string> labels) =>
        _lLecternDisplay.LDisplayNameResolve(labels);

    public string LLecternGraspFormat(int step) => _lLecternDisplay.LDisplayGraspFormat(step);

    public int LLecternGraspRead(long id) => _lLecternDisplay.LDisplayGraspRead(id);

    public void LLecternGraspSave(long id, int grasp) => _lLecternDisplay.LDisplayGraspSave(id, grasp);

    public IReadOnlyList<LFrequency> LLecternFrequencyRead(long id) => _lLecternDisplay.LDisplayFrequencyRead(id);

    public string LLecternEpithetRead(long id) => _lLecternDisplay.LDisplayEpithetRead(id);

    public IReadOnlyList<LUsage> LLecternIncomingRead(long id) => _lLecternDisplay.LDisplayIncomingRead(id);

    public IReadOnlyList<LTranslationTarget> LLecternTargetRead(LEntryDraft draft) =>
        _lLecternDisplay.LDisplayTargetRead(draft);

    public IReadOnlyList<LTranslationTarget> LLecternEtymonRead(LEntryDraft draft) =>
        _lLecternDisplay.LDisplayEtymonRead(draft);

    public static bool LLecternNarrativeCheck(bool editable, string text) =>
        LDisplay.LDisplayNarrativeCheck(editable, text);

    public static bool LLecternEtymonCheck(bool editable, int count) => LDisplay.LDisplayEtymonCheck(editable, count);

    public static bool LLecternEtymologyCheck(string text, int count) => LDisplay.LDisplayEtymologyCheck(text, count);

    public IReadOnlyDictionary<long, string> LLecternCitationRead() => _lLecternDisplay.LDisplayCitationRead();

    public LGlyph? LLecternGlyphRead(string language) => _lLecternDisplay.LDisplayGlyphRead(language);

    public LMentionResult LLecternMentionFind(
        string text, string language, int offset, IReadOnlyList<LMention> mentions) =>
        _lLecternDisplay.LDisplayMentionFind(text, language, offset, mentions);

    public bool LLecternTonalCheck(string language) => _lLecternDisplay.LDisplayTonalCheck(language);

    public bool LLecternFlaggedCheck(LEntryDraft draft) => _lLecternDisplay.LDisplayFlaggedCheck(draft);

    public void LLecternFanqieStart(long id) => _lLecternDisplay.LDisplayFanqieStart(id);

    public IReadOnlyList<LFanqieGroup> LLecternFanqieDivide(long id) => _lLecternDisplay.LDisplayFanqieDivide(id);

    public IReadOnlyList<LFanqieRow> LLecternAnchorRead(long id) => _lLecternDisplay.LDisplayAnchorRead(id);

    public string LLecternReadingRead(long id, string headword) =>
        _lLecternDisplay.LDisplayReadingRead(id, headword);

    public void LLecternFanqieSet(long id, long fanqieId, int rank) =>
        _lLecternDisplay.LDisplayFanqieSet(id, fanqieId, rank);

    public void LLecternScriptStart(long id) => _lLecternDisplay.LDisplayScriptStart(id);

    public IReadOnlyList<LScriptGroup> LLecternScriptDivide(long id) => _lLecternDisplay.LDisplayScriptDivide(id);

    public void LLecternReflexStart(long id) => _lLecternDisplay.LDisplayReflexStart(id);

    public bool LLecternReflexCheck(long id) => _lLecternDisplay.LDisplayReflexCheck(id);

    public void LLecternReflexRebuild(long id) => _lLecternDisplay.LDisplayReflexRebuild(id);

    public IReadOnlyList<LParadigmSlot> LLecternParadigmShow(long id) => _lLecternDisplay.LDisplayParadigmShow(id);

    public void LLecternInflectionStart(long id) => _lLecternDisplay.LDisplayInflectionStart(id);

    public bool LLecternInflectionCheck(long id) => _lLecternDisplay.LDisplayInflectionCheck(id);

    public bool LLecternMorphologyRead() => _lLecternDisplay.LDisplayMorphologyRead();
}
