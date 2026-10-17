using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.UIDemeanor;

internal sealed class BDrop
{
    private readonly bool _bDropEnabled;
    private readonly int _bDropOrigin;
    private readonly int _bDropPage;
    private int _bDropShift;

    internal BDrop(IReadOnlyList<string> labels, int lit, bool enabled, int page)
    {
        ArgumentNullException.ThrowIfNull(labels);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(page);

        BDropLabel = labels;
        _bDropEnabled = enabled;
        _bDropOrigin = lit;
        _bDropPage = page;
        BDropTop = Math.Max(BDropLit - page + 1, 0);
    }

    internal IReadOnlyList<string> BDropLabel { get; }

    internal int BDropLit => Math.Clamp(_bDropOrigin + _bDropShift, 0, Math.Max(BDropLabel.Count - 1, 0));

    internal int BDropTop { get; private set; }

    internal IEnumerable<int> BDropLabelShown =>
        Enumerable.Range(BDropTop, Math.Clamp(BDropLabel.Count - BDropTop, 0, _bDropPage));

    internal bool BDropEnabled => _bDropEnabled && BDropLabel.Count > 0;

    internal bool BDropObserve(ConsoleKeyInfo key)
    {
        int step = key.Key switch
        {
            ConsoleKey.UpArrow => -1,
            ConsoleKey.DownArrow => 1,
            _ => 0,
        };
        switch (key.Key)
        {
            case ConsoleKey.UpArrow or ConsoleKey.DownArrow:
                _bDropShift = BDropLit + step - _bDropOrigin;
                BDropTop = Math.Clamp(BDropTop, BDropLit - _bDropPage + 1, BDropLit);
                return true;
            case ConsoleKey.Enter or ConsoleKey.Spacebar or ConsoleKey.Escape or ConsoleKey.F10:
                return false;
            default:
                return true;
        }
    }
}
