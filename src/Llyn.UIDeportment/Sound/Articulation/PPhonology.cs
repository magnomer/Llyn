using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using Llyn.Core;

namespace Llyn.UIDeportment;

public class PPhonology : UserControl
{
    private readonly ObservableCollection<PInventoryItem> _pInventoryList = [];

    private PWindow _pPhonologyHost = null!;

    private LPhonology _lPhonology = null!;

    public PPhonology()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Sound/Articulation/PPhonology.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        PPhonologyPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        PPhonologyPress.Command = ApplicationCommands.Print;

        PChoice.PChoiceDropperAttach(PSequenceDropper, PSequenceDropdown, PSequence);
        PChoice.PChoiceDropperAttach(PLensDropper, PLensDropdown, PLensDropper);

        PSequenceIcon.PIconSource = PIcon.PIconResolve("sort", 24);
        PLensIcon.PIconSource = PIcon.PIconResolve("filter", 24);
        PPhonologyBinIcon.PIconSource = PIcon.PIconResolve("delete", 24);
        PArticulationHelper.Tag = PIcon.PIconResolve("articulation", 24);
        PPhonologyFresh.Tag = PIcon.PIconResolve("new", 24);
        PPhonologyStore.Tag = PIcon.PIconResolve("save", 24);
        PPhonologyEarlier.Tag = PIcon.PIconResolve("retreat", 24);
        PPhonologyLater.Tag = PIcon.PIconResolve("advance", 24);
        PPhonologyBackward.Tag = PIcon.PIconResolve("undo", 24);
        PPhonologyForward.Tag = PIcon.PIconResolve("redo", 24);
        PPhonologyPortrait.Tag = PIcon.PIconResolve("export", 24);
        PPhonologyPress.Tag = PIcon.PIconResolve("print", 24);
        PPhonologyViewer.Tag = PIcon.PIconResolve("view", 24);
        PPhonologyScribe.Tag = PIcon.PIconResolve("edit", 24);

