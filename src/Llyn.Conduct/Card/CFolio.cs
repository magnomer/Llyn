using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Conduct;

internal static class CFolio
{
    internal static CSentenceOrder CFolioOrderRead(LSentenceOrder order)
    {
        return new CSentenceOrder(order.LSentenceOrderParticle, order.LSentenceOrderDependence);
    }

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

    internal static CEntryDraft CFolioEntryRead(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return new CEntryDraft(
            draft.LEntryDraftHeadword,
            draft.LEntryDraftLanguage,
            draft.LEntryDraftNote,
            draft.LEntryDraftAudio,
            draft.LEntryDraftPronunciation is LPronunciationDraft spoken
                ? CSounding.CSoundingPronunciationRead(spoken)
                : null,
            CSounding.CSoundingPronunciationRead(draft.LEntryDraftAccents),
            draft.LEntryDraftSpeeches
                .Select(static speech => new CSpeechDraft(speech.LSpeechDraftValue, speech.LSpeechDraftName))
                .ToList(),
            CFolioSheetRead(draft.LEntryDraftMeanings),
            CFolioSheetRead(draft.LEntryDraftCollocations),
            CSounding.CSoundingTranscriptionRead(draft.LEntryDraftTranscriptions),
            CSounding.CSoundingReflexRead(draft.LEntryDraftReflexes),
            new CEtymologyDraft(
                draft.LEntryDraftEtymology.LEtymologyDraftText,
                CFolioMentionRead(draft.LEntryDraftEtymology.LEtymologyDraftMentions)));
    }

    internal static CStateValue CFolioStateRead(LStateValue value)
    {
        return new CStateValue(
            value.LStateValueShown ?? string.Empty, value.LStateValueUncertain, value.LStateValueLegible);
    }

    internal static IReadOnlyList<CCardDraft> CFolioSheetRead(IReadOnlyList<LCardDraft> cards)
    {
        return cards
            .Select(static card => new CCardDraft(
                card.LCardDraftId,
                card.LCardDraftPosition,
                CFolioStateRead(card.LCardDraftTitle),
                CFolioStateRead(card.LCardDraftExpression),
                CFolioStateRead(card.LCardDraftMeaning),
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
                        row.LRegisterDraftId, CFolioStateRead(row.LRegisterDraftName)))
                    .ToList(),
                card.LCardDraftTranslation,
                card.LCardDraftTag.Select(static row => new CTagDraft(row.LTagDraftId, row.LTagDraftText)).ToList(),
                CFolioImageRead(card.LCardDraftImage),
                CFolioVideoRead(card.LCardDraftVideo)))
            .ToList();
    }

    private static CSentenceDraft CFolioSentenceRead(LSentenceDraft sentence)
    {
        return new CSentenceDraft(
            sentence.LSentenceDraftId,
            CFolioExampleRead(sentence.LSentenceDraftExample),
            CFolioStateRead(sentence.LSentenceDraftParticle),
            CFolioStateRead(sentence.LSentenceDraftDependence));
    }

    private static CExampleDraft? CFolioExampleRead(LExampleDraft? example)
    {
        return example is null
            ? null
            : new CExampleDraft(
                CFolioStateRead(example.LExampleDraftText),
                example.LExampleDraftReference.LStateAnchorShown,
                CFolioGlossRead(example.LExampleDraftGloss),
                CFolioMentionRead(example.LExampleDraftMention));
    }

    internal static IReadOnlyList<CGlossDraft> CFolioGlossRead(IReadOnlyList<LGlossDraft> glosses)
    {
        return glosses
            .Select(static gloss => new CGlossDraft(
                gloss.LGlossDraftId, gloss.LGlossDraftLanguage, CFolioStateRead(gloss.LGlossDraftText)))
            .ToList();
    }

    private static IReadOnlyList<CMentionDraft> CFolioMentionRead(IReadOnlyList<LMentionDraft> mentions)
    {
        return mentions.Select(CFolioMentionRead).ToList();
    }

    internal static IReadOnlyList<CImageDraft> CFolioImageRead(IReadOnlyList<LImageDraft> images)
    {
        return images
            .Select(static image => new CImageDraft(image.LImageDraftId, CFolioStateRead(image.LImageDraftLocation)))
            .ToList();
    }

    internal static IReadOnlyList<CVideoDraft> CFolioVideoRead(IReadOnlyList<LVideoDraft> videos)
    {
        return videos
            .Select(static video => new CVideoDraft(
                video.LVideoDraftId,
                CFolioStateRead(video.LVideoDraftLocation),
                CFolioStateRead(video.LVideoDraftSpan)))
            .ToList();
    }

    internal static CMentionDraft CFolioMentionRead(LMentionDraft mention)
    {
        return new CMentionDraft(
            mention.LMentionDraftId,
            mention.LMentionDraftEntry,
            mention.LMentionDraftOffset,
            mention.LMentionDraftLength,
            mention.LMentionDraftSense);
    }
}
