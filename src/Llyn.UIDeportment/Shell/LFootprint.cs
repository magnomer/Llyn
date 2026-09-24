using System;
using System.ComponentModel;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class LFootprint
{
    private const double LFootprintShare = 0.8;

    private const int LFootprintDelay = 700;

    private readonly Window _lFootprintWindow;

    private readonly LWindow _lWindow;

    public LFootprint(Window window, LWindow deportment)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(deportment);

        _lFootprintWindow = window;
        _lWindow = deportment;
        window.Loaded += (_, _) => LFootprintAttach();
    }

    public void LFootprintRestore()
    {
        Window window = _lFootprintWindow;

        if (_lWindow.LWindowPostureRead().LPostureStateWindow is not LWindowState state)
        {
            Rect area = SystemParameters.WorkArea;

            window.Width = Math.Clamp(
                area.Width * LFootprintShare, window.MinWidth, Math.Max(window.MinWidth, area.Width));
            window.Height = Math.Clamp(
                area.Height * LFootprintShare, window.MinHeight, Math.Max(window.MinHeight, area.Height));

            return;
        }

        double limitLeft = SystemParameters.VirtualScreenLeft;
        double limitTop = SystemParameters.VirtualScreenTop;
        double width = Math.Clamp(
            state.LWindowStateWidth,
            window.MinWidth,
            Math.Max(window.MinWidth, SystemParameters.VirtualScreenWidth));
        double height = Math.Clamp(
            state.LWindowStateHeight,
            window.MinHeight,
            Math.Max(window.MinHeight, SystemParameters.VirtualScreenHeight));
        double limitRight = Math.Max(limitLeft, limitLeft + SystemParameters.VirtualScreenWidth - width);
        double limitBottom = Math.Max(limitTop, limitTop + SystemParameters.VirtualScreenHeight - height);

        window.WindowStartupLocation = WindowStartupLocation.Manual;
        window.Left = Math.Clamp(state.LWindowStateLeft, limitLeft, limitRight);
        window.Top = Math.Clamp(state.LWindowStateTop, limitTop, limitBottom);
        window.Width = width;
        window.Height = height;

        if (state.LWindowStateMaximized)
        {
            window.WindowState = WindowState.Maximized;
        }
    }

    public void LFootprintClosingHandle(CancelEventArgs e, bool confirmed)
    {
        e.Cancel = !confirmed;

        if (confirmed)
        {
            LFootprintSave(0);
        }
    }

    private void LFootprintAttach()
    {
        _lFootprintWindow.LocationChanged += (_, _) => LFootprintSave(LFootprintDelay);
        _lFootprintWindow.SizeChanged += (_, _) => LFootprintSave(LFootprintDelay);
        _lFootprintWindow.StateChanged += (_, _) => LFootprintSave(LFootprintDelay);
    }

    private void LFootprintSave(int delay)
    {
        Window window = _lFootprintWindow;
        Rect bounds = window.WindowState == WindowState.Normal
            ? new Rect(window.Left, window.Top, window.Width, window.Height)
            : window.RestoreBounds;

        if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        _lWindow.LWindowStateDefer(
            new LWindowState(
                bounds.Left,
                bounds.Top,
                bounds.Width,
                bounds.Height,
                window.WindowState == WindowState.Maximized),
            window.WindowState == WindowState.Minimized,
            delay);
    }
}
