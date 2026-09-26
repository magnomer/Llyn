using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public partial class PTaxonomy : UserControl
{
    private PWindow _pTaxonomyHost = null!;

    private LTaxonomy _lTaxonomy = null!;

    private LEditor _lEditor = null!;

    public PTaxonomy()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Taxonomy/PTaxonomy.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PTaxonomyPressHandle, PTaxonomyPressCheck));
        CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, PTaxonomyPortraitHandle, PTaxonomyPressCheck));
        PTaxonomyPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        PTaxonomyPress.Command = ApplicationCommands.Print;

        PChoice.PChoiceDropperAttach(PFunnelDropper, PFunnelDropdown, PFunnel);
        PChoice.PChoiceDropperAttach(PLatticeDropper, PLatticeDropdown, PLatticeDropper);

        PFunnelIcon.PIconSource = PIcon.PIconResolve("sort", 24);
        PLatticeIcon.PIconSource = PIcon.PIconResolve("filter", 24);
        PTaxonomyBinIcon.PIconSource = PIcon.PIconResolve("delete", 24);
        PTaxonomyFresh.Tag = PIcon.PIconResolve("new", 24);
        PTaxonomyStore.Tag = PIcon.PIconResolve("save", 24);
        PTaxonomyEarlier.Tag = PIcon.PIconResolve("retreat", 24);
        PTaxonomyLater.Tag = PIcon.PIconResolve("advance", 24);
        PTaxonomyBackward.Tag = PIcon.PIconResolve("undo", 24);
        PTaxonomyForward.Tag = PIcon.PIconResolve("redo", 24);
        PTaxonomyPortrait.Tag = PIcon.PIconResolve("export", 24);
        PTaxonomyPress.Tag = PIcon.PIconResolve("print", 24);
        PTaxonomyViewer.Tag = PIcon.PIconResolve("view", 24);
        PTaxonomyScribe.Tag = PIcon.PIconResolve("edit", 24);

        PExploration.TextChanged += PExplorationHandle;
        PScout.TextChanged += PScoutHandle;
        PTaxonomyFresh.Click += PTaxonomyFreshHandle;
        PTaxonomyStore.Click += PTaxonomyStoreHandle;
        PTaxonomyEarlier.Click += PTaxonomyRetreatHandle;
        PTaxonomyLater.Click += PTaxonomyAdvanceHandle;
        PTaxonomyBackward.Click += PTaxonomyUndoHandle;
        PTaxonomyForward.Click += PTaxonomyRedoHandle;
        PTaxonomyViewer.Click += PTaxonomyScribeHandle;
        PTaxonomyScribe.Click += PTaxonomyScribeHandle;
        PTaxonomyBin.Click += PTaxonomyBinHandle;
    }

    private Border PFunnel => (Border)FindName(nameof(PFunnel));

    private ToggleButton PFunnelDropper => (ToggleButton)FindName(nameof(PFunnelDropper));

    private PIconImage PFunnelIcon => (PIconImage)FindName(nameof(PFunnelIcon));

    private Popup PFunnelDropdown => (Popup)FindName(nameof(PFunnelDropdown));

    private StackPanel PFunnelList => (StackPanel)FindName(nameof(PFunnelList));

    private TextBox PExploration => (TextBox)FindName(nameof(PExploration));

    private ItemsControl PDirectory => (ItemsControl)FindName(nameof(PDirectory));

    private TextBlock PDirectoryEmpty => (TextBlock)FindName(nameof(PDirectoryEmpty));

    private ItemsControl PMembership => (ItemsControl)FindName(nameof(PMembership));

    private TextBlock PMembershipEmpty => (TextBlock)FindName(nameof(PMembershipEmpty));

    private TextBox PScout => (TextBox)FindName(nameof(PScout));

    private ToggleButton PLatticeDropper => (ToggleButton)FindName(nameof(PLatticeDropper));

    private PIconImage PLatticeIcon => (PIconImage)FindName(nameof(PLatticeIcon));

    private FrameworkElement PLatticeMark => (FrameworkElement)FindName(nameof(PLatticeMark));

    private Popup PLatticeDropdown => (Popup)FindName(nameof(PLatticeDropdown));

    private StackPanel PLatticeList => (StackPanel)FindName(nameof(PLatticeList));

    private Button PTaxonomyFresh => (Button)FindName(nameof(PTaxonomyFresh));

    private Button PTaxonomyStore => (Button)FindName(nameof(PTaxonomyStore));

    private StackPanel PTaxonomyVoyage => (StackPanel)FindName(nameof(PTaxonomyVoyage));

    private Button PTaxonomyEarlier => (Button)FindName(nameof(PTaxonomyEarlier));

    private Button PTaxonomyLater => (Button)FindName(nameof(PTaxonomyLater));

    private StackPanel PTaxonomyChronicle => (StackPanel)FindName(nameof(PTaxonomyChronicle));

    private Button PTaxonomyBackward => (Button)FindName(nameof(PTaxonomyBackward));

    private Button PTaxonomyForward => (Button)FindName(nameof(PTaxonomyForward));

    private Button PTaxonomyPortrait => (Button)FindName(nameof(PTaxonomyPortrait));

    private Button PTaxonomyPress => (Button)FindName(nameof(PTaxonomyPress));

    private Border PTaxonomyMode => (Border)FindName(nameof(PTaxonomyMode));

    private RadioButton PTaxonomyViewer => (RadioButton)FindName(nameof(PTaxonomyViewer));

    private RadioButton PTaxonomyScribe => (RadioButton)FindName(nameof(PTaxonomyScribe));

    private PDisplay PDisplay => (PDisplay)FindName(nameof(PDisplay));

    private PEditor PEditor => (PEditor)FindName(nameof(PEditor));

    private Button PTaxonomyBin => (Button)FindName(nameof(PTaxonomyBin));

    private PIconImage PTaxonomyBinIcon => (PIconImage)FindName(nameof(PTaxonomyBinIcon));

    internal void PTaxonomyAttach(PWindow host)
    {
        _pTaxonomyHost = host;
        _lEditor = host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm);
        _lTaxonomy = host.PWindowDeportment.LWindowTaxonomyCreate(
            _lEditor, PTaxonomyShownCheck, PTaxonomyDiscardConfirm, host.PWindowDeleteConfirm);
        LPanel panel = _lTaxonomy.LTaxonomyPanel;
        panel.LPanelChanged += PTaxonomyModeUpdate;
        panel.LPanelRowsChanged += PMembershipFind;
        panel.LPanelFailed += host.PWindowFailureShow;

        PDirectory.ItemsSource = _pDirectoryList;
        PMembership.ItemsSource = _pMembershipList;
        PLookItem.PLookItemAttach(PDirectory, PDirectoryApply);
        PLookItem.PLookItemAttach(PMembership, PMembershipApply);

        PDisplay.PDisplayAttach(host, _lEditor.LEditorLectern);
        _lEditor.LEditorStateChanged += PTaxonomyStoreUpdate;
        PEditor.PEditorAttach(host, _lEditor);
        PEditor.PEditorChronicleChanged += PTaxonomyChronicleUpdate;
    }

    private void PTaxonomyStoreUpdate()
    {
        PTaxonomyStore.IsEnabled = _lEditor.LEditorStorable;
    }

    internal void PTaxonomyReset()
    {
        _lTaxonomy.LTaxonomyPanel.LPanelClear();
        PDirectoryReset();
        PDirectoryFind();
    }

    internal bool PTaxonomyDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PTaxonomyChangeCheck()
    {
        return _lTaxonomy.LTaxonomyPanel.LPanelChangeCheck();
    }

    internal void PTaxonomyClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }

    private void PTaxonomyPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lTaxonomy?.LTaxonomyPanel.LPanelPressAllowed ?? false;
    }

    private async void PTaxonomyPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pTaxonomyHost.PWindowPressRun(_lTaxonomy.LTaxonomyPortraitPrint);
    }

    private async void PTaxonomyPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pTaxonomyHost.PWindowPortraitExport(_lTaxonomy.LTaxonomyFileRead(), _lTaxonomy.LTaxonomyPortraitExport);
    }

    internal void PTaxonomyVoyageShow(bool past, bool future)
    {
        PTaxonomyEarlier.IsEnabled = past;
        PTaxonomyLater.IsEnabled = future;
    }

    private void PTaxonomyRetreatHandle(object sender, RoutedEventArgs e)
    {
        _pTaxonomyHost.PVoyageRetreatRun();
    }

    private void PTaxonomyAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _pTaxonomyHost.PVoyageAdvanceRun();
    }

    private void PTaxonomyUndoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleUndo();
    }

    private void PTaxonomyRedoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleRedo();
    }

    private void PTaxonomyChronicleUpdate()
    {
        (bool undo, bool redo) = PEditor.PEditorChronicleRead();
        PTaxonomyBackward.IsEnabled = undo;
        PTaxonomyForward.IsEnabled = redo;
    }

    private bool PTaxonomyShownCheck()
    {
        return IsVisible;
    }

    private bool PTaxonomyDiscardConfirm()
    {
        return _pTaxonomyHost.PWindowDiscardConfirm(true, PEditor.PEditorDraftFinish);
    }

    private void PTaxonomyModeUpdate()
    {
        LPanel panel = _lTaxonomy.LTaxonomyPanel;
        PEditor.Visibility = PLook.PLookVisibleRead(panel.LPanelEditing);
        PDisplay.Visibility = PLook.PLookVisibleRead(panel.LPanelViewerChecked);
        PTaxonomyViewer.IsChecked = PLook.PLookCheckedRead(panel.LPanelViewerChecked);
        PTaxonomyScribe.IsChecked = PLook.PLookCheckedRead(panel.LPanelScribeChecked);
        PTaxonomyVoyage.Visibility = PLook.PLookVisibleRead(panel.LPanelViewerChecked);
        PTaxonomyChronicle.Visibility = PLook.PLookVisibleRead(panel.LPanelScribeChecked);
        PTaxonomyMode.IsEnabled = panel.LPanelModeEnabled;
        PTaxonomyBin.IsEnabled = panel.LPanelBinEnabled;
    }
}
