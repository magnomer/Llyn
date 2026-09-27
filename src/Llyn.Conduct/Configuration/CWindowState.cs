namespace Llyn.Conduct;

public sealed record CWindowState(
    double CWindowStateLeft,
    double CWindowStateTop,
    double CWindowStateWidth,
    double CWindowStateHeight,
    bool CWindowStateMaximized);
