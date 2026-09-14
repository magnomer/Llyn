using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LRequest TRequestHeadwordCreate(long draftId, string text) =>
        new LRequestHeadword(draftId, text);

    internal static LRequest TRequestNoteCreate(long draftId, string text) =>
        new LRequestNote(draftId, text);

    internal static LRequest TRequestTitleCreate(long draftId, long cardId, LStateValue value) =>
        new LRequestCardTitle(draftId, cardId, value.TStateWrittenRead());

    internal static LRequest TRequestAdditionCreate(long draftId, LCardKind kind, long parentId, int position) =>
        new LRequestCardAddition(draftId, kind, parentId, position);

    internal static LRequest TRequestRemovalCreate(long draftId, long cardId) =>
        new LRequestCardRemoval(draftId, cardId);

    internal static LRequest TRequestShiftCreate(long draftId, long cardId, long parentId, int position) =>
        new LRequestCardShift(draftId, cardId, parentId, position);

    internal static LRequest TRequestExpressionCreate(long draftId, long cardId, LStateValue value) =>
        new LRequestCardExpression(draftId, cardId, value.TStateWrittenRead());

    internal static LRequest TRequestMeaningCreate(long draftId, long cardId, LStateValue value) =>
        new LRequestCardMeaning(draftId, cardId, value.TStateWrittenRead());

    internal static LRequest TSentenceAdditionCreate(long draftId, long cardId, int position) =>
        new LRequestSentenceAddition(draftId, cardId, position);

    internal static LRequest TSentenceRemovalCreate(long draftId, long cardId, long sentenceId) =>
        new LRequestSentenceRemoval(draftId, cardId, sentenceId);

    internal static LRequest TSentenceShiftCreate(long draftId, long cardId, long sentenceId, int position) =>
        new LRequestSentenceShift(draftId, cardId, sentenceId, position);

    internal static LRequest TSentenceExampleCreate(long draftId, long cardId, long sentenceId, long exampleId) =>
        new LRequestSentenceExample(draftId, cardId, sentenceId, exampleId);

    internal static LRequest TSentenceTextCreate(long draftId, long cardId, long sentenceId, LStateValue value) =>
        new LRequestSentenceText(draftId, cardId, sentenceId, value.TStateWrittenRead());

    internal static LRequest TSentenceReferenceCreate(
        long draftId, long cardId, long sentenceId, long referenceId) =>
        new LRequestSentenceReference(draftId, cardId, sentenceId, referenceId);

    internal static LRequest TMentionAdditionCreate(
        long draftId, long cardId, long sentenceId, int offset, int length, long entryId, long senseId = 0) =>
        new LRequestMentionAddition(draftId, cardId, sentenceId, offset, length, entryId, senseId);

    internal static LRequest TMentionRemovalCreate(long draftId, long cardId, long sentenceId, long mentionId) =>
        new LRequestMentionRemoval(draftId, cardId, sentenceId, mentionId);

    internal static LRequest TGlossAdditionCreate(
        long draftId, long cardId, long sentenceId, string language, int position) =>
        new LRequestGlossAddition(draftId, cardId, sentenceId, language, position);

    internal static LRequest TGlossRemovalCreate(long draftId, long cardId, long sentenceId, long glossId) =>
        new LRequestGlossRemoval(draftId, cardId, sentenceId, glossId);

    internal static LRequest TGlossTextCreate(
        long draftId, long cardId, long sentenceId, long glossId, LStateValue value) =>
        new LRequestGlossText(draftId, cardId, sentenceId, glossId, value.TStateWrittenRead());

    internal static LRequest TGlossLanguageCreate(
        long draftId, long cardId, long sentenceId, long glossId, string language) =>
        new LRequestGlossLanguage(draftId, cardId, sentenceId, glossId, language);

    internal static LRequest TMentionSenseCreate(
        long draftId, long cardId, long sentenceId, long mentionId, long senseId) =>
        new LRequestMentionSense(draftId, cardId, sentenceId, mentionId, senseId);

    internal static LRequest TSituationAdditionCreate(long draftId, long cardId, string title, int position) =>
        new LRequestSituationAddition(draftId, cardId, TStateValueCreate(title).TStateWrittenRead(), position);

    internal static LRequest TSituationPickCreate(long draftId, long cardId, long situationId, int position) =>
        new LRequestSituationPick(draftId, cardId, situationId, position);

    internal static LRequest TSituationRemovalCreate(long draftId, long cardId, long situationId) =>
        new LRequestSituationRemoval(draftId, cardId, situationId);

    internal static LRequest TSituationShiftCreate(long draftId, long cardId, long situationId, int position) =>
        new LRequestSituationShift(draftId, cardId, situationId, position);

    internal static LRequest TSituationTitleCreate(long draftId, long situationId, LStateValue value) =>
        new LRequestSituationTitle(draftId, situationId, value.TStateWrittenRead());

    internal static LRequest TRegisterAdditionCreate(long draftId, long cardId, string name, int position) =>
        new LRequestRegisterAddition(draftId, cardId, TStateValueCreate(name).TStateWrittenRead(), position);

    internal static LRequest TRegisterPickCreate(long draftId, long cardId, long registerId, int position) =>
        new LRequestRegisterPick(draftId, cardId, registerId, position);

    internal static LRequest TTagAdditionCreate(long draftId, long cardId, string text, int position) =>
        new LRequestTagAddition(draftId, cardId, text, position);

    internal static LRequest TTagPickCreate(long draftId, long cardId, long tagId, int position) =>
        new LRequestTagPick(draftId, cardId, tagId, position);

    internal static LRequest TTranslationPickCreate(long draftId, long cardId, long entryId, int position) =>
        new LRequestTranslationPick(draftId, cardId, entryId, position);

    internal static LRequest TImageAdditionCreate(long draftId, long cardId, string location, int position) =>
        new LRequestImageAddition(draftId, cardId, TStateValueCreate(location).TStateWrittenRead(), position);

    internal static LRequest TImageRemovalCreate(long draftId, long cardId, long imageId) =>
        new LRequestImageRemoval(draftId, cardId, imageId);

    internal static LRequest TVideoRemovalCreate(long draftId, long cardId, long videoId) =>
        new LRequestVideoRemoval(draftId, cardId, videoId);

    internal static LRequest TImageLocationCreate(long draftId, long imageId, LStateValue value) =>
        new LRequestImageLocation(draftId, imageId, value.TStateWrittenRead());

    internal static LRequest TVideoAdditionCreate(long draftId, long cardId, string location, int position) =>
        new LRequestVideoAddition(draftId, cardId, TStateValueCreate(location).TStateWrittenRead(), position);

    internal static LRequest TExampleTextCreate(long draftId, LStateValue value) =>
        new LRequestExampleText(draftId, value.TStateWrittenRead());

    internal static LRequest TExampleLanguageCreate(long draftId, string language) =>
        new LRequestExampleLanguage(draftId, language);

    internal static LRequest TReferenceTitleCreate(long draftId, LStateValue value) =>
        new LRequestReferenceTitle(draftId, value.TStateWrittenRead());

    internal static LRequest TAuthorAdditionCreate(long draftId, string name, int position) =>
        new LRequestAuthorAddition(draftId, name, position);

    internal static LRequest TAuthorPickCreate(long draftId, long authorId, int position) =>
        new LRequestAuthorPick(draftId, authorId, position);

    internal static LRequest TAuthorRemovalCreate(long draftId, long authorId) =>
        new LRequestAuthorRemoval(draftId, authorId);

    internal static LRequest TAuthorShiftCreate(long draftId, long authorId, int position) =>
        new LRequestAuthorShift(draftId, authorId, position);

    internal static LRequest TRequestIpaCreate(long draftId, string text) =>
        new LRequestIpa(draftId, text);

    internal static LRequest TRequestAudioCreate(long draftId, string file, string? source) =>
        new LRequestAudio(draftId, file, source);

    internal static LRequest TPronunciationAdditionCreate(long draftId, string ipa, int position) =>
        new LRequestPronunciationAddition(draftId, ipa, position);

    internal static LRequest TPronunciationRemovalCreate(long draftId, long pronunciationId) =>
        new LRequestPronunciationRemoval(draftId, pronunciationId);

    internal static LRequest TPronunciationShiftCreate(long draftId, long pronunciationId, int position) =>
        new LRequestPronunciationShift(draftId, pronunciationId, position);

    internal static LRequest TPronunciationIpaCreate(long draftId, long pronunciationId, string text) =>
        new LRequestPronunciationIpa(draftId, pronunciationId, text);

    internal static LRequest TPronunciationVarietyCreate(long draftId, long pronunciationId, string text) =>
        new LRequestPronunciationVariety(draftId, pronunciationId, text);

    internal static LRequest TPronunciationAudioCreate(
        long draftId, long pronunciationId, string file, string? source) =>
        new LRequestPronunciationAudio(draftId, pronunciationId, file, source);

    internal static LRequest TTranscriptionAdditionCreate(long draftId, string scheme, int position, bool seeded = false) =>
        new LRequestTranscriptionAddition(draftId, scheme, position, seeded);

    internal static LRequest TTranscriptionRemovalCreate(long draftId, long transcriptionId) =>
        new LRequestTranscriptionRemoval(draftId, transcriptionId);

    internal static LRequest TTranscriptionShiftCreate(long draftId, long transcriptionId, int position) =>
        new LRequestTranscriptionShift(draftId, transcriptionId, position);

    internal static LRequest TTranscriptionSchemeCreate(long draftId, long transcriptionId, string text) =>
        new LRequestTranscriptionScheme(draftId, transcriptionId, text);

    internal static LRequest TTranscriptionTextCreate(long draftId, long transcriptionId, string text) =>
        new LRequestTranscriptionText(draftId, transcriptionId, text);

    internal static LRequest TReferenceBodyCreate(long draftId, LReference reference) =>
        new LRequestReferenceBody(
            draftId,
            reference.LReferenceTitle.TStateWrittenRead(),
            reference.LReferenceYear.TStateWrittenRead(),
            reference.LReferenceKind,
            reference.LReferenceNote.TStateWrittenRead(),
            reference.LReferenceUrl.TStateWrittenRead(),
            reference.LReferenceAuthorState.LStateMarkState);

    internal static LRequest TExampleBodyCreate(long draftId, LExample example) =>
        new LRequestExampleBody(
            draftId,
            example.LExampleLanguage,
            example.LExampleText.TStateWrittenRead(),
            example.LExampleSource.LStateAnchorShow());

    internal static LRequest TSituationBodyCreate(long draftId, long situationId, LSituation situation) =>
        new LRequestSituationBody(
            draftId,
            situationId,
            situation.LSituationTitle.TStateWrittenRead(),
            situation.LSituationDescription.TStateWrittenRead(),
            situation.LSituationKind.TStateWrittenRead());

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

        held = TRequestReadingApply(engine, draftId, held, content);

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

    private static LDraft TRequestReadingApply(LEngine engine, long draftId, LDraft held, LEntryDraft content)
    {
        foreach (LPronunciationDraft spoken in held.LDraftContent.LEntryDraftPronunciations)
        {
            held = engine.LEngineRequestApply(
                new LRequestPronunciationRemoval(draftId, spoken.LPronunciationDraftId));
        }

        foreach (LTranscriptionDraft spelled in held.LDraftContent.LEntryDraftTranscriptions)
        {
            held = engine.LEngineRequestApply(
                new LRequestTranscriptionRemoval(draftId, spelled.LTranscriptionDraftId));
        }

        for (int index = 0; index < content.LEntryDraftPronunciations.Count; index++)
        {
            LPronunciationDraft spoken = content.LEntryDraftPronunciations[index];
            held = engine.LEngineRequestApply(
                new LRequestPronunciationAddition(draftId, spoken.LPronunciationDraftIpa, index));
            long spokenId = held.LDraftContent.LEntryDraftPronunciations[index].LPronunciationDraftId;
            held = engine.LEngineRequestApply(
                new LRequestPronunciationVariety(draftId, spokenId, spoken.LPronunciationDraftVariety));
            held = engine.LEngineRequestApply(new LRequestPronunciationAudio(
                draftId, spokenId, spoken.LPronunciationDraftAudio, spoken.LPronunciationDraftSource));
        }

        for (int index = 0; index < content.LEntryDraftTranscriptions.Count; index++)
        {
            LTranscriptionDraft spelled = content.LEntryDraftTranscriptions[index];
            held = engine.LEngineRequestApply(
                new LRequestTranscriptionAddition(draftId, spelled.LTranscriptionDraftScheme, index));
            long spelledId = held.LDraftContent.LEntryDraftTranscriptions[index].LTranscriptionDraftId;
            held = engine.LEngineRequestApply(
                new LRequestTranscriptionText(draftId, spelledId, spelled.LTranscriptionDraftText));
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

        held = engine.LEngineRequestApply(new LRequestCardTitle(draftId, cardId, card.LCardDraftTitle.TStateWrittenRead()));
        held = engine.LEngineRequestApply(new LRequestCardExpression(draftId, cardId, card.LCardDraftExpression.TStateWrittenRead()));
        held = engine.LEngineRequestApply(new LRequestCardMeaning(draftId, cardId, card.LCardDraftMeaning.TStateWrittenRead()));

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
                    new LRequestSituationAddition(draftId, cardId, situation.LSituationDraftTitle.TStateWrittenRead(), index));

            long situationId = TRequestCardFind(held.LDraftContent, cardId).LCardDraftSituation[index].LSituationDraftId;
            held = engine.LEngineRequestApply(
                new LRequestSituationDescription(draftId, situationId, situation.LSituationDraftDescription.TStateWrittenRead()));
            held = engine.LEngineRequestApply(
                new LRequestSituationKind(draftId, situationId, situation.LSituationDraftKind.TStateWrittenRead()));
        }

        for (int index = 0; index < card.LCardDraftRegister.Count; index++)
        {
            LRegisterDraft register = card.LCardDraftRegister[index];
            held = register.LRegisterDraftId > 0
                ? engine.LEngineRequestApply(
                    new LRequestRegisterPick(draftId, cardId, register.LRegisterDraftId, index))
                : engine.LEngineRequestApply(
                    new LRequestRegisterAddition(draftId, cardId, register.LRegisterDraftName.TStateWrittenRead(), index));
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
                    new LRequestImageAddition(draftId, cardId, image.LImageDraftLocation.TStateWrittenRead(), index));
        }

        for (int index = 0; index < card.LCardDraftVideo.Count; index++)
        {
            LVideoDraft video = card.LCardDraftVideo[index];
            held = video.LVideoDraftId > 0
                ? engine.LEngineRequestApply(new LRequestVideoPick(draftId, cardId, video.LVideoDraftId, index))
                : engine.LEngineRequestApply(
                    new LRequestVideoAddition(draftId, cardId, video.LVideoDraftLocation.TStateWrittenRead(), index));

            long videoId = TRequestCardFind(held.LDraftContent, cardId).LCardDraftVideo[index].LVideoDraftId;
            held = engine.LEngineRequestApply(new LRequestVideoSpan(draftId, videoId, video.LVideoDraftSpan.TStateWrittenRead()));
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
                new LRequestSentenceText(draftId, cardId, sentenceId, example.LExampleDraftText.TStateWrittenRead()));

            if (!example.LExampleDraftReference.LStateAnchorEmpty)
            {
                held = engine.LEngineRequestApply(new LRequestSentenceReference(
                    draftId, cardId, sentenceId, example.LExampleDraftReference.LStateAnchorShow()));
            }
        }

        held = engine.LEngineRequestApply(
            new LRequestSentenceParticle(draftId, cardId, sentenceId, sentence.LSentenceDraftParticle.TStateWrittenRead()));
        held = engine.LEngineRequestApply(
            new LRequestSentenceDependence(draftId, cardId, sentenceId, sentence.LSentenceDraftDependence.TStateWrittenRead()));

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
