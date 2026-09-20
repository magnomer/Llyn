using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PLibrary : UserControl
{
    private readonly ObservableCollection<PIndexItem> _pIndexList = [];

    private PWindow _pLibraryHost = null!;

    private LLibrary _lLibrary = null!;

    public PLibrary()
    {
        InitializeComponent();
    }

    internal void PLibraryAttach(PWindow host, LEngine engine)
    {
        _pLibraryHost = host;
        _lLibrary = new LLibrary(
            engine, engine,
            new LEditor(engine, engine, engine, engine, host.PWindowUnreadableConfirm),
            PLibraryShownCheck,
            PLibraryDiscardConfirm,
            host.PWindowDeleteConfirm);
        _lLibrary.LLibraryFailed += host.PWindowFailureShow;
        _lLibrary.LLibraryEditor.LEditorStateChanged += PLibraryStoreUpdate;
        _lLibrary.LLibraryPanel.LPanelChanged += PLibraryModeUpdate;
        _lLibrary.LLibraryPanel.LPanelRowsChanged += PIndexUpdate;
        _lLibrary.LLibraryPanel.LPanelCleared += PLibraryClearUpdate;
        _lLibrary.LLibraryPanel.LPanelDraftChanged += PLibraryEntryUpdate;
        _lLibrary.LLibraryPanel.LPanelFailed += host.PWindowFailureShow;

        PIndex.ItemsSource = _pIndexList;

        PDisplay.PDisplayAttach(host, _lLibrary.LLibraryEditor.LEditorDisplay);

        PEditor.PEditorAttach(host, _lLibrary.LLibraryEditor);

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PLibraryPressHandle, PLibraryPressCheck));
        CommandBindings.Add(new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, PLibraryPortraitHandle, PLibraryPressCheck));
    }

    private void PLibraryStoreUpdate()
    {
        PLibraryStore.IsEnabled = _lLibrary.LLibraryEditor.LEditorStorable;
    }

    internal async void PLibraryVistaRestore()
    {
        _lLibrary.LLibraryVistaRestore(_pLibraryHost.PWindowPosture);
        _lLibrary.LLibraryPanel.LPanelObserverAttach(
            LSubject.LSubjectVista, new PObserver(this, _lLibrary.LLibraryPanel.LPanelRowsUpdate));
        _lLibrary.LLibraryPanel.LPanelObserverAttach(
            LSubject.LSubjectWorkspace, new PObserver(this, PLibraryWorkspaceUpdate));
        _lLibrary.LLibraryPanel.LPanelObserverAttach(
            LSubject.LSubjectEntry, new PObserver(this, _lLibrary.LLibraryPanel.LPanelEntryHandle));
        _lLibrary.LLibraryPanel.LPanelObserverAttach(
            LSubject.LSubjectReflex, new PObserver(this, _lLibrary.LLibraryPanel.LPanelRowsUpdate));
        _lLibrary.LLibraryPanel.LPanelObserverAttach(
            LSubject.LSubjectSettings, new PObserver(this, _lLibrary.LLibraryPanel.LPanelRowsUpdate));
        _lLibrary.LLibraryPanel.LPanelChosenAttach(
            LSubject.LSubjectEntry, new PObserver(this, _lLibrary.LLibraryPanel.LPanelDraftUpdate));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderApply(POrderDropdown, _lLibrary.LLibraryPanel.LPanelOrder);
        PSieveUpdate();

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
        _lLibrary.LLibraryPanel.LPanelReset();
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
        _lLibrary.LLibraryPanel.LPanelRowSelect(id);
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

    private void PIndexUpdate()
    {
        PSplice.PSpliceApply(
            _pIndexList,
            PIndexItem.PIndexItemBuild(_lLibrary.LLibraryRowsRead()),
            PIndexItem.PIndexItemMatch,
            PIndexItem.PIndexItemSync);
        PIndexEmpty.Visibility = PLook.PLookVisibleRead(_lLibrary.LLibraryIndexEmpty);
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

    private void PSieveUpdate()
    {
        PSieveMark.Visibility = PLook.PLookVisibleRead(_lLibrary.LLibrarySieveActive);
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
        POrderDropper.IsChecked = false;
        _lLibrary.LLibraryOrderSet(PSender.PSenderTagRead(sender));
    }

    private void PSieveHandle(object sender, RoutedEventArgs e)
    {
        _lLibrary.LLibrarySieveSet(PChoice.PChoiceFilterRead(PSieveList));
        PSieveUpdate();
    }

    private void PIndexHandle(object sender, RoutedEventArgs e)
    {
        _lLibrary.LLibraryPanel.LPanelRowSelect(PSender.PSenderItemRead<PIndexItem>(sender)?.PIndexItemId);
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
        await _pLibraryHost.PWindowPressRun(
            ticket => _lLibrary.LLibraryPortraitPrint(_pLibraryHost.PWindowLabelRead(), ticket));
    }

    private async void PLibraryPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _pLibraryHost.PWindowPortraitExport(_lLibrary.LLibraryFileRead(), _lLibrary.LLibraryPortraitExport);
    }
}
