using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;
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

        QChoice.QChoiceDropperAttach(POrderDropper, POrderDropdown, POrder);
        QChoice.QChoiceDropperAttach(PSieveDropper, PSieveDropdown, PSieveDropper);

        POrderIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        PSieveIcon.QIconSource = QIcon.QIconResolve("filter", 24);
        PLibraryBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        PInquiry.SetResourceReference(QField.QFieldHintProperty, "List.Search");
        PLibraryFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        PLibraryStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        PLibraryEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        PLibraryLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        PLibraryBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        PLibraryForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        PLibraryMarkup.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("import", 24));
        PLibraryPortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        PLibraryPress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        PLibraryViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        PLibraryScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

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

        QLookItem.QLookItemAttach(PIndex, PIndexApply);
    }

    private Border POrder => (Border)FindName(nameof(POrder));

    private ToggleButton POrderDropper => (ToggleButton)FindName(nameof(POrderDropper));

    private QIconImage POrderIcon => (QIconImage)FindName(nameof(POrderIcon));

    private TextBox PInquiry => (TextBox)FindName(nameof(PInquiry));

    private ToggleButton PSieveDropper => (ToggleButton)FindName(nameof(PSieveDropper));

    private QIconImage PSieveIcon => (QIconImage)FindName(nameof(PSieveIcon));

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

    private QIconImage PLibraryBinIcon => (QIconImage)FindName(nameof(PLibraryBinIcon));

    internal void PLibraryAttach(PWindow host)
    {
        _pLibraryHost = host;
        LEditor editor = host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm);
        LLectern lectern = new(editor.LEditorDisplay);
        _lLibrary = host.PWindowDeportment.LWindowLibraryCreate(
            editor,
            lectern,
            PLibraryShownCheck,
            PLibraryDiscardConfirm,
            host.PWindowDeleteConfirm);
        _lLibrary.LLibraryFailed += host.PWindowFailureShow;
        _lLibrary.LLibraryEditor.LEditorStateChanged += PLibraryStoreUpdate;
        _lLibrary.LLibraryPanel.LPanelChanged += PLibraryModeUpdate;
        _lLibrary.LLibraryPanel.LPanelFailed += host.PWindowFailureShow;

        _lLibrary.LLibraryIndexAttach(PIndex, PIndexEmpty);

        PDisplay.PDisplayAttach(host, lectern);

        PEditor.PEditorAttach(host, _lLibrary.LLibraryEditor, lectern);

        PEditor.PEditorChronicleChanged += PLibraryChronicleUpdate;
    }

    private void PLibraryStoreUpdate()
    {
        PLibraryStore.IsEnabled = _lLibrary.LLibraryEditor.LEditorStorable;
    }

    internal async void PLibraryVistaRestore()
    {
        _lLibrary.LLibraryVistaRestore(_pLibraryHost.PWindowDeportment);
        LPanel panel = _lLibrary.LLibraryPanel;
        panel.LPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(this, panel.LPanelRowsUpdate));
        panel.LPanelObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(this, PLibraryWorkspaceUpdate));
        panel.LPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(this, panel.LPanelEntryHandle));
        panel.LPanelObserverAttach(
            CSubject.CSubjectReflex, LObserver.LObserverCreate<CBulletin>(this, panel.LPanelRowsUpdate));
        panel.LPanelObserverAttach(
            CSubject.CSubjectSettings, LObserver.LObserverCreate<CBulletin>(this, panel.LPanelRowsUpdate));
        panel.LPanelChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(this, panel.LPanelDraftUpdate));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        QChoice.QChoiceOrderBuild(POrderList, "Order", POrderHandle, LIndex.LIndexOrder);
        QChoice.QChoiceOrderApply(POrderDropdown, _lLibrary.LLibraryPanel.LPanelOrder);
        _lLibrary.LLibrarySieveShow(PSieveMark);

        await LEnsignImage.LEnsignLoad(_pLibraryHost.PWindowDeportment);

        QChoice.QChoiceFilterBuild(
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
        PEditor.Visibility = QLook.QLookVisibleRead(_lLibrary.LLibraryPanel.LPanelEditing);
        PDisplay.Visibility = QLook.QLookVisibleRead(_lLibrary.LLibraryPanel.LPanelViewerChecked);
        PLibraryViewer.IsChecked = QLook.QLookCheckedRead(_lLibrary.LLibraryPanel.LPanelViewerChecked);
        PLibraryScribe.IsChecked = QLook.QLookCheckedRead(_lLibrary.LLibraryPanel.LPanelScribeChecked);
        PLibraryVoyage.Visibility = QLook.QLookVisibleRead(_lLibrary.LLibraryPanel.LPanelViewerChecked);
        PLibraryChronicle.Visibility = QLook.QLookVisibleRead(_lLibrary.LLibraryPanel.LPanelScribeChecked);
        PLibraryMode.IsEnabled = _lLibrary.LLibraryPanel.LPanelModeEnabled;
        PLibraryBin.IsEnabled = _lLibrary.LLibraryPanel.LPanelBinEnabled;
    }

    private string? PLibraryMarkupOpen()
    {
        return PMarkup.PMarkupOpen(_pLibraryHost.PWindowSurface);
    }

    private IReadOnlyList<LMarkupIntake>? PLibraryCustomsShow(LMarkupCargo cargo)
    {
        return QSCustoms.QSCustomsShow(_pLibraryHost, cargo.LMarkupCargoEntry);
    }

    private void PLibraryOmissionShow(IReadOnlyList<LMarkupOmission> omissions)
    {
        QSCustoms.QSCustomsOmissionShow(_pLibraryHost, omissions);
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

        if (QLook.QLookPartFind<Button>(container, "PIndexRow") is Button row)
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
