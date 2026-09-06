using System;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PEditorEntryShow(string id)
    {
        LDraft? started = PEditorDraftStart(id);

        if (started is null)
        {
            PEditorReset();
            return;
        }

        PEditorDraftShow(started.LDraftContent);
        PEditorChangeUpdate();
    }

    private string? PEditorEntryRead()
    {
        if (_pEditorDraft.Length == 0)
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

        return string.IsNullOrWhiteSpace(held?.LDraftEntry) ? null : held.LDraftEntry;
    }

    private void PEditorStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditorChangeSave();

        string held = _pEditorDraft;
        if (held.Length == 0)
        {
            return;
        }

        string? entry = PEditorEntryRead();

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

        _pEditorDraft = string.Empty;

        if (entry is not null)
        {
            PEditorEntryShow(stored.LEntryId);
            return;
        }

        PEditorReset();
    }

    private void PEditorDiscardHandle(object sender, RoutedEventArgs e)
    {
        string? entry = PEditorEntryRead();

        PEditorChangeStop();
        PEditorDraftCancel();

        if (entry is null)
        {
            PEditorReset();
            return;
        }

        PEditorEntryShow(entry);
    }
}