        PLookItem.PLookItemAttach(PInventory, PInventoryItem.PInventoryItemApply);
        PInventory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PInventoryHandle));

        PProbe.TextChanged += PProbeHandle;
        PArticulationHelper.Checked += PArticulationFoldHandle;
        PArticulationHelper.Unchecked += PArticulationFoldHandle;
        PPhonologyFresh.Click += PPhonologyFreshHandle;
        PPhonologyStore.Click += PPhonologyStoreHandle;
        PPhonologyEarlier.Click += PPhonologyRetreatHandle;
        PPhonologyLater.Click += PPhonologyAdvanceHandle;
        PPhonologyBackward.Click += PPhonologyUndoHandle;
        PPhonologyForward.Click += PPhonologyRedoHandle;
        PPhonologyViewer.Click += PPhonologyScribeHandle;
        PPhonologyScribe.Click += PPhonologyScribeHandle;
        PPhonologyBin.Click += PPhonologyBinHandle;
    }

    private Rectangle PArticulationSeam => (Rectangle)FindName(nameof(PArticulationSeam));

    private Border PSequence => (Border)FindName(nameof(PSequence));

    private ToggleButton PSequenceDropper => (ToggleButton)FindName(nameof(PSequenceDropper));

    private PIconImage PSequenceIcon => (PIconImage)FindName(nameof(PSequenceIcon));

    private TextBox PProbe => (TextBox)FindName(nameof(PProbe));

    private ToggleButton PLensDropper => (ToggleButton)FindName(nameof(PLensDropper));

    private PIconImage PLensIcon => (PIconImage)FindName(nameof(PLensIcon));

    private FrameworkElement PLensMark => (FrameworkElement)FindName(nameof(PLensMark));

    private Popup PSequenceDropdown => (Popup)FindName(nameof(PSequenceDropdown));

    private StackPanel PSequenceList => (StackPanel)FindName(nameof(PSequenceList));

    private Popup PLensDropdown => (Popup)FindName(nameof(PLensDropdown));

    private StackPanel PLensList => (StackPanel)FindName(nameof(PLensList));

    private ToggleButton PArticulationHelper => (ToggleButton)FindName(nameof(PArticulationHelper));

    private Button PPhonologyFresh => (Button)FindName(nameof(PPhonologyFresh));

    private Button PPhonologyStore => (Button)FindName(nameof(PPhonologyStore));

    private StackPanel PPhonologyVoyage => (StackPanel)FindName(nameof(PPhonologyVoyage));

    private Button PPhonologyEarlier => (Button)FindName(nameof(PPhonologyEarlier));

    private Button PPhonologyLater => (Button)FindName(nameof(PPhonologyLater));

    private StackPanel PPhonologyChronicle => (StackPanel)FindName(nameof(PPhonologyChronicle));

    private Button PPhonologyBackward => (Button)FindName(nameof(PPhonologyBackward));

    private Button PPhonologyForward => (Button)FindName(nameof(PPhonologyForward));

    private Button PPhonologyPortrait => (Button)FindName(nameof(PPhonologyPortrait));

    private Button PPhonologyPress => (Button)FindName(nameof(PPhonologyPress));

    private Border PPhonologyMode => (Border)FindName(nameof(PPhonologyMode));

    private RadioButton PPhonologyViewer => (RadioButton)FindName(nameof(PPhonologyViewer));

    private RadioButton PPhonologyScribe => (RadioButton)FindName(nameof(PPhonologyScribe));

    private PArticulation PArticulation => (PArticulation)FindName(nameof(PArticulation));

    private ItemsControl PInventory => (ItemsControl)FindName(nameof(PInventory));

    private TextBlock PInventoryEmpty => (TextBlock)FindName(nameof(PInventoryEmpty));

    private PDisplay PDisplay => (PDisplay)FindName(nameof(PDisplay));

    private PEditor PEditor => (PEditor)FindName(nameof(PEditor));

    private Button PPhonologyBin => (Button)FindName(nameof(PPhonologyBin));

    private PIconImage PPhonologyBinIcon => (PIconImage)FindName(nameof(PPhonologyBinIcon));

    internal void PPhonologyAttach(PWindow host)
    {
        _pPhonologyHost = host;
        _lPhonology = host.PWindowDeportment.LWindowPhonologyCreate(
            host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm),
            PPhonologyShownCheck,
            PPhonologyDiscardConfirm,
            host.PWindowDeleteConfirm);
        _lPhonology.LPhonologyEditor.LEditorStateChanged += PPhonologyStoreUpdate;
        _lPhonology.LPhonologyPanel.LPanelChanged += PPhonologyModeUpdate;
        _lPhonology.LPhonologyPanel.LPanelRowsChanged += PInventoryUpdate;
        _lPhonology.LPhonologyPanel.LPanelFailed += host.PWindowFailureShow;

        PInventory.ItemsSource = _pInventoryList;

        PDisplay.PDisplayAttach(host, _lPhonology.LPhonologyEditor.LEditorLectern);

        PEditor.PEditorAttach(host, _lPhonology.LPhonologyEditor);

        PEditor.PEditorChronicleChanged += PPhonologyChronicleUpdate;

        PArticulation.PArticulationAttach(PProbe, PEditor.PPronunciationField);

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PPhonologyPressHandle, PPhonologyPressCheck));
        CommandBindings.Add(
            new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, PPhonologyPortraitHandle,
            PPhonologyPressCheck));
    }

    private void PPhonologyStoreUpdate()
    {
        PPhonologyStore.IsEnabled = _lPhonology.LPhonologyEditor.LEditorStorable;
    }

    internal async void PPhonologyVistaRestore()
    {
        _lPhonology.LPhonologyVistaRestore(_pPhonologyHost.PWindowDeportment);
        _lPhonology.LPhonologyPanel.LPanelObserverAttach(
            LSubject.LSubjectVista, LObserver.LObserverCreate(this, _lPhonology.LPhonologyPanel.LPanelRowsUpdate));
        _lPhonology.LPhonologyPanel.LPanelObserverAttach(
            LSubject.LSubjectWorkspace, LObserver.LObserverCreate(this, PPhonologyWorkspaceUpdate));
        _lPhonology.LPhonologyPanel.LPanelObserverAttach(
            LSubject.LSubjectEntry, LObserver.LObserverCreate(this, _lPhonology.LPhonologyPanel.LPanelEntryHandle));
        _lPhonology.LPhonologyPanel.LPanelObserverAttach(
            LSubject.LSubjectReflex, LObserver.LObserverCreate(this, _lPhonology.LPhonologyPanel.LPanelRowsUpdate));
        _lPhonology.LPhonologyPanel.LPanelObserverAttach(
            LSubject.LSubjectSettings, LObserver.LObserverCreate(this, _lPhonology.LPhonologyPanel.LPanelRowsUpdate));
        _lPhonology.LPhonologyPanel.LPanelChosenAttach(
            LSubject.LSubjectEntry, LObserver.LObserverCreate(this, _lPhonology.LPhonologyPanel.LPanelDraftUpdate));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderBuild(
            PSequenceList,
            "Sequence",
            PSequenceHandle,
            [
                LCatalogOrder.LCatalogOrderHeadword,
                LCatalogOrder.LCatalogOrderReverse,
                LCatalogOrder.LCatalogOrderSound,
                LCatalogOrder.LCatalogOrderPending,
            ]);
        PChoice.PChoiceOrderApply(PSequenceDropdown, _lPhonology.LPhonologyPanel.LPanelOrder);
        PLensUpdate();

        await LEnsignImage.LEnsignLoad(_pPhonologyHost.PWindowDeportment);

        PChoice.PChoiceFilterBuild(
            PLensList,
            _pPhonologyHost.PWindowDeportment.LWindowLanguageRead(),
            _lPhonology.LPhonologyPanel.LPanelFilter,
            PLensHandle);
        _lPhonology.LPhonologyQuerySet(PProbe.Text);
        _lPhonology.LPhonologyPanel.LPanelRowsUpdate();
    }

    private async void PPhonologyWorkspaceUpdate()
    {
        await LEnsignImage.LEnsignLoad(_pPhonologyHost.PWindowDeportment);
        _lPhonology.LPhonologyPanel.LPanelClear();
    }

    internal bool PPhonologyDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PPhonologyChangeCheck()
    {
        return _lPhonology.LPhonologyPanel.LPanelChangeCheck();
    }

    private bool PPhonologyShownCheck()
    {
        return IsVisible;
    }

    internal bool PPhonologyLeaveConfirm()
    {
        return _lPhonology.LPhonologyPanel.LPanelLeaveConfirm();
    }

    private bool PPhonologyDiscardConfirm()
    {
        return _pPhonologyHost.PWindowDiscardConfirm(true, PEditor.PEditorDraftFinish);
    }

    internal void PPhonologyScribeRestore(bool editing)
    {
        _lPhonology.LPhonologyPanel.LPanelScribeRestore(editing);
    }

    internal void PPhonologyClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }

    private void PInventoryUpdate()
    {
        LSplice.LSpliceApply(
            _pInventoryList,
            PInventoryItem.PInventoryItemBuild(_lPhonology.LPhonologyRowsRead()),
            PInventoryItem.PInventoryItemMatch,
            PInventoryItem.PInventoryItemSync);
        PInventoryEmpty.Visibility = PLook.PLookVisibleRead(_lPhonology.LPhonologyInventoryEmpty);
    }

    private void PPhonologyModeUpdate()
    {
        PEditor.Visibility = PLook.PLookVisibleRead(_lPhonology.LPhonologyPanel.LPanelEditing);
        PDisplay.Visibility = PLook.PLookVisibleRead(_lPhonology.LPhonologyPanel.LPanelViewerChecked);
        PPhonologyViewer.IsChecked = PLook.PLookCheckedRead(_lPhonology.LPhonologyPanel.LPanelViewerChecked);
        PPhonologyScribe.IsChecked = PLook.PLookCheckedRead(_lPhonology.LPhonologyPanel.LPanelScribeChecked);
        PPhonologyVoyage.Visibility = PLook.PLookVisibleRead(_lPhonology.LPhonologyPanel.LPanelViewerChecked);
        PPhonologyChronicle.Visibility = PLook.PLookVisibleRead(_lPhonology.LPhonologyPanel.LPanelScribeChecked);
        PPhonologyMode.IsEnabled = _lPhonology.LPhonologyPanel.LPanelModeEnabled;
        PPhonologyBin.IsEnabled = _lPhonology.LPhonologyPanel.LPanelBinEnabled;
    }

    private void PLensUpdate()
    {
        PLensMark.Visibility = PLook.PLookVisibleRead(_lPhonology.LPhonologyFilterActive);
    }

    private void PArticulationFoldHandle(object sender, RoutedEventArgs e)
    {
        PArticulation.Visibility = PLook.PLookVisibleRead(PLook.PLookCheckedRead(PArticulationHelper.IsChecked));
        PArticulationSeam.Visibility = PArticulation.Visibility;
    }

    private void PProbeHandle(object sender, TextChangedEventArgs e)
    {
        _lPhonology.LPhonologyQuerySet(PProbe.Text);
    }

    private void PSequenceHandle(object sender, RoutedEventArgs e)
    {
        PSequenceDropper.IsChecked = false;
        _lPhonology.LPhonologyOrderSet(LChoice.LChoiceOrderRead(sender));
    }

    private void PLensHandle(object sender, RoutedEventArgs e)
    {
        _lPhonology.LPhonologyFilterSet(LChoice.LChoiceFilterRead(PLensList));
        PLensUpdate();
    }

    private void PInventoryHandle(object sender, RoutedEventArgs e)
    {
        _pPhonologyHost.PVoyageRecord();
        _lPhonology.LPhonologyPanel.LPanelRowSelect(
            PSender.PSenderSourceRead<PInventoryItem>(e)?.PInventoryItemId);
    }

    internal long PPhonologyVoyageRead()
    {
        return _lPhonology.LPhonologyPanel.LPanelVoyageRead();
    }

    internal void PInventoryEntryShow(long id)
    {
        _lPhonology.LPhonologyPanel.LPanelRowShow(id);
    }

    private void PPhonologyFreshHandle(object sender, RoutedEventArgs e)
    {
        _lPhonology.LPhonologyPanel.LPanelFreshStart();
    }

    private void PPhonologyScribeHandle(object sender, RoutedEventArgs e)
    {
        _lPhonology.LPhonologyPanel.LPanelScribeSet(ReferenceEquals(sender, PPhonologyScribe));
    }

    private void PPhonologyStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PPhonologyBinHandle(object sender, RoutedEventArgs e)
    {
        _lPhonology.LPhonologyPanel.LPanelDelete();
    }

    private void PPhonologyPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lPhonology.LPhonologyPanel.LPanelPressAllowed;
    }

    private async void PPhonologyPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pPhonologyHost.PWindowPressRun(_lPhonology.LPhonologyPortraitPrint);
    }

    private async void PPhonologyPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pPhonologyHost.PWindowPortraitExport(
            _lPhonology.LPhonologyFileRead(), _lPhonology.LPhonologyPortraitExport);
    }

    internal void PPhonologyVoyageShow(bool past, bool future)
    {
        PPhonologyEarlier.IsEnabled = past;
        PPhonologyLater.IsEnabled = future;
    }

    private void PPhonologyRetreatHandle(object sender, RoutedEventArgs e)
    {
        _pPhonologyHost.PVoyageRetreatRun();
    }

    private void PPhonologyAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _pPhonologyHost.PVoyageAdvanceRun();
    }

    private void PPhonologyUndoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleUndo();
    }

    private void PPhonologyRedoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleRedo();
    }

    private void PPhonologyChronicleUpdate()
    {
        (bool undo, bool redo) = PEditor.PEditorChronicleRead();
        PPhonologyBackward.IsEnabled = undo;
        PPhonologyForward.IsEnabled = redo;
    }
}
