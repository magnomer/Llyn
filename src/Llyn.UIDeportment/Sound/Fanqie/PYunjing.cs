using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public class PYunjing : UserControl
{
    private readonly ObservableCollection<PYunjingItem> _pShengmuList = [];

    private readonly ObservableCollection<PYunjingItem> _pYunmuList = [];

    private readonly ObservableCollection<PXiaoyunItem> _pXiaoyunList = [];

    private readonly QDiwei _qDiwei;

    private PWindow _pYunjingHost = null!;

    private CYunjing _cYunjing = null!;

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
        _cYunjing = host.PWindowForge.QForgeYunjingCreate(PYunjingShownCheck, host.PWindowEnvoy);
        _cYunjing.CYunjingDiweiOpened += PYunjingQueryRefine;
        CPanel panel = _cYunjing.CYunjingPanel;
        QLectern lectern = new(_cYunjing.CYunjingEditor.CEditorDisplay, panel);
        panel.CPanelChanged += PYunjingModeUpdate;
        panel.CPanelRowsChanged += PXiaoyunUpdate;
        panel.CPanelCleared += PYunjingClearUpdate;
        _cYunjing.CYunjingEditor.CEditorDesk.CDeskStateChanged += PYunjingStoreUpdate;
        _cYunjing.CYunjingChanged += PYunjingColumnUpdate;

        PShengmu.ItemsSource = _pShengmuList;
        PYunmu.ItemsSource = _pYunmuList;
        PXiaoyun.ItemsSource = _pXiaoyunList;

        PDisplay.PDisplayAttach(host, lectern);
        _qDiwei.QDiweiAttach(host.PWindowAtelier, _cYunjing);

        PEditor.PEditorIntroduce(host, new QEditor(_cYunjing.CYunjingEditor));

        PEditor.PEditorChronicleChanged += PYunjingChronicleUpdate;

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PYunjingPressHandle, PYunjingPressCheck));
        CommandBindings.Add(
            new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, PYunjingPortraitHandle, PYunjingPressCheck));
    }

    private void PYunjingStoreUpdate()
    {
        PYunjingStore.IsEnabled = _cYunjing.CYunjingEditor.CEditorDesk.CDeskStorable;
    }

    internal void PYunjingVistaRestore()
    {
        _cYunjing.CYunjingShengmuAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(this, _cYunjing.CYunjingRowsResonate));
        _cYunjing.CYunjingYunmuAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(this, _cYunjing.CYunjingRowsResonate));
        _cYunjing.CYunjingShengmuAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(this, PYunjingWorkspaceUpdate));
        _cYunjing.CYunjingShengmuAttach(
            CSubject.CSubjectFanqie, LObserver.LObserverCreate<CBulletin>(this, _cYunjing.CYunjingRowsResonate));
        _cYunjing.CYunjingShengmuAttach(
            CSubject.CSubjectSettings, LObserver.LObserverCreate<CBulletin>(this, _cYunjing.CYunjingRowsResonate));
        _cYunjing.CYunjingShengmuAttach(
            CSubject.CSubjectReflex, LObserver.LObserverCreate<CBulletin>(this, _cYunjing.CYunjingRowsResonate));
        CPanel panel = _cYunjing.CYunjingPanel;
        panel.CPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(this, panel.CPanelRowsResonate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(this, _cYunjing.CYunjingEntryResonate));
        panel.CPanelChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(this, panel.CPanelDraftResonate));
        QChoice.QChoiceOrderBuild(
            PLadderList,
            "Ladder",
            PLadderHandle,
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderUsage,
            ]);
        QChoice.QChoiceOrderApply(PLadderDropdown, _cYunjing.CYunjingShengmuOrder);
        QChoice.QChoiceOrderBuild(
            PStairList,
            "Stair",
            PStairHandle,
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderUsage,
            ]);
        QChoice.QChoiceOrderApply(PStairDropdown, _cYunjing.CYunjingYunmuOrder);
        _cYunjing.CYunjingShengmuFind(PPlumb.Text);
        _cYunjing.CYunjingYunmuFind(PFathom.Text);
        _cYunjing.CYunjingXiaoyunFind(PBeacon.Text);
        _cYunjing.CYunjingRowsResonate();
    }

    private async void PYunjingWorkspaceUpdate()
    {
        await LEnsignImage.LEnsignLoad(_pYunjingHost.PWindowAtelier);
        _cYunjing.CYunjingDiweiCancel();
    }

    private void PYunjingQueryRefine()
    {
        PPlumb.Text = string.Empty;
        PFathom.Text = string.Empty;
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
            PYunjingItem.PYunjingItemBuild(_cYunjing.CYunjingShengmuRead()),
            PYunjingItem.PYunjingItemMatch,
            PYunjingItem.PYunjingItemSync);
        LSplice.LSpliceApply(
            _pYunmuList,
            PYunjingItem.PYunjingItemBuild(_cYunjing.CYunjingYunmuRead()),
            PYunjingItem.PYunjingItemMatch,
            PYunjingItem.PYunjingItemSync);
        PShengmuEmpty.SetResourceReference(TextBlock.TextProperty, _cYunjing.CYunjingShengmuKey);
        PShengmuEmpty.Visibility = QLook.QLookVisibleRead(_cYunjing.CYunjingShengmuEmpty);
        PYunmuEmpty.SetResourceReference(TextBlock.TextProperty, _cYunjing.CYunjingYunmuKey);
        PYunmuEmpty.Visibility = QLook.QLookVisibleRead(_cYunjing.CYunjingYunmuEmpty);
        PDiweiUpdate();
        PYunjingModeUpdate();
    }

    private void PXiaoyunUpdate()
    {
        LSplice.LSpliceApply(
            _pXiaoyunList,
            PXiaoyunItem.PXiaoyunItemBuild(_cYunjing.CYunjingXiaoyunRead()),
            PXiaoyunItem.PXiaoyunItemMatch,
            PXiaoyunItem.PXiaoyunItemSync);
        PXiaoyunEmpty.SetResourceReference(TextBlock.TextProperty, _cYunjing.CYunjingXiaoyunKey);
        PXiaoyunEmpty.Visibility = QLook.QLookVisibleRead(_cYunjing.CYunjingXiaoyunEmpty);
    }

    private void PDiweiUpdate()
    {
        _qDiwei.QDiweiShow(_cYunjing.CYunjingDiweiRead(), _cYunjing.CYunjingDiweiKey);
    }

    private void PYunjingModeUpdate()
    {
        PEditor.Visibility = QLook.QLookVisibleRead(_cYunjing.CYunjingEditorShown);
        PDisplay.Visibility = QLook.QLookVisibleRead(_cYunjing.CYunjingDisplayShown);
        PYunjingDiwei.Visibility = QLook.QLookVisibleRead(_cYunjing.CYunjingDiweiShown);
        PYunjingViewer.IsChecked = QLook.QLookCheckedRead(_cYunjing.CYunjingPanel.CPanelViewerChecked);
        PYunjingScribe.IsChecked = QLook.QLookCheckedRead(_cYunjing.CYunjingPanel.CPanelScribeChecked);
        PYunjingVoyage.Visibility = QLook.QLookVisibleRead(_cYunjing.CYunjingPanel.CPanelViewerChecked);
        PYunjingChronicle.Visibility = QLook.QLookVisibleRead(_cYunjing.CYunjingPanel.CPanelScribeChecked);
        PYunjingMode.IsEnabled = _cYunjing.CYunjingPanel.CPanelModeEnabled;
        PYunjingBin.IsEnabled = _cYunjing.CYunjingPanel.CPanelBinEnabled;
    }

    private void PYunjingClearUpdate()
    {
        PDiweiUpdate();
    }

    private void PPlumbHandle(object sender, TextChangedEventArgs e)
    {
        _cYunjing.CYunjingShengmuFind(PPlumb.Text);
    }

    private void PFathomHandle(object sender, TextChangedEventArgs e)
    {
        _cYunjing.CYunjingYunmuFind(PFathom.Text);
    }

    private void PBeaconHandle(object sender, TextChangedEventArgs e)
    {
        _cYunjing.CYunjingXiaoyunFind(PBeacon.Text);
    }

    private void PLadderHandle(object sender, RoutedEventArgs e)
    {
        PLadderDropper.IsChecked = false;
        _cYunjing.CYunjingShengmuSet(QChoice.QChoiceOrderRead(sender));
    }

    private void PStairHandle(object sender, RoutedEventArgs e)
    {
        PStairDropper.IsChecked = false;
        _cYunjing.CYunjingYunmuSet(QChoice.QChoiceOrderRead(sender));
    }

    private void PYunjingHandle(object sender, RoutedEventArgs e)
    {
        _cYunjing.CYunjingDiweiSelect(
            QSender.QSenderSourceRead<PYunjingItem>(e)?.PYunjingItemId,
            QSender.QSenderSourceRead<PYunjingItem>(e)?.PYunjingItemFinal);
    }

    private void PXiaoyunHandle(object sender, RoutedEventArgs e)
    {
        _cYunjing.CYunjingPanel.CPanelRowSelect(QSender.QSenderSourceRead<PXiaoyunItem>(e)?.PXiaoyunItemId);
    }

    private void PYunjingFreshHandle(object sender, RoutedEventArgs e)
    {
        _cYunjing.CYunjingPanel.CPanelEntryCreate();
    }

    private void PYunjingScribeHandle(object sender, RoutedEventArgs e)
    {
        _cYunjing.CYunjingPanel.CPanelScribeToggle(ReferenceEquals(sender, PYunjingScribe));
    }

    private void PYunjingStoreHandle(object sender, RoutedEventArgs e)
    {
        _cYunjing.CYunjingEditor.CEditorEntrySave();
    }

    private void PYunjingBinHandle(object sender, RoutedEventArgs e)
    {
        _cYunjing.CYunjingPanel.CPanelEntryDelete();
    }

    private void PYunjingPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cYunjing.CYunjingPanel.CPanelPressAllowed;
    }

    private async void PYunjingPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _cYunjing.CYunjingPortraitPrint();
    }

    private async void PYunjingPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _cYunjing.CYunjingPortraitExport();
    }

    internal void PYunjingVoyageShow(bool past, bool future)
    {
        PYunjingEarlier.IsEnabled = past;
        PYunjingLater.IsEnabled = future;
    }

    private void PYunjingRetreatHandle(object sender, RoutedEventArgs e)
    {
        _pYunjingHost.PWindowAtelier.CAtelierNavigation.CNavigationStationUndo();
    }

    private void PYunjingAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _pYunjingHost.PWindowAtelier.CAtelierNavigation.CNavigationStationRedo();
    }

    private void PYunjingUndoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.QChronicleUndoObserve();
    }

    private void PYunjingRedoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.QChronicleRedoObserve();
    }

    private void PYunjingChronicleUpdate()
    {
        (bool undo, bool redo) = _cYunjing.CYunjingEditor.CEditorDesk.CDeskChronicleRead();
        PYunjingBackward.IsEnabled = undo;
        PYunjingForward.IsEnabled = redo;
    }
}
