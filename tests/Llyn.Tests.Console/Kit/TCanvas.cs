using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TCanvas
{
    [Fact]
    public void CanvasFrameDraw_WideCharacterAtEdge_LeavesOneBlankCell()
    {
        TConsoleFake console = new() { TConsoleFakeWidth = 4, TConsoleFakeHeight = 1 };
        BCanvas canvas = new(console, new BCanvasWidth(false), new BTheme(BTheme.BThemeSpareRead), 4, 1);

        canvas.BCanvasFrameDraw([new(0, 0, 4, "xxxx", "text"), new(0, 0, 3, "ab水", "text")]);
        canvas.BCanvasSync();

        Assert.Equal(["ab x"], console.TConsoleFakeGrid);
    }

    [Fact]
    public void CanvasFrameDraw_HalfOfWideCharacter_BlanksTheOtherHalf()
    {
        TConsoleFake console = new() { TConsoleFakeWidth = 4, TConsoleFakeHeight = 1 };
        BCanvas canvas = new(console, new BCanvasWidth(false), new BTheme(BTheme.BThemeSpareRead), 4, 1);

        canvas.BCanvasFrameDraw([new(0, 0, 4, "水水", "text"), new(0, 1, 2, "ab", "text")]);
        canvas.BCanvasSync();

        Assert.Equal([" ab "], console.TConsoleFakeGrid);
    }

    [Fact]
    public void CanvasSync_OneCellChanged_WritesThatRunOnly()
    {
        TConsoleFake console = new() { TConsoleFakeWidth = 4, TConsoleFakeHeight = 2 };
        BCanvas canvas = new(console, new BCanvasWidth(false), new BTheme(BTheme.BThemeSpareRead), 4, 2);
        canvas.BCanvasFrameDraw([new(0, 0, 4, "abcd", "text")]);
        canvas.BCanvasSync();

        canvas.BCanvasFrameDraw([new(0, 0, 4, "abcd", "text"), new(1, 2, 1, "x", "text")]);
        canvas.BCanvasSync();
        canvas.BCanvasSync();

        Assert.Equal(
            ["\e[1;1H\e[96;44mabcd\e[2;1H    \e[0m\e[?25l", "\e[2;3H\e[96;44mx\e[0m"], console.TConsoleFakeFrames);
        Assert.Equal(["abcd", "  x "], console.TConsoleFakeGrid);
    }

    [Fact]
    public void CanvasSync_CaretMark_ShowsCursorAfterFrame()
    {
        TConsoleFake console = new() { TConsoleFakeWidth = 4, TConsoleFakeHeight = 1 };
        BCanvas canvas = new(console, new BCanvasWidth(false), new BTheme(BTheme.BThemeSpareRead), 4, 1);
        Action<string, int> sync = (text, column) =>
        {
            canvas.BCanvasFrameDraw([new(0, 0, 4, text, "text"), new(0, column, 0, string.Empty, "caret")]);
            canvas.BCanvasSync();
        };

        sync("ab", 2);
        sync("ab", 2);
        sync("ab", 1);
        sync("abc", 3);

        Assert.Equal(
            [
                "\e[1;1H\e[96;44mab  \e[0m\e[1;3H\e[?25h",
                "\e[1;2H\e[?25h",
                "\e[?25l\e[1;3H\e[96;44mc\e[0m\e[1;4H\e[?25h",
            ],
            console.TConsoleFakeFrames);
    }

    [Fact]
    public void CanvasSync_NoCaretMark_HidesCursor()
    {
        TConsoleFake console = new() { TConsoleFakeWidth = 4, TConsoleFakeHeight = 1 };
        BCanvas canvas = new(console, new BCanvasWidth(false), new BTheme(BTheme.BThemeSpareRead), 4, 1);
        canvas.BCanvasFrameDraw([new(0, 0, 4, "ab", "text"), new(0, 9, 0, string.Empty, "caret")]);
        canvas.BCanvasSync();
        canvas.BCanvasFrameDraw([new(0, 0, 4, "ab", "text"), new(0, 2, 0, string.Empty, "caret")]);
        canvas.BCanvasSync();

        canvas.BCanvasFrameDraw([new(0, 0, 4, "ab", "text")]);
        canvas.BCanvasSync();
        canvas.BCanvasSync();

        Assert.EndsWith("\e[0m\e[?25l", console.TConsoleFakeFrames[0], StringComparison.Ordinal);
        Assert.Equal(["\e[1;3H\e[?25h", "\e[?25l"], console.TConsoleFakeFrames[1..]);
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
