using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BSNavigation : BSParleyBody
{
    private readonly IReadOnlyList<KeyValuePair<string, string>> _bsNavigationTabs;
    private readonly BDrop _bsNavigationDrop;

    internal BSNavigation(
        BLocalizationCatalog catalog,
        IReadOnlyList<IReadOnlyList<KeyValuePair<string, string>>> tabs,
        CNavigationState state,
        Action<string> choose)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(tabs);
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(choose);

        const string hotkeys = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        _bsNavigationTabs = [.. tabs.SelectMany(static group => group)
            .Where(tab => !state.CNavigationStateHidden.Contains(tab.Key, StringComparer.Ordinal))
            .Take(hotkeys.Length)
            .Select((tab, index) => KeyValuePair.Create(
                tab.Key, $"{hotkeys[index]} {catalog.BLocalizationTextRead(tab.Value)}"))];
        int count = _bsNavigationTabs.Count;
        _bsNavigationDrop = new BDrop(
            [.. _bsNavigationTabs.Select(static tab => tab.Value)],
            Math.Max(_bsNavigationTabs.ToList().FindIndex(tab =>
                string.Equals(tab.Key, state.CNavigationStateTab, StringComparison.Ordinal)), 0),
            true,
            Math.Max(count, 1));
        int down = (count + 1) / 2;
        const BKeymapFocus focus = BKeymapFocus.BKeymapFocusParley;
        Func<BKeymapLive> shown = static () => BKeymapLive.BKeymapLiveShown;
        Action<int> run = index =>
        {
            if (index >= 0 && index < count)
            {
                _bsNavigationDrop.BDropLitMove(index - _bsNavigationDrop.BDropLit);
                choose(_bsNavigationTabs[index].Key);
            }
        };
        Action<int> move = step =>
        {
            int at = _bsNavigationDrop.BDropLit + step;
            if (at >= 0 && at < count)
            {
                _bsNavigationDrop.BDropLitMove(step);
            }
        };
        List<BKeymapRow> rows =
        [
            new(ConsoleKey.UpArrow, 0, focus, "Window.Help.Item", _ => move(-1), shown),
            new(ConsoleKey.DownArrow, 0, focus, "Window.Help.Item", _ => move(1), shown),
            new(ConsoleKey.LeftArrow, 0, focus, "Window.Help.Item", _ => move(-down), shown),
            new(ConsoleKey.RightArrow, 0, focus, "Window.Help.Item", _ => move(down), shown),
            new(ConsoleKey.Enter, 0, focus, "Window.Help.Run", _ => run(_bsNavigationDrop.BDropLit), shown),
        ];
        for (int index = 0; index < count; index++)
        {
            char hotkey = hotkeys[index];
            ConsoleKey key = char.IsAsciiDigit(hotkey) ? ConsoleKey.D0 + (hotkey - '0') : ConsoleKey.A + (hotkey - 'A');
            int chosen = index;
            rows.Add(new(key, 0, focus, "Window.Help.Run", _ => run(chosen), shown));
        }

        BSNavigationKeymap = new BKeymap(rows);
    }

    public int BSParleyBodyWidth
    {
        get
        {
            BCanvasWidth measure = new(false);
            int cells = _bsNavigationTabs.Select(tab => measure.BCanvasWidthResolve(tab.Value)).DefaultIfEmpty(0).Max();
            return (cells * 2) + 9;
        }
    }

    public int BSParleyBodyHeight => (_bsNavigationTabs.Count + 1) / 2;

    public string BSParleyBodyPosition => string.Empty;

    internal BKeymap BSNavigationKeymap { get; }

    public bool BSParleyBodyObserve(ConsoleKeyInfo key)
    {
        if (BSNavigationKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusParley, key) is not BKeymapRow row)
        {
            return false;
        }

        row.BKeymapRowAction(key);
        return true;
    }

    public IEnumerable<BCanvasText> BSParleyBodyDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        int row,
        int column,
        int width,
        bool focused,
        string role)
    {
        int cells = Math.Clamp((width - 3) / 2, 0, (BSParleyBodyWidth - 9) / 2);
        int down = BSParleyBodyHeight;
        for (int index = 0; index < _bsNavigationTabs.Count; index++)
        {
            bool lit = focused && index == _bsNavigationDrop.BDropLit;
            yield return new BCanvasText(
                row + (index % down),
                column + (index / down * (cells + 3)),
                cells,
                _bsNavigationTabs[index].Value,
                lit ? "lit" : role);
        }
    }
}
