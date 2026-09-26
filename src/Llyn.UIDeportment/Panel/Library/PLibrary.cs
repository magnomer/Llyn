using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIDeportment;

public class PLibrary : UserControl
{
    private PWindow _pLibraryHost = null!;

    private LLibrary _lLibrary = null!;

    public PLibrary()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Library/PLibrary.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, PLibraryPressHandle, PLibraryPressCheck));
        CommandBindings.Add(
            new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, PLibraryPortraitHandle, PLibraryPressCheck));
        PLibraryPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        PLibraryPress.Command = ApplicationCommands.Print;

        PChoice.PChoiceDropperAttach(POrderDropper, POrderDropdown, POrder);
        PChoice.PChoiceDropperAttach(PSieveDropper, PSieveDropdown, PSieveDropper);

        POrderIcon.PIconSource = PIcon.PIconResolve("sort", 24);
        PSieveIcon.PIconSource = PIcon.PIconResolve("filter", 24);
        PLibraryBinIcon.PIconSource = PIcon.PIconResolve("delete", 24);
        PLibraryFresh.Tag = PIcon.PIconResolve("new", 24);
        PLibraryStore.Tag = PIcon.PIconResolve("save", 24);
        PLibraryEarlier.Tag = PIcon.PIconResolve("retreat", 24);
        PLibraryLater.Tag = PIcon.PIconResolve("advance", 24);
        PLibraryBackward.Tag = PIcon.PIconResolve("undo", 24);
        PLibraryForward.Tag = PIcon.PIconResolve("redo", 24);
        PLibraryMarkup.Tag = PIcon.PIconResolve("import", 24);
        PLibraryPortrait.Tag = PIcon.PIconResolve("export", 24);
        PLibraryPress.Tag = PIcon.PIconResolve("print", 24);
        PLibraryViewer.Tag = PIcon.PIconResolve("view", 24);
        PLibraryScribe.Tag = PIcon.PIconResolve("edit", 24);

        PInquiry.TextChanged += PInquiryHandle;
        PLibraryFresh.Click += PLibraryFreshHandle;
        PLibraryStore.Click += PLibraryStoreHandle;
        PLibraryEarlier.Click += PLibraryRetreatHandle;
        PLibraryLater.Click += PLibraryAdvanceHandle;
        PLibraryBackward.Click += PLibraryUndoHandle;
        PLibraryForward.Click += PLibraryRedoHandle;
        PLibraryMarkup.Click += PLibraryMarkupHandle;
        PLibraryViewer.Click += PLibraryScribeHandle;
        PLibraryScribe.Click += PLibraryScribeHandle;
        PLibraryBin.Click += PLibraryBinHandle;

        PLookItem.PLookItemAttach(PIndex, PIndexApply);
    }

    private Border POrder => (Border)FindName(nameof(POrder));

    private ToggleButton POrderDropper => (ToggleButton)FindName(nameof(POrderDropper));

    private PIconImage POrderIcon => (PIconImage)FindName(nameof(POrderIcon));

    private TextBox PInquiry => (TextBox)FindName(nameof(PInquiry));

    private ToggleButton PSieveDropper => (ToggleButton)FindName(nameof(PSieveDropper));

    private PIconImage PSieveIcon => (PIconImage)FindName(nameof(PSieveIcon));

    private FrameworkElement PSieveMark => (FrameworkElement)FindName(nameof(PSieveMark));

    private Popup POrderDropdown => (Popup)FindName(nameof(POrderDropdown));

    private StackPanel POrderList => (StackPanel)FindName(nameof(POrderList));

    private Popup PSieveDropdown => (Popup)FindName(nameof(PSieveDropdown));

    private StackPanel PSieveList => (StackPanel)FindName(nameof(PSieveList));

    private ItemsControl PIndex => (ItemsControl)FindName(nameof(PIndex));

    private TextBlock PIndexEmpty => (TextBlock)FindName(nameof(PIndexEmpty));

    private Button PLibraryFresh => (Button)FindName(nameof(PLibraryFresh));

    private Button PLibraryStore => (Button)FindName(nameof(PLibraryStore));

    private StackPanel PLibraryVoyage => (StackPanel)FindName(nameof(PLibraryVoyage));

    private Button PLibraryEarlier => (Button)FindName(nameof(PLibraryEarlier));

    private Button PLibraryLater => (Button)FindName(nameof(PLibraryLater));

    private StackPanel PLibraryChronicle => (StackPanel)FindName(nameof(PLibraryChronicle));

    private Button PLibraryBackward => (Button)FindName(nameof(PLibraryBackward));

    private Button PLibraryForward => (Button)FindName(nameof(PLibraryForward));

    private Button PLibraryMarkup => (Button)FindName(nameof(PLibraryMarkup));

    private Button PLibraryPortrait => (Button)FindName(nameof(PLibraryPortrait));

    private Button PLibraryPress => (Button)FindName(nameof(PLibraryPress));

    private Border PLibraryMode => (Border)FindName(nameof(PLibraryMode));

    private RadioButton PLibraryViewer => (RadioButton)FindName(nameof(PLibraryViewer));

    private RadioButton PLibraryScribe => (RadioButton)FindName(nameof(PLibraryScribe));

    internal PDisplay PDisplay => (PDisplay)FindName(nameof(PDisplay));

    private PEditor PEditor => (PEditor)FindName(nameof(PEditor));

    private Button PLibraryBin => (Button)FindName(nameof(PLibraryBin));

    private PIconImage PLibraryBinIcon => (PIconImage)FindName(nameof(PLibraryBinIcon));

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
        _lLibrary.LLibraryPanel.LPanelFailed += host.PWindowFailureShow;

        _lLibrary.LLibraryIndexAttach(PIndex, PIndexEmpty);

        PDisplay.PDisplayAttach(host, _lLibrary.LLibraryEditor.LEditorLectern);

        PEditor.PEditorAttach(host, _lLibrary.LLibraryEditor);

        PEditor.PEditorChronicleChanged += PLibraryChronicleUpdate;
    }

    private void PLibraryStoreUpdate()
    {
        PLibraryStore.IsEnabled = _lLibrary.LLibraryEditor.LEditorStorable;
    }

    internal async void PLibraryVistaRestore()
    {
        _lLibrary.LLibraryVistaRestore(_pLibraryHost.PWindowDeportment);
        _lLibrary.LLibraryPanel.LPanelObserverAttach(
            LSubject.LSubjectVista, LObserver.LObserverCreate(this, _lLibrary.LLibraryPanel.LPanelRowsUpdate));
        _lLibrary.LLibraryPanel.LPanelObserverAttach(
            LSubject.LSubjectWorkspace, LObserver.LObserverCreate(this, PLibraryWorkspaceUpdate));
        _lLibrary.LLibraryPanel.LPanelObserverAttach(
            LSubject.LSubjectEntry, LObserver.LObserverCreate(this, _lLibrary.LLibraryPanel.LPanelEntryHandle));
        _lLibrary.LLibraryPanel.LPanelObserverAttach(
            LSubject.LSubjectReflex, LObserver.LObserverCreate(this, _lLibrary.LLibraryPanel.LPanelRowsUpdate));
        _lLibrary.LLibraryPanel.LPanelObserverAttach(
            LSubject.LSubjectSettings, LObserver.LObserverCreate(this, _lLibrary.LLibraryPanel.LPanelRowsUpdate));
        _lLibrary.LLibraryPanel.LPanelChosenAttach(
            LSubject.LSubjectEntry, LObserver.LObserverCreate(this, _lLibrary.LLibraryPanel.LPanelDraftUpdate));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderBuild(POrderList, "Order", POrderHandle, LIndex.LIndexOrder);
        PChoice.PChoiceOrderApply(POrderDropdown, _lLibrary.LLibraryPanel.LPanelOrder);
        _lLibrary.LLibrarySieveShow(PSieveMark);

        await LEnsignImage.LEnsignLoad(_pLibraryHost.PWindowDeportment);

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
        await LEnsignImage.LEnsignLoad(_pLibraryHost.PWindowDeportment);
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
        PLibraryVoyage.Visibility = PLook.PLookVisibleRead(_lLibrary.LLibraryPanel.LPanelViewerChecked);
        PLibraryChronicle.Visibility = PLook.PLookVisibleRead(_lLibrary.LLibraryPanel.LPanelScribeChecked);
        PLibraryMode.IsEnabled = _lLibrary.LLibraryPanel.LPanelModeEnabled;
        PLibraryBin.IsEnabled = _lLibrary.LLibraryPanel.LPanelBinEnabled;
    }

    private string? PLibraryMarkupOpen()
    {
        return PMarkup.PMarkupOpen(_pLibraryHost.PWindowSurface);
    }

    private IReadOnlyList<LMarkupIntake>? PLibraryCustomsShow(LMarkupCargo cargo)
    {
        return PSCustoms.PSCustomsShow(
            _pLibraryHost.PWindowSurface, _pLibraryHost.PWindowDeportment, cargo.LMarkupCargoEntry);
    }

    private void PLibraryOmissionShow(IReadOnlyList<LMarkupOmission> omissions)
    {
        PSCustoms.PSCustomsOmissionShow(_pLibraryHost.PWindowSurface, omissions);
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

    private void PIndexApply(FrameworkElement container, object item, string? change)
    {
        LIndexItem.LIndexItemApply(container, item, change);

        if (PLook.PLookPartFind<Button>(container, "PIndexRow") is Button row)
        {
            row.Click -= PIndexHandle;
            row.Click += PIndexHandle;
        }
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
        e.CanExecute = _lLibrary?.LLibraryPanel.LPanelPressAllowed ?? false;
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
