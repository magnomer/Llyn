using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BSCompass : BSParleyBody
{
    private readonly IReadOnlyList<(string, bool, int)> _bsCompassRows;

    private readonly TaskCompletionSource _bsCompassChoice = new();

    private int _bsCompassLit;

    internal BSCompass(
        IReadOnlyList<(string, bool, int)> rows, int top, Action<int> scroll, string title, BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(scroll);
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(measure);

        _bsCompassRows = rows;
        _bsCompassLit = CCompass.CCompassLitRead([.. rows.Select(row => (row.Item2, row.Item3 <= top))]);
        int widest = rows
            .Select(row => measure.BCanvasWidthResolve(row.Item1))
            .DefaultIfEmpty(0)
            .Max();
        BSParleyBodyWidth = Math.Max(widest, measure.BCanvasWidthResolve(title)) + 6;
        if (_bsCompassLit < 0)
        {
            _bsCompassChoice.TrySetResult();
        }

        const BKeymapFocus focus = BKeymapFocus.BKeymapFocusParley;
        Func<BKeymapLive> live = () =>
            _bsCompassLit >= 0 ? BKeymapLive.BKeymapLiveShown : BKeymapLive.BKeymapLiveHidden;
        Action<int> move = step =>
        {
            do
            {
                _bsCompassLit = (_bsCompassLit + step + rows.Count) % rows.Count;
            }
            while (rows[_bsCompassLit].Item2);
        };
        Action<ConsoleKeyInfo> open = key =>
        {
            _bsCompassChoice.TrySetResult();
            scroll(_bsCompassRows[_bsCompassLit].Item3);
        };
        BSParleyBodyKeymap = new BKeymap(
        [
            new(ConsoleKey.UpArrow, 0, focus, "Window.Help.Item", _ => move(-1), live),
            new(ConsoleKey.DownArrow, 0, focus, "Window.Help.Item", _ => move(1), live),
            new(ConsoleKey.Enter, 0, focus, "Window.Help.Follow", open, live),
        ]);
    }

    public int BSParleyBodyWidth { get; }

    public int BSParleyBodyHeight => _bsCompassRows.Count;

    public string BSParleyBodyPosition => string.Empty;

    public BKeymap BSParleyBodyKeymap { get; }

    internal static void BSCompassShow(
        BLocalizationCatalog catalog,
        BBlueprintPart blueprint,
        Func<BSParley, Task?, int?> docket,
        CCompass compass,
        IReadOnlyDictionary<(CCompassPart, int?), int> places,
        int top,
        Action<int> scroll,
        (int, int) edge,
        BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(blueprint);
        ArgumentNullException.ThrowIfNull(docket);
        ArgumentNullException.ThrowIfNull(compass);
        ArgumentNullException.ThrowIfNull(places);
        ArgumentNullException.ThrowIfNull(scroll);
        ArgumentNullException.ThrowIfNull(measure);

        IReadOnlyList<CCompassRow> read = compass.CCompassRead(catalog.BLocalizationTextRead);
        int widest = read.Select(row => measure.BCanvasWidthResolve(row.CCompassRowNumber)).DefaultIfEmpty(0).Max();
        IReadOnlyList<(string, bool, int)> rows =
        [
            .. read.Select(row => (
                row.CCompassRowDepth switch
                {
                    0 => string.Empty,
                    1 => "  " + row.CCompassRowNumber.PadLeft(widest) + "  ",
                    _ => new string(' ', 4 + widest + 2),
                } + row.CCompassRowName,
                row.CCompassRowMuted,
                places.GetValueOrDefault((row.CCompassRowPart, row.CCompassRowCard)))),
        ];
        string title = catalog.BLocalizationTextRead("Contents.Title");
        BSCompass body = new(rows, top, scroll, title, measure);
        (int row, int column) = edge;
        BSParley parley = new(blueprint, title, "dialog", [], [], 0, body)
        {
            BSParleyAnchor = (row, Math.Max(column - body.BSParleyBodyWidth - 1, 0)),
        };
        _ = docket(parley, body._bsCompassChoice.Task);
    }

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

        int top = Math.Max(_bsCompassLit - height + 1, 0);
        for (int line = 0; line < height && top + line < _bsCompassRows.Count; line++)
        {
            string text = _bsCompassRows[top + line].Item1;
            string shown = measure.BCanvasWidthResolve(text) > width ? measure.BCanvasWidthClamp(text, width) : text;
            string drawn = focused && top + line == _bsCompassLit ? "lit"
                : _bsCompassRows[top + line].Item2 ? "muted"
                : role;
            yield return new BCanvasText(row + line, column, width, shown, drawn);
        }
    }
}
