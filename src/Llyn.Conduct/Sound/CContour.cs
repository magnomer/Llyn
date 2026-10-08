using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CContour(
    string CContourText,
    IReadOnlyList<int> CContourLevels,
    IReadOnlyList<string> CContourKeys,
    bool CContourToned);
