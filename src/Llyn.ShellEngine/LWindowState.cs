namespace Llyn.ShellEngine;

public sealed record LWindowState(
    double LWindowStateLeft,
    double LWindowStateTop,
    double LWindowStateWidth,
    double LWindowStateHeight,
    bool LWindowStateMaximized);
