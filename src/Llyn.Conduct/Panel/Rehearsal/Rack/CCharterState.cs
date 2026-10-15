using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CCharterState(
    int CCharterStateOrder,
    int CCharterStateGoal,
    int CCharterStateSpaced,
    int CCharterStatePool,
    int CCharterStateStreak,
    int CCharterStateTarget,
    int? CCharterStateQuorum,
    int CCharterStateLead,
    bool CCharterStateBasic,
    string CCharterStateCode,
    IReadOnlyList<CCharterBearing> CCharterStateBearings);
