using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Microsoft.Data.Sqlite;

namespace Llyn.Tests;

internal static partial class TInterface
{
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
        new LDoctor(database).LDoctorDatabaseCreate();

    internal static bool TDoctorRescueCheck(Exception fault) =>
        LDoctor.LDoctorRescueCheck(fault);

    internal static bool TDoctorBusyCheck(Exception fault) =>
        LDoctor.LDoctorBusyCheck(fault);

    internal static LEntryArchive TEntryArchiveCreate(LDatabase database) =>
        new(database);

    internal static LEtymologyArchive TEtymologyArchiveCreate(LDatabase database) =>
        new(database);

    internal static LEtymology? TEtymologyRead(this LEtymologyVault etymologyVault, long entryId) =>
        etymologyVault.LEtymologyRead(entryId);

    internal static LEtymology? TEtymologySave(
        this LEtymologyVault etymologyVault, long entryId, LEtymology? etymology) =>
        etymologyVault.LEtymologySave(entryId, etymology);

    internal static IReadOnlyList<LEtymon> TEtymonRead(this LEtymologyVault etymologyVault, long entryId) =>
        etymologyVault.LEtymologyEtymonRead(entryId);

    internal static IReadOnlyList<LEtymon> TEtymonSet(
        this LEtymologyVault etymologyVault, long entryId, IReadOnlyList<long> targetIds) =>
        etymologyVault.LEtymologyEtymonSet(entryId, targetIds);

    internal static LShengfuArchive TShengfuArchiveCreate(LDatabase database) =>
        new(database);

    internal static void TShengfuSave(this LShengfuVault shengfuVault, string language, LShengfu row) =>
        shengfuVault.LShengfuSave(language, row);

    internal static LStemArchive TStemArchiveCreate(LDatabase database) =>
        new(database);

    internal static void TStemApply(
        this LStemVault stemVault, string language, string character, IReadOnlyList<string> keys) =>
        stemVault.LStemApply(language, character, keys);

    internal static void TStemRebuild(this LStemVault stemVault, string language, string separator) =>
        stemVault.LStemRebuild(language, separator);

    internal static IReadOnlyList<LStem> TStemRead(this LStemVault stemVault, string language) =>
        stemVault.LStemRead(language);

    internal static LStem? TStemFind(this LStemVault stemVault, string language, string key) =>
        stemVault.LStemFind(language, key);

    internal static IReadOnlyList<string> TStemCharacterRead(this LStemVault stemVault, long stemId) =>
        stemVault.LStemCharacterRead(stemId);

    internal static IReadOnlyList<long> TStemEntryScan(
        this LStemVault stemVault, string language, IReadOnlyList<long> stemIds) =>
        stemVault.LStemEntryScan(language, stemIds);

    internal static LEntry TEntryCreate(
        this LEntryVault entryVault,
        LEntry entry,
        IReadOnlyList<LForm> forms,
        IReadOnlyList<LSpeech> speeches) =>
        entryVault.LEntryCreate(entry, forms, speeches);

    internal static void TEntryDelete(this LEntryVault entryVault, long id)
    {
        entryVault.LEntryDelete(id);
    }

    internal static IReadOnlyList<LEntry> TEntryFind(this LEntryVault entryVault, string query) =>
        entryVault.LEntryFind(query);

    internal static IReadOnlyList<LEntry> TEntryHeadwordFind(
        this LEntryVault entryVault, string language, string headword) =>
        entryVault.LEntryHeadwordFind(language, headword);

    internal static IReadOnlyList<LForm> TEntryFormRead(this LEntryVault entryVault, long id) =>
        entryVault.LEntryFormRead(id);

    internal static LEntry? TEntryRead(this LEntryVault entryVault, long id) =>
        entryVault.LEntryRead(id);

    internal static LEntryDraft? TEntryLoad(this LEntryVault entryVault, long id) =>
        entryVault.LEntryLoad(id);

    internal static IReadOnlyList<LSpeech> TEntrySpeechRead(this LEntryVault entryVault, long id) =>
        entryVault.LEntrySpeechRead(id);

