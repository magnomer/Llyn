using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PTaxonomy : UserControl
{
    private PWindow _pTaxonomyHost = null!;

    private LTaxonomy _lTaxonomy = null!;

    private LEditor _lEditor = null!;

    public PTaxonomy()
    {
        InitializeComponent();
    }

    internal void PTaxonomyAttach(PWindow host)
    {
        _pTaxonomyHost = host;
        _lEditor = host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm);
        _lTaxonomy = host.PWindowDeportment.LWindowTaxonomyCreate(
            _lEditor, PTaxonomyShownCheck, PTaxonomyDiscardConfirm, host.PWindowDeleteConfirm);
        LPanel panel = _lTaxonomy.LTaxonomyPanel;
        panel.LPanelChanged += PTaxonomyModeUpdate;
        panel.LPanelRowsChanged += PMembershipFind;
        panel.LPanelCleared += PDisplay.PDisplayClear;
        panel.LPanelDraftChanged += PTaxonomyEntryUpdate;
        panel.LPanelFailed += host.PWindowFailureShow;

        PDirectory.ItemsSource = _pDirectoryList;
        PMembership.ItemsSource = _pMembershipList;

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
        PTaxonomyMode.IsEnabled = panel.LPanelModeEnabled;
        PTaxonomyBin.IsEnabled = panel.LPanelBinEnabled;
    }
}
