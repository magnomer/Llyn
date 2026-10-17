using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.UIDemeanor;

internal sealed class BPager
{
    private readonly Func<IReadOnlyList<IReadOnlyList<int?>>> _bPagerTarget;
    private readonly int _bPagerView;
    private readonly Stack<(int, int, int)> _bPagerBack = new();

    internal BPager(Func<IReadOnlyList<IReadOnlyList<int?>>> targets, int page, int view)
    {
        ArgumentNullException.ThrowIfNull(targets);
        ArgumentOutOfRangeException.ThrowIfNegative(page);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(view);

        _bPagerTarget = targets;
        _bPagerView = view;
        BPagerPage = page;

        const BKeymapFocus help = BKeymapFocus.BKeymapFocusHelp;
        Func<BKeymapLive> shown = static () => BKeymapLive.BKeymapLiveShown;
        BPagerKeymap = new BKeymap(
        [
            new(ConsoleKey.UpArrow, 0, help, "Window.Help.Scroll", _ => BPagerScroll(BPagerTop - 1), shown),
            new(ConsoleKey.DownArrow, 0, help, "Window.Help.Scroll", _ => BPagerScroll(BPagerTop + 1), shown),
            new(ConsoleKey.PageUp, 0, help, "Window.Help.Scroll", _ => BPagerScroll(BPagerTop - view), shown),
            new(ConsoleKey.PageDown, 0, help, "Window.Help.Scroll", _ => BPagerScroll(BPagerTop + view), shown),
            new(ConsoleKey.Home, 0, help, "Window.Help.Scroll", _ => BPagerScroll(0), shown),
            new(ConsoleKey.End, 0, help, "Window.Help.Scroll", _ => BPagerScroll(int.MaxValue), shown),
            new(ConsoleKey.Tab, 0, help, "Window.Help.Link", _ => BPagerLitMove(1), shown),
            new(ConsoleKey.Tab, ConsoleModifiers.Shift, help, "Window.Help.Link", _ => BPagerLitMove(-1), shown),
            new(ConsoleKey.Enter, 0, help, "Window.Help.Follow", _ => BPagerLitShow(), shown),
            new(ConsoleKey.Backspace, 0, help, "Window.Help.Back", _ => BPagerRetreat(), shown),
        ]);
    }

    internal BKeymap BPagerKeymap { get; }

    internal int BPagerPage { get; private set; }

    internal int BPagerTop { get; private set; }

    internal int BPagerLit { get; private set; }

    internal bool BPagerObserve(ConsoleKeyInfo key)
    {
        if (BPagerKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusHelp, key) is not BKeymapRow row)
        {
            return false;
        }

        row.BKeymapRowAction(key);
        return true;
    }

    internal void BPagerShow(int page)
    {
        IReadOnlyList<IReadOnlyList<int?>> targets = _bPagerTarget();
        ArgumentOutOfRangeException.ThrowIfNegative(page);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(page, targets.Count);

        if (page == BPagerPage)
        {
            return;
        }

        _bPagerBack.Push((BPagerPage, BPagerTop, BPagerLit));
        (BPagerPage, BPagerTop, BPagerLit) = (page, 0, 0);
    }

    private void BPagerScroll(int top)
    {
        int lines = _bPagerTarget()[BPagerPage].Count;
        BPagerTop = Math.Clamp(top, 0, Math.Max(lines - _bPagerView, 0));
    }

    private void BPagerLitMove(int step)
    {
        IReadOnlyList<int?> lines = _bPagerTarget()[BPagerPage];
        int[] links = [.. Enumerable.Range(0, lines.Count).Where(line => lines[line] is not null)];
        if (links.Length == 0)
        {
            return;
        }

        BPagerLit = (BPagerLit + step + links.Length) % links.Length;
        BPagerScroll(Math.Clamp(BPagerTop, links[BPagerLit] - _bPagerView + 1, links[BPagerLit]));
    }

    private void BPagerLitShow()
    {
        IReadOnlyList<int?> lines = _bPagerTarget()[BPagerPage];
        int?[] links = [.. lines.Where(static target => target is not null)];
        if (links is [_, ..] && links[BPagerLit] is int target)
        {
            BPagerShow(target);
        }
    }

    private void BPagerRetreat()
    {
        if (_bPagerBack.TryPop(out (int, int, int) place))
        {
            (BPagerPage, BPagerTop, BPagerLit) = place;
        }
    }
}
