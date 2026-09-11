using Llyn.Core;
using Llyn.Infrastructure;
using Xunit;

namespace Llyn.Tests;

public sealed class TDatabaseSession
{
    [Fact]
    public void SessionStart_NeverCommitted_WritesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);

        using (LDatabaseSession session = workspace.TWorkspaceDatabase.TDatabaseSessionStart())
        {
            entries.TEntryCreate(
                TInterface.TEntryCreate(0, "word", "en", null, null, null, null), [], []);
        }

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
    }

    [Fact]
    public void SessionCommit_SeveralStores_LandsTogether()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LNoteArchive notes = TInterface.TNoteArchiveCreate(workspace.TWorkspaceDatabase);

        using (LDatabaseSession session = workspace.TWorkspaceDatabase.TDatabaseSessionStart())
        {
            LEntry entry = entries.TEntryCreate(
                TInterface.TEntryCreate(0, "word", "en", null, null, null, null), [], []);
            notes.TNoteSave(TInterface.TNoteCreate(entry.LEntryId, "a note"));
            session.TDatabaseSessionCommit();
        }

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM note;"));
    }

    [Fact]
    public void SessionCommit_FailurePartWay_RollsBackAll()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);

        Assert.Throws<InvalidOperationException>(() =>
        {
            using LDatabaseSession session = workspace.TWorkspaceDatabase.TDatabaseSessionStart();
            entries.TEntryCreate(TInterface.TEntryCreate(0, "word", "en", null, null, null, null), [], []);
            entries.TEntryUpdate(TInterface.TEntryCreate(9999, "word", "en", null, null, null, null));
            session.TDatabaseSessionCommit();
        });

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
    }

    [Fact]
    public void MeaningMove_AmongSiblings_RenumbersGroup()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LMeaningArchive meanings = TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = entries.TEntryCreate(
            TInterface.TEntryCreate(0, "word", "en", null, null, null, null), [], []);
        LMeaning first = meanings.TMeaningCreate(
            TInterface.TMeaningCreate(0, entry.LEntryId, null, 0, null, "one", null, null, string.Empty));
        LMeaning second = meanings.TMeaningCreate(
            TInterface.TMeaningCreate(0, entry.LEntryId, null, 0, null, "two", null, null, string.Empty));
        LMeaning third = meanings.TMeaningCreate(
            TInterface.TMeaningCreate(0, entry.LEntryId, null, 0, null, "three", null, null, string.Empty));

        Assert.Equal([0, 1, 2], meanings.TMeaningRead(entry.LEntryId).Select(meaning => meaning.LMeaningPosition));

        meanings.TMeaningMove(third.LMeaningId, 0);

        Assert.Equal(
            [third.LMeaningId, first.LMeaningId, second.LMeaningId],
            meanings.TMeaningRead(entry.LEntryId).Select(meaning => meaning.LMeaningId));
        Assert.Equal([0, 1, 2], meanings.TMeaningRead(entry.LEntryId).Select(meaning => meaning.LMeaningPosition));
    }

    [Fact]
    public void MeaningDelete_MiddleRow_ClosesPositionGap()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LMeaningArchive meanings = TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = entries.TEntryCreate(
            TInterface.TEntryCreate(0, "word", "en", null, null, null, null), [], []);
        LMeaning first = meanings.TMeaningCreate(
            TInterface.TMeaningCreate(0, entry.LEntryId, null, 0, null, "one", null, null, string.Empty));
        meanings.TMeaningCreate(TInterface.TMeaningCreate(0, entry.LEntryId, null, 0, null, "two", null, null, string.Empty));
        meanings.TMeaningCreate(TInterface.TMeaningCreate(0, entry.LEntryId, null, 0, null, "three", null, null, string.Empty));

        meanings.TMeaningDelete(first.LMeaningId);

        Assert.Equal([0, 1], meanings.TMeaningRead(entry.LEntryId).Select(meaning => meaning.LMeaningPosition));
    }

    [Fact]
    public void TagMeaningSave_OverOlderLine_NumbersFromZero()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LMeaningArchive meanings = TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase);
        LTagArchive tags = TInterface.TTagArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = entries.TEntryCreate(
            TInterface.TEntryCreate(0, "word", "en", null, null, null, null), [], []);
        LMeaning meaning = meanings.TMeaningCreate(
            TInterface.TMeaningCreate(0, entry.LEntryId, null, 0, null, null, null, null, string.Empty));

        tags.TTagMeaningSave(
            meaning.LMeaningId, [TInterface.TTagCreate("formal"), TInterface.TTagCreate("archaic"), TInterface.TTagCreate("rare")]);

        Assert.Equal(
            ["formal", "archaic", "rare"],
            tags.TTagMeaningRead(meaning.LMeaningId).Select(tag => tag.LTagText));

        tags.TTagMeaningSave(meaning.LMeaningId, [TInterface.TTagCreate("rare"), TInterface.TTagCreate("archaic")]);

        Assert.Equal(
            ["rare", "archaic"],
            tags.TTagMeaningRead(meaning.LMeaningId).Select(tag => tag.LTagText));
        Assert.Equal(
            [0, 1],
            TDatabasePositionRead(workspace, "sense_tag", "sense_id", meaning.LMeaningId));
    }

    [Fact]
    public void InflectionDelete_MiddleRow_RenumbersKeepsFeatures()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LInflectionArchive inflections = TInterface.TInflectionArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = entries.TEntryCreate(
            TInterface.TEntryCreate(0, "run", "en", null, null, null, null), [], []);
        inflections.TInflectionAppend(entry.LEntryId,
        [
            TInterface.TInflectionCreate(entry.LEntryId, 0, "runs", null, null, [TInterface.TFeatureCreate("number", "singular")]),
            TInterface.TInflectionCreate(entry.LEntryId, 0, "ran", null, null, [TInterface.TFeatureCreate("tense", "past")]),
        ]);
        inflections.TInflectionAppend(entry.LEntryId,
        [
            TInterface.TInflectionCreate(entry.LEntryId, 0, "running", null, null, [TInterface.TFeatureCreate("aspect", "progressive")]),
        ]);

        Assert.Equal(
            ["runs", "ran", "running"],
            inflections.TInflectionRead(entry.LEntryId).Select(inflection => inflection.LInflectionText));

        inflections.TInflectionDelete(entry.LEntryId, 0);

        IReadOnlyList<LInflection> remaining = inflections.TInflectionRead(entry.LEntryId);
        Assert.Equal(["ran", "running"], remaining.Select(inflection => inflection.LInflectionText));
        Assert.Equal([0, 1], remaining.Select(inflection => inflection.LInflectionPosition));
        Assert.Equal("past", remaining[0].LInflectionFeatures[0].LFeatureValueId);
        Assert.Equal("progressive", remaining[1].LInflectionFeatures[0].LFeatureValueId);
    }

    [Fact]
    public void ExampleUpdate_WithTranslation_KeepsTranslation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);

        LExample stored = examples.TExampleCreate(TInterface.TExampleCreate(
            0, "en", "a sentence", "one translation", null));

        examples.TExampleUpdate(stored with { LExampleText = "a changed sentence" });

        LExample? read = examples.TExampleRead(stored.LExampleId);
        Assert.NotNull(read);
        Assert.Equal("a changed sentence", read.LExampleText);
        Assert.Equal("one translation", read.LExampleTranslation);
    }

    [Fact]
    public void IdentityCreate_ManyCalls_ReturnsDistinctValues()
    {
        HashSet<long> identifiers = [];
        for (int count = 0; count < 5000; count++)
        {
            long identifier = TInterface.TIdentityCreate();
            Assert.True(identifier < 0);
            Assert.True(identifiers.Add(identifier));
        }
    }

    private static IReadOnlyList<long> TDatabasePositionRead(
        TWorkspace workspace, string table, string column, long referrerId)
    {
        using Microsoft.Data.Sqlite.SqliteConnection connection = workspace.TWorkspaceConnectionRead();
        using Microsoft.Data.Sqlite.SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"SELECT position FROM {table} WHERE {column} = $referrer ORDER BY position;";
        command.Parameters.AddWithValue("$referrer", referrerId);

        List<long> positions = [];
        using Microsoft.Data.Sqlite.SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            positions.Add(reader.GetInt64(0));
        }

        return positions;
    }
}