    internal static void TEntryUpdate(this LEntryVault entryVault, LEntry entry)
    {
        entryVault.LEntryUpdate(entry);
    }

    internal static LExampleArchive TExampleArchiveCreate(LDatabase database) =>
        new(database);

    internal static LExample TExampleCreate(this LExampleArchive exampleArchive, LExample example) =>
        exampleArchive.LExampleCreate(example);

    internal static IReadOnlyList<LExample> TExampleRead(this LExampleArchive exampleArchive) =>
        exampleArchive.LExampleRead();

    internal static LExample? TExampleRead(this LExampleArchive exampleArchive, long id) =>
        exampleArchive.LExampleRead(id);

    internal static void TExampleDelete(this LExampleArchive exampleArchive, long id, bool detach)
    {
        exampleArchive.LExampleDelete(id, detach);
    }

    internal static void TExampleUpdate(this LExampleArchive exampleArchive, LExample example)
    {
        exampleArchive.LExampleUpdate(example);
    }

    private static long TInterfaceIdentity;

    internal static long TIdentityCreate() =>
        Interlocked.Decrement(ref TInterfaceIdentity);

    internal static LImageArchive TImageArchiveCreate(LDatabase database) =>
        new(database);

    internal static LImage? TImageRead(this LImageArchive imageArchive, long id) =>
        imageArchive.LImageRead(id);

    internal static void TInflectionAppend(
        this LInflectionArchive inflectionArchive,
        long entryId,
        IReadOnlyList<LInflection> inflections)
    {
        inflectionArchive.LInflectionAppend(entryId, inflections);
    }

    internal static LInflectionArchive TInflectionArchiveCreate(LDatabase database) =>
        new(database);

    internal static IReadOnlyList<LInflection> TInflectionRead(
        this LInflectionArchive inflectionArchive,
        long entryId) =>
        inflectionArchive.LInflectionRead(entryId);

    internal static void TExampleTextUpdate(this LExampleArchive exampleArchive, long exampleId, LStateValue text)
    {
        exampleArchive.LExampleTextUpdate(exampleId, text);
    }

    internal static LMorphologyArchive TMorphologyArchiveCreate(LDatabase database) =>
        new(database);

    internal static LFeature TFeatureCreate(
        this LMorphologyArchive morphologyArchive,
        LFeature feature) =>
        morphologyArchive.LFeatureCreate(feature);

    internal static LMorphology TMorphologyCreate(
        this LMorphologyArchive morphologyArchive,
        LMorphology value) =>
        morphologyArchive.LMorphologyCreate(value);

    internal static LMorphology? TMorphologyCodeFind(
        this LMorphologyArchive morphologyArchive,
        string language,
        long speechCode,
        long featureCode,
        long code) =>
        morphologyArchive.LMorphologyCodeFind(language, speechCode, featureCode, code);

    internal static LNoteArchive TNoteArchiveCreate(LDatabase database) =>
        new(database);

    internal static LNote? TNoteRead(this LNoteArchive noteArchive, long entryId) =>
        noteArchive.LNoteRead(entryId);

    internal static void TNoteSave(this LNoteArchive noteArchive, LNote note)
    {
        noteArchive.LNoteSave(note);
    }

    internal static LPronunciationArchive TPronunciationArchiveCreate(LDatabase database) =>
        new(database);

    internal static LPronunciationAudio? TPronunciationAudioRead(
        this LPronunciationArchive pronunciationArchive,
        long pronunciationId) =>
        pronunciationArchive.LPronunciationAudioRead(pronunciationId);

    internal static IReadOnlyList<LPronunciation> TPronunciationRead(
        this LPronunciationArchive pronunciationArchive,
        long entryId) =>
        pronunciationArchive.LPronunciationRead(entryId);

    internal static LTranscriptionArchive TTranscriptionArchiveCreate(LDatabase database) =>
        new(database);

    internal static IReadOnlyList<LTranscription> TTranscriptionRead(
        this LTranscriptionArchive transcriptionArchive,
        long entryId) =>
        transcriptionArchive.LTranscriptionRead(entryId);

