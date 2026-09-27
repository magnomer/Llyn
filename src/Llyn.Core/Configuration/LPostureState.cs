using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LPostureState(
    IReadOnlyList<LLayout>? LPostureStateLayout = null,
    string? LPostureStateMode = null,
    bool LPostureStateSplit = false,
    double LPostureStateVolume = 1);
