using System.Globalization;
using Llyn.Conduct;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TCabinet
{
    [Fact]
    public void CabinetBuild_ChosenTheme_CarriesMark()
    {
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        List<string> chosen = [];

        BRoofMenu menu = BCabinet.BCabinetBuild(
            catalog,
            new ABootstrap().BBlueprintGlyphs["double"],
            "console-dark",
            "double",
            "narrow",
            null,
            chosen.Add,
            static _ => { },
            static _ => { },
            TEnvoyFake.TEnvoyCreate(false, []),
            static (_, _) => { },
            static _ => false,
            new BCanvasWidth(false));
        BRoofItem theme = Assert.IsType<BRoofItem>(menu.BRoofMenuItems[0]);
        BRoofItem[] shades = [.. theme.BRoofItemSubmenu!.OfType<BRoofItem>()];
        shades[0].BRoofItemAction();

        Assert.Equal("Options", menu.BRoofMenuTitle);
        Assert.Equal("Theme", theme.BRoofItemLabel);
        Assert.Equal(["    Blue", "(•) Dark"], shades.Select(static shade => shade.BRoofItemLabel));
        Assert.Equal(["console-blue"], chosen);
    }

    [Fact]
    public void CabinetBuild_SpareStartTheme_MarksNoTheme()
    {
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));

        BRoofMenu menu = BCabinet.BCabinetBuild(
            catalog,
            new ABootstrap().BBlueprintGlyphs["double"],
            null,
            "double",
            "narrow",
            null,
            static _ => { },
            static _ => { },
            static _ => { },
            TEnvoyFake.TEnvoyCreate(false, []),
            static (_, _) => { },
            static _ => false,
            new BCanvasWidth(false));
        BRoofItem theme = Assert.IsType<BRoofItem>(menu.BRoofMenuItems[0]);

        Assert.Equal(
            ["    Blue", "    Dark"],
            theme.BRoofItemSubmenu!.OfType<BRoofItem>().Select(static shade => shade.BRoofItemLabel));
    }

    [Fact]
    public void CabinetBuild_GlyphsInUse_CarriesMark()
    {
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        List<string> chosen = [];

        BRoofMenu menu = BCabinet.BCabinetBuild(
            catalog,
            new ABootstrap().BBlueprintGlyphs["double"],
            BTheme.BThemeDefault,
            "single",
            "narrow",
            null,
            static _ => { },
            chosen.Add,
            static _ => { },
            TEnvoyFake.TEnvoyCreate(false, []),
            static (_, _) => { },
            static _ => false,
            new BCanvasWidth(false));
        BRoofItem glyphs = Assert.IsType<BRoofItem>(menu.BRoofMenuItems[1]);
        BRoofItem[] sets = [.. glyphs.BRoofItemSubmenu!.OfType<BRoofItem>()];
        sets[2].BRoofItemAction();

        Assert.Equal("Glyphs", glyphs.BRoofItemLabel);
        Assert.Equal(ConsoleKey.G, glyphs.BRoofItemHotkey);
        Assert.Equal(
            ["    Double lines", "(•) Single lines", "    ASCII"], sets.Select(static set => set.BRoofItemLabel));
        Assert.Equal(["ascii"], chosen);
    }

    [Fact]
    public void CabinetBuild_AmbiguousInUse_CarriesMark()
    {
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));

        BRoofMenu menu = BCabinet.BCabinetBuild(
            catalog,
            new ABootstrap().BBlueprintGlyphs["double"],
            BTheme.BThemeDefault,
            "double",
            "wide",
            null,
            static _ => { },
            static _ => { },
            static _ => { },
            TEnvoyFake.TEnvoyCreate(false, []),
            static (_, _) => { },
            static _ => false,
            new BCanvasWidth(false));
        BRoofItem ambiguous = Assert.IsType<BRoofItem>(menu.BRoofMenuItems[2]);

        Assert.Equal("Wide ambiguous characters", ambiguous.BRoofItemLabel);
        Assert.Equal(ConsoleKey.W, ambiguous.BRoofItemHotkey);
        Assert.Equal(
            ["    Narrow", "(•) Wide"],
            ambiguous.BRoofItemSubmenu!.OfType<BRoofItem>().Select(static width => width.BRoofItemLabel));
    }

    [Fact]
    public void CabinetBuild_Ledger_MarksLanguageInUse()
    {
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        List<string> chosen = [];
        CLedgerState ledger = new(
            new CSettings("ko", false, false, false, false, 0, string.Empty, string.Empty),
            string.Empty,
            "ko",
            new Dictionary<string, string>(),
            [new("en", "English"), new("ko", "한국어")],
            [],
            new CLedgerShown([], true));

        BRoofMenu menu = BCabinet.BCabinetBuild(
            catalog,
            new ABootstrap().BBlueprintGlyphs["double"],
            BTheme.BThemeDefault,
            "double",
            "narrow",
            ledger,
            static _ => { },
            static _ => { },
            static _ => { },
            TEnvoyFake.TEnvoyCreate(false, []),
            (language, _) => chosen.Add(language),
            static _ => false,
            new BCanvasWidth(false));
        BRoofItem language = Assert.IsType<BRoofItem>(menu.BRoofMenuItems[4]);
        BRoofItem[] languages = [.. language.BRoofItemSubmenu!.OfType<BRoofItem>()];
        languages[0].BRoofItemAction();

        Assert.Null(menu.BRoofMenuItems[3]);
        Assert.Equal("Interface language", language.BRoofItemLabel);
        Assert.True(language.BRoofItemLive());
        Assert.Equal(["    English", "(•) 한국어"], languages.Select(static item => item.BRoofItemLabel));
        Assert.Equal(["en"], chosen);
        Assert.Equal("Settings…", menu.BRoofMenuItems[5]!.BRoofItemLabel);
    }

    [Fact]
    public void WindowKeyObserve_AsciiChosen_DrawsAsciiFrame()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        foreach (ConsoleKeyInfo key in (ConsoleKeyInfo[])
        [
            new('o', ConsoleKey.O, false, true, false),
            new('\0', ConsoleKey.DownArrow, false, false, false),
            new('\0', ConsoleKey.RightArrow, false, false, false),
            new('\0', ConsoleKey.DownArrow, false, false, false),
            new('\0', ConsoleKey.DownArrow, false, false, false),
            new('\r', ConsoleKey.Enter, false, false, false),
        ])
        {
            console.TConsoleFakeKeys.Add(key);
            console.TConsoleFakeAdjust(80, 24);
        }

        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(console, static _ => null);

        string[] before = console.TConsoleFakeShots[0];
        string[] after = console.TConsoleFakeShots[^1];
        Assert.Contains(before, static row => row.Contains('║'));
        Assert.DoesNotContain(after, static row => row.Contains('║') || row.Contains('╔'));
        Assert.Contains(after, static row => row.StartsWith('+') && row.EndsWith('+'));
    }

    [Fact]
    public void WindowKeyObserve_LanguageChosen_SavesAndRelabels()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        foreach (ConsoleKeyInfo key in (ConsoleKeyInfo[])
        [
            new('o', ConsoleKey.O, false, true, false),
            new('\0', ConsoleKey.DownArrow, false, false, false),
            new('\0', ConsoleKey.DownArrow, false, false, false),
            new('\0', ConsoleKey.DownArrow, false, false, false),
            new('\0', ConsoleKey.RightArrow, false, false, false),
            new('\0', ConsoleKey.DownArrow, false, false, false),
            new('\r', ConsoleKey.Enter, false, false, false),
        ])
        {
            console.TConsoleFakeKeys.Add(key);
            console.TConsoleFakeAdjust(80, 24);
        }

        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));
        string? stored = null;

        TEnvoy.TEnvoyRun(console, static _ => null, settled: engine => stored = engine.TEngineLocalizationRead());

        Assert.Equal("ko", stored);
        Assert.Contains("Options", console.TConsoleFakeShots[0][0], StringComparison.Ordinal);
        Assert.Contains("옵션(O)", console.TConsoleFakeShots[^1][0], StringComparison.Ordinal);
    }

    [Fact]
    public void RoofKeyObserve_UpInSubmenu_MovesInsideSubmenu()
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
                window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false));
                window.BWindowKeyObserve(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
            },
            themes: name =>
            {
                asked.Add(name);
                return spare;
            });

        Assert.Equal(["console-blue"], asked);
    }

    [Fact]
    public void WindowSizeObserve_AsciiChosen_DrawsAsciiNotice()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(
            console,
            static _ => null,
            prime: static window =>
            {
                TCabinetAsciiSelect(window);
                window.BWindowSizeObserve(64, 18);
            });

        string[] notice = [.. console.TConsoleFakeShots[^1].Take(18).Select(static row => row[..64])];
        string side = new(' ', 11);
        Assert.Equal($"{side}+{new string('-', 17)} Llyn {new string('-', 17)}+{side}", notice[5]);
        Assert.Equal($"{side}|  {"Llyn needs a console of at least 80",-36}  |{side}", notice[7]);
        Assert.Equal($"{side}+{new string('-', 40)}+{side}", notice[11]);
        Assert.DoesNotContain(notice, static row => row.IndexOfAny(['║', '═', '╔', '╚']) >= 0);
    }

    [Fact]
    public void EnvoyUnionConfirm_AsciiChosen_DrawsAsciiArrow()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        object? answer = TEnvoy.TEnvoyRun(
            console,
            static envoy => envoy.CEnvoyUnionConfirm("Guild.MergeConfirm", "Ann", "Bob"),
            prime: TCabinetAsciiSelect);

        string[] shot = console.TConsoleFakeShots.Last(
            static shot => shot.Any(static row => row.Contains("Ann", StringComparison.Ordinal)));
        Assert.Equal(true, answer);
        Assert.Contains(shot, static row => row.Contains($"|  {"Ann -> Bob",-52}  |", StringComparison.Ordinal));
        Assert.DoesNotContain(shot, static row => row.Contains('→'));
    }

    private static void TCabinetAsciiSelect(BWindow window)
    {
        window.BWindowKeyObserve(new ConsoleKeyInfo('o', ConsoleKey.O, false, true, false));
        window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));
        window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false));
        window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));
        window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));
        window.BWindowKeyObserve(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
    }

    [Fact]
    public void WindowKeyObserve_SettingsChosen_SelectsSettingsTab()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        foreach (ConsoleKeyInfo key in (ConsoleKeyInfo[])
        [
            new('o', ConsoleKey.O, false, true, false),
            new('s', ConsoleKey.S, false, false, false),
        ])
        {
            console.TConsoleFakeKeys.Add(key);
            console.TConsoleFakeAdjust(80, 24);
        }

        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        TEnvoy.TEnvoyRun(console, static _ => null);

        Assert.DoesNotContain(" Settings ", console.TConsoleFakeShots[0][0], StringComparison.Ordinal);
        Assert.Contains(" Settings ", console.TConsoleFakeShots[^1][0], StringComparison.Ordinal);
    }

    [Fact]
    public void WindowKeyObserve_ThemeChosen_RedrawsWholeWindow()
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        foreach (ConsoleKeyInfo key in (ConsoleKeyInfo[])
        [
            new('o', ConsoleKey.O, false, true, false),
            new('\0', ConsoleKey.RightArrow, false, false, false),
            new('\0', ConsoleKey.DownArrow, false, false, false),
            new('\r', ConsoleKey.Enter, false, false, false),
        ])
        {
            console.TConsoleFakeKeys.Add(key);
            console.TConsoleFakeAdjust(80, 24);
        }

        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));
        BTheme dark = new(static _ => "green on black");
        List<string> asked = [];

        TEnvoy.TEnvoyRun(console, static _ => null, themes: name =>
        {
            asked.Add(name);
            return dark;
        });

        string spare = new BTheme(BTheme.BThemeSpareRead).BThemeRead("text");
        string after = console.TConsoleFakeFrames.First(
            frame => frame.Contains(dark.BThemeRead("text"), StringComparison.Ordinal));
        int[,] written = new int[24, 80];
        BCanvasWidth measure = new(false);
        (int row, int column) = (0, 0);
        for (int index = 0; index < after.Length;)
        {
            if (after[index] == '\e')
            {
                int end = index + 2;
                while (after[end] is >= '0' and <= '?')
                {
                    end++;
                }

                if (after[end] == 'H')
                {
                    string[] place = after[(index + 2)..end].Split(';');
                    (row, column) = (int.Parse(place[0], CultureInfo.InvariantCulture) - 1,
                        int.Parse(place[1], CultureInfo.InvariantCulture) - 1);
                }

                index = end + 1;
                continue;
            }

            int length = StringInfo.GetNextTextElementLength(after, index);
            int cells = measure.BCanvasWidthResolve(after.Substring(index, length));
            for (int cell = column; cell < column + cells; cell++)
            {
                written[row, cell]++;
            }

            (column, index) = (column + cells, index + length);
        }

        Assert.Equal(["console-dark"], asked);
        Assert.Contains(spare, console.TConsoleFakeFrames[0], StringComparison.Ordinal);
        Assert.DoesNotContain(spare, after, StringComparison.Ordinal);
        Assert.All(written.Cast<int>(), static count => Assert.Equal(1, count));
        Assert.Contains("Llyn", console.TConsoleFakeShots[^1][0], StringComparison.Ordinal);
    }
}
