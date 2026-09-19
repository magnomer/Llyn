using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PEditor : LReceiver
{
    private readonly ObservableCollection<PNotationItem> _pNotationItem = [];
    private LForay? _pNotationForay;
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
        if (_pNotationForay is not LForay foray)
        {
            return;
        }

        long id = foray.LForayTarget;
        if (foray.LForaySchemed)
        {
            PTranscriptionItem? spelled = PTranscriptionFind(id);
            if (spelled is not null)
            {
                spelled.PTranscriptionItemText = reading.PNotationReadingPhonetic;
                PEditorChangeSave();
            }

            return;
        }

        if (foray.LForayPrimary)
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
        string text = _pNotationRespelling.PRespellingTextRead(phonetic, candidate.LCandidateRespelling);
        string opener = _pNotationRespelling.PRespellingOpener;
        string closer = _pNotationRespelling.PRespellingCloser;
        if (_pNotationForay is LForay foray)
        {
            text = foray.LForaySchemed ? phonetic : text;
            opener = foray.LForaySchemed ? string.Empty : opener;
            closer = foray.LForaySchemed ? string.Empty : closer;
        }

        if (!candidate.LCandidateRegional)
        {
            return new PNotationReading(variety, string.Empty, null, phonetic, text, opener, closer);
        }

        return new PNotationReading(
            variety,
            PAccentItem.PAccentLabelFormat(_pEditorHost, variety),
            PAccentItem.PAccentFlagFind(
                _pNotationForay?.LForayLanguage ?? string.Empty, _pNotationForay?.LForayFlagged ?? false, variety),
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
        _pNotationSearching = word.Length > 0 && _pEditorTenure is not null;
        PNotationUpdate(scheme);

        if (word.Length == 0)
        {
            return;
        }

        if (_pEditorTenure is not LTenure held)
        {
            return;
        }

        try
        {
            string language = held.LTenureLanguageRead();
            _pNotationRespelling = PRespelling.PRespellingRead(_lEngine, language);
            if (scheme.Length == 0)
            {
                if (held.LTenureFlaggedCheck())
                {
                    await PEnsign.PEnsignVarietyLoad(_lEngine, held);
                }

                if (!PNotation.IsOpen)
                {
                    return;
                }
            }

            _pNotationForay = held.LTenureTranscriptionStart(word, target, scheme, this);
        }
        catch (Exception)
        {
            _pNotationSearching = false;
            PNotationUpdate(scheme);
        }
    }

    private void PNotationCancel()
    {
        _pNotationForay?.LForayCancel();
        _pNotationForay = null;
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
        PNotationUpdate(_pNotationForay?.LForayScheme ?? string.Empty);
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
                PLocalizationCatalog.PLocalizationTextRead("Phonetician.Missing"),
                PLocalizationCatalog.PLocalizationTextRead("Phonetician.Broken"));
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
