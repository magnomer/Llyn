using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QNavigation
{
    private readonly Window _qNavigationSurface;

    private readonly CNavigation _cNavigation;

    private readonly QTab[] _qNavigationTabs;

    internal QNavigation(Window surface, CNavigation navigation)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(navigation);

        _qNavigationSurface = surface;
        _cNavigation = navigation;
        _qNavigationTabs = QNavigationTabRead();

        QNavigationIconRefine();
        QNavigationIntroduce();
    }

    private Button QNavigationInput => (Button)_qNavigationSurface.FindName("PNavigationInput");

    private Button QNavigationLibrary => (Button)_qNavigationSurface.FindName("PNavigationLibrary");

    private Button QNavigationFavorite => (Button)_qNavigationSurface.FindName("PNavigationFavorite");

    private Button QNavigationPhonology => (Button)_qNavigationSurface.FindName("PNavigationPhonology");

    private Button QNavigationRepertoire => (Button)_qNavigationSurface.FindName("PNavigationRepertoire");

    private Button QNavigationTenor => (Button)_qNavigationSurface.FindName("PNavigationTenor");

    private Button QNavigationTaxonomy => (Button)_qNavigationSurface.FindName("PNavigationTaxonomy");

    private Button QNavigationCorpus => (Button)_qNavigationSurface.FindName("PNavigationCorpus");

    private Button QNavigationSource => (Button)_qNavigationSurface.FindName("PNavigationSource");

    private Button QNavigationGuild => (Button)_qNavigationSurface.FindName("PNavigationGuild");

    private Button QNavigationXiesheng => (Button)_qNavigationSurface.FindName("PNavigationXiesheng");

    private Button QNavigationYunjing => (Button)_qNavigationSurface.FindName("PNavigationYunjing");

    private Button QNavigationDuplex => (Button)_qNavigationSurface.FindName("PNavigationDuplex");

    private Button QNavigationSettings => (Button)_qNavigationSurface.FindName("PNavigationSettings");

    private void QNavigationObserve(object sender, RoutedEventArgs e)
    {
        if (QNavigationFind(sender) is QTab chosen)
        {
            _cNavigation.CNavigationTabSelect(chosen.QTabMode);
        }
    }

    private void QNavigationIntroduce()
    {
        foreach (QTab tab in _qNavigationTabs)
        {
            QNavigationChosenRefine(tab);
        }

        QNavigationInput.Click += QNavigationObserve;
        QNavigationLibrary.Click += QNavigationObserve;
        QNavigationFavorite.Click += QNavigationObserve;
        QNavigationPhonology.Click += QNavigationObserve;
        QNavigationRepertoire.Click += QNavigationObserve;
        QNavigationTenor.Click += QNavigationObserve;
        QNavigationTaxonomy.Click += QNavigationObserve;
        QNavigationCorpus.Click += QNavigationObserve;
        QNavigationSource.Click += QNavigationObserve;
        QNavigationGuild.Click += QNavigationObserve;
        QNavigationXiesheng.Click += QNavigationObserve;
        QNavigationYunjing.Click += QNavigationObserve;
        QNavigationDuplex.Click += QNavigationObserve;
        QNavigationSettings.Click += QNavigationObserve;

        _cNavigation.CNavigationChanged += QNavigationRefine;
        _qNavigationSurface.PreviewKeyDown += QVoyageKeyObserve;
        _qNavigationSurface.PreviewMouseDown += QVoyageMouseObserve;
    }

    private void QNavigationIconRefine()
    {
        QNavigationInput.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("input", 24));
        QNavigationLibrary.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("library", 24));
        QNavigationFavorite.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("favorite", 24));
        QNavigationPhonology.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("phonology", 24));
        QNavigationRepertoire.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("repertoire", 24));
        QNavigationTenor.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("tenor", 24));
        QNavigationTaxonomy.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("taxonomy", 24));
        QNavigationCorpus.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("corpus", 24));
        QNavigationSource.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("reference", 24));
        QNavigationGuild.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("guild", 24));
        QNavigationXiesheng.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("xiesheng", 24));
        QNavigationYunjing.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("yunjing", 24));
        QNavigationDuplex.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("duplex", 24));
        QNavigationSettings.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("settings", 24));
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
            new QTab("Input", QNavigationInput, QContract.QContractFind<UserControl>(_qNavigationSurface, "PInput")),
            new QTab(
                "Library", QNavigationLibrary, QContract.QContractFind<UserControl>(_qNavigationSurface, "PLibrary")),
            new QTab(
                "Phonology",
                QNavigationPhonology,
                QContract.QContractFind<UserControl>(_qNavigationSurface, "PPhonology")),
            new QTab(
                "Xiesheng",
                QNavigationXiesheng,
                QContract.QContractFind<UserControl>(_qNavigationSurface, "PXiesheng")),
            new QTab(
                "Yunjing", QNavigationYunjing, QContract.QContractFind<UserControl>(_qNavigationSurface, "PYunjing")),
            new QTab(
                "Taxonomy",
                QNavigationTaxonomy,
                QContract.QContractFind<UserControl>(_qNavigationSurface, "PTaxonomy")),
            new QTab("Tenor", QNavigationTenor, QContract.QContractFind<UserControl>(_qNavigationSurface, "PTenor")),
            new QTab(
                "Repertoire",
                QNavigationRepertoire,
                QContract.QContractFind<UserControl>(_qNavigationSurface, "PRepertoire")),
            new QTab("Corpus", QNavigationCorpus, QContract.QContractFind<UserControl>(_qNavigationSurface, "PCorpus")),
            new QTab(
                "Reference",
                QNavigationSource,
                QContract.QContractFind<UserControl>(_qNavigationSurface, "PReference")),
            new QTab("Guild", QNavigationGuild, QContract.QContractFind<UserControl>(_qNavigationSurface, "PGuild")),
            new QTab(
                "Favorite",
                QNavigationFavorite,
                QContract.QContractFind<UserControl>(_qNavigationSurface, "PFavorite")),
            new QTab("Duplex", QNavigationDuplex, QContract.QContractFind<UserControl>(_qNavigationSurface, "PDuplex")),
            new QTab(
                "Settings", QNavigationSettings, QContract.QContractFind<UserControl>(_qNavigationSurface, "PSettings"))
        ];
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
}
