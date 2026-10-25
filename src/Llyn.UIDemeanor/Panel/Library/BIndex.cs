using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Llyn.UIDemeanor;

internal sealed class BIndex<BIndexRow>
{
    private readonly BLocalizationCatalog _bIndexCatalog;
    private readonly string _bIndexTitle;
    private readonly string _bIndexSearch;
    private readonly Func<BIndexRow, (string, string, string, bool)> _bIndexCells;
    private readonly Func<BIndexRow, long> _bIndexKey;
    private readonly Action<string>? _bIndexQuery;
    private readonly Action<BIndexRow> _bIndexChoose;
    private readonly Action? _bIndexClimb;
    private readonly Action<BIndexRow>? _bIndexMark;
    private readonly Action<BIndexRow, int>? _bIndexShift;
    private readonly BField _bIndexField;
    private IReadOnlyList<BIndexRow> _bIndexRows = [];
    private string _bIndexOrder = string.Empty;
    private bool _bIndexFiltered;
    private string _bIndexNotice = string.Empty;
    private long? _bIndexLit;
    private int _bIndexPage = 1;

    internal BIndex(
        BLocalizationCatalog catalog,
        string title,
        string search,
        Func<BIndexRow, (string, string, string, bool)> cells,
        Func<BIndexRow, long> key,
        Action<string>? query,
        Action<BIndexRow> choose,
        Action? climb = null,
        Action<BIndexRow>? mark = null,
        Action<BIndexRow, int>? shift = null)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(search);
        ArgumentNullException.ThrowIfNull(cells);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(choose);

