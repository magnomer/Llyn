using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
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
        TConsoleFake bare = new() { TConsoleFakeTerminal = true };
        bare.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));
        Action<BWindow> menu = static window =>
            window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.F9, false, false, false));

        TEnvoy.TEnvoyRun(console, static envoy => envoy.CEnvoyConfirm("Scribe.DeleteConfirm"), prime: menu);
        TEnvoy.TEnvoyRun(bare, static _ => null, prime: menu);

        string[] shot = console.TConsoleFakeShots[0];
        Assert.Equal("  Llyn ", shot[0][..7]);
        Assert.Equal("║ Connect to Joplin       ║", shot[2].Substring(1, 27));
        Assert.Equal("║ Sync to Joplin          ║", shot[3].Substring(1, 27));
        Assert.Equal($"╟{new string('─', 25)}╢", shot[4].Substring(1, 27));
        Assert.Equal("║ About Llyn              ║", shot[5].Substring(1, 27));
        Assert.Equal("║ Help                 F1 ║", shot[6].Substring(1, 27));
        Assert.Equal("║ Keys                    ║", shot[7].Substring(1, 27));
        string[] uncovered = bare.TConsoleFakeShots[0];
        Assert.Equal($"╟{new string('─', 25)}╢", uncovered[8].Substring(1, 27));
        Assert.Equal("║ Quit                F10 ║", uncovered[9].Substring(1, 27));
    }

    [Fact]
    public void RoofKeyObserve_RightOnSubmenu_OpensBesideItem()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(console, static _ => null, prime: static window =>
        {
            window.BWindowKeyObserve(new ConsoleKeyInfo('o', ConsoleKey.O, false, true, false));
            window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false));
        });

        string[] shot = console.TConsoleFakeShots[0];
        int left = shot[0].IndexOf(" Options ", StringComparison.Ordinal);
        Assert.Equal($"╔{new string('═', 31)}╗╔══════════╗", shot[1].Substring(left, 45));
        Assert.Equal($"║ {"Theme",-25}   ▸ ║║ (•) Blue ║", shot[2].Substring(left, 45));
        Assert.Equal("║     Dark ║", shot[3].Substring(left + 33, 12));
    }

    [Fact]
    public void RoofKeyObserve_LeftInSubmenu_ClosesSubmenuOnly()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(console, static _ => null, prime: static window =>
        {
            window.BWindowKeyObserve(new ConsoleKeyInfo('o', ConsoleKey.O, false, true, false));
            window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false));
            window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false));
        });

        string[] shot = console.TConsoleFakeShots[0];
        int left = shot[0].IndexOf(" Options ", StringComparison.Ordinal);
        Assert.Equal($"║ {"Theme",-25}   ▸ ║", shot[2].Substring(left, 33));
        Assert.DoesNotContain("Blue", string.Join('\n', shot), StringComparison.Ordinal);
    }

    [Fact]
    public void RoofKeyObserve_EnterOnSubmenu_OpensSubmenu()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(console, static _ => null, prime: static window =>
        {
            window.BWindowKeyObserve(new ConsoleKeyInfo('o', ConsoleKey.O, false, true, false));
            window.BWindowKeyObserve(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        });

        string[] shot = console.TConsoleFakeShots[0];
        int left = shot[0].IndexOf(" Options ", StringComparison.Ordinal);
        Assert.Equal($"║ {"Theme",-25}   ▸ ║║ (•) Blue ║", shot[2].Substring(left, 45));
        Assert.Equal("║     Dark ║", shot[3].Substring(left + 33, 12));
    }

    [Fact]
    public void RoofKeyObserve_HotkeyOnSubmenu_OpensSubmenu()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(console, static _ => null, prime: static window =>
        {
            window.BWindowKeyObserve(new ConsoleKeyInfo('o', ConsoleKey.O, false, true, false));
            window.BWindowKeyObserve(new ConsoleKeyInfo('t', ConsoleKey.T, false, false, false));
        });

        string[] shot = console.TConsoleFakeShots[0];
        int left = shot[0].IndexOf(" Options ", StringComparison.Ordinal);
        Assert.Equal($"╔{new string('═', 31)}╗╔══════════╗", shot[1].Substring(left, 45));
        Assert.Equal($"║ {"Theme",-25}   ▸ ║║ (•) Blue ║", shot[2].Substring(left, 45));
    }

    [Fact]
    public void RoofKeyObserve_EscapeInSubmenu_ClosesSubmenuOnly()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(console, static _ => null, prime: static window =>
        {
            window.BWindowKeyObserve(new ConsoleKeyInfo('o', ConsoleKey.O, false, true, false));
            window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false));
            window.BWindowKeyObserve(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));
        });

        string[] shot = console.TConsoleFakeShots[0];
        int left = shot[0].IndexOf(" Options ", StringComparison.Ordinal);
        Assert.Equal($"║ {"Theme",-25}   ▸ ║", shot[2].Substring(left, 33));
        Assert.DoesNotContain("Blue", string.Join('\n', shot), StringComparison.Ordinal);
    }

    [Fact]
    public void RoofKeyObserve_DownInSubmenu_MovesInsideSubmenu()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));
        BTheme spare = new(BTheme.BThemeSpareRead);
        List<string> asked = [];

        TEnvoy.TEnvoyRun(
            console,
            static _ => null,
            prime: static window =>
            {
                window.BWindowKeyObserve(new ConsoleKeyInfo('o', ConsoleKey.O, false, true, false));
                window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false));
                window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));
                window.BWindowKeyObserve(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
            },
            themes: name =>
            {
                asked.Add(name);
                return spare;
            });

        Assert.Equal(["console-dark"], asked);
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
                window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));
                window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));
                window.BWindowKeyObserve(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
            });

        string build = typeof(BWindow).Assembly.GetName().Version!.ToString(3);
        string shot = string.Join('\n', console.TConsoleFakeShots[0]);
        Assert.Contains(build, shot, StringComparison.Ordinal);
        Assert.DoesNotContain("About Llyn", shot, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(ConsoleKey.F1, false, "═ Help ═")]
    [InlineData(ConsoleKey.F10, false, null)]
    [InlineData(ConsoleKey.C, true, null)]
    public void WindowKeyObserve_BoxKeyInMenu_ClosesMenuFirst(ConsoleKey key, bool control, string? box)
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));
        bool quit = false;

        TEnvoy.TEnvoyRun(
            console,
            static _ => null,
            prime: window =>
            {
                window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.F9, false, false, false));
                window.BWindowKeyObserve(new ConsoleKeyInfo(control ? '\u0003' : '\0', key, false, false, control));
                quit = window.BWindowExit.IsCompleted;
                window.BWindowBatchObserve();
            });

        string[] shots = [.. console.TConsoleFakeShots.Select(static shot => string.Join('\n', shot))];
        Assert.DoesNotContain("About Llyn", shots[^1], StringComparison.Ordinal);
        if (box is null)
        {
            Assert.True(quit);
            return;
        }

        Assert.Contains(shots, shot => shot.Contains(box, StringComparison.Ordinal));
        Assert.DoesNotContain(
            shots.Where(shot => shot.Contains(box, StringComparison.Ordinal)),
            static shot => shot.Contains("About Llyn", StringComparison.Ordinal));
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
    public void NavigationBuild_HiddenTab_LeftOutOfPanelMenuAndChooser()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, false);
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        BNavigation tabs = new(new ABootstrap(), atelier);
        CNavigationState state = new("Input", ["Phonology"], new CVoyageState(false, false));

        Dictionary<string, IReadOnlyList<string>> marks = [];

        BCanvasWidth measure = new(false);
        BRoofMenu panel = tabs.BNavigationBuild(catalog, state, marks, static () => { }, measure);
        BSNavigation chooser = new(catalog, tabs.BNavigationTabs, state, static _ => { }, measure);
        string[] items = [.. panel.BRoofMenuItems.OfType<BRoofItem>().Select(static item => item.BRoofItemLabel)];
        string[] choices =
        [
            .. chooser.BSParleyBodyDraw(marks, 0, 0, 80, false, "dialog", measure)
                .Select(static text => text.BCanvasTextValue),
        ];

        Assert.Contains("  List", items);
        Assert.DoesNotContain(items, static item => item.Contains("Sound", StringComparison.Ordinal));
        Assert.DoesNotContain(choices, static choice => choice.Contains("Sound", StringComparison.Ordinal));
        Assert.Equal(["1 New", "2 List", "3 Favorite", "4 Situation"], choices[..4]);
    }

    [Fact]
    public void WindowKeyObserve_EmptyTrail_DimsBackAndForward()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, true, false));
        console.TConsoleFakeAdjust(80, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, true, false));
        console.TConsoleFakeAdjust(80, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(console, static _ => null);

        string dim = new BTheme(BTheme.BThemeSpareRead).BThemeRead("disabled");
        string frames = string.Concat(console.TConsoleFakeFrames);
        Assert.Contains(dim + "◄Back (Alt+Left)", console.TConsoleFakeFrames[0], StringComparison.Ordinal);
        Assert.Contains(dim + "Forward (Alt+Right)►", console.TConsoleFakeFrames[0], StringComparison.Ordinal);
        Assert.Equal(
            frames.Split("◄Back (Alt+Left)").Length, frames.Split(dim + "◄Back (Alt+Left)").Length);
        Assert.Equal(
            frames.Split("Forward (Alt+Right)►").Length, frames.Split(dim + "Forward (Alt+Right)►").Length);
    }

    [Fact]
    public void WindowKeyObserve_TabSelect_RoofShowsTabName()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('p', ConsoleKey.P, false, true, false));
        console.TConsoleFakeAdjust(80, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));
        console.TConsoleFakeAdjust(80, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeAdjust(80, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(console, static _ => null);

        Assert.EndsWith(" List", console.TConsoleFakeShots[^1][0].TrimEnd(), StringComparison.Ordinal);
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
