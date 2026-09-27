using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    private PTab PNavigationInput => (PTab)_pWindowSurface.FindName(nameof(PNavigationInput));

    private PTab PNavigationLibrary => (PTab)_pWindowSurface.FindName(nameof(PNavigationLibrary));

    private PTab PNavigationFavorite => (PTab)_pWindowSurface.FindName(nameof(PNavigationFavorite));

    private PTab PNavigationPhonology => (PTab)_pWindowSurface.FindName(nameof(PNavigationPhonology));

    private PTab PNavigationRepertoire => (PTab)_pWindowSurface.FindName(nameof(PNavigationRepertoire));

    private PTab PNavigationTenor => (PTab)_pWindowSurface.FindName(nameof(PNavigationTenor));

    private PTab PNavigationTaxonomy => (PTab)_pWindowSurface.FindName(nameof(PNavigationTaxonomy));

    private PTab PNavigationCorpus => (PTab)_pWindowSurface.FindName(nameof(PNavigationCorpus));

    private PTab PNavigationSource => (PTab)_pWindowSurface.FindName(nameof(PNavigationSource));

    private PTab PNavigationGuild => (PTab)_pWindowSurface.FindName(nameof(PNavigationGuild));

    private PTab PNavigationXiesheng => (PTab)_pWindowSurface.FindName(nameof(PNavigationXiesheng));

    private PTab PNavigationYunjing => (PTab)_pWindowSurface.FindName(nameof(PNavigationYunjing));

    private PTab PNavigationDuplex => (PTab)_pWindowSurface.FindName(nameof(PNavigationDuplex));

    private PTab PNavigationSettings => (PTab)_pWindowSurface.FindName(nameof(PNavigationSettings));

    private void PNavigationHandle(object sender, RoutedEventArgs e)
    {
        if (PNavigationFind(sender) is not LTab chosen)
        {
            return;
        }

        if (_lNavigation.LNavigationSelect(chosen.LTabMode, PNavigationLeaveCheck))
        {
            PNavigationApply(chosen.LTabMode);
        }
    }

    private void PNavigationAttach()
    {
        _pNavigationTabs = PNavigationTabRead();
        string[] modes = new string[_pNavigationTabs.Length];
        for (int index = 0; index < modes.Length; index++)
        {
            modes[index] = _pNavigationTabs[index].LTabMode;
        }

        _lNavigation = new LNavigation(_lWindow, modes);
        _pWindowSurface.PreviewKeyDown += PVoyageKeyHandle;
        _pWindowSurface.PreviewMouseDown += PVoyageMouseHandle;
    }

    private void PNavigationRestore()
    {
        foreach (LTab tab in _pNavigationTabs)
        {
            tab.LTabButton.Visibility = PNavigationAllowCheck(tab.LTabMode) ? Visibility.Visible : Visibility.Collapsed;
        }

        if (_lNavigation.LNavigationRestore(PNavigationAllowCheck) is string restored)
        {
            PNavigationApply(restored);
            PNavigationFind(restored).LTabScribe?.Invoke(_lWindow.LWindowPostureRead().CPostureStateSplit);
        }
    }

    private bool PNavigationShow(object button, long id)
    {
        (string tab, long station) = PVoyageStationRead();
        if (PNavigationFind(button) is not LTab target)
        {
            return false;
        }

        if (!PNavigationArrivalShow(target.LTabMode, id))
        {
            return false;
        }

        _lNavigation.LNavigationVoyage.LVoyageRecord(tab, station);
        PVoyageUpdate();
        return true;
    }

    private void PNavigationShow(object button, Action arrival)
    {
        if (PNavigationFind(button) is not LTab target)
        {
            return;
        }

        if (!_lNavigation.LNavigationShow(target.LTabMode, PNavigationLeaveCheck))
        {
            return;
        }

        PNavigationApply(target.LTabMode);
        arrival();
    }

    private bool PNavigationArrivalShow(string mode, long id)
    {
        LTab target = PNavigationFind(mode);
        if (target.LTabArrival is null)
        {
            return false;
        }

        if (!_lNavigation.LNavigationShow(mode, PNavigationLeaveCheck))
        {
            return false;
        }

        PNavigationApply(mode);
        target.LTabArrival(id);
        return true;
    }

    private void PNavigationApply(string mode)
    {
        foreach (LTab tab in _pNavigationTabs)
        {
            bool shown = string.Equals(tab.LTabMode, mode, StringComparison.Ordinal);
            tab.LTabButton.SetValue(PTab.PTabChosenProperty, shown);
            tab.LTabPanel.Visibility = shown ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private bool PNavigationLeaveCheck(string mode)
    {
        return PNavigationFind(mode).LTabLeave?.Invoke() ?? true;
    }

    private bool PNavigationAllowCheck(string mode)
    {
        return PNavigationFind(mode).LTabAllowed?.Invoke() ?? true;
    }

    private LTab? PNavigationFind(object? button)
    {
        foreach (LTab tab in _pNavigationTabs)
        {
            if (ReferenceEquals(tab.LTabButton, button))
            {
                return tab;
            }
        }

        return null;
    }

    private LTab PNavigationFind(string mode)
    {
        foreach (LTab tab in _pNavigationTabs)
        {
            if (string.Equals(tab.LTabMode, mode, StringComparison.Ordinal))
            {
                return tab;
            }
        }

        throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
    }

    private LTab[] PNavigationTabRead()
    {
        return
        [
            new LTab("Input", PNavigationInput, PInput),
            new LTab("Library", PNavigationLibrary, PLibrary)
            {
                LTabLeave = PLibrary.PLibraryLeaveConfirm,
                LTabScribe = PLibrary.PLibraryScribeRestore,
                LTabStation = PLibrary.PLibraryVoyageRead,
                LTabVoyage = PLibrary.PLibraryVoyageShow,
                LTabArrival = PLibrary.PIndexEntryShow
            },
            new LTab("Phonology", PNavigationPhonology, PPhonology)
            {
                LTabLeave = PPhonology.PPhonologyLeaveConfirm,
                LTabScribe = PPhonology.PPhonologyScribeRestore,
                LTabStation = PPhonology.PPhonologyVoyageRead,
                LTabVoyage = PPhonology.PPhonologyVoyageShow,
                LTabArrival = PPhonology.PInventoryEntryShow
            },
            new LTab("Xiesheng", PNavigationXiesheng, PXiesheng)
            {
                LTabAllowed = PXiesheng.PXieshengCheck,
                LTabLeave = PXiesheng.PXieshengLeaveConfirm,
                LTabScribe = PXiesheng.PXieshengScribeRestore,
                LTabStation = PXiesheng.PXieshengVoyageRead,
                LTabVoyage = PXiesheng.PXieshengVoyageShow,
                LTabArrival = PXiesheng.PKindredEntryShow
            },
            new LTab("Yunjing", PNavigationYunjing, PYunjing)
            {
                LTabAllowed = PYunjing.PYunjingCheck,
                LTabLeave = PYunjing.PYunjingLeaveConfirm,
                LTabScribe = PYunjing.PYunjingScribeRestore,
                LTabStation = PYunjing.PYunjingVoyageRead,
                LTabVoyage = PYunjing.PYunjingVoyageShow,
                LTabArrival = PYunjing.PXiaoyunEntryShow
            },
            new LTab("Taxonomy", PNavigationTaxonomy, PTaxonomy)
            {
                LTabLeave = PTaxonomy.PTaxonomyLeaveConfirm,
                LTabScribe = PTaxonomy.PTaxonomyScribeRestore,
                LTabStation = PTaxonomy.PTaxonomyVoyageRead,
                LTabVoyage = PTaxonomy.PTaxonomyVoyageShow,
                LTabArrival = PTaxonomy.PDirectoryTagShow
            },
            new LTab("Tenor", PNavigationTenor, PTenor)
            {
                LTabLeave = PTenor.PTenorLeaveConfirm,
                LTabScribe = PTenor.PTenorScribeRestore,
                LTabStation = PTenor.PTenorVoyageRead,
                LTabVoyage = PTenor.PTenorVoyageShow,
                LTabArrival = PTenor.PGamutRegisterShow
            },
            new LTab("Repertoire", PNavigationRepertoire, PRepertoire)
            {
                LTabLeave = PRepertoire.PRepertoireLeaveConfirm,
                LTabScribe = PRepertoire.PRepertoireScribeRestore,
                LTabStation = PRepertoire.PRepertoireVoyageRead,
                LTabVoyage = PRepertoire.PRepertoireVoyageShow,
                LTabArrival = PRepertoire.PAtlasSituationShow
            },
            new LTab("Corpus", PNavigationCorpus, PCorpus)
            {
                LTabLeave = PCorpus.PCorpusLeaveConfirm,
                LTabScribe = PCorpus.PCorpusScribeRestore,
                LTabStation = PCorpus.PCorpusVoyageRead,
                LTabVoyage = PCorpus.PCorpusVoyageShow,
                LTabArrival = PCorpus.PAnthologyExampleShow
            },
            new LTab("Reference", PNavigationSource, PReference)
            {
                LTabLeave = PReference.PReferenceLeaveConfirm,
                LTabScribe = PReference.PReferenceScribeRestore,
                LTabStation = PReference.PReferenceVoyageRead,
                LTabVoyage = PReference.PReferenceVoyageShow,
                LTabArrival = PReference.PShelfSourceShow
            },
            new LTab("Guild", PNavigationGuild, PGuild)
            {
                LTabLeave = PGuild.PGuildLeaveConfirm,
                LTabScribe = PGuild.PGuildScribeRestore,
                LTabStation = PGuild.PGuildVoyageRead,
                LTabVoyage = PGuild.PGuildVoyageShow,
                LTabArrival = PGuild.PRollAuthorShow
            },
            new LTab("Favorite", PNavigationFavorite, PFavorite)
            {
                LTabLeave = PFavorite.PFavoriteLeaveConfirm,
                LTabScribe = PFavorite.PFavoriteScribeRestore,
                LTabStation = PFavorite.PFavoriteVoyageRead,
                LTabVoyage = PFavorite.PFavoriteVoyageShow,
                LTabArrival = PFavorite.PRosterEntryShow
            },
            new LTab("Duplex", PNavigationDuplex, PDuplex),
            new LTab("Settings", PNavigationSettings, PSettings)
        ];
    }

    internal void PVoyageRecord()
    {
        (string tab, long id) = PVoyageStationRead();
        _lNavigation.LNavigationVoyage.LVoyageRecord(tab, id);
        PVoyageUpdate();
    }

    internal void PVoyageRetreatRun()
    {
        (string tab, long id) = PVoyageStationRead();
        _lNavigation.LNavigationVoyage.LVoyageRetreat(tab, id, PNavigationArrivalShow);
        PVoyageUpdate();
    }

    internal void PVoyageAdvanceRun()
    {
        (string tab, long id) = PVoyageStationRead();
        _lNavigation.LNavigationVoyage.LVoyageAdvance(tab, id, PNavigationArrivalShow);
        PVoyageUpdate();
    }

    private (string PVoyageTab, long PVoyageId) PVoyageStationRead()
    {
        LTab tab = PNavigationFind(_lNavigation.LNavigationShownRead());
        return (tab.LTabMode, tab.LTabStation?.Invoke() ?? 0);
    }

    private void PVoyageUpdate()
    {
        bool past = _lNavigation.LNavigationVoyage.LVoyagePastCheck();
        bool future = _lNavigation.LNavigationVoyage.LVoyageFutureCheck();
        foreach (LTab tab in _pNavigationTabs)
        {
            tab.LTabVoyage?.Invoke(past, future);
        }
    }

    private void PVoyageKeyHandle(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.System || Keyboard.Modifiers != ModifierKeys.Alt)
        {
            return;
        }

        if (e.SystemKey == Key.Left)
        {
            PVoyageRetreatRun();
            e.Handled = true;
        }
        else if (e.SystemKey == Key.Right)
        {
            PVoyageAdvanceRun();
            e.Handled = true;
        }
    }

    private void PVoyageMouseHandle(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.XButton1)
        {
            PVoyageRetreatRun();
            e.Handled = true;
        }
        else if (e.ChangedButton == MouseButton.XButton2)
        {
            PVoyageAdvanceRun();
            e.Handled = true;
        }
    }

    internal bool PWindowEntryShow(long id)
    {
        PMentionMenuHide();
        return PNavigationShow(PNavigationLibrary, id);
    }

    internal bool PWindowSituationShow(long id)
    {
        return PNavigationShow(PNavigationRepertoire, id);
    }

    internal bool PWindowTagShow(long id)
    {
        return PNavigationShow(PNavigationTaxonomy, id);
    }

    internal bool PWindowExampleShow(long id)
    {
        return PNavigationShow(PNavigationCorpus, id);
    }

    internal bool PWindowRegisterShow(long id)
    {
        return PNavigationShow(PNavigationTenor, id);
    }

    internal void PWindowDiweiShow(string language, string kind, string key)
    {
        PNavigationShow(PNavigationYunjing, () => PYunjing.PYunjingDiweiShow(language, kind, key));
    }

    internal void PWindowStemShow(string language, string? key)
    {
        PNavigationShow(PNavigationXiesheng, () => PXiesheng.PXieshengStemShow(language, key));
    }

    internal void PWindowMentionHandle(PMention anchor, LMentionResult result)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(result);

        PMentionMenuHide();

        if (result.LMentionResultStored is LMention stored)
        {
            if (!stored.LMentionLinked)
            {
                return;
            }

            PWindowEntryShow(stored.LMentionEntryId);
            if (stored.LMentionSensed)
            {
                PLibrary.PDisplay.PDisplayCardScroll(stored.LMentionSenseId);
            }

            return;
        }

        if (result.LMentionResultSingle)
        {
            PWindowEntryShow(result.LMentionResultFirst);
            return;
        }

        if (result.LMentionResultMany)
        {
            PMentionMenuShow(anchor, anchor.PMentionPieceRead(result.LMentionResultOffset), result);
        }
    }

    internal void PWindowSenseShow(FrameworkElement anchor, Rect place, long entryId, Action<long> chosen)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(chosen);

        IReadOnlyList<CMeaning> meanings;
        try
        {
            meanings = _lWindow.LWindowWorkspace.QWorkspaceMeaningRead(
                entryId, QLocalizationCatalog.QLocalizationTextRead("Display.Unknown"));
        }
        catch (Exception exception)
        {
            PWindowFailureShow("Mention.FindFailed", exception);
            return;
        }

        PMentionMenuShow(anchor, place, entryId, meanings, chosen);
    }

    internal void PWindowProspectShow(
        FrameworkElement anchor, Rect place, string word, string language, Action<long> chosen)
    {
        PInput.PEditor.PProspectShow(anchor, place, word, language, chosen);
    }

    private void PMentionLeaveHandle(object? sender, EventArgs e)
    {
        PMentionMenuHide();
    }
}
