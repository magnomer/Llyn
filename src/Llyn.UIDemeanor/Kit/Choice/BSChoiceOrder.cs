using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BSChoiceOrder : BSParleyBody
{
    private readonly IReadOnlyList<(CCatalogOrder, string, string)> _bsChoiceOrderRows;
    private readonly Func<string> _bsChoiceOrderApplied;
    private int _bsChoiceOrderLit;

    internal BSChoiceOrder(
        BLocalizationCatalog catalog,
        string prefix,
        IReadOnlyList<CCatalogOrder> orders,
        Func<string> applied,
        Action<CCatalogOrder> choose,
        BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(prefix);
        ArgumentNullException.ThrowIfNull(orders);
        ArgumentNullException.ThrowIfNull(applied);
        ArgumentNullException.ThrowIfNull(choose);
        ArgumentNullException.ThrowIfNull(measure);

        _bsChoiceOrderRows = [.. orders.Select(order => (order, CCatalog.CCatalogOrderFormat(order)))
            .Select(row => (row.order, row.Item2, catalog.BLocalizationTextRead(prefix + "." + row.Item2)))];
        _bsChoiceOrderApplied = applied;
        _bsChoiceOrderLit = Math.Max(_bsChoiceOrderRows.Select(row => row.Item2).ToList().IndexOf(applied()), 0);
        int count = _bsChoiceOrderRows.Count;
        BSParleyBodyWidth = _bsChoiceOrderRows.Select(row => measure.BCanvasWidthResolve(row.Item3))
            .DefaultIfEmpty(0)
            .Max() + 10;
        const BKeymapFocus focus = BKeymapFocus.BKeymapFocusParley;
        Func<BKeymapLive> live = () => count > 0 ? BKeymapLive.BKeymapLiveShown : BKeymapLive.BKeymapLiveHidden;
        Action<int> move = step => _bsChoiceOrderLit = Math.Clamp(_bsChoiceOrderLit + step, 0, Math.Max(count - 1, 0));
        BSParleyBodyKeymap = new BKeymap(
        [
            new(ConsoleKey.UpArrow, 0, focus, "Window.Help.Item", _ => move(-1), live),
            new(ConsoleKey.DownArrow, 0, focus, "Window.Help.Item", _ => move(1), live),
            new(ConsoleKey.Enter, 0, focus, "Window.Help.Run", _ => choose(_bsChoiceOrderRows[_bsChoiceOrderLit].Item1),
                live),
        ]);
    }

    public int BSParleyBodyWidth { get; }

    public int BSParleyBodyHeight => _bsChoiceOrderRows.Count;

    public string BSParleyBodyPosition => string.Empty;

    public BKeymap BSParleyBodyKeymap { get; }

    public bool BSParleyBodyObserve(ConsoleKeyInfo key)
    {
        if (BSParleyBodyKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusParley, key) is not BKeymapRow row)
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
        int height,
        bool focused,
        string role,
        BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);

        string chosen = glyphs.GetValueOrDefault("Radio") is [string radio, ..] && radio.Length > 1 ? radio : "(*)";
        string blank =
            $"{chosen[0]}{new string(' ', Math.Max(measure.BCanvasWidthResolve(chosen) - 2, 0))}{chosen[^1]}";
        string applied = _bsChoiceOrderApplied();
        int top = Math.Max(_bsChoiceOrderLit - height + 1, 0);
        for (int line = 0; line < height && top + line < _bsChoiceOrderRows.Count; line++)
        {
            (_, string word, string label) = _bsChoiceOrderRows[top + line];
            bool lit = focused && top + line == _bsChoiceOrderLit;
            string text = $"{(word == applied ? chosen : blank)} {label}";
            yield return new BCanvasText(
                row + line, column, width, measure.BCanvasWidthClamp(text, width), lit ? "lit" : role);
        }
    }
}
