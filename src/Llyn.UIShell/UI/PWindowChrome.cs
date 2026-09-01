using System;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

/// <summary>
/// The window frame the program draws for itself, since the system one is off: the caption buttons,
/// dragging the window by its roof, and the maximize/restore toggle they share.
/// </summary>
public partial class PWindow
{
    private void PCaptionMinimizeHandle(object sender, RoutedEventArgs e)
    {
        SystemCommands.MinimizeWindow(this);
    }

    private void PCaptionMaximizeHandle(object sender, RoutedEventArgs e)
    {
        PWindowMaximizeToggle();
    }

    private void PCaptionExitHandle(object sender, RoutedEventArgs e)
    {
        SystemCommands.CloseWindow(this);
    }

    private void PRoofHandle(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            PWindowMaximizeToggle();
            return;
        }

        if (WindowState == WindowState.Maximized)
        {
            PWindowPointerRestore(e);
        }

        DragMove();
    }

    private void PWindowMaximizeToggle()
    {
        if (WindowState == WindowState.Maximized)
        {
            SystemCommands.RestoreWindow(this);
        }
        else
        {
            SystemCommands.MaximizeWindow(this);
        }
    }

    private void PWindowPointerRestore(MouseButtonEventArgs e)
    {
        Point pointerInWindow = e.GetPosition(this);
        Point pointerOnScreen = PointToScreen(pointerInWindow);
        PresentationSource? source = PresentationSource.FromVisual(this);

        if (source?.CompositionTarget is not null)
        {
            pointerOnScreen = source.CompositionTarget.TransformFromDevice.Transform(pointerOnScreen);
        }

        double restoredWidth = RestoreBounds.Width;
        double horizontalRatio = ActualWidth > 0
            ? Math.Clamp(pointerInWindow.X / ActualWidth, 0, 1)
            : 0.5;

        SystemCommands.RestoreWindow(this);
        Left = pointerOnScreen.X - (restoredWidth * horizontalRatio);
        Top = pointerOnScreen.Y - Math.Min(pointerInWindow.Y, PRoof.ActualHeight / 2);
    }
}
