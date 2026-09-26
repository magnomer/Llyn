using Xunit;

namespace Llyn.Tests;

public sealed class TCoinage
{
    [Fact]
    public void CoinageWordingCheck_BlankWording_IsFalse()
    {
        Assert.False(TInterfaceConduct.TCoinageWordingCheck(null));
        Assert.False(TInterfaceConduct.TCoinageWordingCheck(" \t "));
        Assert.True(TInterfaceConduct.TCoinageWordingCheck(" tag "));
    }
}
