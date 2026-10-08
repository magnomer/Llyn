using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Llyn.UIDeportment;

internal sealed class QContourPlot
{
    internal const double QContourCellWidth = 76;

    private const double QContourPadding = 14;

    private const double QContourAxisWidth = 18;

    private const double QContourCellGap = 10;

    private const double QContourCellInset = 12;

    private const double QContourLevelGap = 18;

    private const double QContourLabelGap = 8;

    private const double QContourLabelHeight = 24;

    private readonly IReadOnlyList<int> _qContourPlotScale;

    private readonly int _qContourPlotCount;

    internal QContourPlot(IReadOnlyList<int> scale, int count)
    {
        _qContourPlotScale = scale;
        _qContourPlotCount = count;
    }

    internal Size QContourPlotSize
    {
        get
        {
            int count = Math.Max(1, _qContourPlotCount);
            double width = 2 * QContourPadding + QContourAxisWidth
                + count * QContourCellWidth + (count - 1) * QContourCellGap;
            double height = 2 * QContourPadding + QContourPlotHeight + QContourLabelGap + QContourLabelHeight;
            return new Size(width, height);
        }
    }

    internal double QContourLabelTop => QContourPadding + QContourPlotHeight + QContourLabelGap;

    private double QContourPlotHeight => Math.Max(0, _qContourPlotScale.Count - 1) * QContourLevelGap;

    internal double QContourLeftRead(int index)
    {
        return QContourPadding + QContourAxisWidth + index * (QContourCellWidth + QContourCellGap);
    }

    internal double QContourRightRead(double width)
    {
        return width - QContourPadding;
    }

    internal double QContourLevelResolve(int level)
    {
        return QContourPadding + QContourDepthRead(level) * QContourLevelGap;
    }

    internal IReadOnlyList<Point> QContourPointResolve(IReadOnlyList<int> levels, int index)
    {
        double start = QContourLeftRead(index) + QContourCellInset;
        double span = QContourCellWidth - 2 * QContourCellInset;
        List<Point> points = [];
        if (levels.Count == 1)
        {
            double y = QContourLevelResolve(levels[0]);
            points.Add(new Point(start, y));
            points.Add(new Point(start + span, y));
            return points;
        }

        for (int step = 0; step < levels.Count; step++)
        {
            double x = start + span * step / (levels.Count - 1);
            points.Add(new Point(x, QContourLevelResolve(levels[step])));
        }

        return points;
    }

    private int QContourDepthRead(int level)
    {
        return _qContourPlotScale.TakeWhile(step => step != level).Count();
    }
}
