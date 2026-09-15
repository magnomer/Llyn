using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PReflexItem> _pReflexItem = [];

    private static string PReflexRequestFormat(long id, string field)
    {
        return string.Concat("Reflex:", id.ToString(CultureInfo.InvariantCulture), ":", field);
    }

    internal void PReflexAddHandle(object sender, ExecutedRoutedEventArgs e)
    {
        PReflexItem? row = e.Parameter as PReflexItem;
        int position = row is null ? _pReflexItem.Count : _pReflexItem.IndexOf(row) + 1;
        PEditorRequestSend(new LRequestReflexAddition(
            _pEditorDraft, row?.PReflexItemLanguage ?? string.Empty, row?.PReflexItemKind ?? string.Empty, position));
    }

    internal void PReflexRemoveHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PReflexItem row)
        {
            PEditorRequestSend(new LRequestReflexRemoval(_pEditorDraft, row.PReflexItemId));
        }
    }

    internal void PReflexMainHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PReflexItem row)
        {
            PEditorRequestSend(new LRequestReflexMain(_pEditorDraft, row.PReflexItemId, !row.PReflexItemMain));
        }
    }

    internal void PReflexRebuildHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (PEditorEntryRead() is not long entry)
        {
            return;
        }

        try
        {
            _lEngine.LEngineReflexRebuild(entry);
        }
        catch (Exception)
        {
        }
    }

    private void PReflexChangeHandle(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not PReflexItem row)
        {
            return;
        }

        string field = e.PropertyName ?? string.Empty;
        LRequest? request = field switch
        {
            nameof(PReflexItem.PReflexItemLanguage) =>
                new LRequestReflexLanguage(_pEditorDraft, row.PReflexItemId, row.PReflexItemLanguage),
            nameof(PReflexItem.PReflexItemKind) =>
                new LRequestReflexKind(_pEditorDraft, row.PReflexItemId, row.PReflexItemKind),
            nameof(PReflexItem.PReflexItemText) => row.PReflexItemRespelled
                ? new LRequestReflexRespelling(_pEditorDraft, row.PReflexItemId, row.PReflexItemText)
                : new LRequestReflexText(_pEditorDraft, row.PReflexItemId, row.PReflexItemText),
            nameof(PReflexItem.PReflexItemNote) =>
                new LRequestReflexNote(_pEditorDraft, row.PReflexItemId, row.PReflexItemNote),
            _ => null,
        };

        if (request is null)
        {
            return;
        }

        if (string.Equals(field, nameof(PReflexItem.PReflexItemLanguage), StringComparison.Ordinal))
        {
            PDisplay.PReflexLeadApply(_pReflexItem);
        }

        PEditorRequestDefer(PReflexRequestFormat(row.PReflexItemId, field), request);
    }

    private void PReflexShow(LEntryDraft draft)
    {
        string language = draft.LEntryDraftLanguage;
        bool declared = language.Length > 0 && _lEngine.LEngineReflexRead(language).Count > 0;
        PReflexBlock.Visibility = declared || draft.LEntryDraftReflexes.Count > 0
            ? Visibility.Visible
            : Visibility.Collapsed;

        PCard.PCardRowShow(
            _pReflexItem,
            draft.LEntryDraftReflexes,
            static row => row.PReflexItemId,
            static reflex => reflex.LReflexDraftId,
            PReflexCreate,
            PReflexUpdate);

        PDisplay.PReflexLeadApply(_pReflexItem);
    }

    private void PReflexPrepare(LEntryDraft draft)
    {
        long? entry = PEditorEntryRead();
        if (entry is null || draft.LEntryDraftReflexes.Count > 0)
        {
            return;
        }

        try
        {
            _lEngine.LEngineReflexStart(entry.Value);
        }
        catch (Exception)
        {
        }
    }

    private PReflexItem PReflexCreate(LReflexDraft reflex)
    {
        PReflexItem row = PReflexItem.PReflexItemCreate(
            _pEditorHost, reflex, PRespelling.PRespellingRead(_lEngine, reflex.LReflexDraftLanguage));
        row.PropertyChanged += PReflexChangeHandle;
        return row;
    }

    private PReflexItem PReflexUpdate(PReflexItem row, LReflexDraft reflex)
    {
        PRespelling respelling = PRespelling.PRespellingRead(_lEngine, reflex.LReflexDraftLanguage);
        if (respelling.PRespellingShown != row.PReflexItemRespelled)
        {
            row.PropertyChanged -= PReflexChangeHandle;
            return PReflexCreate(reflex);
        }

        row.PReflexItemMain = reflex.LReflexDraftMain;

        if (!PEditorRequestCheck(PReflexRequestFormat(row.PReflexItemId, nameof(PReflexItem.PReflexItemLanguage))))
        {
            row.PReflexItemLanguage = reflex.LReflexDraftLanguage;
        }

        if (!PEditorRequestCheck(PReflexRequestFormat(row.PReflexItemId, nameof(PReflexItem.PReflexItemKind))))
        {
            row.PReflexItemKind = reflex.LReflexDraftKind;
        }

        if (!PEditorRequestCheck(PReflexRequestFormat(row.PReflexItemId, nameof(PReflexItem.PReflexItemText))))
        {
            row.PReflexItemText = PReflexItem.PReflexTextRead(reflex, respelling);
        }

        if (!PEditorRequestCheck(PReflexRequestFormat(row.PReflexItemId, nameof(PReflexItem.PReflexItemNote))))
        {
            row.PReflexItemNote = reflex.LReflexDraftNote;
        }

        return row;
    }

    private void PReflexClear()
    {
        foreach (PReflexItem row in _pReflexItem)
        {
            row.PropertyChanged -= PReflexChangeHandle;
        }

        _pReflexItem.Clear();
        PReflexBlock.Visibility = Visibility.Collapsed;
    }
}
