using System.Collections.Generic;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TVolumeCatalog
{
    [Fact]
    public void VolumeCatalogLevel_RepeatedLevel_RaisesNoticePerChange()
    {
        QVolumeCatalog catalog = new();
        List<double> heard = [];
        catalog.PropertyChanged += (_, _) => heard.Add(catalog.QVolumeCatalogLevel);

        catalog.QVolumeCatalogLevel = 0.4;
        catalog.QVolumeCatalogLevel = 0.4;
        catalog.QVolumeCatalogLevel = 1;
        catalog.QVolumeCatalogLevel = 1;

        Assert.Equal(new[] { 0.4, 1.0 }, heard);
        Assert.Equal(1, catalog.QVolumeCatalogLevel);
    }
}
