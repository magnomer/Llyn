using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LPronunciationAudio? TEngineAudioRead(this LEngine engine, long pronunciationId) =>
        engine.LEngineAudioRead(pronunciationId);

    internal static void TEngineAudioSave(
        this LEngine engine,
        long pronunciationId,
        string file,
        string? source)
    {
        engine.LEngineAudioSave(pronunciationId, file, source);
    }

    internal static void TEngineAuthorAttach(
        this LEngine engine,
        long referenceId,
        long authorId,
        int position)
    {
        engine.LEngineAuthorAttach(referenceId, authorId, position);
    }

    internal static LAuthor TEngineAuthorCreate(this LEngine engine, LAuthor author) =>
        engine.LEngineAuthorCreate(author);

    internal static void TEngineAuthorDelete(this LEngine engine, long id)
    {
        engine.LEngineAuthorDelete(id);
    }

    internal static void TEngineAuthorDetach(this LEngine engine, long referenceId, long authorId)
    {
        engine.LEngineAuthorDetach(referenceId, authorId);
    }

    internal static LAuthor? TEngineAuthorRead(this LEngine engine, long id) =>
        engine.LEngineAuthorRead(id);

    internal static IReadOnlyList<LAuthor> TEngineAuthorRead(this LEngine engine) =>
        engine.LEngineAuthorRead();

    internal static IReadOnlyList<LAuthor> TEngineAuthorFind(this LEngine engine, string query) =>
        engine.LEngineAuthorFind(query);

    internal static IReadOnlyList<LCatalogAuthor> TEngineAuthorFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineAuthorFind(query, order);

    internal static void TEngineAuthorAbsorb(this LEngine engine, long kept, long dropped)
    {
        engine.LEngineAuthorAbsorb(kept, dropped);
    }

    internal static IReadOnlyList<LCatalogReference> TEngineOeuvreFind(
        this LEngine engine,
        long? author,
        string query,
        LCatalogFilter kind,
        LCatalogOrder order) =>
        engine.LEngineOeuvreFind(author, query, kind, order);

    internal static IReadOnlyList<LAuthor> TEngineAuthorRead(
        this LEngine engine,
        long ownerId,
        LOwner owner) =>
        engine.LEngineAuthorRead(ownerId, owner);

    internal static void TEngineAuthorUpdate(this LEngine engine, LAuthor author)
    {
        engine.LEngineAuthorUpdate(author);
    }

    internal static LCollocation TEngineCollocationCreate(this LEngine engine, LCollocation collocation) =>
        engine.LEngineCollocationCreate(collocation);

    internal static void TEngineCollocationDelete(this LEngine engine, long id)
    {
        engine.LEngineCollocationDelete(id);
    }

    internal static void TEngineCollocationMove(this LEngine engine, long id, int position)
    {
        engine.LEngineCollocationMove(id, position);
    }

    internal static IReadOnlyList<LCollocation> TEngineCollocationRead(
        this LEngine engine,
        long ownerId,
        LOwner owner) =>
        engine.LEngineCollocationRead(ownerId, owner);

    internal static void TEngineCollocationUpdate(this LEngine engine, LCollocation collocation)
    {
        engine.LEngineCollocationUpdate(collocation);
    }

    internal static void TEngineExampleAttach(
        this LEngine engine,
        long ownerId,
        long exampleId,
        int position,
        LOwner owner)
    {
        engine.LEngineExampleAttach(ownerId, exampleId, position, owner);
    }

    internal static LExample TEngineExampleCommit(this LEngine engine, long id) =>
        engine.LEngineExampleCommit(id);

    internal static LDraft TEngineExampleStart(this LEngine engine, string origin, long? exampleId) =>
        engine.LEngineExampleStart(origin, exampleId);

    internal static LExample TEngineExampleCreate(this LEngine engine, LExample example) =>
        engine.LEngineExampleCreate(example);

    internal static void TEngineExampleDelete(this LEngine engine, long id)
    {
        engine.LEngineExampleDelete(id);
    }

    internal static void TEngineExampleDelete(this LEngine engine, long id, bool detach)
    {
        engine.LEngineExampleDelete(id, detach);
    }

    internal static void TEngineExampleDetach(
        this LEngine engine,
        long ownerId,
        long exampleId,
        LOwner owner)
    {
        engine.LEngineExampleDetach(ownerId, exampleId, owner);
    }

    internal static IReadOnlyList<LExample> TEngineExampleRead(this LEngine engine) =>
        engine.LEngineExampleRead();

    internal static LExample? TEngineExampleRead(this LEngine engine, long id) =>
        engine.LEngineExampleRead(id);

    internal static IReadOnlyList<LExample> TEngineExampleRead(
        this LEngine engine,
        long ownerId,
        LOwner owner) =>
        engine.LEngineExampleRead(ownerId, owner);

    internal static void TEngineExampleRemove(
        this LEngine engine,
        long ownerId,
        long exampleId,
        LOwner owner)
    {
        engine.LEngineExampleRemove(ownerId, exampleId, owner);
    }

    internal static void TEngineExampleUpdate(this LEngine engine, LExample example)
    {
        engine.LEngineExampleUpdate(example);
    }

    internal static void TEngineExampleUpdate(this LEngine engine, long exampleId, LStateAnchor reference)
    {
        engine.LEngineExampleUpdate(exampleId, reference);
    }

    internal static IReadOnlyList<LUsage> TEngineIncomingRead(this LEngine engine, long entryId) =>
        engine.LEngineIncomingRead(entryId);

    internal static void TEngineInflectionAppend(
        this LEngine engine,
        long entryId,
        IReadOnlyList<LInflection> inflections)
    {
        engine.LEngineInflectionAppend(entryId, inflections);
    }

    internal static void TEngineInflectionDelete(this LEngine engine, long entryId, int position)
    {
        engine.LEngineInflectionDelete(entryId, position);
    }

    internal static void TEngineInflectionMove(this LEngine engine, long entryId, int position, int target)
    {
        engine.LEngineInflectionMove(entryId, position, target);
    }

    internal static IReadOnlyList<LInflection> TEngineInflectionRead(this LEngine engine, long entryId) =>
        engine.LEngineInflectionRead(entryId);

    internal static void TEngineInflectionSet(
        this LEngine engine,
        long entryId,
        IReadOnlyList<LInflection> inflections)
    {
        engine.LEngineInflectionSet(entryId, inflections);
    }

    internal static LFeature TEngineFeatureCreate(
        this LEngine engine, LFeature feature) =>
        engine.LEngineFeatureCreate(feature);

    internal static LMorphology TEngineMorphologyCreate(
        this LEngine engine, LMorphology value) =>
        engine.LEngineMorphologyCreate(value);

    internal static IReadOnlyList<LFeature> TEngineFeatureRead(
        this LEngine engine, long speechValueId) =>
        engine.LEngineFeatureRead(speechValueId);

    internal static LFeature? TEngineFeatureFind(
        this LEngine engine, long speechValueId, string name) =>
        engine.LEngineFeatureFind(speechValueId, name);

    internal static LMorphology? TEngineMorphologyRead(this LEngine engine, long id) =>
        engine.LEngineMorphologyRead(id);

    internal static IReadOnlyList<LMorphology> TEngineMorphologyScan(
        this LEngine engine, long featureId) =>
        engine.LEngineMorphologyScan(featureId);

    internal static LMorphology? TEngineMorphologyFind(
        this LEngine engine, long featureId, string name) =>
        engine.LEngineMorphologyFind(featureId, name);

    internal static IReadOnlyList<LParadigmSlot> TEngineParadigmRead(this LEngine engine, long entryId) =>
        engine.LEngineParadigmRead(entryId);

    internal static IReadOnlyList<LParadigmSlot> TEngineParadigmShow(this LEngine engine, long entryId) =>
        engine.LEngineParadigmShow(entryId);

    internal static bool TEngineParadigmMatch(LParadigm paradigm, string headword, string form) =>
        LEngine.LEngineParadigmMatch(paradigm, headword, form);

    internal static void TEngineNoteDelete(this LEngine engine, long entryId)
    {
        engine.LEngineNoteDelete(entryId);
    }

    internal static LNote? TEngineNoteRead(this LEngine engine, long entryId) =>
        engine.LEngineNoteRead(entryId);

    internal static void TEngineNoteSave(this LEngine engine, LNote note)
    {
        engine.LEngineNoteSave(note);
    }

    internal static LPronunciation TEnginePronunciationCreate(
        this LEngine engine,
        LPronunciation pronunciation) =>
        engine.LEnginePronunciationCreate(pronunciation);

    internal static void TEnginePronunciationDelete(this LEngine engine, long id)
    {
        engine.LEnginePronunciationDelete(id);
    }

    internal static IReadOnlyList<LPronunciation> TEnginePronunciationRead(this LEngine engine, long entryId) =>
        engine.LEnginePronunciationRead(entryId);

    internal static IReadOnlyList<LTranscription> TEngineTranscriptionRead(this LEngine engine, long entryId) =>
        engine.LEngineTranscriptionRead(entryId);

    internal static IReadOnlyList<LTranscription> TEngineTranscriptionSet(
        this LEngine engine, long entryId, IReadOnlyList<LTranscription> transcriptions) =>
        engine.LEngineTranscriptionSet(entryId, transcriptions);

    internal static IReadOnlyList<string> TEngineSchemeRead(this LEngine engine, string language) =>
        engine.LEngineSchemeRead(language);

    internal static void TEnginePronunciationUpdate(this LEngine engine, LPronunciation pronunciation)
    {
        engine.LEnginePronunciationUpdate(pronunciation);
    }

    internal static void TEngineReferenceAttach(
        this LEngine engine,
        long ownerId,
        long referenceId,
        int position,
        LOwner owner)
    {
        engine.LEngineReferenceAttach(ownerId, referenceId, position, owner);
    }

    internal static LReference TEngineReferenceCommit(this LEngine engine, long id) =>
        engine.LEngineReferenceCommit(id);

    internal static LReference TEngineReferenceCreate(this LEngine engine, LReference reference) =>
        engine.LEngineReferenceCreate(reference);

    internal static LDraft TEngineReferenceStart(this LEngine engine, string origin, long? referenceId) =>
        engine.LEngineReferenceStart(origin, referenceId);

    internal static void TEngineReferenceDelete(this LEngine engine, long id)
    {
        engine.LEngineReferenceDelete(id);
    }

    internal static void TEngineReferenceDetach(
        this LEngine engine,
        long ownerId,
        long referenceId,
        LOwner owner)
    {
        engine.LEngineReferenceDetach(ownerId, referenceId, owner);
    }

    internal static IReadOnlyList<LReference> TEngineReferenceRead(this LEngine engine) =>
        engine.LEngineReferenceRead();

    internal static LReference? TEngineReferenceRead(this LEngine engine, long id) =>
        engine.LEngineReferenceRead(id);

    internal static IReadOnlyList<LReference> TEngineReferenceRead(
        this LEngine engine,
        long ownerId,
        LOwner owner) =>
        engine.LEngineReferenceRead(ownerId, owner);

    internal static LMeaning TEngineMeaningCreate(this LEngine engine, LMeaning meaning) =>
        engine.LEngineMeaningCreate(meaning);

    internal static void TEngineMeaningDelete(this LEngine engine, long id)
    {
        engine.LEngineMeaningDelete(id);
    }

    internal static void TEngineMeaningMove(this LEngine engine, long id, int position)
    {
        engine.LEngineMeaningMove(id, position);
    }

    internal static LMeaning? TEngineMeaningRead(this LEngine engine, long id) =>
        engine.LEngineMeaningRead(id);

    internal static IReadOnlyList<LMeaning> TEngineMeaningRead(
        this LEngine engine,
        long ownerId,
        LOwner owner) =>
        engine.LEngineMeaningRead(ownerId, owner);

    internal static void TEngineMeaningUpdate(this LEngine engine, LMeaning meaning)
    {
        engine.LEngineMeaningUpdate(meaning);
    }

    internal static void TEngineSituationAttach(
        this LEngine engine,
        long ownerId,
        long situationId,
        int position,
        LOwner owner)
    {
        engine.LEngineSituationAttach(ownerId, situationId, position, owner);
    }

    internal static LSituation TEngineSituationCommit(this LEngine engine, long id) =>
        engine.LEngineSituationCommit(id);

    internal static LSituation TEngineSituationCreate(this LEngine engine, LSituation situation) =>
        engine.LEngineSituationCreate(situation);

    internal static LDraft TEngineSituationStart(this LEngine engine, string origin, long? situationId) =>
        engine.LEngineSituationStart(origin, situationId);

    internal static void TEngineSituationDelete(this LEngine engine, long id)
    {
        engine.LEngineSituationDelete(id);
    }

    internal static void TEngineSituationDelete(this LEngine engine, long id, bool detach)
    {
        engine.LEngineSituationDelete(id, detach);
    }

    internal static void TEngineSituationDetach(
        this LEngine engine,
        long ownerId,
        long situationId,
        LOwner owner)
    {
        engine.LEngineSituationDetach(ownerId, situationId, owner);
    }

    internal static IReadOnlyList<LSituation> TEngineSituationRead(this LEngine engine) =>
        engine.LEngineSituationRead();

    internal static LSituation? TEngineSituationRead(this LEngine engine, long id) =>
        engine.LEngineSituationRead(id);

    internal static IReadOnlyList<LSituation> TEngineSituationRead(
        this LEngine engine,
        long ownerId,
        LOwner owner) =>
        engine.LEngineSituationRead(ownerId, owner);

    internal static void TEngineSituationRemove(
        this LEngine engine,
        long ownerId,
        long situationId,
        LOwner owner)
    {
        engine.LEngineSituationRemove(ownerId, situationId, owner);
    }

    internal static void TEngineSituationUpdate(this LEngine engine, LSituation situation)
    {
        engine.LEngineSituationUpdate(situation);
    }

    internal static LSpeechValue? TEngineSpeechAdd(this LEngine engine, string language, string name) =>
        engine.LEngineSpeechAdd(language, name);

    internal static LSpeechValue TEngineSpeechCreate(this LEngine engine, LSpeechValue value) =>
        engine.LEngineSpeechCreate(value);

    internal static LSpeechValue? TEngineSpeechFind(this LEngine engine, string language, string name) =>
        engine.LEngineSpeechFind(language, name);

    internal static IReadOnlyList<LSpeechValue> TEngineSpeechRead(this LEngine engine, string language) =>
        engine.LEngineSpeechRead(language);

    internal static LSpeechValue? TEngineSpeechRead(this LEngine engine, long id) =>
        engine.LEngineSpeechRead(id);

    internal static LTag TEngineTagCreate(this LEngine engine, string text) =>
        engine.LEngineTagCreate(text);

    internal static void TEngineTagChange(this LEngine engine, long tagId, string renamed)
    {
        engine.LEngineTagChange(tagId, renamed);
    }

    internal static void TEngineTagDelete(this LEngine engine, long tagId)
    {
        engine.LEngineTagDelete(tagId);
    }

    internal static IReadOnlyList<LTag> TEngineTagRead(this LEngine engine) =>
        engine.LEngineTagRead();

    internal static IReadOnlyList<LTag> TEngineTagRead(this LEngine engine, long ownerId, LOwner owner) =>
        engine.LEngineTagRead(ownerId, owner);

    internal static void TEngineTagSave(
        this LEngine engine,
        long ownerId,
        IReadOnlyList<LTag> written,
        LOwner owner)
    {
        engine.LEngineTagSave(ownerId, written, owner);
    }

    internal static LEntry TEngineTranslationCreate(this LEngine engine, string headword, string language) =>
        engine.LEngineTranslationCreate(headword, language);

    internal static IReadOnlyList<LEntry> TEngineTranslationFind(
        this LEngine engine,
        string query,
        long? entryId) =>
        engine.LEngineTranslationFind(query, entryId);

    internal static LEntry? TEngineTranslationResolve(this LEngine engine, string word, long? entryId) =>
        engine.LEngineTranslationResolve(word, entryId);

    internal static IReadOnlyDictionary<long, int> TEngineUsageRead(this LEngine engine, LOwner owner) =>
        engine.LEngineUsageRead(owner);

    internal static IReadOnlyList<LUsage> TEngineUsageRead(this LEngine engine, long id, LOwner owner) =>
        engine.LEngineUsageRead(id, owner);
}
