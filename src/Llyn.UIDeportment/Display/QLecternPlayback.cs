using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternPlayback
{
    private readonly CDisplayPlayback _qLecternPlaybackArea;

    private readonly UIElement _qLecternPlaybackAction;

    private readonly UIElement _qLecternPlaybackTray;

    private readonly RangeBase _qLecternPlaybackVolume;

    public QLecternPlayback(FrameworkElement surface, CDisplayPlayback area)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(area);

        _qLecternPlaybackArea = area;
        Button action = QContract.QContractFind<Button>(surface, "PPlaybackAction");
        _qLecternPlaybackAction = action;
        _qLecternPlaybackTray = QContract.QContractFind<Border>(surface, "PPlayback");
        _qLecternPlaybackVolume = QContract.QContractFind<Slider>(surface, "PVolume");
        ItemsControl accents = QContract.QContractFind<ItemsControl>(surface, "PDisplayAccent");

        accents.CommandBindings.Add(new CommandBinding(
            QAccentCommand.QAccentCommandPlayback, QLecternPlaybackObserve));
        action.Click += QLecternActionObserve;
        action.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("play", 24));
    }

    public void QLecternPlaybackRefine()
    {
        CLecternPlayback playback = _qLecternPlaybackArea.CDisplayPlaybackRead();
        _qLecternPlaybackAction.Visibility = QLook.QLookVisibleRead(playback.CLecternPlaybackRecorded);
        _qLecternPlaybackTray.Visibility = QLook.QLookVisibleRead(playback.CLecternPlaybackAudible);
    }

    private void QLecternPlaybackObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QAccentItem row)
        {
            _qLecternPlaybackArea.CDisplayPlaybackStart(row.QAccentItemAudio, _qLecternPlaybackVolume.Value);
        }
    }

    private void QLecternActionObserve(object sender, RoutedEventArgs e)
    {
        _qLecternPlaybackArea.CDisplayPlaybackStart(_qLecternPlaybackVolume.Value);
    }
}
