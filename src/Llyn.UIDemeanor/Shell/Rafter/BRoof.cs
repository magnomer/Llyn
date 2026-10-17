using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.UIDemeanor;

internal sealed class BRoof
{
    private readonly BBlueprintPart _bRoofWindow;
    private readonly BBlueprintPart _bRoofPart;
    private readonly IReadOnlyList<BRoofMenu> _bRoofMenus;
    private int? _bRoofShown;
    private BDrop? _bRoofDrop;

    internal BRoof(BBlueprintPart window, IReadOnlyList<BRoofMenu> menus)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(menus);

        _bRoofWindow = window;
        _bRoofPart = BContract.BContractFind(window, "ARoof");
        _bRoofMenus = [.. menus.Where(static menu => menu.BRoofMenuItems.Any(static item => item is not null))];

        const BKeymapFocus closed = BKeymapFocus.BKeymapFocusWindow;
        const BKeymapFocus roof = BKeymapFocus.BKeymapFocusRoof;
        Func<BKeymapLive> shown = static () => BKeymapLive.BKeymapLiveShown;
        Action<ConsoleKeyInfo> close = _ => BRoofClose();
        Action<BRoofItem?> run = chosen =>
        {
            if (chosen is BRoofItem item && item.BRoofItemLive())
            {
                BRoofClose();
                item.BRoofItemAction();
            }
        };
        Action<int> change = step =>
        {
            if (_bRoofShown is int open)
            {
                BRoofOpen((open + step + _bRoofMenus.Count) % _bRoofMenus.Count);
            }
        };
        Action<int> move = step =>
        {
            IReadOnlyList<BRoofItem?> items = _bRoofShown is int open ? _bRoofMenus[open].BRoofMenuItems : [];
            for (int turn = 1; _bRoofDrop is BDrop drop && turn <= items.Count; turn++)
            {
                int at = (((drop.BDropLit + (step * turn)) % items.Count) + items.Count) % items.Count;
                if (items[at] is BRoofItem item && item.BRoofItemLive())
                {
                    drop.BDropLitMove(at - drop.BDropLit);
                    return;
                }
            }
        };
        List<BKeymapRow> rows =
        [
            new(
                ConsoleKey.F9,
                0,
                closed,
                "Legend.Menu",
                _ =>
                {
                    if (_bRoofMenus.Count > 0)
                    {
                        BRoofOpen(0);
                    }
                },
                () => _bRoofMenus.Count > 0 ? BKeymapLive.BKeymapLiveShown : BKeymapLive.BKeymapLiveHidden),
            new(ConsoleKey.F9, 0, roof, "Legend.Close", close, shown),
            new(ConsoleKey.Escape, 0, roof, "Legend.Close", close, shown),
            new(ConsoleKey.LeftArrow, 0, roof, "Window.Help.Menu", _ => change(-1), shown),
            new(ConsoleKey.RightArrow, 0, roof, "Window.Help.Menu", _ => change(1), shown),
            new(ConsoleKey.UpArrow, 0, roof, "Window.Help.Item", _ => move(-1), shown),
            new(ConsoleKey.DownArrow, 0, roof, "Window.Help.Item", _ => move(1), shown),
            new(
                ConsoleKey.Enter,
                0,
                roof,
                "Window.Help.Run",
                _ => run(_bRoofShown is int open && _bRoofDrop is BDrop drop
                    ? _bRoofMenus[open].BRoofMenuItems.ElementAtOrDefault(drop.BDropLit)
                    : null),
                shown),
        ];
        for (int index = 0; index < _bRoofMenus.Count; index++)
        {
            int opened = index;
            if (_bRoofMenus[index].BRoofMenuHotkey is ConsoleKey letter
                && !rows.Exists(row => row.BKeymapRowKey == letter && row.BKeymapRowModifiers == ConsoleModifiers.Alt))
            {
                rows.Add(new(letter, ConsoleModifiers.Alt, roof, "Window.Help.Open", _ => BRoofOpen(opened), shown));
            }
        }

        IEnumerable<ConsoleKey> hotkeys = _bRoofMenus
            .SelectMany(static menu => menu.BRoofMenuItems)
            .Select(static item => item?.BRoofItemHotkey)
            .OfType<ConsoleKey>()
            .Distinct();
        foreach (ConsoleKey hotkey in hotkeys)
        {
            rows.Add(new(
                hotkey,
                0,
                roof,
                "Window.Help.Run",
                _ => run(_bRoofShown is int open
                    ? _bRoofMenus[open].BRoofMenuItems.FirstOrDefault(item => item?.BRoofItemHotkey == hotkey)
                    : null),
                shown));
        }

