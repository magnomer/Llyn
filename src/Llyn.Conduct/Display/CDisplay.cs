using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDisplay
{
    private static readonly CLectern _cDisplayBlank = new(
        string.Empty, string.Empty, [], false, [], false, string.Empty, string.Empty, false);

    private readonly LDisplay _cDisplayRule;

    private readonly LEntryPort _cDisplayPort;

    private readonly LPhonologyPort _cDisplayPhonology;

    private readonly CEnvoy _cDisplayEnvoy;

    private readonly LSettingsPort _cDisplaySettings;

    private CMention? _cDisplayMention;

    internal CDisplay(
        LDisplay display, LEntryPort entries, LPhonologyPort phonology, LSettingsPort settings, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cDisplayRule = display;
        _cDisplayPort = entries;
        _cDisplayPhonology = phonology;
        _cDisplayEnvoy = envoy;
        _cDisplaySettings = settings;
    }

    public event Action? CDisplayOpened;

    internal event Action<string, long>? CDisplayRowChosen;

    public event Action? CDisplayClosed;

    public event Action<CBulletin>? CDisplayFavoriteChanged;

    public event Action<CBulletin>? CDisplayGraspChanged;

    public event Action<CBulletin>? CDisplayFrequencyChanged;

    public event Action<CBulletin>? CDisplayParadigmChanged;

    public event Action<CBulletin>? CDisplayReflexChanged;

    public event Action<CBulletin>? CDisplayScriptChanged;

    public event Action<CBulletin>? CDisplayFanqieChanged;

    public event Action<CBulletin>? CDisplayEntryChanged;

    public event Action<CBulletin>? CDisplayWorkspaceChanged;

    public CLectern CDisplayShown { get; private set; } = _cDisplayBlank;

    public int CDisplayGraspStep => _cDisplayRule.LDisplayGraspStep;

    private long? LDisplayChosen => _cDisplayRule.LDisplayChosen;

    private LEntryDraft? LDisplayShown => _cDisplayRule.LDisplaySound.LDisplayShown;

    private CLedgerNoticed LDisplayNoticed => _cDisplayRule.LDisplayNoticed;

    internal void LDisplayVistaAttach()
    {
        _cDisplayRule.LDisplayChosenAttach(
            CSubject.CSubjectFavorite, bulletin => CDisplayFavoriteChanged?.Invoke(bulletin));
        _cDisplayRule.LDisplayChosenAttach(CSubject.CSubjectGrasp, bulletin => CDisplayGraspChanged?.Invoke(bulletin));
        _cDisplayRule.LDisplayChosenAttach(
            CSubject.CSubjectFrequency, bulletin => CDisplayFrequencyChanged?.Invoke(bulletin));
        _cDisplayRule.LDisplayChosenAttach(
            CSubject.CSubjectInflection, bulletin => CDisplayParadigmChanged?.Invoke(bulletin));
        _cDisplayRule.LDisplayChosenAttach(
            CSubject.CSubjectReflex, bulletin => CDisplayReflexChanged?.Invoke(bulletin));
        _cDisplayRule.LDisplayChosenAttach(CSubject.CSubjectEntry, bulletin => CDisplayEntryChanged?.Invoke(bulletin));
        _cDisplayRule.LDisplayObserverAttach(
            CSubject.CSubjectScript, bulletin => CDisplayScriptChanged?.Invoke(bulletin));
        _cDisplayRule.LDisplayObserverAttach(
            CSubject.CSubjectFanqie, bulletin => CDisplayFanqieChanged?.Invoke(bulletin));
        _cDisplayRule.LDisplayObserverAttach(
            CSubject.CSubjectWorkspace, bulletin => CDisplayWorkspaceChanged?.Invoke(bulletin));
        foreach (CSubject subject in (CSubject[])
                 [
                     CSubject.CSubjectExample,
                     CSubject.CSubjectSituation,
                     CSubject.CSubjectReference,
                     CSubject.CSubjectAuthor,
                     CSubject.CSubjectTag,
                     CSubject.CSubjectRegister,
                     CSubject.CSubjectSettings,
                 ])
        {
            _cDisplayRule.LDisplayObserverAttach(subject, bulletin => CDisplayEntryChanged?.Invoke(bulletin));
        }
    }

    public void CDisplayPanelAttach(CPanel panel)
    {
        ArgumentNullException.ThrowIfNull(panel);

        panel.CPanelDraftChanged += LDisplayDraftOpen;
        panel.CPanelCleared += CDisplayEntryClose;
    }

    private void LDisplayDraftOpen(LDraft draft)
    {
        LDisplayEntryOpen(draft.LDraftContent);
    }

    internal void LDisplayEntryOpen(LEntryDraft? draft)
    {
        if (draft is null || LDisplayChosen is not long id)
        {
            CDisplayEntryClose();
            return;
        }

        _cDisplayRule.LDisplaySound.LDisplaySoundShow(id, draft);
        (bool, string, string) stamp = LDisplayStampRead(id);
        CDisplayShown = new CLectern(
            draft.LEntryDraftHeadword,
            draft.LEntryDraftLanguage,
            draft.LEntryDraftNames,
            draft.LEntryDraftMarked,
            CMarkdown.LMarkdownParse(_cDisplayPort, draft.LEntryDraftNote),
            draft.LEntryDraftNoted,
            stamp.Item2,
            stamp.Item3,
            stamp.Item1);
        CDisplayOpened?.Invoke();
    }

    private (bool, string, string) LDisplayStampRead(long id)
    {
        try
        {
            return _cDisplayPort.LEngineStampRead(id);
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Display.StampFailed", exception);
            return (false, string.Empty, string.Empty);
        }
    }

    public void CDisplayEntryClose()
    {
        _cDisplayRule.LDisplaySound.LDisplaySoundClear();
        CDisplayShown = _cDisplayBlank;
        CDisplayClosed?.Invoke();
    }

    public void CDisplayEntryResonate()
    {
        if (LDisplayChosen is null)
        {
            return;
        }

        _cDisplayRule.LDisplayDraftLoad(LDisplayEntryOpen);
    }

    public void CDisplayWorkspaceResonate()
    {
        CDisplayEntryClose();
    }

    public bool CDisplayFavoriteRead()
    {
        return _cDisplayRule.LDisplayFavoriteRead(LDisplayChosen);
    }

    public bool CDisplayFavoriteToggle(bool marked)
    {
        _cDisplayRule.LDisplayFavoriteSave(LDisplayChosen, marked);
        return CDisplayFavoriteRead();
    }

    public CGrasp CDisplayGraspRead()
    {
        int step = _cDisplayRule.LDisplayGraspRead(LDisplayChosen);
        return new CGrasp(step, CDisplayGraspRead(step));
    }

    public string CDisplayGraspRead(int step)
    {
        return _cDisplayRule.LDisplayGraspFormat(LDisplayChosen, step);
    }

    public CGrasp CDisplayGraspSet(int step)
    {
        _cDisplayRule.LDisplayGraspSave(LDisplayChosen, step);
        return CDisplayGraspRead();
    }

    public CFrequency? CDisplayFrequencyRead(Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(lookup);

        return _cDisplayRule.LDisplayFrequencyRead(LDisplayChosen, lookup("Frequency.Once"));
    }

    public CLecternCard CDisplayCardRead()
    {
        if (LDisplayShown is not LEntryDraft shown)
        {
            return new CLecternCard([], [], false, false);
        }

        LSentenceOrder order = LDisplayOrderRead(shown.LEntryDraftLanguage);
        IReadOnlyDictionary<long, string> citations = LDisplayCitationRead(shown);
        IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> targets = LDisplayTranslationRead(shown);
        string mark = _cDisplaySettings.LEngineTextRead("Display.Unknown");
        LMediaPort media = _cDisplayRule.LDisplayMediaPort;
        return new CLecternCard(
            CLeaf.LLeafRead(shown.LEntryDraftMeanings, order, mark, citations, targets, media),
            CLeaf.LLeafRead(shown.LEntryDraftCollocations, order, mark, citations, targets, media),
            shown.LEntryDraftDefined,
            shown.LEntryDraftCollocated);
    }

    private IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LDisplayTranslationRead(LEntryDraft shown)
    {
        try
        {
            return _cDisplayPort.LEngineTranslationRead(shown);
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(
                _cDisplayEnvoy, _cDisplaySettings, "Display.TranslationFailed", exception);
            return new Dictionary<long, IReadOnlyList<LTranslationTarget>>();
        }
    }

    private LSentenceOrder LDisplayOrderRead(string language)
    {
        try
        {
            return _cDisplayPhonology.LEngineOrderRead(language);
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Display.OrderFailed", exception);
            return LSentenceOrder.LSentenceOrderDefault;
        }
    }

    private IReadOnlyDictionary<long, string> LDisplayCitationRead(LEntryDraft shown)
    {
        try
        {
            return _cDisplayPort.LEngineCitationRead(shown);
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Display.CitationFailed", exception);
            return new Dictionary<long, string>();
        }
    }

    public IReadOnlyList<CUsage> CDisplayIncomingRead()
    {
        if (LDisplayChosen is not long id)
        {
            return [];
        }

        try
        {
            return _cDisplayPort.LEngineIncomingRead(id).Select(COeuvre.COeuvreUsageRead).ToList();
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Display.IncomingFailed", exception);
            return [];
        }
    }

    public CLecternEtymology CDisplayEtymologyRead()
    {
        if (LDisplayShown is not LEntryDraft shown)
        {
            return new CLecternEtymology(string.Empty, [], false, false, false, false);
        }

        LEtymologyResult etymology;
        try
        {
            etymology = _cDisplayPort.LEngineEtymologyRead(shown);
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Display.EtymologyFailed", exception);
            etymology = new LEtymologyResult([], shown.LEntryDraftEtymology.LEtymologyDraftNarrated);
        }

        return new CLecternEtymology(
            shown.LEntryDraftEtymology.LEtymologyDraftText,
            CFolio.CFolioTargetRead(etymology.LEtymologyResultTargets),
            etymology.LEtymologyResultFilled,
            etymology.LEtymologyResultNarrated,
            etymology.LEtymologyResultLinked,
            shown.LEntryDraftDerived);
    }

    public bool CDisplayChipOpen(CLeafChip? chip, long? link)
    {
        if (chip is { CLeafChipStored: true })
        {
            CDisplayRowChosen?.Invoke(LDisplayTabRead(chip.CLeafChipSubject), chip.CLeafChipId);
            return true;
        }

        if (LEntryPort.LEngineLinkRead(link) is not long id)
        {
            return false;
        }

        CDisplayRowChosen?.Invoke("Library", id);
        return true;
    }

    private static string LDisplayTabRead(CSubject subject)
    {
        return subject switch
        {
            CSubject.CSubjectSituation => "Repertoire",
            CSubject.CSubjectRegister => "Tenor",
            CSubject.CSubjectTag => "Taxonomy",
            _ => throw new ArgumentOutOfRangeException(nameof(subject), subject, null),
        };
    }

    internal void LDisplayMentionAttach(CMention mention)
    {
        ArgumentNullException.ThrowIfNull(mention);

        _cDisplayMention = mention;
    }

    public CMentionOffer? CDisplayMentionFind(long sentence, int offset)
    {
        return LDisplayMentionOpen(shown => _cDisplayPort.LEngineMentionFind(shown, sentence, offset));
    }

    public CMentionOffer? CDisplayEtymologyFind(int offset)
    {
        return LDisplayMentionOpen(shown => _cDisplayPort.LEngineEtymologyFind(shown, offset));
    }

    private CMentionOffer? LDisplayMentionOpen(Func<LEntryDraft, LMentionResult> find)
    {
        if (_cDisplayMention is not CMention mention || LDisplayShown is not LEntryDraft shown)
        {
            return null;
        }

        try
        {
            return mention.LMentionResultOpen(CMention.CMentionResultRead(find(shown)));
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cDisplayEnvoy, _cDisplaySettings, "Mention.FindFailed", exception);
            return null;
        }
    }

    public (CCompassPart, int)? CDisplayCardFind(long id)
    {
        if (LDisplayShown is not LEntryDraft shown
            || LEntryPort.LEngineCardFind(shown, id) is not (LOwner owner, int index))
        {
            return null;
        }

        return owner switch
        {
            LOwner.LOwnerMeaning => (CCompassPart.CCompassPartMeaning, index),
            LOwner.LOwnerCollocation => (CCompassPart.CCompassPartCollocation, index),
            _ => throw new ArgumentOutOfRangeException(nameof(id), owner, null),
        };
    }
}
