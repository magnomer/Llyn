using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDisplay
{
    private static readonly CLectern _cDisplayBlank = new(
        string.Empty, string.Empty, [], false, string.Empty, false, string.Empty, string.Empty, false);

    private readonly LDisplay _cDisplayRule;

    private readonly LEntryPort _cDisplayPort;

    internal CDisplay(LDisplay display, LEntryPort entries)
    {
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(entries);

        _cDisplayRule = display;
        _cDisplayPort = entries;
    }

    public event Action? CDisplayOpened;

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
            draft.LEntryDraftNote,
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
        catch (Exception)
        {
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

    public void CDisplayPlaybackCancel()
    {
        _cDisplayRule.LDisplaySound.LDisplayPlaybackStop();
    }

    public static bool CDisplayNarrativeCheck(bool editable, string text)
    {
        return !editable && LEntryPort.LEngineNarrativeCheck(text);
    }

    public static bool CDisplayEtymonCheck(bool editable, int count)
    {
        return editable || count > 0;
    }
}
