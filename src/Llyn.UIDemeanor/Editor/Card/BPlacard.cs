using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BPlacard
{
    private readonly Func<(bool, CStep)?> _bPlacardLit;

    private readonly Action _bPlacardKeeper;

    private int _bPlacardPart;

    private int _bPlacardPartLast;

    internal BPlacard(
        BLocalizationCatalog catalog,
        (CCaret, Action) gates,
        Func<(bool, CStep)?> lit,
        Func<Func<bool>, Func<BKeymapLive>> when,
        Action changed)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(gates.Item1);
        ArgumentNullException.ThrowIfNull(gates.Item2);
        ArgumentNullException.ThrowIfNull(lit);
        ArgumentNullException.ThrowIfNull(when);
        ArgumentNullException.ThrowIfNull(changed);

        _bPlacardLit = lit;
        _bPlacardKeeper = gates.Item2;
        Func<int, Func<(bool, long)?>> spot = part => () =>
            _bPlacardLit() is (bool collocated, { CStepCard: CCardDraft card })
                && BPlacardPart == part + BCard.BCardLeadRead(collocated)
                ? (collocated, card.CCardDraftId)
                : null;
        BContext context = new(catalog, gates.Item1, (spot(1), spot(2)), when, changed);
        BLabel label = new(catalog, gates.Item1, spot(3), when, changed);
        BLink link = new(catalog, gates.Item1, spot(4), when, changed);
        BPlacardInlet = [context.BContextSituation, context.BContextRegister, label.BLabelTag, link.BLinkTranslation];
        Func<BKeymapLive> shut = when(BPlacardShutCheck);
        BPlacardKeymap =
        [
            .. context.BContextKeymap,
            .. label.BLabelKeymap,
            .. link.BLinkKeymap,
            new(ConsoleKey.Escape, 0, BKeymapFocus.BKeymapFocusEditor, "Editor.Close", _ =>
            {
                BPlacardClose();
                changed();
            }, shut),
        ];
    }

    internal IReadOnlyList<BInlet> BPlacardInlet { get; }

    internal IReadOnlyList<BKeymapRow> BPlacardKeymap { get; }

    internal int BPlacardPart => _bPlacardPart < 0
        ? Math.Max(_bPlacardPartLast + 1 + _bPlacardPart, 0)
        : Math.Min(_bPlacardPart, _bPlacardPartLast);

    internal bool BPlacardPartCheck(int way)
    {
        BPlacardPartRead();
        return way > 0 ? BPlacardPart < _bPlacardPartLast : BPlacardPart > 0;
    }

    internal void BPlacardPartMove(int way)
    {
        _bPlacardPart += way;
    }

    internal void BPlacardPartSet(bool last)
    {
        _bPlacardPart = last ? -1 : 0;
    }

    internal bool BPlacardObserve(ConsoleKeyInfo key)
    {
        bool taken = false;
        foreach (BInlet inlet in BPlacardInlet)
        {
            taken |= inlet.BInletObserve(key);
        }

        return taken;
    }

    internal void BPlacardRefine()
    {
        BPlacardPartRead();
        (bool, long)? card = _bPlacardLit() is (bool collocated, { CStepCard: CCardDraft draft })
            ? (collocated, draft.CCardDraftId)
            : null;
        foreach (BInlet inlet in BPlacardInlet)
        {
            inlet.BInletRefine(card);
        }
    }

    internal void BPlacardClose()
    {
        bool closed = BPlacardInlet.Any(static inlet => inlet.BInletField is not null);
        foreach (BInlet inlet in BPlacardInlet)
        {
            inlet.BInletClose();
        }

        if (closed)
        {
            _bPlacardKeeper();
        }
    }

    private bool BPlacardShutCheck()
    {
        return BPlacardInlet.Any(static inlet => inlet.BInletShut);
    }

    private void BPlacardPartRead()
    {
        _bPlacardPartLast = _bPlacardLit() is (bool collocated, { CStepCard.CCardDraftFolded: false })
            ? BCard.BCardLeadRead(collocated) + BPlacardInlet.Count
            : 0;
    }

    internal void BPlacardReset()
    {
        foreach (BInlet inlet in BPlacardInlet)
        {
            inlet.BInletReset();
        }

        _bPlacardPart = 0;
    }
}
