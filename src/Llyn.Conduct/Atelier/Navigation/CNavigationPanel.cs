using System;

namespace Llyn.Conduct;

internal sealed record CNavigationPanel(
    Func<bool> CNavigationPanelLeave,
    Func<long> CNavigationPanelStation,
    Action<bool> CNavigationPanelScribe,
    Action<long> CNavigationPanelArrival,
    Func<bool>? CNavigationPanelAllowed);
