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

    private LVista? _pFavoriteVista;

    private async void PFavoriteWorkspaceUpdate()
    {
        await PEnsign.PEnsignLoad(_lEngine);
        PFavoriteReset();
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
        vista.LVistaObserverAttach(LSubject.LSubjectVista, new PObserver(this, PRosterFind));
        vista.LVistaObserverAttach(LSubject.LSubjectWorkspace, new PObserver(this, PFavoriteWorkspaceUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectGrasp, new PObserver(this, PSeriesGraspUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectEntry, new PObserver(this, PRosterEntryUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectFavorite, new PObserver(this, PRosterFind));
        vista.LVistaObserverAttach(LSubject.LSubjectReflex, new PObserver(this, PRosterFind));
        vista.LVistaObserverAttach(LSubject.LSubjectSettings, new PObserver(this, PRosterFind));
        vista.LVistaChosenAttach(LSubject.LSubjectEntry, new PObserver(this, PFavoriteEntryUpdate));
        PDisplay.PDisplayVistaRestore(vista);
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

    private void PRosterFind()
    {
        IReadOnlyList<LVistaRow> favorites;
        try
        {
            favorites = _pFavoriteVista is null ? [] : _lEngine.LEngineFavoriteFind(_pFavoriteVista);
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

        PRosterEntryShow(item.PRosterItemId);
    }

    internal void PRosterEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            _pFavoriteVista?.LVistaSelect(id);
            draft = _pFavoriteVista?.LVistaLoad()?.LDraftContent;
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

        _pFavoriteVista?.LVistaSelect(id);
        PRosterFind();
        PFavoriteEntryShow(draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PRosterEntryUpdate(LBulletin bulletin)
    {
        if (bulletin.LBulletinId > 0 && IsVisible && PEditor.Visibility == Visibility.Visible)
        {
            _pFavoriteVista?.LVistaSelect(bulletin.LBulletinId);
        }

        PFavoriteCommandApply();
        PRosterFind();
    }

    private void PFavoriteEntryUpdate(LBulletin bulletin)
    {
        LEntryDraft? draft;
        try
        {
            draft = _pFavoriteVista?.LVistaLoad()?.LDraftContent;
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

            if (_pFavoriteVista?.LVistaChosen is long chosen)
            {
                PRosterEntryShow(chosen);
                return;
            }

            PFavoriteClear();
            return;
        }

        if (_pFavoriteVista?.LVistaChosen is not long shown)
        {
            PFavoriteClear();
            return;
        }

        PEditor.PEditorEntryShow(shown);
        PFavoriteScribeShow(true);
    }

    private void PFavoriteScribeShow(bool editing)
    {
        _pFavoriteVista?.LVistaEditingSet(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PFavoriteViewer.IsChecked = !editing;
        PFavoriteScribe.IsChecked = editing;
    }

    internal void PFavoriteScribeRestore(bool editing)
    {
        if (editing && _pFavoriteVista?.LVistaChosen is null)
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

    private void PFavoriteEntryShow(LEntryDraft draft)
    {
        PDisplay.PDisplayShow(draft);
        PFavoriteMode.IsEnabled = true;
        PFavoriteCommandApply();
    }

    private void PFavoriteCommandApply()
    {
        PFavoriteBin.IsEnabled = _pFavoriteVista?.LVistaChosen is not null;
    }

    private void PFavoriteClear()
    {
        _pFavoriteVista?.LVistaSelect(null);
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
            _pFavoriteVista?.LVistaDelete();
        }
        catch (Exception exception)
        {
            _pFavoriteHost.PWindowFailureShow("Scribe.DeleteFailed", exception);
            return;
        }

        PFavoriteClear();
    }
}
