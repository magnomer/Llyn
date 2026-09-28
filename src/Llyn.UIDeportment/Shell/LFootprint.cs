using System;
using Llyn.Conduct;
using Llyn.UIDeportment.Capsule;

namespace Llyn.UIDeportment;

public sealed class LFootprint
{
    private const double LFootprintShare = 0.8;

    private const int LFootprintDelay = 700;

    private readonly QPosture _lFootprintPosture;

    public LFootprint(QPosture posture)
    {
        ArgumentNullException.ThrowIfNull(posture);

        _lFootprintPosture = posture;
    }

    public LCapsuleWindow? LFootprintRead(
        double left, double top, double width, double height, double minWidth, double minHeight)
    {
        return LFootprintPlace(
            _lFootprintPosture.QPostureRead().LCapsuleContentWindow,
            left,
            top,
            width,
            height,
            minWidth,
            minHeight);
    }

    public (double LFootprintWidth, double LFootprintHeight) LFootprintSizeRead(
        double width, double height, double minWidth, double minHeight)
    {
        return (
            Math.Clamp(width * LFootprintShare, minWidth, Math.Max(minWidth, width)),
            Math.Clamp(height * LFootprintShare, minHeight, Math.Max(minHeight, height)));
    }

    private static LCapsuleWindow? LFootprintPlace(
        LCapsuleWindow? state, double left, double top, double width, double height, double minWidth, double minHeight)
    {
        if (state is null)
        {
            return null;
        }

        return LFootprintClamp(
            state,
            left,
            top,
            width,
            height,
            Math.Clamp(state.LCapsuleWindowWidth, minWidth, Math.Max(minWidth, width)),
            Math.Clamp(state.LCapsuleWindowHeight, minHeight, Math.Max(minHeight, height)));
    }

    private static LCapsuleWindow LFootprintClamp(
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

    public void LFootprintDefer(LCapsuleWindow state, bool minimized, bool closing)
    {
        ArgumentNullException.ThrowIfNull(state);

        if (!(state.LCapsuleWindowWidth > 0) || !(state.LCapsuleWindowHeight > 0))
        {
            return;
        }

        _lFootprintPosture.QPostureWindowDefer(state, minimized, closing ? 0 : LFootprintDelay);
    }
}
