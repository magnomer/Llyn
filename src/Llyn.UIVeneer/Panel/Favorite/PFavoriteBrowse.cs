using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PFavorite
{
    private readonly ObservableCollection<PRosterItem> _pRosterList = [];

    private async void PFavoriteWorkspaceUpdate()
    {
        await PEnsign.PEnsignLoad(_pFavoriteHost.PWindowDeportment);
        PFavoriteReset();
    }

    private void PRecallHandle(object sender, TextChangedEventArgs e)
    {
        _lFavorite.LFavoriteRecallSet(PRecall.Text ?? string.Empty);
    }

    private void PSeriesHandle(object sender, RoutedEventArgs e)
    {
        PSeriesDropper.IsChecked = false;
        _lFavorite.LFavoriteSeriesSet(LChoice.LChoiceOrderRead(sender));
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
        _lFavorite.LFavoriteStrainerSet(LChoice.LChoiceFilterRead(PStrainerList));
        PStrainerRestore();
    }

    internal async void PFavoriteVistaRestore()
    {
        _lFavorite.LFavoriteVistaRestore(_pFavoriteHost.PWindowDeportment);
        _lFavorite.LFavoritePanel.LPanelObserverAttach(
            LSubject.LSubjectVista, PObserver.PObserverCreate(this, PRosterFind));
        _lFavorite.LFavoritePanel.LPanelObserverAttach(
            LSubject.LSubjectWorkspace, PObserver.PObserverCreate(this, PFavoriteWorkspaceUpdate));
        _lFavorite.LFavoritePanel.LPanelObserverAttach(
            LSubject.LSubjectGrasp, PObserver.PObserverCreate(this, PSeriesGraspUpdate));
        _lFavorite.LFavoritePanel.LPanelObserverAttach(
            LSubject.LSubjectEntry, PObserver.PObserverCreate(this, _lFavorite.LFavoritePanel.LPanelEntryHandle));
        _lFavorite.LFavoritePanel.LPanelObserverAttach(
            LSubject.LSubjectFavorite, PObserver.PObserverCreate(this, PRosterFind));
        _lFavorite.LFavoritePanel.LPanelObserverAttach(
            LSubject.LSubjectReflex, PObserver.PObserverCreate(this, PRosterFind));
        _lFavorite.LFavoritePanel.LPanelObserverAttach(
            LSubject.LSubjectSettings, PObserver.PObserverCreate(this, PRosterFind));
        _lFavorite.LFavoritePanel.LPanelChosenAttach(
            LSubject.LSubjectEntry, PObserver.PObserverCreate(this, _lFavorite.LFavoritePanel.LPanelDraftUpdate));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderBuild(
            PSeriesList,
            "Series",
            PSeriesHandle,
            [
                LCatalogOrder.LCatalogOrderHeadword,
                LCatalogOrder.LCatalogOrderReverse,
                LCatalogOrder.LCatalogOrderLanguage,
                LCatalogOrder.LCatalogOrderMarked,
                LCatalogOrder.LCatalogOrderGrasp,
            ]);
        PChoice.PChoiceOrderApply(PSeriesDropdown, _lFavorite.LFavoriteOrder);
        PStrainerRestore();

        await PEnsign.PEnsignLoad(_pFavoriteHost.PWindowDeportment);

        PChoice.PChoiceFilterBuild(
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

    internal void PRosterEntryShow(long id)
    {
        _lFavorite.LFavoritePanel.LPanelRowShow(id);
    }

    private void PFavoriteEntryUpdate(LDraft draft)
    {
        PDisplay.PDisplayShow(draft.LDraftContent);
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
