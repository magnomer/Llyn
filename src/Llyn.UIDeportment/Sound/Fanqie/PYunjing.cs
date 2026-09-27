using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public class PYunjing : UserControl
{
    private readonly ObservableCollection<PYunjingItem> _pShengmuList = [];

    private readonly ObservableCollection<PYunjingItem> _pYunmuList = [];

    private readonly ObservableCollection<PXiaoyunItem> _pXiaoyunList = [];

    private readonly QDiwei _qDiwei;

    private PWindow _pYunjingHost = null!;

    private LYunjing _lYunjing = null!;

    public PYunjing()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Sound/Fanqie/PYunjing.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));
        _qDiwei = new QDiwei(PYunjingDiwei);

        PYunjingPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        PYunjingPress.Command = ApplicationCommands.Print;

        QChoice.QChoiceDropperAttach(PLadderDropper, PLadderDropdown, PLadder);
        QChoice.QChoiceDropperAttach(PStairDropper, PStairDropdown, PStair);

        PLadderIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        PStairIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        PYunjingBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        PPlumb.SetResourceReference(QField.QFieldHintProperty, "Plumb.Search");
        PFathom.SetResourceReference(QField.QFieldHintProperty, "Fathom.Search");
        PBeacon.SetResourceReference(QField.QFieldHintProperty, "Beacon.Search");
        PYunjingFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        PYunjingStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        PYunjingEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        PYunjingLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        PYunjingBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        PYunjingForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        PYunjingPortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        PYunjingPress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        PYunjingViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        PYunjingScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        QLookItem.QLookItemAttach(PShengmu, PYunjingItem.PYunjingItemApply);
        QLookItem.QLookItemAttach(PYunmu, PYunjingItem.PYunjingItemApply);
        QLookItem.QLookItemAttach(PXiaoyun, PXiaoyunItem.PXiaoyunItemApply);
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

    private QIconImage PLadderIcon => (QIconImage)FindName(nameof(PLadderIcon));

    private TextBox PPlumb => (TextBox)FindName(nameof(PPlumb));

    private Popup PLadderDropdown => (Popup)FindName(nameof(PLadderDropdown));

    private StackPanel PLadderList => (StackPanel)FindName(nameof(PLadderList));

    private ItemsControl PShengmu => (ItemsControl)FindName(nameof(PShengmu));

    private TextBlock PShengmuEmpty => (TextBlock)FindName(nameof(PShengmuEmpty));

    private Border PStair => (Border)FindName(nameof(PStair));

    private ToggleButton PStairDropper => (ToggleButton)FindName(nameof(PStairDropper));

    private QIconImage PStairIcon => (QIconImage)FindName(nameof(PStairIcon));

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

    private UserControl PYunjingDiwei => (UserControl)FindName(nameof(PYunjingDiwei));

    private PEditor PEditor => (PEditor)FindName(nameof(PEditor));

    private Button PYunjingBin => (Button)FindName(nameof(PYunjingBin));

    private QIconImage PYunjingBinIcon => (QIconImage)FindName(nameof(PYunjingBinIcon));

    internal void PYunjingAttach(PWindow host)
    {
        _pYunjingHost = host;
        LEditor editor = host.PWindowForge.QForgeEditorCreate(host.PWindowEnvoy);
        LLectern lectern = new(editor.LEditorDisplay);
        _lYunjing = host.PWindowForge.QForgeYunjingCreate(
            editor,
            lectern,
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

        PDisplay.PDisplayAttach(host, lectern);
        _qDiwei.QDiweiAttach(host.PWindowAtelier, _lYunjing);

        PEditor.PEditorAttach(host, _lYunjing.LYunjingEditor, lectern);

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
        _lYunjing.LYunjingShengmuAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(this, _lYunjing.LYunjingRowsUpdate));
        _lYunjing.LYunjingYunmuAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(this, _lYunjing.LYunjingRowsUpdate));
        _lYunjing.LYunjingShengmuAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(this, PYunjingWorkspaceUpdate));
        _lYunjing.LYunjingShengmuAttach(
            CSubject.CSubjectFanqie, LObserver.LObserverCreate<CBulletin>(this, _lYunjing.LYunjingRowsUpdate));
        _lYunjing.LYunjingShengmuAttach(
            CSubject.CSubjectSettings, LObserver.LObserverCreate<CBulletin>(this, _lYunjing.LYunjingRowsUpdate));
        _lYunjing.LYunjingShengmuAttach(
            CSubject.CSubjectReflex, LObserver.LObserverCreate<CBulletin>(this, _lYunjing.LYunjingRowsUpdate));
        LPanel panel = _lYunjing.LYunjingPanel;
        panel.LPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(this, panel.LPanelRowsUpdate));
        panel.LPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(this, _lYunjing.LYunjingEntryHandle));
        panel.LPanelChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(this, panel.LPanelDraftUpdate));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        QChoice.QChoiceOrderBuild(
            PLadderList,
            "Ladder",
            PLadderHandle,
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderUsage,
            ]);
        QChoice.QChoiceOrderApply(PLadderDropdown, _lYunjing.LYunjingLadder);
        QChoice.QChoiceOrderBuild(
            PStairList,
            "Stair",
            PStairHandle,
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderUsage,
            ]);
        QChoice.QChoiceOrderApply(PStairDropdown, _lYunjing.LYunjingStair);
        _lYunjing.LYunjingPlumbSet(PPlumb.Text);
        _lYunjing.LYunjingFathomSet(PFathom.Text);
        _lYunjing.LYunjingBeaconSet(PBeacon.Text);
        _lYunjing.LYunjingRowsUpdate();
    }

    private async void PYunjingWorkspaceUpdate()
    {
        await LEnsignImage.LEnsignLoad(_pYunjingHost.PWindowAtelier);
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
        PShengmuEmpty.Visibility = QLook.QLookVisibleRead(_lYunjing.LYunjingShengmuEmpty);
        PYunmuEmpty.SetResourceReference(TextBlock.TextProperty, _lYunjing.LYunjingYunmuKey);
        PYunmuEmpty.Visibility = QLook.QLookVisibleRead(_lYunjing.LYunjingYunmuEmpty);
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
        PXiaoyunEmpty.Visibility = QLook.QLookVisibleRead(_lYunjing.LYunjingXiaoyunEmpty);
    }

    private void PDiweiUpdate()
    {
        _qDiwei.QDiweiShow(_lYunjing.LYunjingDiweiRead(), _lYunjing.LYunjingDiweiKey);
    }

    private void PYunjingModeUpdate()
    {
        PEditor.Visibility = QLook.QLookVisibleRead(_lYunjing.LYunjingEditorShown);
        PDisplay.Visibility = QLook.QLookVisibleRead(_lYunjing.LYunjingDisplayShown);
        PYunjingDiwei.Visibility = QLook.QLookVisibleRead(_lYunjing.LYunjingDiweiShown);
        PYunjingViewer.IsChecked = QLook.QLookCheckedRead(_lYunjing.LYunjingPanel.LPanelViewerChecked);
        PYunjingScribe.IsChecked = QLook.QLookCheckedRead(_lYunjing.LYunjingPanel.LPanelScribeChecked);
        PYunjingVoyage.Visibility = QLook.QLookVisibleRead(_lYunjing.LYunjingPanel.LPanelViewerChecked);
        PYunjingChronicle.Visibility = QLook.QLookVisibleRead(_lYunjing.LYunjingPanel.LPanelScribeChecked);
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
        _lYunjing.LYunjingLadderSet(QChoice.QChoiceOrderRead(sender));
    }

    private void PStairHandle(object sender, RoutedEventArgs e)
    {
        PStairDropper.IsChecked = false;
        _lYunjing.LYunjingStairSet(QChoice.QChoiceOrderRead(sender));
    }

    private void PYunjingHandle(object sender, RoutedEventArgs e)
    {
        _lYunjing.LYunjingDiweiSelect(
            QSender.QSenderSourceRead<PYunjingItem>(e)?.PYunjingItemId,
            QSender.QSenderSourceRead<PYunjingItem>(e)?.PYunjingItemFinal);
    }

    private void PXiaoyunHandle(object sender, RoutedEventArgs e)
    {
        _pYunjingHost.PVoyageRecord();
        _lYunjing.LYunjingPanel.LPanelRowSelect(QSender.QSenderSourceRead<PXiaoyunItem>(e)?.PXiaoyunItemId);
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
        PEditor.QChronicleUndo();
    }

    private void PYunjingRedoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.QChronicleRedo();
    }

    private void PYunjingChronicleUpdate()
    {
        (bool undo, bool redo) = PEditor.PEditorChronicleRead();
        PYunjingBackward.IsEnabled = undo;
        PYunjingForward.IsEnabled = redo;
    }
}
