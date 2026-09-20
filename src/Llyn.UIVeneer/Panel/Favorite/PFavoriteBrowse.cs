using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

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
        _lFavorite.LFavoriteSeriesSet(PSender.PSenderOrderRead(sender));
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
        _lFavorite.LFavoriteStrainerSet(PChoice.PChoiceFilterRead(PStrainerList));
        PStrainerRestore();
    }

    internal async void PFavoriteVistaRestore()
    {
        _lFavorite.LFavoriteVistaRestore(_pFavoriteHost.PWindowDeportment);
        _lFavorite.LFavoriteObserverAttach(LSubject.LSubjectVista, PObserver.PObserverCreate(this, PRosterFind));
        _lFavorite.LFavoriteObserverAttach(
            LSubject.LSubjectWorkspace, PObserver.PObserverCreate(this, PFavoriteWorkspaceUpdate));
        _lFavorite.LFavoriteObserverAttach(LSubject.LSubjectGrasp, PObserver.PObserverCreate(this, PSeriesGraspUpdate));
        _lFavorite.LFavoriteObserverAttach(LSubject.LSubjectEntry, PObserver.PObserverCreate(this, PRosterEntryUpdate));
        _lFavorite.LFavoriteObserverAttach(LSubject.LSubjectFavorite, PObserver.PObserverCreate(this, PRosterFind));
        _lFavorite.LFavoriteObserverAttach(LSubject.LSubjectReflex, PObserver.PObserverCreate(this, PRosterFind));
        _lFavorite.LFavoriteObserverAttach(LSubject.LSubjectSettings, PObserver.PObserverCreate(this, PRosterFind));
        _lFavorite.LFavoriteChosenAttach(LSubject.LSubjectEntry, PObserver.PObserverCreate(this, PFavoriteEntryUpdate));
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
                row.LVistaRowLanguage,
                row.LVistaRowEpithet ?? string.Empty,
                row.LVistaRowChosen)
            {
                PRosterItemName = row.LVistaRowName,
            });
        }

        PSplice.PSpliceApply(
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
        PRosterEntryShow(item.PRosterItemId);
    }

    internal void PRosterEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            _lFavorite.LFavoriteSelect(id);
            draft = _lFavorite.LFavoriteLoad();
        }
        catch (Exception exception)
        {
            _pFavoriteHost.PWindowFailureShow("Favorite.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            PFavoriteClear();
            PRosterFind();
            return;
        }

        _lFavorite.LFavoriteSelect(id);
        PRosterFind();
        PFavoriteEntryShow(draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PRosterEntryUpdate(LBulletin bulletin)
    {
        if (bulletin.LBulletinStored)
        {
            if (IsVisible)
            {
                if (PEditor.Visibility == Visibility.Visible)
                {
                    _lFavorite.LFavoriteSelect(bulletin.LBulletinId);
                }
            }
        }

        PFavoriteCommandApply();
        PRosterFind();
    }

    private void PFavoriteEntryUpdate(LBulletin bulletin)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lFavorite.LFavoriteLoad();
        }
        catch (Exception)
        {
            return;
        }

        if (draft is null)
        {
            PFavoriteClear();
            return;
        }

        PFavoriteEntryShow(draft);
    }

    private void PFavoriteScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PFavoriteScribe);
        if (editing == (PEditor.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PFavoriteLeaveConfirm())
            {
                PFavoriteScribeShow(true);
                return;
            }

            PFavoriteScribeShow(false);

            if (_lFavorite.LFavoriteChosen is long chosen)
            {
                PRosterEntryShow(chosen);
                return;
            }

            PFavoriteClear();
            return;
        }

        if (_lFavorite.LFavoriteChosen is not long shown)
        {
            PFavoriteClear();
            return;
        }

        PEditor.PEditorEntryShow(shown);
        PFavoriteScribeShow(true);
    }

    private void PFavoriteScribeShow(bool editing)
    {
        _lFavorite.LFavoriteScribeSet(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PFavoriteViewer.IsChecked = !editing;
        PFavoriteScribe.IsChecked = editing;
    }

    internal void PFavoriteScribeRestore(bool editing)
    {
        if (editing)
        {
            if (_lFavorite.LFavoriteChosen is null)
            {
                return;
            }
        }

        if (editing)
        {
            PFavoriteMode.IsEnabled = true;
        }

        PFavoriteScribeShow(editing);
    }

    internal long PFavoriteVoyageRead()
    {
        return _lFavorite.LFavoriteVoyageRead();
    }

    internal bool PFavoriteLeaveConfirm()
    {
        return _pFavoriteHost.PWindowDiscardConfirm(PFavoriteChangeCheck(), PFavoriteDraftFinish);
    }

    private void PFavoriteEntryShow(LEntryDraft draft)
    {
        PDisplay.PDisplayShow(draft);
        PFavoriteMode.IsEnabled = true;
        PFavoriteCommandApply();
    }

    private void PFavoriteCommandApply()
    {
        PFavoriteBin.IsEnabled = _lFavorite.LFavoriteChosen is not null;
    }

    private void PFavoriteClear()
    {
        _lFavorite.LFavoriteSelect(null);
        PRosterFind();
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PFavoriteScribeShow(false);
        PFavoriteMode.IsEnabled = false;
        PFavoriteCommandApply();
    }

    private void PFavoriteStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PFavoriteBinHandle(object sender, RoutedEventArgs e)
    {
        if (!_pFavoriteHost.PWindowDeleteConfirm())
        {
            return;
        }

        try
        {
            _lFavorite.LFavoriteDelete();
        }
        catch (Exception exception)
        {
            _pFavoriteHost.PWindowFailureShow("Scribe.DeleteFailed", exception);
            return;
        }

        PFavoriteClear();
    }
}
