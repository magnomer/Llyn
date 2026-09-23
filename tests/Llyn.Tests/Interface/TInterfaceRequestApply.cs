using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LDraft TRequestContentApply(this LEngine engine, long draftId, LEntryDraft content)
    {
        LDraft held = engine.LEngineDraft.LEngineDraftRead(draftId)!;

        if (!string.Equals(
            held.LDraftContent.LEntryDraftHeadword, content.LEntryDraftHeadword, StringComparison.Ordinal))
        {
            held = engine.LEngineRequest.LEngineRequestApply(
                new LRequestHeadword(draftId, content.LEntryDraftHeadword));
        }

        if (!string.Equals(
            held.LDraftContent.LEntryDraftLanguage, content.LEntryDraftLanguage, StringComparison.Ordinal))
        {
            held = engine.LEngineRequest.LEngineRequestApply(
                new LRequestLanguage(draftId, content.LEntryDraftLanguage));
        }

        if (!string.Equals(held.LDraftContent.LEntryDraftNote, content.LEntryDraftNote, StringComparison.Ordinal))
        {
            held = engine.LEngineRequest.LEngineRequestApply(new LRequestNote(draftId, content.LEntryDraftNote));
        }

        held = TRequestReadingApply(engine, draftId, held, content);

        if (content.LEntryDraftSpeeches.Count > 0)
        {
            held = engine.LEngineRequest.LEngineRequestApply(new LRequestSpeech(draftId, content.LEntryDraftSpeeches));
        }

        foreach (LCardDraft card in held.LDraftContent.LEntryDraftMeanings)
        {
            held = engine.LEngineRequest.LEngineRequestApply(new LRequestCardRemoval(draftId, card.LCardDraftId));
        }

        foreach (LCardDraft card in held.LDraftContent.LEntryDraftCollocations)
        {
            held = engine.LEngineRequest.LEngineRequestApply(new LRequestCardRemoval(draftId, card.LCardDraftId));
        }

        for (int index = 0; index < content.LEntryDraftMeanings.Count; index++)
        {
            held = TRequestCardApply(
                engine, draftId, LCardKind.LCardKindMeaning, 0, index, content.LEntryDraftMeanings[index]);
        }

        for (int index = 0; index < content.LEntryDraftCollocations.Count; index++)
        {
            held = TRequestCardApply(
                engine, draftId, LCardKind.LCardKindCollocation, 0, index, content.LEntryDraftCollocations[index]);
        }

        return held;
    }

    private static LDraft TRequestReadingApply(LEngine engine, long draftId, LDraft held, LEntryDraft content)
    {
        foreach (LPronunciationDraft spoken in held.LDraftContent.LEntryDraftPronunciations)
        {
            held = engine.LEngineRequest.LEngineRequestApply(
                new LRequestPronunciationRemoval(draftId, spoken.LPronunciationDraftId));
        }

        foreach (LTranscriptionDraft spelled in held.LDraftContent.LEntryDraftTranscriptions)
        {
            held = engine.LEngineRequest.LEngineRequestApply(
                new LRequestTranscriptionRemoval(draftId, spelled.LTranscriptionDraftId));
        }

        for (int index = 0; index < content.LEntryDraftPronunciations.Count; index++)
        {
            LPronunciationDraft spoken = content.LEntryDraftPronunciations[index];
            held = engine.LEngineRequest.LEngineRequestApply(
                new LRequestPronunciationAddition(draftId, spoken.LPronunciationDraftIpa, index));
            long spokenId = held.LDraftContent.LEntryDraftPronunciations[index].LPronunciationDraftId;
            held = engine.LEngineRequest.LEngineRequestApply(
                new LRequestPronunciationVariety(draftId, spokenId, spoken.LPronunciationDraftVariety));
            held = engine.LEngineRequest.LEngineRequestApply(
                new LRequestPronunciationRespelling(draftId, spokenId, spoken.LPronunciationDraftRespelling));
            held = engine.LEngineRequest.LEngineRequestApply(new LRequestPronunciationAudio(
                draftId, spokenId, spoken.LPronunciationDraftAudio, spoken.LPronunciationDraftSource));
        }

        for (int index = 0; index < content.LEntryDraftTranscriptions.Count; index++)
        {
            LTranscriptionDraft spelled = content.LEntryDraftTranscriptions[index];
            held = engine.LEngineRequest.LEngineRequestApply(
                new LRequestTranscriptionAddition(draftId, spelled.LTranscriptionDraftScheme, index));
            long spelledId = held.LDraftContent.LEntryDraftTranscriptions[index].LTranscriptionDraftId;
            held = engine.LEngineRequest.LEngineRequestApply(
                new LRequestTranscriptionText(draftId, spelledId, spelled.LTranscriptionDraftText));
        }

        return held;
    }

    private static LDraft TRequestCardApply(
        LEngine engine, long draftId, LCardKind kind, long parentId, int position, LCardDraft card)
    {
        LDraft held = engine.LEngineRequest.LEngineRequestApply(
            new LRequestCardAddition(draftId, kind, parentId, position));
        IReadOnlyList<LCardDraft> siblings = parentId != 0
            ? TRequestCardFind(held.LDraftContent.LEntryDraftMeanings, parentId)!.LCardDraftChild
            : kind == LCardKind.LCardKindMeaning
                ? held.LDraftContent.LEntryDraftMeanings
                : held.LDraftContent.LEntryDraftCollocations;
        long cardId = siblings[position].LCardDraftId;

        held = engine.LEngineRequest.LEngineRequestApply(
            new LRequestCardTitle(draftId, cardId, card.LCardDraftTitle.TStateWrittenRead()));
        held = engine.LEngineRequest.LEngineRequestApply(
            new LRequestCardExpression(draftId, cardId, card.LCardDraftExpression.TStateWrittenRead()));
        held = engine.LEngineRequest.LEngineRequestApply(
            new LRequestCardMeaning(draftId, cardId, card.LCardDraftMeaning.TStateWrittenRead()));

        for (int index = 0; index < card.LCardDraftSentence.Count; index++)
        {
            held = TRequestSentenceApply(engine, draftId, cardId, index, card.LCardDraftSentence[index]);
        }

        for (int index = 0; index < card.LCardDraftSituation.Count; index++)
        {
            LSituationDraft situation = card.LCardDraftSituation[index];
            held = situation.LSituationDraftId > 0
                ? engine.LEngineRequest.LEngineRequestApply(
                    new LRequestSituationPick(draftId, cardId, situation.LSituationDraftId, index))
                : engine.LEngineRequest.LEngineRequestApply(
                    new LRequestSituationAddition(
                        draftId, cardId, situation.LSituationDraftTitle.TStateWrittenRead(), index));

            long situationId =
                TRequestCardFind(held.LDraftContent, cardId).LCardDraftSituation[index].LSituationDraftId;
            held = engine.LEngineRequest.LEngineRequestApply(
                new LRequestSituationDescription(
                    draftId, situationId, situation.LSituationDraftDescription.TStateWrittenRead()));
            held = engine.LEngineRequest.LEngineRequestApply(
                new LRequestSituationKind(draftId, situationId, situation.LSituationDraftKind.TStateWrittenRead()));
        }

        for (int index = 0; index < card.LCardDraftRegister.Count; index++)
        {
            LRegisterDraft register = card.LCardDraftRegister[index];
            held = register.LRegisterDraftId > 0
                ? engine.LEngineRequest.LEngineRequestApply(
                    new LRequestRegisterPick(draftId, cardId, register.LRegisterDraftId, index))
                : engine.LEngineRequest.LEngineRequestApply(
                    new LRequestRegisterAddition(
                        draftId, cardId, register.LRegisterDraftName.TStateWrittenRead(), index));
        }

        for (int index = 0; index < card.LCardDraftTag.Count; index++)
        {
            LTagDraft tag = card.LCardDraftTag[index];
            held = tag.LTagDraftId > 0
                ? engine.LEngineRequest.LEngineRequestApply(
                    new LRequestTagPick(draftId, cardId, tag.LTagDraftId, index))
                : engine.LEngineRequest.LEngineRequestApply(
                    new LRequestTagAddition(draftId, cardId, tag.LTagDraftText, index));
        }

        for (int index = 0; index < card.LCardDraftTranslation.Count; index++)
        {
            held = engine.LEngineRequest.LEngineRequestApply(
                new LRequestTranslationPick(draftId, cardId, card.LCardDraftTranslation[index], index));
        }

        for (int index = 0; index < card.LCardDraftImage.Count; index++)
        {
            LImageDraft image = card.LCardDraftImage[index];
            held = image.LImageDraftId > 0
                ? engine.LEngineRequest.LEngineRequestApply(
                    new LRequestImagePick(draftId, cardId, image.LImageDraftId, index))
                : engine.LEngineRequest.LEngineRequestApply(
                    new LRequestImageAddition(draftId, cardId, image.LImageDraftLocation.TStateWrittenRead(), index));
        }

        for (int index = 0; index < card.LCardDraftVideo.Count; index++)
        {
            LVideoDraft video = card.LCardDraftVideo[index];
            held = video.LVideoDraftId > 0
                ? engine.LEngineRequest.LEngineRequestApply(
                    new LRequestVideoPick(draftId, cardId, video.LVideoDraftId, index))
                : engine.LEngineRequest.LEngineRequestApply(
                    new LRequestVideoAddition(draftId, cardId, video.LVideoDraftLocation.TStateWrittenRead(), index));

            long videoId = TRequestCardFind(held.LDraftContent, cardId).LCardDraftVideo[index].LVideoDraftId;
            held = engine.LEngineRequest.LEngineRequestApply(
                new LRequestVideoSpan(draftId, videoId, video.LVideoDraftSpan.TStateWrittenRead()));
        }

        for (int index = 0; index < card.LCardDraftChild.Count; index++)
        {
            held = TRequestCardApply(engine, draftId, kind, cardId, index, card.LCardDraftChild[index]);
        }

        return held;
    }

    private static LDraft TRequestSentenceApply(
        LEngine engine, long draftId, long cardId, int position, LSentenceDraft sentence)
    {
        LDraft held = engine.LEngineRequest.LEngineRequestApply(
            new LRequestSentenceAddition(draftId, cardId, position));
        long sentenceId = TRequestCardFind(held.LDraftContent, cardId).LCardDraftSentence[position].LSentenceDraftId;

        if (sentence.LSentenceDraftExample is LExampleDraft example)
        {
            if (example.LExampleDraftId > 0)
            {
                held = engine.LEngineRequest.LEngineRequestApply(
                    new LRequestSentenceExample(draftId, cardId, sentenceId, example.LExampleDraftId));
            }

            held = engine.LEngineRequest.LEngineRequestApply(
                new LRequestSentenceText(draftId, cardId, sentenceId, example.LExampleDraftText.TStateWrittenRead()));

            if (!example.LExampleDraftReference.LStateAnchorEmpty)
            {
                held = engine.LEngineRequest.LEngineRequestApply(new LRequestSentenceReference(
                    draftId, cardId, sentenceId, example.LExampleDraftReference.LStateAnchorShow()));
            }
        }

        held = engine.LEngineRequest.LEngineRequestApply(
            new LRequestSentenceParticle(
                draftId, cardId, sentenceId, sentence.LSentenceDraftParticle.TStateWrittenRead()));
        held = engine.LEngineRequest.LEngineRequestApply(
            new LRequestSentenceDependence(
                draftId, cardId, sentenceId, sentence.LSentenceDraftDependence.TStateWrittenRead()));

        return held;
    }

    internal static LCardDraft TRequestCardFind(LEntryDraft content, long cardId)
    {
        return TRequestCardFind(content.LEntryDraftMeanings, cardId)
            ?? TRequestCardFind(content.LEntryDraftCollocations, cardId)
            ?? throw new InvalidOperationException("The card is not in the draft.");
    }

    private static LCardDraft? TRequestCardFind(IReadOnlyList<LCardDraft> cards, long cardId)
    {
        foreach (LCardDraft card in cards)
        {
            if (card.LCardDraftId == cardId)
            {
                return card;
            }

            if (TRequestCardFind(card.LCardDraftChild, cardId) is LCardDraft child)
            {
                return child;
            }
        }

        return null;
    }
}
