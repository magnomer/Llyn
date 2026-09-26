using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly ObservableCollection<LAccentItem> _pAccentItem = [];
    private string _pAccentLanguage = string.Empty;
    private bool _pAccentFlagged;
    private string _pAccentPrimary = string.Empty;
    private LRespellingMark _pAccentRespelling = LRespellingMark.LRespellingMarkPlain;

    private ItemsControl PAccent => (ItemsControl)FindName(nameof(PAccent));

    private Image PPronunciationFlag => (Image)FindName(nameof(PPronunciationFlag));

    private TextBlock PPronunciationLabel => (TextBlock)FindName(nameof(PPronunciationLabel));

    private void PAccentAttach()
    {
        PAccent.ItemsSource = _pAccentItem;
        PLookItem.PLookItemAttach(PAccent, LAccentItem.LAccentItemApply);
        PField.PFieldCellAttach(PAccent);
        PAccentControl.PAccentControlAttach(PAccent);
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PAccentCommand.PAccentCommandAddition, PAccentAddHandle));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PAccentCommand.PAccentCommandRemoval, PAccentRemoveHandle));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PAccentCommand.PAccentCommandNotation, PAccentNotationHandle));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PAccentCommand.PAccentCommandClip, PAccentClipHandle));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PAccentCommand.PAccentCommandPlayback, PAccentPlaybackHandle));
    }

    private LRequest PAccentRequestCreate(LAccentItem row)
    {
        return _pAccentRespelling.LRespellingMarkShown
            ? new LRequestPronunciationRespelling(PEditorDraft, row.LAccentItemId, row.LAccentItemText)
            : new LRequestPronunciationIpa(PEditorDraft, row.LAccentItemId, row.LAccentItemText);
    }

    internal void PAccentAddHandle(object sender, ExecutedRoutedEventArgs e)
    {
        int position = e.Parameter is LAccentItem row ? _pAccentItem.IndexOf(row) + 2 : 1;
        PEditorRequestSend(new LRequestPronunciationAddition(PEditorDraft, string.Empty, position));
    }

    internal void PAccentRemoveHandle(object sender, ExecutedRoutedEventArgs e)
    {
        PEditorRequestSend(
            new LRequestPronunciationRemoval(PEditorDraft, (e.Parameter as LAccentItem)?.LAccentItemId ?? 0));
    }

    internal async void PAccentNotationHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is LAccentItem row)
        {
            await PNotationOpen(PAccentAnchorRead(e), row.LAccentItemId);
        }
    }

    internal async void PAccentClipHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is LAccentItem row)
        {
            await PClipOpen(PAccentAnchorRead(e), row.LAccentItemId);
        }
    }

    internal void PAccentPlaybackHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is not LAccentItem row)
        {
            return;
        }

        if (!_pEditorHost.PWindowDeportment.LWindowRecordingExist(row.LAccentItemAudio))
        {
            PEditorRequestSend(new LRequestPronunciationAudio(PEditorDraft, row.LAccentItemId, string.Empty, null));
            return;
        }

        _pDownloaderPlayer.Open(new Uri(row.LAccentItemAudio));
        _pDownloaderPlayer.Play();
    }

    private UIElement PAccentAnchorRead(ExecutedRoutedEventArgs e)
    {
        return e.OriginalSource as UIElement ?? PAccent;
    }

    private LAccentItem? PAccentFind(long id)
    {
        foreach (LAccentItem row in _pAccentItem)
        {
            if (row.LAccentItemId == id)
            {
                return row;
            }
        }

        return null;
    }

    private void PAccentChangeHandle(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not LAccentItem row
            || !string.Equals(e.PropertyName, nameof(LAccentItem.LAccentItemText), StringComparison.Ordinal))
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
        LRespellingMark respelling = LRespellingMark.LRespellingMarkRead(_pEditorHost.PWindowDeportment, language);
        if (respelling != _pAccentRespelling)
        {
            _pAccentRespelling = respelling;
            PAccentRowClear();
        }

        PCard.PCardRowShow(
            _pAccentItem,
            draft.LEntryDraftAccents,
            static row => row.LAccentItemId,
            static spoken => spoken.LPronunciationDraftId,
            PAccentCreate,
            PAccentUpdate);

        PAccentPrimaryShow();
        _ = PAccentFlagLoad(language, flagged);
    }

    private LAccentItem PAccentCreate(LPronunciationDraft spoken)
    {
        LAccentItem row = LAccentItem.LAccentItemCreate(
            _pAccentLanguage, _pAccentFlagged, spoken, _pAccentRespelling);
        row.PropertyChanged += PAccentChangeHandle;
        return row;
    }

    private LAccentItem PAccentUpdate(LAccentItem row, LPronunciationDraft spoken)
    {
        if (!spoken.LPronunciationDraftMatch(row.LAccentItemVariety))
        {
            row.PropertyChanged -= PAccentChangeHandle;
            return PAccentCreate(spoken);
        }

        row.LAccentItemText = _pAccentRespelling.LRespellingMarkResolve(spoken);

        row.LAccentItemAudio = spoken.LPronunciationDraftAudio;
        return row;
    }

    private void PAccentPrimaryShow()
    {
        PPronunciationFlag.Source = LAccentItem.LAccentFlagFind(
            _pAccentLanguage, _pAccentFlagged, _pAccentPrimary);
        PPronunciationLabel.Text = PPronunciationFlag.Source is null
            ? LAccentItem.LAccentLabelFormat(_pAccentPrimary)
            : string.Empty;
    }

    private async Task PAccentFlagLoad(string language, bool flagged)
    {
        if (!flagged)
        {
            return;
        }

        List<string> varieties = _pAccentItem.Select(static row => row.LAccentItemVariety).ToList();
        varieties.Add(_pAccentPrimary);

        try
        {
            await LEnsignImage.LEnsignVarietyLoad(
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

        foreach (LAccentItem row in _pAccentItem)
        {
            row.LAccentFlagUpdate(language, flagged);
        }

        PAccentPrimaryShow();
    }

    private void PAccentRowClear()
    {
        foreach (LAccentItem row in _pAccentItem)
        {
            row.PropertyChanged -= PAccentChangeHandle;
        }

        _pAccentItem.Clear();
    }
}
