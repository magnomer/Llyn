using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TSentenceOrder
{
    [Theory]
    [InlineData("English", 0, 1)]
    [InlineData("Japanese", 1, 0)]
    public void SentenceOrderLoad_PackLanguage_WritesPackOrder(string language, int particle, int dependence)
    {
        LSentenceOrder order = TInterface.TSentenceOrderLoad(language);

        Assert.Equal(particle, order.LSentenceOrderParticle);
        Assert.Equal(dependence, order.LSentenceOrderDependence);
    }

    [Fact]
    public void SentenceOrderLoad_LanguageWithNoPack_WritesTheMarkerFirst()
    {
        LSentenceOrder order = TInterface.TSentenceOrderLoad("Klingon");

        Assert.Equal(0, order.LSentenceOrderParticle);
        Assert.Equal(1, order.LSentenceOrderDependence);
    }
}
