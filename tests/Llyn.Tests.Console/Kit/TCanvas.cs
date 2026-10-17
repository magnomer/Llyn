using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TCanvas
{
    [Fact]
    public void CanvasFrameDraw_WideCharacterAtEdge_LeavesOneBlankCell()
    {
        TConsoleFake console = new() { TConsoleFakeWidth = 4, TConsoleFakeHeight = 1 };
        BCanvas canvas = new(console, new BCanvasWidth(false), 4, 1);

        canvas.BCanvasFrameDraw([new(0, 0, 4, "xxxx", "text"), new(0, 0, 3, "ab水", "text")]);
        canvas.BCanvasSync();

        Assert.Equal(["ab x"], console.TConsoleFakeGrid);
    }

    [Fact]
    public void CanvasFrameDraw_HalfOfWideCharacter_BlanksTheOtherHalf()
    {
        TConsoleFake console = new() { TConsoleFakeWidth = 4, TConsoleFakeHeight = 1 };
        BCanvas canvas = new(console, new BCanvasWidth(false), 4, 1);

        canvas.BCanvasFrameDraw([new(0, 0, 4, "水水", "text"), new(0, 1, 2, "ab", "text")]);
        canvas.BCanvasSync();

        Assert.Equal([" ab "], console.TConsoleFakeGrid);
    }

    [Fact]
    public void CanvasSync_OneCellChanged_WritesThatRunOnly()
    {
        TConsoleFake console = new() { TConsoleFakeWidth = 4, TConsoleFakeHeight = 2 };
        BCanvas canvas = new(console, new BCanvasWidth(false), 4, 2);
        canvas.BCanvasFrameDraw([new(0, 0, 4, "abcd", "text")]);
        canvas.BCanvasSync();

        canvas.BCanvasFrameDraw([new(0, 0, 4, "abcd", "text"), new(1, 2, 1, "x", "text")]);
        canvas.BCanvasSync();
        canvas.BCanvasSync();

        Assert.Equal(["\e[1;1Habcd\e[2;1H    ", "\e[2;3Hx"], console.TConsoleFakeFrames);
        Assert.Equal(["abcd", "  x "], console.TConsoleFakeGrid);
    }

    [Fact]
    public void CanvasWidthDivide_WideClusters_BreaksBetweenClusters()
    {
        BCanvasWidth width = new(false);

        IReadOnlyList<string> mixed = width.BCanvasWidthDivide("ab cd 水水水", 5);
        IReadOnlyList<string> joined = width.BCanvasWidthDivide("ééé", 2);

        Assert.Equal(["ab cd", "水水", "水"], mixed);
        Assert.Equal(["éé", "é"], joined);
    }
}
