using Llyn.Conduct;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TBootstrap
{
    [Fact]
    public void BootstrapIntroduce_WindowWorkspaceBusy_DrawsAlertBox()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));

        int code = TInvocation.TInvocationRun(
            console,
            static _ => throw new InvalidDataException("no theme"),
            "en",
            "D:\\Busy",
            (_, _) => throw new IOException("locked"),
            TInterfaceConduct.TAtelierCreate);

        string shot = string.Join('\n', console.TConsoleFakeShots[0]);
        Assert.Contains($"╔{new string('═', 25)} Llyn {new string('═', 25)}╗", shot);
        Assert.Contains($"║  {"D:\\Busy",-52}  ║", shot);
        Assert.Contains($"║  {"locked",-52}  ║", shot);
        Assert.Contains("[ OK ]", shot);
        Assert.Contains(new BTheme(BTheme.BThemeSpareRead).BThemeRead("alert"), console.TConsoleFakeFrames[0]);
        Assert.Equal(3, code);
        string busy = "llyn: The workspace database is in use by another program. Close it and try again.";
        Assert.Equal([busy, "D:\\Busy", "locked"], console.TConsoleFakeError);
        List<string> trail = console.TConsoleFakeTrail;
        int restored = trail.IndexOf("BConsoleWindowRestore");
        Assert.InRange(restored, 0, trail.IndexOf(busy) - 1);
    }

    [Fact]
    public void BootstrapIntroduce_WindowBusyFunctionTen_ExitsThree()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        int code = TInvocation.TInvocationRun(
            console,
            static _ => throw new InvalidDataException("no theme"),
            "en",
            "D:\\Busy",
            (_, _) => throw new IOException("locked"),
            TInterfaceConduct.TAtelierCreate);

        Assert.Contains("[ OK ]", string.Join('\n', console.TConsoleFakeShots[0]));
        Assert.Equal(false, console.TConsoleFakeWindow);
        Assert.Equal(3, code);
    }

    [Fact]
    public void BootstrapRescueConsult_WindowOpen_DrawsInformationBox()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        int code = TBootstrapRescueRun(console, name => LThemeLoader.LThemeLoaderLoad(name).LThemeColorRead);

        string shot = string.Join('\n', console.TConsoleFakeShots[0]);
        Assert.Contains($"║  {"D:\\Llyn\\llyn.db.bak",-52}  ║", shot);
        Assert.Contains($"║  {"damaged",-52}  ║", shot);
        Assert.Contains("[ OK ]", shot);
        Assert.Equal(0, code);
        Assert.Empty(console.TConsoleFakeError);
    }

    [Fact]
    public void BootstrapAtelierRun_RescueAndTheme_DrawsRescueFirst()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeAdjust(100, 30);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        int code = TBootstrapRescueRun(console, static _ => throw new InvalidDataException("no theme"));

        string shot = string.Join('\n', console.TConsoleFakeShots[0]);
        Assert.Contains($"║  {"damaged",-52}  ║", shot);
        Assert.DoesNotContain("The console theme could not be loaded.", shot);
        int rescued = console.TConsoleFakeShots.FindIndex(static grid => string.Join('\n', grid).Contains("damaged"));
        int themed = console.TConsoleFakeShots.FindIndex(
            static grid => string.Join('\n', grid).Contains("The console theme could not be loaded."));
        Assert.Equal(0, rescued);
        Assert.True(themed > rescued);
        Assert.Empty(console.TConsoleFakeError);
        Assert.Equal(0, code);
    }

    [Fact]
    public void BootstrapAtelierRun_SpareTheme_MarksNoTheme()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        foreach (ConsoleKeyInfo key in (ConsoleKeyInfo[])
        [
            new('\r', ConsoleKey.Enter, false, false, false),
            new('o', ConsoleKey.O, false, true, false),
            new('\0', ConsoleKey.RightArrow, false, false, false),
        ])
        {
            console.TConsoleFakeKeys.Add(key);
            console.TConsoleFakeAdjust(80, 24);
        }

        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        int code = TInvocation.TInvocationRun(
            console,
            static _ => throw new InvalidDataException("no theme"),
            "en",
            workspace.TWorkspaceFolder,
            (_, _) => new LEngine(
                workspace.TWorkspaceRigCreate(), _ => workspace.TWorkspaceRigCreate(), static _ => { }, true),
            TInterfaceConduct.TAtelierCreate);

        string shot = string.Join('\n', console.TConsoleFakeShots[^1]);
        Assert.Contains("    Blue", shot);
        Assert.Contains("    Dark", shot);
        Assert.DoesNotContain("(•)", shot);
        Assert.Equal(0, code);
    }

    private static int TBootstrapRescueRun(TConsoleFake console, Func<string, Func<string, string>> theme)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        SynchronizationContext? context = SynchronizationContext.Current;
        BBootstrap bootstrap = new(console, new ABootstrap());
        try
        {
            return bootstrap.BBootstrapIntroduce(
                [],
                theme,
                () => TInterface.TLocalizationLoad("en"),
                () => workspace.TWorkspaceFolder,
                (_, _) => new LEngine(
                    workspace.TWorkspaceRigCreate(), _ => workspace.TWorkspaceRigCreate(), static _ => { }, true),
                static _ => true,
                engine =>
                {
                    using (engine)
                    {
                        bootstrap.BBootstrapRescueConsult(true, "D:\\Llyn\\llyn.db.bak", "damaged");
                        return bootstrap.BBootstrapAtelierRun(
                            detached => TInterfaceConduct.TAtelierCreate(engine, detached), File.Exists);
                    }
                });
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }
    }
}
