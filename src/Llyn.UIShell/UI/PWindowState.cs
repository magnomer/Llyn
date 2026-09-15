using System;
using System.Windows;
using System.Windows.Threading;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PWindow
{
    private const double PWindowStateShare = 0.8;

    private DispatcherTimer? _pWindowStateTimer;

    private void PWindowStateAttach()
    {
        _pWindowStateTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(700)
        };
        _pWindowStateTimer.Tick += (_, _) => PWindowStateSave();

        LocationChanged += (_, _) => PWindowStateDefer();
        SizeChanged += (_, _) => PWindowStateDefer();
        StateChanged += (_, _) => PWindowStateDefer();
    }

    private void PWindowStateDefer()
    {
        if (_pWindowStateTimer is not DispatcherTimer timer || !IsLoaded)
        {
            return;
        }

        timer.Stop();
        timer.Start();
    }

    private void PWindowStateRestore()
    {
        if (_lEngine.LEngineSettingsRead().LSettingsWindow is not LWindowState state)
        {
            Rect area = SystemParameters.WorkArea;

            Width = Math.Clamp(area.Width * PWindowStateShare, MinWidth, Math.Max(MinWidth, area.Width));
            Height = Math.Clamp(area.Height * PWindowStateShare, MinHeight, Math.Max(MinHeight, area.Height));

            return;
        }

        double limitLeft = SystemParameters.VirtualScreenLeft;
        double limitTop = SystemParameters.VirtualScreenTop;
        double width = Math.Clamp(
            state.LWindowStateWidth, MinWidth, Math.Max(MinWidth, SystemParameters.VirtualScreenWidth));
        double height = Math.Clamp(
            state.LWindowStateHeight, MinHeight, Math.Max(MinHeight, SystemParameters.VirtualScreenHeight));
        double limitRight = Math.Max(limitLeft, limitLeft + SystemParameters.VirtualScreenWidth - width);
        double limitBottom = Math.Max(limitTop, limitTop + SystemParameters.VirtualScreenHeight - height);

        WindowStartupLocation = WindowStartupLocation.Manual;
        Left = Math.Clamp(state.LWindowStateLeft, limitLeft, limitRight);
        Top = Math.Clamp(state.LWindowStateTop, limitTop, limitBottom);
        Width = width;
        Height = height;

        if (state.LWindowStateMaximized)
        {
            WindowState = WindowState.Maximized;
        }
    }

    private void PWindowStateSave()
    {
        _pWindowStateTimer?.Stop();

        if (WindowState == WindowState.Minimized)
        {
            return;
        }

        Rect bounds = WindowState == WindowState.Normal
            ? new Rect(Left, Top, Width, Height)
            : RestoreBounds;

        if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        _lEngine.LEngineWindowSave(new LWindowState(
            bounds.Left,
            bounds.Top,
            bounds.Width,
            bounds.Height,
            WindowState == WindowState.Maximized));
    }
}
