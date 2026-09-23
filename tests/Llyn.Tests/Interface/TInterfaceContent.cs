using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LPronunciationAudio? TEngineAudioRead(this LEngine engine, long pronunciationId) =>
        engine.LEnginePronunciation.LEngineAudioRead(pronunciationId);

    internal static void TEngineAudioSave(
        this LEngine engine,
        long pronunciationId,
        string file,
        string? source)
    {
        engine.LEnginePronunciation.LEngineAudioSave(pronunciationId, file, source);
    }

    internal static LCollocation TEngineCollocationCreate(this LEngine engine, LCollocation collocation) =>
        engine.LEngineCard.LEngineCollocationCreate(collocation);

    internal static void TEngineCollocationDelete(this LEngine engine, long id)
    {
        engine.LEngineCard.LEngineCollocationDelete(id);
    }

    internal static void TEngineCollocationMove(this LEngine engine, long id, int position)
    {
        engine.LEngineCard.LEngineCollocationMove(id, position);
    }

    internal static IReadOnlyList<LCollocation> TEngineCollocationRead(
        this LEngine engine,
        long ownerId,
        LOwner owner) =>
        engine.LEngineCard.LEngineCollocationRead(ownerId, owner);

    internal static void TEngineCollocationUpdate(this LEngine engine, LCollocation collocation)
    {
        engine.LEngineCard.LEngineCollocationUpdate(collocation);
    }

    internal static void TEngineExampleAttach(
        this LEngine engine,
        long ownerId,
        long exampleId,
        int position,
        LOwner owner)
    {
        engine.LEngineExample.LEngineExampleAttach(ownerId, exampleId, position, owner);
    }

    internal static LExample TEngineExampleCommit(this LEngine engine, long id) =>
        engine.LEngineExample.LEngineExampleCommit(id);

    internal static LDraft TEngineExampleStart(this LEngine engine, string origin, long? exampleId) =>
        engine.LEngineExample.LEngineExampleStart(origin, exampleId);

    internal static LExample TEngineExampleCreate(this LEngine engine, LExample example) =>
        engine.LEngineExample.LEngineExampleCreate(example);

    internal static void TEngineExampleDelete(this LEngine engine, long id)
    {
        engine.LEngineExample.LEngineExampleDelete(id);
    }

    internal static void TEngineExampleDelete(this LEngine engine, long id, bool detach)
    {
        engine.LEngineExample.LEngineExampleDelete(id, detach);
    }

    internal static void TEngineExampleDetach(
        this LEngine engine,
        long ownerId,
        long exampleId,
        LOwner owner)
    {
        engine.LEngineExample.LEngineExampleDetach(ownerId, exampleId, owner);
    }

    internal static IReadOnlyList<LExample> TEngineExampleRead(this LEngine engine) =>
        engine.LEngineExample.LEngineExampleRead();

    internal static LExample? TEngineExampleRead(this LEngine engine, long id) =>
        engine.LEngineExample.LEngineExampleRead(id);

    internal static IReadOnlyList<LExample> TEngineExampleRead(
        this LEngine engine,
        long ownerId,
        LOwner owner) =>
        engine.LEngineExample.LEngineExampleRead(ownerId, owner);

    internal static void TEngineExampleRemove(
        this LEngine engine,
        long ownerId,
        long exampleId,
        LOwner owner)
    {
        engine.LEngineExample.LEngineExampleRemove(ownerId, exampleId, owner);
    }

    internal static void TEngineExampleUpdate(this LEngine engine, LExample example)
    {
        engine.LEngineExample.LEngineExampleUpdate(example);
    }

    internal static void TEngineExampleUpdate(this LEngine engine, long exampleId, LStateAnchor reference)
    {
        engine.LEngineExample.LEngineExampleUpdate(exampleId, reference);
    }

    internal static IReadOnlyList<LUsage> TEngineIncomingRead(this LEngine engine, long entryId) =>
        engine.LEngineCard.LEngineIncomingRead(entryId);

    internal static void TEngineInflectionAppend(
        this LEngine engine,
        long entryId,
        IReadOnlyList<LInflection> inflections)
    {
        engine.LEngineVocabulary.LEngineInflectionAppend(entryId, inflections);
    }

    internal static void TEngineInflectionDelete(this LEngine engine, long entryId, int position)
    {
        engine.LEngineVocabulary.LEngineInflectionDelete(entryId, position);
    }

    internal static void TEngineInflectionMove(this LEngine engine, long entryId, int position, int target)
    {
        engine.LEngineVocabulary.LEngineInflectionMove(entryId, position, target);
    }

    internal static IReadOnlyList<LInflection> TEngineInflectionRead(this LEngine engine, long entryId) =>
        engine.LEngineVocabulary.LEngineInflectionRead(entryId);

    internal static void TEngineInflectionSet(
        this LEngine engine,
        long entryId,
        IReadOnlyList<LInflection> inflections)
    {
        engine.LEngineVocabulary.LEngineInflectionSet(entryId, inflections);
    }

    internal static LFeature TEngineFeatureCreate(
        this LEngine engine, LFeature feature) =>
        engine.LEngineVocabulary.LEngineFeatureCreate(feature);

    internal static LMorphology TEngineMorphologyCreate(
        this LEngine engine, LMorphology value) =>
        engine.LEngineVocabulary.LEngineMorphologyCreate(value);

    internal static IReadOnlyList<LFeature> TEngineFeatureRead(
        this LEngine engine, long speechValueId) =>
        engine.LEngineVocabulary.LEngineFeatureRead(speechValueId);

    internal static LFeature? TEngineFeatureFind(
        this LEngine engine, long speechValueId, string name) =>
        engine.LEngineVocabulary.LEngineFeatureFind(speechValueId, name);

    internal static LMorphology? TEngineMorphologyRead(this LEngine engine, long id) =>
        engine.LEngineVocabulary.LEngineMorphologyRead(id);

    internal static IReadOnlyList<LMorphology> TEngineMorphologyScan(
        this LEngine engine, long featureId) =>
        engine.LEngineVocabulary.LEngineMorphologyScan(featureId);

    internal static LMorphology? TEngineMorphologyFind(
        this LEngine engine, long featureId, string name) =>
        engine.LEngineVocabulary.LEngineMorphologyFind(featureId, name);

    internal static IReadOnlyList<LParadigmSlot> TEngineParadigmRead(this LEngine engine, long entryId) =>
        engine.LEngineVocabulary.LEngineParadigmRead(entryId);

    internal static IReadOnlyList<LParadigmSlot> TEngineParadigmShow(this LEngine engine, long entryId) =>
        engine.LEngineVocabulary.LEngineParadigmShow(entryId);

    internal static bool TEngineParadigmMatch(LParadigm paradigm, string headword, string form) =>
        LVocabularyFacade.LEngineParadigmMatch(paradigm, headword, form);

    internal static void TEngineNoteDelete(this LEngine engine, long entryId)
    {
        engine.LEnginePronunciation.LEngineNoteDelete(entryId);
    }

    internal static LNote? TEngineNoteRead(this LEngine engine, long entryId) =>
        engine.LEnginePronunciation.LEngineNoteRead(entryId);

    internal static void TEngineNoteSave(this LEngine engine, LNote note)
    {
        engine.LEnginePronunciation.LEngineNoteSave(note);
    }

    internal static LPronunciation TEnginePronunciationCreate(
        this LEngine engine,
        LPronunciation pronunciation) =>
        engine.LEnginePronunciation.LEnginePronunciationCreate(pronunciation);

    internal static void TEnginePronunciationDelete(this LEngine engine, long id)
    {
        engine.LEnginePronunciation.LEnginePronunciationDelete(id);
    }

    internal static IReadOnlyList<LPronunciation> TEnginePronunciationRead(this LEngine engine, long entryId) =>
        engine.LEnginePronunciation.LEnginePronunciationRead(entryId);

    internal static IReadOnlyList<LTranscription> TEngineTranscriptionRead(this LEngine engine, long entryId) =>
        engine.LEnginePronunciation.LEngineTranscriptionRead(entryId);

    internal static IReadOnlyList<LTranscription> TEngineTranscriptionSet(
        this LEngine engine, long entryId, IReadOnlyList<LTranscription> transcriptions) =>
        engine.LEnginePronunciation.LEngineTranscriptionSet(entryId, transcriptions);

    internal static IReadOnlyList<string> TEngineSchemeRead(this LEngine engine, string language) =>
        engine.LEnginePronunciation.LEngineSchemeRead(language);

    internal static LGlyph? TEngineGlyphRead(this LEngine engine, string language) =>
        engine.LEngineEntry.LEngineGlyphRead(language);

    internal static LEntry TEngineGlyphResolve(this LEngine engine, string character, string language) =>
        engine.LEngineEntry.LEngineGlyphResolve(character, language);

    internal static void TEnginePronunciationUpdate(this LEngine engine, LPronunciation pronunciation)
    {
        engine.LEnginePronunciation.LEnginePronunciationUpdate(pronunciation);
    }

    internal static LMeaning TEngineMeaningCreate(this LEngine engine, LMeaning meaning) =>
        engine.LEngineCard.LEngineMeaningCreate(meaning);

    internal static void TEngineMeaningDelete(this LEngine engine, long id)
    {
        engine.LEngineCard.LEngineMeaningDelete(id);
    }

    internal static void TEngineMeaningMove(this LEngine engine, long id, int position)
    {
        engine.LEngineCard.LEngineMeaningMove(id, position);
    }

    internal static LMeaning? TEngineMeaningRead(this LEngine engine, long id) =>
        engine.LEngineCard.LEngineMeaningRead(id);

    internal static IReadOnlyList<LMentionLabel> TEngineMentionResolve(
        this LEngine engine, string text, IReadOnlyList<LMentionDraft> mentions) =>
        engine.LEngineMention.LEngineMentionResolve(text, mentions);

    internal static IReadOnlyList<LMeaning> TEngineMeaningRead(
        this LEngine engine,
        long ownerId,
        LOwner owner) =>
        engine.LEngineCard.LEngineMeaningRead(ownerId, owner);

    internal static void TEngineMeaningUpdate(this LEngine engine, LMeaning meaning)
    {
        engine.LEngineCard.LEngineMeaningUpdate(meaning);
    }

    internal static void TEngineSituationAttach(
        this LEngine engine,
        long ownerId,
        long situationId,
        int position,
        LOwner owner)
    {
        engine.LEngineSituation.LEngineSituationAttach(ownerId, situationId, position, owner);
    }

    internal static LSituation TEngineSituationCommit(this LEngine engine, long id) =>
        engine.LEngineSituation.LEngineSituationCommit(id);

    internal static LSituation TEngineSituationCreate(this LEngine engine, LSituation situation) =>
        engine.LEngineSituation.LEngineSituationCreate(situation);

    internal static LDraft TEngineSituationStart(this LEngine engine, string origin, long? situationId) =>
        engine.LEngineSituation.LEngineSituationStart(origin, situationId);

    internal static void TEngineSituationDelete(this LEngine engine, long id)
    {
        engine.LEngineSituation.LEngineSituationDelete(id);
    }

    internal static void TEngineSituationDelete(this LEngine engine, long id, bool detach)
    {
        engine.LEngineSituation.LEngineSituationDelete(id, detach);
    }

    internal static void TEngineSituationDetach(
        this LEngine engine,
        long ownerId,
        long situationId,
        LOwner owner)
    {
        engine.LEngineSituation.LEngineSituationDetach(ownerId, situationId, owner);
    }

    internal static IReadOnlyList<LSituation> TEngineSituationRead(this LEngine engine) =>
        engine.LEngineSituation.LEngineSituationRead();

    internal static LSituation? TEngineSituationRead(this LEngine engine, long id) =>
        engine.LEngineSituation.LEngineSituationRead(id);

    internal static IReadOnlyList<LSituation> TEngineSituationRead(
        this LEngine engine,
        long ownerId,
        LOwner owner) =>
        engine.LEngineSituation.LEngineSituationRead(ownerId, owner);

    internal static void TEngineSituationRemove(
        this LEngine engine,
        long ownerId,
        long situationId,
        LOwner owner)
    {
        engine.LEngineSituation.LEngineSituationRemove(ownerId, situationId, owner);
    }

    internal static void TEngineSituationUpdate(this LEngine engine, LSituation situation)
    {
        engine.LEngineSituation.LEngineSituationUpdate(situation);
    }

    internal static LSpeechValue? TEngineSpeechAdd(this LEngine engine, string language, string name) =>
        engine.LEngineVocabulary.LEngineSpeechAdd(language, name);

    internal static LSpeechValue TEngineSpeechCreate(this LEngine engine, LSpeechValue value) =>
        engine.LEngineVocabulary.LEngineSpeechCreate(value);

    internal static LSpeechValue? TEngineSpeechFind(this LEngine engine, string language, string name) =>
        engine.LEngineVocabulary.LEngineSpeechFind(language, name);

    internal static IReadOnlyList<LSpeechValue> TEngineSpeechRead(this LEngine engine, string language) =>
        engine.LEngineVocabulary.LEngineSpeechRead(language);

    internal static LSpeechValue? TEngineSpeechRead(this LEngine engine, long id) =>
        engine.LEngineVocabulary.LEngineSpeechRead(id);

    internal static LTag TEngineTagCreate(this LEngine engine, string text) =>
        engine.LEngineCard.LEngineTagCreate(text);

    internal static void TEngineTagChange(this LEngine engine, long tagId, string renamed)
    {
        engine.LEngineCard.LEngineTagChange(tagId, renamed);
    }

    internal static void TEngineTagDelete(this LEngine engine, long tagId)
    {
        engine.LEngineCard.LEngineTagDelete(tagId);
    }

    internal static IReadOnlyList<LTag> TEngineTagRead(this LEngine engine) =>
        engine.LEngineCard.LEngineTagRead();

    internal static IReadOnlyList<LTag> TEngineTagRead(this LEngine engine, long ownerId, LOwner owner) =>
        engine.LEngineCard.LEngineTagRead(ownerId, owner);

    internal static void TEngineTagSave(
        this LEngine engine,
        long ownerId,
        IReadOnlyList<LTag> written,
        LOwner owner)
    {
        engine.LEngineCard.LEngineTagSave(ownerId, written, owner);
    }

    internal static LEntry TEngineTranslationCreate(this LEngine engine, string headword, string language) =>
        engine.LEngineCard.LEngineTranslationCreate(headword, language);

    internal static IReadOnlyList<LEntry> TEngineTranslationFind(
        this LEngine engine,
        string query,
        long? entryId) =>
        engine.LEngineCard.LEngineTranslationFind(query, entryId);

    internal static LEntry? TEngineTranslationResolve(this LEngine engine, string word, long? entryId) =>
        engine.LEngineCard.LEngineTranslationResolve(word, entryId);

    internal static IReadOnlyDictionary<long, int> TEngineUsageRead(this LEngine engine, LOwner owner) =>
        engine.LEngineEntry.LEngineUsageRead(owner);

    internal static IReadOnlyList<LUsage> TEngineUsageRead(this LEngine engine, long id, LOwner owner) =>
        engine.LEngineEntry.LEngineUsageRead(id, owner);
}
