using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BLink
{
    private const string BLinkFresh = "+ ";

    private const string BLinkGap = "  ";

    internal BLink(
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

        Action<bool, long, long?, string, string> insert = caret.CCaretTranslationInsert;
        BLinkTranslation = new BInlet(
            catalog,
            ("Card.TranslationHint", "link"),
            (caret.CCaretTranslation, BLinkDraw),
            (BLinkObserve(caret.CCaretTranslationAdd, insert),
                BLinkResolve(caret.CCaretTranslationResolve, true, insert),
                BLinkResolve(caret.CCaretTranslationResolve, false, insert)),
            (caret, spot, when, changed));
    }

    internal BInlet BLinkTranslation { get; }

    internal IReadOnlyList<BKeymapRow> BLinkKeymap => BLinkTranslation.BInletKeymap;

    private static IReadOnlyList<(long, string)> BLinkDraw(CCardDraft draft)
    {
        return [.. draft.CCardDraftTranslation.Select(static target => (
            target.CTranslationTargetId,
            $"<{target.CTranslationTargetHeadword} {target.CTranslationTargetLanguage}>"))];
    }

    private static Func<bool, long, string, (string, IReadOnlyList<BInletRow>)> BLinkResolve(
        Func<bool, long, string, bool, CProspect> gate, bool offered, Action<bool, long, long?, string, string> insert)
    {
        return BLinkObserve((collocated, card, text) => gate(collocated, card, text, offered), insert);
    }

    private static Func<bool, long, string, (string, IReadOnlyList<BInletRow>)> BLinkObserve(
        Func<bool, long, string, CProspect> gate, Action<bool, long, long?, string, string> insert)
    {
        return (collocated, card, text) =>
        {
            CProspect prospect = gate(collocated, card, text);
            IReadOnlyList<BInletRow> rows = prospect.CProspectShown
                ?
                [
                    .. prospect.CProspectRows.Select(row => new BInletRow(
                        row.CVistaRowName,
                        string.Empty,
                        BLinkGap + row.CVistaRowLanguage
                            + (row.CVistaRowEpithet.Length > 0 ? BLinkGap + row.CVistaRowEpithet : string.Empty),
                        string.Empty,
                        (owner, id) => insert(
                            owner, id, row.CVistaRowId, row.CVistaRowHeadword, row.CVistaRowLanguage))),
                    .. prospect.CProspectLanguages.Select(language => new BInletRow(
                        BLinkFresh + prospect.CProspectWord,
                        string.Empty,
                        BLinkGap + language,
                        string.Empty,
                        (owner, id) => insert(owner, id, null, prospect.CProspectWord, language))),
                ]
                : [];
            return (prospect.CProspectText, rows);
        };
    }
}
