using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QFavorite
{
    private readonly UserControl _qFavoriteSurface;

    private readonly QEditor _qFavoriteEditor;

    private readonly QDisplay _qFavoriteDisplay;

    private readonly QPanelRail _qFavoriteRail;

    private readonly QChoiceOrder _qFavoriteOrder;

    private readonly QChoiceFilter _qFavoriteFilter;

    private readonly ObservableCollection<QRosterItem> _qRosterList = [];

    private CAtelier _cAtelier = null!;

    private CEnvoy _cEnvoy = null!;

    private CFavorite _cFavorite = null!;

    internal QFavorite(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qFavoriteSurface = surface;
        _qFavoriteEditor = new QEditor(QContract.QContractFind<FrameworkElement>(surface, "PEditor"));
        _qFavoriteDisplay = new QDisplay(QContract.QContractFind<FrameworkElement>(surface, "PDisplay"));

        surface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QFavoritePressObserve, QFavoritePressCheck));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QFavoritePortraitObserve, QFavoritePressCheck));
        _qFavoriteRail = new QPanelRail(
            QContract.QContractFind<UserControl>(surface, "PFavoriteRail"),
            QFavoriteBin, QFavoriteBinIcon, false, true);
        _qFavoriteOrder = new QChoiceOrder(QContract.QContractFind<UserControl>(surface, "PFavoriteOrder"), QSeries);
        _qFavoriteFilter = new QChoiceFilter(QContract.QContractFind<UserControl>(surface, "PFavoriteFilter"));

        QRecall.SetResourceReference(QField.QFieldHintProperty, "Favorite.Search");

        QRecall.TextChanged += QRecallObserve;
        _qFavoriteRail.QPanelRailStored += QFavoriteStoreObserve;
        _qFavoriteRail.QPanelRailToggled += QFavoriteScribeObserve;
        _qFavoriteRail.QPanelRailDeleted += QFavoriteBinObserve;
    }

    private Border QSeries => QContract.QContractFind<Border>(_qFavoriteSurface, "PSeries");

    private TextBox QRecall => QContract.QContractFind<TextBox>(_qFavoriteSurface, "PRecall");

    private ItemsControl QRoster => QContract.QContractFind<ItemsControl>(_qFavoriteSurface, "PRoster");

    private TextBlock QRosterEmpty => QContract.QContractFind<TextBlock>(_qFavoriteSurface, "PRosterEmpty");

    private Button QFavoriteBin => QContract.QContractFind<Button>(_qFavoriteSurface, "PFavoriteBin");

    private QIconImage QFavoriteBinIcon => QContract.QContractFind<QIconImage>(_qFavoriteSurface, "PFavoriteBinIcon");

    internal void QFavoriteIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cAtelier = atelier;
        _cEnvoy = envoy;
        _cFavorite = CFavorite.CFavoriteCreate(
            atelier,
            QFavoriteShownCheck,
            envoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        CPanel panel = _cFavorite.CFavoritePanel;
        _qFavoriteOrder.QChoiceOrderIntroduce(panel.CPanelAperture, "Series", CFavorite.CFavoriteOrderRead());
        _qFavoriteFilter.QChoiceFilterIntroduce(panel.CPanelAperture, "Strainer");
        panel.CPanelChanged += QFavoriteModeUpdate;
        panel.CPanelAperture.CApertureRowsChanged += QRosterRefine;
        _cFavorite.CFavoriteWorkspaceChanged += QFavoriteWorkspaceRefine;

        QRoster.ItemsSource = _qRosterList;
        QLookItem.QLookItemAttach(QRoster, QRosterApply);

        _qFavoriteDisplay.QDisplayIntroduce(
            atelier, envoy, volume, mentionMenu, _cFavorite.CFavoriteEditor.CEditorDisplay);
        _cFavorite.CFavoriteEditor.CEditorDesk.CDeskStateChanged += QFavoriteStoreUpdate;
        _qFavoriteEditor.QEditorIntroduce(atelier, envoy, volume, mentionMenu, _cFavorite.CFavoriteEditor);
        _qFavoriteEditor.QEditorChronicleChanged += QFavoriteChronicleUpdate;
        _qFavoriteRail.QPanelRailIntroduce(atelier.CAtelierNavigation, _qFavoriteEditor);
    }

    private void QFavoriteStoreUpdate()
    {
        _qFavoriteRail.QEntryStorableRefine(_cFavorite.CFavoriteEditor.CEditorDesk.CDeskDraft.CDeskDraftStorable);
    }

    internal void QFavoriteExitRefine()
    {
        _qFavoriteEditor.QEditorPlayerRefine();
    }

    private void QFavoritePressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cFavorite?.CFavoritePanel.CPanelPressAllowed ?? false;
    }

    private async void QFavoritePressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cFavorite.CFavoritePortraitPrint();
    }

    private async void QFavoritePortraitObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cFavorite.CFavoritePortraitExport();
    }

    private void QFavoriteChronicleUpdate()
    {
        (bool undo, bool redo) = _cFavorite.CFavoriteEditor.CEditorDesk.CDeskChronicle.CDeskChronicleRead();
        _qFavoriteRail.QChronicleRefine(undo, redo);
    }

    private bool QFavoriteShownCheck()
    {
        return _qFavoriteSurface.IsVisible;
    }

    private void QFavoriteModeUpdate()
    {
        CPanel panel = _cFavorite.CFavoritePanel;
        _qFavoriteEditor.QEditorVisibleRefine(QLook.QLookVisibleRead(panel.CPanelEditing));
        _qFavoriteDisplay.QDisplayVisibleRefine(QLook.QLookVisibleRead(panel.CPanelViewerChecked));
        _qFavoriteRail.QPanelRailRefine(panel.CPanelScribeChecked, panel.CPanelModeEnabled, panel.CPanelBinEnabled);
    }

    private async void QFavoriteWorkspaceRefine()
    {
        await _cAtelier.CAtelierCatalog.CCatalogEnsignLoad(_cEnvoy, QEnsignImage.QEnsignDraw);
    }

    private void QRecallObserve(object sender, TextChangedEventArgs e)
    {
        _cFavorite.CFavoritePanel.CPanelAperture.CApertureQuerySet(QRecall.Text ?? string.Empty);
    }

    internal async void QFavoriteVistaRefine()
    {
        _qFavoriteOrder.QChoiceOrderRefine();
        _qFavoriteFilter.QChoiceFilterRefine();
        CEnsignSheet<IReadOnlyList<CVistaRow>> sheet =
            await _cFavorite.CFavoriteRowsLoad(QEnsignImage.QEnsignDraw);
        _qFavoriteFilter.QChoiceFilterBuild(sheet.CEnsignSheetLanguages);
        QRosterRefine(sheet.CEnsignSheetRows);
    }

    private void QRosterRefine()
    {
        QRosterRefine(_cFavorite.CFavoriteRowsRead());
    }

    private void QRosterRefine(IReadOnlyList<CVistaRow> rows)
    {
        List<QRosterItem> fresh = [];
        foreach (CVistaRow row in rows)
        {
            fresh.Add(new QRosterItem(
                row.CVistaRowId,
                row.CVistaRowHeadword,
                row.CVistaRowName,
                row.CVistaRowLanguage,
                row.CVistaRowEpithet,
                row.CVistaRowChosen));
        }

        QSplice.QSpliceRefine(
            _qRosterList, fresh, QRosterItem.QRosterItemMatch, QRosterItem.QRosterItemSync);

        QRosterEmpty.Visibility = QLook.QLookVisibleRead(_qRosterList.Count == 0);
    }

    private void QRosterObserve(object sender, RoutedEventArgs e)
    {
        _cFavorite.CFavoritePanel.CPanelRowSelect(
            ((sender as FrameworkElement)?.DataContext as QRosterItem)?.QRosterItemId);
    }

    private void QRosterApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QRosterItem roster)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PRosterRow") is Button row)
        {
            if (roster.QRosterItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QRosterObserve;
            row.Click += QRosterObserve;
        }

        if (QLook.QLookPartFind<Image>(container, "PRosterFlag") is Image flag)
        {
            flag.Source = roster.QRosterItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "PRosterName") is Run name)
        {
            name.Text = roster.QRosterItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PRosterEpithet") is Run epithet)
        {
            epithet.Text = QLook.QLookEpithetRead(roster.QRosterItemEpithet);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PRosterLanguage") is TextBlock language)
        {
            language.Text = roster.QRosterItemLanguage;
        }
    }

    private void QFavoriteScribeObserve(bool scribe)
    {
        _cFavorite.CFavoritePanel.CPanelScribeToggle(scribe);
    }

    private void QFavoriteStoreObserve()
    {
        _cFavorite.CFavoriteEditor.CEditorEntrySave();
    }

    private void QFavoriteBinObserve()
    {
        _cFavorite.CFavoritePanel.CPanelBin.CPanelBinDelete();
    }
}
