using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LRequest TRequestHeadwordCreate(long draftId, string text) =>
        new LRequestHeadword(draftId, text);

    internal static LRequest TRequestNoteCreate(long draftId, string text) =>
        new LRequestNote(draftId, text);

    internal static LRequest TRequestAuthorCreate(long draftId, string text) =>
        new LRequestAuthorName(draftId, text);

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

    internal static LRequest TAuthorNameCreate(long draftId, string name) =>
        new LRequestAuthorName(draftId, name);

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

    internal static LRequest TRequestRespellingCreate(long draftId, string text) =>
        new LRequestRespelling(draftId, text);

    internal static LRequest TRequestLanguageCreate(long draftId, string text) =>
        new LRequestLanguage(draftId, text);

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

    internal static LRequest TPronunciationRespellingCreate(long draftId, long pronunciationId, string text) =>
        new LRequestPronunciationRespelling(draftId, pronunciationId, text);

    internal static LRequest TPronunciationAudioCreate(
        long draftId, long pronunciationId, string file, string? source) =>
        new LRequestPronunciationAudio(draftId, pronunciationId, file, source);

    internal static LRequest TTranscriptionAdditionCreate(
        long draftId, string scheme, int position, bool seeded = false) =>
        new LRequestTranscriptionAddition(draftId, scheme, position, seeded);

    internal static LRequest TTranscriptionRemovalCreate(long draftId, long transcriptionId) =>
        new LRequestTranscriptionRemoval(draftId, transcriptionId);

    internal static LRequest TTranscriptionShiftCreate(long draftId, long transcriptionId, int position) =>
        new LRequestTranscriptionShift(draftId, transcriptionId, position);

    internal static LRequest TTranscriptionSchemeCreate(long draftId, long transcriptionId, string text) =>
        new LRequestTranscriptionScheme(draftId, transcriptionId, text);

    internal static LRequest TTranscriptionTextCreate(long draftId, long transcriptionId, string text) =>
        new LRequestTranscriptionText(draftId, transcriptionId, text);

    internal static LRequest TReflexAdditionCreate(long draftId, string language, string kind, int position) =>
        new LRequestReflexAddition(draftId, language, kind, position);

    internal static LRequest TReflexRemovalCreate(long draftId, long reflexId) =>
        new LRequestReflexRemoval(draftId, reflexId);

    internal static LRequest TReflexTextCreate(long draftId, long reflexId, string text) =>
        new LRequestReflexText(draftId, reflexId, text);

    internal static LRequest TReflexRespellingCreate(long draftId, long reflexId, string text) =>
        new LRequestReflexRespelling(draftId, reflexId, text);

    internal static LRequest TReflexLanguageCreate(long draftId, long reflexId, string text) =>
        new LRequestReflexLanguage(draftId, reflexId, text);

    internal static LRequest TReflexKindCreate(long draftId, long reflexId, string text) =>
        new LRequestReflexKind(draftId, reflexId, text);

    internal static LRequest TReflexMainCreate(long draftId, long reflexId, bool main) =>
        new LRequestReflexMain(draftId, reflexId, main);

    internal static LRequest TReflexRemarkCreate(long draftId, long reflexId, string text) =>
        new LRequestReflexRemark(draftId, reflexId, text);

    internal static LRequest TReflexAnchorCreate(long draftId, long reflexId, long fanqieId, bool anchored) =>
        new LRequestReflexAnchor(draftId, reflexId, fanqieId, anchored);

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
}
