using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PFavorite
{
    private readonly ObservableCollection<PRosterItem> _pRosterList = [];

    private string? _pRosterEntry;

    private LCatalogOrder _pSeriesChoice;

    private async void PFavoriteHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
        {
            return;
        }

        PRoster.ItemsSource = _pRosterList;

        await PEnsign.PEnsignLoad(_lEngine);

        PRosterFind(PRecall.Text ?? string.Empty);
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

    internal void PSeriesRestore(LCatalogOrder order)
    {
        _pSeriesChoice = order;
        PChoice.PChoiceOrderApply(PSeriesDropdown, order);
    }

    private void PRosterFind(string query)
    {
        IReadOnlyList<LFavorite> favorites;
        try
        {
            favorites = _lEngine.LEngineFavoriteFind(query, _pSeriesChoice);
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
                favorite.LFavoriteEntry.LEntryLanguage));
        }

        PRosterEmpty.Visibility = _pRosterList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PRosterUpdate()
    {
        PRosterFind(PRecall.Text ?? string.Empty);
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

    internal void PRosterEntryShow(string id)
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
        PFavoriteEntryShow(id, draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PRosterEntryUpdate(string id)
    {
        _pRosterEntry = id;
        PRosterFind(PRecall.Text ?? string.Empty);

        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception)
        {
            return;
        }

        if (draft is not null)
        {
            PFavoriteEntryShow(id, draft);
        }
    }

    private void PEditorEntryRestore()
    {
        if (_pRosterEntry is null)
        {
            PEditor.PEditorReset();
            return;
        }

        PEditor.PEditorEntryShow(_pRosterEntry);
    }

    private void PFavoriteScribeHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            if (!PFavoriteLeaveConfirm())
            {
                return;
            }

            PFavoriteScribeShow(false);

            if (_pRosterEntry is not null)
            {
                PRosterEntryShow(_pRosterEntry);
            }

            return;
        }

        if (_pRosterEntry is null)
        {
            return;
        }

        PEditor.PEditorEntryShow(_pRosterEntry);
        PFavoriteScribeShow(true);
    }

    private void PFavoriteScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PFavoriteScribe.SetResourceReference(ButtonBase.ContentProperty, editing ? "Scribe.Read" : "Scribe.Edit");
    }

    internal void PFavoriteScribeRestore(bool editing)
    {
        if (editing)
        {
            PFavoriteScribe.IsEnabled = true;
        }

        PFavoriteScribeShow(editing);
    }

    internal bool PFavoriteLeaveConfirm()
    {
        return _pFavoriteHost.PWindowDiscardConfirm(PFavoriteChangeCheck());
    }

    private void PFavoriteEntryShow(string id, LEntryDraft draft)
    {
        PDisplay.PDisplayShow(id, draft);
        PFavoriteScribe.IsEnabled = true;
    }

    private void PFavoriteClear()
    {
        _pRosterEntry = null;
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PFavoriteScribeShow(false);
        PFavoriteScribe.IsEnabled = false;
    }
}
