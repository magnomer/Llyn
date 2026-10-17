using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TWindow
{
    [Fact]
    public void WindowOpen_EightyByTwentyFour_DrawsEmptyFrame()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TInvocation.TInvocationRun(console, workspace);

        string top = $"╔{new string('═', 38)}╗";
        string body = $"║{new string(' ', 38)}║";
        string rule = $"╟{new string('─', 38)}╢";
        string bottom = $"╚{new string('═', 38)}╝";
        string[] frame =
        [
            new string(' ', 80),
            top + top,
            .. Enumerable.Repeat(body + body, 17),
            rule + rule,
            body + body,
            bottom + bottom,
            " Opening…".PadRight(80),
            new string(' ', 72) + "10Quit  ",
        ];
        Assert.Equal(frame, console.TConsoleFakeGrid);
    }

    [Fact]
    public void WindowSizeObserve_Grown_RedrawsFrameByHalves()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeAdjust(64, 18);
        console.TConsoleFakeAdjust(101, 30);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TInvocation.TInvocationRun(console, workspace);

        string top = $"╔{new string('═', 48)}╗╔{new string('═', 49)}╗";
        string body = $"║{new string(' ', 48)}║║{new string(' ', 49)}║";
        string rule = $"╟{new string('─', 48)}╢╟{new string('─', 49)}╢";
        string bottom = $"╚{new string('═', 48)}╝╚{new string('═', 49)}╝";
        string[] frame =
        [
            new string(' ', 101),
            top,
            .. Enumerable.Repeat(body, 23),
            rule,
            body,
            bottom,
            " Opening…".PadRight(101),
            (new string(' ', 72) + "10Quit").PadRight(101),
        ];
        Assert.Equal(frame, console.TConsoleFakeGrid);
    }

    [Fact]
    public void WindowSizeObserve_BelowEightyByTwentyFour_DrawsNotice()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeAdjust(64, 18);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TInvocation.TInvocationRun(console, workspace);

        string[] notice =
        [
            "Llyn needs a console of at least 80 by 24 cells. This one is 64".PadRight(64),
            "by 18. Make the window larger to go on.".PadRight(64),
            .. Enumerable.Repeat(new string(' ', 64), 16),
        ];
        Assert.Equal(notice, console.TConsoleFakeGrid);
    }

    [Fact]
    public void WindowSizeObserve_KoreanNotice_WrapsByCells()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeAdjust(21, 8);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TInvocation.TInvocationRun(
            console,
            "ko",
            workspace.TWorkspaceFolder,
            (_, _) => new LEngine(
                workspace.TWorkspaceRigCreate(), _ => workspace.TWorkspaceRigCreate(), static _ => { }, true),
            TInterfaceConduct.TAtelierCreate);

        string[] notice =
        [
            "Llyn에는 가로 80칸,  ",
            "세로 24칸 이상인     ",
            "콘솔이 필요합니다. 이",
            "콘솔은 가로 21칸,    ",
            "세로 8칸입니다. 창을 ",
            "키우면 이어집니다.   ",
            new string(' ', 21),
            new string(' ', 21),
        ];
        Assert.Equal(notice, console.TConsoleFakeGrid);
    }

    [Fact]
    public void WindowKeyObserve_FunctionTenUnderNotice_ExitsZero()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeAdjust(64, 18);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        int code = TInvocation.TInvocationRun(console, workspace);

        Assert.Equal(0, code);
        Assert.False(console.TConsoleFakeWindow);
        Assert.Empty(console.TConsoleFakeError);
    }

    [Fact]
    public void WindowBatchObserve_KeysWithoutChange_PrintsOneFrame()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        foreach (char letter in "abc")
        {
            console.TConsoleFakeKeys.Add(new ConsoleKeyInfo(letter, ConsoleKey.A, false, false, false));
        }

        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TInvocation.TInvocationRun(console, workspace);

        Assert.Single(console.TConsoleFakeFrames);
    }

    [Fact]
    public void WindowKeyObserve_FunctionTen_ExitsZero()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        int code = TInvocation.TInvocationRun(console, workspace);

        Assert.Equal(0, code);
        Assert.False(console.TConsoleFakeWindow);
        Assert.Empty(console.TConsoleFakeError);
    }

    [Fact]
    public void WindowKeyObserve_ControlC_ExitsZero()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\u0003', ConsoleKey.C, false, false, true));

        int code = TInvocation.TInvocationRun(console, workspace);

        Assert.Equal(0, code);
        Assert.False(console.TConsoleFakeWindow);
    }

    [Fact]
    public void BootstrapIntroduce_WindowWorkspaceBusy_ExitsThree()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };

        int code = TInvocation.TInvocationRun(console, "D:\\Busy", (_, _) => throw new IOException("locked"));

        Assert.Equal(3, code);
        string busy = "llyn: The workspace database is in use by another program. Close it and try again.";
        Assert.Equal([busy, "D:\\Busy", "locked"], console.TConsoleFakeError);
        Assert.False(console.TConsoleFakeWindow);
        Assert.Empty(console.TConsoleFakeFrames);
    }

    [Fact]
    public void BootstrapIntroduce_WindowFault_RestoresConsole()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };

        int code = TInvocation.TInvocationRun(
            console,
            workspace.TWorkspaceFolder,
            (_, _) => new LEngine(
                workspace.TWorkspaceRigCreate(), _ => workspace.TWorkspaceRigCreate(), static _ => { }, true),
            (_, _) => throw new InvalidOperationException("broken"));

        Assert.Equal(4, code);
        Assert.False(console.TConsoleFakeWindow);
        Assert.Equal("broken", console.TConsoleFakeError[^1]);
    }

    [Fact]
    public void BootstrapIntroduce_WindowRefusal_PrintsAfterRestore()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };

        TInvocation.TInvocationRun(console, "D:\\Busy", (_, _) => throw new IOException("locked"));

        string busy = "llyn: The workspace database is in use by another program. Close it and try again.";
        string[] trail =
        [
            nameof(BConsole.BConsoleWindowRestore),
            busy,
            "D:\\Busy",
            "locked",
            nameof(BConsole.BConsoleModeRestore),
        ];
        Assert.Equal(trail, console.TConsoleFakeTrail);
    }

    [Fact]
    public void BootstrapIntroduce_WindowFault_PrintsAfterRestore()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };

        TInvocation.TInvocationRun(
            console,
            workspace.TWorkspaceFolder,
            (_, _) => new LEngine(
                workspace.TWorkspaceRigCreate(), _ => workspace.TWorkspaceRigCreate(), static _ => { }, true),
            (_, _) => throw new InvalidOperationException("broken"));

        string fault = "llyn: Something went wrong. The details were written to the audit log.";
        string[] trail =
        [
            nameof(BConsole.BConsoleWindowRestore),
            fault,
            string.Empty,
            "broken",
            nameof(BConsole.BConsoleModeRestore),
        ];
        Assert.Equal(trail, console.TConsoleFakeTrail);
    }
}