        _bIndexCatalog = catalog;
        _bIndexTitle = title;
        _bIndexSearch = search;
        _bIndexCells = cells;
        _bIndexKey = key;
        _bIndexQuery = query;
        _bIndexChoose = choose;
        _bIndexClimb = climb;
        _bIndexMark = mark;
        _bIndexShift = shift;
        _bIndexField = new BField(text =>
        {
            _bIndexLit = null;
            query?.Invoke(text);
        });
    }

    internal void BIndexRefine(IReadOnlyList<BIndexRow> rows, string order, bool filtered, string notice)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(notice);

        if (_bIndexLit is long lit && !rows.Any(entry => _bIndexKey(entry) == lit))
        {
            _bIndexLit = null;
        }

        _bIndexRows = rows;
        _bIndexOrder = order;
        _bIndexFiltered = filtered;
        _bIndexNotice = notice;
    }

    internal void BIndexLitClear()
    {
        _bIndexLit = null;
    }

    internal void BIndexLitSettle(Action change)
    {
        ArgumentNullException.ThrowIfNull(change);

        long? pinned = _bIndexLit;
        int place = BIndexLitMove(_bIndexRows, 0);
        long? settled = _bIndexRows.Count == 0 ? null : _bIndexKey(_bIndexRows[place]);
        _bIndexLit = settled;
        change();
        if (settled is long kept && _bIndexRows.Any(entry => _bIndexKey(entry) == kept))
        {
            _bIndexLit = pinned;
        }
        else if (_bIndexRows.Count > 0)
        {
            _bIndexLit = _bIndexKey(_bIndexRows[Math.Min(place, _bIndexRows.Count - 1)]);
        }
    }

    internal bool BIndexObserve(ConsoleKeyInfo key)
    {
        IReadOnlyList<BIndexRow> rows = _bIndexRows;
        int lit = BIndexLitMove(rows, 0);
        bool plain = key.Modifiers == 0;
        int? step = !plain ? null : key.Key switch
        {
            ConsoleKey.UpArrow => -1,
            ConsoleKey.DownArrow => 1,
            ConsoleKey.PageUp => -_bIndexPage,
            ConsoleKey.PageDown => _bIndexPage,
            ConsoleKey.Home => -rows.Count,
            ConsoleKey.End => rows.Count,
            _ => null,
        };
        if (step is int moved)
        {
            if (rows.Count > 0)
            {
                _bIndexLit = _bIndexKey(rows[BIndexLitMove(rows, moved)]);
            }

            return true;
        }

        switch (key.Key)
        {
            case ConsoleKey.UpArrow or ConsoleKey.DownArrow
                when key.Modifiers == ConsoleModifiers.Control && _bIndexShift is not null:
                int place = key.Key == ConsoleKey.UpArrow ? -1 : 1;
                if (lit < rows.Count)
                {
                    _bIndexShift(rows[lit], place);
                    _bIndexLit = _bIndexKey(rows[lit]);
                }

                return true;
            case ConsoleKey.Insert when plain && _bIndexMark is not null:
                if (lit < rows.Count)
                {
                    long next = _bIndexKey(rows[BIndexLitMove(rows, 1)]);
                    _bIndexMark(rows[lit]);
                    _bIndexLit = next;
                }

                return true;
            case ConsoleKey.Enter when plain:
                if (lit < rows.Count)
                {
                    _bIndexChoose(rows[lit]);
                }

                return lit < rows.Count;
            case ConsoleKey.Escape when plain:
                if (_bIndexField.BFieldText.Length == 0)
                {
                    return false;
                }

                _bIndexField.BFieldClear();
                _bIndexLit = null;
                if (_bIndexQuery is Action<string> emptied)
                {
                    emptied(string.Empty);
                }

                return true;
            case ConsoleKey.Backspace when plain:
                if (_bIndexField.BFieldText.Length > 0)
                {
                    return _bIndexField.BSParleyBodyObserve(key);
                }

                _bIndexClimb?.Invoke();
                return _bIndexClimb is not null;
        }

        if (key.Modifiers.HasFlag(ConsoleModifiers.Control) || key.Modifiers.HasFlag(ConsoleModifiers.Alt)
            || char.IsControl(key.KeyChar))
        {
            return false;
        }

        if (_bIndexQuery is not null)
        {
            return _bIndexField.BSParleyBodyObserve(key);
        }

        string letter = key.KeyChar.ToString();
        for (int offset = 1; offset <= rows.Count; offset++)
        {
            int index = (lit + offset) % rows.Count;
            if (_bIndexCells(rows[index]).Item1.StartsWith(letter, StringComparison.CurrentCultureIgnoreCase))
            {
                _bIndexLit = _bIndexKey(rows[index]);
                break;
            }
        }

        return true;
    }

    internal IEnumerable<BCanvasText> BIndexDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        int row,
        int column,
        int width,
        int height,
        bool focused,
        BCanvasWidth measure)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);

        if (width < 3 || height < 4)
        {
            yield break;
        }

        IReadOnlyList<BIndexRow> rows = _bIndexRows;
        int inner = width - 2;
        int left = column + 1;
        string title = " " + _bIndexCatalog.BLocalizationTextRead(_bIndexTitle) + " ";
        int titleCells = measure.BCanvasWidthResolve(title);
        if (titleCells <= inner)
        {
            yield return new BCanvasText(
                row, column + ((width - titleCells) / 2), titleCells, title, focused ? "title" : "side");
        }

        int page = height - 4;
        _bIndexPage = Math.Max(page, 1);
        int lit = BIndexLitMove(rows, 0);
        int top = Math.Max(lit - page + 1, 0);
        string chosen = glyphs.GetValueOrDefault("Pending") is [string pending, ..] ? pending : "o";
        int language = Math.Min(
            rows.Select(entry => measure.BCanvasWidthResolve(_bIndexCells(entry).Item3)).DefaultIfEmpty(0).Max(),
            inner / 4);
        for (int line = 0; line < page && top + line < rows.Count; line++)
        {
            int index = top + line;
            (string name, string epithet, string tongue, bool picked) = _bIndexCells(rows[index]);
            string role = index != lit ? "side" : focused ? "lit" : "lit-other";
            int at = row + 1 + line;
            yield return new BCanvasText(at, left, inner, string.Empty, role);
            string mark = picked && index != lit ? chosen : string.Empty;
            yield return new BCanvasText(at, left, 2, measure.BCanvasWidthClamp(mark, 2), role);
            int span = Math.Max(inner - 2 - language - (language > 0 ? 1 : 0), 0);
            int cells = Math.Min(measure.BCanvasWidthResolve(name), span);
            yield return new BCanvasText(at, left + 2, cells, measure.BCanvasWidthClamp(name, cells), role);
            int rest = span - cells - 2;
            if (rest > 0 && epithet.Length > 0)
            {
                yield return new BCanvasText(
                    at, left + 2 + cells + 2, rest, measure.BCanvasWidthClamp(epithet, rest),
                    index == lit ? role : "muted");
            }

            if (language > 0)
            {
                yield return new BCanvasText(
                    at, left + inner - language, language, measure.BCanvasWidthClamp(tongue, language), role);
            }
        }

        string notice = _bIndexNotice;
        if (rows.Count == 0 && notice.Length > 0)
        {
            string words = _bIndexCatalog.BLocalizationTextRead(notice);
            int cells = Math.Min(measure.BCanvasWidthResolve(words), inner);
            string text = measure.BCanvasWidthClamp(words, cells);
            yield return new BCanvasText(
                row + 1 + ((page - 1) / 2), left + ((inner - cells) / 2), cells, text, "muted");
        }

        if (_bIndexQuery is not null)
        {
            string label = _bIndexCatalog.BLocalizationTextRead(_bIndexSearch) + " ";
            int cells = Math.Min(measure.BCanvasWidthResolve(label), inner);
            yield return new BCanvasText(
                row + height - 2, left, cells, measure.BCanvasWidthClamp(label, cells), "side");
            foreach (BCanvasText text in _bIndexField.BSParleyBodyDraw(
                glyphs, row + height - 2, left + cells, inner - cells, 1, focused, "field", measure))
            {
                yield return text;
            }
        }

        string position = string.Format(
            CultureInfo.CurrentCulture,
            _bIndexCatalog.BLocalizationTextRead("Index.Position"),
            rows.Count == 0 ? 0 : lit + 1,
            rows.Count);
        string ordering = _bIndexCatalog.BLocalizationTextRead(_bIndexOrder);
        string border = _bIndexFiltered
            ? $" {position}  {ordering}  {_bIndexCatalog.BLocalizationTextRead("Index.Filtered")} "
            : $" {position}  {ordering} ";
        int borderCells = Math.Min(measure.BCanvasWidthResolve(border), Math.Max(inner - 2, 0));
        yield return new BCanvasText(
            row + height - 1, left + 1, borderCells, measure.BCanvasWidthClamp(border, borderCells), "side");
    }

    private int BIndexLitMove(IReadOnlyList<BIndexRow> rows, int step)
    {
        int found = _bIndexLit is long lit ? rows.ToList().FindIndex(entry => _bIndexKey(entry) == lit) : -1;
        int start = found >= 0 ? found : Math.Max(rows.ToList().FindIndex(entry => _bIndexCells(entry).Item4), 0);
        return Math.Clamp(start + step, 0, Math.Max(rows.Count - 1, 0));
    }
}
