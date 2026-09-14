using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PDisplay : UserControl
{
    private readonly ObservableCollection<PUsageItem> _pDisplayIncoming = [];

    private PWindow _pDisplayHost = null!;

    private LEngine _lEngine = null!;

    private long? _pDisplayEntry;

    private PObserver? _pDisplayObserver;

    public PDisplay()
    {
        InitializeComponent();
        PDisplayIncoming.ItemsSource = _pDisplayIncoming;
        PDisplayAccent.ItemsSource = _pDisplayAccent;
        PDisplayTranscription.ItemsSource = _pDisplayTranscription;

        PDisplayPlaybackAttach();
        AddHandler(PMention.PMentionClickEvent, new EventHandler<PMentionArgument>(PDisplayMentionHandle));

        PCompassAttach();
    }

    internal void PDisplayAttach(PWindow host, LEngine engine)
    {
        _pDisplayHost = host;
        _lEngine = engine;

        _pDisplayObserver = new PObserver(this, PDisplayBulletinHandle);
        engine.LEngineObserverAttach(_pDisplayObserver);

        PVolumeLoad();
    }

    private void PDisplayBulletinHandle(LBulletin bulletin)
    {
        if (_pDisplayEntry is not long shown)
        {
            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectFavorite)
        {
            if (shown == bulletin.LBulletinId)
            {
                PDisplayFavoriteShow(shown);
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectGrasp)
        {
            if (shown == bulletin.LBulletinId)
            {
                PDisplayGraspShow(shown);
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectFrequency)
        {
            if (shown == bulletin.LBulletinId)
            {
                PDisplayFrequencyShow(shown);
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectInflection)
        {
            if (shown == bulletin.LBulletinId)
            {
                PDisplayParadigmShow(shown);
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            PDisplayClear();
            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectDraft)
        {
            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectEntry
            && bulletin.LBulletinId > 0
            && shown != bulletin.LBulletinId)
        {
            return;
        }

        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(shown);
        }
        catch (Exception)
        {
            return;
        }

        if (draft is null)
        {
            PDisplayClear();
            return;
        }

        PDisplayShow(shown, draft);
    }

    private static IReadOnlyList<string> PDisplaySpeechShow(IReadOnlyList<LSpeechDraft> speeches)
    {
        List<string> named = new(speeches.Count);
        foreach (LSpeechDraft speech in speeches)
        {
            if (speech.LSpeechDraftName.Length > 0)
            {
                named.Add(speech.LSpeechDraftName);
            }
        }

        return named;
    }

    internal void PDisplayShow(long id, LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        _pDisplayEntry = id;
        PDisplayFavoriteShow(id);
        PDisplayGraspShow(id);

        _pDisplayRecording = _lEngine.LEngineRecordingExist(draft.LEntryDraftAudio)
            ? draft.LEntryDraftAudio
            : null;
        PPlaybackAction.Visibility = _pDisplayRecording is null ? Visibility.Collapsed : Visibility.Visible;
        PVolumeLoad();

        PDisplayHeadword.Text = draft.LEntryDraftHeadword;
        PDisplayLanguageShow(draft.LEntryDraftLanguage);
        PDisplayPronunciation.Text = draft.LEntryDraftIpa;
        PDisplayPronunciationSurface.Visibility = draft.LEntryDraftIpa.Length == 0
            ? Visibility.Collapsed
            : Visibility.Visible;
        PDisplayAccentShow(draft);
        PDisplayTranscriptionShow(draft);
        PDisplayPlaybackShow();

        PDisplayFrameShow(draft.LEntryDraftLanguage);
        PDisplayExampleShow(draft.LEntryDraftLanguage);
        PDisplayCitationShow();
        PDisplayTranslationShow(draft);
        PDisplayIncomingShow(id);

        PDisplaySpeech.ItemsSource = PDisplaySpeechShow(draft.LEntryDraftSpeeches);
        PDisplaySpeechSection.Visibility = draft.LEntryDraftSpeeches.Count == 0
            ? Visibility.Collapsed
            : Visibility.Visible;
        PDisplayFrequencyShow(id);
        PDisplayParadigmShow(id);
        PDisplayMeaning.ItemsSource = draft.LEntryDraftMeanings;
        PDisplayCollocation.ItemsSource = draft.LEntryDraftCollocations;
        PDisplayMeaningSection.Visibility = draft.LEntryDraftMeanings.Count == 0
            ? Visibility.Collapsed
            : Visibility.Visible;
        PDisplayCollocationSection.Visibility = draft.LEntryDraftCollocations.Count == 0
            ? Visibility.Collapsed
            : Visibility.Visible;

        PMarkdown.PMarkdownShow(PDisplayNote, draft.LEntryDraftNote);
        PDisplayNoteSection.Visibility = draft.LEntryDraftNote.Length == 0
            ? Visibility.Collapsed
            : Visibility.Visible;

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
            entry = _lEngine.LEngineEntryRead(id);
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
        LFrequency? frequency;
        try
        {
            frequency = _lEngine.LEngineFrequencyRead(id);
        }
        catch (Exception)
        {
            frequency = null;
        }

        if (frequency is null)
        {
            PDisplayFrequency.Text = string.Empty;
            PDisplayFrequencyChip.ToolTip = null;
            PDisplayFrequencySection.Visibility = Visibility.Collapsed;
            return;
        }

        PDisplayFrequency.Text = PFrequencyLabel.PFrequencyLabelFormat(frequency);
        PDisplayFrequencyChip.ToolTip = PFrequencyLabel.PFrequencySourceFormat(
            frequency, _pDisplayHost.PLocalizationTextRead("Frequency.Unit"));
        PDisplayFrequencySection.Visibility = Visibility.Visible;
    }

    private static string PDisplayStampFormat(string? utc)
    {
        if (utc is null
            || !DateTimeOffset.TryParse(utc, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset parsed))
        {
            return string.Empty;
        }

        return parsed.ToLocalTime().ToString("g", CultureInfo.CurrentCulture);
    }

    internal void PDisplayClear()
    {
        _pDisplayEntry = null;
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
        PDisplayAccentClear();
        _pDisplayTranscription.Clear();
        PDisplaySpeech.ItemsSource = null;
        PDisplaySpeechSection.Visibility = Visibility.Collapsed;
        PDisplayFrequency.Text = string.Empty;
        PDisplayFrequencyChip.ToolTip = null;
        PDisplayFrequencySection.Visibility = Visibility.Collapsed;
        PDisplayParadigm.PParadigmItems = null;
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
        if (_pDisplayObserver is not null)
        {
            _lEngine.LEngineObserverDetach(_pDisplayObserver);
            _pDisplayObserver = null;
        }

        _pDisplayPlayer.Close();
    }

    private void PDisplayTranslationShow(LEntryDraft draft)
    {
        List<long> ids = [];
        PDisplayTranslationRead(draft.LEntryDraftMeanings, ids);
        PDisplayTranslationRead(draft.LEntryDraftCollocations, ids);

        PLinkConverter converter = PDisplayTranslationRead();
        try
        {
            converter.PLinkConverterShow(_lEngine.LEngineTargetRead(ids));
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

    private static void PDisplayTranslationRead(IReadOnlyList<LCardDraft> cards, List<long> ids)
    {
        foreach (LCardDraft card in cards)
        {
            foreach (long id in card.LCardDraftTranslation)
            {
                if (!ids.Contains(id))
                {
                    ids.Add(id);
                }
            }
        }
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
            order = _lEngine.LEngineOrderRead(language);
        }
        catch (Exception)
        {
            order = LSentenceOrder.LSentenceOrderDefault;
        }

        ((PSentenceConverter)Resources["Display.Card.Frame"]).PSentenceConverterApply(order);
    }

    private void PDisplayExampleShow(string language)
    {
        PFont.PFontExampleApply(Resources, _lEngine, language);
    }

    private void PDisplayIncomingShow(long id)
    {
        _pDisplayIncoming.Clear();

        IReadOnlyList<LUsage> incoming;
        try
        {
            incoming = _lEngine.LEngineIncomingRead(id);
        }
        catch (Exception)
        {
            incoming = [];
        }

        string unknown = _pDisplayHost.PLocalizationTextRead("Display.Unknown");
        string meaning = _pDisplayHost.PLocalizationTextRead("Display.MeaningSingle");
        string collocation = _pDisplayHost.PLocalizationTextRead("Display.CollocationSingle");

        foreach (LUsage usage in incoming)
        {
            _pDisplayIncoming.Add(new PUsageItem(
                usage,
                usage.LUsageOwner == LOwner.LOwnerCollocation ? collocation : meaning,
                unknown,
                string.Empty));
        }

        PTwin.PTwinNameApply(
            _pDisplayIncoming, row => row.PUsageItemHeadword, (row, name) => row.PUsageItemName = name);

        PDisplayIncomingSection.Visibility = _pDisplayIncoming.Count == 0
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private void PDisplayCardHandle(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not FrameworkElement chip)
        {
            return;
        }

        switch (chip.DataContext)
        {
            case LSituationDraft situation when situation.LSituationDraftId != 0:
                e.Handled = true;
                _pDisplayHost.PWindowSituationShow(situation.LSituationDraftId);
                break;
            case LRegisterDraft register when register.LRegisterDraftId != 0:
                e.Handled = true;
                _pDisplayHost.PWindowRegisterShow(register.LRegisterDraftId);
                break;
            case PLinkChip link when link.PLinkChipId != 0:
                e.Handled = true;
                _pDisplayHost.PWindowEntryShow(link.PLinkChipId);
                break;
            case LTagDraft tag when tag.LTagDraftId != 0:
                e.Handled = true;
                _pDisplayHost.PWindowTagShow(tag.LTagDraftId);
                break;
        }
    }

    private void PDisplayIncomingHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement row && row.DataContext is PUsageItem item)
        {
            _pDisplayHost.PWindowEntryShow(item.PUsageItemEntry);
        }
    }

    private async void PDisplayLanguageShow(string language)
    {
        PDisplayLanguage.Text = language;
        PDisplayLanguageFlag.Source = null;
        PFont.PFontApply(_lEngine, language, PDisplayHeadword);

        await PEnsign.PEnsignLoad(_lEngine);

        if (!string.Equals(PDisplayLanguage.Text, language, StringComparison.Ordinal))
        {
            return;
        }

        PDisplayLanguageFlag.Source = PEnsign.PEnsignFind(language);
    }
}
