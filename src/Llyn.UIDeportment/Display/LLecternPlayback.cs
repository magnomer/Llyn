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

    private Action<double> _lLecternPlaybackSeam = null!;

    public LLecternPlayback(LDisplaySound display)
    {
        ArgumentNullException.ThrowIfNull(display);

        _lLecternPlaybackDisplay = display;
    }

    public void LLecternPlaybackAttach(
        LWindow window, UIElement action, UIElement tray, RangeBase volume, Action<double> volumeSeam)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(tray);
        ArgumentNullException.ThrowIfNull(volume);
        ArgumentNullException.ThrowIfNull(volumeSeam);

        _lLecternPlaybackWindow = window;
        _lLecternPlaybackAction = action;
        _lLecternPlaybackTray = tray;
        _lLecternPlaybackVolume = volume;
        _lLecternPlaybackSeam = volumeSeam;

        volume.ValueChanged += (_, e) => LLecternVolumeHandle(e.NewValue);
        volume.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler((_, _) => LLecternVolumeSave()));
        volume.AddHandler(UIElement.MouseUpEvent, new MouseButtonEventHandler((_, _) => LLecternVolumeSave()), true);
        volume.AddHandler(UIElement.KeyUpEvent, new KeyEventHandler((_, _) => LLecternVolumeSave()), true);
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

        _lLecternPlaybackDisplay.LDisplayRecordingPlay(row.LAccentItemAudio);
    }

    public void LLecternActionHandle()
    {
        _lLecternPlaybackDisplay.LDisplayRecordingPlay();
    }

    private void LLecternVolumeLoad()
    {
        _lLecternPlaybackVolume.Value = _lLecternPlaybackWindow.LWindowPostureRead().LPostureStateVolume;
        LLecternVolumeHandle(_lLecternPlaybackVolume.Value);
    }

    private void LLecternVolumeHandle(double level)
    {
        _lLecternPlaybackDisplay.LDisplayVolumeSet(level);
        _lLecternPlaybackSeam(level);
    }

    private void LLecternVolumeSave()
    {
        _lLecternPlaybackWindow.LWindowVolumeSave(_lLecternPlaybackVolume.Value);
    }
}
