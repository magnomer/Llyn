using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Application;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly ObservableCollection<PNotationItem> _pNotationItem = [];
    private readonly PNotationTemplate _pNotationTemplate;
    private bool _pNotationSearching;

    private Popup PNotation => (Popup)FindName(nameof(PNotation));

    private Border PNotationProgress => (Border)FindName(nameof(PNotationProgress));

    private TextBlock PNotationNotice => (TextBlock)FindName(nameof(PNotationNotice));

    private ItemsControl PNotationList => (ItemsControl)FindName(nameof(PNotationList));

    private Button PPhonetician => (Button)FindName(nameof(PPhonetician));

    private void PNotationAttach()
    {
        PNotationList.ItemsSource = _pNotationItem;
        QLookItem.QLookItemAttach(
            PNotationList,
            (container, item, _) => PNotationItem.PNotationItemApply(
                container, item, _pNotationTemplate.PNotationSelectorHandle));
        PNotation.Closed += PNotationClosedHandle;
        PPhonetician.Click += PPhoneticianHandle;
        PPhonetician.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("lookup", 24));
    }

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
        if (!_lEditor.LEditorStudio.CEditorDesk.CDeskErrand.CErrandTranscriptionHeld)
        {
            return;
        }

        long id = _lEditor.LEditorStudio.CEditorDesk.CDeskErrand.CErrandTranscriptionTarget;
        if (_lEditor.LEditorStudio.CEditorDesk.CDeskErrand.CErrandTranscriptionSchemed)
        {
            if (PTranscriptionFind(id) is { } spelled)
            {
                PEditorRequestSend(new LRequestTranscriptionText(
                    PEditorDraft, spelled.LTranscriptionItemId, reading.PNotationReadingPhonetic));
            }

            return;
        }

        if (_lEditor.LEditorStudio.CEditorDesk.CDeskErrand.CErrandTranscriptionPrimary)
        {
            PEditorRequestSend(new LRequestIpa(PEditorDraft, reading.PNotationReadingPhonetic));
        }
        else
        {
            if (PAccentFind(id) is null)
            {
                return;
            }

            PEditorRequestSend(new LRequestPronunciationIpa(PEditorDraft, id, reading.PNotationReadingPhonetic));
        }

        _lEditor.LEditorStudio.CEditorVarietySet(
            _lEditor.LEditorStudio.CEditorDesk.CDeskErrand.CErrandTranscriptionPrimary,
            id,
            reading.PNotationReadingVariety);
    }

    private PNotationReading PNotationReadingCreate(CCandidate candidate)
    {
        return PNotationReadingCreate(
            candidate,
            _pEditorHost.PWindowAtelier.CAtelierRespelling.CRespellingMarkRead(
                _lEditor.LEditorLanguage, _lEditor.LEditorStudio.CEditorDesk.CDeskErrand.CErrandTranscriptionSchemed));
    }

    private PNotationReading PNotationReadingCreate(CCandidate candidate, CRespellingMark respelling)
    {
        string variety = candidate.CCandidateVariety;
        string phonetic = candidate.CCandidatePhonetic ?? string.Empty;
        string text = CRespelling.CRespellingResolve(
            respelling, candidate.CCandidatePhonetic ?? string.Empty, candidate.CCandidateRespelling);
        string opener = respelling.CRespellingMarkOpener;
        string closer = respelling.CRespellingMarkCloser;

        if (!candidate.CCandidateRegional)
        {
            return new PNotationReading(variety, string.Empty, null, phonetic, text, opener, closer);
        }

        CVariety regional = CSounding.CSoundingVarietyRead(
            _lEditor.LEditorStudio.CEditorDesk.CDeskErrand.CErrandTranscriptionLanguage, variety);
        return new PNotationReading(
            variety,
            QAccentItem.QAccentLabelRefine(regional),
            QAccentItem.QAccentEnsignRefine(
                regional, _lEditor.LEditorStudio.CEditorDesk.CDeskErrand.CErrandTranscriptionFlagged),
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
            if (scheme.Length == 0)
            {
                if (_lEditor.LEditorFlagged)
                {
                    await LEnsignImage.LEnsignVarietyLoad(
                        _pEditorHost.PWindowAtelier, _lEditor.LEditorLanguage, _lEditor.LEditorVarietyNames);
                }

                if (!PNotation.IsOpen)
                {
                    return;
                }
            }

            _lEditor.LEditorStudio.CEditorDesk.CDeskErrand.CErrandTranscriptionStart(
                word,
                target,
                scheme,
                LObserver.LObserverCreate<CLookupStep>(
                    this, _lEditor.LEditorStudio.CEditorDesk.CDeskErrand.CErrandLookupResonate));
        }
        catch (Exception)
        {
            _pNotationSearching = false;
            PNotationUpdate(scheme);
        }
    }

    private void PNotationCancel()
    {
        _lEditor.LEditorStudio.CEditorDesk.CDeskErrand.CErrandCancel();
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

        PNotationItem row = new(source, order, QLocalizationCatalog.QLocalizationTextRead("Phonetician.Searching"));
        _pNotationItem.Insert(position, row);
        return row;
    }

    private void PNotationUpdate()
    {
        PNotationUpdate(_lEditor.LEditorStudio.CEditorDesk.CDeskErrand.CErrandTranscriptionScheme);
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

        PNotationNotice.Text = QLocalizationCatalog.QLocalizationTextRead(
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

    private void PNotationCandidateHandle(CCandidate candidate)
    {
        PNotationPlace(candidate.CCandidateSource, candidate.CCandidateOrder).PNotationItemShow(
            candidate,
            PNotationReadingCreate(candidate),
            QLocalizationCatalog.QLocalizationTextRead("Phonetician.Missing"),
            QLocalizationCatalog.QLocalizationTextRead("Phonetician.Broken"));
        PNotationUpdate();
    }

    private void PNotationFinishHandle()
    {
        _pNotationSearching = false;
        PNotationUpdate();
    }
}
