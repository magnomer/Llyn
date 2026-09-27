using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PFavorite
{
    private readonly ObservableCollection<PRosterItem> _pRosterList = [];

    private async void PFavoriteWorkspaceUpdate()
    {
        await LEnsignImage.LEnsignLoad(_pFavoriteHost.PWindowDeportment);
        PFavoriteReset();
    }

    private void PRecallHandle(object sender, TextChangedEventArgs e)
    {
        _lFavorite.LFavoriteRecallSet(PRecall.Text ?? string.Empty);
    }

    private void PSeriesHandle(object sender, RoutedEventArgs e)
    {
        PSeriesDropper.IsChecked = false;
        _lFavorite.LFavoriteSeriesSet(QChoice.QChoiceOrderRead(sender));
    }

    private void PSeriesGraspUpdate()
    {
        if (!_lFavorite.LFavoriteGraspOrdered)
        {
            return;
        }

        PRosterFind();
    }

    private void PStrainerHandle(object sender, RoutedEventArgs e)
    {
        _lFavorite.LFavoriteStrainerSet(QChoice.QChoiceFilterRead(PStrainerList));
        PStrainerRestore();
    }

    internal async void PFavoriteVistaRestore()
    {
        LPanel panel = _lFavorite.LFavoritePanel;
        panel.LPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(this, PRosterFind));
        panel.LPanelObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(this, PFavoriteWorkspaceUpdate));
        panel.LPanelObserverAttach(
            CSubject.CSubjectGrasp, LObserver.LObserverCreate<CBulletin>(this, PSeriesGraspUpdate));
        panel.LPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(this, panel.LPanelEntryHandle));
        panel.LPanelObserverAttach(
            CSubject.CSubjectFavorite, LObserver.LObserverCreate<CBulletin>(this, PRosterFind));
        panel.LPanelObserverAttach(
            CSubject.CSubjectReflex, LObserver.LObserverCreate<CBulletin>(this, PRosterFind));
        panel.LPanelObserverAttach(
            CSubject.CSubjectSettings, LObserver.LObserverCreate<CBulletin>(this, PRosterFind));
        panel.LPanelChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(this, panel.LPanelDraftUpdate));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        QChoice.QChoiceOrderBuild(
            PSeriesList,
            "Series",
            PSeriesHandle,
            [
                CCatalogOrder.CCatalogOrderHeadword,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderLanguage,
                CCatalogOrder.CCatalogOrderMarked,
                CCatalogOrder.CCatalogOrderGrasp,
            ]);
        QChoice.QChoiceOrderApply(PSeriesDropdown, _lFavorite.LFavoriteOrder);
        PStrainerRestore();

        await LEnsignImage.LEnsignLoad(_pFavoriteHost.PWindowDeportment);

        QChoice.QChoiceFilterBuild(
            PStrainerList, _lFavorite.LFavoriteLanguageRead(), _lFavorite.LFavoriteFilter, PStrainerHandle);
        _lFavorite.LFavoriteRecallSet(PRecall.Text ?? string.Empty);
        PRosterFind();
    }

    private void PStrainerRestore()
    {
        PStrainerMark.Visibility = _lFavorite.LFavoriteFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PRosterFind()
    {
        IReadOnlyList<LVistaRow> favorites;
        try
        {
            favorites = _lFavorite.LFavoriteRowsRead();
        }
        catch (Exception exception)
        {
            _pFavoriteHost.PWindowFailureShow("Favorite.LoadFailed", exception);
            favorites = [];
        }

        List<PRosterItem> fresh = [];
        foreach (LVistaRow row in favorites)
        {
            fresh.Add(new PRosterItem(
                row.LVistaRowId,
                row.LVistaRowHeadword,
                row.LVistaRowName,
                row.LVistaRowLanguage,
                row.LVistaRowEpithet ?? string.Empty,
                row.LVistaRowChosen));
        }

        LSplice.LSpliceApply(
            _pRosterList, fresh, PRosterItem.PRosterItemMatch, PRosterItem.PRosterItemSync);

        PRosterEmpty.Visibility = _pRosterList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PRosterHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PRosterItem item)
        {
            return;
        }

        if (!PFavoriteLeaveConfirm())
        {
            return;
        }

        _pFavoriteHost.PVoyageRecord();
        _lFavorite.LFavoritePanel.LPanelRowShow(item.PRosterItemId);
    }

    private void PRosterApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PRosterItem roster)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PRosterRow") is Button row)
        {
            if (roster.PRosterItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= PRosterHandle;
            row.Click += PRosterHandle;
        }

        if (QLook.QLookPartFind<Image>(container, "PRosterFlag") is Image flag)
        {
            flag.Source = roster.PRosterItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "PRosterName") is Run name)
        {
            name.Text = roster.PRosterItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PRosterEpithet") is Run epithet)
        {
            epithet.Text = " " + roster.PRosterItemEpithet;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PRosterLanguage") is TextBlock language)
        {
            language.Text = roster.PRosterItemLanguage;
        }
    }

    internal void PRosterEntryShow(long id)
    {
        _lFavorite.LFavoritePanel.LPanelRowShow(id);
    }

    private void PFavoriteScribeHandle(object sender, RoutedEventArgs e)
    {
        _lFavorite.LFavoritePanel.LPanelScribeSet(ReferenceEquals(sender, PFavoriteScribe));
    }

    internal void PFavoriteScribeRestore(bool editing)
    {
        _lFavorite.LFavoritePanel.LPanelScribeRestore(editing);
    }

    internal long PFavoriteVoyageRead()
    {
        return _lFavorite.LFavoriteVoyageRead();
    }

    internal bool PFavoriteLeaveConfirm()
    {
        return _lFavorite.LFavoritePanel.LPanelLeaveConfirm();
    }

    private void PFavoriteStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PFavoriteBinHandle(object sender, RoutedEventArgs e)
    {
        _lFavorite.LFavoritePanel.LPanelDelete();
    }
}
