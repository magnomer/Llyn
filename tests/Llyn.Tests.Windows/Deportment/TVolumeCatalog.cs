using System.Collections.Generic;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TVolumeCatalog
{
    [Fact]
    public void VolumeCatalogLevel_RepeatedLevel_RaisesOneNoticePerChange()
    {
        PVolumeCatalog catalog = new();
        List<double> heard = [];
        catalog.PropertyChanged += (_, _) => heard.Add(catalog.PVolumeCatalogLevel);

        catalog.PVolumeCatalogLevel = 0.4;
        catalog.PVolumeCatalogLevel = 0.4;
        catalog.PVolumeCatalogLevel = 1;
        catalog.PVolumeCatalogLevel = 1;

        Assert.Equal(new[] { 0.4, 1.0 }, heard);
        Assert.Equal(1, catalog.PVolumeCatalogLevel);
    }

    [Theory]
    [InlineData(-0.5, 0)]
    [InlineData(1.5, 1)]
    [InlineData(0.25, 0.25)]
    public void VolumeCatalogLevel_OutOfRangeLevel_ClampsToTheNearestEnd(double asked, double held)
    {
        PVolumeCatalog catalog = new();

        catalog.PVolumeCatalogLevel = asked;

        Assert.Equal(held, catalog.PVolumeCatalogLevel);
    }
}
