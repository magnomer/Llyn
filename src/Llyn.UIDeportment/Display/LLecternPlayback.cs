using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class LLecternPlayback
{
    private readonly LDisplaySound _lLecternPlaybackDisplay;

    private CAtelier _lLecternPlaybackAtelier = null!;

    private UIElement _lLecternPlaybackAction = null!;

    private UIElement _lLecternPlaybackTray = null!;

    private RangeBase _lLecternPlaybackVolume = null!;

    public LLecternPlayback(LDisplaySound display)
    {
        ArgumentNullException.ThrowIfNull(display);

        _lLecternPlaybackDisplay = display;
    }

    public void LLecternPlaybackAttach(CAtelier atelier, UIElement action, UIElement tray, RangeBase volume)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(tray);
        ArgumentNullException.ThrowIfNull(volume);

        _lLecternPlaybackAtelier = atelier;
        _lLecternPlaybackAction = action;
        _lLecternPlaybackTray = tray;
        _lLecternPlaybackVolume = volume;

        volume.ValueChanged += LLecternVolumeHandle;
        volume.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(LLecternVolumeSettle));
        volume.AddHandler(UIElement.MouseUpEvent, new MouseButtonEventHandler(LLecternVolumeSettle), true);
        volume.AddHandler(UIElement.KeyUpEvent, new KeyEventHandler(LLecternVolumeSettle), true);
        LLecternVolumeLoad();
    }

    public void LLecternPlaybackShow()
    {
        _lLecternPlaybackAction.Visibility = _lLecternPlaybackDisplay.LDisplayRecordingCheck()
            ? Visibility.Visible
            : Visibility.Collapsed;
        _lLecternPlaybackTray.Visibility = _lLecternPlaybackDisplay.LDisplayAudibleCheck()
            ? Visibility.Visible
            : Visibility.Collapsed;
        LLecternVolumeLoad();
    }

    public void LLecternPlaybackClear()
    {
        _lLecternPlaybackAction.Visibility = Visibility.Collapsed;
        _lLecternPlaybackTray.Visibility = Visibility.Collapsed;
    }

    public void LLecternPlaybackHandle(object parameter)
    {
        if (parameter is not QAccentItem row)
        {
            return;
        }

        _lLecternPlaybackDisplay.LDisplayRecordingPlay(
            row.QAccentItemAudio, _lLecternPlaybackAtelier.CAtelierVolumeRead());
    }

    public void LLecternActionHandle()
    {
        _lLecternPlaybackDisplay.LDisplayRecordingPlay(
            _lLecternPlaybackAtelier.CAtelierVolumeRead());
    }

    private void LLecternVolumeLoad()
    {
        _lLecternPlaybackVolume.Value = _lLecternPlaybackAtelier.CAtelierVolumeRead();
    }

    private void LLecternVolumeHandle(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _lLecternPlaybackAtelier.CAtelierVolumeSet(e.NewValue, false);
    }

    private void LLecternVolumeSettle(object sender, RoutedEventArgs e)
    {
        _lLecternPlaybackAtelier.CAtelierVolumeSet(_lLecternPlaybackVolume.Value, true);
    }
}
