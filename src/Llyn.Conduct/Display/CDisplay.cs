using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDisplay
{
    private static readonly CLectern _cDisplayBlank = new(
        string.Empty, string.Empty, [], false, [], false, string.Empty, string.Empty, false, string.Empty);

    private readonly LEntryPort _cDisplayEntryPort;

    private readonly LMarkdownPort _cDisplayMarkdownPort;

    private readonly CEnvoy _cDisplayEnvoy;

    private readonly LSettingsPort _cDisplaySettings;

    internal CDisplay(
        LDraftPort drafts,
        CEntryBundle entries,
        CPhonologyBundle phonology,
        LSettingsPort settings,
        LMediaPort media,
        CEnvoy envoy,
        CLedgerNoticed noticed)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        LDisplayRule = new LDisplay(
            drafts,
            entries.CEntryBundleEntry,
            entries.CEntryBundleFavorite,
            entries.CEntryBundleVista,
            entries.CEntryBundleGrasp,
            entries.CEntryBundlePronunciation,
            phonology.CPhonologyBundleLanguage,
            phonology.CPhonologyBundleReflex,
            phonology.CPhonologyBundleFanqie,
            phonology.CPhonologyBundleScript,
            phonology.CPhonologyBundleParadigm,
            settings,
            media,
            envoy,
            noticed);
        _cDisplayEntryPort = entries.CEntryBundleEntry;
        _cDisplayMarkdownPort = entries.CEntryBundleMarkdown;
        _cDisplayEnvoy = envoy;
        _cDisplaySettings = settings;
        CDisplaySound = new CDisplaySound(
            LDisplayRule,
            this,
            drafts,
            entries.CEntryBundleEntry,
            entries.CEntryBundleGlyph,
            phonology.CPhonologyBundleFanqie,
            phonology.CPhonologyBundleDiwei,
            phonology.CPhonologyBundleScript,
            phonology.CPhonologyBundleParadigm,
            phonology.CPhonologyBundleReflex,
            settings,
            envoy);
        CDisplayFold = new CFold(
            () => LDisplayRule.LDisplaySound.LDisplayEntry,
            phonology.CPhonologyBundleReflex,
            settings,
            envoy,
            LDisplayRule.LDisplayNoticed);
        CDisplayAccent = new CDisplayAccent(LDisplayRule, phonology.CPhonologyBundleLanguage, settings, envoy);
        CDisplayPlayback = new CDisplayPlayback(LDisplayRule, media, settings, envoy);
        CDisplayGrasp = new CDisplayGrasp(LDisplayRule);
        CDisplayCard = new CDisplayCard(
            LDisplayRule,
            entries.CEntryBundleCard,
            entries.CEntryBundleReference,
            entries.CEntryBundleExample,
            phonology.CPhonologyBundleSentence,
            settings,
            envoy);
        CDisplayRoute = new CDisplayRoute(LDisplayRule, entries.CEntryBundleMention, settings, envoy);
        CDisplayCompass = new CCompass(
            LDisplayRule, entries.CEntryBundleEntry, entries.CEntryBundleVista, settings, envoy);
    }

    public event Action? CDisplayOpened;

    public event Action? CDisplayClosed;

    public event Action<CBulletin>? CDisplayFavoriteChanged;

    public event Action<CBulletin>? CDisplayFrequencyChanged;

    public event Action<CBulletin>? CDisplayParadigmChanged;

    public event Action<CBulletin>? CDisplayReflexChanged;

    public event Action<CBulletin>? CDisplayScriptChanged;

    public event Action<CBulletin>? CDisplayFanqieChanged;

    public event Action<CBulletin>? CDisplayEntryChanged;

    public event Action<CBulletin>? CDisplayWorkspaceChanged;

    public event Action<CBulletin>? CDisplayFoldChanged;

    public CLectern CDisplayShown { get; private set; } = _cDisplayBlank;

    public CDisplaySound CDisplaySound { get; }

    public CFold CDisplayFold { get; }

    public CDisplayAccent CDisplayAccent { get; }

    public CDisplayPlayback CDisplayPlayback { get; }

    public CDisplayGrasp CDisplayGrasp { get; }

    public CDisplayCard CDisplayCard { get; }

    public CDisplayRoute CDisplayRoute { get; }

    public CCompass CDisplayCompass { get; }

    internal LDisplay LDisplayRule { get; }

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
        CDisplayGrasp.LDisplayGraspAttach();
        LDisplayRule.LDisplayChosenAttach(
            CSubject.CSubjectFrequency, bulletin => CDisplayFrequencyChanged?.Invoke(bulletin));
        LDisplayRule.LDisplayChosenAttach(
            CSubject.CSubjectInflection, bulletin => CDisplayParadigmChanged?.Invoke(bulletin));
        LDisplayRule.LDisplayChosenAttach(
            CSubject.CSubjectReflex, bulletin => CDisplayReflexChanged?.Invoke(bulletin));
        LDisplayRule.LDisplayChosenAttach(CSubject.CSubjectEntry, bulletin => CDisplayEntryChanged?.Invoke(bulletin));
        LDisplayRule.LDisplayChosenAttach(CSubject.CSubjectFold, bulletin => CDisplayFoldChanged?.Invoke(bulletin));
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
        if (draft is null || LDisplayRule.LDisplayChosen is not long id)
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
            CMarkdown.LMarkdownParse(_cDisplayMarkdownPort, draft.LEntryDraftNote),
            draft.LEntryDraftNoted,
            stamp.Item2,
            stamp.Item3,
            stamp.Item1,
            _cDisplayEntryPort.LEngineUnitFormat(draft.LEntryDraftUnit));
        CDisplayOpened?.Invoke();
    }

    private (bool, string, string) LDisplayStampRead(long id)
    {
        try
        {
            return _cDisplayEntryPort.LEngineStampRead(id);
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
        if (LDisplayRule.LDisplayChosen is null)
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
        return LDisplayRule.LDisplayFavoriteRead(LDisplayRule.LDisplayChosen);
    }

    public bool CDisplayFavoriteToggle(bool marked)
    {
        LDisplayRule.LDisplayFavoriteSave(LDisplayRule.LDisplayChosen, marked);
        return CDisplayFavoriteRead();
    }

    public CFrequency? CDisplayFrequencyRead(Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(lookup);

        return LDisplayRule.LDisplayFrequencyRead(LDisplayRule.LDisplayChosen, lookup("Frequency.Once"));
    }
}
