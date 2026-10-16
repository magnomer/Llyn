using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CStudyRoll(
    IReadOnlyList<CDeckFolder> CStudyRollFolders,
    IReadOnlyList<CStudyRow> CStudyRollDecks,
    string? CStudyRollEmpty,
    CSittingState? CStudyRollSitting);
