using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CLecternFanqie(
    IReadOnlyList<CFanqieGroup> CLecternFanqieGroups,
    bool CLecternFanqiePending,
    string CLecternFanqieReading,
    CFont CLecternFanqieFont);
