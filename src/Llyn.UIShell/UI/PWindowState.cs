using System;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PWindow
{
    private void PWindowStateRestore()
    {
        if (_lEngine.LEngineSettingsRead().LSettingsWindow is not LWindowState state)
        {
            return;
        }

        double limitLeft = SystemParameters.VirtualScreenLeft;
        double limitTop = SystemParameters.VirtualScreenTop;
        double width = Math.Clamp(state.LWindowStateWidth, MinWidth, Math.Max(MinWidth, SystemParameters.VirtualScreenWidth));
        double height = Math.Clamp(state.LWindowStateHeight, MinHeight, Math.Max(MinHeight, SystemParameters.VirtualScreenHeight));
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
        Rect bounds = WindowState == WindowState.Normal
            ? new Rect(Left, Top, Width, Height)
            : RestoreBounds;

        if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        _lEngine.LEngineSettingsSave(_lEngine.LEngineSettingsRead() with
        {
            LSettingsWindow = new LWindowState(
                bounds.Left,
                bounds.Top,
                bounds.Width,
                bounds.Height,
                WindowState == WindowState.Maximized)
        });
    }
}
