using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PFavorite : UserControl
{
    private PWindow _pFavoriteHost = null!;

    private LFavorite _lFavorite = null!;

    private LEditor _lEditor = null!;

    public PFavorite()
    {
        InitializeComponent();
    }

    internal void PFavoriteAttach(PWindow host)
    {
        _pFavoriteHost = host;
        _lEditor = host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm);
        _lFavorite = host.PWindowDeportment.LWindowFavoriteCreate(
            _lEditor, PFavoriteShownCheck, PFavoriteDiscardConfirm, host.PWindowDeleteConfirm);
        LPanel panel = _lFavorite.LFavoritePanel;
        panel.LPanelChanged += PFavoriteModeUpdate;
        panel.LPanelRowsChanged += PRosterFind;
        panel.LPanelCleared += PDisplay.PDisplayClear;
        panel.LPanelDraftChanged += PFavoriteEntryUpdate;
        panel.LPanelFailed += host.PWindowFailureShow;

        PRoster.ItemsSource = _pRosterList;

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
        PFavoriteMode.IsEnabled = panel.LPanelModeEnabled;
        PFavoriteBin.IsEnabled = panel.LPanelBinEnabled;
    }
}
