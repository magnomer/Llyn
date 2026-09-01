using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly MediaPlayer _pDownloaderPlayer = new();

    private string? _pRecording;
    private string? _pRecordingSource;

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
