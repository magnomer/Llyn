using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed record CLeaf(
    long CLeafId,
    int CLeafPosition,
    CStateWording CLeafTitle,
    CStateWording CLeafExpression,
    CStateWording CLeafMeaning,
    IReadOnlyList<CLeafChip> CLeafSituation,
    IReadOnlyList<CLeafChip> CLeafRegister,
    IReadOnlyList<CLeafChip> CLeafTag,
    IReadOnlyList<CTranslationTarget> CLeafTranslation,
    IReadOnlyList<CLeafLine> CLeafSentence,
    IReadOnlyList<CImageDraft> CLeafImage,
    IReadOnlyList<CVideoDraft> CLeafVideo,
    bool CLeafFolded,
    bool CLeafStored)
{
    internal static IReadOnlyList<CLeaf> LLeafRead(
        IReadOnlyList<LCardDraft> cards,
        LSentenceOrder order,
        string mark,
        IReadOnlyDictionary<long, string> citations,
        IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> targets,
        IReadOnlySet<long> folds,
        LMediaPort media,
        LExamplePort examples)
    {
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentNullException.ThrowIfNull(targets);
        ArgumentNullException.ThrowIfNull(folds);

        return CFolio.CFolioOrderRead(cards)
            .Select(card => new CLeaf(
                card.LCardDraftId,
                card.LCardDraftPosition,
                CStateWording.LStateWordingRead(CFolio.CFolioStateRead(card.LCardDraftTitle), null),
                CStateWording.LStateWordingRead(CFolio.CFolioStateRead(card.LCardDraftExpression), null),
                CStateWording.LStateWordingRead(CFolio.CFolioStateRead(card.LCardDraftMeaning), null),
                card.LCardDraftSituation.Select(CLeafChip.LLeafChipRead).ToList(),
                card.LCardDraftRegister.Select(CLeafChip.LLeafChipRead).ToList(),
                card.LCardDraftTag.Select(CLeafChip.LLeafChipRead).ToList(),
                CFolio.CFolioTargetRead(targets.GetValueOrDefault(card.LCardDraftId, [])),
                card.LCardDraftSentence
                    .Select(sentence => CLeafLine.LLeafLineRead(sentence, order, mark, citations, examples))
                    .ToList(),
                CFolio.CFolioImageRead(card.LCardDraftImage, media),
                CFolio.CFolioVideoRead(card.LCardDraftVideo, media),
                folds.Contains(card.LCardDraftId),
                card.LCardDraftStored))
            .ToList();
    }
}
