using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PYunjing : UserControl
{
    private readonly ObservableCollection<PYunjingItem> _pShengmuList = [];

    private readonly ObservableCollection<PYunjingItem> _pYunmuList = [];

    private readonly ObservableCollection<PXiaoyunItem> _pXiaoyunList = [];

    private PWindow _pYunjingHost = null!;

    private LYunjing _lYunjing = null!;

    public PYunjing()
    {
        InitializeComponent();
    }

    internal void PYunjingAttach(PWindow host)
    {
        _pYunjingHost = host;
        _lYunjing = host.PWindowDeportment.LWindowYunjingCreate(
            host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm),
            PYunjingShownCheck,
            PYunjingLeaveConfirm,
            host.PWindowDeleteConfirm);
        _lYunjing.LYunjingEditor.LEditorStateChanged += PYunjingStoreUpdate;
        _lYunjing.LYunjingChanged += PYunjingColumnUpdate;
        _lYunjing.LYunjingGlyphChosen += host.PWindowGlyphShow;
        _lYunjing.LYunjingPanel.LPanelChanged += PYunjingModeUpdate;
        _lYunjing.LYunjingPanel.LPanelRowsChanged += PXiaoyunUpdate;
        _lYunjing.LYunjingPanel.LPanelCleared += PYunjingClearUpdate;
        _lYunjing.LYunjingPanel.LPanelDraftChanged += PYunjingEntryUpdate;
        _lYunjing.LYunjingPanel.LPanelFailed += host.PWindowFailureShow;

        PShengmu.ItemsSource = _pShengmuList;
        PYunmu.ItemsSource = _pYunmuList;
        PXiaoyun.ItemsSource = _pXiaoyunList;

        PDisplay.PDisplayAttach(host, _lYunjing.LYunjingEditor.LEditorDisplay);
        PYunjingDiwei.PDiweiAttach(host.PWindowDeportment);
        PYunjingDiwei.PDiweiEntryNotice = _lYunjing.LYunjingGlyphSelect;
        PYunjingDiwei.PDiweiSwitchNotice = _lYunjing.LYunjingTallySet;

        PEditor.PEditorAttach(host, _lYunjing.LYunjingEditor);

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PYunjingPressHandle, PYunjingPressCheck));
        CommandBindings.Add(
            new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, PYunjingPortraitHandle, PYunjingPressCheck));
    }

    private void PYunjingStoreUpdate()
    {
        PYunjingStore.IsEnabled = _lYunjing.LYunjingEditor.LEditorStorable;
    }

    internal bool PYunjingCheck()
    {
        return _lYunjing.LYunjingAllowed;
    }

    internal void PYunjingVistaRestore()
    {
        _lYunjing.LYunjingVistaRestore(_pYunjingHost.PWindowPosture);
        _lYunjing.LYunjingShengmuAttach(
            LSubject.LSubjectVista, PObserver.PObserverCreate(this, _lYunjing.LYunjingRowsUpdate));
        _lYunjing.LYunjingYunmuAttach(
            LSubject.LSubjectVista, PObserver.PObserverCreate(this, _lYunjing.LYunjingRowsUpdate));
        _lYunjing.LYunjingShengmuAttach(
            LSubject.LSubjectWorkspace, PObserver.PObserverCreate(this, PYunjingWorkspaceUpdate));
        _lYunjing.LYunjingShengmuAttach(
            LSubject.LSubjectFanqie, PObserver.PObserverCreate(this, _lYunjing.LYunjingRowsUpdate));
        _lYunjing.LYunjingShengmuAttach(
            LSubject.LSubjectSettings, PObserver.PObserverCreate(this, _lYunjing.LYunjingRowsUpdate));
        _lYunjing.LYunjingShengmuAttach(
            LSubject.LSubjectReflex, PObserver.PObserverCreate(this, _lYunjing.LYunjingRowsUpdate));
        _lYunjing.LYunjingPanel.LPanelObserverAttach(
            LSubject.LSubjectVista, PObserver.PObserverCreate(this, _lYunjing.LYunjingPanel.LPanelRowsUpdate));
        _lYunjing.LYunjingPanel.LPanelObserverAttach(
            LSubject.LSubjectEntry, PObserver.PObserverCreate(this, _lYunjing.LYunjingEntryHandle));
        _lYunjing.LYunjingPanel.LPanelChosenAttach(
            LSubject.LSubjectEntry, PObserver.PObserverCreate(this, _lYunjing.LYunjingPanel.LPanelDraftUpdate));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderApply(PLadderDropdown, _lYunjing.LYunjingLadder);
        PChoice.PChoiceOrderApply(PStairDropdown, _lYunjing.LYunjingStair);
        _lYunjing.LYunjingPlumbSet(PPlumb.Text);
        _lYunjing.LYunjingFathomSet(PFathom.Text);
        _lYunjing.LYunjingBeaconSet(PBeacon.Text);
        _lYunjing.LYunjingRowsUpdate();
    }

    private async void PYunjingWorkspaceUpdate()
    {
        await PEnsign.PEnsignLoad(_pYunjingHost.PWindowDeportment);
        _lYunjing.LYunjingReset();
    }

    internal bool PYunjingDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PYunjingChangeCheck()
    {
        return _lYunjing.LYunjingPanel.LPanelChangeCheck();
    }

    internal bool PYunjingLeaveConfirm()
    {
        return _pYunjingHost.PWindowDiscardConfirm(true, PEditor.PEditorDraftFinish);
    }

    internal void PYunjingDiweiShow(string language, string kind, string key)
    {
        PPlumb.Text = string.Empty;
        PFathom.Text = string.Empty;
        _lYunjing.LYunjingDiweiShow(language, kind, key);
    }

    internal void PYunjingScribeRestore(bool editing)
    {
        _lYunjing.LYunjingPanel.LPanelScribeRestore(editing);
    }

    internal void PYunjingClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }

    private bool PYunjingShownCheck()
    {
        return IsVisible;
    }

    private void PYunjingColumnUpdate()
    {
        PSplice.PSpliceApply(
            _pShengmuList,
            PYunjingItem.PYunjingItemBuild(_lYunjing.LYunjingShengmuRead()),
            PYunjingItem.PYunjingItemMatch,
            PYunjingItem.PYunjingItemSync);
        PSplice.PSpliceApply(
            _pYunmuList,
            PYunjingItem.PYunjingItemBuild(_lYunjing.LYunjingYunmuRead()),
            PYunjingItem.PYunjingItemMatch,
            PYunjingItem.PYunjingItemSync);
        PShengmuEmpty.SetResourceReference(TextBlock.TextProperty, _lYunjing.LYunjingShengmuKey);
        PShengmuEmpty.Visibility = PLook.PLookVisibleRead(_lYunjing.LYunjingShengmuEmpty);
        PYunmuEmpty.SetResourceReference(TextBlock.TextProperty, _lYunjing.LYunjingYunmuKey);
        PYunmuEmpty.Visibility = PLook.PLookVisibleRead(_lYunjing.LYunjingYunmuEmpty);
        PDiweiUpdate();
        PYunjingModeUpdate();
    }

    private void PXiaoyunUpdate()
    {
        PSplice.PSpliceApply(
            _pXiaoyunList,
            PXiaoyunItem.PXiaoyunItemBuild(_lYunjing.LYunjingXiaoyunRead()),
            PXiaoyunItem.PXiaoyunItemMatch,
            PXiaoyunItem.PXiaoyunItemSync);
        PXiaoyunEmpty.SetResourceReference(TextBlock.TextProperty, _lYunjing.LYunjingXiaoyunKey);
        PXiaoyunEmpty.Visibility = PLook.PLookVisibleRead(_lYunjing.LYunjingXiaoyunEmpty);
    }

    private void PDiweiUpdate()
    {
        PYunjingDiwei.PDiweiShow(_lYunjing.LYunjingDiweiRead(), _lYunjing.LYunjingDiweiKey);
    }

    private void PYunjingModeUpdate()
    {
        PEditor.Visibility = PLook.PLookVisibleRead(_lYunjing.LYunjingEditorShown);
        PDisplay.Visibility = PLook.PLookVisibleRead(_lYunjing.LYunjingDisplayShown);
        PYunjingDiwei.Visibility = PLook.PLookVisibleRead(_lYunjing.LYunjingDiweiShown);
        PYunjingViewer.IsChecked = PLook.PLookCheckedRead(_lYunjing.LYunjingPanel.LPanelViewerChecked);
        PYunjingScribe.IsChecked = PLook.PLookCheckedRead(_lYunjing.LYunjingPanel.LPanelScribeChecked);
        PYunjingMode.IsEnabled = _lYunjing.LYunjingPanel.LPanelModeEnabled;
        PYunjingBin.IsEnabled = _lYunjing.LYunjingPanel.LPanelBinEnabled;
    }

    private void PYunjingClearUpdate()
    {
        PDisplay.PDisplayClear();
        PDiweiUpdate();
    }

    private void PYunjingEntryUpdate(LDraft draft)
    {
        PDisplay.PDisplayShow(draft.LDraftContent);
    }

    private void PPlumbHandle(object sender, TextChangedEventArgs e)
    {
        _lYunjing.LYunjingPlumbSet(PPlumb.Text);
    }

    private void PFathomHandle(object sender, TextChangedEventArgs e)
    {
        _lYunjing.LYunjingFathomSet(PFathom.Text);
    }

    private void PBeaconHandle(object sender, TextChangedEventArgs e)
    {
        _lYunjing.LYunjingBeaconSet(PBeacon.Text);
    }

    private void PLadderHandle(object sender, RoutedEventArgs e)
    {
        PLadderDropper.IsChecked = false;
        _lYunjing.LYunjingLadderSet(PSender.PSenderTagRead(sender));
    }

    private void PStairHandle(object sender, RoutedEventArgs e)
    {
        PStairDropper.IsChecked = false;
        _lYunjing.LYunjingStairSet(PSender.PSenderTagRead(sender));
    }

    private void PYunjingHandle(object sender, RoutedEventArgs e)
    {
        _lYunjing.LYunjingDiweiSelect(
            PSender.PSenderItemRead<PYunjingItem>(sender)?.PYunjingItemId,
            PSender.PSenderItemRead<PYunjingItem>(sender)?.PYunjingItemFinal);
    }

    private void PXiaoyunHandle(object sender, RoutedEventArgs e)
    {
        _lYunjing.LYunjingPanel.LPanelRowSelect(PSender.PSenderItemRead<PXiaoyunItem>(sender)?.PXiaoyunItemId);
    }

    private void PYunjingFreshHandle(object sender, RoutedEventArgs e)
    {
        _lYunjing.LYunjingPanel.LPanelFreshStart();
    }

    private void PYunjingScribeHandle(object sender, RoutedEventArgs e)
    {
        _lYunjing.LYunjingPanel.LPanelScribeSet(ReferenceEquals(sender, PYunjingScribe));
    }

    private void PYunjingStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PYunjingBinHandle(object sender, RoutedEventArgs e)
    {
        _lYunjing.LYunjingPanel.LPanelDelete();
    }

    private void PYunjingPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lYunjing.LYunjingPanel.LPanelPressAllowed;
    }

    private async void PYunjingPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pYunjingHost.PWindowPressRun(
            ticket => _lYunjing.LYunjingPortraitPrint(_pYunjingHost.PWindowLabelRead(), ticket));
    }

    private async void PYunjingPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pYunjingHost.PWindowPortraitExport(_lYunjing.LYunjingFileRead(), _lYunjing.LYunjingPortraitExport);
    }
}
