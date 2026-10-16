using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TCueMarkup
{
    [Fact]
    public void CueMarkupFormat_BarInText_EscapesBar()
    {
        Assert.Equal("a\u2223b", TInterface.TCueMarkupFormat("a|b"));
    }

    [Fact]
    public void CueMarkupParse_BarInText_ReturnsLineBreak()
    {
        LCueFace face = TInterface.TCueFaceCreate(
            [TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "edge of a river|place for money")]);

        Assert.Equal("edge of a river\nplace for money", TInterface.TCueMarkupParse(face, 1).LCueSideText);
    }

    [Fact]
    public void CueMarkupParse_MediaCells_KeepsFilesApart()
    {
        LCueFace face = TInterface.TCueFaceCreate(
            [
                TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bank"),
                TInterface.TCueCellCreate(LCueColumn.LCueColumnPicture, 1, @"C:\Pictures\bank.png"),
                TInterface.TCueCellCreate(LCueColumn.LCueColumnSound, 1, "bank.mp3| |shore.mp3"),
            ]);

        LCueSide side = TInterface.TCueMarkupParse(face, 1);

        Assert.Equal("bank", side.LCueSideText);
        Assert.Equal([@"C:\Pictures\bank.png"], side.LCueSidePictures);
        Assert.Equal(["bank.mp3", "shore.mp3"], side.LCueSideSounds);
    }

    [Fact]
    public void CueMarkupParse_SecondSide_ReturnsSecondText()
    {
        LCueFace face = TInterface.TCueFaceCreate(
            [
                TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bank"),
                TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 2, "shore"),
                TInterface.TCueCellCreate(LCueColumn.LCueColumnCategory, 2, "noun"),
            ]);

        Assert.Equal("shore", TInterface.TCueMarkupParse(face, 2).LCueSideText);
    }
}
