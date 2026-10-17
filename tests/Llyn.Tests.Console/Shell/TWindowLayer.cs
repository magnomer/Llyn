using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TWindowLayer
{
    [Fact]
    public void WindowParleyConsult_QueuedWork_RunsUnderParley()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        object? answer = TEnvoy.TEnvoyRun(console, static envoy =>
        {
            bool ran = false;
            SynchronizationContext.Current!.Post(_ => ran = true, null);
            envoy.CEnvoyConfirm("Scribe.DeleteConfirm");
            return ran;
        });

        Assert.Equal(true, answer);
    }

    [Fact]
    public void WindowFrameBuild_ParleyOpen_ShowsCloseLegend()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(console, static envoy => envoy.CEnvoyConfirm("Scribe.DeleteConfirm"));

        Assert.Equal("1Help   " + new string(' ', 64) + "10Close ", console.TConsoleFakeShots[0][23]);
    }

    [Fact]
    public void WindowKeyObserve_FunctionNine_DrawsLlynMenuUnderBox()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(
            console,
            static envoy => envoy.CEnvoyConfirm("Scribe.DeleteConfirm"),
            prime: static window =>
                window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.F9, false, false, false)));

        string[] shot = console.TConsoleFakeShots[0];
        Assert.Equal("  Llyn ", shot[0][..7]);
        Assert.Equal("║ About Llyn       ║", shot[2].Substring(1, 20));
        Assert.Equal("║ Help          F1 ║", shot[3].Substring(1, 20));
        Assert.Equal("║ Keys             ║", shot[4].Substring(1, 20));
        Assert.Equal("╟──────────────────╢", shot[5].Substring(1, 20));
        Assert.Equal("║ Quit         F10 ║", shot[6].Substring(1, 20));
    }

    [Fact]
    public void WindowKeyObserve_EnterOnAbout_ShowsVersionAfterMenuCloses()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(
            console,
            static _ => null,
            prime: static window =>
            {
                window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.F9, false, false, false));
                window.BWindowKeyObserve(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
            });

        string build = typeof(BWindow).Assembly.GetName().Version!.ToString(3);
        string shot = string.Join('\n', console.TConsoleFakeShots[0]);
        Assert.Contains(build, shot, StringComparison.Ordinal);
        Assert.DoesNotContain("About Llyn", shot, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(ConsoleKey.F1, false)]
    [InlineData(ConsoleKey.F10, false)]
    [InlineData(ConsoleKey.C, true)]
    public void WindowKeyObserve_BoxKeyInMenu_ClosesMenuFirst(ConsoleKey key, bool control)
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(
            console,
            static _ => null,
            prime: window =>
            {
                window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.F9, false, false, false));
                window.BWindowKeyObserve(new ConsoleKeyInfo(control ? '\u0003' : '\0', key, false, false, control));
                window.BWindowBatchObserve();
            });

        string shot = string.Join('\n', console.TConsoleFakeShots[^1]);
        Assert.DoesNotContain("About Llyn", shot, StringComparison.Ordinal);
    }

    [Fact]
    public void WindowKeyObserve_AltP_DrawsPanelMenuWithRules()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('p', ConsoleKey.P, false, true, false));
        console.TConsoleFakeAdjust(80, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(console, static _ => null);

        string shot = string.Join('\n', console.TConsoleFakeShots[1]);
        Assert.Contains("    New ", shot, StringComparison.Ordinal);
        Assert.Contains("    Rime table ", shot, StringComparison.Ordinal);
        Assert.Contains("╟────", shot, StringComparison.Ordinal);
    }

    [Fact]
    public void WindowKeyObserve_UpInTallMenu_ShowsLastItem()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('p', ConsoleKey.P, false, true, false));
        console.TConsoleFakeAdjust(80, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false));
        console.TConsoleFakeAdjust(80, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(console, static _ => null);

        Assert.Contains(
            console.TConsoleFakeShots[1], static line => line.Contains("╚════", StringComparison.Ordinal));
        Assert.Contains(
            console.TConsoleFakeShots[2], static line => line.Contains("Choose tab", StringComparison.Ordinal));
    }

    [Fact]
    public void WindowKeyObserve_AltFunctionOneThenHotkey_SelectsTab()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F1, false, true, false));
        console.TConsoleFakeAdjust(80, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('2', ConsoleKey.D2, false, false, false));
        console.TConsoleFakeAdjust(80, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(console, static _ => null);

        Assert.Contains("1 New", string.Join('\n', console.TConsoleFakeShots[1]), StringComparison.Ordinal);
        Assert.Contains(" List ", console.TConsoleFakeShots[2][0], StringComparison.Ordinal);
    }

    [Fact]
    public void WindowKeyObserve_ControlCUnderParley_AnswersSafe()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\u0003', ConsoleKey.C, false, false, true));

        object? answer = TEnvoy.TEnvoyRun(console, static envoy => envoy.CEnvoyConfirm("Scribe.DeleteConfirm"));

        Assert.Equal(false, answer);
    }
}
