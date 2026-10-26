using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BCard
{
    private const int BCardPartSpan = 4;

    private readonly Func<(bool, CStep)?> _bCardLit;

    private readonly Action _bCardKeeper;

    private int _bCardPart;

    private int _bCardPartLast;

    internal BCard(
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

        _bCardLit = lit;
        _bCardKeeper = gates.Item2;
        Func<int, Func<(bool, long)?>> spot = part => () =>
            BCardPart == part && _bCardLit() is (bool collocated, { CStepCard: CCardDraft card })
                ? (collocated, card.CCardDraftId)
                : null;
        BContext context = new(catalog, gates.Item1, (spot(1), spot(2)), when, changed);
        BLabel label = new(catalog, gates.Item1, spot(3), when, changed);
        BLink link = new(catalog, gates.Item1, spot(4), when, changed);
        BCardInlet = [context.BContextSituation, context.BContextRegister, label.BLabelTag, link.BLinkTranslation];
        Func<BKeymapLive> shut = when(BCardShutCheck);
        BCardKeymap =
        [
            .. context.BContextKeymap,
            .. label.BLabelKeymap,
            .. link.BLinkKeymap,
            new(ConsoleKey.Escape, 0, BKeymapFocus.BKeymapFocusEditor, "Editor.Close", _ =>
            {
                BCardClose();
                changed();
            }, shut),
        ];
    }

    internal IReadOnlyList<BInlet> BCardInlet { get; }

    internal IReadOnlyList<BKeymapRow> BCardKeymap { get; }

    internal int BCardPart => Math.Min(_bCardPart, _bCardPartLast);

    internal bool BCardPartCheck(int way)
    {
        BCardPartRead();
        return way > 0 ? BCardPart < _bCardPartLast : BCardPart > 0;
    }

    internal void BCardPartMove(int way)
    {
        _bCardPart += way;
    }

    internal void BCardPartSet(bool last)
    {
        _bCardPart = last ? BCardPartSpan : 0;
    }

    internal bool BCardObserve(ConsoleKeyInfo key)
    {
        bool taken = false;
        foreach (BInlet inlet in BCardInlet)
        {
            taken |= inlet.BInletObserve(key);
        }

        return taken;
    }

    internal void BCardRefine()
    {
        BCardPartRead();
        (bool, long)? card = _bCardLit() is (bool collocated, { CStepCard: CCardDraft draft })
            ? (collocated, draft.CCardDraftId)
            : null;
        foreach (BInlet inlet in BCardInlet)
        {
            inlet.BInletRefine(card);
        }
    }

    internal void BCardClose()
    {
        bool closed = BCardInlet.Any(static inlet => inlet.BInletField is not null);
        foreach (BInlet inlet in BCardInlet)
        {
            inlet.BInletClose();
        }

        if (closed)
        {
            _bCardKeeper();
        }
    }

    private bool BCardShutCheck()
    {
        return BCardInlet.Any(static inlet => inlet.BInletShut);
    }

    private void BCardPartRead()
    {
        _bCardPartLast = _bCardLit() is (_, { CStepCard.CCardDraftFolded: false }) ? BCardPartSpan : 0;
    }

    internal void BCardReset()
    {
        foreach (BInlet inlet in BCardInlet)
        {
            inlet.BInletReset();
        }

        _bCardPart = 0;
    }
}
