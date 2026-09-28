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

        if (_cNavigation.CNavigationTabSelect(chosen.LTabMode, false, PNavigationLeaveCheck))
        {
            PNavigationApply(chosen.LTabMode);
        }
    }

    private void PNavigationAttach()
    {
        LTab[] tabs = PNavigationTabRead();
        _pNavigationTabs = tabs;
        string[] modes = new string[tabs.Length];
        for (int index = 0; index < modes.Length; index++)
        {
            modes[index] = tabs[index].LTabMode;
        }

        _cNavigation = CNavigation.CNavigationCreate(PWindowAtelier, modes);
        _pWindowSurface.PreviewKeyDown += PVoyageKeyHandle;
        _pWindowSurface.PreviewMouseDown += PVoyageMouseHandle;
    }

    private void PNavigationRestore()
    {
        foreach (LTab tab in _pNavigationTabs)
        {
            tab.LTabButton.Visibility = PNavigationAllowCheck(tab.LTabMode) ? Visibility.Visible : Visibility.Collapsed;
        }

        if (_cNavigation.CNavigationTabOpen(PNavigationAllowCheck) is string restored)
        {
            PNavigationApply(restored);
            PNavigationFind(restored).LTabScribe?.Invoke(
                PWindowAtelier.CAtelierSplitRead());
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

        _lVoyage.LVoyageRecord(tab, station);
        PVoyageUpdate();
        return true;
    }

    private void PNavigationShow(object button, Action arrival)
    {
        if (PNavigationFind(button) is not LTab target)
        {
            return;
        }

        if (!_cNavigation.CNavigationTabSelect(target.LTabMode, true, PNavigationLeaveCheck))
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

        if (!_cNavigation.CNavigationTabSelect(mode, true, PNavigationLeaveCheck))
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
                LTabLeave = _qLibrary.QLibraryLeaveConfirm,
                LTabScribe = _qLibrary.QLibraryScribeRestore,
                LTabStation = _qLibrary.QLibraryVoyageRead,
                LTabVoyage = _qLibrary.QLibraryVoyageShow,
                LTabArrival = _qLibrary.QIndexEntryShow
            },
            new LTab("Phonology", PNavigationPhonology, PPhonology)
            {
                LTabLeave = _qPhonology.QPhonologyLeaveConfirm,
                LTabScribe = _qPhonology.QPhonologyScribeRestore,
                LTabStation = _qPhonology.QPhonologyVoyageRead,
                LTabVoyage = _qPhonology.QPhonologyVoyageShow,
                LTabArrival = _qPhonology.QInventoryEntryShow
            },
            new LTab("Xiesheng", PNavigationXiesheng, PXiesheng)
            {
                LTabAllowed = _qXiesheng.QXieshengCheck,
                LTabLeave = _qXiesheng.QXieshengLeaveConfirm,
                LTabScribe = _qXiesheng.QXieshengScribeRestore,
                LTabStation = _qXiesheng.QXieshengVoyageRead,
                LTabVoyage = _qXiesheng.QXieshengVoyageShow,
                LTabArrival = _qXiesheng.QKindredEntryShow
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
                LTabLeave = _qTaxonomy.QTaxonomyLeaveConfirm,
                LTabScribe = _qTaxonomy.QTaxonomyScribeRestore,
                LTabStation = _qTaxonomy.QTaxonomyVoyageRead,
                LTabVoyage = _qTaxonomy.QTaxonomyVoyageShow,
                LTabArrival = _qTaxonomy.QDirectoryTagShow
            },
            new LTab("Tenor", PNavigationTenor, PTenor)
            {
                LTabLeave = _qTenor.QTenorLeaveConfirm,
                LTabScribe = _qTenor.QTenorScribeRestore,
                LTabStation = _qTenor.QTenorVoyageRead,
                LTabVoyage = _qTenor.QTenorVoyageShow,
                LTabArrival = _qTenor.QGamutRegisterShow
            },
            new LTab("Repertoire", PNavigationRepertoire, PRepertoire)
            {
                LTabLeave = _qRepertoire.QRepertoireLeaveConfirm,
                LTabScribe = _qRepertoire.QRepertoireScribeRestore,
                LTabStation = _qRepertoire.QRepertoireVoyageRead,
                LTabVoyage = _qRepertoire.QRepertoireVoyageShow,
                LTabArrival = _qRepertoire.QAtlasSituationShow
            },
            new LTab("Corpus", PNavigationCorpus, PCorpus)
            {
                LTabLeave = _qCorpus.QCorpusLeaveConfirm,
                LTabScribe = _qCorpus.QCorpusScribeRestore,
                LTabStation = _qCorpus.QCorpusVoyageRead,
                LTabVoyage = _qCorpus.QCorpusVoyageShow,
                LTabArrival = _qCorpus.QAnthologyExampleShow
            },
            new LTab("Reference", PNavigationSource, PReference)
            {
                LTabLeave = _qReference.QReferenceLeaveConfirm,
                LTabScribe = _qReference.QReferenceScribeRestore,
                LTabStation = _qReference.QReferenceVoyageRead,
                LTabVoyage = _qReference.QReferenceVoyageShow,
                LTabArrival = _qReference.QShelfSourceShow
            },
            new LTab("Guild", PNavigationGuild, PGuild)
            {
                LTabLeave = _qGuild.QGuildLeaveConfirm,
                LTabScribe = _qGuild.QGuildScribeRestore,
                LTabStation = _qGuild.QGuildVoyageRead,
                LTabVoyage = _qGuild.QGuildVoyageShow,
                LTabArrival = _qGuild.QRollAuthorShow
            },
            new LTab("Favorite", PNavigationFavorite, PFavorite)
            {
                LTabLeave = _qFavorite.QFavoriteLeaveConfirm,
                LTabScribe = _qFavorite.QFavoriteScribeRestore,
                LTabStation = _qFavorite.QFavoriteVoyageRead,
                LTabVoyage = _qFavorite.QFavoriteVoyageShow,
                LTabArrival = _qFavorite.QRosterEntryShow
            },
            new LTab("Duplex", PNavigationDuplex, PDuplex),
            new LTab("Settings", PNavigationSettings, PSettings)
        ];
    }

    internal void PVoyageRecord()
    {
        (string tab, long id) = PVoyageStationRead();
        _lVoyage.LVoyageRecord(tab, id);
        PVoyageUpdate();
    }

    internal void PVoyageRetreatRun()
    {
        (string tab, long id) = PVoyageStationRead();
        _lVoyage.LVoyageRetreat(tab, id, PNavigationArrivalShow);
        PVoyageUpdate();
    }

    internal void PVoyageAdvanceRun()
    {
        (string tab, long id) = PVoyageStationRead();
        _lVoyage.LVoyageAdvance(tab, id, PNavigationArrivalShow);
        PVoyageUpdate();
    }

    private (string PVoyageTab, long PVoyageId) PVoyageStationRead()
    {
        LTab tab = PNavigationFind(_cNavigation.CNavigationTabRead());
        return (tab.LTabMode, tab.LTabStation?.Invoke() ?? 0);
    }

    private void PVoyageUpdate()
    {
        bool past = _lVoyage.LVoyagePastCheck();
        bool future = _lVoyage.LVoyageFutureCheck();
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
        PNavigationShow(PNavigationXiesheng, () => _qXiesheng.QXieshengStemShow(language, key));
    }

    internal void PWindowMentionHandle(PMention anchor, CMentionResult result)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(result);

        PMentionMenuHide();

        if (result.CMentionResultStored is CMentionMark stored)
        {
            if (stored.CMentionMarkEntry == 0)
            {
                return;
            }

            PWindowEntryShow(stored.CMentionMarkEntry);
            if (stored.CMentionMarkSense != 0)
            {
                _qLibrary.QLibraryDisplay.PDisplayCardScroll(stored.CMentionMarkSense);
            }

            return;
        }

        if (result.CMentionResultSingle)
        {
            PWindowEntryShow(result.CMentionResultFirst);
            return;
        }

        if (result.CMentionResultMany)
        {
            PMentionMenuShow(anchor, anchor.PMentionPieceRead(result.CMentionResultOffset), result);
        }
    }

    internal void PWindowSenseShow(FrameworkElement anchor, Rect place, long entryId, Action<long> chosen)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(chosen);

        IReadOnlyList<CMeaning> meanings;
        try
        {
            meanings = PWindowAtelier.CAtelierCatalog.CCatalogMeaningSort(entryId);
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
