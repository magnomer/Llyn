using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CSoundingFanqie(
    IReadOnlyList<CFanqieGroup> CSoundingFanqieGroups,
    bool CSoundingFanqiePending,
    bool CSoundingFanqieRebuildable,
    CFont CSoundingFanqieFont);
