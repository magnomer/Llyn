using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public static class QContourInk
{
    public static IReadOnlyList<QContourItem> QContourInkBuild(
        IReadOnlyList<CContour> syllables, FrameworkElement scope)
    {
        ArgumentNullException.ThrowIfNull(syllables);
        ArgumentNullException.ThrowIfNull(scope);

        return syllables
            .Select(syllable => new QContourItem(
                syllable.CContourText,
                syllable.CContourLevels,
                syllable.CContourKeys
                    .Select(key => scope.TryFindResource(key) as Brush ?? Brushes.Gray)
                    .ToList(),
                syllable.CContourToned))
            .ToList();
    }
}
