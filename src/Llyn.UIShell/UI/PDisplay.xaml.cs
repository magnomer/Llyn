using System;
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

    private LEngine _lEngine = null!;

    private string? _pDisplayRecording;

    public PDisplay()
    {
        InitializeComponent();
    }

    internal void PDisplayAttach(LEngine engine)
    {
        _lEngine = engine;
    }

    internal void PDisplayShow(LEntryDraft draft)
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
