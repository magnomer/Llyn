using System.Collections.Generic;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed record CContour(string CContourText, IReadOnlyList<int> CContourLevels, bool CContourToned)
{
    public const int CContourFloor = LPhonologyPort.LEngineContourFloor;

    public const int CContourCeiling = LPhonologyPort.LEngineContourCeiling;
}
