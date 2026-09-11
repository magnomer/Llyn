using System;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PEditorEntryShow(long id)
    {
        LDraft? started = PEditorDraftStart(id);

        if (started is null)
        {
            PEditorReset();
            return;
        }

        PEditorDraftShow(started.LDraftContent);
        PEditorChangeUpdate();
        PEditorFavoriteShow();
    }

    internal void PEditorFavoriteShow()
    {
        long? entry = PEditorEntryRead();

        if (entry is null)
        {
            PEditorFavorite.IsEnabled = false;
            PEditorFavorite.IsChecked = false;
            return;
        }

        PEditorFavorite.IsEnabled = true;

        try
        {
            PEditorFavorite.IsChecked = _lEngine.LEngineFavoriteCheck(entry.Value);
        }
        catch (Exception)
        {
            PEditorFavorite.IsChecked = false;
        }
    }

    private void PEditorFavoriteHandle(object sender, RoutedEventArgs e)
    {
        long? entry = PEditorEntryRead();

        if (entry is null)
        {
            PEditorFavorite.IsChecked = false;
            return;
        }

        bool marked = PEditorFavorite.IsChecked == true;
        try
        {
            if (marked)
            {
                _lEngine.LEngineFavoriteSave(entry.Value);
            }
            else
            {
                _lEngine.LEngineFavoriteDelete(entry.Value);
            }
        }
        catch (Exception exception)
        {
            PEditorFavorite.IsChecked = !marked;
            _pEditorHost.PWindowFailureShow("Favorite.MarkFailed", exception);
        }
    }

    private long? PEditorEntryRead()
    {
        if (_pEditorDraft == 0)
        {
            return null;
        }

        LDraft? held;
        try
        {
            held = _lEngine.LEngineDraftRead(_pEditorDraft);
        }
        catch (Exception)
        {
            return null;
        }

        return held?.LDraftEntry is null or 0 ? null : held.LDraftEntry;
    }

    private void PEditorStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditorEntrySave();
    }

    internal void PEditorEntrySave()
    {
        PEditorChangeSave();

        long held = _pEditorDraft;
        if (held == 0)
        {
            return;
        }

        long? entry = PEditorEntryRead();

        LEntry stored;
        try
        {
            stored = _lEngine.LEngineDraftCommit(held);
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow(entry is null ? "Input.SaveFailed" : "Input.UpdateFailed", exception);
            return;
        }

        _pEditorDraft = 0;

        if (entry is not null)
        {
            PEditorEntryShow(stored.LEntryId);
            return;
        }

        PEditorReset();
    }

    private void PEditorDiscardHandle(object sender, RoutedEventArgs e)
    {
        long? entry = PEditorEntryRead();

        PEditorChangeStop();
        PEditorDraftCancel();

        if (entry is null)
        {
            PEditorReset();
            return;
        }

        PEditorEntryShow(entry.Value);
    }
}
