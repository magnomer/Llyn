using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Llyn.UIDemeanor;

internal sealed class BRoof
{
    private readonly BBlueprintPart _bRoofWindow;
    private readonly BBlueprintPart _bRoofPart;
    private IReadOnlyList<BRoofMenu> _bRoofMenus;
    private int? _bRoofShown;
    private BDrop? _bRoofDrop;
    private BDrop? _bRoofSubmenu;

    internal BRoof(BBlueprintPart window, IReadOnlyList<BRoofMenu> menus)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(menus);

        _bRoofWindow = window;
        _bRoofPart = BContract.BContractFind(window, "ARoof");
        BRoofApply(menus);
    }

    internal BKeymap BRoofKeymap { get; private set; }

    internal BKeymapFocus BRoofKeymapFocus =>
        _bRoofShown is null ? BKeymapFocus.BKeymapFocusWindow : BKeymapFocus.BKeymapFocusRoof;

    [MemberNotNull(nameof(_bRoofMenus), nameof(BRoofKeymap))]
    internal void BRoofApply(IReadOnlyList<BRoofMenu> menus)
    {
        ArgumentNullException.ThrowIfNull(menus);

        (int? shown, BDrop? drop, BDrop? submenu) = (_bRoofShown, _bRoofDrop, _bRoofSubmenu);
        BRoofMenu? before = shown is int was && _bRoofMenus is { } held ? held[was] : null;
        _bRoofMenus = [.. menus.Where(static menu => menu.BRoofMenuItems.Any(static item => item is not null))];
        BRoofKeymap = BRoofKeymapBuild();
        BRoofClose();
        if (shown is not int old || before is null || drop is null)
        {
            return;
        }

        int titled = _bRoofMenus.ToList().FindIndex(menu => menu.BRoofMenuTitle == before.BRoofMenuTitle);
        int open = titled >= 0 ? titled : old;
        if (open >= _bRoofMenus.Count)
        {
            return;
        }

        Func<IReadOnlyList<BRoofItem?>, IReadOnlyList<BRoofItem?>, BDrop, BDrop, BRoofItem?> keep =
            (earlier, items, past, fresh) =>
            {
                string? label = earlier.ElementAtOrDefault(past.BDropLit)?.BRoofItemLabel;
                int named = items.ToList().FindIndex(item => item is not null && item.BRoofItemLabel == label);
                int at = named >= 0 ? named : past.BDropLit;
                if (items.ElementAtOrDefault(at) is not BRoofItem item || !item.BRoofItemLive())
                {
                    return null;
                }

                fresh.BDropLitMove(at - fresh.BDropLit);
                return item;
            };
        BRoofOpen(open);
        IReadOnlyList<BRoofItem?> items = _bRoofMenus[open].BRoofMenuItems;
        if (_bRoofDrop is not BDrop opened
            || keep(before.BRoofMenuItems, items, drop, opened) is not { BRoofItemSubmenu: [_, ..] inner }
            || submenu is null
            || before.BRoofMenuItems.ElementAtOrDefault(drop.BDropLit)?.BRoofItemSubmenu is not { } earlier)
        {
            return;
        }

        BDrop nested = BRoofDropBuild(inner, opened.BDropLit - opened.BDropTop);
        _bRoofSubmenu = nested;
        _ = keep(earlier, inner, submenu, nested);
    }

    private BKeymap BRoofKeymapBuild()
    {
        const BKeymapFocus closed = BKeymapFocus.BKeymapFocusWindow;
        const BKeymapFocus roof = BKeymapFocus.BKeymapFocusRoof;
        Func<BKeymapLive> shown = static () => BKeymapLive.BKeymapLiveShown;
        Action<ConsoleKeyInfo> close = _ => BRoofClose();
        Func<IReadOnlyList<BRoofItem?>> level = () =>
            _bRoofShown is not int open ? []
            : _bRoofSubmenu is not null && _bRoofDrop is BDrop parent
                ? _bRoofMenus[open].BRoofMenuItems[parent.BDropLit]?.BRoofItemSubmenu ?? []
            : _bRoofMenus[open].BRoofMenuItems;
        Func<int> lit = () => (_bRoofSubmenu ?? _bRoofDrop)?.BDropLit ?? -1;
        Action<int> branch = at =>
        {
            if (_bRoofDrop is not BDrop drop
                || level()[at] is not { BRoofItemSubmenu: [_, ..] and IReadOnlyList<BRoofItem?> inner })
            {
                return;
            }

            drop.BDropLitMove(at - drop.BDropLit);
            _bRoofSubmenu = BRoofDropBuild(inner, drop.BDropLit - drop.BDropTop);
        };
        Action<int> run = at =>
        {
            if (level().ElementAtOrDefault(at) is not BRoofItem item || !item.BRoofItemLive())
            {
                return;
            }

            if (_bRoofSubmenu is null && item.BRoofItemSubmenu is not null)
            {
                branch(at);
                return;
            }

            BRoofClose();
            item.BRoofItemAction();
        };
        Action<int> change = step =>
        {
            if (_bRoofShown is int open)
            {
                BRoofOpen((open + step + _bRoofMenus.Count) % _bRoofMenus.Count);
            }
        };
        Func<bool> fold = () =>
        {
            bool folded = _bRoofSubmenu is not null;
            _bRoofSubmenu = null;
            return folded;
        };
        Action<ConsoleKeyInfo> right = _ =>
        {
            if (_bRoofSubmenu is null && level().ElementAtOrDefault(lit()) is { BRoofItemSubmenu: not null } item
                && item.BRoofItemLive())
            {
                branch(lit());
                return;
            }

            change(1);
        };
        Action<int> move = step =>
        {
            IReadOnlyList<BRoofItem?> items = level();
            for (int turn = 1; (_bRoofSubmenu ?? _bRoofDrop) is BDrop drop && turn <= items.Count; turn++)
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
            new(ConsoleKey.Escape, 0, roof, "Legend.Close", key => { if (!fold()) { close(key); } }, shown),
            new(ConsoleKey.LeftArrow, 0, roof, "Window.Help.Menu", _ => { if (!fold()) { change(-1); } }, shown),
            new(ConsoleKey.RightArrow, 0, roof, "Window.Help.Menu", right, shown),
            new(ConsoleKey.UpArrow, 0, roof, "Window.Help.Item", _ => move(-1), shown),
            new(ConsoleKey.DownArrow, 0, roof, "Window.Help.Item", _ => move(1), shown),
            new(ConsoleKey.Enter, 0, roof, "Window.Help.Run", _ => run(lit()), shown),
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
            .SelectMany(static item => (IEnumerable<BRoofItem?>)[item, .. item?.BRoofItemSubmenu ?? []])
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
                _ => run(level().ToList().FindIndex(item => item?.BRoofItemHotkey == hotkey)),
                shown));
        }

        return new BKeymap(rows);
    }

    internal void BRoofOpen(int menu)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(menu);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(menu, _bRoofMenus.Count);

        _bRoofShown = menu;
        _bRoofDrop = BRoofDropBuild(_bRoofMenus[menu].BRoofMenuItems, 0);
        _bRoofSubmenu = null;
    }

    internal void BRoofClose()
    {
        _bRoofShown = null;
        _bRoofDrop = null;
        _bRoofSubmenu = null;
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

    internal IReadOnlyList<BCanvasText> BRoofDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs, BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);

        BBlueprintPart part = _bRoofPart;
        (int row, int column, int width) =
            (part.BBlueprintPartRow, part.BBlueprintPartColumn, part.BBlueprintPartWidth);
        string theme = part.BBlueprintPartTheme;
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
                BCanvasText[] box = [.. BRoofBuild(menu.BRoofMenuItems, drop, row + 1, at, glyphs, measure)];
                texts.AddRange(box);
                if (_bRoofSubmenu is BDrop inner
                    && menu.BRoofMenuItems[drop.BDropLit]?.BRoofItemSubmenu is IReadOnlyList<BRoofItem?> nested)
                {
                    int line = row + 1 + drop.BDropLit - drop.BDropTop;
                    int edge = box[0].BCanvasTextColumn + box[0].BCanvasTextSpan;
                    texts.AddRange(BRoofBuild(nested, inner, line, edge, glyphs, measure));
                }
            }

            at += cells;
        }

        return texts;
    }

    private BDrop BRoofDropBuild(IReadOnlyList<BRoofItem?> items, int offset)
    {
        int top = _bRoofPart.BBlueprintPartRow + 1 + offset;
        return new BDrop(
            [.. items.Select(static item => item?.BRoofItemLabel ?? string.Empty)],
            items.ToList().FindIndex(static item => item is not null && item.BRoofItemLive()),
            true,
            Math.Max(_bRoofWindow.BBlueprintPartHeight - top - 3, 1));
    }

    private IEnumerable<BCanvasText> BRoofBuild(
        IReadOnlyList<BRoofItem?> items,
        BDrop drop,
        int top,
        int left,
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure)
    {
        string branch = glyphs.GetValueOrDefault("Submenu") is [string arrow] ? arrow : ">";
        string[] keys = [.. items.Select(item => item?.BRoofItemKey?.ToString()
            ?? (item?.BRoofItemSubmenu is null ? string.Empty : branch))];
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
