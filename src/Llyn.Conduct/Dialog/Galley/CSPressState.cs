namespace Llyn.Conduct;

public sealed record CSPressState(
    int CSPressStateCount,
    double CSPressStateWidth,
    double CSPressStateHeight,
    CSLensState CSPressStateLens,
    string? CSPressStateFailure,
    CSPressSetting CSPressStateSetting);
