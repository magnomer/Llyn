using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class QWindow
{
    private Button QNavigationInput => (Button)_qWindowSurface.FindName("PNavigationInput");

    private Button QNavigationLibrary => (Button)_qWindowSurface.FindName("PNavigationLibrary");

    private Button QNavigationFavorite => (Button)_qWindowSurface.FindName("PNavigationFavorite");

    private Button QNavigationPhonology => (Button)_qWindowSurface.FindName("PNavigationPhonology");

    private Button QNavigationRepertoire => (Button)_qWindowSurface.FindName("PNavigationRepertoire");

    private Button QNavigationTenor => (Button)_qWindowSurface.FindName("PNavigationTenor");

    private Button QNavigationTaxonomy => (Button)_qWindowSurface.FindName("PNavigationTaxonomy");

    private Button QNavigationCorpus => (Button)_qWindowSurface.FindName("PNavigationCorpus");

    private Button QNavigationSource => (Button)_qWindowSurface.FindName("PNavigationSource");

    private Button QNavigationGuild => (Button)_qWindowSurface.FindName("PNavigationGuild");

    private Button QNavigationXiesheng => (Button)_qWindowSurface.FindName("PNavigationXiesheng");

    private Button QNavigationYunjing => (Button)_qWindowSurface.FindName("PNavigationYunjing");

    private Button QNavigationDuplex => (Button)_qWindowSurface.FindName("PNavigationDuplex");

    private Button QNavigationSettings => (Button)_qWindowSurface.FindName("PNavigationSettings");

    private void QNavigationObserve(object sender, RoutedEventArgs e)
    {
        if (QNavigationFind(sender) is QTab chosen)
        {
            _cNavigation.CNavigationTabSelect(chosen.QTabMode);
        }
    }

    private void QNavigationIntroduce()
    {
        _qNavigationTabs = QNavigationTabRead();
        foreach (QTab tab in _qNavigationTabs)
        {
            QNavigationChosenRefine(tab);
        }

        _cNavigation = QWindowAtelier.CAtelierNavigation;
        _cNavigation.CNavigationChanged += QNavigationRefine;
        _cNavigation.CNavigationArrived += QMentionMenuHide;
        QWindowAtelier.CAtelierMention.CMentionSenseChosen += QMentionSenseRefine;
        _qWindowSurface.PreviewKeyDown += QVoyageKeyObserve;
        _qWindowSurface.PreviewMouseDown += QVoyageMouseObserve;
    }

    private void QNavigationRefine(CNavigationState state)
    {
        foreach (QTab tab in _qNavigationTabs)
        {
            tab.QTabButton.Visibility = state.CNavigationStateHidden.Contains(tab.QTabMode)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        if (state.CNavigationStateTab is string mode)
        {
            foreach (QTab tab in _qNavigationTabs)
            {
                bool shown = string.Equals(tab.QTabMode, mode, StringComparison.Ordinal);
                tab.QTabPanel.Visibility = shown ? Visibility.Visible : Visibility.Collapsed;
                QNavigationChosenRefine(tab);
            }
        }

        QVoyageRefine(state.CNavigationStateVoyage);
    }

    private static void QNavigationChosenRefine(QTab tab)
    {
        if (tab.QTabPanel.Visibility == Visibility.Visible)
        {
            tab.QTabButton.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
        }
        else
        {
            tab.QTabButton.ClearValue(QLook.QLookCueProperty);
        }
    }

    private QTab? QNavigationFind(object? button)
    {
        foreach (QTab tab in _qNavigationTabs)
        {
            if (ReferenceEquals(tab.QTabButton, button))
            {
                return tab;
            }
        }

        return null;
    }

    private QTab[] QNavigationTabRead()
    {
        return
        [
            new QTab("Input", QNavigationInput, QInputSurface),
            new QTab("Library", QNavigationLibrary, QLibrarySurface)
            {
                QTabVoyage = _qLibrary.QLibraryVoyageRefine
            },
            new QTab("Phonology", QNavigationPhonology, QPhonologySurface)
            {
                QTabVoyage = _qPhonology.QPhonologyVoyageRefine
            },
            new QTab("Xiesheng", QNavigationXiesheng, QXieshengSurface)
            {
                QTabVoyage = _qXiesheng.QXieshengVoyageRefine
            },
            new QTab("Yunjing", QNavigationYunjing, QYunjingSurface)
            {
                QTabVoyage = _qYunjing.QYunjingVoyageRefine
            },
            new QTab("Taxonomy", QNavigationTaxonomy, QTaxonomySurface)
            {
                QTabVoyage = _qTaxonomy.QTaxonomyVoyageRefine
            },
            new QTab("Tenor", QNavigationTenor, QTenorSurface)
            {
                QTabVoyage = _qTenor.QTenorVoyageRefine
            },
            new QTab("Repertoire", QNavigationRepertoire, QRepertoireSurface)
            {
                QTabVoyage = _qRepertoire.QRepertoireVoyageRefine
            },
            new QTab("Corpus", QNavigationCorpus, QCorpusSurface)
            {
                QTabVoyage = _qCorpus.QCorpusVoyageShow
            },
            new QTab("Reference", QNavigationSource, QReferenceSurface)
            {
                QTabVoyage = _qReference.QReferenceVoyageRefine
            },
            new QTab("Guild", QNavigationGuild, QGuildSurface)
            {
                QTabVoyage = _qGuild.QGuildVoyageShow
            },
            new QTab("Favorite", QNavigationFavorite, QFavoriteSurface)
            {
                QTabVoyage = _qFavorite.QFavoriteVoyageShow
            },
            new QTab("Duplex", QNavigationDuplex, QDuplexSurface),
            new QTab("Settings", QNavigationSettings, QSettingsSurface)
        ];
    }

    private void QVoyageRefine(CVoyageState state)
    {
        foreach (QTab tab in _qNavigationTabs)
        {
            tab.QTabVoyage?.Invoke(state.CVoyageStatePast, state.CVoyageStateFuture);
        }
    }

    private void QVoyageKeyObserve(object sender, KeyEventArgs e)
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

    private void QVoyageMouseObserve(object sender, MouseButtonEventArgs e)
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

    internal void QWindowMentionRefine(PMention anchor, CMentionOffer? offer)
    {
        ArgumentNullException.ThrowIfNull(anchor);

        if (offer is null)
        {
            return;
        }

        QMentionOfferRefine(anchor, anchor.PMentionPieceRead(offer.CMentionOfferOffset), offer);
    }

    private void QMentionSenseRefine(long sense)
    {
        _qLibrary.QLibraryLectern.QLecternCard.QLecternSpotlightRefine(sense);
    }

    private void QMentionLeaveRefine(object? sender, EventArgs e)
    {
        QMentionMenuHide();
    }
}
