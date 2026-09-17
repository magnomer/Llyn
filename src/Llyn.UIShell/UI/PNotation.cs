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
    private PRespelling _pNotationRespelling = PRespelling.PRespellingPlain;
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
        return _pEditorTenure?.LTenureRead()?.LDraftContent;
    }

    private PNotationReading PNotationReadingCreate(LCandidate candidate)
    {
        string variety = candidate.LCandidateVariety;
        string phonetic = candidate.LCandidatePhonetic ?? string.Empty;
        string text = _pNotationScheme.Length == 0
            ? _pNotationRespelling.PRespellingTextRead(phonetic, candidate.LCandidateRespelling)
            : phonetic;
        string opener = _pNotationScheme.Length == 0 ? _pNotationRespelling.PRespellingOpener : string.Empty;
        string closer = _pNotationScheme.Length == 0 ? _pNotationRespelling.PRespellingCloser : string.Empty;
        if (variety.Length == 0)
        {
            return new PNotationReading(variety, string.Empty, null, phonetic, text, opener, closer);
        }

        return new PNotationReading(
            variety,
            PAccentItem.PAccentLabelFormat(_pEditorHost, variety),
            PAccentItem.PAccentFlagFind(_pNotationLanguage, _pNotationFlagged, variety),
            phonetic,
            text,
            opener,
            closer);
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
            _pNotationRespelling = PRespelling.PRespellingRead(_lEngine, _pNotationLanguage);
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
                    PEditorDraft, word, _pNotationLanguage, _pNotationScheme, this, cancellation);
            }
            else
            {
                await _lEngine.LEnginePronunciationFind(
                    PEditorDraft, word, _pNotationLanguage, this, cancellation);
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