    internal static LRevisionArchive TRevisionArchiveCreate(LDatabase database) =>
        new(database);

    internal static IReadOnlyList<LRevisionChange> TRevisionChangeRead(this TWorkspace workspace, long revisionId)
    {
        using SqliteConnection connection = workspace.TWorkspaceConnectionRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT target_ref, target_type, kind, summary
            FROM revision_change WHERE revision_parent = $revision ORDER BY position;
            """;
        command.Parameters.AddWithValue("$revision", revisionId);

        List<LRevisionChange> changes = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            changes.Add(new LRevisionChange(
                reader.GetInt64(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3)));
        }

        return changes;
    }

    internal static Guid TRealmValueRead(TWorkspace workspace)
    {
        using SqliteConnection connection = workspace.TWorkspaceConnectionRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT value FROM realm WHERE realm_id = 1;";
        return new Guid((byte[])command.ExecuteScalar()!);
    }

    internal static LSpeechArchive TSpeechArchiveCreate(LDatabase database) =>
        new(database);

    internal static LSpeechPack TSpeechPackLoad(string language) =>
        LSpeechLoader.LSpeechLoaderLoad(language);

    internal static LSpeechValue TSpeechValueCreate(
        this LSpeechArchive speechArchive,
        LSpeechValue value) =>
        speechArchive.LSpeechValueCreate(value);

    internal static LDiweiArchive TDiweiArchiveCreate(LDatabase database) =>
        new(database);

    internal static void TDiweiApply(
        this LDiweiArchive diweiArchive, string language, string character, LHypothesis? hypothesis)
    {
        diweiArchive.LDiweiApply(language, character, hypothesis);
    }

    internal static void TDiweiRebuild(this LDiweiArchive diweiArchive, string language, LHypothesis? hypothesis)
    {
        diweiArchive.LDiweiRebuild(language, hypothesis);
    }

    internal static IReadOnlyList<LDiwei> TDiweiRead(this LDiweiArchive diweiArchive, string language, string kind) =>
        diweiArchive.LDiweiRead(language, kind);

    internal static LDiwei? TDiweiFind(
        this LDiweiArchive diweiArchive, string language, string kind, string key) =>
        diweiArchive.LDiweiFind(language, kind, key);

    internal static IReadOnlyList<long> TDiweiEntryScan(
        this LDiweiArchive diweiArchive, string language, IReadOnlyList<long> diweiIds) =>
        diweiArchive.LDiweiEntryScan(language, diweiIds);

    internal static LFanqieArchive TFanqieArchiveCreate(LDatabase database) =>
        new(database);

    internal static void TFanqieSave(
        this LFanqieArchive fanqieArchive, string language, string character, IReadOnlyList<LFanqieRow> rows)
    {
        fanqieArchive.LFanqieSave(language, character, rows);
    }

    internal static IReadOnlyList<LFanqieRow> TFanqieRead(
        this LFanqieArchive fanqieArchive, string language, string character) =>
        fanqieArchive.LFanqieRead(language, character);

    internal static string TEntryEpithetRead(this LEntryVault entryVault, long entryId) =>
        entryVault.LEntryEpithetRead(entryId);

    internal static LVideoArchive TVideoArchiveCreate(LDatabase database) =>
        new(database);

    internal static LVideo? TVideoRead(this LVideoArchive videoArchive, long id) =>
        videoArchive.LVideoRead(id);

    internal static LWorkspaceArchive TWorkspaceArchiveCreate(LDatabase database) =>
        new(database);

    internal static string TWorkspaceCourtRead(string root) =>
        LWorkspaceRoot.LWorkspaceCourtRead(root);

    internal static string TWorkspaceDraftRead(string root) =>
        LWorkspaceRoot.LWorkspaceDraftRead(root);

    internal static LWorkspaceState TWorkspaceStateRead(this LWorkspaceArchive workspaceArchive) =>
        workspaceArchive.LWorkspaceStateRead();
}
