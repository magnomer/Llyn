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
    private long _pNotationTarget;
    private string _pNotationScheme = string.Empty;

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
        _pNotationTarget = target;
        _pNotationScheme = scheme;
        PNotation.PlacementTarget = anchor;
        PNotation.IsOpen = true;
        await PNotationStart();
    }

    private void PNotationApply(PNotationReading reading)
    {
        long id = _pNotationTarget;
        if (_pNotationScheme.Length > 0)
        {
            PTranscriptionItem? spelled = PTranscriptionFind(id);
            if (spelled is not null)
            {
                spelled.PTranscriptionItemText = reading.PNotationReadingPhonetic;
                PEditorChangeSave();
            }

            return;
        }

        if (id == 0)
        {
            PPronunciationField.Text = reading.PNotationReadingPhonetic;
            PEditorChangeSave();
            id = PNotationDraftRead()?.LEntryDraftPronunciation?.LPronunciationDraftId ?? 0;
        }
        else
        {
            PAccentItem? row = PAccentFind(id);
            if (row is null)
            {
                return;
            }

            row.PAccentItemIpa = reading.PNotationReadingPhonetic;
            PEditorChangeSave();
        }

        PNotationVarietySend(id, reading.PNotationReadingVariety);
    }

    private void PNotationVarietySend(long pronunciationId, string variety)
    {
        if (pronunciationId == 0 || variety.Length == 0)
        {
            return;
        }

        PEditorRequestSend(new LRequestPronunciationVariety(_pEditorDraft, pronunciationId, variety));
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
        bool bracketed = _pNotationScheme.Length == 0;
        if (variety.Length == 0)
        {
            return new PNotationReading(
                variety, string.Empty, null, candidate.LCandidatePhonetic ?? string.Empty, bracketed);
        }

        return new PNotationReading(
            variety,
            PAccentItem.PAccentLabelFormat(_pEditorHost, variety),
            PAccentItem.PAccentFlagFind(_pNotationLanguage, _pNotationFlagged, variety),
            candidate.LCandidatePhonetic ?? string.Empty,
            bracketed);
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
            if (_pNotationFlagged && _pNotationScheme.Length == 0)
            {
                await PEnsign.PEnsignVarietyLoad(
                    _lEngine,
                    _pNotationLanguage,
                    _lEngine.LEngineVarietyRead(_pNotationLanguage).Select(variety => variety.LVarietyName));
                cancellation.ThrowIfCancellationRequested();
            }

            if (_pNotationScheme.Length > 0)
            {
                await _lEngine.LEngineTranscriptionFind(
                    _pEditorDraft, word, _pNotationLanguage, _pNotationScheme, this, cancellation);
            }
            else
            {
                await _lEngine.LEnginePronunciationFind(
                    _pEditorDraft, word, _pNotationLanguage, this, cancellation);
            }
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

        PNotationItem row = new(source, order, _pEditorHost.PLocalizationTextRead("Phonetician.Searching"));
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
            _pNotationSearching ? "Phonetician.Searching"
            : _pNotationScheme.Length > 0 ? "Transcription.Empty"
            : "Phonetician.Empty");
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
                _pEditorHost.PLocalizationTextRead("Phonetician.Missing"),
                _pEditorHost.PLocalizationTextRead("Phonetician.Broken"));
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
