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
        if (PNavigationFind(sender) is QTab chosen)
        {
            _cNavigation.CNavigationTabSelect(chosen.QTabMode);
        }
    }

    private void PNavigationIntroduce()
    {
        _pNavigationTabs = PNavigationTabRead();
        _cNavigation = PWindowAtelier.CAtelierNavigation;
        _cNavigation.CNavigationChanged += PNavigationRefine;
        _cNavigation.CNavigationArrived += PMentionMenuHide;
        PWindowAtelier.CAtelierMention.CMentionSenseChosen += PMentionSenseRefine;
        _pWindowSurface.PreviewKeyDown += PVoyageKeyObserve;
        _pWindowSurface.PreviewMouseDown += PVoyageMouseObserve;
    }

    private void PNavigationRefine(CNavigationState state)
    {
        foreach (QTab tab in _pNavigationTabs)
        {
            tab.QTabButton.Visibility = state.CNavigationStateHidden.Contains(tab.QTabMode)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        if (state.CNavigationStateTab is string mode)
        {
            foreach (QTab tab in _pNavigationTabs)
            {
                bool shown = string.Equals(tab.QTabMode, mode, StringComparison.Ordinal);
                tab.QTabButton.SetValue(PTab.PTabChosenProperty, shown);
                tab.QTabPanel.Visibility = shown ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        PVoyageRefine(state.CNavigationStateVoyage);
    }

    private QTab? PNavigationFind(object? button)
    {
        foreach (QTab tab in _pNavigationTabs)
        {
            if (ReferenceEquals(tab.QTabButton, button))
            {
                return tab;
            }
        }

        return null;
    }

    private QTab[] PNavigationTabRead()
    {
        return
        [
            new QTab("Input", PNavigationInput, PInput),
            new QTab("Library", PNavigationLibrary, PLibrary)
            {
                QTabVoyage = _qLibrary.QLibraryVoyageRefine
            },
            new QTab("Phonology", PNavigationPhonology, PPhonology)
            {
                QTabVoyage = _qPhonology.QPhonologyVoyageRefine
            },
            new QTab("Xiesheng", PNavigationXiesheng, PXiesheng)
            {
                QTabVoyage = _qXiesheng.QXieshengVoyageShow
            },
            new QTab("Yunjing", PNavigationYunjing, PYunjing)
            {
                QTabVoyage = PYunjing.PYunjingVoyageShow
            },
            new QTab("Taxonomy", PNavigationTaxonomy, PTaxonomy)
            {
                QTabVoyage = _qTaxonomy.QTaxonomyVoyageRefine
            },
            new QTab("Tenor", PNavigationTenor, PTenor)
            {
                QTabVoyage = _qTenor.QTenorVoyageRefine
            },
            new QTab("Repertoire", PNavigationRepertoire, PRepertoire)
            {
                QTabVoyage = _qRepertoire.QRepertoireVoyageRefine
            },
            new QTab("Corpus", PNavigationCorpus, PCorpus)
            {
                QTabVoyage = _qCorpus.QCorpusVoyageShow
            },
            new QTab("Reference", PNavigationSource, PReference)
            {
                QTabVoyage = _qReference.QReferenceVoyageRefine
            },
            new QTab("Guild", PNavigationGuild, PGuild)
            {
                QTabVoyage = _qGuild.QGuildVoyageShow
            },
            new QTab("Favorite", PNavigationFavorite, PFavorite)
            {
                QTabVoyage = _qFavorite.QFavoriteVoyageShow
            },
            new QTab("Duplex", PNavigationDuplex, PDuplex),
            new QTab("Settings", PNavigationSettings, PSettings)
        ];
    }

    private void PVoyageRefine(CVoyageState state)
    {
        foreach (QTab tab in _pNavigationTabs)
        {
            tab.QTabVoyage?.Invoke(state.CVoyageStatePast, state.CVoyageStateFuture);
        }
    }

    private void PVoyageKeyObserve(object sender, KeyEventArgs e)
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

    private void PVoyageMouseObserve(object sender, MouseButtonEventArgs e)
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

    internal void PWindowMentionRefine(PMention anchor, CMentionOffer? offer)
    {
        ArgumentNullException.ThrowIfNull(anchor);

        if (offer is null)
        {
            return;
        }

        PMentionOfferRefine(anchor, anchor.PMentionPieceRead(offer.CMentionOfferOffset), offer);
    }

    private void PMentionSenseRefine(long sense)
    {
        _qLibrary.QLibraryLectern.QLecternCard.QLecternSpotlightRefine(sense);
    }

    private void PMentionLeaveRefine(object? sender, EventArgs e)
    {
        PMentionMenuHide();
    }
}
