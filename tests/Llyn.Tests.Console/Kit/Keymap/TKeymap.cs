using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TKeymap
{
    private static IEnumerable<ConsoleKeyInfo> TKeymapPresses =>
        from key in Enum.GetValues<ConsoleKey>()
        from modifiers in Enumerable.Range(0, 8)
        select new ConsoleKeyInfo('\0', key, (modifiers & 1) != 0, (modifiers & 2) != 0, (modifiers & 4) != 0);

    [Fact]
    public void Keymap_AltLetterRow_Throws()
    {
        BKeymapRow row = new(
            ConsoleKey.E, ConsoleModifiers.Alt, BKeymapFocus.BKeymapFocusWindow, "Legend.Quit", static _ => { },
            static () => BKeymapLive.BKeymapLiveShown);

        Assert.Throws<ArgumentException>(() => new BKeymap([row]));
    }

    [Fact]
    public void Keymap_NeverBoundRow_Throws()
    {
        (ConsoleKey TKeymapKey, ConsoleModifiers TKeymapHeld)[] never =
        [
            (ConsoleKey.F11, 0),
            (ConsoleKey.F4, ConsoleModifiers.Alt),
            (ConsoleKey.Enter, ConsoleModifiers.Alt),
            (ConsoleKey.V, ConsoleModifiers.Control),
            (ConsoleKey.Tab, ConsoleModifiers.Control),
            .. Enumerable.Range((int)ConsoleKey.A, 26)
                .Where(static key => key != (int)ConsoleKey.Z)
                .Select(static key => ((ConsoleKey)key, ConsoleModifiers.Control | ConsoleModifiers.Shift)),
        ];

        foreach ((ConsoleKey key, ConsoleModifiers held) in never)
        {
            foreach (ConsoleModifiers modifiers in Enumerable.Range(0, 8).Select(static mix => (ConsoleModifiers)mix))
            {
                if ((modifiers & held) == held)
                {
                    BKeymapRow row = new(
                        key, modifiers, BKeymapFocus.BKeymapFocusWindow, "Legend.Quit", static _ => { },
                        static () => BKeymapLive.BKeymapLiveShown);
                    Assert.Throws<ArgumentException>(() => new BKeymap([row]));
                }
            }
        }
    }

    [Fact]
    public void KeymapFind_ControlChar_FindsInterrupt()
    {
        BKeymap keymap = TKeymapWindowCreate();

        BKeymapRow? row = keymap.BKeymapFind(
            BKeymapFocus.BKeymapFocusWindow, new ConsoleKeyInfo('\u0003', ConsoleKey.C, false, false, false));

        Assert.Equal(ConsoleModifiers.Control, row?.BKeymapRowModifiers);
    }

    [Fact]
    public void KeymapFind_NeverBoundKey_ReturnsNoRow()
    {
        BKeymap keymap = TKeymapWindowCreate();
        ConsoleKeyInfo[] never =
        [
            .. TKeymapPresses.Where(static press => press.Key == ConsoleKey.F11),
            new('\0', ConsoleKey.F4, false, true, false),
            new('\0', ConsoleKey.Enter, false, true, false),
            new('\0', ConsoleKey.V, false, false, true),
            new('\0', ConsoleKey.Tab, false, false, true),
            new('\0', ConsoleKey.Tab, true, false, true),
            .. Enumerable.Range((int)ConsoleKey.A, 26)
                .Where(static key => key != (int)ConsoleKey.Z)
                .Select(static key => new ConsoleKeyInfo('\0', (ConsoleKey)key, true, false, true)),
        ];

        foreach (BKeymapFocus focus in Enum.GetValues<BKeymapFocus>())
        {
            Assert.All(never, press => Assert.Null(keymap.BKeymapFind(focus, press)));
        }
    }

    [Fact]
    public void KeymapFind_AltLetter_ReturnsNoRow()
    {
        BKeymap keymap = TKeymapWindowCreate();
        ConsoleKeyInfo[] letters =
        [
            .. TKeymapPresses.Where(static press => press.Key is >= ConsoleKey.A and <= ConsoleKey.Z
                && press.Modifiers.HasFlag(ConsoleModifiers.Alt)),
        ];

        foreach (BKeymapFocus focus in Enum.GetValues<BKeymapFocus>())
        {
            Assert.All(letters, press => Assert.Null(keymap.BKeymapFind(focus, press)));
        }
    }

    [Fact]
    public void KeymapFind_GlobalKey_KeepsOneLabel()
    {
        BKeymap keymap = TKeymapWindowCreate();
        BKeymapFocus[] outside =
        [
            .. Enum.GetValues<BKeymapFocus>()
                .Except([BKeymapFocus.BKeymapFocusParley, BKeymapFocus.BKeymapFocusHelp]),
        ];

        Assert.Contains(BKeymapFocus.BKeymapFocusNotice, outside);
        foreach (ConsoleKeyInfo press in TKeymapPresses)
        {
            string[] labels =
            [
                .. outside
                    .Select(focus => keymap.BKeymapFind(focus, press))
                    .OfType<BKeymapRow>()
                    .Select(static row => row.BKeymapRowLabel)
                    .Distinct(),
            ];
            Assert.True(labels.Length <= 1, $"{press.Modifiers} {press.Key}: {string.Join(", ", labels)}");
        }
    }

    [Fact]
    public void KeymapFind_EveryRow_ResolvesLabel()
    {
        BKeymap keymap = TKeymapWindowCreate();
        string[] labels =
        [
            .. Enum.GetValues<BKeymapFocus>()
                .SelectMany(focus => TKeymapPresses.Select(press => keymap.BKeymapFind(focus, press)))
                .OfType<BKeymapRow>()
                .Select(static row => row.BKeymapRowLabel)
                .Append("Legend.Import")
                .Distinct(),
        ];

        Assert.NotEmpty(labels);
        foreach (string language in new[] { "en", "ko" })
        {
            IReadOnlyDictionary<string, string> texts = TInterface.TLocalizationLoad(language);
            Assert.All(labels, label => Assert.True(texts.ContainsKey(label), $"{language}: {label}"));
        }
    }

    private static BKeymap TKeymapWindowCreate()
    {
        TConsoleFake console = new();
        BLocalizationCatalog catalog = new();
        ABootstrap bootstrap = new();
        BWindow window = new(
            console,
            new BLoop(static _ => { }),
            catalog,
            null,
            new BEnvoy(console, catalog, bootstrap),
            bootstrap,
            new BTheme(BTheme.BThemeSpareRead));
        return window.BWindowKeymap;
    }
}
