using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDisplay
{
    private static readonly CLectern _cDisplayBlank = new(
        string.Empty, string.Empty, [], false, [], false, string.Empty, string.Empty, false);

    private readonly LEntryPort _cDisplayPort;

    private readonly CEnvoy _cDisplayEnvoy;

    private readonly LSettingsPort _cDisplaySettings;

    internal CDisplay(
        LDraftPort drafts,
        LEntryPort entries,
        LPhonologyPort phonology,
        LSettingsPort settings,
        LMediaPort media,
        CEnvoy envoy,
        CLedgerNoticed noticed)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        LDisplayRule = new LDisplay(drafts, entries, phonology, settings, media, envoy, noticed);
        _cDisplayPort = entries;
        _cDisplayEnvoy = envoy;
        _cDisplaySettings = settings;
        CDisplaySound = new CDisplaySound(LDisplayRule, this, drafts, entries, phonology, media, settings, envoy);
        CDisplayCard = new CDisplayCard(LDisplayRule, entries, phonology, settings, envoy);
        CDisplayRoute = new CDisplayRoute(LDisplayRule, entries, settings, envoy);
        CDisplayCompass = new CCompass(LDisplayRule, entries, settings, envoy);
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

    public CDisplaySound CDisplaySound { get; }

    public CDisplayCard CDisplayCard { get; }

    public CDisplayRoute CDisplayRoute { get; }

    public CCompass CDisplayCompass { get; }

    internal LDisplay LDisplayRule { get; }

    public int CDisplayGraspStep => LDisplayRule.LDisplayGraspStep;

    private long? LDisplayChosen => LDisplayRule.LDisplayChosen;

    internal void LDisplayNavigationAttach(CNavigation navigation, CMention mention)
    {
        ArgumentNullException.ThrowIfNull(navigation);

        CDisplayRoute.LDisplayMentionAttach(mention);
        CDisplayRoute.CDisplayRowChosen += (tab, id) => navigation.LNavigationRowOpen(tab, id);
        CDisplaySound.CDisplayRowChosen += (tab, id) => navigation.LNavigationRowOpen(tab, id);
        CDisplaySound.CDisplayDiweiChosen +=
            (language, kind, key) => navigation.LNavigationDiweiOpen(language, kind, key);
        CDisplaySound.CDisplayStemChosen += (language, key) => navigation.LNavigationStemOpen(language, key);
    }

    internal void LDisplayVistaRestore(LVista vista)
    {
        LDisplayRule.LDisplayVistaRestore(vista);
        LDisplayRule.LDisplayChosenAttach(
            CSubject.CSubjectFavorite, bulletin => CDisplayFavoriteChanged?.Invoke(bulletin));
        LDisplayRule.LDisplayChosenAttach(CSubject.CSubjectGrasp, bulletin => CDisplayGraspChanged?.Invoke(bulletin));
        LDisplayRule.LDisplayChosenAttach(
            CSubject.CSubjectFrequency, bulletin => CDisplayFrequencyChanged?.Invoke(bulletin));
        LDisplayRule.LDisplayChosenAttach(
            CSubject.CSubjectInflection, bulletin => CDisplayParadigmChanged?.Invoke(bulletin));
        LDisplayRule.LDisplayChosenAttach(
            CSubject.CSubjectReflex, bulletin => CDisplayReflexChanged?.Invoke(bulletin));
        LDisplayRule.LDisplayChosenAttach(CSubject.CSubjectEntry, bulletin => CDisplayEntryChanged?.Invoke(bulletin));
        LDisplayRule.LDisplayObserverAttach(
            CSubject.CSubjectScript, bulletin => CDisplayScriptChanged?.Invoke(bulletin));
        LDisplayRule.LDisplayObserverAttach(
            CSubject.CSubjectFanqie, bulletin => CDisplayFanqieChanged?.Invoke(bulletin));
        LDisplayRule.LDisplayObserverAttach(
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
            LDisplayRule.LDisplayObserverAttach(subject, bulletin => CDisplayEntryChanged?.Invoke(bulletin));
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

        LDisplayRule.LDisplaySound.LDisplaySoundShow(id, draft);
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
            LDisplayRule.LDisplayNoticed.LLedgerRepaintShow(
                _cDisplayEnvoy, _cDisplaySettings, "Display.StampFailed", exception);
            return (false, string.Empty, string.Empty);
        }
    }

    public void CDisplayEntryClose()
    {
        LDisplayRule.LDisplaySound.LDisplaySoundClear();
        CDisplayShown = _cDisplayBlank;
        CDisplayClosed?.Invoke();
    }

    public void CDisplayEntryResonate()
    {
        if (LDisplayChosen is null)
        {
            return;
        }

        LDisplayRule.LDisplayDraftLoad(LDisplayEntryOpen);
    }

    public void CDisplayWorkspaceResonate()
    {
        CDisplayEntryClose();
    }

    public bool CDisplayFavoriteRead()
    {
        return LDisplayRule.LDisplayFavoriteRead(LDisplayChosen);
    }

    public bool CDisplayFavoriteToggle(bool marked)
    {
        LDisplayRule.LDisplayFavoriteSave(LDisplayChosen, marked);
        return CDisplayFavoriteRead();
    }

    public CGrasp CDisplayGraspRead()
    {
        int step = LDisplayRule.LDisplayGraspRead(LDisplayChosen);
        return new CGrasp(step, CDisplayGraspRead(step));
    }

    public string CDisplayGraspRead(int step)
    {
        return LDisplayRule.LDisplayGraspFormat(LDisplayChosen, step);
    }

    public CGrasp CDisplayGraspSet(int step)
    {
        LDisplayRule.LDisplayGraspSave(LDisplayChosen, step);
        return CDisplayGraspRead();
    }

    public CFrequency? CDisplayFrequencyRead(Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(lookup);

        return LDisplayRule.LDisplayFrequencyRead(LDisplayChosen, lookup("Frequency.Once"));
    }
}
