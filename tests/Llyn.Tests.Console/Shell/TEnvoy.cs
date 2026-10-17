using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TEnvoy
{
    [Fact]
    public void EnvoyConfirm_Enter_AnswersYes()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        object? answer = TEnvoyRun(console, static envoy => envoy.CEnvoyConfirm("Scribe.DeleteConfirm"));

        Assert.Equal(true, answer);
    }

    [Fact]
    public void EnvoyConfirm_RightThenEnter_AnswersNo()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        object? answer = TEnvoyRun(console, static envoy => envoy.CEnvoyConfirm("Scribe.DeleteConfirm"));

        Assert.Equal(false, answer);
    }

    [Fact]
    public void EnvoyConfirm_Escape_AnswersNo()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        object? answer = TEnvoyRun(console, static envoy => envoy.CEnvoyConfirm("Scribe.DeleteConfirm"));

        Assert.Equal(false, answer);
    }

    [Fact]
    public void EnvoyConfirm_FunctionTen_AnswersNo()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        object? answer = TEnvoyRun(console, static envoy => envoy.CEnvoyConfirm("Scribe.DeleteConfirm"));

        Assert.Equal(false, answer);
    }

    [Fact]
    public void EnvoyConfirm_ShiftFunctionTen_KeepsBox()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, true, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        object? answer = TEnvoyRun(console, static envoy => envoy.CEnvoyConfirm("Scribe.DeleteConfirm"));

        Assert.Equal(true, answer);
    }

    [Fact]
    public void EnvoyConfirm_Plain_DrawsAlertBox()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoyRun(console, static envoy => envoy.CEnvoyConfirm("Scribe.DeleteConfirm"));

        string blank = $"║{new string(' ', 56)}║";
        string[] box =
        [
            $"╔{new string('═', 25)} Llyn {new string('═', 25)}╗",
            blank,
            $"║  {"Delete this entry? It cannot be undone.",-52}  ║",
            blank,
            $"║{new string(' ', 19)}[ Yes ]{new string(' ', 5)}[ No ]{new string(' ', 19)}║",
            blank,
            $"╚{new string('═', 56)}╝",
        ];
        string[] shot = console.TConsoleFakeShots[0];
        Assert.Equal(box, shot[8..15].Select(static row => row[10..68]));
        Assert.Contains(new BTheme(BTheme.BThemeSpareRead).BThemeRead("alert"), console.TConsoleFakeFrames[0]);
    }

    [Fact]
    public void EnvoyConfirm_Tally_DrawsCountLine()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoyRun(console, static envoy => envoy.CEnvoyConfirm("Deck.DeleteConfirm", "Deck.DeleteCount", 14));

        string[] shot = console.TConsoleFakeShots[0];
        Assert.Equal($"║  {"Delete this deck, with every flashcard in it? It",-52}  ║", shot[8][10..68]);
        Assert.Equal($"║  {"Flashcard count: 14",-52}  ║", shot[11][10..68]);
        Assert.DoesNotContain(new BTheme(BTheme.BThemeSpareRead).BThemeRead("alert"), console.TConsoleFakeFrames[0]);
    }

    [Fact]
    public void EnvoyUnionConfirm_Names_DrawsArrowLine()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        object? answer = TEnvoyRun(
            console, static envoy => envoy.CEnvoyUnionConfirm("Guild.MergeConfirm", "Ann", "Bob"));

        Assert.Equal(true, answer);
        Assert.Equal($"║  {"Ann → Bob",-52}  ║", console.TConsoleFakeShots[0][11][10..68]);
    }

    [Fact]
    public void EnvoyFailureShow_HeadlineOnly_DrawsNoDetailLine()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoyRun(console, static envoy =>
        {
            envoy.CEnvoyFailureShow("Layout.SaveFailed");
            return null;
        });

        string blank = $"║{new string(' ', 56)}║";
        string[] box =
        [
            $"╔{new string('═', 25)} Llyn {new string('═', 25)}╗",
            blank,
            $"║  {"The layout could not be saved.",-52}  ║",
            blank,
            $"║{new string(' ', 25)}[ OK ]{new string(' ', 25)}║",
            blank,
            $"╚{new string('═', 56)}╝",
        ];
        Assert.Equal(box, console.TConsoleFakeShots[0][8..15].Select(static row => row[10..68]));
        Assert.Empty(console.TConsoleFakeError);
    }

    [Fact]
    public void EnvoyFailureShow_LongPath_WrapsAtSeparators()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));
        const string folder = "D:\\Llyn\\audit\\a-very-long-folder-name-for-wrapping\\";
        const string file = "another-long-folder-name\\fault-2026-10-16-0912.txt";

        TEnvoyRun(console, static envoy =>
        {
            CLedgerNotice notice = new("Notice.Unexpected", "Notice.Recorded", folder + file);
            envoy.CEnvoyFailureShow("Layout.SaveFailed", notice);
            return null;
        });

        string[] shot = console.TConsoleFakeShots[0];
        Assert.Equal($"║  {folder,-52}  ║", shot[12][10..68]);
        Assert.Equal($"║  {file,-52}  ║", shot[13][10..68]);
    }

    [Fact]
    public void EnvoyFailureShow_UnderConfirm_DrawsAbove()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        object? answer = TEnvoyRun(console, static envoy =>
        {
            SynchronizationContext.Current!.Post(_ => envoy.CEnvoyFailureShow("Layout.SaveFailed"), null);
            return envoy.CEnvoyConfirm("Scribe.DeleteConfirm");
        });

        Assert.Equal(true, answer);
        Assert.Equal($"║  {"The layout could not be saved.",-52}  ║", console.TConsoleFakeShots[0][10][10..68]);
    }

    [Fact]
    public void EnvoyFaultShow_FaultBoxDrawFails_HoldsNotice()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        int code = TInvocation.TInvocationRun(
            console,
            workspace.TWorkspaceFolder,
            (_, _) => new LEngine(
                workspace.TWorkspaceRigCreate(), _ => workspace.TWorkspaceRigCreate(), static _ => { }, true),
            (engine, detached) =>
            {
                SynchronizationContext.Current!.Post(
                    _ =>
                    {
                        console.TConsoleFakeFaults = 1;
                        throw new InvalidOperationException("broken");
                    },
                    null);
                return TInterfaceConduct.TAtelierCreate(engine, detached);
            });

        Assert.Equal(0, code);
        Assert.Contains("The console refused the frame.", console.TConsoleFakeError);
    }

    [Fact]
    public void EnvoyLeaveConfirm_Enter_AnswersStore()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        object? answer = TEnvoyRun(console, static envoy => envoy.CEnvoyLeaveConfirm());

        Assert.Equal(true, answer);
        Assert.Contains("[ Save ]     [ Discard ]     [ Cancel ]", string.Join('\n', console.TConsoleFakeShots[0]));
    }

    [Fact]
    public void EnvoyLeaveConfirm_RightThenEnter_AnswersDiscard()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        object? answer = TEnvoyRun(console, static envoy => envoy.CEnvoyLeaveConfirm());

        Assert.Equal(false, answer);
    }

    [Fact]
    public void EnvoyLeaveConfirm_Escape_AnswersStay()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        object? answer = TEnvoyRun(console, static envoy => envoy.CEnvoyLeaveConfirm());

        Assert.Null(answer);
    }

    [Fact]
    public void EnvoyCoinageRead_Typed_AnswersWordingUntouched()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        foreach (char letter in "  formal ")
        {
            console.TConsoleFakeKeys.Add(new ConsoleKeyInfo(letter, ConsoleKey.A, false, false, false));
        }

        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        object? answer = TEnvoyRun(console, static envoy => envoy.CEnvoyCoinageRead("Coinage.Tag"));

        Assert.Equal("  formal ", answer);
    }

    [Fact]
    public void EnvoyCoinageRead_EnterBeforeTyping_WaitsForWording()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        object? answer = TEnvoyRun(console, static envoy => envoy.CEnvoyCoinageRead("Coinage.Tag"));

        Assert.Equal("x", answer);
        Assert.Contains("[ Create ]", string.Join('\n', console.TConsoleFakeShots[0]));
    }

    [Fact]
    public void EnvoyCoinageRead_Escape_AnswersNull()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        object? answer = TEnvoyRun(console, static envoy => envoy.CEnvoyCoinageRead("Coinage.Tag"));

        Assert.Null(answer);
    }

    [Fact]
    public void EnvoyCoinageRead_TabToButtons_IgnoresTyping()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('b', ConsoleKey.B, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('c', ConsoleKey.C, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        object? answer = TEnvoyRun(console, static envoy => envoy.CEnvoyCoinageRead("Coinage.Tag"));

        Assert.Equal("ac", answer);
    }

    internal static object? TEnvoyRun(
        TConsoleFake console, Func<BEnvoy, object?> ask, string language = "en", Action<BWindow>? prime = null)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        SynchronizationContext? context = SynchronizationContext.Current;
        BLoop loop = new(static _ => { });
        SynchronizationContext.SetSynchronizationContext(loop);
        try
        {
            BLocalizationCatalog catalog = new();
            catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad(language));
            ABootstrap bootstrap = new();
            BEnvoy envoy = new(console, catalog, bootstrap);
            using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, false);
            BWindow window = new(
                console, loop, catalog, atelier, envoy, bootstrap, new BTheme(BTheme.BThemeSpareRead));
            envoy.BEnvoyWindowAttach(window);
            object? answer = null;
            loop.BLoopDefer(window.BWindowOpen);
            loop.BLoopDefer(() =>
            {
                prime?.Invoke(window);
                answer = ask(envoy);
            });
            loop.BLoopBatchAttach(window.BWindowBatchObserve);
            loop.BLoopKeyAttach(console, window.BWindowKeyObserve);
            loop.BLoopRun(window.BWindowExit);
            return answer;
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }
    }
}
