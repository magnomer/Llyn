using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private readonly MediaPlayer _pDisplayPlayer = new();

    private string? _pDisplayRecording;

    private void PDisplayPlaybackAttach()
    {
        PVolume.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(PVolumeSave));
        PVolume.AddHandler(MouseUpEvent, new MouseButtonEventHandler(PVolumeSave), true);
        PVolume.AddHandler(KeyUpEvent, new KeyEventHandler(PVolumeSave), true);
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

    private void PDisplayPlaybackShow()
    {
        bool audible = _pDisplayRecording is not null
            || _pDisplayAccent.Any(static row => row.PAccentItemAudio.Length > 0);
        PPlayback.Visibility = audible ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PVolumeHandle(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _pDisplayPlayer.Volume = e.NewValue;
        PVolumeCatalog.PVolumeCatalogCurrent.PVolumeCatalogLevel = e.NewValue;
    }

    private void PVolumeSave(object sender, RoutedEventArgs e)
    {
        if (_lEngine.LEngineSettingsRead().LSettingsVolumeMatch(PVolume.Value))
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
