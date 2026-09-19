using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PYunjing : UserControl
{
    private readonly ObservableCollection<PYunjingItem> _pShengmuList = [];

    private readonly ObservableCollection<PYunjingItem> _pYunmuList = [];

    private readonly ObservableCollection<PXiaoyunItem> _pXiaoyunList = [];

    private PWindow _pYunjingHost = null!;

    private LEngine _lEngine = null!;

    private LYunjing _lYunjing = null!;

    public PYunjing()
    {
        InitializeComponent();
    }

    internal void PYunjingAttach(PWindow host, LEngine engine)
    {
        _pYunjingHost = host;
        _lEngine = engine;
        _lYunjing = new LYunjing(
            engine,
            new LEditor(engine, host.PWindowUnreadableConfirm),
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

        PDisplay.PDisplayAttach(host, engine);
        PYunjingDiwei.PDiweiAttach(engine);
        PYunjingDiwei.PDiweiEntryNotice = _lYunjing.LYunjingGlyphSelect;
        PYunjingDiwei.PDiweiSwitchNotice = _lYunjing.LYunjingTallySet;

        PEditor.PEditorAttach(host, engine, _lYunjing.LYunjingEditor);

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PYunjingPressHandle, PYunjingPressCheck));
        CommandBindings.Add(new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, PYunjingPortraitHandle, PYunjingPressCheck));
    }

    private void PYunjingStoreUpdate()
    {
        PYunjingStore.IsEnabled = _lYunjing.LYunjingEditor.LEditorStorable;
    }

    internal bool PYunjingCheck()
    {
        return _lYunjing.LYunjingAllowed;
    }

    internal void PYunjingVistaRestore(LVista shengmu, LVista yunmu, LVista xiaoyun)
    {
        _lYunjing.LYunjingVistaRestore(shengmu, yunmu, xiaoyun);
        shengmu.LVistaObserverAttach(LSubject.LSubjectVista, new PObserver(this, _lYunjing.LYunjingRowsUpdate));
        yunmu.LVistaObserverAttach(LSubject.LSubjectVista, new PObserver(this, _lYunjing.LYunjingRowsUpdate));
        shengmu.LVistaObserverAttach(LSubject.LSubjectWorkspace, new PObserver(this, PYunjingWorkspaceUpdate));
        shengmu.LVistaObserverAttach(LSubject.LSubjectFanqie, new PObserver(this, _lYunjing.LYunjingRowsUpdate));
        shengmu.LVistaObserverAttach(LSubject.LSubjectSettings, new PObserver(this, _lYunjing.LYunjingRowsUpdate));
        shengmu.LVistaObserverAttach(LSubject.LSubjectReflex, new PObserver(this, _lYunjing.LYunjingRowsUpdate));
        xiaoyun.LVistaObserverAttach(
            LSubject.LSubjectVista, new PObserver(this, _lYunjing.LYunjingPanel.LPanelRowsUpdate));
        xiaoyun.LVistaObserverAttach(LSubject.LSubjectEntry, new PObserver(this, _lYunjing.LYunjingEntryHandle));
        xiaoyun.LVistaChosenAttach(
            LSubject.LSubjectEntry, new PObserver(this, _lYunjing.LYunjingPanel.LPanelDraftUpdate));
        PDisplay.PDisplayVistaRestore(xiaoyun);
        PEditor.PEditorVistaRestore(xiaoyun);
        PChoice.PChoiceOrderApply(PLadderDropdown, shengmu.LVistaOrder);
        PChoice.PChoiceOrderApply(PStairDropdown, yunmu.LVistaOrder);
        _lYunjing.LYunjingPlumbSet(PPlumb.Text);
        _lYunjing.LYunjingFathomSet(PFathom.Text);
        _lYunjing.LYunjingBeaconSet(PBeacon.Text);
        _lYunjing.LYunjingRowsUpdate();
    }

    private async void PYunjingWorkspaceUpdate()
    {
        await PEnsign.PEnsignLoad(_lEngine);
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
        await _pYunjingHost.PWindowPortraitExport(_lYunjing.LYunjingPanel.LPanelVista);
    }
}
