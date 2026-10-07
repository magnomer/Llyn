using System.Collections.Generic;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed record CContour(
    string CContourText,
    IReadOnlyList<int> CContourLevels,
    IReadOnlyList<string> CContourKeys,
    bool CContourToned)
{
    public static IReadOnlyList<int> CContourScale => LPhonologyPort.LEngineContourScale;
}
