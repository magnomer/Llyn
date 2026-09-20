using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private readonly ObservableCollection<PAccentItem> _pDisplayAccent = [];
    private string _pDisplayAccentLanguage = string.Empty;
    private bool _pDisplayAccentFlagged;
    private string _pDisplayAccentPrimary = string.Empty;

    private void PDisplayAccentShow(LEntryDraft draft)
    {
        string language = draft.LEntryDraftLanguage;
        bool flagged = _lDisplay.LDisplayFlaggedCheck(draft);
        _pDisplayAccentLanguage = language;
        _pDisplayAccentFlagged = flagged;
        _pDisplayAccentPrimary = draft.LEntryDraftPronunciation?.LPronunciationDraftVariety ?? string.Empty;
        PRespelling respelling = PRespelling.PRespellingRead(_pDisplayHost.PWindowDeportment, language);

        _pDisplayAccent.Clear();
        foreach (LPronunciationDraft spoken in draft.LEntryDraftAccents)
        {
            if (spoken.LPronunciationDraftNotated)
            {
                _pDisplayAccent.Add(
                    PAccentItem.PAccentItemCreate(_pDisplayHost, language, flagged, spoken, respelling));
            }
        }

        PDisplayPrimaryShow();
        _ = PDisplayFlagLoad(language, flagged);
    }

    internal void PDisplayPlaybackHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is not PAccentItem row)
        {
            return;
        }

        if (!_pDisplayHost.PWindowDeportment.LWindowRecordingExist(row.PAccentItemAudio))
        {
            return;
        }

        _pDisplayPlayer.Open(new Uri(row.PAccentItemAudio));
        _pDisplayPlayer.Play();
    }

    private void PDisplayPrimaryShow()
    {
        PDisplayPronunciationFlag.Source = PAccentItem.PAccentFlagFind(
            _pDisplayAccentLanguage, _pDisplayAccentFlagged, _pDisplayAccentPrimary);
        PDisplayPronunciationLabel.Text = PDisplayPronunciationFlag.Source is null
            ? PAccentItem.PAccentLabelFormat(_pDisplayHost, _pDisplayAccentPrimary)
            : string.Empty;
    }

    private async Task PDisplayFlagLoad(string language, bool flagged)
    {
        if (!flagged)
        {
            return;
        }

        List<string> varieties = _pDisplayAccent.Select(static row => row.PAccentItemVariety).ToList();
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

        foreach (PAccentItem row in _pDisplayAccent)
        {
            row.PAccentFlagUpdate(language, flagged);
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
