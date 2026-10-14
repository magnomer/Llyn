using System;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TContourPlot
{
    private static readonly int[] TContourPlotScale = [1, 2, 3, 4, 5];

    [Fact]
    public void ContourOriginRead_NarrowSlot_ReturnsLeftOverhang()
    {
        QContourPlot plot = new(TContourPlotScale, 2);

        double origin = plot.QContourOriginRead(100);

        Assert.Equal(-54, origin);
        Assert.Equal(50, origin + plot.QContourPlotSize.Width / 2);
    }

    [Fact]
    public void ContourOriginRead_WideSlot_ReturnsCentredInset()
    {
        QContourPlot plot = new(TContourPlotScale, 2);

        double origin = plot.QContourOriginRead(308);

        Assert.Equal(50, origin);
        Assert.Equal(154, origin + plot.QContourPlotSize.Width / 2);
    }

    [Fact]
    public void ContourOriginRead_OddRemainder_ReturnsWholePixel()
    {
        QContourPlot plot = new(TContourPlotScale, 2);

        double origin = plot.QContourOriginRead(101);

        Assert.Equal(origin, Math.Round(origin));
    }
}
