using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public partial class PFavorite : UserControl
{
    private PWindow _pFavoriteHost = null!;

    private LFavorite _lFavorite = null!;

    private LEditor _lEditor = null!;

    public PFavorite()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Favorite/PFavorite.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PFavoritePressHandle, PFavoritePressCheck));
        CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, PFavoritePortraitHandle, PFavoritePressCheck));
        PFavoritePortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        PFavoritePress.Command = ApplicationCommands.Print;

        PChoice.PChoiceDropperAttach(PSeriesDropper, PSeriesDropdown, PSeries);
        PChoice.PChoiceDropperAttach(PStrainerDropper, PStrainerDropdown, PStrainerDropper);

        PSeriesIcon.PIconSource = PIcon.PIconResolve("sort", 24);
        PStrainerIcon.PIconSource = PIcon.PIconResolve("filter", 24);
        PFavoriteBinIcon.PIconSource = PIcon.PIconResolve("delete", 24);
        PFavoriteStore.Tag = PIcon.PIconResolve("save", 24);
        PFavoriteEarlier.Tag = PIcon.PIconResolve("retreat", 24);
        PFavoriteLater.Tag = PIcon.PIconResolve("advance", 24);
        PFavoriteBackward.Tag = PIcon.PIconResolve("undo", 24);
        PFavoriteForward.Tag = PIcon.PIconResolve("redo", 24);
        PFavoritePortrait.Tag = PIcon.PIconResolve("export", 24);
        PFavoritePress.Tag = PIcon.PIconResolve("print", 24);
        PFavoriteViewer.Tag = PIcon.PIconResolve("view", 24);
        PFavoriteScribe.Tag = PIcon.PIconResolve("edit", 24);

        PRecall.TextChanged += PRecallHandle;
        PFavoriteStore.Click += PFavoriteStoreHandle;
        PFavoriteEarlier.Click += PFavoriteRetreatHandle;
        PFavoriteLater.Click += PFavoriteAdvanceHandle;
        PFavoriteBackward.Click += PFavoriteUndoHandle;
        PFavoriteForward.Click += PFavoriteRedoHandle;
        PFavoriteViewer.Click += PFavoriteScribeHandle;
        PFavoriteScribe.Click += PFavoriteScribeHandle;
        PFavoriteBin.Click += PFavoriteBinHandle;
    }

    private Border PSeries => (Border)FindName(nameof(PSeries));

    private ToggleButton PSeriesDropper => (ToggleButton)FindName(nameof(PSeriesDropper));

    private PIconImage PSeriesIcon => (PIconImage)FindName(nameof(PSeriesIcon));

    private Popup PSeriesDropdown => (Popup)FindName(nameof(PSeriesDropdown));

    private StackPanel PSeriesList => (StackPanel)FindName(nameof(PSeriesList));

    private TextBox PRecall => (TextBox)FindName(nameof(PRecall));

    private ToggleButton PStrainerDropper => (ToggleButton)FindName(nameof(PStrainerDropper));

    private PIconImage PStrainerIcon => (PIconImage)FindName(nameof(PStrainerIcon));

    private FrameworkElement PStrainerMark => (FrameworkElement)FindName(nameof(PStrainerMark));

    private Popup PStrainerDropdown => (Popup)FindName(nameof(PStrainerDropdown));

    private StackPanel PStrainerList => (StackPanel)FindName(nameof(PStrainerList));

    private ItemsControl PRoster => (ItemsControl)FindName(nameof(PRoster));

    private TextBlock PRosterEmpty => (TextBlock)FindName(nameof(PRosterEmpty));

    private Button PFavoriteStore => (Button)FindName(nameof(PFavoriteStore));

    private StackPanel PFavoriteVoyage => (StackPanel)FindName(nameof(PFavoriteVoyage));

    private Button PFavoriteEarlier => (Button)FindName(nameof(PFavoriteEarlier));

    private Button PFavoriteLater => (Button)FindName(nameof(PFavoriteLater));

    private StackPanel PFavoriteChronicle => (StackPanel)FindName(nameof(PFavoriteChronicle));

    private Button PFavoriteBackward => (Button)FindName(nameof(PFavoriteBackward));

    private Button PFavoriteForward => (Button)FindName(nameof(PFavoriteForward));

    private Button PFavoritePortrait => (Button)FindName(nameof(PFavoritePortrait));

    private Button PFavoritePress => (Button)FindName(nameof(PFavoritePress));

    private Border PFavoriteMode => (Border)FindName(nameof(PFavoriteMode));

    private RadioButton PFavoriteViewer => (RadioButton)FindName(nameof(PFavoriteViewer));

    private RadioButton PFavoriteScribe => (RadioButton)FindName(nameof(PFavoriteScribe));

    private PDisplay PDisplay => (PDisplay)FindName(nameof(PDisplay));

    private PEditor PEditor => (PEditor)FindName(nameof(PEditor));

    private Button PFavoriteBin => (Button)FindName(nameof(PFavoriteBin));

    private PIconImage PFavoriteBinIcon => (PIconImage)FindName(nameof(PFavoriteBinIcon));

    internal void PFavoriteAttach(PWindow host)
    {
        _pFavoriteHost = host;
        _lEditor = host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm);
        _lFavorite = host.PWindowDeportment.LWindowFavoriteCreate(
            _lEditor, PFavoriteShownCheck, PFavoriteDiscardConfirm, host.PWindowDeleteConfirm);
        LPanel panel = _lFavorite.LFavoritePanel;
        panel.LPanelChanged += PFavoriteModeUpdate;
        panel.LPanelRowsChanged += PRosterFind;
        panel.LPanelFailed += host.PWindowFailureShow;

        PRoster.ItemsSource = _pRosterList;
        PLookItem.PLookItemAttach(PRoster, PRosterApply);

        PDisplay.PDisplayAttach(host, _lEditor.LEditorLectern);
        _lEditor.LEditorStateChanged += PFavoriteStoreUpdate;
        PEditor.PEditorAttach(host, _lEditor);
        PEditor.PEditorChronicleChanged += PFavoriteChronicleUpdate;
    }

    private void PFavoriteStoreUpdate()
    {
        PFavoriteStore.IsEnabled = _lEditor.LEditorStorable;
    }

    internal void PFavoriteReset()
    {
        _lFavorite.LFavoritePanel.LPanelClear();
    }

    internal bool PFavoriteDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PFavoriteChangeCheck()
    {
        return _lFavorite.LFavoritePanel.LPanelChangeCheck();
    }

    internal void PFavoriteClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }

    private void PFavoritePressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lFavorite?.LFavoritePanel.LPanelPressAllowed ?? false;
    }

    private async void PFavoritePressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pFavoriteHost.PWindowPressRun(_lFavorite.LFavoritePortraitPrint);
    }

    private async void PFavoritePortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pFavoriteHost.PWindowPortraitExport(_lFavorite.LFavoriteFileRead(), _lFavorite.LFavoritePortraitExport);
    }

    internal void PFavoriteVoyageShow(bool past, bool future)
    {
        PFavoriteEarlier.IsEnabled = past;
        PFavoriteLater.IsEnabled = future;
    }

    private void PFavoriteRetreatHandle(object sender, RoutedEventArgs e)
    {
        _pFavoriteHost.PVoyageRetreatRun();
    }

    private void PFavoriteAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _pFavoriteHost.PVoyageAdvanceRun();
    }

    private void PFavoriteUndoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleUndo();
    }

    private void PFavoriteRedoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleRedo();
    }

    private void PFavoriteChronicleUpdate()
    {
        (bool undo, bool redo) = PEditor.PEditorChronicleRead();
        PFavoriteBackward.IsEnabled = undo;
        PFavoriteForward.IsEnabled = redo;
    }

    private bool PFavoriteShownCheck()
    {
        return IsVisible;
    }

    private bool PFavoriteDiscardConfirm()
    {
        return _pFavoriteHost.PWindowDiscardConfirm(true, PEditor.PEditorDraftFinish);
    }

    private void PFavoriteModeUpdate()
    {
        LPanel panel = _lFavorite.LFavoritePanel;
        PEditor.Visibility = PLook.PLookVisibleRead(panel.LPanelEditing);
        PDisplay.Visibility = PLook.PLookVisibleRead(panel.LPanelViewerChecked);
        PFavoriteViewer.IsChecked = PLook.PLookCheckedRead(panel.LPanelViewerChecked);
        PFavoriteScribe.IsChecked = PLook.PLookCheckedRead(panel.LPanelScribeChecked);
        PFavoriteVoyage.Visibility = PLook.PLookVisibleRead(panel.LPanelViewerChecked);
        PFavoriteChronicle.Visibility = PLook.PLookVisibleRead(panel.LPanelScribeChecked);
        PFavoriteMode.IsEnabled = panel.LPanelModeEnabled;
        PFavoriteBin.IsEnabled = panel.LPanelBinEnabled;
    }
}
