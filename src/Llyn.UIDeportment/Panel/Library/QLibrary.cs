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

    private readonly QEditor _qLibraryEditor;

    private readonly QDisplay _qLibraryDisplay;

    private QWindow _qLibraryHost = null!;

    private CLibrary _cLibrary = null!;

    private QIndex _qIndex = null!;

    internal QLectern QLibraryLectern { get; private set; } = null!;

    internal QLibrary(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qLibrarySurface = surface;
        _qLibraryEditor = new QEditor(QContract.QContractFind<FrameworkElement>(surface, "PEditor"));
        _qLibraryDisplay = new QDisplay(QContract.QContractFind<FrameworkElement>(surface, "PDisplay"));

        surface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QLibraryPressObserve, QLibraryPressRefine));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QLibraryPortraitObserve, QLibraryPressRefine));
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

        QInquiry.TextChanged += QInquiryObserve;
        QLibraryFresh.Click += QLibraryFreshObserve;
        QLibraryStore.Click += QLibraryStoreObserve;
        QLibraryEarlier.Click += QLibraryRetreatObserve;
        QLibraryLater.Click += QLibraryAdvanceObserve;
        QLibraryBackward.Click += QLibraryUndoObserve;
        QLibraryForward.Click += QLibraryRedoObserve;
        QLibraryMarkup.Click += QLibraryMarkupObserve;
        QLibraryViewer.Click += QLibraryViewerObserve;
        QLibraryScribe.Click += QLibraryScribeObserve;
        QLibraryBin.Click += QLibraryBinObserve;
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

    private Button QLibraryBin => QContract.QContractFind<Button>(_qLibrarySurface, "PLibraryBin");

    private QIconImage QLibraryBinIcon => QContract.QContractFind<QIconImage>(_qLibrarySurface, "PLibraryBinIcon");

    internal void QLibraryIntroduce(QWindow host)
    {
        _qLibraryHost = host;
        _cLibrary = CLibrary.CLibraryCreate(
            host.QWindowAtelier,
            QLibraryShownCheck,
            host.QWindowEnvoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        CPanel panel = _cLibrary.CLibraryPanel;
        QLectern lectern = new(_cLibrary.CLibraryEditor.CEditorDisplay, panel);
        QLibraryLectern = lectern;
        QChoice.QChoiceOrderBuild(QOrderList, "Order", QOrderObserve, CLibrary.CLibraryOrderRead());
        panel.CPanelChanged += QLibraryModeRefine;
        _cLibrary.CLibraryWorkspaceChanged += QLibraryWorkspaceRefine;
        _cLibrary.CLibraryEditor.CEditorDesk.CDeskStateChanged += QLibraryStoreRefine;

        QLibraryIndexIntroduce(
            QContract.QContractFind<ItemsControl>(_qLibrarySurface, "PIndex"),
            QContract.QContractFind<FrameworkElement>(_qLibrarySurface, "PIndexEmpty"));

        _qLibraryDisplay.QDisplayIntroduce(host, lectern);

        _qLibraryEditor.QEditorIntroduce(host, _cLibrary.CLibraryEditor);

        _qLibraryEditor.QEditorChronicleChanged += QLibraryChronicleRefine;
    }

    private void QLibraryIndexIntroduce(ItemsControl view, FrameworkElement empty)
    {
        _qIndex = new QIndex(view, empty);
        QLookItem.QLookItemAttach(view, QLibraryRowRefine);
        _cLibrary.CLibraryPanel.CPanelRowsChanged += QLibraryIndexRefine;
    }

    private void QLibraryIndexRefine()
    {
        QLibraryIndexRefine(_cLibrary.CLibraryRowsRead());
    }

    private void QLibraryIndexRefine(IReadOnlyList<CVistaRow> rows)
    {
        _qIndex.QIndexRefine(rows, _cLibrary.CLibraryEmpty);
    }

    private void QLibrarySieveRefine()
    {
        QSieveMark.Visibility = QLook.QLookVisibleRead(_cLibrary.CLibraryFiltered);
    }

    private void QLibraryStoreRefine()
    {
        QLibraryStore.IsEnabled = _cLibrary.CLibraryEditor.CEditorDesk.CDeskStorable;
    }

    internal async void QLibraryVistaRefine()
    {
        QChoice.QChoiceOrderApply(QOrderDropdown, _cLibrary.CLibraryPanel.CPanelOrder);
        QLibrarySieveRefine();
        CEnsignSheet<IReadOnlyList<CVistaRow>> sheet =
            await _cLibrary.CLibraryRowsLoad(QEnsignImage.QEnsignDraw);
        QSieveBuild(sheet.CEnsignSheetLanguages);
        QLibraryIndexRefine(sheet.CEnsignSheetRows);
    }

    private void QSieveBuild(IReadOnlyList<string> languages)
    {
        QChoice.QChoiceFilterBuild(QSieveList, languages, _cLibrary.CLibraryPanel.CPanelFilter, QSieveObserve);
    }

    private async void QLibraryWorkspaceRefine()
    {
        await _qLibraryHost.QWindowAtelier.CAtelierCatalog.CCatalogEnsignLoad(
            _qLibraryHost.QWindowEnvoy, QEnsignImage.QEnsignDraw);
    }

    private bool QLibraryShownCheck()
    {
        return _qLibrarySurface.IsVisible;
    }

    internal void QLibraryExitRefine()
    {
        _qLibraryEditor.QEditorPlayerRefine();
    }

    private void QLibraryModeRefine()
    {
        CPanel panel = _cLibrary.CLibraryPanel;
        _qLibraryEditor.QEditorVisibleRefine(QLook.QLookVisibleRead(panel.CPanelEditing));
        _qLibraryDisplay.QDisplayVisibleRefine(QLook.QLookVisibleRead(panel.CPanelViewerChecked));
        QLibraryViewer.IsChecked = panel.CPanelViewerChecked;
        QLibraryScribe.IsChecked = panel.CPanelScribeChecked;
        QLibraryVoyage.Visibility = QLook.QLookVisibleRead(panel.CPanelViewerChecked);
        QLibraryChronicle.Visibility = QLook.QLookVisibleRead(panel.CPanelScribeChecked);
        QLibraryMode.IsEnabled = panel.CPanelModeEnabled;
        QLibraryBin.IsEnabled = panel.CPanelBinEnabled;
    }

    private void QInquiryObserve(object sender, TextChangedEventArgs e)
    {
        _cLibrary.CLibraryQuerySet(QInquiry.Text);
    }

    private void QOrderObserve(object sender, RoutedEventArgs e)
    {
        _cLibrary.CLibraryOrderSet(QChoice.QChoiceOrderRead(sender));
        QOrderRefine();
    }

    private void QOrderRefine()
    {
        QOrderDropper.IsChecked = false;
    }

    private void QSieveObserve(object sender, RoutedEventArgs e)
    {
        _cLibrary.CLibraryFilterSet(QChoice.QChoiceFilterRead(sender));
        QLibrarySieveRefine();
    }

    private void QLibraryIndexObserve(object sender, RoutedEventArgs e)
    {
        _cLibrary.CLibraryPanel.CPanelRowSelect(
            ((sender as FrameworkElement)?.DataContext as QIndexItem)?.QIndexItemId);
    }

    private void QLibraryRowRefine(FrameworkElement container, object item, string? change)
    {
        QIndexItem.QIndexItemRefine(container, item, change);

        if (QLook.QLookPartFind<Button>(container, "PIndexRow") is Button row)
        {
            row.Click -= QLibraryIndexObserve;
            row.Click += QLibraryIndexObserve;
        }
    }

    private void QLibraryFreshObserve(object sender, RoutedEventArgs e)
    {
        _cLibrary.CLibraryPanel.CPanelEntryCreate();
    }

    private void QLibraryViewerObserve(object sender, RoutedEventArgs e)
    {
        _cLibrary.CLibraryPanel.CPanelScribeToggle(false);
    }

    private void QLibraryScribeObserve(object sender, RoutedEventArgs e)
    {
        _cLibrary.CLibraryPanel.CPanelScribeToggle(true);
    }

    private void QLibraryStoreObserve(object sender, RoutedEventArgs e)
    {
        _cLibrary.CLibraryEditor.CEditorEntrySave();
    }

    private void QLibraryBinObserve(object sender, RoutedEventArgs e)
    {
        _cLibrary.CLibraryPanel.CPanelEntryDelete();
    }

    private async void QLibraryMarkupObserve(object sender, RoutedEventArgs e)
    {
        await _cLibrary.CLibraryMarkupImport();
    }

    private void QLibraryPressRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cLibrary?.CLibraryPanel.CPanelPressAllowed ?? false;
    }

    private async void QLibraryPressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cLibrary.CLibraryPortraitPrint();
    }

    private async void QLibraryPortraitObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cLibrary.CLibraryPortraitExport();
    }

    internal void QLibraryVoyageRefine(bool past, bool future)
    {
        QLibraryEarlier.IsEnabled = past;
        QLibraryLater.IsEnabled = future;
    }

    private void QLibraryRetreatObserve(object sender, RoutedEventArgs e)
    {
        _qLibraryHost.QWindowAtelier.CAtelierNavigation.CNavigationStationUndo();
    }

    private void QLibraryAdvanceObserve(object sender, RoutedEventArgs e)
    {
        _qLibraryHost.QWindowAtelier.CAtelierNavigation.CNavigationStationRedo();
    }

    private void QLibraryUndoObserve(object sender, RoutedEventArgs e)
    {
        _qLibraryEditor.QChronicleUndoObserve();
    }

    private void QLibraryRedoObserve(object sender, RoutedEventArgs e)
    {
        _qLibraryEditor.QChronicleRedoObserve();
    }

    private void QLibraryChronicleRefine()
    {
        (bool undo, bool redo) = _cLibrary.CLibraryEditor.CEditorDesk.CDeskChronicleRead();
        QLibraryBackward.IsEnabled = undo;
        QLibraryForward.IsEnabled = redo;
    }
}
