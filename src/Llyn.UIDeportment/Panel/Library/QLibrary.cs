using System;
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

    private CLibrary _cLibrary = null!;

    private LEditor _lEditor = null!;

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
        _cLibrary = host.PWindowForge.QForgeLibraryCreate(QLibraryShownCheck, host.PWindowEnvoy);
        _lEditor = new LEditor(_cLibrary.CLibraryEditor);
        CPanel panel = _cLibrary.CLibraryPanel;
        QLectern lectern = new(_lEditor.LEditorStudio.CEditorDisplay, panel);
        panel.CPanelChanged += QLibraryModeUpdate;
        _lEditor.LEditorStudio.CEditorDesk.CDeskStateChanged += QLibraryStoreUpdate;

        QLibraryIndexAttach(
            QContract.QContractFind<ItemsControl>(_qLibrarySurface, "PIndex"),
            QContract.QContractFind<FrameworkElement>(_qLibrarySurface, "PIndexEmpty"));

        QLibraryDisplay.PDisplayAttach(host, lectern);

        QLibraryEditor.PEditorAttach(host, _lEditor);

        QLibraryEditor.PEditorChronicleChanged += QLibraryChronicleUpdate;
    }

    private void QLibraryIndexAttach(ItemsControl view, FrameworkElement empty)
    {
        _qIndex = new QIndex(view, empty);
        QLookItem.QLookItemAttach(view, QIndexApply);
        _cLibrary.CLibraryPanel.CPanelRowsChanged += QLibraryIndexShow;
    }

    private void QLibraryIndexShow()
    {
        _qIndex.QIndexShow(_cLibrary.CLibraryRowsRead(), true);
    }

    private void QLibrarySieveShow(FrameworkElement mark)
    {
        mark.Visibility = _cLibrary.CLibraryFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QLibraryStoreUpdate()
    {
        QLibraryStore.IsEnabled = _lEditor.LEditorStorable;
    }

    internal async void QLibraryVistaRestore()
    {
        UserControl surface = _qLibrarySurface;
        CPanel panel = _cLibrary.CLibraryPanel;
        panel.CPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelRowsResonate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(surface, QLibraryWorkspaceUpdate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelEntryResonate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectReflex, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelRowsResonate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectSettings, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelRowsResonate));
        panel.CPanelChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelDraftResonate));
        QLibraryEditor.PEditorVistaRestore();
        QChoice.QChoiceOrderBuild(QOrderList, "Order", QOrderHandle, QIndex.QIndexOrder);
        QChoice.QChoiceOrderApply(QOrderDropdown, panel.CPanelOrder);
        QLibrarySieveShow(QSieveMark);

        await LEnsignImage.LEnsignLoad(_qLibraryHost.PWindowAtelier);

        QChoice.QChoiceFilterBuild(
            QSieveList,
            _qLibraryHost.PWindowAtelier.CAtelierCatalog.CCatalogLanguageRead(),
            panel.CPanelFilter,
            QSieveHandle);
        _cLibrary.CLibraryQuerySet(QInquiry.Text);
        panel.CPanelRowsResonate();
    }

    private async void QLibraryWorkspaceUpdate()
    {
        await LEnsignImage.LEnsignLoad(_qLibraryHost.PWindowAtelier);
        _cLibrary.CLibraryPanel.CPanelEntryClose();
    }

    internal bool QLibraryDraftFinish(bool store)
    {
        return QLibraryEditor.PEditorDraftFinish(store);
    }

    internal bool QLibraryChangeCheck()
    {
        return _cLibrary.CLibraryPanel.CPanelChangeCheck();
    }

    internal bool QLibraryLeaveConfirm()
    {
        return _cLibrary.CLibraryPanel.CPanelLeaveConfirm();
    }

    internal long QLibraryVoyageRead()
    {
        return _cLibrary.CLibraryPanel.CPanelChosenRead();
    }

    internal void QIndexEntryShow(long id)
    {
        _cLibrary.CLibraryPanel.CPanelRowOpen(id);
    }

    private bool QLibraryShownCheck()
    {
        return _qLibrarySurface.IsVisible;
    }

    internal void QLibraryScribeRestore(bool editing)
    {
        _cLibrary.CLibraryPanel.CPanelScribeRestore(editing);
    }

    internal void QLibraryClose()
    {
        QLibraryEditor.PEditorClose();
        QLibraryDisplay.PDisplayClose();
    }

    private void QLibraryModeUpdate()
    {
        CPanel panel = _cLibrary.CLibraryPanel;
        QLibraryEditor.Visibility = QLook.QLookVisibleRead(panel.CPanelEditing);
        QLibraryDisplay.Visibility = QLook.QLookVisibleRead(panel.CPanelViewerChecked);
        QLibraryViewer.IsChecked = QLook.QLookCheckedRead(panel.CPanelViewerChecked);
        QLibraryScribe.IsChecked = QLook.QLookCheckedRead(panel.CPanelScribeChecked);
        QLibraryVoyage.Visibility = QLook.QLookVisibleRead(panel.CPanelViewerChecked);
        QLibraryChronicle.Visibility = QLook.QLookVisibleRead(panel.CPanelScribeChecked);
        QLibraryMode.IsEnabled = panel.CPanelModeEnabled;
        QLibraryBin.IsEnabled = panel.CPanelBinEnabled;
    }

    private string? QLibraryMarkupOpen()
    {
        return QMarkup.QMarkupOpen(_qLibraryHost.PWindowSurface);
    }

    private void QInquiryHandle(object sender, TextChangedEventArgs e)
    {
        _cLibrary.CLibraryQuerySet(QInquiry.Text);
    }

    private void QOrderHandle(object sender, RoutedEventArgs e)
    {
        QOrderDropper.IsChecked = false;
        _cLibrary.CLibraryOrderSet(QChoice.QChoiceOrderRead(sender));
    }

    private void QSieveHandle(object sender, RoutedEventArgs e)
    {
        _cLibrary.CLibraryFilterSet(QChoice.QChoiceFilterRead(sender));
        QLibrarySieveShow(QSieveMark);
    }

    private void QIndexHandle(object sender, RoutedEventArgs e)
    {
        QLibraryRowSelect((sender as FrameworkElement)?.DataContext as QIndexItem);
    }

    private void QLibraryRowSelect(QIndexItem? item)
    {
        _qLibraryHost.PVoyageRecord();
        _cLibrary.CLibraryPanel.CPanelRowSelect(item?.QIndexItemId);
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
        _cLibrary.CLibraryPanel.CPanelEntryCreate();
    }

    private void QLibraryScribeHandle(object sender, RoutedEventArgs e)
    {
        _cLibrary.CLibraryPanel.CPanelScribeToggle(ReferenceEquals(sender, QLibraryScribe));
    }

    private void QLibraryStoreHandle(object sender, RoutedEventArgs e)
    {
        QLibraryEditor.PEditorEntrySave();
    }

    private void QLibraryBinHandle(object sender, RoutedEventArgs e)
    {
        _cLibrary.CLibraryPanel.CPanelEntryDelete();
    }

    private async void QLibraryMarkupHandle(object sender, RoutedEventArgs e)
    {
        await _cLibrary.CLibraryMarkupImport(QLibraryMarkupOpen());
    }

    private void QLibraryPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cLibrary?.CLibraryPanel.CPanelPressAllowed ?? false;
    }

    private async void QLibraryPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _qLibraryHost.PWindowPressRun(_cLibrary.CLibraryPortraitPrint);
    }

    private async void QLibraryPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        await _qLibraryHost.PWindowPortraitExport(_cLibrary.CLibraryFileRead(), _cLibrary.CLibraryPortraitExport);
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
