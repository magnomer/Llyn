using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LPronunciationAudio? TEngineAudioRead(this LEngine engine, string pronunciationId) =>
        engine.LEngineAudioRead(pronunciationId);

    internal static void TEngineAudioSave(
        this LEngine engine,
        string pronunciationId,
        string file,
        string? source)
    {
        engine.LEngineAudioSave(pronunciationId, file, source);
    }

    internal static void TEngineAuthorAttach(
        this LEngine engine,
        string referenceId,
        string authorId,
        int position)
    {
        engine.LEngineAuthorAttach(referenceId, authorId, position);
    }

    internal static LAuthor TEngineAuthorCreate(this LEngine engine, LAuthor author) =>
        engine.LEngineAuthorCreate(author);

    internal static void TEngineAuthorDelete(this LEngine engine, string id)
    {
        engine.LEngineAuthorDelete(id);
    }

    internal static void TEngineAuthorDetach(this LEngine engine, string referenceId, string authorId)
    {
        engine.LEngineAuthorDetach(referenceId, authorId);
    }

    internal static LAuthor? TEngineAuthorRead(this LEngine engine, string id) =>
        engine.LEngineAuthorRead(id);

    internal static IReadOnlyList<LAuthor> TEngineAuthorRead(
        this LEngine engine,
        string ownerId,
        LOwner owner) =>
        engine.LEngineAuthorRead(ownerId, owner);

    internal static void TEngineAuthorUpdate(this LEngine engine, LAuthor author)
    {
        engine.LEngineAuthorUpdate(author);
    }

    internal static LCollocation TEngineCollocationCreate(this LEngine engine, LCollocation collocation) =>
        engine.LEngineCollocationCreate(collocation);

    internal static void TEngineCollocationDelete(this LEngine engine, string id)
    {
        engine.LEngineCollocationDelete(id);
    }

    internal static void TEngineCollocationMove(this LEngine engine, string id, int position)
    {
        engine.LEngineCollocationMove(id, position);
    }

    internal static IReadOnlyList<LCollocation> TEngineCollocationRead(
        this LEngine engine,
        string ownerId,
        LOwner owner) =>
        engine.LEngineCollocationRead(ownerId, owner);

    internal static void TEngineCollocationUpdate(this LEngine engine, LCollocation collocation)
    {
        engine.LEngineCollocationUpdate(collocation);
    }

    internal static void TEngineExampleAttach(
        this LEngine engine,
        string ownerId,
        string exampleId,
        int position,
        LOwner owner)
    {
        engine.LEngineExampleAttach(ownerId, exampleId, position, owner);
    }

    internal static LExample TEngineExampleCreate(this LEngine engine, LExample example) =>
        engine.LEngineExampleCreate(example);

    internal static void TEngineExampleDelete(this LEngine engine, string id)
    {
        engine.LEngineExampleDelete(id);
    }

    internal static void TEngineExampleDelete(this LEngine engine, string id, bool detach)
    {
        engine.LEngineExampleDelete(id, detach);
    }

    internal static void TEngineExampleDetach(
        this LEngine engine,
        string ownerId,
        string exampleId,
        LOwner owner)
    {
        engine.LEngineExampleDetach(ownerId, exampleId, owner);
    }

    internal static IReadOnlyList<LExample> TEngineExampleRead(this LEngine engine) =>
        engine.LEngineExampleRead();

    internal static LExample? TEngineExampleRead(this LEngine engine, string id) =>
        engine.LEngineExampleRead(id);

    internal static IReadOnlyList<LExample> TEngineExampleRead(
        this LEngine engine,
        string ownerId,
        LOwner owner) =>
        engine.LEngineExampleRead(ownerId, owner);

    internal static void TEngineExampleRemove(
        this LEngine engine,
        string ownerId,
        string exampleId,
        LOwner owner)
    {
        engine.LEngineExampleRemove(ownerId, exampleId, owner);
    }

    internal static void TEngineExampleUpdate(this LEngine engine, LExample example)
    {
        engine.LEngineExampleUpdate(example);
    }

    internal static void TEngineExampleUpdate(this LEngine engine, string exampleId, LStateValue reference)
    {
        engine.LEngineExampleUpdate(exampleId, reference);
    }

    internal static IReadOnlyList<LUsage> TEngineIncomingRead(this LEngine engine, string entryId) =>
        engine.LEngineIncomingRead(entryId);

    internal static void TEngineInflectionAppend(
        this LEngine engine,
        string entryId,
        IReadOnlyList<LInflection> inflections)
    {
        engine.LEngineInflectionAppend(entryId, inflections);
    }

    internal static void TEngineInflectionDelete(this LEngine engine, string entryId, int position)
    {
        engine.LEngineInflectionDelete(entryId, position);
    }

    internal static void TEngineInflectionMove(this LEngine engine, string entryId, int position, int target)
    {
        engine.LEngineInflectionMove(entryId, position, target);
    }

    internal static IReadOnlyList<LInflection> TEngineInflectionRead(this LEngine engine, string entryId) =>
        engine.LEngineInflectionRead(entryId);

    internal static void TEngineInflectionSet(
        this LEngine engine,
        string entryId,
        IReadOnlyList<LInflection> inflections)
    {
        engine.LEngineInflectionSet(entryId, inflections);
    }

    internal static void TEngineMorphologyCreate(this LEngine engine, LMorphology morphology)
    {
        engine.LEngineMorphologyCreate(morphology);
    }

    internal static LMorphology? TEngineMorphologyRead(
        this LEngine engine,
        string language,
        string speechId,
        string featureId,
        string valueId) =>
        engine.LEngineMorphologyRead(language, speechId, featureId, valueId);

    internal static void TEngineNoteDelete(this LEngine engine, string entryId)
    {
        engine.LEngineNoteDelete(entryId);
    }

    internal static LNote? TEngineNoteRead(this LEngine engine, string entryId) =>
        engine.LEngineNoteRead(entryId);

    internal static void TEngineNoteSave(this LEngine engine, LNote note)
    {
        engine.LEngineNoteSave(note);
    }

    internal static LPronunciation TEnginePronunciationCreate(
        this LEngine engine,
        LPronunciation pronunciation) =>
        engine.LEnginePronunciationCreate(pronunciation);

    internal static void TEnginePronunciationDelete(this LEngine engine, string id)
    {
        engine.LEnginePronunciationDelete(id);
    }

    internal static LPronunciation? TEnginePronunciationRead(this LEngine engine, string entryId) =>
        engine.LEnginePronunciationRead(entryId);

    internal static void TEnginePronunciationUpdate(this LEngine engine, LPronunciation pronunciation)
    {
        engine.LEnginePronunciationUpdate(pronunciation);
    }

    internal static void TEngineReferenceAttach(
        this LEngine engine,
        string ownerId,
        string referenceId,
        int position,
        LOwner owner)
    {
        engine.LEngineReferenceAttach(ownerId, referenceId, position, owner);
    }

    internal static LReference TEngineReferenceCreate(this LEngine engine, LReference reference) =>
        engine.LEngineReferenceCreate(reference);

    internal static void TEngineReferenceDelete(this LEngine engine, string id)
    {
        engine.LEngineReferenceDelete(id);
    }

    internal static void TEngineReferenceDetach(
        this LEngine engine,
        string ownerId,
        string referenceId,
        LOwner owner)
    {
        engine.LEngineReferenceDetach(ownerId, referenceId, owner);
    }

    internal static IReadOnlyList<LReference> TEngineReferenceRead(this LEngine engine) =>
        engine.LEngineReferenceRead();

    internal static LReference? TEngineReferenceRead(this LEngine engine, string id) =>
        engine.LEngineReferenceRead(id);

    internal static IReadOnlyList<LReference> TEngineReferenceRead(
        this LEngine engine,
        string ownerId,
        LOwner owner) =>
        engine.LEngineReferenceRead(ownerId, owner);

    internal static LRelation TEngineRelationCreate(this LEngine engine, LRelation relation) =>
        engine.LEngineRelationCreate(relation);

    internal static void TEngineRelationDelete(this LEngine engine, string id)
    {
        engine.LEngineRelationDelete(id);
    }

    internal static void TEngineRelationMove(this LEngine engine, string id, int position)
    {
        engine.LEngineRelationMove(id, position);
    }

    internal static IReadOnlyList<LRelation> TEngineRelationRead(this LEngine engine, string meaningId) =>
        engine.LEngineRelationRead(meaningId);

    internal static void TEngineRelationUpdate(this LEngine engine, LRelation relation)
    {
        engine.LEngineRelationUpdate(relation);
    }

    internal static LMeaning TEngineMeaningCreate(this LEngine engine, LMeaning meaning) =>
        engine.LEngineMeaningCreate(meaning);

    internal static void TEngineMeaningDelete(this LEngine engine, string id)
    {
        engine.LEngineMeaningDelete(id);
    }

    internal static IReadOnlyList<LMeaning> TEngineMeaningFind(this LEngine engine, string query) =>
        engine.LEngineMeaningFind(query);

    internal static void TEngineMeaningMove(this LEngine engine, string id, int position)
    {
        engine.LEngineMeaningMove(id, position);
    }

    internal static LMeaning? TEngineMeaningRead(this LEngine engine, string id) =>
        engine.LEngineMeaningRead(id);

    internal static IReadOnlyList<LMeaning> TEngineMeaningRead(
        this LEngine engine,
        string ownerId,
        LOwner owner) =>
        engine.LEngineMeaningRead(ownerId, owner);

    internal static void TEngineMeaningUpdate(this LEngine engine, LMeaning meaning)
    {
        engine.LEngineMeaningUpdate(meaning);
    }

    internal static void TEngineSituationAttach(
        this LEngine engine,
        string ownerId,
        string situationId,
        int position,
        LOwner owner)
    {
        engine.LEngineSituationAttach(ownerId, situationId, position, owner);
    }

    internal static LSituation TEngineSituationCreate(this LEngine engine, LSituation situation) =>
        engine.LEngineSituationCreate(situation);

    internal static void TEngineSituationDelete(this LEngine engine, string id)
    {
        engine.LEngineSituationDelete(id);
    }

    internal static void TEngineSituationDelete(this LEngine engine, string id, bool detach)
    {
        engine.LEngineSituationDelete(id, detach);
    }

    internal static void TEngineSituationDetach(
        this LEngine engine,
        string ownerId,
        string situationId,
        LOwner owner)
    {
        engine.LEngineSituationDetach(ownerId, situationId, owner);
    }

    internal static IReadOnlyList<LSituation> TEngineSituationRead(this LEngine engine) =>
        engine.LEngineSituationRead();

    internal static LSituation? TEngineSituationRead(this LEngine engine, string id) =>
        engine.LEngineSituationRead(id);

    internal static IReadOnlyList<LSituation> TEngineSituationRead(
        this LEngine engine,
        string ownerId,
        LOwner owner) =>
        engine.LEngineSituationRead(ownerId, owner);

    internal static void TEngineSituationRemove(
        this LEngine engine,
        string ownerId,
        string situationId,
        LOwner owner)
    {
        engine.LEngineSituationRemove(ownerId, situationId, owner);
    }

    internal static void TEngineSituationUpdate(this LEngine engine, LSituation situation)
    {
        engine.LEngineSituationUpdate(situation);
    }

    internal static void TEngineSpeechCreate(this LEngine engine, LSpeechValue value)
    {
        engine.LEngineSpeechCreate(value);
    }

    internal static string? TEngineSpeechFind(this LEngine engine, string language, string name) =>
        engine.LEngineSpeechFind(language, name);

    internal static IReadOnlyList<LSpeechValue> TEngineSpeechRead(this LEngine engine, string language) =>
        engine.LEngineSpeechRead(language);

    internal static string? TEngineSpeechRead(this LEngine engine, string language, string valueId) =>
        engine.LEngineSpeechRead(language, valueId);

    internal static LSynonym TEngineSynonymCreate(this LEngine engine, LSynonym synonym) =>
        engine.LEngineSynonymCreate(synonym);

    internal static void TEngineSynonymDelete(this LEngine engine, string id)
    {
        engine.LEngineSynonymDelete(id);
    }

    internal static void TEngineSynonymMove(this LEngine engine, string id, int position)
    {
        engine.LEngineSynonymMove(id, position);
    }

    internal static IReadOnlyList<LSynonym> TEngineSynonymRead(this LEngine engine, string collocationId) =>
        engine.LEngineSynonymRead(collocationId);

    internal static void TEngineSynonymUpdate(this LEngine engine, LSynonym synonym)
    {
        engine.LEngineSynonymUpdate(synonym);
    }

    internal static void TEngineTagChange(this LEngine engine, string text, string renamed)
    {
        engine.LEngineTagChange(text, renamed);
    }

    internal static void TEngineTagDelete(this LEngine engine, string text)
    {
        engine.LEngineTagDelete(text);
    }

    internal static IReadOnlyList<LTag> TEngineTagRead(this LEngine engine) =>
        engine.LEngineTagRead();

    internal static IReadOnlyList<LTag> TEngineTagRead(this LEngine engine, string ownerId, LOwner owner) =>
        engine.LEngineTagRead(ownerId, owner);

    internal static void TEngineTagSave(
        this LEngine engine,
        string ownerId,
        IReadOnlyList<LTag> written,
        LOwner owner)
    {
        engine.LEngineTagSave(ownerId, written, owner);
    }

    internal static LEntry TEngineTranslationCreate(this LEngine engine, string headword, string language) =>
        engine.LEngineTranslationCreate(headword, language);

    internal static void TEngineTranslationDelete(this LEngine engine, string id)
    {
        engine.LEngineTranslationDelete(id);
    }

    internal static IReadOnlyList<LEntry> TEngineTranslationFind(
        this LEngine engine,
        string query,
        string? entryId) =>
        engine.LEngineTranslationFind(query, entryId);

    internal static LEntry? TEngineTranslationResolve(this LEngine engine, string word, string? entryId) =>
        engine.LEngineTranslationResolve(word, entryId);

    internal static IReadOnlyDictionary<string, int> TEngineUsageRead(this LEngine engine, LOwner owner) =>
        engine.LEngineUsageRead(owner);

    internal static IReadOnlyList<LUsage> TEngineUsageRead(this LEngine engine, string id, LOwner owner) =>
        engine.LEngineUsageRead(id, owner);
}
