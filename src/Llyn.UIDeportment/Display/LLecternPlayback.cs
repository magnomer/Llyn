using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class LLecternPlayback
{
    private readonly LDisplaySound _lLecternPlaybackDisplay;

    private LWindow _lLecternPlaybackWindow = null!;

    private UIElement _lLecternPlaybackAction = null!;

    private UIElement _lLecternPlaybackTray = null!;

    private RangeBase _lLecternPlaybackVolume = null!;

    public LLecternPlayback(LDisplaySound display)
    {
        ArgumentNullException.ThrowIfNull(display);

        _lLecternPlaybackDisplay = display;
    }

    public void LLecternPlaybackAttach(LWindow window, UIElement action, UIElement tray, RangeBase volume)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(tray);
        ArgumentNullException.ThrowIfNull(volume);

        _lLecternPlaybackWindow = window;
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
        if (parameter is not LAccentItem row)
        {
            return;
        }

        _lLecternPlaybackDisplay.LDisplayRecordingPlay(
            row.LAccentItemAudio, _lLecternPlaybackWindow.LWindowAtelier.CAtelierVolumeRead());
    }

    public void LLecternActionHandle()
    {
        _lLecternPlaybackDisplay.LDisplayRecordingPlay(
            _lLecternPlaybackWindow.LWindowAtelier.CAtelierVolumeRead());
    }

    private void LLecternVolumeLoad()
    {
        _lLecternPlaybackVolume.Value = _lLecternPlaybackWindow.LWindowAtelier.CAtelierVolumeRead();
    }

    private void LLecternVolumeHandle(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _lLecternPlaybackWindow.LWindowAtelier.CAtelierVolumeSet(e.NewValue, false);
    }

    private void LLecternVolumeSettle(object sender, RoutedEventArgs e)
    {
        _lLecternPlaybackWindow.LWindowAtelier.CAtelierVolumeSet(_lLecternPlaybackVolume.Value, true);
    }
}
