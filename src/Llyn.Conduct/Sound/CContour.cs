using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed record CContour(
    string CContourText,
    IReadOnlyList<int> CContourLevels,
    IReadOnlyList<string> CContourKeys,
    bool CContourToned)
{
    internal static IReadOnlyList<CContour> CContourRead(
        IReadOnlyList<LContour> syllables, IReadOnlyList<int> scale)
    {
        ArgumentNullException.ThrowIfNull(syllables);
        ArgumentNullException.ThrowIfNull(scale);

        return syllables
            .Select(syllable => CContourInk.CContourInkBuild(syllable, scale))
            .ToList();
    }
}
