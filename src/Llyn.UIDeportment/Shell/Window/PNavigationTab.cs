using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Llyn.Conduct;

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

    private void PNavigationObserve(object sender, RoutedEventArgs e)
    {
        if (PNavigationFind(sender) is LTab chosen)
        {
            _cNavigation.CNavigationTabSelect(chosen.LTabMode);
        }
    }

    private void PNavigationIntroduce()
    {
        _pNavigationTabs = PNavigationTabRead();
        _cNavigation = PWindowAtelier.CAtelierNavigation;
        _cNavigation.CNavigationChanged += PNavigationRefine;
        _cNavigation.CNavigationArrived += PMentionMenuHide;
        _pWindowSurface.PreviewKeyDown += PVoyageKeyHandle;
        _pWindowSurface.PreviewMouseDown += PVoyageMouseHandle;
    }

    private void PNavigationRefine(CNavigationState state)
    {
        foreach (LTab tab in _pNavigationTabs)
        {
            tab.LTabButton.Visibility = state.CNavigationStateHidden.Contains(tab.LTabMode)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        if (state.CNavigationStateTab is string mode)
        {
            foreach (LTab tab in _pNavigationTabs)
            {
                bool shown = string.Equals(tab.LTabMode, mode, StringComparison.Ordinal);
                tab.LTabButton.SetValue(PTab.PTabChosenProperty, shown);
                tab.LTabPanel.Visibility = shown ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        PVoyageRefine(state.CNavigationStateVoyage);
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

    private LTab[] PNavigationTabRead()
    {
        return
        [
            new LTab("Input", PNavigationInput, PInput),
            new LTab("Library", PNavigationLibrary, PLibrary)
            {
                LTabVoyage = _qLibrary.QLibraryVoyageShow
            },
            new LTab("Phonology", PNavigationPhonology, PPhonology)
            {
                LTabVoyage = _qPhonology.QPhonologyVoyageShow
            },
            new LTab("Xiesheng", PNavigationXiesheng, PXiesheng)
            {
                LTabVoyage = _qXiesheng.QXieshengVoyageShow
            },
            new LTab("Yunjing", PNavigationYunjing, PYunjing)
            {
                LTabVoyage = PYunjing.PYunjingVoyageShow
            },
            new LTab("Taxonomy", PNavigationTaxonomy, PTaxonomy)
            {
                LTabVoyage = _qTaxonomy.QTaxonomyVoyageShow
            },
            new LTab("Tenor", PNavigationTenor, PTenor)
            {
                LTabVoyage = _qTenor.QTenorVoyageShow
            },
            new LTab("Repertoire", PNavigationRepertoire, PRepertoire)
            {
                LTabVoyage = _qRepertoire.QRepertoireVoyageShow
            },
            new LTab("Corpus", PNavigationCorpus, PCorpus)
            {
                LTabVoyage = _qCorpus.QCorpusVoyageShow
            },
            new LTab("Reference", PNavigationSource, PReference)
            {
                LTabVoyage = _qReference.QReferenceVoyageShow
            },
            new LTab("Guild", PNavigationGuild, PGuild)
            {
                LTabVoyage = _qGuild.QGuildVoyageShow
            },
            new LTab("Favorite", PNavigationFavorite, PFavorite)
            {
                LTabVoyage = _qFavorite.QFavoriteVoyageShow
            },
            new LTab("Duplex", PNavigationDuplex, PDuplex),
            new LTab("Settings", PNavigationSettings, PSettings)
        ];
    }

    private void PVoyageRefine(CVoyageState state)
    {
        foreach (LTab tab in _pNavigationTabs)
        {
            tab.LTabVoyage?.Invoke(state.CVoyageStatePast, state.CVoyageStateFuture);
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
            _cNavigation.CNavigationStationUndo();
            e.Handled = true;
        }
        else if (e.SystemKey == Key.Right)
        {
            _cNavigation.CNavigationStationRedo();
            e.Handled = true;
        }
    }

    private void PVoyageMouseHandle(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.XButton1)
        {
            _cNavigation.CNavigationStationUndo();
            e.Handled = true;
        }
        else if (e.ChangedButton == MouseButton.XButton2)
        {
            _cNavigation.CNavigationStationRedo();
            e.Handled = true;
        }
    }

    internal bool PWindowEntryShow(long id)
    {
        PMentionMenuHide();
        return _cNavigation.CNavigationEntryOpen(id);
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
