using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CNavigationState(
    string? CNavigationStateTab,
    IReadOnlyList<string> CNavigationStateHidden,
    CVoyageState CNavigationStateVoyage);
