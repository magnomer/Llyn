using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TLantern
{
    [Fact]
    public void LanternMove_DownAndUp_WrapAround()
    {
        Assert.Equal(1, CLantern.CLanternMove(0, 3, 1));
        Assert.Equal(0, CLantern.CLanternMove(2, 3, 1));
        Assert.Equal(2, CLantern.CLanternMove(0, 3, -1));
        Assert.Equal(1, CLantern.CLanternMove(2, 3, -1));
    }

    [Fact]
    public void LanternMove_NoLitRow_StartsAtEitherEnd()
    {
        Assert.Equal(0, CLantern.CLanternMove(-1, 3, 1));
        Assert.Equal(2, CLantern.CLanternMove(-1, 3, -1));
    }

    [Fact]
    public void LanternMove_EmptyOffer_LightsNothing()
    {
        Assert.Null(CLantern.CLanternMove(-1, 0, 1));
        Assert.Null(CLantern.CLanternMove(0, 0, -1));
    }
}
