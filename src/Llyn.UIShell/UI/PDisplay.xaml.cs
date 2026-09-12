using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PDisplay : UserControl
{
    private readonly MediaPlayer _pDisplayPlayer = new();

    private readonly ObservableCollection<PUsageItem> _pDisplayIncoming = [];

    private PWindow _pDisplayHost = null!;

    private LEngine _lEngine = null!;

    private string? _pDisplayRecording;

    private long? _pDisplayEntry;

    private PObserver? _pDisplayObserver;

    public PDisplay()
    {
        InitializeComponent();
        PDisplayIncoming.ItemsSource = _pDisplayIncoming;

        PVolume.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(PVolumeSave));
        PVolume.AddHandler(MouseUpEvent, new MouseButtonEventHandler(PVolumeSave), true);
        PVolume.AddHandler(KeyUpEvent, new KeyEventHandler(PVolumeSave), true);
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

        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            PDisplayClear();
            return;
        }

        if (bulletin.LBulletinId > 0
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

        _pDisplayRecording = draft.LEntryDraftAudio.Length > 0 && File.Exists(draft.LEntryDraftAudio)
            ? draft.LEntryDraftAudio
            : null;
        PPlayback.Visibility = _pDisplayRecording is null ? Visibility.Collapsed : Visibility.Visible;
        PVolumeLoad();

        PDisplayHeadword.Text = draft.LEntryDraftHeadword;
        PDisplayLanguageShow(draft.LEntryDraftLanguage);
        PDisplayPronunciation.Text = draft.LEntryDraftIpa;
        PDisplayPronunciationSurface.Visibility = draft.LEntryDraftIpa.Length == 0
            ? Visibility.Collapsed
            : Visibility.Visible;
        PPlayback.Margin = draft.LEntryDraftIpa.Length == 0
            ? new Thickness(0)
            : new Thickness(10, 0, 0, 0);

        PDisplayFrameShow(draft.LEntryDraftLanguage);
        PDisplayExampleShow(draft.LEntryDraftLanguage);
        PDisplayTranslationShow(draft);
        PDisplayIncomingShow(id);

        PDisplaySpeech.ItemsSource = PDisplaySpeechShow(draft.LEntryDraftSpeeches);
        PDisplaySpeechSection.Visibility = draft.LEntryDraftSpeeches.Count == 0
            ? Visibility.Collapsed
            : Visibility.Visible;
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

        PDisplayEmpty.Visibility = Visibility.Collapsed;
        PDisplayContents.Visibility = Visibility.Visible;

        PCompassUpdate();
    }

    internal void PDisplayClear()
    {
        _pDisplayEntry = null;
        PDisplayFavorite.IsChecked = false;
        _pDisplayRecording = null;
        _pDisplayPlayer.Stop();
        PDisplayLanguage.Text = string.Empty;
        PDisplayLanguageFlag.Source = null;
        PPlayback.Visibility = Visibility.Collapsed;
        PDisplayPronunciationSurface.Visibility = Visibility.Collapsed;
        PDisplaySpeech.ItemsSource = null;
        PDisplaySpeechSection.Visibility = Visibility.Collapsed;
        _pDisplayIncoming.Clear();
        PDisplayIncomingSection.Visibility = Visibility.Collapsed;
        PDisplayTranslationRead().PLinkConverterClear();
        PDisplayMeaning.ItemsSource = null;
        PDisplayCollocation.ItemsSource = null;
        PDisplayMeaningSection.Visibility = Visibility.Collapsed;
        PDisplayCollocationSection.Visibility = Visibility.Collapsed;
        PDisplayNoteSection.Visibility = Visibility.Collapsed;
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

    private void PDisplayFavoriteShow(long id)
    {
        try
        {
            PDisplayFavorite.IsChecked = _lEngine.LEngineFavoriteCheck(id);
        }
        catch (Exception)
        {
            PDisplayFavorite.IsChecked = false;
        }
    }

    private void PDisplayFavoriteHandle(object sender, RoutedEventArgs e)
    {
        if (_pDisplayEntry is null)
        {
            PDisplayFavorite.IsChecked = false;
            return;
        }

        bool marked = PDisplayFavorite.IsChecked == true;
        try
        {
            if (marked)
            {
                _lEngine.LEngineFavoriteSave(_pDisplayEntry!.Value);
            }
            else
            {
                _lEngine.LEngineFavoriteDelete(_pDisplayEntry!.Value);
            }
        }
        catch (Exception exception)
        {
            PDisplayFavorite.IsChecked = !marked;
            _pDisplayHost.PWindowFailureShow("Favorite.MarkFailed", exception);
            return;
        }

    }

    private void PPlaybackActionHandle(object sender, RoutedEventArgs e)
    {
        if (_pDisplayRecording is null)
        {
            return;
        }

        _pDisplayPlayer.Open(new Uri(_pDisplayRecording));
        _pDisplayPlayer.Play();
    }

    private void PVolumeHandle(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _pDisplayPlayer.Volume = e.NewValue;
        PVolumeCatalog.PVolumeCatalogCurrent.PVolumeCatalogLevel = e.NewValue;
    }

    private void PVolumeSave(object sender, RoutedEventArgs e)
    {
        if (PVolume.Value == _lEngine.LEngineSettingsRead().LSettingsVolume)
        {
            return;
        }

        _lEngine.LEngineVolumeSave(PVolume.Value);
    }

    private void PVolumeLoad()
    {
        PVolume.Value = _lEngine.LEngineSettingsRead().LSettingsVolume;
        PVolumeCatalog.PVolumeCatalogCurrent.PVolumeCatalogLevel = PVolume.Value;
    }
}
