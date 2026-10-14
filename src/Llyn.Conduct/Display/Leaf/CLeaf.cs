using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed record CLeaf(
    long CLeafId,
    int CLeafPosition,
    CStateWording CLeafTitle,
    CStateWording CLeafMeaning,
    IReadOnlyList<CLeafChip> CLeafSituation,
    IReadOnlyList<CLeafChip> CLeafRegister,
    IReadOnlyList<CLeafChip> CLeafTag,
    IReadOnlyList<CTranslationTarget> CLeafTranslation,
    IReadOnlyList<CLeafLine> CLeafSentence,
    IReadOnlyList<CImageDraft> CLeafImage,
    IReadOnlyList<CVideoDraft> CLeafVideo,
    bool CLeafFolded,
    bool CLeafStored,
    IReadOnlyList<CLeaf> CLeafCollocation,
    IReadOnlyList<(long CLeafSeeId, string CLeafSeeText)> CLeafSee,
    CStateWording CLeafPhrase,
    IReadOnlyList<string> CLeafFlag,
    string CLeafExample,
    string CLeafAlso,
    string CLeafHeading)
{
    public bool CLeafQuoted => CLeafExample.Length > 0;

    public bool CLeafShared => CLeafAlso.Length > 0;

    internal static IReadOnlyList<CLeaf> LLeafRead(
        IReadOnlyList<LCardDraft> cards,
        LCollocationLayout? layout,
        LSentenceOrder order,
        string mark,
        string stub,
        string example,
        string exampleOne,
        string also,
        string heading,
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
                card.LCardDraftStored,
                LLeafRead(
                    layout?.LCollocationLayoutHome.GetValueOrDefault(card.LCardDraftId, []) ?? [],
                    layout, order, mark, stub, example, exampleOne, also, heading, citations, targets, folds, media,
                    examples),
                (layout?.LCollocationLayoutStub.GetValueOrDefault(card.LCardDraftId, []) ?? [])
                    .Select(line =>
                    {
                        CStateValue expression = CFolio.CFolioStateRead(line.LCollocationPhrase);
                        return (line.LCollocationId, string.Format(
                            CultureInfo.CurrentCulture,
                            stub,
                            expression.CStateValueUncertain ? mark : expression.CStateValueText,
                            line.LMeaningPath));
                    })
                    .ToList(),
                CStateWording.LStateWordingRead(
                    CFolio.CFolioStateRead(
                        layout?.LCollocationLayoutPhrase.GetValueOrDefault(card.LCardDraftId)
                        ?? LStateValue.LStateValueUnspecified),
                    null),
                layout?.LCollocationLayoutFlag.GetValueOrDefault(card.LCardDraftId, []) ?? [],
                (layout?.LCollocationLayoutExample.GetValueOrDefault(card.LCardDraftId) ?? 0) switch
                {
                    0 => string.Empty,
                    1 => string.Format(CultureInfo.CurrentCulture, exampleOne, 1),
                    int count => string.Format(CultureInfo.CurrentCulture, example, count),
                },
                (layout?.LCollocationLayoutAlso.GetValueOrDefault(card.LCardDraftId, []) ?? []) is { Count: > 0 } paths
                    ? string.Format(CultureInfo.CurrentCulture, also, string.Join(", ", paths))
                    : string.Empty,
                (layout?.LCollocationLayoutHome.GetValueOrDefault(card.LCardDraftId, []) ?? []).Count > 0
                    ? heading
                    : string.Empty))
            .ToList();
    }
}
