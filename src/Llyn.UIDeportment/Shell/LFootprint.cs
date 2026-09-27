using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class LFootprint
{
    private const double LFootprintShare = 0.8;

    private const int LFootprintDelay = 700;

    private readonly LWindow _lWindow;

    public LFootprint(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        _lWindow = window;
    }

    public CWindowState? LFootprintRead(
        double left, double top, double width, double height, double minWidth, double minHeight)
    {
        return LFootprintPlace(
            _lWindow.LWindowPostureRead().CPostureStateWindow, left, top, width, height, minWidth, minHeight);
    }

    public (double LFootprintWidth, double LFootprintHeight) LFootprintSizeRead(
        double width, double height, double minWidth, double minHeight)
    {
        return (
            Math.Clamp(width * LFootprintShare, minWidth, Math.Max(minWidth, width)),
            Math.Clamp(height * LFootprintShare, minHeight, Math.Max(minHeight, height)));
    }

    public void LFootprintSave(CWindowState state, bool minimized)
    {
        LFootprintDefer(state, minimized, LFootprintDelay);
    }

    public void LFootprintClose(CWindowState state, bool minimized)
    {
        LFootprintDefer(state, minimized, 0);
    }

    private static CWindowState? LFootprintPlace(
        CWindowState? state, double left, double top, double width, double height, double minWidth, double minHeight)
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
            Math.Clamp(state.CWindowStateWidth, minWidth, Math.Max(minWidth, width)),
            Math.Clamp(state.CWindowStateHeight, minHeight, Math.Max(minHeight, height)));
    }

    private static CWindowState LFootprintClamp(
        CWindowState state,
        double left,
        double top,
        double width,
        double height,
        double placedWidth,
        double placedHeight)
    {
        return new CWindowState(
            Math.Clamp(state.CWindowStateLeft, left, Math.Max(left, left + width - placedWidth)),
            Math.Clamp(state.CWindowStateTop, top, Math.Max(top, top + height - placedHeight)),
            placedWidth,
            placedHeight,
            state.CWindowStateMaximized);
    }

    private void LFootprintDefer(CWindowState state, bool minimized, int delay)
    {
        ArgumentNullException.ThrowIfNull(state);

        if (!(state.CWindowStateWidth > 0) || !(state.CWindowStateHeight > 0))
        {
            return;
        }

        _lWindow.LWindowStateDefer(state, minimized, delay);
    }

    internal static CPostureState LFootprintPostureRead(LPostureState state)
    {
        return new CPostureState(
            LFootprintStateRead(state.LPostureStateWindow),
            state.LPostureStateLinked,
            state.LPostureStateSplit,
            state.LPostureStateVolume);
    }

    internal static CWindowState? LFootprintStateRead(LWindowState? window)
    {
        return window is null
            ? null
            : new CWindowState(
                window.LWindowStateLeft,
                window.LWindowStateTop,
                window.LWindowStateWidth,
                window.LWindowStateHeight,
                window.LWindowStateMaximized);
    }

    internal static LWindowState LFootprintStateRead(CWindowState window)
    {
        ArgumentNullException.ThrowIfNull(window);

        return new LWindowState(
            window.CWindowStateLeft,
            window.CWindowStateTop,
            window.CWindowStateWidth,
            window.CWindowStateHeight,
            window.CWindowStateMaximized);
    }

    internal static CLayout? LFootprintLayoutRead(IReadOnlyList<LLayout> layout, string tab)
    {
        foreach (LLayout record in layout)
        {
            if (record.LLayoutTabMatch(tab))
            {
                return new CLayout(record.LLayoutTab, record.LLayoutLeft, record.LLayoutMiddle);
            }
        }

        return null;
    }

    internal static List<LLayout> LFootprintLayoutRead(IEnumerable<CLayout> layout)
    {
        ArgumentNullException.ThrowIfNull(layout);

        List<LLayout> saved = [];
        foreach (CLayout tab in layout)
        {
            saved.Add(new LLayout(tab.CLayoutTab, tab.CLayoutLeft, tab.CLayoutMiddle));
        }

        return saved;
    }
}
