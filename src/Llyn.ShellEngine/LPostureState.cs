using System.Collections.Generic;

namespace Llyn.ShellEngine;

public sealed record LPostureState(
    LWindowState? LPostureStateWindow = null,
    IReadOnlyList<LLayout>? LPostureStateLayout = null,
    bool LPostureStateLinked = true,
    string? LPostureStateMode = null,
    bool LPostureStateSplit = false,
    double LPostureStateVolume = 1);
