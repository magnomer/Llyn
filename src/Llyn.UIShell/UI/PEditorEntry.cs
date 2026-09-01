using System;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private string? _pEditorEntry;

    internal void PEditorEntryShow(string id)
    {
        _pEditorEntry = null;

        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception)
        {
            draft = null;
        }

        if (draft is null)
        {
            PEditorReset();
            return;
        }

        PEditorDraftShow(draft);
        _pEditorEntry = id;
    }

    private void PStoreHandle(object sender, RoutedEventArgs e)
    {
        string? entry = _pEditorEntry;

        LEntry stored;
        try
        {
            stored = entry is null
                ? _lEngine.LEngineEntrySave(PEditorDraftRead())
                : _lEngine.LEngineEntryUpdate(entry, PEditorDraftRead());
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow(entry is null ? "Input.SaveFailed" : "Input.UpdateFailed", exception);
            return;
        }

        if (entry is not null)
        {
            PEditorEntryShow(stored.LEntryId);
            PEditorStoreDispatcher?.Invoke(stored.LEntryId);
            return;
        }

        PEditorReset();
        PEditorStoreDispatcher?.Invoke(stored.LEntryId);
    }

    private void PDiscardHandle(object sender, RoutedEventArgs e)
    {
        if (PEditorDiscardDispatcher is not null)
        {
            PEditorDiscardDispatcher();
            return;
        }

        PEditorReset();
    }
}
