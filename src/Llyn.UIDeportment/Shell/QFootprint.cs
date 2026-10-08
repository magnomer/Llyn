using System;
using System.Windows;
using Llyn.Conduct;
using Llyn.UIDeportment.Capsule;

namespace Llyn.UIDeportment;

public sealed class QFootprint
{
    private const double QFootprintShare = 0.8;

    private const int QFootprintDelay = 700;

    private readonly Window _qFootprintWindow;

    private readonly QPosture _qFootprintPosture;

    public QFootprint(Window window, QPosture posture)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(posture);

        _qFootprintWindow = window;
        _qFootprintPosture = posture;
    }

    public void QFootprintRefine()
    {
        Window window = _qFootprintWindow;
        if (QFootprintRead(
                SystemParameters.VirtualScreenLeft,
                SystemParameters.VirtualScreenTop,
                SystemParameters.VirtualScreenWidth,
                SystemParameters.VirtualScreenHeight,
                window.MinWidth,
                window.MinHeight) is not LCapsuleWindow state)
        {
            Rect area = SystemParameters.WorkArea;
            (window.Width, window.Height) =
                QFootprintSizeRead(area.Width, area.Height, window.MinWidth, window.MinHeight);
            return;
        }

        window.WindowStartupLocation = WindowStartupLocation.Manual;
        window.Left = state.LCapsuleWindowLeft;
        window.Top = state.LCapsuleWindowTop;
        window.Width = state.LCapsuleWindowWidth;
        window.Height = state.LCapsuleWindowHeight;

        if (state.LCapsuleWindowMaximized)
        {
            window.WindowState = WindowState.Maximized;
        }
    }

    public void QFootprintAttach()
    {
        _qFootprintWindow.LocationChanged += (_, _) => QFootprintSave(false);
        _qFootprintWindow.SizeChanged += (_, _) => QFootprintSave(false);
        _qFootprintWindow.StateChanged += (_, _) => QFootprintSave(false);
    }

    public void QFootprintSave(bool closing)
    {
        QFootprintDefer(QFootprintWindowRead(), _qFootprintWindow.WindowState == WindowState.Minimized, closing);
    }

    private LCapsuleWindow QFootprintWindowRead()
    {
        Window window = _qFootprintWindow;
        Rect bounds = window.WindowState == WindowState.Normal
            ? new Rect(window.Left, window.Top, window.Width, window.Height)
            : window.RestoreBounds;

        return new LCapsuleWindow(
            bounds.Left, bounds.Top, bounds.Width, bounds.Height, window.WindowState == WindowState.Maximized);
    }

    public LCapsuleWindow? QFootprintRead(
        double left, double top, double width, double height, double minWidth, double minHeight)
    {
        return QFootprintPlace(
            _qFootprintPosture.QPostureRead().LCapsuleContentWindow,
            left,
            top,
            width,
            height,
            minWidth,
            minHeight);
    }

    public (double QFootprintWidth, double QFootprintHeight) QFootprintSizeRead(
        double width, double height, double minWidth, double minHeight)
    {
        return (
            Math.Clamp(width * QFootprintShare, minWidth, Math.Max(minWidth, width)),
            Math.Clamp(height * QFootprintShare, minHeight, Math.Max(minHeight, height)));
    }

    private static LCapsuleWindow? QFootprintPlace(
        LCapsuleWindow? state, double left, double top, double width, double height, double minWidth, double minHeight)
    {
        if (state is null)
        {
            return null;
        }

        return QFootprintClamp(
            state,
            left,
            top,
            width,
            height,
            Math.Clamp(state.LCapsuleWindowWidth, minWidth, Math.Max(minWidth, width)),
            Math.Clamp(state.LCapsuleWindowHeight, minHeight, Math.Max(minHeight, height)));
    }

    private static LCapsuleWindow QFootprintClamp(
        LCapsuleWindow state,
        double left,
        double top,
        double width,
        double height,
        double placedWidth,
        double placedHeight)
    {
        return new LCapsuleWindow(
            Math.Clamp(state.LCapsuleWindowLeft, left, Math.Max(left, left + width - placedWidth)),
            Math.Clamp(state.LCapsuleWindowTop, top, Math.Max(top, top + height - placedHeight)),
            placedWidth,
            placedHeight,
            state.LCapsuleWindowMaximized);
    }

    public void QFootprintDefer(LCapsuleWindow state, bool minimized, bool closing)
    {
        ArgumentNullException.ThrowIfNull(state);

        if (!(state.LCapsuleWindowWidth > 0) || !(state.LCapsuleWindowHeight > 0))
        {
            return;
        }

        _qFootprintPosture.QPostureWindowDefer(state, minimized, closing ? 0 : QFootprintDelay);
    }
}
