using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LRequest TRequestHeadwordCreate(long draftId, string text) =>
        new LRequestHeadword(draftId, text);

    internal static LRequest TRequestTitleCreate(long draftId, long cardId, LStateValue value) =>
        new LRequestCardTitle(draftId, cardId, value);

    internal static LRequest TRequestAdditionCreate(long draftId, LCardKind kind, long parentId, int position) =>
        new LRequestCardAddition(draftId, kind, parentId, position);

    internal static LRequest TRequestRemovalCreate(long draftId, long cardId) =>
        new LRequestCardRemoval(draftId, cardId);

    internal static LRequest TRequestShiftCreate(long draftId, long cardId, long parentId, int position) =>
        new LRequestCardShift(draftId, cardId, parentId, position);

    internal static LRequest TRequestExpressionCreate(long draftId, long cardId, LStateValue value) =>
        new LRequestCardExpression(draftId, cardId, value);

    internal static LRequest TRequestMeaningCreate(long draftId, long cardId, LStateValue value) =>
        new LRequestCardMeaning(draftId, cardId, value);

    internal static LRequest TSentenceAdditionCreate(long draftId, long cardId, int position) =>
        new LRequestSentenceAddition(draftId, cardId, position);

    internal static LRequest TSentenceRemovalCreate(long draftId, long cardId, long sentenceId) =>
        new LRequestSentenceRemoval(draftId, cardId, sentenceId);

    internal static LRequest TSentenceShiftCreate(long draftId, long cardId, long sentenceId, int position) =>
        new LRequestSentenceShift(draftId, cardId, sentenceId, position);

    internal static LRequest TSentenceExampleCreate(long draftId, long cardId, long sentenceId, long exampleId) =>
        new LRequestSentenceExample(draftId, cardId, sentenceId, exampleId);

    internal static LRequest TSentenceTextCreate(long draftId, long cardId, long sentenceId, LStateValue value) =>
        new LRequestSentenceText(draftId, cardId, sentenceId, value);

    internal static LRequest TSentenceReferenceCreate(
        long draftId, long cardId, long sentenceId, long referenceId) =>
        new LRequestSentenceReference(draftId, cardId, sentenceId, referenceId);

    internal static LRequest TSituationAdditionCreate(long draftId, long cardId, string title, int position) =>
        new LRequestSituationAddition(draftId, cardId, TStateValueCreate(title), position);

    internal static LRequest TSituationPickCreate(long draftId, long cardId, long situationId, int position) =>
        new LRequestSituationPick(draftId, cardId, situationId, position);

    internal static LRequest TSituationRemovalCreate(long draftId, long cardId, long situationId) =>
        new LRequestSituationRemoval(draftId, cardId, situationId);

    internal static LRequest TSituationShiftCreate(long draftId, long cardId, long situationId, int position) =>
        new LRequestSituationShift(draftId, cardId, situationId, position);

    internal static LRequest TSituationTitleCreate(long draftId, long situationId, LStateValue value) =>
        new LRequestSituationTitle(draftId, situationId, value);

    internal static LRequest TRegisterAdditionCreate(long draftId, long cardId, string name, int position) =>
        new LRequestRegisterAddition(draftId, cardId, TStateValueCreate(name), position);

    internal static LRequest TRegisterPickCreate(long draftId, long cardId, long registerId, int position) =>
        new LRequestRegisterPick(draftId, cardId, registerId, position);

    internal static LRequest TTagAdditionCreate(long draftId, long cardId, string text, int position) =>
        new LRequestTagAddition(draftId, cardId, text, position);

    internal static LRequest TTagPickCreate(long draftId, long cardId, long tagId, int position) =>
        new LRequestTagPick(draftId, cardId, tagId, position);

    internal static LRequest TTranslationPickCreate(long draftId, long cardId, long entryId, int position) =>
        new LRequestTranslationPick(draftId, cardId, entryId, position);

    internal static LRequest TImageAdditionCreate(long draftId, long cardId, string location, int position) =>
        new LRequestImageAddition(draftId, cardId, TStateValueCreate(location), position);

    internal static LRequest TImageLocationCreate(long draftId, long imageId, LStateValue value) =>
        new LRequestImageLocation(draftId, imageId, value);

    internal static LRequest TVideoAdditionCreate(long draftId, long cardId, string location, int position) =>
        new LRequestVideoAddition(draftId, cardId, TStateValueCreate(location), position);

    internal static LRequest TExampleTextCreate(long draftId, LStateValue value) =>
        new LRequestExampleText(draftId, value);

    internal static LRequest TExampleLanguageCreate(long draftId, string language) =>
        new LRequestExampleLanguage(draftId, language);

    internal static LRequest TReferenceTitleCreate(long draftId, LStateValue value) =>
        new LRequestReferenceTitle(draftId, value);

    internal static LRequest TAuthorAdditionCreate(long draftId, string name, int position) =>
        new LRequestAuthorAddition(draftId, name, position);

    internal static LRequest TAuthorPickCreate(long draftId, long authorId, int position) =>
        new LRequestAuthorPick(draftId, authorId, position);

    internal static LRequest TAuthorRemovalCreate(long draftId, long authorId) =>
        new LRequestAuthorRemoval(draftId, authorId);

    internal static LRequest TAuthorShiftCreate(long draftId, long authorId, int position) =>
        new LRequestAuthorShift(draftId, authorId, position);

    internal static LRequest TReferenceBodyCreate(long draftId, LReference reference) =>
        new LRequestReferenceBody(draftId, reference);

    internal static LRequest TExampleBodyCreate(long draftId, LExample example) =>
        new LRequestExampleBody(draftId, example);

    internal static LRequest TSituationBodyCreate(long draftId, long situationId, LSituation situation) =>
        new LRequestSituationBody(draftId, situationId, situation);

    internal static LDraft TRequestContentApply(this LEngine engine, long draftId, LEntryDraft content)
    {
        LDraft held = engine.LEngineDraftRead(draftId)!;

        if (!string.Equals(held.LDraftContent.LEntryDraftHeadword, content.LEntryDraftHeadword, StringComparison.Ordinal))
        {
            held = engine.LEngineRequestApply(new LRequestHeadword(draftId, content.LEntryDraftHeadword));
        }

        if (!string.Equals(held.LDraftContent.LEntryDraftLanguage, content.LEntryDraftLanguage, StringComparison.Ordinal))
        {
            held = engine.LEngineRequestApply(new LRequestLanguage(draftId, content.LEntryDraftLanguage));
        }

        if (!string.Equals(held.LDraftContent.LEntryDraftNote, content.LEntryDraftNote, StringComparison.Ordinal))
        {
            held = engine.LEngineRequestApply(new LRequestNote(draftId, content.LEntryDraftNote));
        }

        if (!string.Equals(held.LDraftContent.LEntryDraftIpa, content.LEntryDraftIpa, StringComparison.Ordinal))
        {
            held = engine.LEngineRequestApply(new LRequestIpa(draftId, content.LEntryDraftIpa));
        }

        if (content.LEntryDraftSpeeches.Count > 0)
        {
            held = engine.LEngineRequestApply(new LRequestSpeech(draftId, content.LEntryDraftSpeeches));
        }

        foreach (LCardDraft card in held.LDraftContent.LEntryDraftMeanings)
        {
            held = engine.LEngineRequestApply(new LRequestCardRemoval(draftId, card.LCardDraftId));
        }

        foreach (LCardDraft card in held.LDraftContent.LEntryDraftCollocations)
        {
            held = engine.LEngineRequestApply(new LRequestCardRemoval(draftId, card.LCardDraftId));
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

    private static LDraft TRequestCardApply(
        LEngine engine, long draftId, LCardKind kind, long parentId, int position, LCardDraft card)
    {
        LDraft held = engine.LEngineRequestApply(new LRequestCardAddition(draftId, kind, parentId, position));
        IReadOnlyList<LCardDraft> siblings = parentId != 0
            ? TRequestCardFind(held.LDraftContent.LEntryDraftMeanings, parentId)!.LCardDraftChild
            : kind == LCardKind.LCardKindMeaning
                ? held.LDraftContent.LEntryDraftMeanings
                : held.LDraftContent.LEntryDraftCollocations;
        long cardId = siblings[position].LCardDraftId;

        held = engine.LEngineRequestApply(new LRequestCardTitle(draftId, cardId, card.LCardDraftTitle));
        held = engine.LEngineRequestApply(new LRequestCardExpression(draftId, cardId, card.LCardDraftExpression));
        held = engine.LEngineRequestApply(new LRequestCardMeaning(draftId, cardId, card.LCardDraftMeaning));

        if (card.LCardDraftGloss is not null)
        {
            held = engine.LEngineRequestApply(new LRequestCardGloss(draftId, cardId, card.LCardDraftGloss));
        }

        if (card.LCardDraftLabels.Length > 0)
        {
            held = engine.LEngineRequestApply(new LRequestCardLabels(draftId, cardId, card.LCardDraftLabels));
        }

        for (int index = 0; index < card.LCardDraftSentence.Count; index++)
        {
            held = TRequestSentenceApply(engine, draftId, cardId, index, card.LCardDraftSentence[index]);
        }

        for (int index = 0; index < card.LCardDraftSituation.Count; index++)
        {
            LSituationDraft situation = card.LCardDraftSituation[index];
            held = situation.LSituationDraftId > 0
                ? engine.LEngineRequestApply(
                    new LRequestSituationPick(draftId, cardId, situation.LSituationDraftId, index))
                : engine.LEngineRequestApply(
                    new LRequestSituationAddition(draftId, cardId, situation.LSituationDraftTitle, index));

            long situationId = TRequestCardFind(held.LDraftContent, cardId).LCardDraftSituation[index].LSituationDraftId;
            held = engine.LEngineRequestApply(
                new LRequestSituationDescription(draftId, situationId, situation.LSituationDraftDescription));
            held = engine.LEngineRequestApply(
                new LRequestSituationKind(draftId, situationId, situation.LSituationDraftKind));
        }

        for (int index = 0; index < card.LCardDraftRegister.Count; index++)
        {
            LRegisterDraft register = card.LCardDraftRegister[index];
            held = register.LRegisterDraftId > 0
                ? engine.LEngineRequestApply(
                    new LRequestRegisterPick(draftId, cardId, register.LRegisterDraftId, index))
                : engine.LEngineRequestApply(
                    new LRequestRegisterAddition(draftId, cardId, register.LRegisterDraftName, index));
        }

        for (int index = 0; index < card.LCardDraftTag.Count; index++)
        {
            LTagDraft tag = card.LCardDraftTag[index];
            held = tag.LTagDraftId > 0
                ? engine.LEngineRequestApply(new LRequestTagPick(draftId, cardId, tag.LTagDraftId, index))
                : engine.LEngineRequestApply(new LRequestTagAddition(draftId, cardId, tag.LTagDraftText, index));
        }

        for (int index = 0; index < card.LCardDraftTranslation.Count; index++)
        {
            held = engine.LEngineRequestApply(
                new LRequestTranslationPick(draftId, cardId, card.LCardDraftTranslation[index], index));
        }

        for (int index = 0; index < card.LCardDraftImage.Count; index++)
        {
            LImageDraft image = card.LCardDraftImage[index];
            held = image.LImageDraftId > 0
                ? engine.LEngineRequestApply(new LRequestImagePick(draftId, cardId, image.LImageDraftId, index))
                : engine.LEngineRequestApply(
                    new LRequestImageAddition(draftId, cardId, image.LImageDraftLocation, index));
        }

        for (int index = 0; index < card.LCardDraftVideo.Count; index++)
        {
            LVideoDraft video = card.LCardDraftVideo[index];
            held = video.LVideoDraftId > 0
                ? engine.LEngineRequestApply(new LRequestVideoPick(draftId, cardId, video.LVideoDraftId, index))
                : engine.LEngineRequestApply(
                    new LRequestVideoAddition(draftId, cardId, video.LVideoDraftLocation, index));

            long videoId = TRequestCardFind(held.LDraftContent, cardId).LCardDraftVideo[index].LVideoDraftId;
            held = engine.LEngineRequestApply(new LRequestVideoSpan(draftId, videoId, video.LVideoDraftSpan));
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
        LDraft held = engine.LEngineRequestApply(new LRequestSentenceAddition(draftId, cardId, position));
        long sentenceId = TRequestCardFind(held.LDraftContent, cardId).LCardDraftSentence[position].LSentenceDraftId;

        if (sentence.LSentenceDraftExample is LExampleDraft example)
        {
            if (example.LExampleDraftId > 0)
            {
                held = engine.LEngineRequestApply(
                    new LRequestSentenceExample(draftId, cardId, sentenceId, example.LExampleDraftId));
            }

            held = engine.LEngineRequestApply(
                new LRequestSentenceText(draftId, cardId, sentenceId, example.LExampleDraftText));

            if (!example.LExampleDraftReference.LStateAnchorEmpty)
            {
                held = engine.LEngineRequestApply(new LRequestSentenceReference(
                    draftId, cardId, sentenceId, example.LExampleDraftReference.LStateAnchorShow()));
            }
        }

        held = engine.LEngineRequestApply(
            new LRequestSentenceParticle(draftId, cardId, sentenceId, sentence.LSentenceDraftParticle));
        held = engine.LEngineRequestApply(
            new LRequestSentenceDependence(draftId, cardId, sentenceId, sentence.LSentenceDraftDependence));

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
