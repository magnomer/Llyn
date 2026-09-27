using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TXieshengRows
{
    [Fact]
    public void GroveBuild_StoredStem_CarriesKeyCountAndMark()
    {
        CStem row = Assert.Single(TInterfaceGate.TXieshengGroveBuild(
            [TInterfaceGate.TStemCreate(7, "Middle Chinese", "工", 3, true)]));

        Assert.Equal(new CStem(7, "工", 3, true), row);
    }

    [Fact]
    public void PageBuild_BlankPage_CarriesNoCharacter()
    {
        CStemPage page = TInterfaceGate.TXieshengPageBuild(TInterfaceGate.TStemPageBlank);

        Assert.Empty(page.CStemPageKey);
        Assert.Empty(page.CStemPageLanguage);
        Assert.Empty(page.CStemPageCharacters);
        Assert.True(page.CStemPageEmpty);
    }

    [Fact]
    public void PageBuild_SeriesPage_KeepsCharacterOrder()
    {
        CStemPage page = TInterfaceGate.TXieshengPageBuild(
            TInterfaceGate.TStemPageCreate("Middle Chinese", "工", ["江", "紅", "空"]));

        Assert.Equal("Middle Chinese", page.CStemPageLanguage);
        Assert.Equal("工", page.CStemPageKey);
        Assert.Equal(["江", "紅", "空"], page.CStemPageCharacters);
        Assert.False(page.CStemPageEmpty);
    }
}
