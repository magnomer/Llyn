using System;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public static class QCaption
{
    public static void QCaptionMaximizeRefine(Window window)
    {
        if (window.WindowState == WindowState.Maximized)
        {
            SystemCommands.RestoreWindow(window);
        }
        else
        {
            SystemCommands.MaximizeWindow(window);
        }
    }

    public static void QCaptionDragRefine(Window window, FrameworkElement roof, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            QCaptionMaximizeRefine(window);
            return;
        }

        if (window.WindowState == WindowState.Maximized)
        {
            QCaptionPointerRefine(window, roof, e);
        }

        window.DragMove();
    }

    private static void QCaptionPointerRefine(Window window, FrameworkElement roof, MouseButtonEventArgs e)
    {
        Point pointerInWindow = e.GetPosition(window);
        Point pointerOnScreen = window.PointToScreen(pointerInWindow);
        PresentationSource? source = PresentationSource.FromVisual(window);

        if (source?.CompositionTarget is not null)
        {
            pointerOnScreen = source.CompositionTarget.TransformFromDevice.Transform(pointerOnScreen);
        }

        double restoredWidth = window.RestoreBounds.Width;
        double horizontalRatio = window.ActualWidth > 0
            ? Math.Clamp(pointerInWindow.X / window.ActualWidth, 0, 1)
            : 0.5;

        SystemCommands.RestoreWindow(window);
        window.Left = pointerOnScreen.X - (restoredWidth * horizontalRatio);
        window.Top = pointerOnScreen.Y - Math.Min(pointerInWindow.Y, roof.ActualHeight / 2);
    }
}
