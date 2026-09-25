using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PLibrary : UserControl
{
    private PWindow _pLibraryHost = null!;

    private LLibrary _lLibrary = null!;

    public PLibrary()
    {
        InitializeComponent();
    }

    internal void PLibraryAttach(PWindow host)
    {
        _pLibraryHost = host;
        _lLibrary = host.PWindowDeportment.LWindowLibraryCreate(
            host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm),
            PLibraryShownCheck,
            PLibraryDiscardConfirm,
            host.PWindowDeleteConfirm);
        _lLibrary.LLibraryFailed += host.PWindowFailureShow;
        _lLibrary.LLibraryEditor.LEditorStateChanged += PLibraryStoreUpdate;
        _lLibrary.LLibraryPanel.LPanelChanged += PLibraryModeUpdate;
        _lLibrary.LLibraryPanel.LPanelCleared += PLibraryClearUpdate;
        _lLibrary.LLibraryPanel.LPanelDraftChanged += PLibraryEntryUpdate;
        _lLibrary.LLibraryPanel.LPanelFailed += host.PWindowFailureShow;

        _lLibrary.LLibraryIndexAttach(PIndex, PIndexEmpty, PEnsign.PEnsignFind);

        PDisplay.PDisplayAttach(host, _lLibrary.LLibraryEditor.LEditorDisplay);

        PEditor.PEditorAttach(host, _lLibrary.LLibraryEditor);

        PEditor.PEditorChronicleChanged += PLibraryChronicleUpdate;

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PLibraryPressHandle, PLibraryPressCheck));
        CommandBindings.Add(
            new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, PLibraryPortraitHandle, PLibraryPressCheck));
    }

    private void PLibraryStoreUpdate()
    {
        PLibraryStore.IsEnabled = _lLibrary.LLibraryEditor.LEditorStorable;
    }

    internal async void PLibraryVistaRestore()
    {
        _lLibrary.LLibraryVistaRestore(_pLibraryHost.PWindowDeportment);
        _lLibrary.LLibraryPanel.LPanelObserverAttach(
            LSubject.LSubjectVista, PObserver.PObserverCreate(this, _lLibrary.LLibraryPanel.LPanelRowsUpdate));
        _lLibrary.LLibraryPanel.LPanelObserverAttach(
            LSubject.LSubjectWorkspace, PObserver.PObserverCreate(this, PLibraryWorkspaceUpdate));
        _lLibrary.LLibraryPanel.LPanelObserverAttach(
            LSubject.LSubjectEntry, PObserver.PObserverCreate(this, _lLibrary.LLibraryPanel.LPanelEntryHandle));
        _lLibrary.LLibraryPanel.LPanelObserverAttach(
            LSubject.LSubjectReflex, PObserver.PObserverCreate(this, _lLibrary.LLibraryPanel.LPanelRowsUpdate));
        _lLibrary.LLibraryPanel.LPanelObserverAttach(
            LSubject.LSubjectSettings, PObserver.PObserverCreate(this, _lLibrary.LLibraryPanel.LPanelRowsUpdate));
        _lLibrary.LLibraryPanel.LPanelChosenAttach(
            LSubject.LSubjectEntry, PObserver.PObserverCreate(this, _lLibrary.LLibraryPanel.LPanelDraftUpdate));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderBuild(POrderList, "Order", POrderHandle, LIndex.LIndexOrder);
        PChoice.PChoiceOrderApply(POrderDropdown, _lLibrary.LLibraryPanel.LPanelOrder);
        _lLibrary.LLibrarySieveShow(PSieveMark);

        await PEnsign.PEnsignLoad(_pLibraryHost.PWindowDeportment);

        PChoice.PChoiceFilterBuild(
            PSieveList,
            _pLibraryHost.PWindowDeportment.LWindowLanguageRead(),
            _lLibrary.LLibraryPanel.LPanelFilter,
            PSieveHandle);
        _lLibrary.LLibraryInquirySet(PInquiry.Text);
        _lLibrary.LLibraryPanel.LPanelRowsUpdate();
    }

    private async void PLibraryWorkspaceUpdate()
    {
        await PEnsign.PEnsignLoad(_pLibraryHost.PWindowDeportment);
        _lLibrary.LLibraryPanel.LPanelClear();
    }

    internal bool PLibraryDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PLibraryChangeCheck()
    {
        return _lLibrary.LLibraryPanel.LPanelChangeCheck();
    }

    internal bool PLibraryLeaveConfirm()
    {
        return _lLibrary.LLibraryPanel.LPanelLeaveConfirm();
    }

    internal long PLibraryVoyageRead()
    {
        return _lLibrary.LLibraryVoyageRead();
    }

    internal void PIndexEntryShow(long id)
    {
        _lLibrary.LLibraryPanel.LPanelRowShow(id);
    }

    private bool PLibraryShownCheck()
    {
        return IsVisible;
    }

    private bool PLibraryDiscardConfirm()
    {
        return _pLibraryHost.PWindowDiscardConfirm(true, PEditor.PEditorDraftFinish);
    }

    internal void PLibraryScribeRestore(bool editing)
    {
        _lLibrary.LLibraryPanel.LPanelScribeRestore(editing);
    }

    internal void PLibraryClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }

    private void PLibraryModeUpdate()
    {
        PEditor.Visibility = PLook.PLookVisibleRead(_lLibrary.LLibraryPanel.LPanelEditing);
        PDisplay.Visibility = PLook.PLookVisibleRead(_lLibrary.LLibraryPanel.LPanelViewerChecked);
        PLibraryViewer.IsChecked = PLook.PLookCheckedRead(_lLibrary.LLibraryPanel.LPanelViewerChecked);
        PLibraryScribe.IsChecked = PLook.PLookCheckedRead(_lLibrary.LLibraryPanel.LPanelScribeChecked);
        PLibraryMode.IsEnabled = _lLibrary.LLibraryPanel.LPanelModeEnabled;
        PLibraryBin.IsEnabled = _lLibrary.LLibraryPanel.LPanelBinEnabled;
    }

    private void PLibraryClearUpdate()
    {
        PDisplay.PDisplayClear();
    }

    private void PLibraryEntryUpdate(LDraft draft)
    {
        PDisplay.PDisplayShow(draft.LDraftContent);
    }

    private string? PLibraryMarkupOpen()
    {
        return PMarkup.PMarkupOpen(_pLibraryHost);
    }

    private IReadOnlyList<LMarkupIntake>? PLibraryCustomsShow(LMarkupCargo cargo)
    {
        return PSCustoms.PSCustomsShow(_pLibraryHost, _pLibraryHost.PWindowDeportment, cargo.LMarkupCargoEntry);
    }

    private void PLibraryOmissionShow(IReadOnlyList<LMarkupOmission> omissions)
    {
        PSCustoms.PSCustomsOmissionShow(_pLibraryHost, omissions);
    }

    private void PInquiryHandle(object sender, TextChangedEventArgs e)
    {
        _lLibrary.LLibraryInquirySet(PInquiry.Text);
    }

    private void POrderHandle(object sender, RoutedEventArgs e)
    {
        _lLibrary.LLibraryOrderHandle(sender, POrderDropper);
    }

    private void PSieveHandle(object sender, RoutedEventArgs e)
    {
        _lLibrary.LLibrarySieveHandle(PSieveList, PSieveMark);
    }

    private void PIndexHandle(object sender, RoutedEventArgs e)
    {
        _pLibraryHost.PVoyageRecord();
        _lLibrary.LLibraryIndexSelect(sender);
    }

    private void PLibraryFreshHandle(object sender, RoutedEventArgs e)
    {
        _lLibrary.LLibraryPanel.LPanelFreshStart();
    }

    private void PLibraryScribeHandle(object sender, RoutedEventArgs e)
    {
        _lLibrary.LLibraryPanel.LPanelScribeSet(ReferenceEquals(sender, PLibraryScribe));
    }

    private void PLibraryStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PLibraryBinHandle(object sender, RoutedEventArgs e)
    {
        _lLibrary.LLibraryPanel.LPanelDelete();
    }

    private async void PLibraryMarkupHandle(object sender, RoutedEventArgs e)
    {
        await _lLibrary.LLibraryMarkupStart(PLibraryMarkupOpen, PLibraryCustomsShow, PLibraryOmissionShow);
    }

    private void PLibraryPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lLibrary.LLibraryPanel.LPanelPressAllowed;
    }

    private async void PLibraryPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pLibraryHost.PWindowPressRun(_lLibrary.LLibraryPortraitPrint);
    }

    private async void PLibraryPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pLibraryHost.PWindowPortraitExport(_lLibrary.LLibraryFileRead(), _lLibrary.LLibraryPortraitExport);
    }

    internal void PLibraryVoyageShow(bool past, bool future)
    {
        PLibraryEarlier.IsEnabled = past;
        PLibraryLater.IsEnabled = future;
    }

    private void PLibraryRetreatHandle(object sender, RoutedEventArgs e)
    {
        _pLibraryHost.PVoyageRetreatRun();
    }

    private void PLibraryAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _pLibraryHost.PVoyageAdvanceRun();
    }

    private void PLibraryUndoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleUndo();
    }

    private void PLibraryRedoHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PChronicleRedo();
    }

    private void PLibraryChronicleUpdate()
    {
        (bool undo, bool redo) = PEditor.PEditorChronicleRead();
        PLibraryBackward.IsEnabled = undo;
        PLibraryForward.IsEnabled = redo;
    }
}
