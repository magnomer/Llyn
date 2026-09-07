using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly MediaPlayer _pDownloaderPlayer = new();

    private string? _pRecording;
    private string? _pRecordingSource;

    private bool _pRecordingStored;

    private void PPlaybackActionHandle(object sender, RoutedEventArgs e)
    {
        if (_pRecording is null || !File.Exists(_pRecording))
        {
            PRecordingClear();
            return;
        }

        _pDownloaderPlayer.Open(new Uri(_pRecording));
        _pDownloaderPlayer.Play();
    }

    private void PVolumeHandle(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _pDownloaderPlayer.Volume = e.NewValue;
    }

    private void PVolumeSave(object sender, RoutedEventArgs e)
    {
        if (PVolume.Value == _lEngine.LEngineSettingsRead().LSettingsVolume)
        {
            return;
        }

        _lEngine.LEngineVolumeSave(PVolume.Value);
    }

    internal void PVolumeAttach()
    {
        PVolume.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(PVolumeSave));
        PVolume.AddHandler(MouseUpEvent, new MouseButtonEventHandler(PVolumeSave), true);
        PVolume.AddHandler(KeyUpEvent, new KeyEventHandler(PVolumeSave), true);
    }

    internal void PVolumeLoad()
    {
        PVolume.Value = _lEngine.LEngineSettingsRead().LSettingsVolume;
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
