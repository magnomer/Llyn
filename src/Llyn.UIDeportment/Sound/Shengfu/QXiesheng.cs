using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QXiesheng
{
    private readonly ObservableCollection<QGroveItem> _qGroveList = [];

    private readonly ObservableCollection<QKindredItem> _qKindredList = [];

    private readonly UserControl _qXieshengSurface;

    private readonly QStem _qStem;

    private PWindow _qXieshengHost = null!;

    private CXiesheng _cXiesheng = null!;

    private LEditor _lEditor = null!;

    internal QXiesheng(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qXieshengSurface = surface;
        _qStem = new QStem(QXieshengStem);

        surface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QXieshengPressHandle, QXieshengPressCheck));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QXieshengPortraitHandle, QXieshengPressCheck));
        QXieshengPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        QXieshengPress.Command = ApplicationCommands.Print;

        QChoice.QChoiceDropperAttach(QRungDropper, QRungDropdown, QRungBar);

        QRungIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        QXieshengBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        QLodestar.SetResourceReference(QField.QFieldHintProperty, "Lodestar.Search");
        QSextant.SetResourceReference(QField.QFieldHintProperty, "Sextant.Search");
        QXieshengFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        QXieshengStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        QXieshengEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        QXieshengLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        QXieshengBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        QXieshengForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        QXieshengPortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        QXieshengPress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        QXieshengViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        QXieshengScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        QLookItem.QLookItemAttach(QGrove, QGroveItem.QGroveItemApply);
        QLookItem.QLookItemAttach(QKindred, QKindredItem.QKindredItemApply);
        QGrove.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QGroveHandle));
        QKindred.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QKindredHandle));

        QLodestar.TextChanged += QLodestarHandle;
        QSextant.TextChanged += QSextantHandle;
        QXieshengFresh.Click += QXieshengFreshHandle;
        QXieshengStore.Click += QXieshengStoreHandle;
        QXieshengEarlier.Click += QXieshengRetreatHandle;
        QXieshengLater.Click += QXieshengAdvanceHandle;
        QXieshengBackward.Click += QXieshengUndoHandle;
        QXieshengForward.Click += QXieshengRedoHandle;
        QXieshengViewer.Click += QXieshengScribeHandle;
        QXieshengScribe.Click += QXieshengScribeHandle;
        QXieshengBin.Click += QXieshengBinHandle;
    }

    private Border QRungBar => QContract.QContractFind<Border>(_qXieshengSurface, "PRungBar");

    private ToggleButton QRungDropper => QContract.QContractFind<ToggleButton>(_qXieshengSurface, "PRungDropper");

    private QIconImage QRungIcon => QContract.QContractFind<QIconImage>(_qXieshengSurface, "PRungIcon");

    private TextBox QLodestar => QContract.QContractFind<TextBox>(_qXieshengSurface, "PLodestar");

    private Popup QRungDropdown => QContract.QContractFind<Popup>(_qXieshengSurface, "PRungDropdown");

    private StackPanel QRungList => QContract.QContractFind<StackPanel>(_qXieshengSurface, "PRungList");

    private ItemsControl QGrove => QContract.QContractFind<ItemsControl>(_qXieshengSurface, "PGrove");

    private TextBlock QGroveEmpty => QContract.QContractFind<TextBlock>(_qXieshengSurface, "PGroveEmpty");

    private TextBox QSextant => QContract.QContractFind<TextBox>(_qXieshengSurface, "PSextant");

    private ItemsControl QKindred => QContract.QContractFind<ItemsControl>(_qXieshengSurface, "PKindred");

    private TextBlock QKindredEmpty => QContract.QContractFind<TextBlock>(_qXieshengSurface, "PKindredEmpty");

    private Button QXieshengFresh => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengFresh");

    private Button QXieshengStore => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengStore");

    private StackPanel QXieshengVoyage => QContract.QContractFind<StackPanel>(_qXieshengSurface, "PXieshengVoyage");

    private Button QXieshengEarlier => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengEarlier");

    private Button QXieshengLater => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengLater");

    private StackPanel QXieshengChronicle =>
        QContract.QContractFind<StackPanel>(_qXieshengSurface, "PXieshengChronicle");

    private Button QXieshengBackward => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengBackward");

    private Button QXieshengForward => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengForward");

    private Button QXieshengPortrait => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengPortrait");

    private Button QXieshengPress => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengPress");

    private Border QXieshengMode => QContract.QContractFind<Border>(_qXieshengSurface, "PXieshengMode");

    private RadioButton QXieshengViewer => QContract.QContractFind<RadioButton>(_qXieshengSurface, "PXieshengViewer");

    private RadioButton QXieshengScribe => QContract.QContractFind<RadioButton>(_qXieshengSurface, "PXieshengScribe");

    private PDisplay QXieshengDisplay => QContract.QContractFind<PDisplay>(_qXieshengSurface, "PDisplay");

    private UserControl QXieshengStem => QContract.QContractFind<UserControl>(_qXieshengSurface, "PXieshengStem");

    private PEditor QXieshengEditor => QContract.QContractFind<PEditor>(_qXieshengSurface, "PEditor");

    private Button QXieshengBin => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengBin");

    private QIconImage QXieshengBinIcon => QContract.QContractFind<QIconImage>(_qXieshengSurface, "PXieshengBinIcon");

    internal void QXieshengAttach(PWindow host)
    {
        _qXieshengHost = host;
        _cXiesheng = host.PWindowForge.QForgeXieshengCreate(QXieshengShownCheck, host.PWindowEnvoy);
        _cXiesheng.CXieshengStemOpened += QLodestarRefine;
        _lEditor = new LEditor(_cXiesheng.CXieshengEditor);
        CPanel panel = _cXiesheng.CXieshengPanel;
        QLectern lectern = new(_lEditor.LEditorStudio.CEditorDisplay, panel);
        panel.CPanelChanged += QXieshengModeUpdate;
        panel.CPanelRowsChanged += QKindredUpdate;
        panel.CPanelCleared += QXieshengClearUpdate;
        _lEditor.LEditorStudio.CEditorDesk.CDeskStateChanged += QXieshengStoreUpdate;
        _cXiesheng.CXieshengChanged += QXieshengColumnUpdate;

        QGrove.ItemsSource = _qGroveList;
        QKindred.ItemsSource = _qKindredList;

        QXieshengDisplay.PDisplayAttach(host, lectern);
        _qStem.QStemAttach(host.PWindowAtelier, _cXiesheng);

        QXieshengEditor.PEditorAttach(host, _lEditor);

        QXieshengEditor.PEditorChronicleChanged += QXieshengChronicleUpdate;
    }

    private void QXieshengStoreUpdate()
    {
        QXieshengStore.IsEnabled = _lEditor.LEditorStorable;
    }

    internal void QXieshengVistaRestore()
    {
        UserControl surface = _qXieshengSurface;
        _cXiesheng.CXieshengObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(surface, _cXiesheng.CXieshengRowsResonate));
        _cXiesheng.CXieshengObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(surface, QXieshengWorkspaceUpdate));
        _cXiesheng.CXieshengObserverAttach(
            CSubject.CSubjectFanqie, LObserver.LObserverCreate<CBulletin>(surface, _cXiesheng.CXieshengRowsResonate));
        _cXiesheng.CXieshengObserverAttach(
            CSubject.CSubjectSettings,
            LObserver.LObserverCreate<CBulletin>(surface, _cXiesheng.CXieshengRowsResonate));
        CPanel panel = _cXiesheng.CXieshengPanel;
        panel.CPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelRowsResonate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, _cXiesheng.CXieshengEntryResonate));
        panel.CPanelChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelDraftResonate));
        QXieshengEditor.PEditorVistaRestore();
        QChoice.QChoiceOrderBuild(
            QRungList,
            "Rung",
            QRungHandle,
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderUsage,
            ]);
        QChoice.QChoiceOrderApply(QRungDropdown, _cXiesheng.CXieshengOrder);
        _cXiesheng.CXieshengGroveFind(QLodestar.Text);
        _cXiesheng.CXieshengKindredFind(QSextant.Text);
        _cXiesheng.CXieshengRowsResonate();
    }

    private async void QXieshengWorkspaceUpdate()
    {
        await LEnsignImage.LEnsignLoad(_qXieshengHost.PWindowAtelier);
        _cXiesheng.CXieshengStemCancel();
    }

    private void QLodestarRefine()
    {
        QLodestar.Text = string.Empty;
    }

    internal void QXieshengClose()
    {
        QXieshengEditor.PEditorClose();
        QXieshengDisplay.PDisplayClose();
    }

    private bool QXieshengShownCheck()
    {
        return _qXieshengSurface.IsVisible;
    }

    private void QXieshengColumnUpdate()
    {
        LSplice.LSpliceApply(
            _qGroveList,
            QGroveItem.QGroveItemBuild(_cXiesheng.CXieshengGroveRead()),
            QGroveItem.QGroveItemMatch,
            QGroveItem.QGroveItemSync);
        QGroveEmpty.SetResourceReference(TextBlock.TextProperty, _cXiesheng.CXieshengGroveKey);
        QGroveEmpty.Visibility = QLook.QLookVisibleRead(_cXiesheng.CXieshengGroveEmpty);
        QStemUpdate();
        QXieshengModeUpdate();
    }

    private void QKindredUpdate()
    {
        LSplice.LSpliceApply(
            _qKindredList,
            QKindredItem.QKindredItemBuild(_cXiesheng.CXieshengKindredRead()),
            QKindredItem.QKindredItemMatch,
            QKindredItem.QKindredItemSync);
        QKindredEmpty.SetResourceReference(TextBlock.TextProperty, _cXiesheng.CXieshengKindredKey);
        QKindredEmpty.Visibility = QLook.QLookVisibleRead(_cXiesheng.CXieshengKindredEmpty);
    }

    private void QStemUpdate()
    {
        _qStem.QStemShow(_cXiesheng.CXieshengStemRead());
    }

    private void QXieshengModeUpdate()
    {
        QXieshengEditor.Visibility = QLook.QLookVisibleRead(_cXiesheng.CXieshengEditorShown);
        QXieshengDisplay.Visibility = QLook.QLookVisibleRead(_cXiesheng.CXieshengDisplayShown);
        QXieshengStem.Visibility = QLook.QLookVisibleRead(_cXiesheng.CXieshengStemShown);
        QXieshengViewer.IsChecked = QLook.QLookCheckedRead(_cXiesheng.CXieshengPanel.CPanelViewerChecked);
        QXieshengScribe.IsChecked = QLook.QLookCheckedRead(_cXiesheng.CXieshengPanel.CPanelScribeChecked);
        QXieshengVoyage.Visibility = QLook.QLookVisibleRead(_cXiesheng.CXieshengPanel.CPanelViewerChecked);
        QXieshengChronicle.Visibility = QLook.QLookVisibleRead(_cXiesheng.CXieshengPanel.CPanelScribeChecked);
        QXieshengMode.IsEnabled = _cXiesheng.CXieshengPanel.CPanelModeEnabled;
        QXieshengBin.IsEnabled = _cXiesheng.CXieshengPanel.CPanelBinEnabled;
    }

    private void QXieshengClearUpdate()
    {
        QStemUpdate();
    }

    private void QLodestarHandle(object sender, TextChangedEventArgs e)
    {
        _cXiesheng.CXieshengGroveFind(QLodestar.Text);
    }

    private void QSextantHandle(object sender, TextChangedEventArgs e)
    {
        _cXiesheng.CXieshengKindredFind(QSextant.Text);
    }

    private void QRungHandle(object sender, RoutedEventArgs e)
    {
        QRungDropper.IsChecked = false;
        _cXiesheng.CXieshengGroveSet(QChoice.QChoiceOrderRead(sender));
    }

    private void QGroveHandle(object sender, RoutedEventArgs e)
    {
        _cXiesheng.CXieshengStemSelect(QSender.QSenderSourceRead<QGroveItem>(e)?.QGroveItemId);
    }

    private void QKindredHandle(object sender, RoutedEventArgs e)
    {
        _cXiesheng.CXieshengPanel.CPanelRowSelect(QSender.QSenderSourceRead<QKindredItem>(e)?.QKindredItemId);
    }

    private void QXieshengFreshHandle(object sender, RoutedEventArgs e)
    {
        _cXiesheng.CXieshengPanel.CPanelEntryCreate();
    }

    private void QXieshengScribeHandle(object sender, RoutedEventArgs e)
    {
        _cXiesheng.CXieshengPanel.CPanelScribeToggle(ReferenceEquals(sender, QXieshengScribe));
    }

    private void QXieshengStoreHandle(object sender, RoutedEventArgs e)
    {
        QXieshengEditor.PEditorEntrySave();
    }

    private void QXieshengBinHandle(object sender, RoutedEventArgs e)
    {
        _cXiesheng.CXieshengPanel.CPanelEntryDelete();
    }

    private void QXieshengPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cXiesheng?.CXieshengPanel.CPanelPressAllowed ?? false;
    }

    private async void QXieshengPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _cXiesheng.CXieshengPortraitPrint();
    }

    private async void QXieshengPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _cXiesheng.CXieshengPortraitExport();
    }

    internal void QXieshengVoyageShow(bool past, bool future)
    {
        QXieshengEarlier.IsEnabled = past;
        QXieshengLater.IsEnabled = future;
    }

    private void QXieshengRetreatHandle(object sender, RoutedEventArgs e)
    {
        _qXieshengHost.PWindowAtelier.CAtelierNavigation.CNavigationStationUndo();
    }

    private void QXieshengAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _qXieshengHost.PWindowAtelier.CAtelierNavigation.CNavigationStationRedo();
    }

    private void QXieshengUndoHandle(object sender, RoutedEventArgs e)
    {
        QXieshengEditor.QChronicleUndo();
    }

    private void QXieshengRedoHandle(object sender, RoutedEventArgs e)
    {
        QXieshengEditor.QChronicleRedo();
    }

    private void QXieshengChronicleUpdate()
    {
        (bool undo, bool redo) = QXieshengEditor.PEditorChronicleRead();
        QXieshengBackward.IsEnabled = undo;
        QXieshengForward.IsEnabled = redo;
    }
}
