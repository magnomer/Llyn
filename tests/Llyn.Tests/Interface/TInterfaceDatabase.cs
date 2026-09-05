using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.Tests;

internal static partial class TInterface
{
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

    internal static IReadOnlyList<LCourtLink> TCourtArchiveCancel(string root, string draftId) =>
        LCourtArchive.LCourtArchiveCancel(root, draftId);

    internal static LCourtLink? TCourtArchiveRead(string root, string id) =>
        LCourtArchive.LCourtArchiveRead(root, id);

    internal static IReadOnlyList<LCourtLink> TCourtArchiveResolve(
        string root,
        string draftId,
        string realId) =>
        LCourtArchive.LCourtArchiveResolve(root, draftId, realId);

    internal static void TCourtArchiveSave(string root, LCourtLink link)
    {
        LCourtArchive.LCourtArchiveSave(root, link);
    }

    internal static IReadOnlyList<LCourtLink> TCourtArchiveScan(string root) =>
        LCourtArchive.LCourtArchiveScan(root);

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

    internal static IReadOnlyList<LExample> TExampleCollocationRead(
        this LExampleLink exampleLink,
        string collocationId) =>
        exampleLink.LExampleCollocationRead(collocationId);

    internal static LExample TExampleCreate(this LExampleArchive exampleArchive, LExample example) =>
        exampleArchive.LExampleCreate(example);

    internal static LExampleLink TExampleLinkCreate(LDatabase database) =>
        new(database);

    internal static IReadOnlyList<LExample> TExampleRead(this LExampleArchive exampleArchive) =>
        exampleArchive.LExampleRead();

    internal static LExample? TExampleRead(this LExampleArchive exampleArchive, string id) =>
        exampleArchive.LExampleRead(id);

    internal static void TExampleSenseAttach(
        this LExampleLink exampleLink,
        string senseId,
        string exampleId,
        int position)
    {
        exampleLink.LExampleSenseAttach(senseId, exampleId, position);
    }

    internal static IReadOnlyList<LExample> TExampleSenseRead(
        this LExampleLink exampleLink,
        string senseId) =>
        exampleLink.LExampleSenseRead(senseId);

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

    internal static LSenseArchive TSenseArchiveCreate(LDatabase database) =>
        new(database);

    internal static LSense TSenseCreate(this LSenseArchive senseArchive, LSense sense) =>
        senseArchive.LSenseCreate(sense);

    internal static void TSenseDelete(this LSenseArchive senseArchive, string id)
    {
        senseArchive.LSenseDelete(id);
    }

    internal static void TSenseMove(this LSenseArchive senseArchive, string id, int position)
    {
        senseArchive.LSenseMove(id, position);
    }

    internal static IReadOnlyList<LSense> TSenseRead(this LSenseArchive senseArchive, string entryId) =>
        senseArchive.LSenseRead(entryId);

    internal static LSituationArchive TSituationArchiveCreate(LDatabase database) =>
        new(database);

    internal static IReadOnlyList<LSituation> TSituationCollocationRead(
        this LSituationArchive situationArchive,
        string collocationId) =>
        situationArchive.LSituationCollocationRead(collocationId);

    internal static IReadOnlyList<LSituation> TSituationSenseRead(
        this LSituationArchive situationArchive,
        string senseId) =>
        situationArchive.LSituationSenseRead(senseId);

    internal static LTagArchive TTagArchiveCreate(LDatabase database) =>
        new(database);

    internal static IReadOnlyList<LTag> TTagCollocationRead(
        this LTagArchive tagArchive,
        string collocationId) =>
        tagArchive.LTagCollocationRead(collocationId);

    internal static IReadOnlyList<LTag> TTagSenseRead(this LTagArchive tagArchive, string senseId) =>
        tagArchive.LTagSenseRead(senseId);

    internal static void TTagSenseSave(this LTagArchive tagArchive, string senseId, IReadOnlyList<LTag> tags)
    {
        tagArchive.LTagSenseSave(senseId, tags);
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

    internal static IReadOnlyList<LTranslation> TTranslationSenseRead(
        this LTranslationArchive translationArchive,
        string senseId) =>
        translationArchive.LTranslationSenseRead(senseId);

    internal static void TTranslationSenseSave(
        this LTranslationArchive translationArchive,
        string senseId,
        IReadOnlyList<LTranslation> translations)
    {
        translationArchive.LTranslationSenseSave(senseId, translations);
    }

    internal static IReadOnlyList<LTranslationTarget> TTranslationTargetRead(
        this LTranslationArchive translationArchive,
        IReadOnlyList<string> ids) =>
        translationArchive.LTranslationTargetRead(ids);

    internal static LWorkspaceArchive TWorkspaceArchiveCreate(LDatabase database) =>
        new(database);

    internal static string TWorkspaceDraftRead(string root) =>
        LWorkspaceRoot.LWorkspaceDraftRead(root);

    internal static LWorkspaceState TWorkspaceStateRead(this LWorkspaceArchive workspaceArchive) =>
        workspaceArchive.LWorkspaceStateRead();
}
