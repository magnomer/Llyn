using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternPlayback
{
    private readonly CDisplaySound _qLecternPlaybackArea;

    private UIElement _qLecternPlaybackAction = null!;

    private UIElement _qLecternPlaybackTray = null!;

    private RangeBase _qLecternPlaybackVolume = null!;

    public QLecternPlayback(CDisplaySound area)
    {
        ArgumentNullException.ThrowIfNull(area);

        _qLecternPlaybackArea = area;
    }

    public void QLecternPlaybackIntroduce(UIElement action, UIElement tray, RangeBase volume)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(tray);
        ArgumentNullException.ThrowIfNull(volume);

        _qLecternPlaybackAction = action;
        _qLecternPlaybackTray = tray;
        _qLecternPlaybackVolume = volume;
    }

    public void QLecternPlaybackRefine()
    {
        CLecternPlayback playback = _qLecternPlaybackArea.CDisplayPlaybackRead();
        _qLecternPlaybackAction.Visibility = QLook.QLookVisibleRead(playback.CLecternPlaybackRecorded);
        _qLecternPlaybackTray.Visibility = QLook.QLookVisibleRead(playback.CLecternPlaybackAudible);
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
}
