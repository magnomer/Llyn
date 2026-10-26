using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BLabel
{
    internal BLabel(
        BLocalizationCatalog catalog,
        CCaret caret,
        Func<(bool, long)?> spot,
        Func<Func<bool>, Func<BKeymapLive>> when,
        Action changed)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(caret);
        ArgumentNullException.ThrowIfNull(spot);
        ArgumentNullException.ThrowIfNull(when);
        ArgumentNullException.ThrowIfNull(changed);

        BLabelTag = new BInlet(
            catalog,
            ("Card.LabelHint", "text"),
            (caret.CCaretTag, static draft =>
                [.. draft.CCardDraftTag.Select(static tag => (tag.CTagDraftId, $"#{tag.CTagDraftText}"))]),
            (BInlet.BInletOfferRead(caret.CCaretTagAdd, caret.CCaretTagInsert),
                BInlet.BInletOfferRead(caret.CCaretTagSettle, caret.CCaretTagInsert),
                BInlet.BInletOfferRead(caret.CCaretTagSettle, caret.CCaretTagInsert)),
            (caret, spot, when, changed));
    }

    internal BInlet BLabelTag { get; }

    internal IReadOnlyList<BKeymapRow> BLabelKeymap => BLabelTag.BInletKeymap;
}
