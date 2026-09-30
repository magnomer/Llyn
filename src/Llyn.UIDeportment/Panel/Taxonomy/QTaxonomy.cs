using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QTaxonomy
{
    private readonly UserControl _qTaxonomySurface;

    private PWindow _qTaxonomyHost = null!;

    private CTaxonomy _cTaxonomy = null!;

    internal QTaxonomy(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qTaxonomySurface = surface;

        surface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QTaxonomyPressObserve, QTaxonomyPressRefine));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QTaxonomyPortraitObserve, QTaxonomyPressRefine));
        QTaxonomyPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        QTaxonomyPress.Command = ApplicationCommands.Print;

        QChoice.QChoiceDropperAttach(QFunnelDropper, QFunnelDropdown, QFunnel);
        QChoice.QChoiceDropperAttach(QLatticeDropper, QLatticeDropdown, QLatticeDropper);

        QFunnelIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        QLatticeIcon.QIconSource = QIcon.QIconResolve("filter", 24);
        QTaxonomyBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        QExploration.SetResourceReference(QField.QFieldHintProperty, "Tag.Search");
        QScout.SetResourceReference(QField.QFieldHintProperty, "Scout.Search");
        QTaxonomyFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        QTaxonomyStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        QTaxonomyEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        QTaxonomyLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        QTaxonomyBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        QTaxonomyForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        QTaxonomyPortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        QTaxonomyPress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        QTaxonomyViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        QTaxonomyScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        QExploration.TextChanged += QExplorationObserve;
        QScout.TextChanged += QScoutObserve;
        QTaxonomyFresh.Click += QTaxonomyFreshObserve;
        QTaxonomyStore.Click += QTaxonomyStoreObserve;
        QTaxonomyEarlier.Click += QTaxonomyRetreatObserve;
        QTaxonomyLater.Click += QTaxonomyAdvanceObserve;
        QTaxonomyBackward.Click += QTaxonomyUndoObserve;
        QTaxonomyForward.Click += QTaxonomyRedoObserve;
        QTaxonomyViewer.Click += QTaxonomyViewerObserve;
        QTaxonomyScribe.Click += QTaxonomyScribeObserve;
        QTaxonomyBin.Click += QTaxonomyBinObserve;
    }

    private Border QFunnel => QContract.QContractFind<Border>(_qTaxonomySurface, "PFunnel");

    private ToggleButton QFunnelDropper => QContract.QContractFind<ToggleButton>(_qTaxonomySurface, "PFunnelDropper");

    private QIconImage QFunnelIcon => QContract.QContractFind<QIconImage>(_qTaxonomySurface, "PFunnelIcon");

    private Popup QFunnelDropdown => QContract.QContractFind<Popup>(_qTaxonomySurface, "PFunnelDropdown");

    private StackPanel QFunnelList => QContract.QContractFind<StackPanel>(_qTaxonomySurface, "PFunnelList");

    private TextBox QExploration => QContract.QContractFind<TextBox>(_qTaxonomySurface, "PExploration");

    private ItemsControl QDirectory => QContract.QContractFind<ItemsControl>(_qTaxonomySurface, "PDirectory");

    private TextBlock QDirectoryEmpty => QContract.QContractFind<TextBlock>(_qTaxonomySurface, "PDirectoryEmpty");

    private ItemsControl QMembership => QContract.QContractFind<ItemsControl>(_qTaxonomySurface, "PMembership");

    private TextBlock QMembershipEmpty => QContract.QContractFind<TextBlock>(_qTaxonomySurface, "PMembershipEmpty");

    private TextBox QScout => QContract.QContractFind<TextBox>(_qTaxonomySurface, "PScout");

    private ToggleButton QLatticeDropper =>
        QContract.QContractFind<ToggleButton>(_qTaxonomySurface, "PLatticeDropper");

    private QIconImage QLatticeIcon => QContract.QContractFind<QIconImage>(_qTaxonomySurface, "PLatticeIcon");

    private FrameworkElement QLatticeMark =>
        QContract.QContractFind<FrameworkElement>(_qTaxonomySurface, "PLatticeMark");

    private Popup QLatticeDropdown => QContract.QContractFind<Popup>(_qTaxonomySurface, "PLatticeDropdown");

    private StackPanel QLatticeList => QContract.QContractFind<StackPanel>(_qTaxonomySurface, "PLatticeList");

    private Button QTaxonomyFresh => QContract.QContractFind<Button>(_qTaxonomySurface, "PTaxonomyFresh");

    private Button QTaxonomyStore => QContract.QContractFind<Button>(_qTaxonomySurface, "PTaxonomyStore");

    private StackPanel QTaxonomyVoyage => QContract.QContractFind<StackPanel>(_qTaxonomySurface, "PTaxonomyVoyage");

    private Button QTaxonomyEarlier => QContract.QContractFind<Button>(_qTaxonomySurface, "PTaxonomyEarlier");

    private Button QTaxonomyLater => QContract.QContractFind<Button>(_qTaxonomySurface, "PTaxonomyLater");

    private StackPanel QTaxonomyChronicle =>
        QContract.QContractFind<StackPanel>(_qTaxonomySurface, "PTaxonomyChronicle");

    private Button QTaxonomyBackward => QContract.QContractFind<Button>(_qTaxonomySurface, "PTaxonomyBackward");

    private Button QTaxonomyForward => QContract.QContractFind<Button>(_qTaxonomySurface, "PTaxonomyForward");

    private Button QTaxonomyPortrait => QContract.QContractFind<Button>(_qTaxonomySurface, "PTaxonomyPortrait");

    private Button QTaxonomyPress => QContract.QContractFind<Button>(_qTaxonomySurface, "PTaxonomyPress");

    private Border QTaxonomyMode => QContract.QContractFind<Border>(_qTaxonomySurface, "PTaxonomyMode");

    private RadioButton QTaxonomyViewer => QContract.QContractFind<RadioButton>(_qTaxonomySurface, "PTaxonomyViewer");

    private RadioButton QTaxonomyScribe => QContract.QContractFind<RadioButton>(_qTaxonomySurface, "PTaxonomyScribe");

    private PDisplay QTaxonomyDisplay => QContract.QContractFind<PDisplay>(_qTaxonomySurface, "PDisplay");

    private PEditor QTaxonomyEditor => QContract.QContractFind<PEditor>(_qTaxonomySurface, "PEditor");

    private Button QTaxonomyBin => QContract.QContractFind<Button>(_qTaxonomySurface, "PTaxonomyBin");

    private QIconImage QTaxonomyBinIcon => QContract.QContractFind<QIconImage>(_qTaxonomySurface, "PTaxonomyBinIcon");

    internal void QTaxonomyIntroduce(PWindow host)
    {
        _qTaxonomyHost = host;
        _cTaxonomy = CTaxonomy.CTaxonomyCreate(
            host.PWindowAtelier,
            QTaxonomyShownCheck,
            host.PWindowEnvoy,
            LObserver.LObserverCreate<Action>(static run => run()));
        _cTaxonomy.CTaxonomyTagOpened += QDirectoryTagRefine;
        _cTaxonomy.CTaxonomyRowsChanged += QDirectoryRefine;
        _cTaxonomy.CTaxonomyWorkspaceChanged += QTaxonomyWorkspaceRefine;
        CPanel panel = _cTaxonomy.CTaxonomyMembership.CMembershipPanel;
        QLectern lectern = new(_cTaxonomy.CTaxonomyEditor.CEditorDisplay, panel);
        panel.CPanelChanged += QTaxonomyModeRefine;
        panel.CPanelRowsChanged += QMembershipRefine;

        QDirectory.ItemsSource = _qDirectoryList;
        QMembership.ItemsSource = _qMembershipList;
        QLookItem.QLookItemAttach(QDirectory, QDirectoryItemRefine);
        QLookItem.QLookItemAttach(QMembership, QMembershipItemRefine);

        QTaxonomyDisplay.PDisplayAttach(host, lectern);
        _cTaxonomy.CTaxonomyEditor.CEditorDesk.CDeskStateChanged += QTaxonomyStoreRefine;
        QTaxonomyEditor.PEditorIntroduce(host, new QEditor(_cTaxonomy.CTaxonomyEditor));
        QTaxonomyEditor.PEditorChronicleChanged += QTaxonomyChronicleRefine;
    }

    private void QTaxonomyStoreRefine()
    {
        QTaxonomyStore.IsEnabled = _cTaxonomy.CTaxonomyEditor.CEditorDesk.CDeskStorable;
    }

    internal void QTaxonomyExitRefine()
    {
        QTaxonomyEditor.PEditorPlayerRefine();
    }

    private void QTaxonomyPressRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cTaxonomy?.CTaxonomyMembership.CMembershipPanel.CPanelPressAllowed ?? false;
    }

    private async void QTaxonomyPressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cTaxonomy.CTaxonomyMembership.CMembershipPortraitPrint();
    }

    private async void QTaxonomyPortraitObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cTaxonomy.CTaxonomyMembership.CMembershipPortraitExport();
    }

    internal void QTaxonomyVoyageRefine(bool past, bool future)
    {
        QTaxonomyEarlier.IsEnabled = past;
        QTaxonomyLater.IsEnabled = future;
    }

    private void QTaxonomyRetreatObserve(object sender, RoutedEventArgs e)
    {
        _qTaxonomyHost.PWindowAtelier.CAtelierNavigation.CNavigationStationUndo();
    }

    private void QTaxonomyAdvanceObserve(object sender, RoutedEventArgs e)
    {
        _qTaxonomyHost.PWindowAtelier.CAtelierNavigation.CNavigationStationRedo();
    }

    private void QTaxonomyUndoObserve(object sender, RoutedEventArgs e)
    {
        QTaxonomyEditor.QChronicleUndoObserve();
    }

    private void QTaxonomyRedoObserve(object sender, RoutedEventArgs e)
    {
        QTaxonomyEditor.QChronicleRedoObserve();
    }

    private void QTaxonomyChronicleRefine()
    {
        (bool undo, bool redo) = _cTaxonomy.CTaxonomyEditor.CEditorDesk.CDeskChronicleRead();
        QTaxonomyBackward.IsEnabled = undo;
        QTaxonomyForward.IsEnabled = redo;
    }

    private bool QTaxonomyShownCheck()
    {
        return _qTaxonomySurface.IsVisible;
    }

    private void QTaxonomyModeRefine()
    {
        CPanel panel = _cTaxonomy.CTaxonomyMembership.CMembershipPanel;
        QTaxonomyEditor.Visibility = QLook.QLookVisibleRead(panel.CPanelEditing);
        QTaxonomyDisplay.Visibility = QLook.QLookVisibleRead(panel.CPanelViewerChecked);
        QTaxonomyViewer.IsChecked = panel.CPanelViewerChecked;
        QTaxonomyScribe.IsChecked = panel.CPanelScribeChecked;
        QTaxonomyVoyage.Visibility = QLook.QLookVisibleRead(panel.CPanelViewerChecked);
        QTaxonomyChronicle.Visibility = QLook.QLookVisibleRead(panel.CPanelScribeChecked);
        QTaxonomyMode.IsEnabled = panel.CPanelModeEnabled;
        QTaxonomyBin.IsEnabled = panel.CPanelBinEnabled;
    }
}
