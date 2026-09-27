using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QLibrary
{
    private readonly UserControl _qLibrarySurface;

    private PWindow _qLibraryHost = null!;

    private LLibrary _lLibrary = null!;

    private QIndex _qIndex = null!;

    internal QLibrary(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qLibrarySurface = surface;

        surface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QLibraryPressHandle, QLibraryPressCheck));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QLibraryPortraitHandle, QLibraryPressCheck));
        QLibraryPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        QLibraryPress.Command = ApplicationCommands.Print;

        QChoice.QChoiceDropperAttach(QOrderDropper, QOrderDropdown, QOrder);
        QChoice.QChoiceDropperAttach(QSieveDropper, QSieveDropdown, QSieveDropper);

        QOrderIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        QSieveIcon.QIconSource = QIcon.QIconResolve("filter", 24);
        QLibraryBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        QInquiry.SetResourceReference(QField.QFieldHintProperty, "List.Search");
        QLibraryFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        QLibraryStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        QLibraryEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        QLibraryLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        QLibraryBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        QLibraryForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        QLibraryMarkup.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("import", 24));
        QLibraryPortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        QLibraryPress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        QLibraryViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        QLibraryScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        QInquiry.TextChanged += QInquiryHandle;
        QLibraryFresh.Click += QLibraryFreshHandle;
        QLibraryStore.Click += QLibraryStoreHandle;
        QLibraryEarlier.Click += QLibraryRetreatHandle;
        QLibraryLater.Click += QLibraryAdvanceHandle;
        QLibraryBackward.Click += QLibraryUndoHandle;
        QLibraryForward.Click += QLibraryRedoHandle;
        QLibraryMarkup.Click += QLibraryMarkupHandle;
        QLibraryViewer.Click += QLibraryScribeHandle;
        QLibraryScribe.Click += QLibraryScribeHandle;
        QLibraryBin.Click += QLibraryBinHandle;
    }

    private Border QOrder => QContract.QContractFind<Border>(_qLibrarySurface, "POrder");

    private ToggleButton QOrderDropper => QContract.QContractFind<ToggleButton>(_qLibrarySurface, "POrderDropper");

    private QIconImage QOrderIcon => QContract.QContractFind<QIconImage>(_qLibrarySurface, "POrderIcon");

    private TextBox QInquiry => QContract.QContractFind<TextBox>(_qLibrarySurface, "PInquiry");

    private ToggleButton QSieveDropper => QContract.QContractFind<ToggleButton>(_qLibrarySurface, "PSieveDropper");

    private QIconImage QSieveIcon => QContract.QContractFind<QIconImage>(_qLibrarySurface, "PSieveIcon");

    private FrameworkElement QSieveMark => QContract.QContractFind<FrameworkElement>(_qLibrarySurface, "PSieveMark");

    private Popup QOrderDropdown => QContract.QContractFind<Popup>(_qLibrarySurface, "POrderDropdown");

    private StackPanel QOrderList => QContract.QContractFind<StackPanel>(_qLibrarySurface, "POrderList");

    private Popup QSieveDropdown => QContract.QContractFind<Popup>(_qLibrarySurface, "PSieveDropdown");

    private StackPanel QSieveList => QContract.QContractFind<StackPanel>(_qLibrarySurface, "PSieveList");

    private Button QLibraryFresh => QContract.QContractFind<Button>(_qLibrarySurface, "PLibraryFresh");

    private Button QLibraryStore => QContract.QContractFind<Button>(_qLibrarySurface, "PLibraryStore");

    private StackPanel QLibraryVoyage => QContract.QContractFind<StackPanel>(_qLibrarySurface, "PLibraryVoyage");

    private Button QLibraryEarlier => QContract.QContractFind<Button>(_qLibrarySurface, "PLibraryEarlier");

    private Button QLibraryLater => QContract.QContractFind<Button>(_qLibrarySurface, "PLibraryLater");

    private StackPanel QLibraryChronicle =>
        QContract.QContractFind<StackPanel>(_qLibrarySurface, "PLibraryChronicle");

    private Button QLibraryBackward => QContract.QContractFind<Button>(_qLibrarySurface, "PLibraryBackward");

    private Button QLibraryForward => QContract.QContractFind<Button>(_qLibrarySurface, "PLibraryForward");

    private Button QLibraryMarkup => QContract.QContractFind<Button>(_qLibrarySurface, "PLibraryMarkup");

    private Button QLibraryPortrait => QContract.QContractFind<Button>(_qLibrarySurface, "PLibraryPortrait");

    private Button QLibraryPress => QContract.QContractFind<Button>(_qLibrarySurface, "PLibraryPress");

    private Border QLibraryMode => QContract.QContractFind<Border>(_qLibrarySurface, "PLibraryMode");

    private RadioButton QLibraryViewer => QContract.QContractFind<RadioButton>(_qLibrarySurface, "PLibraryViewer");

    private RadioButton QLibraryScribe => QContract.QContractFind<RadioButton>(_qLibrarySurface, "PLibraryScribe");

    internal PDisplay QLibraryDisplay => QContract.QContractFind<PDisplay>(_qLibrarySurface, "PDisplay");

    private PEditor QLibraryEditor => QContract.QContractFind<PEditor>(_qLibrarySurface, "PEditor");

    private Button QLibraryBin => QContract.QContractFind<Button>(_qLibrarySurface, "PLibraryBin");

    private QIconImage QLibraryBinIcon => QContract.QContractFind<QIconImage>(_qLibrarySurface, "PLibraryBinIcon");

    internal void QLibraryAttach(PWindow host)
    {
        _qLibraryHost = host;
        LEditor editor = host.PWindowDeportment.LWindowForge.QForgeEditorCreate(host.PWindowUnreadableConfirm);
        LLectern lectern = new(editor.LEditorDisplay);
        _lLibrary = host.PWindowDeportment.LWindowForge.QForgeLibraryCreate(
            editor,
            lectern,
            QLibraryShownCheck,
            QLibraryDiscardConfirm,
            host.PWindowDeleteConfirm);
        _lLibrary.LLibraryFailed += host.PWindowFailureShow;
        _lLibrary.LLibraryEditor.LEditorStateChanged += QLibraryStoreUpdate;
        _lLibrary.LLibraryPanel.LPanelChanged += QLibraryModeUpdate;
        _lLibrary.LLibraryPanel.LPanelFailed += host.PWindowFailureShow;

        QLibraryIndexAttach(
            QContract.QContractFind<ItemsControl>(_qLibrarySurface, "PIndex"),
            QContract.QContractFind<FrameworkElement>(_qLibrarySurface, "PIndexEmpty"));

        QLibraryDisplay.PDisplayAttach(host, lectern);

        QLibraryEditor.PEditorAttach(host, _lLibrary.LLibraryEditor, lectern);

        QLibraryEditor.PEditorChronicleChanged += QLibraryChronicleUpdate;
    }

    private void QLibraryIndexAttach(ItemsControl view, FrameworkElement empty)
    {
        _qIndex = new QIndex(view, empty);
        QLookItem.QLookItemAttach(view, QIndexApply);
        _lLibrary.LLibraryPanel.LPanelRowsChanged += QLibraryIndexShow;
    }

    private void QLibraryIndexShow()
    {
        _qIndex.QIndexShow(_lLibrary.LLibraryRowsRead(), true);
    }

    private void QLibrarySieveShow(FrameworkElement mark)
    {
        mark.Visibility = _lLibrary.LLibraryFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QLibraryStoreUpdate()
    {
        QLibraryStore.IsEnabled = _lLibrary.LLibraryEditor.LEditorStorable;
    }

    internal async void QLibraryVistaRestore()
    {
        UserControl surface = _qLibrarySurface;
        LPanel panel = _lLibrary.LLibraryPanel;
        panel.LPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(surface, panel.LPanelRowsUpdate));
        panel.LPanelObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(surface, QLibraryWorkspaceUpdate));
        panel.LPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, panel.LPanelEntryHandle));
        panel.LPanelObserverAttach(
            CSubject.CSubjectReflex, LObserver.LObserverCreate<CBulletin>(surface, panel.LPanelRowsUpdate));
        panel.LPanelObserverAttach(
            CSubject.CSubjectSettings, LObserver.LObserverCreate<CBulletin>(surface, panel.LPanelRowsUpdate));
        panel.LPanelChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, panel.LPanelDraftUpdate));
        QLibraryDisplay.PDisplayObserverAttach();
        QLibraryEditor.PEditorVistaRestore();
        QChoice.QChoiceOrderBuild(QOrderList, "Order", QOrderHandle, QIndex.QIndexOrder);
        QChoice.QChoiceOrderApply(QOrderDropdown, panel.LPanelOrder);
        QLibrarySieveShow(QSieveMark);

        await LEnsignImage.LEnsignLoad(_qLibraryHost.PWindowDeportment);

        QChoice.QChoiceFilterBuild(
            QSieveList,
            _qLibraryHost.PWindowDeportment.LWindowWorkspace.QWorkspaceLanguageRead(),
            panel.LPanelFilter,
            QSieveHandle);
        _lLibrary.LLibraryInquirySet(QInquiry.Text);
        panel.LPanelRowsUpdate();
    }

    private async void QLibraryWorkspaceUpdate()
    {
        await LEnsignImage.LEnsignLoad(_qLibraryHost.PWindowDeportment);
        _lLibrary.LLibraryPanel.LPanelClear();
    }

    internal bool QLibraryDraftFinish(bool store)
    {
        return QLibraryEditor.PEditorDraftFinish(store);
    }

    internal bool QLibraryChangeCheck()
    {
        return _lLibrary.LLibraryPanel.LPanelChangeCheck();
    }

    internal bool QLibraryLeaveConfirm()
    {
        return _lLibrary.LLibraryPanel.LPanelLeaveConfirm();
    }

    internal long QLibraryVoyageRead()
    {
        return _lLibrary.LLibraryVoyageRead();
    }

    internal void QIndexEntryShow(long id)
    {
        _lLibrary.LLibraryPanel.LPanelRowShow(id);
    }

    private bool QLibraryShownCheck()
    {
        return _qLibrarySurface.IsVisible;
    }

    private bool QLibraryDiscardConfirm()
    {
        return _qLibraryHost.PWindowDiscardConfirm(true, QLibraryEditor.PEditorDraftFinish);
    }

    internal void QLibraryScribeRestore(bool editing)
    {
        _lLibrary.LLibraryPanel.LPanelScribeRestore(editing);
    }

    internal void QLibraryClose()
    {
        QLibraryEditor.PEditorClose();
        QLibraryDisplay.PDisplayClose();
    }

    private void QLibraryModeUpdate()
    {
        LPanel panel = _lLibrary.LLibraryPanel;
        QLibraryEditor.Visibility = QLook.QLookVisibleRead(panel.LPanelEditing);
        QLibraryDisplay.Visibility = QLook.QLookVisibleRead(panel.LPanelViewerChecked);
        QLibraryViewer.IsChecked = QLook.QLookCheckedRead(panel.LPanelViewerChecked);
        QLibraryScribe.IsChecked = QLook.QLookCheckedRead(panel.LPanelScribeChecked);
        QLibraryVoyage.Visibility = QLook.QLookVisibleRead(panel.LPanelViewerChecked);
        QLibraryChronicle.Visibility = QLook.QLookVisibleRead(panel.LPanelScribeChecked);
        QLibraryMode.IsEnabled = panel.LPanelModeEnabled;
        QLibraryBin.IsEnabled = panel.LPanelBinEnabled;
    }

    private string? QLibraryMarkupOpen()
    {
        return QMarkup.QMarkupOpen(_qLibraryHost.PWindowSurface);
    }

    private IReadOnlyList<CSCustomsRow>? QLibraryCustomsShow(IReadOnlyList<CMarkupEntry> entries)
    {
        return QSCustoms.QSCustomsShow(_qLibraryHost, entries);
    }

    private void QLibraryOmissionShow(IReadOnlyList<CMarkupOmission> omissions)
    {
        QSCustoms.QSCustomsOmissionShow(_qLibraryHost, omissions);
    }

    private void QInquiryHandle(object sender, TextChangedEventArgs e)
    {
        _lLibrary.LLibraryInquirySet(QInquiry.Text);
    }

    private void QOrderHandle(object sender, RoutedEventArgs e)
    {
        QOrderDropper.IsChecked = false;
        _lLibrary.LLibraryOrderSet(QChoice.QChoiceOrderRead(sender));
    }

    private void QSieveHandle(object sender, RoutedEventArgs e)
    {
        _lLibrary.LLibrarySieveSet(QChoice.QChoiceFilterRead(sender));
        QLibrarySieveShow(QSieveMark);
    }

    private void QIndexHandle(object sender, RoutedEventArgs e)
    {
        QLibraryRowSelect((sender as FrameworkElement)?.DataContext as QIndexItem);
    }

    private void QLibraryRowSelect(QIndexItem? item)
    {
        _qLibraryHost.PVoyageRecord();
        _lLibrary.LLibraryPanel.LPanelRowSelect(item?.QIndexItemId);
    }

    private void QIndexApply(FrameworkElement container, object item, string? change)
    {
        QIndexItem.QIndexItemApply(container, item, change);

        if (QLook.QLookPartFind<Button>(container, "PIndexRow") is Button row)
        {
            row.Click -= QIndexHandle;
            row.Click += QIndexHandle;
        }
    }

    private void QLibraryFreshHandle(object sender, RoutedEventArgs e)
    {
        _lLibrary.LLibraryPanel.LPanelFreshStart();
    }

    private void QLibraryScribeHandle(object sender, RoutedEventArgs e)
    {
        _lLibrary.LLibraryPanel.LPanelScribeSet(ReferenceEquals(sender, QLibraryScribe));
    }

    private void QLibraryStoreHandle(object sender, RoutedEventArgs e)
    {
        QLibraryEditor.PEditorEntrySave();
    }

    private void QLibraryBinHandle(object sender, RoutedEventArgs e)
    {
        _lLibrary.LLibraryPanel.LPanelDelete();
    }

    private async void QLibraryMarkupHandle(object sender, RoutedEventArgs e)
    {
        await _lLibrary.LLibraryMarkupStart(QLibraryMarkupOpen, QLibraryCustomsShow, QLibraryOmissionShow);
    }

    private void QLibraryPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lLibrary?.LLibraryPanel.LPanelPressAllowed ?? false;
    }

    private async void QLibraryPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _qLibraryHost.PWindowPressRun(_lLibrary.LLibraryPortraitPrint);
    }

    private async void QLibraryPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _qLibraryHost.PWindowPortraitExport(_lLibrary.LLibraryFileRead(), _lLibrary.LLibraryPortraitExport);
    }

    internal void QLibraryVoyageShow(bool past, bool future)
    {
        QLibraryEarlier.IsEnabled = past;
        QLibraryLater.IsEnabled = future;
    }

    private void QLibraryRetreatHandle(object sender, RoutedEventArgs e)
    {
        _qLibraryHost.PVoyageRetreatRun();
    }

    private void QLibraryAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _qLibraryHost.PVoyageAdvanceRun();
    }

    private void QLibraryUndoHandle(object sender, RoutedEventArgs e)
    {
        QLibraryEditor.QChronicleUndo();
    }

    private void QLibraryRedoHandle(object sender, RoutedEventArgs e)
    {
        QLibraryEditor.QChronicleRedo();
    }

    private void QLibraryChronicleUpdate()
    {
        (bool undo, bool redo) = QLibraryEditor.PEditorChronicleRead();
        QLibraryBackward.IsEnabled = undo;
        QLibraryForward.IsEnabled = redo;
    }
}
