using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CScoreboardCensus(
    string CScoreboardCensusTotal,
    string CScoreboardCensusActive,
    string CScoreboardCensusExcluded,
    string CScoreboardCensusOrphaned,
    string CScoreboardCensusFlagged,
    string CScoreboardCensusSynced,
    IReadOnlyList<CScoreboardWeek> CScoreboardCensusWeeks,
    string? CScoreboardCensusEmpty);
