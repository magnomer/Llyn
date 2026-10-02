using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QYunjing
{
    private readonly ObservableCollection<PYunjingItem> _qShengmuList = [];

    private readonly ObservableCollection<PYunjingItem> _qYunmuList = [];

    private readonly ObservableCollection<PXiaoyunItem> _qXiaoyunList = [];

    private readonly UserControl _qYunjingSurface;

    private readonly QEditor _qYunjingEditor;

    private readonly QDisplay _qYunjingDisplay;

    private readonly QDiwei _qDiwei;

    private QWindow _qYunjingHost = null!;

    private CYunjing _cYunjing = null!;

    internal QYunjing(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qYunjingSurface = surface;
        _qYunjingEditor = new QEditor(QContract.QContractFind<FrameworkElement>(surface, "PEditor"));
        _qYunjingDisplay = new QDisplay(QContract.QContractFind<FrameworkElement>(surface, "PDisplay"));
        _qDiwei = new QDiwei(QYunjingDiwei);

        QYunjingPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        QYunjingPress.Command = ApplicationCommands.Print;

        QChoice.QChoiceDropperAttach(QLadderDropper, QLadderDropdown, QLadder);
        QChoice.QChoiceDropperAttach(QStairDropper, QStairDropdown, QStair);

        QLadderIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        QStairIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        QYunjingBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        QPlumb.SetResourceReference(QField.QFieldHintProperty, "Plumb.Search");
        QFathom.SetResourceReference(QField.QFieldHintProperty, "Fathom.Search");
        QBeacon.SetResourceReference(QField.QFieldHintProperty, "Beacon.Search");
        QYunjingFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        QYunjingStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        QYunjingEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        QYunjingLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        QYunjingBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        QYunjingForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        QYunjingPortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        QYunjingPress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        QYunjingViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        QYunjingScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        QLookItem.QLookItemAttach(QShengmu, PYunjingItem.PYunjingItemRefine);
        QLookItem.QLookItemAttach(QYunmu, PYunjingItem.PYunjingItemRefine);
        QLookItem.QLookItemAttach(QXiaoyun, PXiaoyunItem.PXiaoyunItemRefine);
        QShengmu.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QYunjingObserve));
        QYunmu.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QYunjingObserve));
        QXiaoyun.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QXiaoyunObserve));

        QPlumb.TextChanged += QPlumbObserve;
        QFathom.TextChanged += QFathomObserve;
        QBeacon.TextChanged += QBeaconObserve;
        QYunjingFresh.Click += QYunjingFreshObserve;
        QYunjingStore.Click += QYunjingStoreObserve;
        QYunjingEarlier.Click += QYunjingRetreatObserve;
        QYunjingLater.Click += QYunjingAdvanceObserve;
        QYunjingBackward.Click += QYunjingUndoObserve;
        QYunjingForward.Click += QYunjingRedoObserve;
        QYunjingViewer.Click += QYunjingViewerObserve;
        QYunjingScribe.Click += QYunjingScribeObserve;
        QYunjingBin.Click += QYunjingBinObserve;
    }

    private Border QLadder => QContract.QContractFind<Border>(_qYunjingSurface, "PLadder");

    private ToggleButton QLadderDropper => QContract.QContractFind<ToggleButton>(_qYunjingSurface, "PLadderDropper");

    private QIconImage QLadderIcon => QContract.QContractFind<QIconImage>(_qYunjingSurface, "PLadderIcon");

    private TextBox QPlumb => QContract.QContractFind<TextBox>(_qYunjingSurface, "PPlumb");

    private Popup QLadderDropdown => QContract.QContractFind<Popup>(_qYunjingSurface, "PLadderDropdown");

    private StackPanel QLadderList => QContract.QContractFind<StackPanel>(_qYunjingSurface, "PLadderList");

    private ItemsControl QShengmu => QContract.QContractFind<ItemsControl>(_qYunjingSurface, "PShengmu");

    private TextBlock QShengmuEmpty => QContract.QContractFind<TextBlock>(_qYunjingSurface, "PShengmuEmpty");

    private Border QStair => QContract.QContractFind<Border>(_qYunjingSurface, "PStair");

    private ToggleButton QStairDropper => QContract.QContractFind<ToggleButton>(_qYunjingSurface, "PStairDropper");

    private QIconImage QStairIcon => QContract.QContractFind<QIconImage>(_qYunjingSurface, "PStairIcon");

    private TextBox QFathom => QContract.QContractFind<TextBox>(_qYunjingSurface, "PFathom");

    private Popup QStairDropdown => QContract.QContractFind<Popup>(_qYunjingSurface, "PStairDropdown");

    private StackPanel QStairList => QContract.QContractFind<StackPanel>(_qYunjingSurface, "PStairList");

    private ItemsControl QYunmu => QContract.QContractFind<ItemsControl>(_qYunjingSurface, "PYunmu");

    private TextBlock QYunmuEmpty => QContract.QContractFind<TextBlock>(_qYunjingSurface, "PYunmuEmpty");

    private TextBox QBeacon => QContract.QContractFind<TextBox>(_qYunjingSurface, "PBeacon");

    private ItemsControl QXiaoyun => QContract.QContractFind<ItemsControl>(_qYunjingSurface, "PXiaoyun");

    private TextBlock QXiaoyunEmpty => QContract.QContractFind<TextBlock>(_qYunjingSurface, "PXiaoyunEmpty");

    private Button QYunjingFresh => QContract.QContractFind<Button>(_qYunjingSurface, "PYunjingFresh");

    private Button QYunjingStore => QContract.QContractFind<Button>(_qYunjingSurface, "PYunjingStore");

    private StackPanel QYunjingVoyage => QContract.QContractFind<StackPanel>(_qYunjingSurface, "PYunjingVoyage");

    private Button QYunjingEarlier => QContract.QContractFind<Button>(_qYunjingSurface, "PYunjingEarlier");

    private Button QYunjingLater => QContract.QContractFind<Button>(_qYunjingSurface, "PYunjingLater");

    private StackPanel QYunjingChronicle =>
        QContract.QContractFind<StackPanel>(_qYunjingSurface, "PYunjingChronicle");

    private Button QYunjingBackward => QContract.QContractFind<Button>(_qYunjingSurface, "PYunjingBackward");

    private Button QYunjingForward => QContract.QContractFind<Button>(_qYunjingSurface, "PYunjingForward");

    private Button QYunjingPortrait => QContract.QContractFind<Button>(_qYunjingSurface, "PYunjingPortrait");

    private Button QYunjingPress => QContract.QContractFind<Button>(_qYunjingSurface, "PYunjingPress");

    private Border QYunjingMode => QContract.QContractFind<Border>(_qYunjingSurface, "PYunjingMode");

    private RadioButton QYunjingViewer => QContract.QContractFind<RadioButton>(_qYunjingSurface, "PYunjingViewer");

    private RadioButton QYunjingScribe => QContract.QContractFind<RadioButton>(_qYunjingSurface, "PYunjingScribe");

    private UserControl QYunjingDiwei => QContract.QContractFind<UserControl>(_qYunjingSurface, "PYunjingDiwei");

    private Button QYunjingBin => QContract.QContractFind<Button>(_qYunjingSurface, "PYunjingBin");

    private QIconImage QYunjingBinIcon => QContract.QContractFind<QIconImage>(_qYunjingSurface, "PYunjingBinIcon");

    internal void QYunjingIntroduce(QWindow host)
    {
        _qYunjingHost = host;
        _cYunjing = CYunjing.CYunjingCreate(
            host.QWindowAtelier,
            QYunjingShownCheck,
            host.QWindowEnvoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        CPanel panel = _cYunjing.CYunjingPanel;
        QLectern lectern = new(_cYunjing.CYunjingEditor.CEditorDisplay, panel);
        QChoice.QChoiceOrderBuild(QLadderList, "Ladder", QLadderObserve, CYunjing.CYunjingOrderRead());
        QChoice.QChoiceOrderBuild(QStairList, "Stair", QStairObserve, CYunjing.CYunjingOrderRead());
        _cYunjing.CYunjingDiweiOpened += QYunjingQueryRefine;
        _cYunjing.CYunjingChanged += QShengmuRefine;
        _cYunjing.CYunjingChanged += QYunmuRefine;
        _cYunjing.CYunjingChanged += QYunjingDiweiRefine;
        _cYunjing.CYunjingChanged += QYunjingModeRefine;
        _cYunjing.CYunjingWorkspaceChanged += QYunjingWorkspaceRefine;
        panel.CPanelChanged += QYunjingModeRefine;
        panel.CPanelRowsChanged += QXiaoyunRefine;
        panel.CPanelCleared += QYunjingDiweiRefine;
        _cYunjing.CYunjingEditor.CEditorDesk.CDeskStateChanged += QYunjingStoreRefine;

        QShengmu.ItemsSource = _qShengmuList;
        QYunmu.ItemsSource = _qYunmuList;
        QXiaoyun.ItemsSource = _qXiaoyunList;

        _qYunjingDisplay.QDisplayIntroduce(host, lectern);
        _qDiwei.QDiweiIntroduce(_cYunjing);

        _qYunjingEditor.QEditorIntroduce(host, _cYunjing.CYunjingEditor);

        _qYunjingEditor.QEditorChronicleChanged += QYunjingChronicleRefine;

        _qYunjingSurface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QYunjingPressObserve, QYunjingPressRefine));
        _qYunjingSurface.CommandBindings.Add(
            new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, QYunjingPortraitObserve, QYunjingPressRefine));
    }

    private void QYunjingStoreRefine()
    {
        QYunjingStore.IsEnabled = _cYunjing.CYunjingEditor.CEditorDesk.CDeskStorable;
    }

    internal void QYunjingVistaRefine()
    {
        QChoice.QChoiceOrderApply(QLadderDropdown, _cYunjing.CYunjingShengmuOrder);
        QChoice.QChoiceOrderApply(QStairDropdown, _cYunjing.CYunjingYunmuOrder);
        QYunjingModeRefine();
        QXiaoyunRefine();
    }

    private async void QYunjingWorkspaceRefine()
    {
        QXiaoyunRefine((await _cYunjing.CYunjingXiaoyunLoad(QEnsignImage.QEnsignDraw)).CEnsignSheetRows);
    }

    private void QYunjingQueryRefine()
    {
        QPlumb.Text = string.Empty;
        QFathom.Text = string.Empty;
    }

    internal void QYunjingExitRefine()
    {
        _qYunjingEditor.QEditorPlayerRefine();
    }

    private bool QYunjingShownCheck()
    {
        return _qYunjingSurface.IsVisible;
    }

    internal void QShengmuRefine()
    {
        QSplice.QSpliceRefine(
            _qShengmuList,
            PYunjingItem.PYunjingItemBuild(_cYunjing.CYunjingShengmuRead()),
            PYunjingItem.PYunjingItemMatch,
            PYunjingItem.PYunjingItemSync);
        QShengmuEmpty.SetResourceReference(TextBlock.TextProperty, _cYunjing.CYunjingShengmuKey);
        QShengmuEmpty.Visibility = QLook.QLookVisibleRead(_cYunjing.CYunjingShengmuEmpty);
    }

    internal void QYunmuRefine()
    {
        QSplice.QSpliceRefine(
            _qYunmuList,
            PYunjingItem.PYunjingItemBuild(_cYunjing.CYunjingYunmuRead()),
            PYunjingItem.PYunjingItemMatch,
            PYunjingItem.PYunjingItemSync);
        QYunmuEmpty.SetResourceReference(TextBlock.TextProperty, _cYunjing.CYunjingYunmuKey);
        QYunmuEmpty.Visibility = QLook.QLookVisibleRead(_cYunjing.CYunjingYunmuEmpty);
    }

    private void QXiaoyunRefine()
    {
        QXiaoyunRefine(_cYunjing.CYunjingXiaoyunRead());
    }

    private void QXiaoyunRefine(IReadOnlyList<CVistaRow> rows)
    {
        QSplice.QSpliceRefine(
            _qXiaoyunList,
            PXiaoyunItem.PXiaoyunItemBuild(rows),
            PXiaoyunItem.PXiaoyunItemMatch,
            PXiaoyunItem.PXiaoyunItemSync);
        QXiaoyunEmpty.SetResourceReference(TextBlock.TextProperty, _cYunjing.CYunjingXiaoyunKey);
        QXiaoyunEmpty.Visibility = QLook.QLookVisibleRead(_cYunjing.CYunjingXiaoyunEmpty);
    }

    internal void QYunjingDiweiRefine()
    {
        _qDiwei.QDiweiRefine(_cYunjing.CYunjingDiweiRead(), _cYunjing.CYunjingDiweiKey);
    }

    private void QYunjingModeRefine()
    {
        CPanel panel = _cYunjing.CYunjingPanel;
        _qYunjingEditor.QEditorVisibleRefine(QLook.QLookVisibleRead(_cYunjing.CYunjingEditorShown));
        _qYunjingDisplay.QDisplayVisibleRefine(QLook.QLookVisibleRead(_cYunjing.CYunjingDisplayShown));
        QYunjingDiwei.Visibility = QLook.QLookVisibleRead(_cYunjing.CYunjingDiweiShown);
        QYunjingViewer.IsChecked = panel.CPanelViewerChecked;
        QYunjingScribe.IsChecked = panel.CPanelScribeChecked;
        QYunjingVoyage.Visibility = QLook.QLookVisibleRead(panel.CPanelViewerChecked);
        QYunjingChronicle.Visibility = QLook.QLookVisibleRead(panel.CPanelScribeChecked);
        QYunjingMode.IsEnabled = panel.CPanelModeEnabled;
        QYunjingBin.IsEnabled = panel.CPanelBinEnabled;
    }

    private void QPlumbObserve(object sender, TextChangedEventArgs e)
    {
        _cYunjing.CYunjingShengmuFind(QPlumb.Text);
    }

    private void QFathomObserve(object sender, TextChangedEventArgs e)
    {
        _cYunjing.CYunjingYunmuFind(QFathom.Text);
    }

    private void QBeaconObserve(object sender, TextChangedEventArgs e)
    {
        _cYunjing.CYunjingXiaoyunFind(QBeacon.Text);
    }

    private void QLadderObserve(object sender, RoutedEventArgs e)
    {
        _cYunjing.CYunjingShengmuSet(QChoice.QChoiceOrderRead(sender));
        QLadderRefine();
    }

    private void QLadderRefine()
    {
        QLadderDropper.IsChecked = false;
    }

    private void QStairObserve(object sender, RoutedEventArgs e)
    {
        _cYunjing.CYunjingYunmuSet(QChoice.QChoiceOrderRead(sender));
        QStairRefine();
    }

    private void QStairRefine()
    {
        QStairDropper.IsChecked = false;
    }

    private void QYunjingObserve(object sender, RoutedEventArgs e)
    {
        _cYunjing.CYunjingDiweiSelect(
            QSender.QSenderSourceRead<PYunjingItem>(e)?.PYunjingItemId,
            QSender.QSenderSourceRead<PYunjingItem>(e)?.PYunjingItemFinal);
    }

    private void QXiaoyunObserve(object sender, RoutedEventArgs e)
    {
        _cYunjing.CYunjingPanel.CPanelRowSelect(QSender.QSenderSourceRead<PXiaoyunItem>(e)?.PXiaoyunItemId);
    }

    private void QYunjingFreshObserve(object sender, RoutedEventArgs e)
    {
        _cYunjing.CYunjingPanel.CPanelEntryCreate();
    }

    private void QYunjingViewerObserve(object sender, RoutedEventArgs e)
    {
        _cYunjing.CYunjingPanel.CPanelScribeToggle(false);
    }

    private void QYunjingScribeObserve(object sender, RoutedEventArgs e)
    {
        _cYunjing.CYunjingPanel.CPanelScribeToggle(true);
    }

    private void QYunjingStoreObserve(object sender, RoutedEventArgs e)
    {
        _cYunjing.CYunjingEditor.CEditorEntrySave();
    }

    private void QYunjingBinObserve(object sender, RoutedEventArgs e)
    {
        _cYunjing.CYunjingPanel.CPanelEntryDelete();
    }

    private void QYunjingPressRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cYunjing.CYunjingPanel.CPanelPressAllowed;
    }

    private async void QYunjingPressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cYunjing.CYunjingPortraitPrint();
    }

    private async void QYunjingPortraitObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cYunjing.CYunjingPortraitExport();
    }

    internal void QYunjingVoyageRefine(bool past, bool future)
    {
        QYunjingEarlier.IsEnabled = past;
        QYunjingLater.IsEnabled = future;
    }

    private void QYunjingRetreatObserve(object sender, RoutedEventArgs e)
    {
        _qYunjingHost.QWindowAtelier.CAtelierNavigation.CNavigationStationUndo();
    }

    private void QYunjingAdvanceObserve(object sender, RoutedEventArgs e)
    {
        _qYunjingHost.QWindowAtelier.CAtelierNavigation.CNavigationStationRedo();
    }

    private void QYunjingUndoObserve(object sender, RoutedEventArgs e)
    {
        _qYunjingEditor.QChronicleUndoObserve();
    }

    private void QYunjingRedoObserve(object sender, RoutedEventArgs e)
    {
        _qYunjingEditor.QChronicleRedoObserve();
    }

    private void QYunjingChronicleRefine()
    {
        (bool undo, bool redo) = _cYunjing.CYunjingEditor.CEditorDesk.CDeskChronicleRead();
        QYunjingBackward.IsEnabled = undo;
        QYunjingForward.IsEnabled = redo;
    }
}
