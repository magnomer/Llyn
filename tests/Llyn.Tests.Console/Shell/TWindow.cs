using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
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

        string status = console.TConsoleFakeGrid[22];
        Assert.Matches("^ Joplin: Connect +[0-9]+ entries · [0-9]+ KB $", status);
        Assert.Equal(80, status.Length);
        string top = $"╔{new string('═', 38)}╗";
        string body = $"║{new string(' ', 38)}║";
        string rule = $"╟{new string('─', 38)}╢";
        string bottom = $"╚{new string('═', 38)}╝";
        string voyage = $"║{"◄Back (Alt+Left)",-18}Forward (Alt+Right)►║";
        string[] frame =
        [
            "  Llyn  Panel  Options".PadRight(80),
            top + top,
            .. Enumerable.Repeat(body + body, 17),
            rule + rule,
            body + voyage,
            bottom + bottom,
            status,
            "1Help   " + new string(' ', 56) + "9Menu   10Quit  ",
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

        string status = console.TConsoleFakeGrid[28];
        Assert.Matches("^ Joplin: Connect +[0-9]+ entries · [0-9]+ KB $", status);
        Assert.Equal(101, status.Length);
        string top = $"╔{new string('═', 48)}╗╔{new string('═', 49)}╗";
        string body = $"║{new string(' ', 48)}║║{new string(' ', 49)}║";
        string rule = $"╟{new string('─', 48)}╢╟{new string('─', 49)}╢";
        string bottom = $"╚{new string('═', 48)}╝╚{new string('═', 49)}╝";
        string voyage = $"║{new string(' ', 48)}║║{"◄Back (Alt+Left)",-29}Forward (Alt+Right)►║";
        string[] frame =
        [
            "  Llyn  Panel  Options".PadRight(101),
            top,
            .. Enumerable.Repeat(body, 23),
            rule,
            voyage,
            bottom,
            status,
            ("1Help   " + new string(' ', 56) + "9Menu   10Quit").PadRight(101),
        ];
        Assert.Equal(frame, console.TConsoleFakeGrid);
    }

    [Fact]
    public void WindowSizeObserve_BelowEightyByTwentyFour_DrawsFramedNotice()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeAdjust(64, 18);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TInvocation.TInvocationRun(console, workspace);

        string side = new(' ', 11);
        string[] notice =
        [
            .. Enumerable.Repeat(new string(' ', 64), 5),
            $"{side}╔{new string('═', 17)} Llyn {new string('═', 17)}╗{side}",
            $"{side}║{new string(' ', 40)}║{side}",
            $"{side}║  {"Llyn needs a console of at least 80",-36}  ║{side}",
            $"{side}║  {"by 24 cells. This one is 64 by 18.",-36}  ║{side}",
            $"{side}║  {"Make the window larger to go on.",-36}  ║{side}",
            $"{side}║{new string(' ', 40)}║{side}",
            $"{side}╚{new string('═', 40)}╝{side}",
            .. Enumerable.Repeat(new string(' ', 64), 5),
            "10Quit".PadRight(64),
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
    public void WindowKeyObserve_ShiftFunctionTen_KeepsWindow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, true, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        int code = TInvocation.TInvocationRun(console, workspace);

        Assert.Equal(0, code);
        Assert.Empty(console.TConsoleFakeKeys);
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
    public void BootstrapFaultObserve_WindowRunning_DrawsFaultBox()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        int code = TInvocation.TInvocationRun(
            console,
            workspace.TWorkspaceFolder,
            (_, _) => new LEngine(
                workspace.TWorkspaceRigCreate(), _ => workspace.TWorkspaceRigCreate(), static _ => { }, true),
            static (engine, detached) =>
            {
                SynchronizationContext.Current!.Post(static _ => throw new InvalidOperationException("broken"), null);
                return TInterfaceConduct.TAtelierCreate(engine, detached);
            });

        string[] shot = console.TConsoleFakeShots[0];
        Assert.Equal($"║  {"Something went wrong. The details were written to",-52}  ║", shot[8][10..68]);
        Assert.Equal($"║  {"broken",-52}  ║", shot[11][10..68]);
        Assert.Equal(0, code);
        Assert.Empty(console.TConsoleFakeError);
    }

    [Fact]
    public void BootstrapIntroduce_WindowWorkspaceBusy_ExitsThree()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));

        int code = TInvocation.TInvocationRun(console, "D:\\Busy", (_, _) => throw new IOException("locked"));

        Assert.Equal(3, code);
        string busy = "llyn: The workspace database is in use by another program. Close it and try again.";
        Assert.Equal([busy, "D:\\Busy", "locked"], console.TConsoleFakeError);
        Assert.False(console.TConsoleFakeWindow);
        Assert.NotEmpty(console.TConsoleFakeFrames);
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
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));

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

    [Fact]
    public void WindowKeyObserve_WideChosen_WidensMenuAndCanvas()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true, TConsoleFakeAmbiguous = true };
        foreach (ConsoleKeyInfo key in (ConsoleKeyInfo[])
        [
            new('o', ConsoleKey.O, false, true, false),
            new('\0', ConsoleKey.DownArrow, false, false, false),
            new('\0', ConsoleKey.DownArrow, false, false, false),
            new('\0', ConsoleKey.RightArrow, false, false, false),
            new('\0', ConsoleKey.DownArrow, false, false, false),
            new('\r', ConsoleKey.Enter, false, false, false),
            new('o', ConsoleKey.O, false, true, false),
            new('\0', ConsoleKey.DownArrow, false, false, false),
            new('\0', ConsoleKey.DownArrow, false, false, false),
            new('\0', ConsoleKey.RightArrow, false, false, false),
        ])
        {
            console.TConsoleFakeKeys.Add(key);
            console.TConsoleFakeAdjust(80, 24);
        }

        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(console, static _ => null);

        string[] shot = console.TConsoleFakeShots[^1];
        string narrow = Assert.Single(shot, static row => row.Contains("Narrow", StringComparison.Ordinal));
        string wide = Assert.Single(shot, static row => row.Contains("(•) Wide", StringComparison.Ordinal));
        BCanvasWidth measure = new(true);
        Assert.Equal(
            measure.BCanvasWidthResolve(wide.AsSpan(0, wide.IndexOf("(•) Wide", StringComparison.Ordinal) + 4)),
            measure.BCanvasWidthResolve(narrow.AsSpan(0, narrow.IndexOf("Narrow", StringComparison.Ordinal))));
    }

    [Fact]
    public void MuralDraw_LowerParleyField_ShowsNoCaret()
    {
        ABootstrap bootstrap = new();
        BBlueprintPart window = BContract.BContractBlueprintFind(bootstrap, "AWindow");
        BBlueprintPart box = BContract.BContractBlueprintFind(bootstrap, "ASParley");
        BBlueprint.BBlueprintPlace(window, 0, 0, 80, 24);
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        BMural mural = new(
            bootstrap, window, catalog, new BNavigation(bootstrap, null), new BEstablishment(window, catalog));
        BRoof roof = new(window, []);
        Func<BSParleyBody?, BSParley> open = body => new(box, "Box", "dialog", [], [new BSParleyButton("OK")], 0, body);
        Func<BSParley[], BCanvasText[]> marks = parleys => [.. mural.BMuralDraw(
                80,
                24,
                BKeymapFocus.BKeymapFocusParley,
                roof,
                null,
                null,
                parleys,
                [],
                bootstrap.BBlueprintGlyphs,
                new Dictionary<string, IReadOnlyList<string>>(),
                static (_, _) => null,
                new BCanvasWidth(false))
            .Where(static text => text.BCanvasTextRole == "caret")];
        BSParley lower = open(new BField(_ => { }, "abcdef"));
        BSParley top = open(new BField(_ => { }, "ab"));
        BCanvasText lowerAlone = Assert.Single(marks([lower]));
        BCanvasText topAlone = Assert.Single(marks([top]));

        Assert.Empty(marks([lower, open(null)]));
        Assert.Equal([topAlone], marks([lower, top]));
        Assert.NotEqual(lowerAlone, topAlone);
    }
}
