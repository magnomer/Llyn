using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

public sealed class PGauge : Panel
{
    protected override Size MeasureOverride(Size available)
    {
        InternalChildren[0].Measure(available);
        InternalChildren[1].Measure(available);
        return InternalChildren[0].DesiredSize;
    }

    protected override Size ArrangeOverride(Size final)
    {
        InternalChildren[0].Arrange(new Rect(final));
        InternalChildren[1].Arrange(new Rect(0, 0, InternalChildren[1].DesiredSize.Width, final.Height));
        return final;
    }
}
