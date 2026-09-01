using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Llyn.UIShell;

/// <summary>
/// The recording the editing form currently carries: which file it is, where it came from, whether it
/// is the loaded entry's own, and playing it back. The downloader attaches one and the entry loader
/// attaches one; this is where it lives, plays, and is let go of.
/// </summary>
public partial class PEditor
{
    private readonly MediaPlayer _pDownloaderPlayer = new();

    private string? _pRecording;
    private string? _pRecordingSource;

    // Whether the recording on the form came back with a loaded entry rather than from the
    // downloader. A fetched recording belongs to the spelling it was fetched for; a stored one
    // belongs to the entry, and survives an edit of the headword.
    private bool _pRecordingStored;

    private void PPlaybackHandle(object sender, RoutedEventArgs e)
    {
        if (_pRecording is null || !File.Exists(_pRecording))
        {
            PRecordingClear();
            return;
        }

        _pDownloaderPlayer.Open(new Uri(_pRecording));
        _pDownloaderPlayer.Play();
    }

    // A recording the downloader fetched is audio of one spelling, so changing the spelling throws it
    // away rather than leaving the wrong word attached. A recording that came back with a loaded entry
    // is the entry's own, and correcting a typo in the headword must not delete it — dropping it here
    // is what made the next save write the entry with no audio row at all.
    private void PHeadwordHandle(object sender, TextChangedEventArgs e)
    {
        if (_pRecordingStored)
        {
            return;
        }

        PRecordingClear();
    }

    private void PRecordingClear()
    {
        _pRecording = null;
        _pRecordingSource = null;
        _pRecordingStored = false;
        _pDownloaderPlayer.Stop();
        PPlayback.Visibility = Visibility.Collapsed;
    }
}
