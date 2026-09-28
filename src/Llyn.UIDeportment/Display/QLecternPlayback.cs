using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternPlayback
{
    private readonly CDisplaySound _qLecternPlaybackArea;

    private CAtelier _qLecternPlaybackAtelier = null!;

    private UIElement _qLecternPlaybackAction = null!;

    private UIElement _qLecternPlaybackTray = null!;

    private RangeBase _qLecternPlaybackVolume = null!;

    public QLecternPlayback(CDisplaySound area)
    {
        ArgumentNullException.ThrowIfNull(area);

        _qLecternPlaybackArea = area;
    }

    public void QLecternPlaybackIntroduce(CAtelier atelier, UIElement action, UIElement tray, RangeBase volume)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(tray);
        ArgumentNullException.ThrowIfNull(volume);

        _qLecternPlaybackAtelier = atelier;
        _qLecternPlaybackAction = action;
        _qLecternPlaybackTray = tray;
        _qLecternPlaybackVolume = volume;

        volume.ValueChanged += QLecternVolumeObserve;
        volume.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(QLecternSettleObserve));
        volume.AddHandler(UIElement.MouseUpEvent, new MouseButtonEventHandler(QLecternSettleObserve), true);
        volume.AddHandler(UIElement.KeyUpEvent, new KeyEventHandler(QLecternSettleObserve), true);
        QLecternVolumeRefine();
    }

    public void QLecternPlaybackRefine()
    {
        CLecternPlayback playback = _qLecternPlaybackArea.CDisplayPlaybackRead();
        _qLecternPlaybackAction.Visibility = QLook.QLookVisibleRead(playback.CLecternPlaybackRecorded);
        _qLecternPlaybackTray.Visibility = QLook.QLookVisibleRead(playback.CLecternPlaybackAudible);
    }

    public void QLecternVolumeRefine()
    {
        _qLecternPlaybackVolume.Value = _qLecternPlaybackAtelier.CAtelierVolumeRead();
    }

    public void QLecternTrayRefine()
    {
        _qLecternPlaybackAction.Visibility = Visibility.Collapsed;
        _qLecternPlaybackTray.Visibility = Visibility.Collapsed;
    }

    public void QLecternPlaybackObserve(object parameter)
    {
        if (parameter is QAccentItem row)
        {
            _qLecternPlaybackArea.CDisplayPlaybackStart(row.QAccentItemAudio, _qLecternPlaybackVolume.Value);
        }
    }

    public void QLecternActionObserve()
    {
        _qLecternPlaybackArea.CDisplayPlaybackStart(_qLecternPlaybackVolume.Value);
    }

    private void QLecternVolumeObserve(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _qLecternPlaybackAtelier.CAtelierVolumeSet(e.NewValue, false);
    }

    private void QLecternSettleObserve(object sender, RoutedEventArgs e)
    {
        _qLecternPlaybackAtelier.CAtelierVolumeSet(_qLecternPlaybackVolume.Value, true);
    }
}
