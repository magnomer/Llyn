using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private readonly ObservableCollection<LAccentItem> _pDisplayAccent = [];
    private string _pDisplayAccentLanguage = string.Empty;
    private bool _pDisplayAccentFlagged;
    private string _pDisplayAccentPrimary = string.Empty;

    private void PDisplayAccentShow(LEntryDraft draft)
    {
        string language = draft.LEntryDraftLanguage;
        bool flagged = _lLectern.LLecternFlaggedCheck(draft);
        _pDisplayAccentLanguage = language;
        _pDisplayAccentFlagged = flagged;
        _pDisplayAccentPrimary = draft.LEntryDraftPronunciation?.LPronunciationDraftVariety ?? string.Empty;
        LRespellingMark respelling = LRespellingMark.LRespellingMarkRead(_pDisplayHost.PWindowDeportment, language);

        _pDisplayAccent.Clear();
        foreach (LPronunciationDraft spoken in draft.LEntryDraftAccents)
        {
            if (spoken.LPronunciationDraftNotated)
            {
                _pDisplayAccent.Add(
                    LAccentItem.LAccentItemCreate(language, flagged, spoken, respelling, PEnsign.PEnsignFind));
            }
        }

        PDisplayPrimaryShow();
        _ = PDisplayFlagLoad(language, flagged);
    }

    internal void PDisplayPlaybackHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is not LAccentItem row)
        {
            return;
        }

        if (!_pDisplayHost.PWindowDeportment.LWindowRecordingExist(row.LAccentItemAudio))
        {
            return;
        }

        _pDisplayPlayer.Open(new Uri(row.LAccentItemAudio));
        _pDisplayPlayer.Play();
    }

    private void PDisplayPrimaryShow()
    {
        PDisplayPronunciationFlag.Source = LAccentItem.LAccentFlagFind(
            _pDisplayAccentLanguage, _pDisplayAccentFlagged, _pDisplayAccentPrimary, PEnsign.PEnsignFind);
        PDisplayPronunciationLabel.Text = PDisplayPronunciationFlag.Source is null
            ? LAccentItem.LAccentLabelFormat(_pDisplayAccentPrimary)
            : string.Empty;
    }

    private async Task PDisplayFlagLoad(string language, bool flagged)
    {
        if (!flagged)
        {
            return;
        }

        List<string> varieties = _pDisplayAccent.Select(static row => row.LAccentItemVariety).ToList();
        varieties.Add(_pDisplayAccentPrimary);

        try
        {
            await PEnsign.PEnsignVarietyLoad(
                _pDisplayHost.PWindowDeportment, language, varieties.Where(static variety => variety.Length > 0));
        }
        catch (Exception)
        {
            return;
        }

        if (!string.Equals(_pDisplayAccentLanguage, language, StringComparison.Ordinal) || !_pDisplayAccentFlagged)
        {
            return;
        }

        foreach (LAccentItem row in _pDisplayAccent)
        {
            row.LAccentFlagUpdate(language, flagged, PEnsign.PEnsignFind);
        }

        PDisplayPrimaryShow();
    }

    private void PDisplayAccentClear()
    {
        _pDisplayAccent.Clear();
        _pDisplayAccentLanguage = string.Empty;
        _pDisplayAccentFlagged = false;
        _pDisplayAccentPrimary = string.Empty;
        PDisplayPrimaryShow();
    }
}
