using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static bool TClaimArchiveCheck(string root, string draftId) =>
        LClaimArchive.LClaimArchiveCheck(root, draftId);

    internal static void TClaimArchiveDelete(string root, string draftId)
    {
        LClaimArchive.LClaimArchiveDelete(root, draftId);
    }

    internal static LClaim? TClaimArchiveRead(string root, string draftId) =>
        LClaimArchive.LClaimArchiveRead(root, draftId);

    internal static void TClaimArchiveSave(string root, LClaim claim)
    {
        LClaimArchive.LClaimArchiveSave(root, claim);
    }

    internal static IReadOnlyList<LClaim> TClaimArchiveScan(string root) =>
        LClaimArchive.LClaimArchiveScan(root);

    internal static LClaim TClaimCreate(string draftId, int process, DateTimeOffset moment) =>
        new(draftId, process, moment);

    internal static LCollocationArchive TCollocationArchiveCreate(LDatabase database) =>
        new(database);

    internal static LCollocation TCollocationCreate(
        this LCollocationArchive collocationArchive,
        LCollocation collocation) =>
        collocationArchive.LCollocationCreate(collocation);

    internal static IReadOnlyList<LCollocation> TCollocationRead(
        this LCollocationArchive collocationArchive,
        string entryId) =>
        collocationArchive.LCollocationRead(entryId);

    internal static void TCollocationDelete(this LCollocationArchive collocationArchive, string id)
    {
        collocationArchive.LCollocationDelete(id);
    }

    internal static LCourtLink? TCourtArchiveRead(string root, string id) =>
        LCourtArchive.LCourtArchiveRead(root, id);

    internal static void TCourtArchiveSave(string root, LCourtLink link)
    {
        LCourtArchive.LCourtArchiveSave(root, link);
    }

    internal static IReadOnlyList<LCourtLink> TCourtArchiveScan(string root) =>
        LCourtArchive.LCourtArchiveScan(root);

    internal static IReadOnlyList<LCourtLink> TCourtArchiveSettle(string root, string draftId) =>
        LCourtArchive.LCourtArchiveSettle(root, draftId);

    internal static void TCourtArchiveSweep(string root)
    {
        LCourtArchive.LCourtArchiveSweep(root);
    }

    internal static void TDatabaseCreate(this LDatabase database)
    {
        database.LDatabaseCreate();
    }

    internal static void TDatabaseSessionCommit(this LDatabaseSession databaseSession)
    {
        databaseSession.LDatabaseSessionCommit();
    }

    internal static LDatabaseSession TDatabaseSessionStart(this LDatabase database) =>
        database.LDatabaseSessionStart();

    internal static LDoctorRescue TDoctorDatabaseCreate(LDatabase database) =>
        LDoctor.LDoctorDatabaseCreate(database);

    internal static void TDraftArchiveDelete(string root, string id)
    {
        LDraftArchive.LDraftArchiveDelete(root, id);
    }

    internal static LDraft? TDraftArchiveRead(string root, string id) =>
        LDraftArchive.LDraftArchiveRead(root, id);

    internal static void TDraftArchiveSave(string root, LDraft draft)
    {
        LDraftArchive.LDraftArchiveSave(root, draft);
    }

    internal static IReadOnlyList<LDraft> TDraftArchiveScan(string root) =>
        LDraftArchive.LDraftArchiveScan(root);

    internal static void TDraftArchiveSweep(string root)
    {
        LDraftArchive.LDraftArchiveSweep(root);
    }

    internal static LEntryArchive TEntryArchiveCreate(LDatabase database) =>
        new(database);

    internal static LEntry TEntryCreate(
        this LEntryArchive entryArchive,
        LEntry entry,
        IReadOnlyList<LForm> forms,
        IReadOnlyList<LSpeech> speeches) =>
        entryArchive.LEntryCreate(entry, forms, speeches);

    internal static void TEntryDelete(this LEntryArchive entryArchive, string id)
    {
        entryArchive.LEntryDelete(id);
    }

    internal static IReadOnlyList<LEntry> TEntryFind(this LEntryArchive entryArchive, string query) =>
        entryArchive.LEntryFind(query);

    internal static IReadOnlyList<LForm> TEntryFormRead(this LEntryArchive entryArchive, string id) =>
        entryArchive.LEntryFormRead(id);

    internal static LEntry? TEntryRead(this LEntryArchive entryArchive, string id) =>
        entryArchive.LEntryRead(id);

    internal static IReadOnlyList<LSpeech> TEntrySpeechRead(this LEntryArchive entryArchive, string id) =>
        entryArchive.LEntrySpeechRead(id);

    internal static void TEntryUpdate(this LEntryArchive entryArchive, LEntry entry)
    {
        entryArchive.LEntryUpdate(entry);
    }

    internal static LExampleArchive TExampleArchiveCreate(LDatabase database) =>
        new(database);

    internal static LExample TExampleCreate(this LExampleArchive exampleArchive, LExample example) =>
        exampleArchive.LExampleCreate(example);

    internal static IReadOnlyList<LExample> TExampleRead(this LExampleArchive exampleArchive) =>
        exampleArchive.LExampleRead();

    internal static LExample? TExampleRead(this LExampleArchive exampleArchive, string id) =>
        exampleArchive.LExampleRead(id);

    internal static void TExampleDelete(this LExampleArchive exampleArchive, string id, bool detach)
    {
        exampleArchive.LExampleDelete(id, detach);
    }

    internal static LSentenceArchive TSentenceArchiveCreate(LDatabase database) =>
        new(database);

    internal static void TSentenceMeaningAttach(
        this LSentenceArchive sentenceArchive,
        string meaningId,
        string exampleId,
        int position)
    {
        sentenceArchive.LSentenceMeaningAttach(meaningId, exampleId, position);
    }

    internal static void TSentenceMeaningDetach(
        this LSentenceArchive sentenceArchive,
        string meaningId,
        string exampleId)
    {
        sentenceArchive.LSentenceMeaningDetach(meaningId, exampleId);
    }

    internal static void TSentenceMeaningSave(
        this LSentenceArchive sentenceArchive,
        string meaningId,
        IReadOnlyList<LSentence> sentences)
    {
        sentenceArchive.LSentenceMeaningSave(meaningId, sentences);
    }

    internal static IReadOnlyList<LSentence> TSentenceMeaningRead(
        this LSentenceArchive sentenceArchive,
        string meaningId) =>
        sentenceArchive.LSentenceMeaningRead(meaningId);

    internal static void TSentenceCollocationAttach(
        this LSentenceArchive sentenceArchive,
        string collocationId,
        string exampleId,
        int position)
    {
        sentenceArchive.LSentenceCollocationAttach(collocationId, exampleId, position);
    }

    internal static void TSentenceCollocationDetach(
        this LSentenceArchive sentenceArchive,
        string collocationId,
        string exampleId)
    {
        sentenceArchive.LSentenceCollocationDetach(collocationId, exampleId);
    }

    internal static void TSentenceCollocationSave(
        this LSentenceArchive sentenceArchive,
        string collocationId,
        IReadOnlyList<LSentence> sentences)
    {
        sentenceArchive.LSentenceCollocationSave(collocationId, sentences);
    }

    internal static IReadOnlyList<LSentence> TSentenceCollocationRead(
        this LSentenceArchive sentenceArchive,
        string collocationId) =>
        sentenceArchive.LSentenceCollocationRead(collocationId);

    internal static void TExampleUpdate(this LExampleArchive exampleArchive, LExample example)
    {
        exampleArchive.LExampleUpdate(example);
    }

    internal static string TIdentityCreate() =>
        LIdentity.LIdentityCreate();

    internal static void TInflectionAppend(
        this LInflectionArchive inflectionArchive,
        string entryId,
        IReadOnlyList<LInflection> inflections)
    {
        inflectionArchive.LInflectionAppend(entryId, inflections);
    }

    internal static LInflectionArchive TInflectionArchiveCreate(LDatabase database) =>
        new(database);

    internal static void TInflectionDelete(
        this LInflectionArchive inflectionArchive,
        string entryId,
        int position)
    {
        inflectionArchive.LInflectionDelete(entryId, position);
    }

    internal static IReadOnlyList<LInflection> TInflectionRead(
        this LInflectionArchive inflectionArchive,
        string entryId) =>
        inflectionArchive.LInflectionRead(entryId);

    internal static LNoteArchive TNoteArchiveCreate(LDatabase database) =>
        new(database);

    internal static LNote? TNoteRead(this LNoteArchive noteArchive, string entryId) =>
        noteArchive.LNoteRead(entryId);

    internal static void TNoteSave(this LNoteArchive noteArchive, LNote note)
    {
        noteArchive.LNoteSave(note);
    }

    internal static LPronunciationArchive TPronunciationArchiveCreate(LDatabase database) =>
        new(database);

    internal static LPronunciationAudio? TPronunciationAudioRead(
        this LPronunciationArchive pronunciationArchive,
        string pronunciationId) =>
        pronunciationArchive.LPronunciationAudioRead(pronunciationId);

    internal static LPronunciation? TPronunciationRead(
        this LPronunciationArchive pronunciationArchive,
        string entryId) =>
        pronunciationArchive.LPronunciationRead(entryId);

    internal static LRelationArchive TRelationArchiveCreate(LDatabase database) =>
        new(database);

    internal static LRelation TRelationCreate(this LRelationArchive relationArchive, LRelation relation) =>
        relationArchive.LRelationCreate(relation);

    internal static LRevisionArchive TRevisionArchiveCreate(LDatabase database) =>
        new(database);

    internal static IReadOnlyList<LRevisionChange> TRevisionChangeRead(
        this LRevisionArchive revisionArchive,
        string revisionId) =>
        revisionArchive.LRevisionChangeRead(revisionId);

    internal static LRevision? TRevisionLatestRead(this LRevisionArchive revisionArchive) =>
        revisionArchive.LRevisionLatestRead();

    internal static LMeaningArchive TMeaningArchiveCreate(LDatabase database) =>
        new(database);

    internal static LMeaning TMeaningCreate(this LMeaningArchive meaningArchive, LMeaning meaning) =>
        meaningArchive.LMeaningCreate(meaning);

    internal static void TMeaningDelete(this LMeaningArchive meaningArchive, string id)
    {
        meaningArchive.LMeaningDelete(id);
    }

    internal static void TMeaningMove(this LMeaningArchive meaningArchive, string id, int position)
    {
        meaningArchive.LMeaningMove(id, position);
    }

    internal static IReadOnlyList<LMeaning> TMeaningRead(this LMeaningArchive meaningArchive, string entryId) =>
        meaningArchive.LMeaningRead(entryId);

    internal static LSituationArchive TSituationArchiveCreate(LDatabase database) =>
        new(database);

    internal static IReadOnlyList<LSituation> TSituationCollocationRead(
        this LSituationArchive situationArchive,
        string collocationId) =>
        situationArchive.LSituationCollocationRead(collocationId);

    internal static IReadOnlyList<LSituation> TSituationMeaningRead(
        this LSituationArchive situationArchive,
        string meaningId) =>
        situationArchive.LSituationMeaningRead(meaningId);

    internal static LTagArchive TTagArchiveCreate(LDatabase database) =>
        new(database);

    internal static IReadOnlyList<LTag> TTagCollocationRead(
        this LTagArchive tagArchive,
        string collocationId) =>
        tagArchive.LTagCollocationRead(collocationId);

    internal static IReadOnlyList<LTag> TTagMeaningRead(this LTagArchive tagArchive, string meaningId) =>
        tagArchive.LTagMeaningRead(meaningId);

    internal static void TTagMeaningSave(this LTagArchive tagArchive, string meaningId, IReadOnlyList<LTag> tags)
    {
        tagArchive.LTagMeaningSave(meaningId, tags);
    }

    internal static LTranslationArchive TTranslationArchiveCreate(LDatabase database) =>
        new(database);

    internal static IReadOnlyList<LTranslation> TTranslationCollocationRead(
        this LTranslationArchive translationArchive,
        string collocationId) =>
        translationArchive.LTranslationCollocationRead(collocationId);

    internal static void TTranslationCollocationSave(
        this LTranslationArchive translationArchive,
        string collocationId,
        IReadOnlyList<LTranslation> translations)
    {
        translationArchive.LTranslationCollocationSave(collocationId, translations);
    }

    internal static IReadOnlyList<LUsage> TTranslationIncomingRead(
        this LTranslationArchive translationArchive,
        string entryId) =>
        translationArchive.LTranslationIncomingRead(entryId);

    internal static IReadOnlyList<LTranslation> TTranslationMeaningRead(
        this LTranslationArchive translationArchive,
        string meaningId) =>
        translationArchive.LTranslationMeaningRead(meaningId);

    internal static void TTranslationMeaningSave(
        this LTranslationArchive translationArchive,
        string meaningId,
        IReadOnlyList<LTranslation> translations)
    {
        translationArchive.LTranslationMeaningSave(meaningId, translations);
    }

    internal static IReadOnlyList<LTranslationTarget> TTranslationTargetRead(
        this LTranslationArchive translationArchive,
        IReadOnlyList<string> ids) =>
        translationArchive.LTranslationTargetRead(ids);

    internal static LWorkspaceArchive TWorkspaceArchiveCreate(LDatabase database) =>
        new(database);

    internal static string TWorkspaceCourtRead(string root) =>
        LWorkspaceRoot.LWorkspaceCourtRead(root);

    internal static string TWorkspaceDraftRead(string root) =>
        LWorkspaceRoot.LWorkspaceDraftRead(root);

    internal static LWorkspaceState TWorkspaceStateRead(this LWorkspaceArchive workspaceArchive) =>
        workspaceArchive.LWorkspaceStateRead();
}
