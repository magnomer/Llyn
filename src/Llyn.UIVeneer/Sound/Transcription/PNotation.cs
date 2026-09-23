using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private readonly ObservableCollection<PNotationItem> _pNotationItem = [];
    private bool _pNotationSearching;
    private PRespelling _pNotationRespelling = PRespelling.PRespellingPlain;

    private async void PPhoneticianHandle(object sender, RoutedEventArgs e)
    {
        await PNotationOpen(PPhonetician, 0);
    }

    private void PNotationClosedHandle(object? sender, EventArgs e)
    {
        PNotationCancel();
    }

    internal void PNotationSelectorHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PNotationReading reading })
        {
            return;
        }

        PNotationApply(reading);
        PNotation.IsOpen = false;
    }

    private async Task PNotationOpen(UIElement anchor, long target)
    {
        await PNotationOpen(anchor, target, string.Empty);
    }

    private async Task PNotationOpen(UIElement anchor, long target, string scheme)
    {
        PNotation.IsOpen = false;
        PNotation.PlacementTarget = anchor;
        PNotation.IsOpen = true;
        await PNotationStart(target, scheme);
    }

    private void PNotationApply(PNotationReading reading)
    {
        if (!_lEditor.LEditorNotation.LNotationHeld)
        {
            return;
        }

        long id = _lEditor.LEditorNotation.LNotationTarget;
        if (_lEditor.LEditorNotation.LNotationSchemed)
        {
            if (PTranscriptionFind(id) is { } spelled)
            {
                PEditorRequestSend(new LRequestTranscriptionText(
                    PEditorDraft, spelled.PTranscriptionItemId, reading.PNotationReadingPhonetic));
            }

            return;
        }

        if (_lEditor.LEditorNotation.LNotationPrimary)
        {
            PEditorRequestSend(new LRequestIpa(PEditorDraft, reading.PNotationReadingPhonetic));
            id = PNotationDraftRead()?.LEntryDraftPronunciation?.LPronunciationDraftId ?? 0;
        }
        else
        {
            if (PAccentFind(id) is null)
            {
                return;
            }

            PEditorRequestSend(new LRequestPronunciationIpa(PEditorDraft, id, reading.PNotationReadingPhonetic));
        }

        PNotationVarietySend(id, reading.PNotationReadingVariety);
    }

    private void PNotationVarietySend(long pronunciationId, string variety)
    {
        if (pronunciationId == 0 || variety.Length == 0)
        {
            return;
        }

        PEditorRequestSend(new LRequestPronunciationVariety(PEditorDraft, pronunciationId, variety));
    }

    private LEntryDraft? PNotationDraftRead()
    {
        return _lEditor.LEditorDraftRead();
    }

    private PNotationReading PNotationReadingCreate(LCandidate candidate)
    {
        string variety = candidate.LCandidateVariety;
        string phonetic = candidate.LCandidatePhonetic ?? string.Empty;
        string text = _pNotationRespelling.PRespellingTextRead(phonetic, candidate.LCandidateRespelling);
        string opener = _pNotationRespelling.PRespellingOpener;
        string closer = _pNotationRespelling.PRespellingCloser;
        if (_lEditor.LEditorNotation.LNotationSchemed)
        {
            text = phonetic;
            opener = string.Empty;
            closer = string.Empty;
        }

        if (!candidate.LCandidateRegional)
        {
            return new PNotationReading(variety, string.Empty, null, phonetic, text, opener, closer);
        }

        return new PNotationReading(
            variety,
            PAccentItem.PAccentLabelFormat(_pEditorHost, variety),
            PAccentItem.PAccentFlagFind(
                _lEditor.LEditorNotation.LNotationLanguage, _lEditor.LEditorNotation.LNotationFlagged, variety),
            phonetic,
            text,
            opener,
            closer);
    }

    private async Task PNotationStart(long target, string scheme)
    {
        PNotationCancel();

        string word = PHeadword.Text?.Trim() ?? string.Empty;
        _pNotationItem.Clear();
        _pNotationSearching = word.Length > 0;
        PNotationUpdate(scheme);

        if (word.Length == 0)
        {
            return;
        }

        try
        {
            _pNotationRespelling = PRespelling.PRespellingRead(
                _pEditorHost.PWindowDeportment, _lEditor.LEditorLanguage);
            if (scheme.Length == 0)
            {
                if (_lEditor.LEditorFlagged)
                {
                    await PEnsign.PEnsignVarietyLoad(_pEditorHost.PWindowDeportment, _lEditor);
                }

                if (!PNotation.IsOpen)
                {
                    return;
                }
            }

            _lEditor.LEditorNotationStart(
                word,
                target,
                scheme,
                PObserver.PObserverCreate<LLookupStep>(this, _lEditor.LEditorNotation.LNotationStepHandle));
        }
        catch (Exception)
        {
            _pNotationSearching = false;
            PNotationUpdate(scheme);
        }
    }

    private void PNotationCancel()
    {
        _lEditor.LEditorNotation.LNotationCancel();
    }

    private PNotationItem PNotationPlace(string source, int order)
    {
        int position = 0;
        while (position < _pNotationItem.Count &&
            _pNotationItem[position].PNotationItemOrder < order)
        {
            position++;
        }

        if (position < _pNotationItem.Count && _pNotationItem[position].PNotationItemOrder == order)
        {
            return _pNotationItem[position];
        }

        PNotationItem row = new(source, order, PLocalizationCatalog.PLocalizationTextRead("Phonetician.Searching"));
        _pNotationItem.Insert(position, row);
        return row;
    }

    private void PNotationUpdate()
    {
        PNotationUpdate(_lEditor.LEditorNotation.LNotationScheme);
    }

    private void PNotationUpdate(string scheme)
    {
        bool candidates = _pNotationItem.Count > 0;

        PNotationList.Visibility = candidates ? Visibility.Visible : Visibility.Collapsed;
        PNotationProgress.Visibility = _pNotationSearching ? Visibility.Visible : Visibility.Collapsed;

        if (candidates)
        {
            PNotationNotice.Visibility = Visibility.Collapsed;
            return;
        }

        PNotationNotice.Text = PLocalizationCatalog.PLocalizationTextRead(
            _pNotationSearching ? "Phonetician.Searching"
            : scheme.Length > 0 ? "Transcription.Empty"
            : "Phonetician.Empty");
        PNotationNotice.Visibility = Visibility.Visible;
    }

    private void PNotationSourceHandle(string source, int order)
    {
        PNotationPlace(source, order);
        PNotationUpdate();
    }

    private void PNotationCandidateHandle(LCandidate candidate)
    {
        PNotationPlace(candidate.LCandidateSource, candidate.LCandidateOrder).PNotationItemShow(
            candidate,
            PNotationReadingCreate(candidate),
            PLocalizationCatalog.PLocalizationTextRead("Phonetician.Missing"),
            PLocalizationCatalog.PLocalizationTextRead("Phonetician.Broken"));
        PNotationUpdate();
    }

    private void PNotationFinishHandle()
    {
        _pNotationSearching = false;
        PNotationUpdate();
    }
}
