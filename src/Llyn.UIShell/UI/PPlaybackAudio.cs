using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly MediaPlayer _pDownloaderPlayer = new();

    private string? _pRecording;
    private string? _pRecordingSource;

    private void PPlaybackActionHandle(object sender, RoutedEventArgs e)
    {
        if (_pRecording is null || !_lEngine.LEngineRecordingExist(_pRecording))
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

    internal void PVolumeAttach()
    {
        PVolume.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(PVolumeSave));
        PVolume.AddHandler(MouseUpEvent, new MouseButtonEventHandler(PVolumeSave), true);
        PVolume.AddHandler(KeyUpEvent, new KeyEventHandler(PVolumeSave), true);
    }

    internal void PVolumeLoad()
    {
        PVolume.Value = _lEngine.LEngineSettingsRead().LSettingsVolume;
        PVolumeCatalog.PVolumeCatalogCurrent.PVolumeCatalogLevel = PVolume.Value;
    }

    private void PRecordingClear()
    {
        _pRecording = null;
        _pRecordingSource = null;
        _pDownloaderPlayer.Stop();
        PPlaybackAction.Visibility = Visibility.Collapsed;
        PPlaybackTrayShow();
    }

    private void PPlaybackTrayShow()
    {
        bool audible = _pRecording is not null
            || _pAccentItem.Any(static row => row.PAccentItemAudio.Length > 0);
        PPlayback.Visibility = audible ? Visibility.Visible : Visibility.Collapsed;
    }
}
