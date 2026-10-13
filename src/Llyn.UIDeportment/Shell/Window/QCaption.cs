using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Llyn.UIDeportment;

public sealed class QCaption
{
    private readonly Window _qCaptionWindow;

    public QCaption(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        _qCaptionWindow = window;
    }

    private Grid QRoof => (Grid)_qCaptionWindow.FindName("PRoof");

    private Button QCaptionMinimize => (Button)_qCaptionWindow.FindName("PCaptionMinimize");

    private Button QCaptionMaximize => (Button)_qCaptionWindow.FindName("PCaptionMaximize");

    private Button QCaptionExit => (Button)_qCaptionWindow.FindName("PCaptionExit");

    public void QCaptionIntroduce()
    {
        ((Shape)QCaptionMinimize.Content).SetBinding(
            Shape.StrokeProperty, new Binding(nameof(Control.Foreground)) { Source = QCaptionMinimize });
        ((Shape)QCaptionMaximize.Content).SetBinding(
            Shape.StrokeProperty, new Binding(nameof(Control.Foreground)) { Source = QCaptionMaximize });
        ((Shape)QCaptionExit.Content).SetBinding(
            Shape.StrokeProperty, new Binding(nameof(Control.Foreground)) { Source = QCaptionExit });

        QRoof.MouseLeftButtonDown += QRoofRefine;
        QCaptionMinimize.Click += QCaptionMinimizeRefine;
        QCaptionMaximize.Click += QCaptionMaximizeRefine;
        QCaptionExit.Click += QCaptionExitRefine;
    }

    private void QCaptionMinimizeRefine(object sender, RoutedEventArgs e)
    {
        SystemCommands.MinimizeWindow(_qCaptionWindow);
    }

    private void QCaptionMaximizeRefine(object sender, RoutedEventArgs e)
    {
        if (_qCaptionWindow.WindowState == WindowState.Maximized)
        {
            SystemCommands.RestoreWindow(_qCaptionWindow);
        }
        else
        {
            SystemCommands.MaximizeWindow(_qCaptionWindow);
        }
    }

    private void QCaptionExitRefine(object sender, RoutedEventArgs e)
    {
        SystemCommands.CloseWindow(_qCaptionWindow);
    }

    private void QRoofRefine(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            QCaptionMaximizeRefine(sender, e);
            return;
        }

        if (_qCaptionWindow.WindowState == WindowState.Maximized)
        {
            QCaptionPointerRefine(e);
        }

        _qCaptionWindow.DragMove();
    }

    private void QCaptionPointerRefine(MouseButtonEventArgs e)
    {
        Window window = _qCaptionWindow;
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
        window.Top = pointerOnScreen.Y - Math.Min(pointerInWindow.Y, QRoof.ActualHeight / 2);
    }
}
