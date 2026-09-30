using System;
using System.Collections.Generic;
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
            draft.LEntryDraftLanguage,
            draft.LEntryDraftNote,
            CFolioSheetRead(draft.LEntryDraftMeanings, targets, media, "Card.DefinitionHint"),
            CFolioSheetRead(draft.LEntryDraftCollocations, targets, media, "Card.MeaningHint"),
            CSounding.CSoundingTranscriptionRead(draft.LEntryDraftTranscriptions),
            CSounding.CSoundingReflexRead(draft.LEntryDraftReflexes),
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
        return cards
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

    private static CSentenceDraft CFolioSentenceRead(LSentenceDraft sentence)
    {
        CExampleDraft? example = CFolioExampleRead(sentence.LSentenceDraftExample);
        return new CSentenceDraft(
            sentence.LSentenceDraftId,
            example,
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
