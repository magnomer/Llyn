using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BContext
{
    internal BContext(
        BLocalizationCatalog catalog,
        CCaret caret,
        (Func<(bool, long)?>, Func<(bool, long)?>) spots,
        Func<Func<bool>, Func<BKeymapLive>> when,
        Action changed)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(caret);
        ArgumentNullException.ThrowIfNull(spots.Item1);
        ArgumentNullException.ThrowIfNull(spots.Item2);
        ArgumentNullException.ThrowIfNull(when);
        ArgumentNullException.ThrowIfNull(changed);

        Func<CStateWording, string> word = wording => wording.CStateWordingKey is string key
            ? catalog.BLocalizationTextRead(key)
            : wording.CStateWordingText;
        BContextSituation = new BInlet(
            catalog,
            ("Card.SituationHint", "situation"),
            (caret.CCaretSituation, draft => [.. draft.CCardDraftSituation.Select(situation =>
                (situation.CSituationDraftId, $"[{word(situation.CSituationDraftWording)}]"))]),
            (BInlet.BInletOfferRead(caret.CCaretSituationAdd, caret.CCaretSituationInsert),
                BInlet.BInletOfferRead(caret.CCaretSituationSettle, caret.CCaretSituationInsert),
                BInlet.BInletOfferRead(caret.CCaretSituationSettle, caret.CCaretSituationInsert)),
            (caret, spots.Item1, when, changed));
        BContextRegister = new BInlet(
            catalog,
            ("Card.RegisterHint", "register"),
            (caret.CCaretRegister, draft => [.. draft.CCardDraftRegister.Select(register =>
                (register.CRegisterDraftId, $"{{{word(register.CRegisterDraftName)}}}"))]),
            (BInlet.BInletOfferRead(caret.CCaretRegisterAdd, caret.CCaretRegisterInsert),
                BInlet.BInletOfferRead(caret.CCaretRegisterSettle, caret.CCaretRegisterInsert),
                BInlet.BInletOfferRead(caret.CCaretRegisterSettle, caret.CCaretRegisterInsert)),
            (caret, spots.Item2, when, changed));
        BContextKeymap = [.. BContextSituation.BInletKeymap, .. BContextRegister.BInletKeymap];
    }

    internal BInlet BContextSituation { get; }

    internal BInlet BContextRegister { get; }

    internal IReadOnlyList<BKeymapRow> BContextKeymap { get; }
}
