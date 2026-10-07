using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Conduct;

internal static class CContourInk
{
    internal static CContour CContourInkBuild(LContour syllable, IReadOnlyList<int> scale)
    {
        ArgumentNullException.ThrowIfNull(syllable);
        ArgumentNullException.ThrowIfNull(scale);

        List<(int CContourInkLevel, LContourRole CContourInkRole)> kept = syllable.LContourLevels
            .Zip(syllable.LContourRoles)
            .Where(pair => scale.Contains(pair.First))
            .ToList();
        return new CContour(
            syllable.LContourText,
            kept.Select(static pair => pair.CContourInkLevel).ToList(),
            kept.Select(static pair => CContourInkRead(pair.CContourInkRole)).ToList(),
            syllable.LContourToned && kept.Count > 0);
    }

    private static string CContourInkRead(LContourRole role)
    {
        return role switch
        {
            LContourRole.LContourRoleTop => "Theme.Contour.Top",
            LContourRole.LContourRoleHigh => "Theme.Contour.High",
            LContourRole.LContourRoleMid => "Theme.Contour.Mid",
            LContourRole.LContourRoleLow => "Theme.Contour.Low",
            LContourRole.LContourRoleBottom => "Theme.Contour.Bottom",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null),
        };
    }
}
