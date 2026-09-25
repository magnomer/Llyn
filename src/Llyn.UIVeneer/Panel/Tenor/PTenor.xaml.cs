using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PTenor : UserControl
{
    private PWindow _pTenorHost = null!;

    private LTenor _lTenor = null!;

    private LEditor _lEditor = null!;

    public PTenor()
    {
        InitializeComponent();
    }

    internal void PTenorAttach(PWindow host)
    {
        _pTenorHost = host;
        _lEditor = host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm);
        _lTenor = host.PWindowDeportment.LWindowTenorCreate(
            _lEditor, PTenorShownCheck, PTenorDiscardConfirm, host.PWindowDeleteConfirm);
        LPanel panel = _lTenor.LTenorPanel;
        panel.LPanelChanged += PTenorModeUpdate;
        panel.LPanelRowsChanged += PCohortFind;
        panel.LPanelCleared += PDisplay.PDisplayClear;
        panel.LPanelDraftChanged += PTenorEntryUpdate;
        panel.LPanelFailed += host.PWindowFailureShow;

        PGamut.ItemsSource = _pGamutList;
        PCohort.ItemsSource = _pCohortList;

        PDisplay.PDisplayAttach(host, _lEditor.LEditorLectern);
        _lEditor.LEditorStateChanged += PTenorStoreUpdate;
        PEditor.PEditorAttach(host, _lEditor);
        PEditor.PEditorChronicleChanged += PTenorChronicleUpdate;
    }

    private void PTenorStoreUpdate()
    {
        PTenorStore.IsEnabled = _lEditor.LEditorStorable;
    }

    internal void PTenorReset()
    {
        _lTenor.LTenorPanel.LPanelClear();
        PGamutReset();
        PGamutFind();
    }

    internal bool PTenorDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PTenorChangeCheck()
    {
        return _lTenor.LTenorPanel.LPanelChangeCheck();
    }

    internal void PTenorClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }

    private void PTenorPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lTenor?.LTenorPanel.LPanelPressAllowed ?? false;
    }

    private async void PTenorPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pTenorHost.PWindowPressRun(_lTenor.LTenorPortraitPrint);
    }

    private async void PTenorPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pTenorHost.PWindowPortraitExport(_lTenor.LTenorFileRead(), _lTenor.LTenorPortraitExport);
    }

    internal void PTenorVoyageShow(bool past, bool future)
    {
        PTenorEarlier.IsEnabled = past;
        PTenorLater.IsEnabled = future;
    }

    private void PTenorRetreatHandle(object sender, RoutedEventArgs e)
    {
        _pTenorHost.PVoyageRetreatRun();
    }

    private void PTenorAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _pTenorHost.PVoyageAdvanceRun();
    }

    private void PTenorUndoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleUndo();
    }

    private void PTenorRedoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleRedo();
    }

    private void PTenorChronicleUpdate()
    {
        (bool undo, bool redo) = PEditor.PEditorChronicleRead();
        PTenorBackward.IsEnabled = undo;
        PTenorForward.IsEnabled = redo;
    }

    private bool PTenorShownCheck()
    {
        return IsVisible;
    }

    private bool PTenorDiscardConfirm()
    {
        return _pTenorHost.PWindowDiscardConfirm(true, PEditor.PEditorDraftFinish);
    }

    private void PTenorModeUpdate()
    {
        LPanel panel = _lTenor.LTenorPanel;
        PEditor.Visibility = PLook.PLookVisibleRead(panel.LPanelEditing);
        PDisplay.Visibility = PLook.PLookVisibleRead(panel.LPanelViewerChecked);
        PTenorViewer.IsChecked = PLook.PLookCheckedRead(panel.LPanelViewerChecked);
        PTenorScribe.IsChecked = PLook.PLookCheckedRead(panel.LPanelScribeChecked);
        PTenorMode.IsEnabled = panel.LPanelModeEnabled;
        PTenorBin.IsEnabled = panel.LPanelBinEnabled;
    }
}
