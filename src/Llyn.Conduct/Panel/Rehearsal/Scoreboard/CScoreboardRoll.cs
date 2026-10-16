using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CScoreboardRoll(
    IReadOnlyList<CDeckFolder> CScoreboardRollFolders,
    IReadOnlyList<CScoreboardRow> CScoreboardRollDecks);
