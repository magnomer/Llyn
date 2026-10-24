using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class THelp
{
    [Fact]
    public void WindowKeyObserve_FunctionOne_ShowsWindowPage()
    {
        TConsoleFake console = THelpRun(ConsoleKey.F1, ConsoleKey.End, ConsoleKey.Escape, ConsoleKey.F10);

        string opened = THelpShotFind(console, "The window");
        string scrolled = THelpShotFind(console, "  ► Keys everywhere");

        Assert.Contains("═ Help ═", opened);
        Assert.Contains("These keys work inside help.", opened);
        Assert.Contains("    A box", scrolled);
        Assert.Contains("═ 1 of 3 ═", scrolled);
        Assert.DoesNotContain("F1,", scrolled);
    }

    [Fact]
    public void WindowKeyObserve_FunctionOneOverBox_ShowsBoxPage()
    {
        TConsoleFake console = THelpPrepare(
            false, ConsoleKey.F1, ConsoleKey.Escape, ConsoleKey.Escape, ConsoleKey.F10);

        object? answer = TEnvoy.TEnvoyRun(console, static envoy => envoy.CEnvoyConfirm("Scribe.DeleteConfirm"));

        string shot = THelpShotFind(console, "Press the lit button.");
        Assert.Equal(false, answer);
        Assert.Contains("A box", shot);
        Assert.Contains("F10, Esc", shot);
        Assert.Contains("Tab, Shift+Tab", shot);
        Assert.Contains("Ctrl+C", shot);
    }

    [Fact]
    public void WindowKeyObserve_FunctionOneInHelp_ShowsEverywhere()
    {
        TConsoleFake console = THelpRun(ConsoleKey.F1, ConsoleKey.F1, ConsoleKey.Escape, ConsoleKey.F10);

        string shot = THelpShotFind(console, "F10, Ctrl+C");

        Assert.Contains("Keys everywhere", shot);
        Assert.Contains("These keys work wherever the window has the focus, outside a box.", shot);
    }

    [Fact]
    public void WindowKeyObserve_EnterOnLink_FollowsAndReturns()
    {
        TConsoleFake console = THelpRun(
            ConsoleKey.F1, ConsoleKey.Enter, ConsoleKey.Backspace, ConsoleKey.Escape, ConsoleKey.F10);

        List<string> shots = [.. console.TConsoleFakeShots.Select(static shot => string.Join('\n', shot))];
        int followed = shots.FindIndex(static shot => shot.Contains("F10, Ctrl+C", StringComparison.Ordinal));

        Assert.True(followed > 0);
        Assert.Contains(
            shots.Skip(followed + 1),
            static shot => shot.Contains("These keys work inside help.", StringComparison.Ordinal));
    }

    [Fact]
    public void HelpPageBuild_TabsPage_NamesChooserMenuAndHistoryKeys()
    {
        TConsoleFake console = THelpRun(
            ConsoleKey.F1, ConsoleKey.Tab, ConsoleKey.Tab, ConsoleKey.Enter, ConsoleKey.Escape, ConsoleKey.F10);

        string shot = THelpShotFind(console, "A menu on the top row");

        Assert.Contains("Alt+F1", shot);
        Assert.Contains("Alt+Left", shot);
        Assert.Contains("Alt+Right", shot);
        Assert.Contains("Alt+P", shot);
        Assert.DoesNotContain("F10", shot);
        Assert.DoesNotContain("Ctrl+C", shot);
    }

    [Fact]
    public void HelpPageBuild_BoxOverChooser_NamesChooserKeys()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F1, false, true, false));
        console.TConsoleFakeAdjust(81, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F1, false, false, false));
        console.TConsoleFakeAdjust(83, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));
        console.TConsoleFakeAdjust(85, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));
        console.TConsoleFakeAdjust(87, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        Assert.Equal(0, TInvocation.TInvocationRun(console, workspace));

        string shot = THelpShotFind(console, "A box opens over the window");
        Assert.Contains("Up, Down, Left, Right", shot);
        Assert.DoesNotContain("button.", shot);
        Assert.Contains("Enter, 1, 2, 3", shot);
    }

    [Fact]
    public void HelpPageBuild_BoxOverChooserButtons_NamesButtonKeys()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F1, false, true, false));
        console.TConsoleFakeAdjust(81, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false));
        console.TConsoleFakeAdjust(83, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F1, false, false, false));
        console.TConsoleFakeAdjust(85, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));
        console.TConsoleFakeAdjust(87, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));
        console.TConsoleFakeAdjust(89, 24);
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        Assert.Equal(0, TInvocation.TInvocationRun(console, workspace));

        string shot = THelpShotFind(console, "A box opens over the window");
        Assert.Contains("button.", shot);
        Assert.DoesNotContain("1, 2, 3", shot);
        Assert.DoesNotContain("Up, Down", shot);
    }

    [Fact]
    public void WindowFrameBuild_HelpOpen_ShowsCloseLegend()
    {
        TConsoleFake console = THelpRun(ConsoleKey.F1, ConsoleKey.Escape, ConsoleKey.F10);

        string[] shot = console.TConsoleFakeShots.Last(
            static lines => string.Join('\n', lines).Contains("The window", StringComparison.Ordinal));

        Assert.Equal(new string(' ', 72) + "10Close", shot[23].TrimEnd());
    }

    [Fact]
    public void HelpDraw_EveryPage_NamesOnlyBoundKeys()
    {
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        const BKeymapFocus window = BKeymapFocus.BKeymapFocusWindow;
        Action<ConsoleKeyInfo> none = static _ => { };
        Func<BKeymapLive> shown = static () => BKeymapLive.BKeymapLiveShown;
        Func<BKeymapLive> hidden = static () => BKeymapLive.BKeymapLiveHidden;
        Func<BKeymapLive> dim = static () => BKeymapLive.BKeymapLiveDim;
        BKeymapRow quit = new(ConsoleKey.F10, 0, window, "Legend.Quit", none, shown);
        BKeymapRow play = new(ConsoleKey.F5, 0, window, "Legend.Help", none, hidden);
        BKeymapRow import = new(ConsoleKey.F2, 0, BKeymapFocus.BKeymapFocusParley, "Legend.Import", none, dim);
        Func<BKeymapFocus, IReadOnlyList<BKeymapRow>> keys = focus => focus switch
        {
            BKeymapFocus.BKeymapFocusWindow => [quit, play],
            BKeymapFocus.BKeymapFocusParley => [import],
            _ => [],
        };
        BCanvasWidth measure = new(false);
        BSHelp everywhere = new(catalog, keys, BKeymapFocus.BKeymapFocusWindow, measure);
        everywhere.BSParleyBodyObserve(new ConsoleKeyInfo('\0', ConsoleKey.F1, false, false, false));
        BSHelp box = new(catalog, keys, BKeymapFocus.BKeymapFocusParley, measure);

        Dictionary<string, IReadOnlyList<string>> glyphs = [];
        BCanvasText[] first = [.. everywhere.BSParleyBodyDraw(glyphs, 0, 0, 72, 24, true, "dialog", measure)];
        BCanvasText[] last = [.. box.BSParleyBodyDraw(glyphs, 0, 0, 72, 24, true, "dialog", measure)];

        Assert.Contains(first, static text => text.BCanvasTextValue == "  F10   Quit");
        Assert.DoesNotContain(first, static text => text.BCanvasTextValue.Contains("F5", StringComparison.Ordinal));
        Assert.Contains(
            last, static text => text.BCanvasTextValue == "  F2   Import" && text.BCanvasTextRole == "disabled");
        Assert.Contains(first, static text => text.BCanvasTextValue == "  > The window");
    }

    private static TConsoleFake THelpRun(params ConsoleKey[] keys)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = THelpPrepare(true, keys);

        Assert.Equal(0, TInvocation.TInvocationRun(console, workspace));
        return console;
    }

    private static TConsoleFake THelpPrepare(bool redrawn, params ConsoleKey[] keys)
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        for (int index = 0; index < keys.Length; index++)
        {
            char typed = keys[index] switch
            {
                ConsoleKey.Enter => '\r',
                ConsoleKey.Escape => '\e',
                ConsoleKey.Backspace => '\b',
                _ => '\0',
            };
            console.TConsoleFakeKeys.Add(new ConsoleKeyInfo(typed, keys[index], false, false, false));
            if (index < keys.Length - 1)
            {
                console.TConsoleFakeAdjust(81 + (2 * index), 24);
            }

            if (redrawn && index < keys.Length - 1)
            {
                console.TConsoleFakeAdjust(82 + (2 * index), 24);
            }
        }

        return console;
    }

    private static string THelpShotFind(TConsoleFake console, string text)
    {
        return console.TConsoleFakeShots
            .Select(static shot => string.Join('\n', shot))
            .Last(shot => shot.Contains(text, StringComparison.Ordinal));
    }
}
