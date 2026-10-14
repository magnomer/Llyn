using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CDeckRoll(
    IReadOnlyList<CDeckFolder> CDeckRollFolders,
    IReadOnlyList<CDeckRow> CDeckRollDecks,
    bool CDeckRollEmpty);
