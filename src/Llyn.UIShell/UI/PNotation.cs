using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor : LReceiver
{
    private readonly ObservableCollection<PNotationItem> _pNotationItem = [];
    private CancellationTokenSource? _pNotationCancellation;
    private bool _pNotationSearching;
    private bool _pNotationFlagged;
    private string _pNotationLanguage = string.Empty;

    private async void PTranscriberCheckedHandle(object sender, RoutedEventArgs e)
    {
        await PNotationStart();
    }

    private void PTranscriberUncheckedHandle(object sender, RoutedEventArgs e)
    {
        PNotationCancel();
    }

    internal void PNotationSelectorHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PNotationReading reading })
        {
            return;
        }

        PNotationPrimaryApply(reading);
        PTranscriber.IsChecked = false;
    }

    internal void PNotationAllHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PNotationItem row } || !row.PNotationItemReady)
        {
            return;
        }

        PNotationReading[] readings = [.. row.PNotationItemReading];
        PNotationPrimaryApply(readings[0]);

        foreach (PNotationReading reading in readings.Skip(1))
        {
            if (!PNotationHeldCheck(reading))
            {
                PNotationReadingAdd(reading);
            }
        }

        PTranscriber.IsChecked = false;
    }

    private void PNotationPrimaryApply(PNotationReading reading)
    {
        PPronunciationField.Text = reading.PNotationReadingPhonetic;
        PEditorChangeSave();

        PNotationVarietySend(
            PNotationDraftRead()?.LEntryDraftPronunciation?.LPronunciationDraftId ?? 0,
            reading.PNotationReadingVariety);
    }

    private void PNotationReadingAdd(PNotationReading reading)
    {
        PEditorRequestSend(
            new LRequestPronunciationAddition(_pEditorDraft, reading.PNotationReadingPhonetic, int.MaxValue));

        PNotationVarietySend(
            PNotationDraftRead()?.LEntryDraftPronunciations[^1].LPronunciationDraftId ?? 0,
            reading.PNotationReadingVariety);
    }

    private void PNotationVarietySend(long pronunciationId, string variety)
    {
        if (pronunciationId == 0 || variety.Length == 0)
        {
            return;
        }

        PEditorRequestSend(new LRequestPronunciationVariety(_pEditorDraft, pronunciationId, variety));
    }

    private bool PNotationHeldCheck(PNotationReading reading)
    {
        LEntryDraft? draft = PNotationDraftRead();
        if (draft is null)
        {
            return false;
        }

        foreach (LPronunciationDraft spoken in draft.LEntryDraftPronunciations)
        {
            if (string.Equals(spoken.LPronunciationDraftVariety, reading.PNotationReadingVariety, StringComparison.Ordinal)
                && string.Equals(spoken.LPronunciationDraftIpa, reading.PNotationReadingPhonetic, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private LEntryDraft? PNotationDraftRead()
    {
        if (_pEditorHalted || _pEditorDraft == 0)
        {
            return null;
        }

        return _lEngine.LEngineDraftRead(_pEditorDraft)?.LDraftContent;
    }

    private PNotationReading PNotationReadingCreate(LCandidate candidate)
    {
        string variety = candidate.LCandidateVariety;
        if (variety.Length == 0)
        {
            return new PNotationReading(variety, string.Empty, null, candidate.LCandidatePhonetic ?? string.Empty);
        }

        return new PNotationReading(
            variety,
            PAccentItem.PAccentLabelFormat(_pEditorHost, variety),
            PAccentItem.PAccentFlagFind(_pNotationLanguage, _pNotationFlagged, variety),
            candidate.LCandidatePhonetic ?? string.Empty);
    }

    private async Task PNotationStart()
    {
        PNotationCancel();

        string word = PHeadword.Text?.Trim() ?? string.Empty;
        _pNotationItem.Clear();
        _pNotationSearching = word.Length > 0;
        PNotationUpdate();

        if (word.Length == 0)
        {
            return;
        }

        _pNotationCancellation = new CancellationTokenSource();
        CancellationToken cancellation = _pNotationCancellation.Token;

        try
        {
            _pNotationLanguage = _pSpeakerChoice;
            _pNotationFlagged = _lEngine.LEngineFlaggedCheck(_pNotationLanguage);
            if (_pNotationFlagged)
            {
                await PEnsign.PEnsignVarietyLoad(
                    _lEngine,
                    _pNotationLanguage,
                    _lEngine.LEngineVarietyRead(_pNotationLanguage).Select(variety => variety.LVarietyName));
                cancellation.ThrowIfCancellationRequested();
            }

            await _lEngine.LEnginePronunciationFind(
                _pEditorDraft, word, _pNotationLanguage, this, cancellation);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception)
        {
            _pNotationSearching = false;
            PNotationUpdate();
        }
    }

    private void PNotationCancel()
    {
        _pNotationCancellation?.Cancel();
        _pNotationCancellation?.Dispose();
        _pNotationCancellation = null;
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

        PNotationItem row = new(source, order, _pEditorHost.PLocalizationTextRead("Transcriber.Searching"));
        _pNotationItem.Insert(position, row);
        return row;
    }

    private void PNotationUpdate()
    {
        bool candidates = _pNotationItem.Count > 0;

        PNotationList.Visibility = candidates ? Visibility.Visible : Visibility.Collapsed;
        PNotationProgress.Visibility = _pNotationSearching ? Visibility.Visible : Visibility.Collapsed;

        if (candidates)
        {
            PNotationNotice.Visibility = Visibility.Collapsed;
            return;
        }

        PNotationNotice.Text = _pEditorHost.PLocalizationTextRead(
            _pNotationSearching ? "Transcriber.Searching" : "Transcriber.Empty");
        PNotationNotice.Visibility = Visibility.Visible;
    }

    void LReceiver.LReceiverSourceStart(string source, int order)
    {
        Dispatcher.BeginInvoke(() =>
        {
            PNotationPlace(source, order);
            PNotationUpdate();
        });
    }

    void LReceiver.LReceiverCandidateAdd(LCandidate candidate)
    {
        Dispatcher.BeginInvoke(() =>
        {
            PNotationPlace(candidate.LCandidateSource, candidate.LCandidateOrder).PNotationItemShow(
                candidate,
                PNotationReadingCreate(candidate),
                _pEditorHost.PLocalizationTextRead("Transcriber.Missing"),
                _pEditorHost.PLocalizationTextRead("Transcriber.Broken"));
            PNotationUpdate();
        });
    }

    void LReceiver.LReceiverLookupFinish()
    {
        Dispatcher.BeginInvoke(() =>
        {
            _pNotationSearching = false;
            PNotationUpdate();
        });
    }
}
