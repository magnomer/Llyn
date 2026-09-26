using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIDeportment;

public class PYunjing : UserControl
{
    private readonly ObservableCollection<PYunjingItem> _pShengmuList = [];

    private readonly ObservableCollection<PYunjingItem> _pYunmuList = [];

    private readonly ObservableCollection<PXiaoyunItem> _pXiaoyunList = [];

    private PWindow _pYunjingHost = null!;

    private LYunjing _lYunjing = null!;

    public PYunjing()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Sound/Fanqie/PYunjing.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        PYunjingPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        PYunjingPress.Command = ApplicationCommands.Print;

        PChoice.PChoiceDropperAttach(PLadderDropper, PLadderDropdown, PLadder);
        PChoice.PChoiceDropperAttach(PStairDropper, PStairDropdown, PStair);

        PLadderIcon.PIconSource = PIcon.PIconResolve("sort", 24);
        PStairIcon.PIconSource = PIcon.PIconResolve("sort", 24);
        PYunjingBinIcon.PIconSource = PIcon.PIconResolve("delete", 24);
        PYunjingFresh.Tag = PIcon.PIconResolve("new", 24);
        PYunjingStore.Tag = PIcon.PIconResolve("save", 24);
        PYunjingEarlier.Tag = PIcon.PIconResolve("retreat", 24);
        PYunjingLater.Tag = PIcon.PIconResolve("advance", 24);
        PYunjingBackward.Tag = PIcon.PIconResolve("undo", 24);
        PYunjingForward.Tag = PIcon.PIconResolve("redo", 24);
        PYunjingPortrait.Tag = PIcon.PIconResolve("export", 24);
        PYunjingPress.Tag = PIcon.PIconResolve("print", 24);
        PYunjingViewer.Tag = PIcon.PIconResolve("view", 24);
        PYunjingScribe.Tag = PIcon.PIconResolve("edit", 24);

