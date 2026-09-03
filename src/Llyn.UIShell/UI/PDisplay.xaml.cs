using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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

    public PDisplay()
    {
        InitializeComponent();
        PDisplayIncoming.ItemsSource = _pDisplayIncoming;
    }

    internal void PDisplayAttach(PWindow host, LEngine engine)
    {
        _pDisplayHost = host;
        _lEngine = engine;
    }

    internal void PDisplayShow(string id, LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        _pDisplayRecording = draft.LEntryDraftAudio.Length > 0 && File.Exists(draft.LEntryDraftAudio)
            ? draft.LEntryDraftAudio
            : null;
        PDisplayPlayback.Visibility = _pDisplayRecording is null ? Visibility.Collapsed : Visibility.Visible;

        PDisplayHeadword.Text = draft.LEntryDraftHeadword;
        PDisplayLanguageShow(draft.LEntryDraftLanguage);
        PDisplayPronunciation.Text = draft.LEntryDraftPronunciation;
        PDisplayPronunciationSurface.Visibility = draft.LEntryDraftPronunciation.Length == 0
            ? Visibility.Collapsed
            : Visibility.Visible;

        PDisplayTranslationShow(draft);
        PDisplayIncomingShow(id);

        PDisplaySpeech.ItemsSource = draft.LEntryDraftSpeeches ?? [];
        PDisplaySense.ItemsSource = draft.LEntryDraftSenses;
        PDisplayCollocation.ItemsSource = draft.LEntryDraftCollocations;
        PDisplaySenseSection.Visibility = draft.LEntryDraftSenses.Count == 0
            ? Visibility.Collapsed
            : Visibility.Visible;
        PDisplayCollocationSection.Visibility = draft.LEntryDraftCollocations.Count == 0
            ? Visibility.Collapsed
            : Visibility.Visible;

        PDisplayNote.Text = draft.LEntryDraftNote;
        PDisplayNoteSection.Visibility = draft.LEntryDraftNote.Length == 0
            ? Visibility.Collapsed
            : Visibility.Visible;

        PDisplayEmpty.Visibility = Visibility.Collapsed;
        PDisplayContents.Visibility = Visibility.Visible;
    }

    internal void PDisplayClear()
    {
        _pDisplayRecording = null;
        _pDisplayPlayer.Stop();
        PDisplayLanguage.Text = string.Empty;
        PDisplayLanguageFlag.Source = null;
        PDisplayPlayback.Visibility = Visibility.Collapsed;
        PDisplayPronunciationSurface.Visibility = Visibility.Collapsed;
        PDisplaySpeech.ItemsSource = null;
        _pDisplayIncoming.Clear();
        PDisplayTranslationRead().PTranslationConverterClear();
        PDisplaySense.ItemsSource = null;
        PDisplayCollocation.ItemsSource = null;
        PDisplaySenseSection.Visibility = Visibility.Collapsed;
        PDisplayCollocationSection.Visibility = Visibility.Collapsed;
        PDisplayNoteSection.Visibility = Visibility.Collapsed;
        PDisplayContents.Visibility = Visibility.Collapsed;
        PDisplayEmpty.Visibility = Visibility.Visible;
    }

    internal void PDisplayClose()
    {
        _pDisplayPlayer.Close();
    }

    private void PDisplayTranslationShow(LEntryDraft draft)
    {
        List<string> ids = [];
        PDisplayTranslationRead(draft.LEntryDraftSenses, ids);
        PDisplayTranslationRead(draft.LEntryDraftCollocations, ids);

        PTranslationConverter converter = PDisplayTranslationRead();
        try
        {
            converter.PTranslationConverterShow(_lEngine.LEngineTargetRead(ids));
        }
        catch (Exception)
        {
            converter.PTranslationConverterClear();
        }

        PDisplayCardUpdate();
    }

    private void PDisplayCardUpdate()
    {
        System.Collections.IEnumerable? senses = PDisplaySense.ItemsSource;
        System.Collections.IEnumerable? collocations = PDisplayCollocation.ItemsSource;

        PDisplaySense.ItemsSource = null;
        PDisplayCollocation.ItemsSource = null;
        PDisplaySense.ItemsSource = senses;
        PDisplayCollocation.ItemsSource = collocations;
    }

    private static void PDisplayTranslationRead(IReadOnlyList<LCardDraft> cards, List<string> ids)
    {
        foreach (LCardDraft card in cards)
        {
            foreach (string id in card.LCardDraftTranslation)
            {
                if (!ids.Contains(id))
                {
                    ids.Add(id);
                }
            }
        }
    }

    private PTranslationConverter PDisplayTranslationRead()
    {
        return (PTranslationConverter)Resources["Display.Card.Translation"];
    }

    private void PDisplayIncomingShow(string id)
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

        string unreadable = _pDisplayHost.PLocalizationTextRead("Display.Unreadable");
        string sense = _pDisplayHost.PLocalizationTextRead("Display.SenseSingle");
        string collocation = _pDisplayHost.PLocalizationTextRead("Display.CollocationSingle");

        foreach (LUsage usage in incoming)
        {
            _pDisplayIncoming.Add(new PUsageItem(
                usage,
                usage.LUsageOwner == LOwner.LOwnerCollocation ? collocation : sense,
                unreadable,
                string.Empty));
        }

        PDisplayIncomingEmpty.Visibility = _pDisplayIncoming.Count == 0
            ? Visibility.Visible
            : Visibility.Collapsed;
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

        string? path;
        try
        {
            path = await _lEngine.LEngineFlagRead(language, CancellationToken.None);
        }
        catch (Exception)
        {
            return;
        }

        if (!string.Equals(PDisplayLanguage.Text, language, StringComparison.Ordinal))
        {
            return;
        }

        PDisplayLanguageFlag.Source = path is not null && File.Exists(path)
            ? PLangcodeIndicator.PLangcodeIndicatorResolve(path)
            : null;
    }

    private void PDisplayPlaybackHandle(object sender, RoutedEventArgs e)
    {
        if (_pDisplayRecording is null)
        {
            return;
        }

        _pDisplayPlayer.Open(new Uri(_pDisplayRecording));
        _pDisplayPlayer.Play();
    }
}
