using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PDisplay : UserControl
{
    private readonly ObservableCollection<PUsageItem> _pDisplayIncoming = [];

    private PWindow _pDisplayHost = null!;

    private LDisplay _lDisplay = null!;

    public PDisplay()
    {
        InitializeComponent();
        Resources.MergedDictionaries.Add(new PDisplayCompass(this));
        PDisplayIncoming.ItemsSource = _pDisplayIncoming;
        PDisplayAccent.ItemsSource = _pDisplayAccent;
        PDisplayTranscription.ItemsSource = _pDisplayTranscription;
        PDisplayReflex.ItemsSource = _pDisplayReflex;
        PDisplayGlyph.ItemsSource = _pDisplayGlyph;

        PDisplaySwath.PSwathAttach(PDisplayContents);
        PDisplayPlaybackAttach();
        AddHandler(PMention.PMentionClickEvent, new EventHandler<PMentionArgument>(PDisplayMentionHandle));

        CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandEntry, PDisplayEtymologyHandle));

        PCompassAttach();
    }

    internal void PDisplayAttach(PWindow host, LDisplay display)
    {
        _pDisplayHost = host;
        _lDisplay = display;
        PDisplayGrasp.PGraspLimit = display.LDisplayGraspStep;
        PMedia.PMediaAttach(this, host.PWindowDeportment);

        PVolumeLoad();
    }

    private static IReadOnlyList<string> PDisplaySpeechShow(IReadOnlyList<LSpeechDraft> speeches)
    {
        List<string> named = new(speeches.Count);
        foreach (LSpeechDraft speech in speeches)
        {
            if (speech.LSpeechDraftNamed)
            {
                named.Add(speech.LSpeechDraftName);
            }
        }

        return named;
    }

    internal void PDisplayShow(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (_lDisplay.LDisplayChosen is not long id)
        {
            PDisplayClear();
            return;
        }

        PDisplaySwath.PSwathClear();
        PDisplayFavoriteShow(id);
        PDisplayGraspShow(id);

        _pDisplayRecording = _pDisplayHost.PWindowDeportment.LWindowRecordingExist(draft.LEntryDraftAudio)
            ? draft.LEntryDraftAudio
            : null;
        PPlaybackAction.Visibility = _pDisplayRecording is null ? Visibility.Collapsed : Visibility.Visible;
        PVolumeLoad();

        PDisplayHeadword.Text = draft.LEntryDraftHeadword;
        PDisplayLanguageShow(draft.LEntryDraftLanguage);
        PDisplayReflexStart(id);
        PDisplayReflexShow(id, draft);
        PReflexPendingShow(id);
        PRespelling respelling = PRespelling.PRespellingRead(
            _pDisplayHost.PWindowDeportment, draft.LEntryDraftLanguage);
        PDisplayPronunciation.Text = draft.LEntryDraftPronunciation is LPronunciationDraft primary
            ? respelling.PRespellingTextRead(primary)
            : string.Empty;
        PDisplayPronunciationOpener.Text = respelling.PRespellingOpener;
        PDisplayPronunciationCloser.Text = respelling.PRespellingCloser;
        PDisplayContour.PContourTonal = _lDisplay.LDisplayTonalCheck(draft.LEntryDraftLanguage);
        PDisplayPronunciationSurface.Visibility = PDisplayPronunciation.Text.Length == 0
            ? Visibility.Collapsed
            : Visibility.Visible;
        PDisplayPronunciationLead.SharedSizeGroup = PDisplayPronunciation.Text.Length == 0 ? null : "PReadingLabel";
        PDisplayAccentShow(draft);
        PDisplayGlyphShow(draft);
        PDisplayTranscriptionShow(draft);
        PDisplayPlaybackShow();

        PDisplayFrameShow(draft.LEntryDraftLanguage);
        PDisplayExampleShow(draft.LEntryDraftLanguage);
        PDisplayCitationShow();
        PDisplayTranslationShow(draft);
        PDisplayIncomingShow(id);

        PDisplaySpeech.ItemsSource = PDisplaySpeechShow(draft.LEntryDraftSpeeches);
        PDisplaySpeechSection.Visibility = draft.LEntryDraftMarked ? Visibility.Visible : Visibility.Collapsed;
        PDisplayFrequencyShow(id);
        PDisplayParadigmShow(id);
        PDisplayScriptStart(id);
        PDisplayScriptShow(id, draft.LEntryDraftLanguage);
        PDisplayFanqieStart(id);
        PDisplayFanqieShow(id, draft.LEntryDraftLanguage);
        PDisplayMeaning.ItemsSource = draft.LEntryDraftMeanings;
        PDisplayCollocation.ItemsSource = draft.LEntryDraftCollocations;
        PDisplayMeaningSection.Visibility = draft.LEntryDraftDefined ? Visibility.Visible : Visibility.Collapsed;
        PDisplayCollocationSection.Visibility = draft.LEntryDraftCollocated
            ? Visibility.Visible
            : Visibility.Collapsed;

        PDisplayEtymologyShow(draft);
        PMarkdown.PMarkdownShow(PDisplayNote, draft.LEntryDraftNote, _pDisplayHost.PWindowDeportment);
        PDisplayNoteSection.Visibility = draft.LEntryDraftNoted ? Visibility.Visible : Visibility.Collapsed;

        PDisplayStampShow(id);

        PDisplayEmpty.Visibility = Visibility.Collapsed;
        PDisplayContents.Visibility = Visibility.Visible;

        PCompassUpdate();
    }

    private void PDisplayStampShow(long id)
    {
        LEntry? entry;
        try
        {
            entry = _lDisplay.LDisplayEntryRead(id);
        }
        catch (Exception)
        {
            entry = null;
        }

        PDisplayStampAdded.Text = PDisplayStampFormat(entry?.LEntryAddedUtc);
        PDisplayStampUpdated.Text = PDisplayStampFormat(entry?.LEntryUpdatedUtc);
        PDisplayStampSection.Visibility = entry is null
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private void PDisplayFrequencyShow(long id)
    {
        IReadOnlyList<LFrequency> frequency;
        try
        {
            frequency = _lDisplay.LDisplayFrequencyRead(id);
        }
        catch (Exception)
        {
            frequency = [];
        }

        PFrequencyLabel.PFrequencyChipShow(
            PDisplayFrequencySection, PDisplayFrequencyChip, PDisplayFrequency, PDisplayFrequencyBand, frequency);
    }

    private static string PDisplayStampFormat(string? utc)
    {
        if (utc is null
            || !DateTimeOffset.TryParse(
                utc, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset parsed))
        {
            return string.Empty;
        }

        return parsed.ToLocalTime().ToString("g", CultureInfo.CurrentCulture);
    }

    internal void PDisplayClear()
    {
        PDisplaySwath.PSwathClear();
        PDisplayFavorite.IsChecked = false;
        PDisplayGrasp.PGraspStep = 0;
        PDisplayGraspLabel.Text = string.Empty;
        _pDisplayRecording = null;
        _pDisplayPlayer.Stop();
        PDisplayLanguage.Text = string.Empty;
        PDisplayLanguageFlag.Source = null;
        PPlayback.Visibility = Visibility.Collapsed;
        PPlaybackAction.Visibility = Visibility.Collapsed;
        PDisplayPronunciationSurface.Visibility = Visibility.Collapsed;
        PDisplayPronunciationLead.SharedSizeGroup = null;
        PDisplayAccentClear();
        _pDisplayTranscription.Clear();
        _pDisplayReflex.Clear();
        PDisplayReflexFold.Visibility = Visibility.Collapsed;
        PDisplayReflexLoading.Visibility = Visibility.Collapsed;
        PDisplayGlyphClear();
        PDisplaySpeech.ItemsSource = null;
        PDisplaySpeechSection.Visibility = Visibility.Collapsed;
        PDisplayFrequency.Text = string.Empty;
        PDisplayFrequencyChip.ToolTip = null;
        PDisplayFrequencySection.Visibility = Visibility.Collapsed;
        PDisplayParadigm.PParadigmItems = null;
        PDisplayScriptClear();
        PDisplayFanqieClear();
        _pDisplayIncoming.Clear();
        PDisplayIncomingSection.Visibility = Visibility.Collapsed;
        PDisplayTranslationRead().PLinkConverterClear();
        PDisplayCitationRead().PCitationConverterClear();
        PDisplayMeaning.ItemsSource = null;
        PDisplayCollocation.ItemsSource = null;
        PDisplayMeaningSection.Visibility = Visibility.Collapsed;
        PDisplayCollocationSection.Visibility = Visibility.Collapsed;
        PDisplayNoteSection.Visibility = Visibility.Collapsed;
        PDisplayStampSection.Visibility = Visibility.Collapsed;
        PDisplayContents.Visibility = Visibility.Collapsed;
        PDisplayEmpty.Visibility = Visibility.Visible;

        PCompassClear();
    }

    internal void PDisplayClose()
    {
        _pDisplayPlayer.Close();
    }

    private void PDisplayTranslationShow(LEntryDraft draft)
    {
        PLinkConverter converter = PDisplayTranslationRead();
        try
        {
            converter.PLinkConverterShow(_lDisplay.LDisplayTargetRead(draft));
        }
        catch (Exception)
        {
            converter.PLinkConverterClear();
        }

        PDisplayCardUpdate();
    }

    private void PDisplayCardUpdate()
    {
        System.Collections.IEnumerable? meanings = PDisplayMeaning.ItemsSource;
        System.Collections.IEnumerable? collocations = PDisplayCollocation.ItemsSource;

        PDisplayMeaning.ItemsSource = null;
        PDisplayCollocation.ItemsSource = null;
        PDisplayMeaning.ItemsSource = meanings;
        PDisplayCollocation.ItemsSource = collocations;
    }

    private PLinkConverter PDisplayTranslationRead()
    {
        return (PLinkConverter)Resources["Display.Card.Translation"];
    }

    private void PDisplayFrameShow(string language)
    {
        LSentenceOrder order;
        try
        {
            order = _pDisplayHost.PWindowDeportment.LWindowOrderRead(language);
        }
        catch (Exception)
        {
            order = LSentenceOrder.LSentenceOrderDefault;
        }

        ((PSentenceConverter)Resources["Display.Card.Frame"]).PSentenceConverterApply(order);
    }

    private void PDisplayExampleShow(string language)
    {
        PFont.PFontExampleApply(Resources, _pDisplayHost.PWindowDeportment, language);
    }

    private void PDisplayCardHandle(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not FrameworkElement chip)
        {
            return;
        }

        switch (chip.DataContext)
        {
            case LSituationDraft { LSituationDraftStored: true } situation:
                e.Handled = true;
                _pDisplayHost.PWindowSituationShow(situation.LSituationDraftId);
                break;
            case LRegisterDraft { LRegisterDraftStored: true } register:
                e.Handled = true;
                _pDisplayHost.PWindowRegisterShow(register.LRegisterDraftId);
                break;
            case PLinkChip link when link.PLinkChipId != 0:
                e.Handled = true;
                _pDisplayHost.PWindowEntryShow(link.PLinkChipId);
                break;
            case LTagDraft { LTagDraftStored: true } tag:
                e.Handled = true;
                _pDisplayHost.PWindowTagShow(tag.LTagDraftId);
                break;
        }
    }

    private async void PDisplayLanguageShow(string language)
    {
        PDisplayLanguage.Text = language;
        PDisplayLanguageFlag.Source = null;
        PFont.PFontApply(_pDisplayHost.PWindowDeportment, language, PDisplayHeadword);
        PFont.PFontPlace(PDisplayHeadword);

        await PEnsign.PEnsignLoad(_pDisplayHost.PWindowDeportment);

        if (!string.Equals(PDisplayLanguage.Text, language, StringComparison.Ordinal))
        {
            return;
        }

        PDisplayLanguageFlag.Source = PEnsign.PEnsignFind(language);
    }
}
