using Xunit;

namespace Llyn.Tests;

public sealed class TCoinage
{
    [Fact]
    public void CoinageWordingCheck_BlankWording_IsFalse()
    {
        Assert.False(TInterfaceConductDialog.TCoinageWordingCheck(null));
        Assert.False(TInterfaceConductDialog.TCoinageWordingCheck(" \t "));
        Assert.True(TInterfaceConductDialog.TCoinageWordingCheck(" tag "));
    }
}
