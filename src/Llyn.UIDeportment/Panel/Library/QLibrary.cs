using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QLibrary
{
    private readonly UserControl _qLibrarySurface;

    private readonly QEditor _qLibraryEditor;

    private readonly QDisplay _qLibraryDisplay;

    private readonly QPanelRail _qLibraryRail;

    private readonly QChoiceOrder _qLibraryOrder;

    private readonly QChoiceFilter _qLibraryFilter;

    private CAtelier _cAtelier = null!;

    private CEnvoy _cEnvoy = null!;

    private CLibrary _cLibrary = null!;

    private QIndex _qIndex = null!;

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
        _qLibraryRail = new QPanelRail(
            QContract.QContractFind<UserControl>(surface, "PLibraryRail"), QLibraryBin, QLibraryBinIcon, true, true);
        _qLibraryOrder = new QChoiceOrder(QContract.QContractFind<UserControl>(surface, "PLibraryOrder"), QOrder);
        _qLibraryFilter = new QChoiceFilter(QContract.QContractFind<UserControl>(surface, "PLibraryFilter"));

        QInquiry.SetResourceReference(QField.QFieldHintProperty, "List.Search");
        QLibraryMarkup.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("import", 24));

        QInquiry.TextChanged += QInquiryObserve;
        QLibraryMarkup.Click += QLibraryMarkupObserve;
        _qLibraryRail.QPanelRailCreated += QLibraryFreshObserve;
        _qLibraryRail.QPanelRailStored += QLibraryStoreObserve;
        _qLibraryRail.QPanelRailToggled += QLibraryScribeObserve;
        _qLibraryRail.QPanelRailDeleted += QLibraryBinObserve;
    }

    private Border QOrder => QContract.QContractFind<Border>(_qLibrarySurface, "POrder");

    private TextBox QInquiry => QContract.QContractFind<TextBox>(_qLibrarySurface, "PInquiry");

    private Button QLibraryMarkup => QContract.QContractFind<Button>(_qLibrarySurface, "PLibraryMarkup");

    private Button QLibraryBin => QContract.QContractFind<Button>(_qLibrarySurface, "PLibraryBin");

    private QIconImage QLibraryBinIcon => QContract.QContractFind<QIconImage>(_qLibrarySurface, "PLibraryBinIcon");

    internal void QLibraryIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cAtelier = atelier;
        _cEnvoy = envoy;
        _cLibrary = CLibrary.CLibraryCreate(
            atelier,
            QLibraryShownCheck,
            envoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        CPanel panel = _cLibrary.CLibraryPanel;
        _qLibraryOrder.QChoiceOrderIntroduce(panel.CPanelAperture, "Order", CLibrary.CLibraryOrderRead());
        _qLibraryFilter.QChoiceFilterIntroduce(panel.CPanelAperture, "Sieve");
        panel.CPanelChanged += QLibraryModeRefine;
        _cLibrary.CLibraryWorkspaceChanged += QLibraryWorkspaceRefine;
        _cLibrary.CLibraryEditor.CEditorDesk.CDeskStateChanged += QLibraryStoreRefine;

        QLibraryIndexIntroduce(
            QContract.QContractFind<ItemsControl>(_qLibrarySurface, "PIndex"),
            QContract.QContractFind<FrameworkElement>(_qLibrarySurface, "PIndexEmpty"));

        _qLibraryDisplay.QDisplayIntroduce(
            atelier, envoy, volume, mentionMenu, _cLibrary.CLibraryEditor.CEditorDisplay);
        atelier.CAtelierMention.CMentionSenseChosen +=
            _qLibraryDisplay.QDisplayLectern.QLecternCompass.QCompassSpotlightRefine;

        _qLibraryEditor.QEditorIntroduce(atelier, envoy, volume, mentionMenu, _cLibrary.CLibraryEditor);

        _qLibraryEditor.QEditorChronicleChanged += QLibraryChronicleRefine;
        _qLibraryRail.QPanelRailIntroduce(atelier.CAtelierNavigation, _qLibraryEditor);
    }

    private void QLibraryIndexIntroduce(ItemsControl view, FrameworkElement empty)
    {
        _qIndex = new QIndex(view, empty);
        QLookItem.QLookItemAttach(view, QLibraryRowRefine);
        _cLibrary.CLibraryPanel.CPanelAperture.CApertureRowsChanged += QLibraryIndexRefine;
    }

    private void QLibraryIndexRefine()
    {
        QLibraryIndexRefine(_cLibrary.CLibraryRowsRead());
    }

    private void QLibraryIndexRefine(IReadOnlyList<CVistaRow> rows)
    {
        _qIndex.QIndexRefine(rows, _cLibrary.CLibraryPanel.CPanelAperture.CApertureEmpty);
    }

    private void QLibraryStoreRefine()
    {
        _qLibraryRail.QEntryStorableRefine(_cLibrary.CLibraryEditor.CEditorDesk.CDeskDraft.CDeskDraftStorable);
    }

    internal async void QLibraryVistaRefine()
    {
        _qLibraryOrder.QChoiceOrderRefine();
        _qLibraryFilter.QChoiceFilterRefine();
        CEnsignSheet<IReadOnlyList<CVistaRow>> sheet =
            await _cLibrary.CLibraryRowsLoad(QEnsignImage.QEnsignDraw);
        _qLibraryFilter.QChoiceFilterBuild(sheet.CEnsignSheetLanguages);
        QLibraryIndexRefine(sheet.CEnsignSheetRows);
    }

    private async void QLibraryWorkspaceRefine()
    {
        await _cAtelier.CAtelierCatalog.CCatalogEnsignLoad(_cEnvoy, QEnsignImage.QEnsignDraw);
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
        _qLibraryRail.QPanelRailRefine(panel.CPanelScribeChecked, panel.CPanelModeEnabled, panel.CPanelBinEnabled);
    }

    private void QInquiryObserve(object sender, TextChangedEventArgs e)
    {
        _cLibrary.CLibraryPanel.CPanelAperture.CApertureQuerySet(QInquiry.Text);
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

    private void QLibraryFreshObserve()
    {
        _cLibrary.CLibraryPanel.CPanelEntryCreate();
    }

    private void QLibraryScribeObserve(bool scribe)
    {
        _cLibrary.CLibraryPanel.CPanelScribeToggle(scribe);
    }

    private void QLibraryStoreObserve()
    {
        _cLibrary.CLibraryEditor.CEditorEntrySave();
    }

    private void QLibraryBinObserve()
    {
        _cLibrary.CLibraryPanel.CPanelBin.CPanelBinDelete();
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

    private void QLibraryChronicleRefine()
    {
        (bool undo, bool redo) = _cLibrary.CLibraryEditor.CEditorDesk.CDeskChronicle.CDeskChronicleRead();
        _qLibraryRail.QChronicleRefine(undo, redo);
    }
}
