using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.UIDemeanor;

internal sealed class BTable
{
    private readonly int _bTableCount;
    private readonly int _bTablePage;
    private readonly IReadOnlyList<(Func<int, BDrop>, Action<int, int>)> _bTableCell;

    internal BTable(int count, int page, IReadOnlyList<(Func<int, BDrop>, Action<int, int>)> cells)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(page);
        ArgumentNullException.ThrowIfNull(cells);

        _bTableCount = count;
        _bTablePage = page;
        _bTableCell = cells;
    }

    internal int BTableRow { get; private set; }

    internal int BTableCell { get; private set; }

    internal int BTableTop { get; private set; }

    internal BDrop? BTableDrop { get; private set; }

    internal IEnumerable<int> BTableRowShown =>
        Enumerable.Range(BTableTop, Math.Clamp(_bTableCount - BTableTop, 0, _bTablePage));

    internal bool BTableObserve(ConsoleKeyInfo key)
    {
        if (BTableDrop is BDrop drop)
        {
            if (key.Key is ConsoleKey.Enter or ConsoleKey.Spacebar)
            {
                _bTableCell[BTableCell].Item2(BTableRow, drop.BDropLit);
            }

            BTableDrop = drop.BDropObserve(key) ? drop : null;
            return key.Key != ConsoleKey.F10;
        }

        bool scroll = _bTableCell.Count == 0;
        int step = key.Key switch
        {
            ConsoleKey.UpArrow or ConsoleKey.LeftArrow => -1,
            ConsoleKey.DownArrow or ConsoleKey.RightArrow => 1,
            _ => 0,
        };
        switch (key.Key)
        {
            case ConsoleKey.UpArrow or ConsoleKey.DownArrow when scroll:
                break;
            case ConsoleKey.UpArrow or ConsoleKey.DownArrow:
                BTableRow = Math.Clamp(BTableRow + step, 0, Math.Max(_bTableCount - 1, 0));
                break;
            case ConsoleKey.LeftArrow or ConsoleKey.RightArrow when !scroll:
                BTableCell = Math.Clamp(BTableCell + step, 0, _bTableCell.Count - 1);
                break;
            case ConsoleKey.Enter or ConsoleKey.Spacebar when !scroll && BTableRow < _bTableCount:
                BDrop opened = _bTableCell[BTableCell].Item1(BTableRow);
                BTableDrop = opened.BDropEnabled ? opened : null;
                break;
            case ConsoleKey.Enter or ConsoleKey.Spacebar when !scroll:
                break;
            default:
                return false;
        }

        BTableTop = scroll
            ? Math.Clamp(BTableTop + step, 0, Math.Max(_bTableCount - _bTablePage, 0))
            : Math.Clamp(BTableTop, BTableRow - _bTablePage + 1, BTableRow);
        return true;
    }
}