        PLookItem.PLookItemAttach(PShengmu, PYunjingItem.PYunjingItemApply);
        PLookItem.PLookItemAttach(PYunmu, PYunjingItem.PYunjingItemApply);
        PLookItem.PLookItemAttach(PXiaoyun, PXiaoyunItem.PXiaoyunItemApply);
        PShengmu.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PYunjingHandle));
        PYunmu.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PYunjingHandle));
        PXiaoyun.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PXiaoyunHandle));

        PPlumb.TextChanged += PPlumbHandle;
        PFathom.TextChanged += PFathomHandle;
        PBeacon.TextChanged += PBeaconHandle;
        PYunjingFresh.Click += PYunjingFreshHandle;
        PYunjingStore.Click += PYunjingStoreHandle;
        PYunjingEarlier.Click += PYunjingRetreatHandle;
        PYunjingLater.Click += PYunjingAdvanceHandle;
        PYunjingBackward.Click += PYunjingUndoHandle;
        PYunjingForward.Click += PYunjingRedoHandle;
        PYunjingViewer.Click += PYunjingScribeHandle;
        PYunjingScribe.Click += PYunjingScribeHandle;
        PYunjingBin.Click += PYunjingBinHandle;
    }

    private Border PLadder => (Border)FindName(nameof(PLadder));

    private ToggleButton PLadderDropper => (ToggleButton)FindName(nameof(PLadderDropper));

    private PIconImage PLadderIcon => (PIconImage)FindName(nameof(PLadderIcon));

    private TextBox PPlumb => (TextBox)FindName(nameof(PPlumb));

    private Popup PLadderDropdown => (Popup)FindName(nameof(PLadderDropdown));

    private StackPanel PLadderList => (StackPanel)FindName(nameof(PLadderList));

    private ItemsControl PShengmu => (ItemsControl)FindName(nameof(PShengmu));

    private TextBlock PShengmuEmpty => (TextBlock)FindName(nameof(PShengmuEmpty));

    private Border PStair => (Border)FindName(nameof(PStair));

    private ToggleButton PStairDropper => (ToggleButton)FindName(nameof(PStairDropper));

    private PIconImage PStairIcon => (PIconImage)FindName(nameof(PStairIcon));

    private TextBox PFathom => (TextBox)FindName(nameof(PFathom));

    private Popup PStairDropdown => (Popup)FindName(nameof(PStairDropdown));

    private StackPanel PStairList => (StackPanel)FindName(nameof(PStairList));

    private ItemsControl PYunmu => (ItemsControl)FindName(nameof(PYunmu));

    private TextBlock PYunmuEmpty => (TextBlock)FindName(nameof(PYunmuEmpty));

    private TextBox PBeacon => (TextBox)FindName(nameof(PBeacon));

    private ItemsControl PXiaoyun => (ItemsControl)FindName(nameof(PXiaoyun));

    private TextBlock PXiaoyunEmpty => (TextBlock)FindName(nameof(PXiaoyunEmpty));

    private Button PYunjingFresh => (Button)FindName(nameof(PYunjingFresh));

    private Button PYunjingStore => (Button)FindName(nameof(PYunjingStore));

    private StackPanel PYunjingVoyage => (StackPanel)FindName(nameof(PYunjingVoyage));

    private Button PYunjingEarlier => (Button)FindName(nameof(PYunjingEarlier));

    private Button PYunjingLater => (Button)FindName(nameof(PYunjingLater));

    private StackPanel PYunjingChronicle => (StackPanel)FindName(nameof(PYunjingChronicle));

    private Button PYunjingBackward => (Button)FindName(nameof(PYunjingBackward));

    private Button PYunjingForward => (Button)FindName(nameof(PYunjingForward));

    private Button PYunjingPortrait => (Button)FindName(nameof(PYunjingPortrait));

    private Button PYunjingPress => (Button)FindName(nameof(PYunjingPress));

    private Border PYunjingMode => (Border)FindName(nameof(PYunjingMode));

    private RadioButton PYunjingViewer => (RadioButton)FindName(nameof(PYunjingViewer));

    private RadioButton PYunjingScribe => (RadioButton)FindName(nameof(PYunjingScribe));

    private PDisplay PDisplay => (PDisplay)FindName(nameof(PDisplay));

    private PDiwei PYunjingDiwei => (PDiwei)FindName(nameof(PYunjingDiwei));

    private PEditor PEditor => (PEditor)FindName(nameof(PEditor));

    private Button PYunjingBin => (Button)FindName(nameof(PYunjingBin));

    private PIconImage PYunjingBinIcon => (PIconImage)FindName(nameof(PYunjingBinIcon));

    internal void PYunjingAttach(PWindow host)
    {
        _pYunjingHost = host;
        _lYunjing = host.PWindowDeportment.LWindowYunjingCreate(
            host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm),
            PYunjingShownCheck,
            PYunjingDiscardConfirm,
            host.PWindowDeleteConfirm);
        _lYunjing.LYunjingEditor.LEditorStateChanged += PYunjingStoreUpdate;
        _lYunjing.LYunjingChanged += PYunjingColumnUpdate;
        _lYunjing.LYunjingGlyphChosen += host.PWindowGlyphShow;
        _lYunjing.LYunjingPanel.LPanelChanged += PYunjingModeUpdate;
        _lYunjing.LYunjingPanel.LPanelRowsChanged += PXiaoyunUpdate;
        _lYunjing.LYunjingPanel.LPanelCleared += PYunjingClearUpdate;
        _lYunjing.LYunjingPanel.LPanelFailed += host.PWindowFailureShow;

        PShengmu.ItemsSource = _pShengmuList;
        PYunmu.ItemsSource = _pYunmuList;
        PXiaoyun.ItemsSource = _pXiaoyunList;

        PDisplay.PDisplayAttach(host, _lYunjing.LYunjingEditor.LEditorLectern);
        PYunjingDiwei.PDiweiAttach(host.PWindowDeportment);
        PYunjingDiwei.PDiweiEntryNotice = _lYunjing.LYunjingGlyphSelect;
        PYunjingDiwei.PDiweiSwitchNotice = _lYunjing.LYunjingTallySet;

        PEditor.PEditorAttach(host, _lYunjing.LYunjingEditor);

        PEditor.PEditorChronicleChanged += PYunjingChronicleUpdate;

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
        _lYunjing.LYunjingVistaRestore(_pYunjingHost.PWindowDeportment);
        _lYunjing.LYunjingShengmuAttach(
            LSubject.LSubjectVista, LObserver.LObserverCreate(this, _lYunjing.LYunjingRowsUpdate));
        _lYunjing.LYunjingYunmuAttach(
            LSubject.LSubjectVista, LObserver.LObserverCreate(this, _lYunjing.LYunjingRowsUpdate));
        _lYunjing.LYunjingShengmuAttach(
            LSubject.LSubjectWorkspace, LObserver.LObserverCreate(this, PYunjingWorkspaceUpdate));
        _lYunjing.LYunjingShengmuAttach(
            LSubject.LSubjectFanqie, LObserver.LObserverCreate(this, _lYunjing.LYunjingRowsUpdate));
        _lYunjing.LYunjingShengmuAttach(
            LSubject.LSubjectSettings, LObserver.LObserverCreate(this, _lYunjing.LYunjingRowsUpdate));
        _lYunjing.LYunjingShengmuAttach(
            LSubject.LSubjectReflex, LObserver.LObserverCreate(this, _lYunjing.LYunjingRowsUpdate));
        _lYunjing.LYunjingPanel.LPanelObserverAttach(
            LSubject.LSubjectVista, LObserver.LObserverCreate(this, _lYunjing.LYunjingPanel.LPanelRowsUpdate));
        _lYunjing.LYunjingPanel.LPanelObserverAttach(
            LSubject.LSubjectEntry, LObserver.LObserverCreate(this, _lYunjing.LYunjingEntryHandle));
        _lYunjing.LYunjingPanel.LPanelChosenAttach(
            LSubject.LSubjectEntry, LObserver.LObserverCreate(this, _lYunjing.LYunjingPanel.LPanelDraftUpdate));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderBuild(
            PLadderList,
            "Ladder",
            PLadderHandle,
            [
                LCatalogOrder.LCatalogOrderName,
                LCatalogOrder.LCatalogOrderReverse,
                LCatalogOrder.LCatalogOrderUsage,
            ]);
        PChoice.PChoiceOrderApply(PLadderDropdown, _lYunjing.LYunjingLadder);
        PChoice.PChoiceOrderBuild(
            PStairList,
            "Stair",
            PStairHandle,
            [
                LCatalogOrder.LCatalogOrderName,
                LCatalogOrder.LCatalogOrderReverse,
                LCatalogOrder.LCatalogOrderUsage,
            ]);
        PChoice.PChoiceOrderApply(PStairDropdown, _lYunjing.LYunjingStair);
        _lYunjing.LYunjingPlumbSet(PPlumb.Text);
        _lYunjing.LYunjingFathomSet(PFathom.Text);
        _lYunjing.LYunjingBeaconSet(PBeacon.Text);
        _lYunjing.LYunjingRowsUpdate();
    }

    private async void PYunjingWorkspaceUpdate()
    {
        await LEnsignImage.LEnsignLoad(_pYunjingHost.PWindowDeportment);
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
        return _lYunjing.LYunjingPanel.LPanelLeaveConfirm();
    }

    private bool PYunjingDiscardConfirm()
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
        LSplice.LSpliceApply(
            _pShengmuList,
            PYunjingItem.PYunjingItemBuild(_lYunjing.LYunjingShengmuRead()),
            PYunjingItem.PYunjingItemMatch,
            PYunjingItem.PYunjingItemSync);
        LSplice.LSpliceApply(
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
        LSplice.LSpliceApply(
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
        PYunjingVoyage.Visibility = PLook.PLookVisibleRead(_lYunjing.LYunjingPanel.LPanelViewerChecked);
        PYunjingChronicle.Visibility = PLook.PLookVisibleRead(_lYunjing.LYunjingPanel.LPanelScribeChecked);
        PYunjingMode.IsEnabled = _lYunjing.LYunjingPanel.LPanelModeEnabled;
        PYunjingBin.IsEnabled = _lYunjing.LYunjingPanel.LPanelBinEnabled;
    }

    private void PYunjingClearUpdate()
    {
        PDiweiUpdate();
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
        _lYunjing.LYunjingLadderSet(LChoice.LChoiceOrderRead(sender));
    }

    private void PStairHandle(object sender, RoutedEventArgs e)
    {
        PStairDropper.IsChecked = false;
        _lYunjing.LYunjingStairSet(LChoice.LChoiceOrderRead(sender));
    }

    private void PYunjingHandle(object sender, RoutedEventArgs e)
    {
        _lYunjing.LYunjingDiweiSelect(
            PSender.PSenderSourceRead<PYunjingItem>(e)?.PYunjingItemId,
            PSender.PSenderSourceRead<PYunjingItem>(e)?.PYunjingItemFinal);
    }

    private void PXiaoyunHandle(object sender, RoutedEventArgs e)
    {
        _pYunjingHost.PVoyageRecord();
        _lYunjing.LYunjingPanel.LPanelRowSelect(PSender.PSenderSourceRead<PXiaoyunItem>(e)?.PXiaoyunItemId);
    }

    internal long PYunjingVoyageRead()
    {
        return _lYunjing.LYunjingPanel.LPanelVoyageRead();
    }

    internal void PXiaoyunEntryShow(long id)
    {
        _lYunjing.LYunjingPanel.LPanelRowShow(id);
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
        await _pYunjingHost.PWindowPressRun(_lYunjing.LYunjingPortraitPrint);
    }

    private async void PYunjingPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pYunjingHost.PWindowPortraitExport(_lYunjing.LYunjingFileRead(), _lYunjing.LYunjingPortraitExport);
    }

    internal void PYunjingVoyageShow(bool past, bool future)
    {
        PYunjingEarlier.IsEnabled = past;
        PYunjingLater.IsEnabled = future;
    }

    private void PYunjingRetreatHandle(object sender, RoutedEventArgs e)
    {
        _pYunjingHost.PVoyageRetreatRun();
    }

    private void PYunjingAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _pYunjingHost.PVoyageAdvanceRun();
    }

    private void PYunjingUndoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleUndo();
    }

    private void PYunjingRedoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleRedo();
    }

    private void PYunjingChronicleUpdate()
    {
        (bool undo, bool redo) = PEditor.PEditorChronicleRead();
        PYunjingBackward.IsEnabled = undo;
        PYunjingForward.IsEnabled = redo;
    }
}
