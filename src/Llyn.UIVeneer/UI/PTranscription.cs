using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private readonly ObservableCollection<PTranscriptionItem> _pTranscriptionItem = [];
    private IReadOnlyList<string> _pTranscriptionSchemes = [];

    internal void PTranscriptionAddHandle(object sender, ExecutedRoutedEventArgs e)
    {
        string? scheme = PTranscriptionSchemeFind();
        if (scheme is null)
        {
            return;
        }

        int position = e.Parameter is PTranscriptionItem row
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
        if (e.Parameter is PTranscriptionItem row)
        {
            PEditorRequestSend(new LRequestTranscriptionRemoval(PEditorDraft, row.PTranscriptionItemId));
        }
    }

    internal async void PTranscriptionNotationHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PTranscriptionItem row)
        {
            await PNotationOpen(
                e.OriginalSource as UIElement ?? PTranscription,
                row.PTranscriptionItemId,
                row.PTranscriptionItemScheme);
        }
    }

    private string? PTranscriptionSchemeFind()
    {
        foreach (string scheme in _pTranscriptionSchemes)
        {
            if (_pTranscriptionItem.All(
                row => !string.Equals(row.PTranscriptionItemScheme, scheme, StringComparison.Ordinal)))
            {
                return scheme;
            }
        }

        return null;
    }

    private PTranscriptionItem? PTranscriptionFind(long id)
    {
        foreach (PTranscriptionItem row in _pTranscriptionItem)
        {
            if (row.PTranscriptionItemId == id)
            {
                return row;
            }
        }

        foreach (PTranscriptionItem row in _pGlyphItem)
        {
            if (row.PTranscriptionItemId == id)
            {
                return row;
            }
        }

        return null;
    }

    private void PTranscriptionChangeHandle(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not PTranscriptionItem row)
        {
            return;
        }

        if (string.Equals(
            e.PropertyName, nameof(PTranscriptionItem.PTranscriptionItemScheme), StringComparison.Ordinal))
        {
            PEditorRequestSend(
                new LRequestTranscriptionScheme(PEditorDraft, row.PTranscriptionItemId, row.PTranscriptionItemScheme));
            return;
        }

        if (!string.Equals(e.PropertyName, nameof(PTranscriptionItem.PTranscriptionItemText), StringComparison.Ordinal))
        {
            return;
        }

        PEditorRequestDefer(
            new LRequestTranscriptionText(PEditorDraft, row.PTranscriptionItemId, row.PTranscriptionItemText));
    }

    private void PTranscriptionShow(LEntryDraft draft)
    {
        _pTranscriptionSchemes = _lEngine.LEngineSchemeRead(draft.LEntryDraftLanguage);
        PTranscription.Visibility = _pTranscriptionSchemes.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

        PCard.PCardRowShow(
            _pTranscriptionItem,
            PTranscriptionScan(draft),
            static row => row.PTranscriptionItemId,
            static spelled => spelled.LTranscriptionDraftId,
            PTranscriptionCreate,
            PTranscriptionUpdate);

        foreach (PTranscriptionItem row in _pTranscriptionItem)
        {
            row.PTranscriptionItemUpdate(_pTranscriptionSchemes, _pTranscriptionItem);
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

    private void PTranscriptionPrepare(LEntryDraft draft)
    {
        IReadOnlyList<string> schemes = _lEngine.LEngineSchemeRead(draft.LEntryDraftLanguage);
        if (schemes.Count == 0)
        {
            return;
        }

        if (PTranscriptionScan(draft).Count > 0)
        {
            return;
        }

        PEditorRequestSend(new LRequestTranscriptionAddition(PEditorDraft, schemes[0], 0, true));
    }

    private PTranscriptionItem PTranscriptionCreate(LTranscriptionDraft spelled)
    {
        PTranscriptionItem row = PTranscriptionItem.PTranscriptionItemCreate(_pEditorHost, spelled);
        row.PropertyChanged += PTranscriptionChangeHandle;
        return row;
    }

    private PTranscriptionItem PTranscriptionUpdate(PTranscriptionItem row, LTranscriptionDraft spelled)
    {
        row.PTranscriptionItemScheme = spelled.LTranscriptionDraftScheme;
        row.PTranscriptionItemText = spelled.LTranscriptionDraftText;
        return row;
    }

    private void PTranscriptionClear()
    {
        foreach (PTranscriptionItem row in _pTranscriptionItem)
        {
            row.PropertyChanged -= PTranscriptionChangeHandle;
        }

        _pTranscriptionItem.Clear();
        _pTranscriptionSchemes = [];
        PTranscription.Visibility = Visibility.Collapsed;
    }
}
