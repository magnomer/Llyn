using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TSentenceOrder
{
    [Fact]
    public void SentenceOrderLoad_English_WritesTheMarkerFirst()
    {
        LSentenceOrder order = TInterface.TSentenceOrderLoad("English");

        Assert.Equal(0, order.LSentenceOrderParticle);
        Assert.Equal(1, order.LSentenceOrderDependence);
    }

    [Fact]
    public void SentenceOrderLoad_Japanese_WritesTheRoleFirst()
    {
        LSentenceOrder order = TInterface.TSentenceOrderLoad("Japanese");

        Assert.Equal(1, order.LSentenceOrderParticle);
        Assert.Equal(0, order.LSentenceOrderDependence);
    }

    [Fact]
    public void SentenceOrderLoad_LanguageWithNoPack_WritesTheMarkerFirst()
    {
        LSentenceOrder order = TInterface.TSentenceOrderLoad("Klingon");

        Assert.Equal(0, order.LSentenceOrderParticle);
        Assert.Equal(1, order.LSentenceOrderDependence);
    }
}
