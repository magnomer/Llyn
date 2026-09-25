using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PXiesheng : UserControl
{
    private readonly ObservableCollection<PGroveItem> _pGroveList = [];

    private readonly ObservableCollection<PKindredItem> _pKindredList = [];

    private PWindow _pXieshengHost = null!;

    private LXiesheng _lXiesheng = null!;

    public PXiesheng()
    {
        InitializeComponent();
    }

    internal void PXieshengAttach(PWindow host)
    {
        _pXieshengHost = host;
        _lXiesheng = host.PWindowDeportment.LWindowXieshengCreate(
            host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm),
            PXieshengShownCheck,
            PXieshengDiscardConfirm,
            host.PWindowDeleteConfirm);
        _lXiesheng.LXieshengEditor.LEditorStateChanged += PXieshengStoreUpdate;
        _lXiesheng.LXieshengChanged += PXieshengColumnUpdate;
        _lXiesheng.LXieshengGlyphChosen += host.PWindowGlyphShow;
        _lXiesheng.LXieshengPanel.LPanelChanged += PXieshengModeUpdate;
        _lXiesheng.LXieshengPanel.LPanelRowsChanged += PKindredUpdate;
        _lXiesheng.LXieshengPanel.LPanelCleared += PXieshengClearUpdate;
        _lXiesheng.LXieshengPanel.LPanelDraftChanged += PXieshengEntryUpdate;
        _lXiesheng.LXieshengPanel.LPanelFailed += host.PWindowFailureShow;

        PGrove.ItemsSource = _pGroveList;
        PKindred.ItemsSource = _pKindredList;

        PDisplay.PDisplayAttach(host, _lXiesheng.LXieshengEditor.LEditorLectern);
        PXieshengStem.PStemAttach(host.PWindowDeportment);
        PXieshengStem.PStemEntryNotice = _lXiesheng.LXieshengGlyphSelect;

        PEditor.PEditorAttach(host, _lXiesheng.LXieshengEditor);

        PEditor.PEditorChronicleChanged += PXieshengChronicleUpdate;

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PXieshengPressHandle, PXieshengPressCheck));
        CommandBindings.Add(
            new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, PXieshengPortraitHandle, PXieshengPressCheck));
    }

    private void PXieshengStoreUpdate()
    {
        PXieshengStore.IsEnabled = _lXiesheng.LXieshengEditor.LEditorStorable;
    }

    internal bool PXieshengCheck()
    {
        return _lXiesheng.LXieshengAllowed;
    }

    internal void PXieshengVistaRestore()
    {
        _lXiesheng.LXieshengVistaRestore(_pXieshengHost.PWindowDeportment);
        _lXiesheng.LXieshengGroveAttach(
            LSubject.LSubjectVista, PObserver.PObserverCreate(this, _lXiesheng.LXieshengRowsUpdate));
        _lXiesheng.LXieshengGroveAttach(
            LSubject.LSubjectWorkspace, PObserver.PObserverCreate(this, PXieshengWorkspaceUpdate));
        _lXiesheng.LXieshengGroveAttach(
            LSubject.LSubjectFanqie, PObserver.PObserverCreate(this, _lXiesheng.LXieshengRowsUpdate));
        _lXiesheng.LXieshengGroveAttach(
            LSubject.LSubjectSettings, PObserver.PObserverCreate(this, _lXiesheng.LXieshengRowsUpdate));
        _lXiesheng.LXieshengPanel.LPanelObserverAttach(
            LSubject.LSubjectVista, PObserver.PObserverCreate(this, _lXiesheng.LXieshengPanel.LPanelRowsUpdate));
        _lXiesheng.LXieshengPanel.LPanelObserverAttach(
            LSubject.LSubjectEntry, PObserver.PObserverCreate(this, _lXiesheng.LXieshengEntryHandle));
        _lXiesheng.LXieshengPanel.LPanelChosenAttach(
            LSubject.LSubjectEntry, PObserver.PObserverCreate(this, _lXiesheng.LXieshengPanel.LPanelDraftUpdate));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderBuild(
            PRungList,
            "Rung",
            PRungHandle,
            [
                LCatalogOrder.LCatalogOrderName,
                LCatalogOrder.LCatalogOrderReverse,
                LCatalogOrder.LCatalogOrderUsage,
            ]);
        PChoice.PChoiceOrderApply(PRungDropdown, _lXiesheng.LXieshengRung);
        _lXiesheng.LXieshengLodestarSet(PLodestar.Text);
        _lXiesheng.LXieshengSextantSet(PSextant.Text);
        _lXiesheng.LXieshengRowsUpdate();
    }

    private async void PXieshengWorkspaceUpdate()
    {
        await PEnsign.PEnsignLoad(_pXieshengHost.PWindowDeportment);
        _lXiesheng.LXieshengReset();
    }

    internal bool PXieshengDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PXieshengChangeCheck()
    {
        return _lXiesheng.LXieshengPanel.LPanelChangeCheck();
    }

    internal bool PXieshengLeaveConfirm()
    {
        return _lXiesheng.LXieshengPanel.LPanelLeaveConfirm();
    }

    private bool PXieshengDiscardConfirm()
    {
        return _pXieshengHost.PWindowDiscardConfirm(true, PEditor.PEditorDraftFinish);
    }

    internal void PXieshengStemShow(string language, string? key)
    {
        PLodestar.Text = string.Empty;
        _lXiesheng.LXieshengStemShow(language, key);
    }

    internal void PXieshengScribeRestore(bool editing)
    {
        _lXiesheng.LXieshengPanel.LPanelScribeRestore(editing);
    }

    internal void PXieshengClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }

    private bool PXieshengShownCheck()
    {
        return IsVisible;
    }

    private void PXieshengColumnUpdate()
    {
        LSplice.LSpliceApply(
            _pGroveList,
            PGroveItem.PGroveItemBuild(_lXiesheng.LXieshengGroveRead()),
            PGroveItem.PGroveItemMatch,
            PGroveItem.PGroveItemSync);
        PGroveEmpty.SetResourceReference(TextBlock.TextProperty, _lXiesheng.LXieshengGroveKey);
        PGroveEmpty.Visibility = PLook.PLookVisibleRead(_lXiesheng.LXieshengGroveEmpty);
        PStemUpdate();
        PXieshengModeUpdate();
    }

    private void PKindredUpdate()
    {
        LSplice.LSpliceApply(
            _pKindredList,
            PKindredItem.PKindredItemBuild(_lXiesheng.LXieshengKindredRead()),
            PKindredItem.PKindredItemMatch,
            PKindredItem.PKindredItemSync);
        PKindredEmpty.SetResourceReference(TextBlock.TextProperty, _lXiesheng.LXieshengKindredKey);
        PKindredEmpty.Visibility = PLook.PLookVisibleRead(_lXiesheng.LXieshengKindredEmpty);
    }

    private void PStemUpdate()
    {
        PXieshengStem.PStemShow(_lXiesheng.LXieshengStemRead());
    }

    private void PXieshengModeUpdate()
    {
        PEditor.Visibility = PLook.PLookVisibleRead(_lXiesheng.LXieshengEditorShown);
        PDisplay.Visibility = PLook.PLookVisibleRead(_lXiesheng.LXieshengDisplayShown);
        PXieshengStem.Visibility = PLook.PLookVisibleRead(_lXiesheng.LXieshengStemShown);
        PXieshengViewer.IsChecked = PLook.PLookCheckedRead(_lXiesheng.LXieshengPanel.LPanelViewerChecked);
        PXieshengScribe.IsChecked = PLook.PLookCheckedRead(_lXiesheng.LXieshengPanel.LPanelScribeChecked);
        PXieshengMode.IsEnabled = _lXiesheng.LXieshengPanel.LPanelModeEnabled;
        PXieshengBin.IsEnabled = _lXiesheng.LXieshengPanel.LPanelBinEnabled;
    }

    private void PXieshengClearUpdate()
    {
        PDisplay.PDisplayClear();
        PStemUpdate();
    }

    private void PXieshengEntryUpdate(LDraft draft)
    {
        PDisplay.PDisplayShow(draft.LDraftContent);
    }

    private void PLodestarHandle(object sender, TextChangedEventArgs e)
    {
        _lXiesheng.LXieshengLodestarSet(PLodestar.Text);
    }

    private void PSextantHandle(object sender, TextChangedEventArgs e)
    {
        _lXiesheng.LXieshengSextantSet(PSextant.Text);
    }

    private void PRungHandle(object sender, RoutedEventArgs e)
    {
        PRungDropper.IsChecked = false;
        _lXiesheng.LXieshengRungSet(LChoice.LChoiceOrderRead(sender));
    }

    private void PGroveHandle(object sender, RoutedEventArgs e)
    {
        _lXiesheng.LXieshengStemSelect(PSender.PSenderItemRead<PGroveItem>(sender)?.PGroveItemId);
    }

    private void PKindredHandle(object sender, RoutedEventArgs e)
    {
        _pXieshengHost.PVoyageRecord();
        _lXiesheng.LXieshengPanel.LPanelRowSelect(PSender.PSenderItemRead<PKindredItem>(sender)?.PKindredItemId);
    }

    internal long PXieshengVoyageRead()
    {
        return _lXiesheng.LXieshengPanel.LPanelVoyageRead();
    }

    internal void PKindredEntryShow(long id)
    {
        _lXiesheng.LXieshengPanel.LPanelRowShow(id);
    }

    private void PXieshengFreshHandle(object sender, RoutedEventArgs e)
    {
        _lXiesheng.LXieshengPanel.LPanelFreshStart();
    }

    private void PXieshengScribeHandle(object sender, RoutedEventArgs e)
    {
        _lXiesheng.LXieshengPanel.LPanelScribeSet(ReferenceEquals(sender, PXieshengScribe));
    }

    private void PXieshengStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PXieshengBinHandle(object sender, RoutedEventArgs e)
    {
        _lXiesheng.LXieshengPanel.LPanelDelete();
    }

    private void PXieshengPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lXiesheng.LXieshengPanel.LPanelPressAllowed;
    }

    private async void PXieshengPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pXieshengHost.PWindowPressRun(_lXiesheng.LXieshengPortraitPrint);
    }

    private async void PXieshengPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pXieshengHost.PWindowPortraitExport(
            _lXiesheng.LXieshengFileRead(), _lXiesheng.LXieshengPortraitExport);
    }

    internal void PXieshengVoyageShow(bool past, bool future)
    {
        PXieshengEarlier.IsEnabled = past;
        PXieshengLater.IsEnabled = future;
    }

    private void PXieshengRetreatHandle(object sender, RoutedEventArgs e)
    {
        _pXieshengHost.PVoyageRetreatRun();
    }

    private void PXieshengAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _pXieshengHost.PVoyageAdvanceRun();
    }

    private void PXieshengUndoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleUndo();
    }

    private void PXieshengRedoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleRedo();
    }

    private void PXieshengChronicleUpdate()
    {
        (bool undo, bool redo) = PEditor.PEditorChronicleRead();
        PXieshengBackward.IsEnabled = undo;
        PXieshengForward.IsEnabled = redo;
    }
}
