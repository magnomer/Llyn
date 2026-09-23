using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private readonly ObservableCollection<PAccentItem> _pAccentItem = [];
    private string _pAccentLanguage = string.Empty;
    private bool _pAccentFlagged;
    private string _pAccentPrimary = string.Empty;
    private long _pAccentPrimaryId;
    private PRespelling _pAccentRespelling = PRespelling.PRespellingPlain;

    private LRequest PAccentRequestCreate(PAccentItem row)
    {
        return _pAccentRespelling.PRespellingShown
            ? new LRequestPronunciationRespelling(PEditorDraft, row.PAccentItemId, row.PAccentItemText)
            : new LRequestPronunciationIpa(PEditorDraft, row.PAccentItemId, row.PAccentItemText);
    }

    internal void PAccentAddHandle(object sender, ExecutedRoutedEventArgs e)
    {
        int position = e.Parameter is PAccentItem row ? _pAccentItem.IndexOf(row) + 2 : 1;
        PEditorRequestSend(new LRequestPronunciationAddition(PEditorDraft, string.Empty, position));
    }

    internal void PAccentRemoveHandle(object sender, ExecutedRoutedEventArgs e)
    {
        long id = e.Parameter is PAccentItem row ? row.PAccentItemId : _pAccentPrimaryId;
        if (id != 0)
        {
            PEditorRequestSend(new LRequestPronunciationRemoval(PEditorDraft, id));
        }
    }

    internal async void PAccentNotationHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PAccentItem row)
        {
            await PNotationOpen(PAccentAnchorRead(e), row.PAccentItemId);
        }
    }

    internal async void PAccentClipHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PAccentItem row)
        {
            await PClipOpen(PAccentAnchorRead(e), row.PAccentItemId);
        }
    }

    internal void PAccentPlaybackHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is not PAccentItem row)
        {
            return;
        }

        if (!_pEditorHost.PWindowDeportment.LWindowRecordingExist(row.PAccentItemAudio))
        {
            PEditorRequestSend(new LRequestPronunciationAudio(PEditorDraft, row.PAccentItemId, string.Empty, null));
            return;
        }

        _pDownloaderPlayer.Open(new Uri(row.PAccentItemAudio));
        _pDownloaderPlayer.Play();
    }

    private UIElement PAccentAnchorRead(ExecutedRoutedEventArgs e)
    {
        return e.OriginalSource as UIElement ?? PAccent;
    }

    private PAccentItem? PAccentFind(long id)
    {
        foreach (PAccentItem row in _pAccentItem)
        {
            if (row.PAccentItemId == id)
            {
                return row;
            }
        }

        return null;
    }

    private void PAccentChangeHandle(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not PAccentItem row
            || !string.Equals(e.PropertyName, nameof(PAccentItem.PAccentItemText), StringComparison.Ordinal))
        {
            return;
        }

        PEditorRequestDefer(PAccentRequestCreate(row));
    }

    private void PAccentShow(LEntryDraft draft)
    {
        string language = draft.LEntryDraftLanguage;
        bool flagged = _lEditor.LEditorFlagged;
        _pAccentLanguage = language;
        _pAccentFlagged = flagged;
        _pAccentPrimary = draft.LEntryDraftPronunciation?.LPronunciationDraftVariety ?? string.Empty;
        _pAccentPrimaryId = draft.LEntryDraftPronunciation?.LPronunciationDraftId ?? 0;
        PRespelling respelling = PRespelling.PRespellingRead(_pEditorHost.PWindowDeportment, language);
        if (respelling != _pAccentRespelling)
        {
            _pAccentRespelling = respelling;
            PAccentRowClear();
        }

        PCard.PCardRowShow(
            _pAccentItem,
            draft.LEntryDraftAccents,
            static row => row.PAccentItemId,
            static spoken => spoken.LPronunciationDraftId,
            PAccentCreate,
            PAccentUpdate);

        PAccentPrimaryShow();
        _ = PAccentFlagLoad(language, flagged);
    }

    private PAccentItem PAccentCreate(LPronunciationDraft spoken)
    {
        PAccentItem row = PAccentItem.PAccentItemCreate(
            _pEditorHost, _pAccentLanguage, _pAccentFlagged, spoken, _pAccentRespelling);
        row.PropertyChanged += PAccentChangeHandle;
        return row;
    }

    private PAccentItem PAccentUpdate(PAccentItem row, LPronunciationDraft spoken)
    {
        if (!spoken.LPronunciationDraftMatch(row.PAccentItemVariety))
        {
            row.PropertyChanged -= PAccentChangeHandle;
            return PAccentCreate(spoken);
        }

        row.PAccentItemText = _pAccentRespelling.PRespellingTextRead(spoken);

        row.PAccentItemAudio = spoken.LPronunciationDraftAudio;
        return row;
    }

    private void PAccentPrimaryShow()
    {
        PPronunciationFlag.Source = PAccentItem.PAccentFlagFind(_pAccentLanguage, _pAccentFlagged, _pAccentPrimary);
        PPronunciationLabel.Text = PPronunciationFlag.Source is null
            ? PAccentItem.PAccentLabelFormat(_pEditorHost, _pAccentPrimary)
            : string.Empty;
    }

    private async Task PAccentFlagLoad(string language, bool flagged)
    {
        if (!flagged)
        {
            return;
        }

        List<string> varieties = _pAccentItem.Select(static row => row.PAccentItemVariety).ToList();
        varieties.Add(_pAccentPrimary);

        try
        {
            await PEnsign.PEnsignVarietyLoad(
                _pEditorHost.PWindowDeportment, language, varieties.Where(static variety => variety.Length > 0));
        }
        catch (Exception)
        {
            return;
        }

        if (!string.Equals(_pAccentLanguage, language, StringComparison.Ordinal) || !_pAccentFlagged)
        {
            return;
        }

        foreach (PAccentItem row in _pAccentItem)
        {
            row.PAccentFlagUpdate(language, flagged);
        }

        PAccentPrimaryShow();
    }

    private void PAccentRowClear()
    {
        foreach (PAccentItem row in _pAccentItem)
        {
            row.PropertyChanged -= PAccentChangeHandle;
        }

        _pAccentItem.Clear();
    }

    private void PAccentClear()
    {
        PAccentRowClear();
        _pAccentLanguage = string.Empty;
        _pAccentFlagged = false;
        _pAccentPrimary = string.Empty;
        _pAccentPrimaryId = 0;
        PAccentPrimaryShow();
    }
}
