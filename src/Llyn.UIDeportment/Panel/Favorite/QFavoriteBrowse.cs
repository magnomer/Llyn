using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QFavorite
{
    private readonly ObservableCollection<QRosterItem> _qRosterList = [];

    private async void QFavoriteWorkspaceRefine()
    {
        await LEnsignImage.LEnsignLoad(_qFavoriteHost.PWindowAtelier.CAtelierCatalog.CCatalogEnsignLoad);
    }

    private void QRecallObserve(object sender, TextChangedEventArgs e)
    {
        _cFavorite.CFavoriteQuerySet(QRecall.Text ?? string.Empty);
    }

    private void QSeriesObserve(object sender, RoutedEventArgs e)
    {
        _cFavorite.CFavoriteOrderSet(QChoice.QChoiceOrderRead(sender));
        QSeriesDropperRefine();
    }

    private void QSeriesDropperRefine()
    {
        QSeriesDropper.IsChecked = false;
    }

    private void QStrainerObserve(object sender, RoutedEventArgs e)
    {
        _cFavorite.CFavoriteFilterSet(QChoice.QChoiceFilterRead(sender));
        QStrainerRefine();
    }

    internal async void QFavoriteVistaRefine()
    {
        QChoice.QChoiceOrderApply(QSeriesDropdown, _cFavorite.CFavoritePanel.CPanelOrder);
        QStrainerRefine();
        IReadOnlyList<string> languages =
            await LEnsignImage.LEnsignLoad(_qFavoriteHost.PWindowAtelier.CAtelierCatalog.CCatalogEnsignLoad);
        QStrainerBuild(languages);
        QRosterRefine();
    }

    private void QStrainerBuild(IReadOnlyList<string> languages)
    {
        QChoice.QChoiceFilterBuild(
            QStrainerList, languages, _cFavorite.CFavoritePanel.CPanelFilter, QStrainerObserve);
    }

    private void QStrainerRefine()
    {
        QStrainerMark.Visibility = QLook.QLookVisibleRead(_cFavorite.CFavoriteFiltered);
    }

    private void QRosterRefine()
    {
        List<QRosterItem> fresh = [];
        foreach (CVistaRow row in _cFavorite.CFavoriteRowsRead())
        {
            fresh.Add(new QRosterItem(
                row.CVistaRowId,
                row.CVistaRowHeadword,
                row.CVistaRowName,
                row.CVistaRowLanguage,
                row.CVistaRowEpithet,
                row.CVistaRowChosen));
        }

        LSplice.LSpliceApply(
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
            epithet.Text = " " + roster.QRosterItemEpithet;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PRosterLanguage") is TextBlock language)
        {
            language.Text = roster.QRosterItemLanguage;
        }
    }

    private void QFavoriteViewerObserve(object sender, RoutedEventArgs e)
    {
        _cFavorite.CFavoritePanel.CPanelScribeToggle(false);
    }

    private void QFavoriteScribeObserve(object sender, RoutedEventArgs e)
    {
        _cFavorite.CFavoritePanel.CPanelScribeToggle(true);
    }

    private void QFavoriteStoreObserve(object sender, RoutedEventArgs e)
    {
        _cFavorite.CFavoriteEditor.CEditorEntrySave();
    }

    private void QFavoriteBinObserve(object sender, RoutedEventArgs e)
    {
        _cFavorite.CFavoritePanel.CPanelEntryDelete();
    }
}
