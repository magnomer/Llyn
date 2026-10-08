using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QPanelRail
{
    private readonly UserControl _qPanelRailScope;

    private readonly Button _qPanelRailBin;

    private CNavigation _qPanelRailNavigation = null!;

    private QChronicleHost _qPanelRailHost = null!;

    internal QPanelRail(UserControl rail, Button bin, QIconImage binIcon, bool fresh, bool portrait)
    {
        ArgumentNullException.ThrowIfNull(rail);
        ArgumentNullException.ThrowIfNull(bin);
        ArgumentNullException.ThrowIfNull(binIcon);

        _qPanelRailScope = rail;
        _qPanelRailBin = bin;

        QPanelRailPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        QPanelRailPress.Command = ApplicationCommands.Print;

        binIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        QPanelRailFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        QPanelRailStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        QPanelRailEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        QPanelRailLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        QPanelRailBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        QPanelRailForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        QPanelRailPortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        QPanelRailPress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        QPanelRailViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        QPanelRailScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        QPanelRailFresh.Visibility = QLook.QLookVisibleRead(fresh);
        QPanelRailPortrait.Visibility = QLook.QLookVisibleRead(portrait);

        QPanelRailFresh.Click += QEntryCreateObserve;
        QPanelRailStore.Click += QEntryStoreObserve;
        QPanelRailEarlier.Click += QVoyageRetreatObserve;
        QPanelRailLater.Click += QVoyageAdvanceObserve;
        QPanelRailBackward.Click += QChronicleBackwardObserve;
        QPanelRailForward.Click += QChronicleForwardObserve;
        QPanelRailViewer.Click += QEntryViewerObserve;
        QPanelRailScribe.Click += QEntryScribeObserve;
        _qPanelRailBin.Click += QEntryDeleteObserve;
    }

    internal event Action? QPanelRailCreated;

    internal event Action? QPanelRailStored;

    internal event Action<bool>? QPanelRailToggled;

    internal event Action? QPanelRailDeleted;

    private Button QPanelRailFresh => QContract.QContractFind<Button>(_qPanelRailScope, "PPanelRailFresh");

    private Button QPanelRailStore => QContract.QContractFind<Button>(_qPanelRailScope, "PPanelRailStore");

    private StackPanel QPanelRailVoyage => QContract.QContractFind<StackPanel>(_qPanelRailScope, "PPanelRailVoyage");

    private Button QPanelRailEarlier => QContract.QContractFind<Button>(_qPanelRailScope, "PPanelRailEarlier");

    private Button QPanelRailLater => QContract.QContractFind<Button>(_qPanelRailScope, "PPanelRailLater");

    private StackPanel QPanelRailChronicle =>
        QContract.QContractFind<StackPanel>(_qPanelRailScope, "PPanelRailChronicle");

    private Button QPanelRailBackward => QContract.QContractFind<Button>(_qPanelRailScope, "PPanelRailBackward");

    private Button QPanelRailForward => QContract.QContractFind<Button>(_qPanelRailScope, "PPanelRailForward");

    private Button QPanelRailPortrait => QContract.QContractFind<Button>(_qPanelRailScope, "PPanelRailPortrait");

    private Button QPanelRailPress => QContract.QContractFind<Button>(_qPanelRailScope, "PPanelRailPress");

    private Border QPanelRailMode => QContract.QContractFind<Border>(_qPanelRailScope, "PPanelRailMode");

    private RadioButton QPanelRailViewer => QContract.QContractFind<RadioButton>(_qPanelRailScope, "PPanelRailViewer");

    private RadioButton QPanelRailScribe => QContract.QContractFind<RadioButton>(_qPanelRailScope, "PPanelRailScribe");

    internal void QPanelRailIntroduce(CNavigation navigation, QChronicleHost host)
    {
        ArgumentNullException.ThrowIfNull(navigation);
        ArgumentNullException.ThrowIfNull(host);

        _qPanelRailNavigation = navigation;
        _qPanelRailHost = host;
        _qPanelRailNavigation.CNavigationChanged += QVoyageRefine;
    }

    internal void QPanelRailRefine(bool scribe, bool modeEnabled, bool binEnabled)
    {
        QPanelRailViewer.IsChecked = !scribe;
        QPanelRailScribe.IsChecked = scribe;
        QPanelRailVoyage.Visibility = QLook.QLookVisibleRead(!scribe);
        QPanelRailChronicle.Visibility = QLook.QLookVisibleRead(scribe);
        QPanelRailMode.IsEnabled = modeEnabled;
        _qPanelRailBin.IsEnabled = binEnabled;
    }

    internal void QEntryStorableRefine(bool storable)
    {
        QPanelRailStore.IsEnabled = storable;
    }

    internal void QChronicleRefine(bool undo, bool redo)
    {
        QPanelRailBackward.IsEnabled = undo;
        QPanelRailForward.IsEnabled = redo;
    }

    private void QVoyageRefine(CNavigationState state)
    {
        QPanelRailEarlier.IsEnabled = state.CNavigationStateVoyage.CVoyageStatePast;
        QPanelRailLater.IsEnabled = state.CNavigationStateVoyage.CVoyageStateFuture;
    }

    private void QVoyageRetreatObserve(object sender, RoutedEventArgs e)
    {
        _qPanelRailNavigation.CNavigationStationUndo();
    }

    private void QVoyageAdvanceObserve(object sender, RoutedEventArgs e)
    {
        _qPanelRailNavigation.CNavigationStationRedo();
    }

    private void QChronicleBackwardObserve(object sender, RoutedEventArgs e)
    {
        _qPanelRailHost.QChronicleUndoObserve();
    }

    private void QChronicleForwardObserve(object sender, RoutedEventArgs e)
    {
        _qPanelRailHost.QChronicleRedoObserve();
    }

    private void QEntryCreateObserve(object sender, RoutedEventArgs e)
    {
        QPanelRailCreated?.Invoke();
    }

    private void QEntryStoreObserve(object sender, RoutedEventArgs e)
    {
        QPanelRailStored?.Invoke();
    }

    private void QEntryViewerObserve(object sender, RoutedEventArgs e)
    {
        QPanelRailToggled?.Invoke(false);
    }

    private void QEntryScribeObserve(object sender, RoutedEventArgs e)
    {
        QPanelRailToggled?.Invoke(true);
    }

    private void QEntryDeleteObserve(object sender, RoutedEventArgs e)
    {
        QPanelRailDeleted?.Invoke();
    }
}
