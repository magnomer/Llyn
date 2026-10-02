using System;
using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

public sealed class QRail : Panel
{
    private const double QRailGap = 12;

    private bool _qRailFold;

    protected override Size MeasureOverride(Size available)
    {
        Size room = new(double.PositiveInfinity, double.PositiveInfinity);

        double line = 0;
        double stack = 0;
        double widest = 0;
        double tallest = 0;

        foreach (UIElement child in InternalChildren)
        {
            child.Measure(room);

            if (child.Visibility == Visibility.Collapsed)
            {
                continue;
            }

            line += line > 0 ? QRailGap + child.DesiredSize.Width : child.DesiredSize.Width;
            stack += stack > 0 ? QRailGap + child.DesiredSize.Height : child.DesiredSize.Height;
            widest = Math.Max(widest, child.DesiredSize.Width);
            tallest = Math.Max(tallest, child.DesiredSize.Height);
        }

        _qRailFold = double.IsFinite(available.Width) && line > available.Width;

        return _qRailFold ? new Size(widest, stack) : new Size(line, tallest);
    }

    protected override Size ArrangeOverride(Size room)
    {
        double top = 0;
        double edge = room.Width;
        bool lead = true;

        foreach (UIElement child in InternalChildren)
        {
            if (child.Visibility == Visibility.Collapsed)
            {
                continue;
            }

            Size size = child.DesiredSize;

            if (_qRailFold)
            {
                child.Arrange(new Rect(0, top, size.Width, size.Height));
                top += size.Height + QRailGap;
                continue;
            }

            if (lead)
            {
                child.Arrange(new Rect(0, 0, size.Width, size.Height));
                lead = false;
                continue;
            }

            edge -= size.Width;
            child.Arrange(new Rect(Math.Max(0, edge), 0, size.Width, size.Height));
            edge -= QRailGap;
        }

        return room;
    }
}
