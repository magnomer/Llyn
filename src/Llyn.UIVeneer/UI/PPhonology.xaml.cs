using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PPhonology : UserControl
{
    private readonly ObservableCollection<PInventoryItem> _pInventoryList = [];

    private PWindow _pPhonologyHost = null!;

    private LPhonology _lPhonology = null!;

    public PPhonology()
    {
        InitializeComponent();
    }

    internal void PPhonologyAttach(PWindow host)
    {
        _pPhonologyHost = host;
        _lPhonology = host.PWindowDeportment.LWindowPhonologyCreate(
            host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm),
            PPhonologyShownCheck,
            PPhonologyLeaveConfirm,
            host.PWindowDeleteConfirm);
        _lPhonology.LPhonologyEditor.LEditorStateChanged += PPhonologyStoreUpdate;
        _lPhonology.LPhonologyPanel.LPanelChanged += PPhonologyModeUpdate;
        _lPhonology.LPhonologyPanel.LPanelRowsChanged += PInventoryUpdate;
        _lPhonology.LPhonologyPanel.LPanelCleared += PPhonologyClearUpdate;
        _lPhonology.LPhonologyPanel.LPanelDraftChanged += PPhonologyEntryUpdate;
        _lPhonology.LPhonologyPanel.LPanelFailed += host.PWindowFailureShow;

        PInventory.ItemsSource = _pInventoryList;

        PDisplay.PDisplayAttach(host, _lPhonology.LPhonologyEditor.LEditorDisplay);

        PEditor.PEditorAttach(host, _lPhonology.LPhonologyEditor);

        PArticulation.PArticulationAttach(PProbe, PEditor.PPronunciationField);

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PPhonologyPressHandle, PPhonologyPressCheck));
        CommandBindings.Add(new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, PPhonologyPortraitHandle, PPhonologyPressCheck));
    }

    private void PPhonologyStoreUpdate()
    {
        PPhonologyStore.IsEnabled = _lPhonology.LPhonologyEditor.LEditorStorable;
    }

    internal async void PPhonologyVistaRestore()
    {
        _lPhonology.LPhonologyVistaRestore(_pPhonologyHost.PWindowPosture);
        _lPhonology.LPhonologyPanel.LPanelObserverAttach(
            LSubject.LSubjectVista, new PObserver(this, _lPhonology.LPhonologyPanel.LPanelRowsUpdate));
        _lPhonology.LPhonologyPanel.LPanelObserverAttach(
            LSubject.LSubjectWorkspace, new PObserver(this, PPhonologyWorkspaceUpdate));
        _lPhonology.LPhonologyPanel.LPanelObserverAttach(
            LSubject.LSubjectEntry, new PObserver(this, _lPhonology.LPhonologyPanel.LPanelEntryHandle));
        _lPhonology.LPhonologyPanel.LPanelObserverAttach(
            LSubject.LSubjectReflex, new PObserver(this, _lPhonology.LPhonologyPanel.LPanelRowsUpdate));
        _lPhonology.LPhonologyPanel.LPanelObserverAttach(
            LSubject.LSubjectSettings, new PObserver(this, _lPhonology.LPhonologyPanel.LPanelRowsUpdate));
        _lPhonology.LPhonologyPanel.LPanelChosenAttach(
            LSubject.LSubjectEntry, new PObserver(this, _lPhonology.LPhonologyPanel.LPanelDraftUpdate));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderApply(PSequenceDropdown, _lPhonology.LPhonologyPanel.LPanelOrder);
        PLensUpdate();

        await PEnsign.PEnsignLoad(_pPhonologyHost.PWindowDeportment);

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
        await PEnsign.PEnsignLoad(_pPhonologyHost.PWindowDeportment);
        _lPhonology.LPhonologyPanel.LPanelReset();
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

    private bool PPhonologyLeaveConfirm()
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
        PSplice.PSpliceApply(
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
        PPhonologyMode.IsEnabled = _lPhonology.LPhonologyPanel.LPanelModeEnabled;
        PPhonologyBin.IsEnabled = _lPhonology.LPhonologyPanel.LPanelBinEnabled;
    }

    private void PPhonologyClearUpdate()
    {
        PDisplay.PDisplayClear();
    }

    private void PPhonologyEntryUpdate(LDraft draft)
    {
        PDisplay.PDisplayShow(draft.LDraftContent);
    }

    private void PLensUpdate()
    {
        PLensMark.Visibility = PLook.PLookVisibleRead(_lPhonology.LPhonologyFilterActive);
    }

    private void PProbeHandle(object sender, TextChangedEventArgs e)
    {
        _lPhonology.LPhonologyQuerySet(PProbe.Text);
    }

    private void PSequenceHandle(object sender, RoutedEventArgs e)
    {
        PSequenceDropper.IsChecked = false;
        _lPhonology.LPhonologyOrderSet(PSender.PSenderTagRead(sender));
    }

    private void PLensHandle(object sender, RoutedEventArgs e)
    {
        _lPhonology.LPhonologyFilterSet(PChoice.PChoiceFilterRead(PLensList));
        PLensUpdate();
    }

    private void PInventoryHandle(object sender, RoutedEventArgs e)
    {
        _lPhonology.LPhonologyPanel.LPanelRowSelect(
            PSender.PSenderItemRead<PInventoryItem>(sender)?.PInventoryItemId);
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
        await _pPhonologyHost.PWindowPressRun(
            ticket => _lPhonology.LPhonologyPortraitPrint(_pPhonologyHost.PWindowLabelRead(), ticket));
    }

    private async void PPhonologyPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pPhonologyHost.PWindowPortraitExport(
            _lPhonology.LPhonologyFileRead(), _lPhonology.LPhonologyPortraitExport);
    }
}
