using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Llyn.Application;
using Llyn.Core;

using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private readonly ObservableCollection<LTranscriptionItem> _pTranscriptionItem = [];
    private IReadOnlyList<string> _pTranscriptionSchemes = [];

    internal void PTranscriptionAddHandle(object sender, ExecutedRoutedEventArgs e)
    {
        string? scheme = PTranscriptionSchemeFind();
        if (scheme is null)
        {
            return;
        }

        int position = e.Parameter is LTranscriptionItem row
            ? _pTranscriptionItem.IndexOf(row) + 1
            : _pTranscriptionItem.Count;
        PEditorRequestSend(new LRequestTranscriptionAddition(PEditorDraft, scheme, position));
    }

    internal void PTranscriptionAddCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = PTranscriptionSchemeFind() is not null;
    }

    internal void PTranscriptionRemoveHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is LTranscriptionItem row)
        {
            PEditorRequestSend(new LRequestTranscriptionRemoval(PEditorDraft, row.LTranscriptionItemId));
        }
    }

    internal async void PTranscriptionNotationHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is LTranscriptionItem row)
        {
            await PNotationOpen(
                e.OriginalSource as UIElement ?? PTranscription,
                row.LTranscriptionItemId,
                row.LTranscriptionItemScheme);
        }
    }

    private string? PTranscriptionSchemeFind()
    {
        foreach (string scheme in _pTranscriptionSchemes)
        {
            if (_pTranscriptionItem.All(
                row => !string.Equals(row.LTranscriptionItemScheme, scheme, StringComparison.Ordinal)))
            {
                return scheme;
            }
        }

        return null;
    }

    private LTranscriptionItem? PTranscriptionFind(long id)
    {
        foreach (LTranscriptionItem row in _pTranscriptionItem)
        {
            if (row.LTranscriptionItemId == id)
            {
                return row;
            }
        }

        foreach (LTranscriptionItem row in _pGlyphItem)
        {
            if (row.LTranscriptionItemId == id)
            {
                return row;
            }
        }

        return null;
    }

    private void PTranscriptionChangeHandle(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not LTranscriptionItem row)
        {
            return;
        }

        if (string.Equals(
            e.PropertyName, nameof(LTranscriptionItem.LTranscriptionItemScheme), StringComparison.Ordinal))
        {
            PEditorRequestSend(
                new LRequestTranscriptionScheme(PEditorDraft, row.LTranscriptionItemId, row.LTranscriptionItemScheme));
            return;
        }

        if (!string.Equals(e.PropertyName, nameof(LTranscriptionItem.LTranscriptionItemText), StringComparison.Ordinal))
        {
            return;
        }

        PEditorRequestDefer(
            new LRequestTranscriptionText(PEditorDraft, row.LTranscriptionItemId, row.LTranscriptionItemText));
    }

    private void PTranscriptionShow(LEntryDraft draft)
    {
        _pTranscriptionSchemes = _pEditorHost.PWindowDeportment.LWindowSchemeRead(draft.LEntryDraftLanguage);
        PTranscription.Visibility = _pTranscriptionSchemes.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

        PCard.PCardRowShow(
            _pTranscriptionItem,
            PTranscriptionScan(draft),
            static row => row.LTranscriptionItemId,
            static spelled => spelled.LTranscriptionDraftId,
            PTranscriptionCreate,
            PTranscriptionUpdate);

        foreach (LTranscriptionItem row in _pTranscriptionItem)
        {
            row.LTranscriptionItemUpdate(_pTranscriptionSchemes, _pTranscriptionItem);
        }
    }

    private IReadOnlyList<LTranscriptionDraft> PTranscriptionScan(LEntryDraft draft)
    {
        List<LTranscriptionDraft> rows = [];
        foreach (LTranscriptionDraft spelled in draft.LEntryDraftTranscriptions)
        {
            if (!PGlyphSchemeCheck(spelled.LTranscriptionDraftScheme))
            {
                rows.Add(spelled);
            }
        }

        return rows;
    }

    private LTranscriptionItem PTranscriptionCreate(LTranscriptionDraft spelled)
    {
        LTranscriptionItem row = LTranscriptionItem.LTranscriptionItemCreate(spelled);
        row.PropertyChanged += PTranscriptionChangeHandle;
        return row;
    }

    private LTranscriptionItem PTranscriptionUpdate(LTranscriptionItem row, LTranscriptionDraft spelled)
    {
        row.LTranscriptionItemScheme = spelled.LTranscriptionDraftScheme;
        row.LTranscriptionItemText = spelled.LTranscriptionDraftText;
        return row;
    }
}
