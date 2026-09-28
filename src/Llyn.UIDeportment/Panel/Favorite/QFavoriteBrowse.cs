using System;
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

    private async void QFavoriteWorkspaceUpdate()
    {
        await LEnsignImage.LEnsignLoad(_qFavoriteHost.PWindowAtelier);
        QFavoriteReset();
    }

    private void QRecallHandle(object sender, TextChangedEventArgs e)
    {
        _cFavorite.CFavoriteQuerySet(QRecall.Text ?? string.Empty);
    }

    private void QSeriesHandle(object sender, RoutedEventArgs e)
    {
        QSeriesDropper.IsChecked = false;
        _cFavorite.CFavoriteOrderSet(QChoice.QChoiceOrderRead(sender));
    }

    private void QStrainerHandle(object sender, RoutedEventArgs e)
    {
        _cFavorite.CFavoriteFilterSet(QChoice.QChoiceFilterRead(sender));
        QStrainerRestore();
    }

    internal async void QFavoriteVistaRestore()
    {
        UserControl surface = _qFavoriteSurface;
        CPanel panel = _cFavorite.CFavoritePanel;
        panel.CPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(surface, QRosterFind));
        panel.CPanelObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(surface, QFavoriteWorkspaceUpdate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectGrasp, LObserver.LObserverCreate<CBulletin>(surface, _cFavorite.CFavoriteGraspResonate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelEntryResonate));
        panel.CPanelObserverAttach(
            CSubject.CSubjectFavorite, LObserver.LObserverCreate<CBulletin>(surface, QRosterFind));
        panel.CPanelObserverAttach(
            CSubject.CSubjectReflex, LObserver.LObserverCreate<CBulletin>(surface, QRosterFind));
        panel.CPanelObserverAttach(
            CSubject.CSubjectSettings, LObserver.LObserverCreate<CBulletin>(surface, QRosterFind));
        panel.CPanelChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(surface, panel.CPanelDraftResonate));
        QFavoriteDisplay.PDisplayObserverAttach();
        QFavoriteEditor.PEditorVistaRestore();
        QChoice.QChoiceOrderBuild(
            QSeriesList,
            "Series",
            QSeriesHandle,
            [
                CCatalogOrder.CCatalogOrderHeadword,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderLanguage,
                CCatalogOrder.CCatalogOrderMarked,
                CCatalogOrder.CCatalogOrderGrasp,
            ]);
        QChoice.QChoiceOrderApply(QSeriesDropdown, panel.CPanelOrder);
        QStrainerRestore();

        await LEnsignImage.LEnsignLoad(_qFavoriteHost.PWindowAtelier);

        QChoice.QChoiceFilterBuild(
            QStrainerList, _cFavorite.CFavoriteLanguageRead(), panel.CPanelFilter, QStrainerHandle);
        _cFavorite.CFavoriteQuerySet(QRecall.Text ?? string.Empty);
        QRosterFind();
    }

    private void QStrainerRestore()
    {
        QStrainerMark.Visibility = _cFavorite.CFavoriteFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QRosterFind()
    {
        IReadOnlyList<CVistaRow> favorites;
        try
        {
            favorites = _cFavorite.CFavoriteRowsRead();
        }
        catch (Exception exception)
        {
            _qFavoriteHost.PWindowFailureShow("Favorite.LoadFailed", exception);
            favorites = [];
        }

        List<QRosterItem> fresh = [];
        foreach (CVistaRow row in favorites)
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

        QRosterEmpty.Visibility = _qRosterList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QRosterHandle(object sender, RoutedEventArgs e)
    {
        QRosterRowShow((sender as FrameworkElement)?.DataContext as QRosterItem);
    }

    private void QRosterRowShow(QRosterItem? item)
    {
        if (item is null)
        {
            return;
        }

        if (!QFavoriteLeaveConfirm())
        {
            return;
        }

        _qFavoriteHost.PVoyageRecord();
        _cFavorite.CFavoritePanel.CPanelRowOpen(item.QRosterItemId);
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

            row.Click -= QRosterHandle;
            row.Click += QRosterHandle;
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

    internal void QRosterEntryShow(long id)
    {
        _cFavorite.CFavoritePanel.CPanelRowOpen(id);
    }

    private void QFavoriteScribeHandle(object sender, RoutedEventArgs e)
    {
        _cFavorite.CFavoritePanel.CPanelScribeToggle(ReferenceEquals(sender, QFavoriteScribe));
    }

    internal void QFavoriteScribeRestore(bool editing)
    {
        _cFavorite.CFavoritePanel.CPanelScribeRestore(editing);
    }

    internal long QFavoriteVoyageRead()
    {
        return _cFavorite.CFavoritePanel.CPanelChosenRead();
    }

    internal bool QFavoriteLeaveConfirm()
    {
        return _cFavorite.CFavoritePanel.CPanelLeaveConfirm();
    }

    private void QFavoriteStoreHandle(object sender, RoutedEventArgs e)
    {
        QFavoriteEditor.PEditorEntrySave();
    }

    private void QFavoriteBinHandle(object sender, RoutedEventArgs e)
    {
        _cFavorite.CFavoritePanel.CPanelEntryDelete();
    }
}
