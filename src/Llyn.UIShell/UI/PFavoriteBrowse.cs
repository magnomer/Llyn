using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PFavorite
{
    private readonly ObservableCollection<PRosterItem> _pRosterList = [];

    private long? _pRosterEntry;

    private LCatalogOrder _pSeriesChoice;

    private LCatalogFilter _pStrainerChoice = LCatalogFilter.LCatalogFilterEmpty;

    private async void PFavoriteBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            await PEnsign.PEnsignLoad(_lEngine);
            PFavoriteReset();
            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectGrasp)
        {
            PSeriesGraspUpdate();
            return;
        }

        if (!PBulletin.PBulletinEntryCheck(bulletin.LBulletinSubject))
        {
            return;
        }

        PRosterEntryUpdate(bulletin.LBulletinId);
    }

    private void PRecallHandle(object sender, TextChangedEventArgs e)
    {
        PRosterFind(PRecall.Text ?? string.Empty);
    }

    private void PSeriesHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        _pSeriesChoice = LCatalog.LCatalogOrderParse(choice, _pSeriesChoice);
        _lEngine.LEngineSeriesSave(_pSeriesChoice);
        PSeriesDropper.IsChecked = false;
        PRosterFind(PRecall.Text ?? string.Empty);
    }

    private void PSeriesGraspUpdate()
    {
        if (_pSeriesChoice != LCatalogOrder.LCatalogOrderGrasp)
        {
            return;
        }

        PRosterFind(PRecall.Text ?? string.Empty);
    }

    internal async void PSeriesRestore(LCatalogOrder order)
    {
        _pSeriesChoice = order;
        PChoice.PChoiceOrderApply(PSeriesDropdown, order);

        await PEnsign.PEnsignLoad(_lEngine);

        PRosterFind(PRecall.Text ?? string.Empty);
    }

    private void PStrainerHandle(object sender, RoutedEventArgs e)
    {
        _pStrainerChoice = PChoice.PChoiceFilterRead(PStrainerList);
        _lEngine.LEngineStrainerSave(_pStrainerChoice);
        PStrainerMark.Visibility = _pStrainerChoice.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;
        PRosterFind(PRecall.Text ?? string.Empty);
    }

    internal async void PStrainerRestore(LCatalogFilter filter)
    {
        _pStrainerChoice = filter;
        PStrainerMark.Visibility = filter.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PStrainerList, _lEngine.LEngineLanguageRead(), filter, PStrainerHandle);
    }

    private void PRosterSelect(long? id)
    {
        foreach (PRosterItem item in _pRosterList)
        {
            item.PRosterItemChosen = id is not null
                && item.PRosterItemId == id;
        }
    }

    private void PRosterFind(string query)
    {
        IReadOnlyList<LFavorite> favorites;
        try
        {
            favorites = _lEngine.LEngineFavoriteFind(query, _pSeriesChoice, _pStrainerChoice);
        }
        catch (Exception exception)
        {
            _pFavoriteHost.PWindowFailureShow("Favorite.LoadFailed", exception);
            favorites = [];
        }

        _pRosterList.Clear();
        foreach (LFavorite favorite in favorites)
        {
            _pRosterList.Add(new PRosterItem(
                favorite.LFavoriteEntry.LEntryId,
                favorite.LFavoriteEntry.LEntryHeadword,
                favorite.LFavoriteEntry.LEntryLanguage,
                _lEngine.LEngineEpithetRead(favorite.LFavoriteEntry.LEntryId)));
        }

        PTwin.PTwinNameApply(
            _pRosterList,
            row => row.PRosterItemHeadword,
            (row, name) => row.PRosterItemName = name,
            row => row.PRosterItemId);

        PRosterEmpty.Visibility = _pRosterList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PRosterSelect(_pRosterEntry);
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

        PRosterEntryShow(item.PRosterItemId);
    }

    internal void PRosterEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pFavoriteHost.PWindowFailureShow("Favorite.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            PFavoriteClear();
            PRosterFind(PRecall.Text ?? string.Empty);
            return;
        }

        _pRosterEntry = id;
        PRosterSelect(id);
        PFavoriteBin.IsEnabled = true;
        PFavoriteEntryShow(id, draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PRosterEntryUpdate(long id)
    {
        if (id > 0 && IsVisible && PEditor.Visibility == Visibility.Visible)
        {
            _pRosterEntry = id;
            PRosterSelect(id);
        PFavoriteBin.IsEnabled = true;
        }

        PRosterFind(PRecall.Text ?? string.Empty);

        if (_pRosterEntry is not long shown
            || (id > 0 && shown != id))
        {
            return;
        }

        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(shown);
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

        PFavoriteEntryShow(shown, draft);
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

            if (_pRosterEntry is not null)
            {
                PRosterEntryShow(_pRosterEntry.Value);
                return;
            }

            PFavoriteClear();
            return;
        }

        if (_pRosterEntry is null)
        {
            PFavoriteClear();
            return;
        }

        PEditor.PEditorEntryShow(_pRosterEntry.Value);
        PFavoriteScribeShow(true);
    }

    private void PFavoriteScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PFavoriteViewer.IsChecked = !editing;
        PFavoriteScribe.IsChecked = editing;
    }

    internal void PFavoriteScribeRestore(bool editing)
    {
        if (editing && _pRosterEntry is null)
        {
            return;
        }

        if (editing)
        {
            PFavoriteMode.IsEnabled = true;
        }

        PFavoriteScribeShow(editing);
    }

    internal bool PFavoriteLeaveConfirm()
    {
        return _pFavoriteHost.PWindowDiscardConfirm(PFavoriteChangeCheck(), PFavoriteDraftFinish);
    }

    private void PFavoriteEntryShow(long id, LEntryDraft draft)
    {
        PDisplay.PDisplayShow(id, draft);
        PFavoriteMode.IsEnabled = true;
    }

    private void PFavoriteClear()
    {
        _pRosterEntry = null;
        PRosterSelect(null);
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PFavoriteScribeShow(false);
        PFavoriteMode.IsEnabled = false;
        PFavoriteBin.IsEnabled = false;
    }

    private void PFavoriteStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void PFavoriteBinHandle(object sender, RoutedEventArgs e)
    {
        if (_pRosterEntry is not long id)
        {
            return;
        }

        if (!_pFavoriteHost.PWindowDeleteConfirm())
        {
            return;
        }

        try
        {
            _lEngine.LEngineEntryDelete(id);
        }
        catch (Exception exception)
        {
            _pFavoriteHost.PWindowFailureShow("Scribe.DeleteFailed", exception);
            return;
        }

        PFavoriteClear();
    }
}
