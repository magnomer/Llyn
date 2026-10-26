using System;
using System.Collections.Generic;
using System.Globalization;

namespace Llyn.UIDemeanor;

internal sealed class BCardBadge
{
    private readonly (Func<bool, long, bool>, Func<bool, long, string, bool>, Action<bool, long>) _bCardBadgeGate;

    private BField? _bCardBadgeEntry;

    private bool _bCardBadgeFresh;

    internal BCardBadge((Func<bool, long, bool>, Func<bool, long, string, bool>, Action<bool, long>) gates)
    {
        ArgumentNullException.ThrowIfNull(gates.Item1);
        ArgumentNullException.ThrowIfNull(gates.Item2);
        ArgumentNullException.ThrowIfNull(gates.Item3);

        _bCardBadgeGate = gates;
    }

    internal (bool, long)? BCardBadgeCard { get; private set; }

    internal void BCardBadgeOpen((bool, long) card, int position)
    {
        (bool collocated, long id) = card;
        bool opened = _bCardBadgeGate.Item1(collocated, id);
        if (!opened)
        {
            return;
        }

        BCardBadgeCard = card;
        _bCardBadgeEntry = new BField(static _ => { }, position.ToString(CultureInfo.CurrentCulture));
        _bCardBadgeFresh = true;
    }

    internal bool BCardBadgeObserve(ConsoleKeyInfo key)
    {
        if (_bCardBadgeEntry is not BField entry)
        {
            return false;
        }

        bool typed = !char.IsControl(key.KeyChar);
        if (typed && !char.IsAsciiDigit(key.KeyChar))
        {
            return true;
        }

        if (typed && _bCardBadgeFresh)
        {
            entry.BFieldClear();
        }

        _bCardBadgeFresh = false;
        return entry.BSParleyBodyObserve(key);
    }

    internal void BCardBadgeSettle()
    {
        if (_bCardBadgeEntry is not BField entry)
        {
            return;
        }

        _ = entry.BFieldPendingApply(null);
        string text = entry.BFieldText;
        (bool collocated, long id) = BCardBadgeCard.GetValueOrDefault();
        BCardBadgeReset();
        _ = _bCardBadgeGate.Item2(collocated, id, text);
    }

    internal void BCardBadgeClose()
    {
        if (_bCardBadgeEntry is null)
        {
            return;
        }

        (bool collocated, long id) = BCardBadgeCard.GetValueOrDefault();
        BCardBadgeReset();
        _bCardBadgeGate.Item3(collocated, id);
    }

    internal void BCardBadgeReset()
    {
        _bCardBadgeEntry = null;
        BCardBadgeCard = null;
        _bCardBadgeFresh = false;
    }

    internal IEnumerable<BCanvasText> BCardBadgeDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        (int, int, string) place,
        bool focused)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);

        (int line, int column, string role) = place;
        if (_bCardBadgeEntry is not BField entry)
        {
            yield break;
        }

        int cells = measure.BCanvasWidthResolve(entry.BFieldText) + 1;
        yield return new BCanvasText(line, column, 1, "[", role);
        foreach (BCanvasText text in entry.BSParleyBodyDraw(
            glyphs, line, column + 1, cells, 1, focused, "field", measure))
        {
            yield return text;
        }

        yield return new BCanvasText(line, column + 1 + cells, 1, "]", role);
    }
}
