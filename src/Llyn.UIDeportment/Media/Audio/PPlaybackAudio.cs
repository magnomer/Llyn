using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Llyn.Conduct;


namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly MediaPlayer _pDownloaderPlayer = new();

    private Button PPlaybackAction => (Button)FindName(nameof(PPlaybackAction));

    private Border PPlayback => (Border)FindName(nameof(PPlayback));

    private Slider PVolume => (Slider)FindName(nameof(PVolume));

    internal void PPlaybackRefine(CEntryDraft _)
    {
        PPlaybackAudioRefine(_qEditor.QEditorArea.CEditorTimbre.CTimbrePlaybackRead());
    }

    private void PPlaybackActionObserve(object sender, RoutedEventArgs e)
    {
        PPlaybackActionRefine(_qEditor.QEditorArea.CEditorTimbre.CTimbrePlaybackStart(
            (sender as FrameworkElement)?.Tag as string));
    }

    private void PPlaybackActionRefine(Uri? address)
    {
        if (address is null)
        {
            PPlaybackAudioRefine(_qEditor.QEditorArea.CEditorTimbre.CTimbrePlaybackRead());
            return;
        }

        _pDownloaderPlayer.Open(address);
        _pDownloaderPlayer.Play();
    }

    private void PPlaybackAudioRefine(CTimbrePlayback playback)
    {
        if (!Equals(PPlaybackAction.Tag, playback.CTimbrePlaybackAudio))
        {
            _pDownloaderPlayer.Stop();
            PPlaybackAction.Tag = playback.CTimbrePlaybackAudio;
            PPlaybackAction.Visibility = QLook.QLookVisibleRead(playback.CTimbrePlaybackAudio is not null);
        }

        PPlayback.Visibility = QLook.QLookVisibleRead(playback.CTimbrePlaybackAudible);
    }

    private void PVolumePlayerRefine(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _pDownloaderPlayer.Volume = e.NewValue;
        PVolumeCatalog.PVolumeCatalogCurrent.PVolumeCatalogLevel = e.NewValue;
    }

    private void PVolumeObserve(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _pEditorHost.PWindowAtelier.CAtelierVolumeSet(e.NewValue, false);
    }

    private void PVolumeLevelRefine(object? sender, PropertyChangedEventArgs e)
    {
        PVolume.Value = PVolumeCatalog.PVolumeCatalogCurrent.PVolumeCatalogLevel;
    }

    private void PVolumeSaveObserve(object sender, RoutedEventArgs e)
    {
        _pEditorHost.PWindowAtelier.CAtelierVolumeSet(PVolume.Value, true);
    }

    internal void PVolumeAttach()
    {
        PVolume.Value = PVolumeCatalog.PVolumeCatalogCurrent.PVolumeCatalogLevel;
        PVolume.ValueChanged += PVolumePlayerRefine;
        PVolume.ValueChanged += PVolumeObserve;
        PVolumeCatalog.PVolumeCatalogCurrent.PropertyChanged += PVolumeLevelRefine;
        PVolume.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(PVolumeSaveObserve));
        PVolume.AddHandler(MouseUpEvent, new MouseButtonEventHandler(PVolumeSaveObserve), true);
        PVolume.AddHandler(KeyUpEvent, new KeyEventHandler(PVolumeSaveObserve), true);
    }

    internal void PVolumeRefine()
    {
        PVolume.Value = _pEditorHost.PWindowAtelier.CAtelierVolumeRead();
        _pDownloaderPlayer.Volume = PVolume.Value;
    }
}
