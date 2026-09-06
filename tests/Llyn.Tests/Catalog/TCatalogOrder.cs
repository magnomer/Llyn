using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TCatalogOrder
{
    [Fact]
    public void CatalogOrderParse_FormattedChoice_ReturnsSameChoice()
    {
        foreach (LCatalogOrder order in Enum.GetValues<LCatalogOrder>())
        {
            Assert.Equal(
                order,
                TInterface.TCatalogOrderParse(
                    TInterface.TCatalogOrderFormat(order),
                    LCatalogOrder.LCatalogOrderUsage));
        }
    }

    [Fact]
    public void CatalogOrderParse_UnwrittenChoice_ReturnsFallback()
    {
        Assert.Equal(
            LCatalogOrder.LCatalogOrderText,
            TInterface.TCatalogOrderParse("Whatever", LCatalogOrder.LCatalogOrderText));
        Assert.Equal(
            LCatalogOrder.LCatalogOrderText,
            TInterface.TCatalogOrderParse(null, LCatalogOrder.LCatalogOrderText));
    }
}
