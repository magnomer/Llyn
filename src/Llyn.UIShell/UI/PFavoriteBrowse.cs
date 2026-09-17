using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PFavorite
{
    private readonly ObservableCollection<PRosterItem> _pRosterList = [];

    private long? _pRosterEntry;

    private LVista? _pFavoriteVista;

    private async void PFavoriteBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectVista)
        {
            if (_pFavoriteVista is not null && bulletin.LBulletinId == _pFavoriteVista.LVistaId)
            {
                PRosterFind();
            }

            return;
        }

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
        _pFavoriteVista?.LVistaQuerySet(PRecall.Text ?? string.Empty);
    }

    private void PSeriesHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice } || _pFavoriteVista is null)
        {
            return;
        }

        PSeriesDropper.IsChecked = false;
        _pFavoriteVista.LVistaOrderSet(LCatalog.LCatalogOrderParse(choice, _pFavoriteVista.LVistaOrder));
    }

    private void PSeriesGraspUpdate()
    {
        if (_pFavoriteVista?.LVistaOrder != LCatalogOrder.LCatalogOrderGrasp)
        {
            return;
        }

        PRosterFind();
    }

    private void PStrainerHandle(object sender, RoutedEventArgs e)
    {
        if (_pFavoriteVista is null)
        {
            return;
        }

        _pFavoriteVista.LVistaFilterSet(PChoice.PChoiceFilterRead(PStrainerList));
        PStrainerRestore();
    }

    internal async void PFavoriteVistaRestore(LVista vista)
    {
        _pFavoriteVista = vista;
        PSeriesRestore();
        PStrainerRestore();

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PStrainerList, _lEngine.LEngineLanguageRead(), vista.LVistaFilter, PStrainerHandle);
        vista.LVistaQuerySet(PRecall.Text ?? string.Empty);
        PRosterFind();
    }

    private void PSeriesRestore()
    {
        if (_pFavoriteVista is not null)
        {
            PChoice.PChoiceOrderApply(PSeriesDropdown, _pFavoriteVista.LVistaOrder);
        }
    }

    private void PStrainerRestore()
    {
        bool active = _pFavoriteVista?.LVistaFilter.LCatalogFilterActive == true;
        PStrainerMark.Visibility = active ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PRosterSelect(long? id)
    {
        foreach (PRosterItem item in _pRosterList)
        {
            item.PRosterItemChosen = id is not null
                && item.PRosterItemId == id;
        }
    }

    private void PRosterFind()
    {
        _pRosterList.Clear();
        if (_pFavoriteVista is null)
        {
            return;
        }

        IReadOnlyList<LVistaRow> favorites;
        try
        {
            favorites = _lEngine.LEngineFavoriteFind(_pFavoriteVista);
        }
        catch (Exception exception)
        {
            _pFavoriteHost.PWindowFailureShow("Favorite.LoadFailed", exception);
            favorites = [];
        }

        foreach (LVistaRow row in favorites)
        {
            _pRosterList.Add(new PRosterItem(
                row.LVistaRowId,
                row.LVistaRowHeadword,
                row.LVistaRowLanguage,
                row.LVistaRowEpithet ?? string.Empty)
            {
                PRosterItemName = row.LVistaRowName,
            });
        }

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
            PRosterFind();
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

        PRosterFind();

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
