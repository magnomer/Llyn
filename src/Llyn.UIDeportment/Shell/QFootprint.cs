using System;
using Llyn.Conduct;
using Llyn.UIDeportment.Capsule;

namespace Llyn.UIDeportment;

public sealed class QFootprint
{
    private const double QFootprintShare = 0.8;

    private const int QFootprintDelay = 700;

    private readonly QPosture _qFootprintPosture;

    public QFootprint(QPosture posture)
    {
        ArgumentNullException.ThrowIfNull(posture);

        _qFootprintPosture = posture;
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