        BRoofKeymap = new BKeymap(rows);
    }

    internal BKeymap BRoofKeymap { get; }

    internal BKeymapFocus BRoofKeymapFocus =>
        _bRoofShown is null ? BKeymapFocus.BKeymapFocusWindow : BKeymapFocus.BKeymapFocusRoof;

    internal void BRoofOpen(int menu)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(menu);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(menu, _bRoofMenus.Count);

        IReadOnlyList<BRoofItem?> items = _bRoofMenus[menu].BRoofMenuItems;
        int top = _bRoofPart.BBlueprintPartRow + 1;
        _bRoofShown = menu;
        _bRoofDrop = new BDrop(
            [.. items.Select(static item => item?.BRoofItemLabel ?? string.Empty)],
            items.ToList().FindIndex(static item => item is not null && item.BRoofItemLive()),
            true,
            Math.Max(_bRoofWindow.BBlueprintPartHeight - top - 3, 1));
    }

    internal void BRoofClose()
    {
        _bRoofShown = null;
        _bRoofDrop = null;
    }

    internal bool BRoofKeyObserve(ConsoleKeyInfo key)
    {
        BKeymapRow? row = BRoofKeymap.BKeymapFind(BRoofKeymapFocus, key)
            ?? (key.Modifiers.HasFlag(ConsoleModifiers.Alt)
                ? BRoofKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusRoof, key)
                : null);
        if (row is null)
        {
            return false;
        }

        row.BKeymapRowAction(key);
        return true;
    }

    internal IReadOnlyList<BCanvasText> BRoofDraw(IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs)
    {
        ArgumentNullException.ThrowIfNull(glyphs);

        BBlueprintPart part = _bRoofPart;
        (int row, int column, int width) =
            (part.BBlueprintPartRow, part.BBlueprintPartColumn, part.BBlueprintPartWidth);
        string theme = part.BBlueprintPartTheme;
        BCanvasWidth measure = new(false);
        List<BCanvasText> texts = [new(row, column, width, new string(' ', width), theme)];
        int at = column + 1;
        for (int index = 0; index < _bRoofMenus.Count; index++)
        {
            BRoofMenu menu = _bRoofMenus[index];
            string title = $" {menu.BRoofMenuTitle} ";
            int cells = measure.BCanvasWidthResolve(title);
            if (at + cells > column + width)
            {
                break;
            }

            bool open = index == _bRoofShown;
            texts.Add(new(row, at, cells, title, open ? "lit-other" : theme));
            int spot = menu.BRoofMenuHotkey is ConsoleKey letter
                ? title.IndexOf(((char)letter).ToString(), StringComparison.OrdinalIgnoreCase)
                : -1;
            if (!open && spot >= 0)
            {
                int cell = at + measure.BCanvasWidthResolve(title.AsSpan(0, spot));
                texts.Add(new(row, cell, 1, title[spot..(spot + 1)], "menu-hotkey"));
            }

            if (open && _bRoofDrop is BDrop drop)
            {
                texts.AddRange(BRoofBuild(menu, drop, row + 1, at, glyphs));
            }

            at += cells;
        }

        return texts;
    }

    private IEnumerable<BCanvasText> BRoofBuild(
        BRoofMenu menu, BDrop drop, int top, int left, IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs)
    {
        BCanvasWidth measure = new(false);
        IReadOnlyList<BRoofItem?> items = menu.BRoofMenuItems;
        string[] keys = [.. items.Select(static item => item?.BRoofItemKey?.ToString() ?? string.Empty)];
        int label = items.Max(item => item is null ? 0 : measure.BCanvasWidthResolve(item.BRoofItemLabel));
        int key = keys.Max(text => measure.BCanvasWidthResolve(text));
        int inner = label + key + (key > 0 ? 5 : 2);
        int across = inner + 2;
        int right = _bRoofPart.BBlueprintPartColumn + _bRoofPart.BBlueprintPartWidth;
        left = Math.Max(Math.Min(left, right - across - 1), _bRoofPart.BBlueprintPartColumn);
        IReadOnlyList<string> frame = glyphs.GetValueOrDefault("Frame") is [_, _, _, _, _, _] marks
            ? marks
            : ["+", "-", "+", "|", "+", "+"];
        IReadOnlyList<string> rule = glyphs.GetValueOrDefault("Rule") is [_, _, _] divider ? divider : ["+", "-", "+"];
        string bar = string.Concat(Enumerable.Repeat(frame[1], inner));
        yield return new(top, left, across, $"{frame[0]}{bar}{frame[2]}", "menu");
        int[] shown = [.. drop.BDropLabelShown];
        foreach (int index in shown)
        {
            int line = top + 1 + index - drop.BDropTop;
            if (items[index] is not BRoofItem item)
            {
                string middle = string.Concat(Enumerable.Repeat(rule[1], inner));
                yield return new(line, left, across, $"{rule[0]}{middle}{rule[2]}", "menu");
                continue;
            }

            bool live = item.BRoofItemLive();
            bool lit = live && index == drop.BDropLit;
            string name = item.BRoofItemLabel;
            int space = inner - 2 - measure.BCanvasWidthResolve(name) - measure.BCanvasWidthResolve(keys[index]);
            string body = $" {name}{new string(' ', space)}{keys[index]} ";
            yield return new(line, left, 1, frame[3], "menu");
            yield return new(line, left + 1, inner, body, !live ? "disabled" : lit ? "lit-other" : "menu");
            yield return new(line, left + across - 1, 1, frame[3], "menu");
            int spot = item.BRoofItemHotkey is ConsoleKey letter
                ? name.IndexOf(((char)letter).ToString(), StringComparison.OrdinalIgnoreCase)
                : -1;
            if (live && !lit && spot >= 0)
            {
                int cell = left + 2 + measure.BCanvasWidthResolve(name.AsSpan(0, spot));
                yield return new(line, cell, 1, name[spot..(spot + 1)], "menu-hotkey");
            }
        }

        int down = shown.Length + 2;
        yield return new(top + down - 1, left, across, $"{frame[4]}{bar}{frame[5]}", "menu");
        for (int line = top + 1; line < top + down; line++)
        {
            yield return new(line, left + across, 1, " ", "shadow");
        }

        yield return new(top + down, left + 1, across, new string(' ', across), "shadow");
    }
}
