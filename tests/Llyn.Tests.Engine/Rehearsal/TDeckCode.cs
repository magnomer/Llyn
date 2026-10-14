using Xunit;

namespace Llyn.Tests;

public sealed class TDeckCode
{
    [Fact]
    public void DeckCodeResolve_NameTaken_AppendsTwo()
    {
        Assert.Equal("French", TInterface.TDeckCodeResolve("French", ["Spanish"]));
        Assert.Equal("French 2", TInterface.TDeckCodeResolve("French", ["French"]));
        Assert.Equal("French 3", TInterface.TDeckCodeResolve("French", ["French", "French 2"]));
    }

    [Fact]
    public void DeckCodeResolve_TakenOtherCase_AppendsTwo()
    {
        Assert.Equal("French 2", TInterface.TDeckCodeResolve("French", ["FRENCH"]));
    }

    [Fact]
    public void DeckCodeCheck_NonAsciiOtherCase_ReturnsFalse()
    {
        Assert.False(TInterface.TDeckCodeCheck("Été", ["éTÉ"]));
        Assert.True(TInterface.TDeckCodeCheck("Été", ["Ete"]));
    }
}
