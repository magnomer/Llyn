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

    [Fact]
    public void SentenceOrderFormat_ParticleLeading_WrapsTheFrameInParentheses()
    {
        LSentenceOrder leading = TInterface.TSentenceOrderCreate(0, 1);
        LSentenceOrder trailing = TInterface.TSentenceOrderCreate(1, 0);

        Assert.True(leading.LSentenceOrderLeading);
        Assert.Equal("(+to sth)", leading.TSentenceOrderFormat("to", "sth"));
        Assert.Equal("(+sth to)", trailing.TSentenceOrderFormat("to", "sth"));
        Assert.Equal("(+to)", leading.TSentenceOrderFormat("to", string.Empty));
        Assert.Equal(string.Empty, leading.TSentenceOrderFormat(string.Empty, string.Empty));
        Assert.Equal("(+to sth) text", leading.TSentenceOrderFormat("to", "sth", "text"));
        Assert.Equal("text", leading.TSentenceOrderFormat(string.Empty, string.Empty, "text"));
    }
}
