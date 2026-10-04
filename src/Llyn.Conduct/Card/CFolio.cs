using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal static class CFolio
{
    internal static IReadOnlyList<CTranslationTarget> CFolioTargetRead(IReadOnlyList<LTranslationTarget> targets)
    {
        ArgumentNullException.ThrowIfNull(targets);

        return targets
            .Select(static target => new CTranslationTarget(
                target.LTranslationTargetId,
                target.LTranslationTargetHeadword,
                target.LTranslationTargetLanguage))
            .ToList();
    }

    internal static CEntryDraft CFolioEntryRead(
        LEntryDraft draft, IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> targets, LMediaPort media)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(targets);
        ArgumentNullException.ThrowIfNull(media);

        return new CEntryDraft(
            draft.LEntryDraftHeadword,
            draft.LEntryDraftNote,
            CFolioSheetRead(draft.LEntryDraftMeanings, targets, media, "Card.DefinitionHint"),
            CFolioSheetRead(draft.LEntryDraftCollocations, targets, media, "Card.MeaningHint"),
            new CEtymologyDraft(draft.LEntryDraftEtymology.LEtymologyDraftText));
    }

    internal static CStateValue CFolioStateRead(LStateValue value)
    {
        return new CStateValue(value.LStateValuePlain, value.LStateValueUncertain);
    }

    private static IReadOnlyList<CCardDraft> CFolioSheetRead(
        IReadOnlyList<LCardDraft> cards,
        IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> targets,
        LMediaPort media,
        string meaning)
    {
        return CFolioOrderRead(cards)
            .Select(card => new CCardDraft(
                card.LCardDraftId,
                card.LCardDraftPosition,
                CStateWording.LStateWordingRead(CFolioStateRead(card.LCardDraftTitle), null),
                CStateWording.LStateWordingRead(
                    CFolioStateRead(card.LCardDraftExpression), null, "Card.ExpressionHint"),
                CStateWording.LStateWordingRead(CFolioStateRead(card.LCardDraftMeaning), null, meaning),
                card.LCardDraftSentence.Select(CFolioSentenceRead).ToList(),
                card.LCardDraftSituation
                    .Select(static row => new CSituationDraft(
                        row.LSituationDraftId,
                        CFolioStateRead(row.LSituationDraftTitle),
                        CFolioStateRead(row.LSituationDraftKind),
                        CFolioStateRead(row.LSituationDraftDescription),
                        [],
                        []))
                    .ToList(),
                card.LCardDraftRegister
                    .Select(static row => new CRegisterDraft(
                        row.LRegisterDraftId,
                        CStateWording.LStateWordingRead(CFolioStateRead(row.LRegisterDraftName), null)))
                    .ToList(),
                CFolioTargetRead(targets[card.LCardDraftId]),
                card.LCardDraftTag.Select(static row => new CTagDraft(row.LTagDraftId, row.LTagDraftText)).ToList(),
                CFolioImageRead(card.LCardDraftImage, media),
                CFolioVideoRead(card.LCardDraftVideo, media)))
            .ToList();
    }

    internal static int? CFolioPlaceRead(LEntryDraft content, long cardId, int place)
    {
        ArgumentNullException.ThrowIfNull(content);

        IReadOnlyList<LCardDraft> cards = CFolioListRead(content, cardId);
        IReadOnlyList<LCardDraft> shown = CFolioOrderRead(cards);
        if (place < 0 || place >= shown.Count)
        {
            return null;
        }

        long target = shown[place].LCardDraftId;
        for (int index = 0; index < cards.Count; index++)
        {
            if (cards[index].LCardDraftId == target)
            {
                return index;
            }
        }

        return null;
    }

    internal static int? CFolioOrdinalRead(LEntryDraft content, long cardId, string ordinal)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(ordinal);

        if (!int.TryParse(ordinal, NumberStyles.Integer, CultureInfo.InvariantCulture, out int wanted))
        {
            return null;
        }

        int count = Math.Max(CFolioListRead(content, cardId).Count, 1);
        return Math.Clamp(wanted, 1, count) - 1;
    }

    internal static IReadOnlyList<LCardDraft> CFolioOrderRead(IReadOnlyList<LCardDraft> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        return cards.OrderBy(static card => card.LCardDraftPosition).ToList();
    }

    private static IReadOnlyList<LCardDraft> CFolioListRead(LEntryDraft content, long cardId)
    {
        return content.LEntryDraftMeanings.Any(card => card.LCardDraftId == cardId)
            ? content.LEntryDraftMeanings
            : content.LEntryDraftCollocations;
    }

    private static CSentenceDraft CFolioSentenceRead(LSentenceDraft sentence)
    {
        CExampleDraft? example = CFolioExampleRead(sentence.LSentenceDraftExample);
        return new CSentenceDraft(
            sentence.LSentenceDraftId,
            example,
            sentence.LSentenceDraftCited,
            CStateWording.LStateWordingRead(
                example?.CExampleDraftText ?? CStateValue.CStateValueEmpty, null, "Card.ExampleHint"),
            CStateWording.LStateWordingRead(
                CFolioStateRead(sentence.LSentenceDraftParticle), null, "Card.ParticleHint"),
            CStateWording.LStateWordingRead(
                CFolioStateRead(sentence.LSentenceDraftDependence), null, "Card.DependenceHint"));
    }

    private static CExampleDraft? CFolioExampleRead(LExampleDraft? example)
    {
        return example is null
            ? null
            : new CExampleDraft(
                CFolioStateRead(example.LExampleDraftText),
                example.LExampleDraftReference.LStateAnchorShown,
                CFolioGlossRead(example.LExampleDraftGloss));
    }

    internal static IReadOnlyList<CGlossDraft> CFolioGlossRead(IReadOnlyList<LGlossDraft> glosses)
    {
        return glosses
            .Select(static gloss => new CGlossDraft(
                gloss.LGlossDraftId,
                gloss.LGlossDraftLanguage,
                CFolioStateRead(gloss.LGlossDraftText),
                gloss.LGlossDraftNamed))
            .ToList();
    }

    internal static IReadOnlyList<CImageDraft> CFolioImageRead(IReadOnlyList<LImageDraft> images, LMediaPort media)
    {
        ArgumentNullException.ThrowIfNull(media);

        return images
            .Select(image => new CImageDraft(
                image.LImageDraftId,
                CFolioStateRead(image.LImageDraftLocation),
                image.LImageDraftEmpty,
                media.LEngineLocationRead(image.LImageDraftLocation.LStateValuePlain)))
            .ToList();
    }

    internal static IReadOnlyList<CVideoDraft> CFolioVideoRead(IReadOnlyList<LVideoDraft> videos, LMediaPort media)
    {
        ArgumentNullException.ThrowIfNull(media);

        return videos
            .Select(video =>
            {
                (TimeSpan from, TimeSpan? until) = LMediaPort.LEngineSpanRead(video);
                return new CVideoDraft(
                    video.LVideoDraftId,
                    CFolioStateRead(video.LVideoDraftLocation),
                    CFolioStateRead(video.LVideoDraftSpan),
                    video.LVideoDraftEmpty,
                    media.LEngineScreenRead(video.LVideoDraftLocation.LStateValuePlain) is (Uri address, var film)
                        ? new CScreen(address, film)
                        : null,
                    from,
                    until);
            })
            .ToList();
    }
}
