using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QPhonology
{
    private readonly ObservableCollection<QInventoryItem> _qInventoryList = [];

    private readonly UserControl _qPhonologySurface;

    private readonly QArticulation _qArticulation;

    private PWindow _qPhonologyHost = null!;

    private CPhonology _cPhonology = null!;

    private LEditor _lEditor = null!;

    internal QPhonology(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qPhonologySurface = surface;
        _qArticulation = new QArticulation(QPhonologyArticulation);

        surface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QPhonologyPressHandle, QPhonologyPressCheck));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QPhonologyPortraitHandle, QPhonologyPressCheck));
        QPhonologyPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        QPhonologyPress.Command = ApplicationCommands.Print;

        QChoice.QChoiceDropperAttach(QSequenceDropper, QSequenceDropdown, QSequence);
        QChoice.QChoiceDropperAttach(QLensDropper, QLensDropdown, QLensDropper);

        QSequenceIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        QLensIcon.QIconSource = QIcon.QIconResolve("filter", 24);
        QPhonologyBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        QProbe.SetResourceReference(QField.QFieldHintProperty, "Sound.Search");
        QArticulationHelper.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("articulation", 24));
        QPhonologyFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        QPhonologyStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        QPhonologyEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        QPhonologyLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        QPhonologyBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        QPhonologyForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        QPhonologyPortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        QPhonologyPress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        QPhonologyViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        QPhonologyScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        QLookItem.QLookItemAttach(QInventory, QInventoryItem.QInventoryItemApply);
        QInventory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QInventoryHandle));

        QProbe.TextChanged += QProbeHandle;
        QArticulationHelper.Checked += QArticulationFoldHandle;
        QArticulationHelper.Unchecked += QArticulationFoldHandle;
        QPhonologyFresh.Click += QPhonologyFreshHandle;
        QPhonologyStore.Click += QPhonologyStoreHandle;
        QPhonologyEarlier.Click += QPhonologyRetreatHandle;
        QPhonologyLater.Click += QPhonologyAdvanceHandle;
        QPhonologyBackward.Click += QPhonologyUndoHandle;
        QPhonologyForward.Click += QPhonologyRedoHandle;
        QPhonologyViewer.Click += QPhonologyScribeHandle;
        QPhonologyScribe.Click += QPhonologyScribeHandle;
        QPhonologyBin.Click += QPhonologyBinHandle;
    }

    private Rectangle QArticulationSeam =>
        QContract.QContractFind<Rectangle>(_qPhonologySurface, "PArticulationSeam");

    private Border QSequence => QContract.QContractFind<Border>(_qPhonologySurface, "PSequence");

    private ToggleButton QSequenceDropper =>
        QContract.QContractFind<ToggleButton>(_qPhonologySurface, "PSequenceDropper");

    private QIconImage QSequenceIcon => QContract.QContractFind<QIconImage>(_qPhonologySurface, "PSequenceIcon");

    private TextBox QProbe => QContract.QContractFind<TextBox>(_qPhonologySurface, "PProbe");

    private ToggleButton QLensDropper => QContract.QContractFind<ToggleButton>(_qPhonologySurface, "PLensDropper");

    private QIconImage QLensIcon => QContract.QContractFind<QIconImage>(_qPhonologySurface, "PLensIcon");

    private FrameworkElement QLensMark => QContract.QContractFind<FrameworkElement>(_qPhonologySurface, "PLensMark");

    private Popup QSequenceDropdown => QContract.QContractFind<Popup>(_qPhonologySurface, "PSequenceDropdown");

    private StackPanel QSequenceList => QContract.QContractFind<StackPanel>(_qPhonologySurface, "PSequenceList");

    private Popup QLensDropdown => QContract.QContractFind<Popup>(_qPhonologySurface, "PLensDropdown");

    private StackPanel QLensList => QContract.QContractFind<StackPanel>(_qPhonologySurface, "PLensList");

    private ToggleButton QArticulationHelper =>
        QContract.QContractFind<ToggleButton>(_qPhonologySurface, "PArticulationHelper");

    private Button QPhonologyFresh => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyFresh");

    private Button QPhonologyStore => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyStore");

    private StackPanel QPhonologyVoyage =>
        QContract.QContractFind<StackPanel>(_qPhonologySurface, "PPhonologyVoyage");

    private Button QPhonologyEarlier => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyEarlier");

    private Button QPhonologyLater => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyLater");

    private StackPanel QPhonologyChronicle =>
        QContract.QContractFind<StackPanel>(_qPhonologySurface, "PPhonologyChronicle");

    private Button QPhonologyBackward => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyBackward");

    private Button QPhonologyForward => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyForward");

    private Button QPhonologyPortrait => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyPortrait");

    private Button QPhonologyPress => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyPress");

    private Border QPhonologyMode => QContract.QContractFind<Border>(_qPhonologySurface, "PPhonologyMode");

    private RadioButton QPhonologyViewer =>
        QContract.QContractFind<RadioButton>(_qPhonologySurface, "PPhonologyViewer");

    private RadioButton QPhonologyScribe =>
        QContract.QContractFind<RadioButton>(_qPhonologySurface, "PPhonologyScribe");

    private UserControl QPhonologyArticulation =>
        QContract.QContractFind<UserControl>(_qPhonologySurface, "PArticulation");

    private ItemsControl QInventory => QContract.QContractFind<ItemsControl>(_qPhonologySurface, "PInventory");

    private TextBlock QInventoryEmpty => QContract.QContractFind<TextBlock>(_qPhonologySurface, "PInventoryEmpty");

    private PDisplay QPhonologyDisplay => QContract.QContractFind<PDisplay>(_qPhonologySurface, "PDisplay");

    private PEditor QPhonologyEditor => QContract.QContractFind<PEditor>(_qPhonologySurface, "PEditor");

    private Button QPhonologyBin => QContract.QContractFind<Button>(_qPhonologySurface, "PPhonologyBin");

    private QIconImage QPhonologyBinIcon =>
        QContract.QContractFind<QIconImage>(_qPhonologySurface, "PPhonologyBinIcon");

    internal void QPhonologyAttach(PWindow host)
    {
        _qPhonologyHost = host;
        _cPhonology = host.PWindowForge.QForgePhonologyCreate(QPhonologyShownCheck, host.PWindowEnvoy);
        _lEditor = new LEditor(_cPhonology.CPhonologyEditor);
        CPanel panel = _cPhonology.CPhonologyPanel;
        QLectern lectern = new(_lEditor.LEditorStudio.CEditorDisplay, panel);
        panel.CPanelChanged += QPhonologyModeUpdate;
        panel.CPanelRowsChanged += QInventoryUpdate;
        _lEditor.LEditorStudio.CEditorDesk.CDeskStateChanged += QPhonologyStoreUpdate;

        QInventory.ItemsSource = _qInventoryList;

        QPhonologyDisplay.PDisplayAttach(host, lectern);

        QPhonologyEditor.PEditorAttach(host, _lEditor);

        QPhonologyEditor.PEditorChronicleChanged += QPhonologyChronicleUpdate;

        _qArticulation.QArticulationAttach(QProbe, QPhonologyEditor.PPronunciationField);
    }

    private void QPhonologyStoreUpdate()
    {
        QPhonologyStore.IsEnabled = _lEditor.LEditorStorable;
    }

    internal async void QPhonologyVistaRestore()
    {
        UserControl surface = _qPhonologySurface;
        CPanel panel = _cPhonology.CPhonologyPanel;
        panel.CPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelRowsResonate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(surface, QPhonologyWorkspaceUpdate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelEntryResonate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectReflex, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelRowsResonate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectSettings, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelRowsResonate));
        panel.CPanelChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelDraftResonate));
        QPhonologyEditor.PEditorVistaRestore();
        QChoice.QChoiceOrderBuild(
            QSequenceList,
            "Sequence",
            QSequenceHandle,
            [
                CCatalogOrder.CCatalogOrderHeadword,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderSound,
                CCatalogOrder.CCatalogOrderPending,
            ]);
        QChoice.QChoiceOrderApply(QSequenceDropdown, _cPhonology.CPhonologyPanel.CPanelOrder);
        QLensUpdate();

        await LEnsignImage.LEnsignLoad(_qPhonologyHost.PWindowAtelier);

        QChoice.QChoiceFilterBuild(
            QLensList,
            _qPhonologyHost.PWindowAtelier.CAtelierCatalog.CCatalogLanguageRead(),
            _cPhonology.CPhonologyPanel.CPanelFilter,
            QLensHandle);
        _cPhonology.CPhonologyQuerySet(QProbe.Text);
        _cPhonology.CPhonologyPanel.CPanelRowsResonate();
    }

    private async void QPhonologyWorkspaceUpdate()
    {
        await LEnsignImage.LEnsignLoad(_qPhonologyHost.PWindowAtelier);
        _cPhonology.CPhonologyPanel.CPanelEntryClose();
    }

    internal bool QPhonologyDraftFinish(bool store)
    {
        return QPhonologyEditor.PEditorDraftFinish(store);
    }

    internal bool QPhonologyChangeCheck()
    {
        return _cPhonology.CPhonologyPanel.CPanelChangeCheck();
    }

    private bool QPhonologyShownCheck()
    {
        return _qPhonologySurface.IsVisible;
    }

    internal bool QPhonologyLeaveConfirm()
    {
        return _cPhonology.CPhonologyPanel.CPanelLeaveConfirm();
    }

    internal void QPhonologyScribeRestore(bool editing)
    {
        _cPhonology.CPhonologyPanel.CPanelScribeRestore(editing);
    }

    internal void QPhonologyClose()
    {
        QPhonologyEditor.PEditorClose();
        QPhonologyDisplay.PDisplayClose();
    }

    private void QInventoryUpdate()
    {
        LSplice.LSpliceApply(
            _qInventoryList,
            QInventoryItem.QInventoryItemBuild(_cPhonology.CPhonologyRowsRead()),
            QInventoryItem.QInventoryItemMatch,
            QInventoryItem.QInventoryItemSync);
        QInventoryEmpty.Visibility = QLook.QLookVisibleRead(_cPhonology.CPhonologyEmpty);
    }

    private void QPhonologyModeUpdate()
    {
        CPanel panel = _cPhonology.CPhonologyPanel;
        QPhonologyEditor.Visibility = QLook.QLookVisibleRead(panel.CPanelEditing);
        QPhonologyDisplay.Visibility = QLook.QLookVisibleRead(panel.CPanelViewerChecked);
        QPhonologyViewer.IsChecked = QLook.QLookCheckedRead(panel.CPanelViewerChecked);
        QPhonologyScribe.IsChecked = QLook.QLookCheckedRead(panel.CPanelScribeChecked);
        QPhonologyVoyage.Visibility = QLook.QLookVisibleRead(panel.CPanelViewerChecked);
        QPhonologyChronicle.Visibility = QLook.QLookVisibleRead(panel.CPanelScribeChecked);
        QPhonologyMode.IsEnabled = panel.CPanelModeEnabled;
        QPhonologyBin.IsEnabled = panel.CPanelBinEnabled;
    }

    private void QLensUpdate()
    {
        QLensMark.Visibility = QLook.QLookVisibleRead(_cPhonology.CPhonologyFiltered);
    }

    private void QArticulationFoldHandle(object sender, RoutedEventArgs e)
    {
        QPhonologyArticulation.Visibility =
            QLook.QLookVisibleRead(QLook.QLookCheckedRead(QArticulationHelper.IsChecked));
        QArticulationSeam.Visibility = QPhonologyArticulation.Visibility;
    }

    private void QProbeHandle(object sender, TextChangedEventArgs e)
    {
        _cPhonology.CPhonologyQuerySet(QProbe.Text);
    }

    private void QSequenceHandle(object sender, RoutedEventArgs e)
    {
        QSequenceDropper.IsChecked = false;
        _cPhonology.CPhonologyOrderSet(QChoice.QChoiceOrderRead(sender));
    }

    private void QLensHandle(object sender, RoutedEventArgs e)
    {
        _cPhonology.CPhonologyFilterSet(QChoice.QChoiceFilterRead(sender));
        QLensUpdate();
    }

    private void QInventoryHandle(object sender, RoutedEventArgs e)
    {
        _qPhonologyHost.PVoyageRecord();
        _cPhonology.CPhonologyPanel.CPanelRowSelect(
            QSender.QSenderSourceRead<QInventoryItem>(e)?.QInventoryItemId);
    }

    internal long QPhonologyVoyageRead()
    {
        return _cPhonology.CPhonologyPanel.CPanelChosenRead();
    }

    internal void QInventoryEntryShow(long id)
    {
        _cPhonology.CPhonologyPanel.CPanelRowOpen(id);
    }

    private void QPhonologyFreshHandle(object sender, RoutedEventArgs e)
    {
        _cPhonology.CPhonologyPanel.CPanelEntryCreate();
    }

    private void QPhonologyScribeHandle(object sender, RoutedEventArgs e)
    {
        _cPhonology.CPhonologyPanel.CPanelScribeToggle(ReferenceEquals(sender, QPhonologyScribe));
    }

    private void QPhonologyStoreHandle(object sender, RoutedEventArgs e)
    {
        QPhonologyEditor.PEditorEntrySave();
    }

    private void QPhonologyBinHandle(object sender, RoutedEventArgs e)
    {
        _cPhonology.CPhonologyPanel.CPanelEntryDelete();
    }

    private void QPhonologyPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cPhonology?.CPhonologyPanel.CPanelPressAllowed ?? false;
    }

    private async void QPhonologyPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _qPhonologyHost.PWindowPressRun(_cPhonology.CPhonologyPortraitPrint);
    }

    private async void QPhonologyPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _qPhonologyHost.PWindowPortraitExport(
            _cPhonology.CPhonologyFileRead(), _cPhonology.CPhonologyPortraitExport);
    }

    internal void QPhonologyVoyageShow(bool past, bool future)
    {
        QPhonologyEarlier.IsEnabled = past;
        QPhonologyLater.IsEnabled = future;
    }

    private void QPhonologyRetreatHandle(object sender, RoutedEventArgs e)
    {
        _qPhonologyHost.PVoyageRetreatRun();
    }

    private void QPhonologyAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _qPhonologyHost.PVoyageAdvanceRun();
    }

    private void QPhonologyUndoHandle(object sender, RoutedEventArgs e)
    {
        QPhonologyEditor.QChronicleUndo();
    }

    private void QPhonologyRedoHandle(object sender, RoutedEventArgs e)
    {
        QPhonologyEditor.QChronicleRedo();
    }

    private void QPhonologyChronicleUpdate()
    {
        (bool undo, bool redo) = QPhonologyEditor.PEditorChronicleRead();
        QPhonologyBackward.IsEnabled = undo;
        QPhonologyForward.IsEnabled = redo;
    }
}
