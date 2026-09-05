using Llyn.Core;
using Llyn.Infrastructure;
using Xunit;

namespace Llyn.Tests;

public sealed class TDatabaseSession
{
    [Fact]
    public void WorkInAnUncommittedSessionLeavesNothingBehind()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);

        using (LDatabaseSession session = workspace.TWorkspaceDatabase.TDatabaseSessionStart())
        {
            entries.TEntryCreate(
                TInterface.TEntryCreate(string.Empty, "word", "en", null, null, null, null), [], []);
        }

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
    }

    [Fact]
    public void WorkAcrossSeveralStoresLandsTogetherWhenTheSessionCommits()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LNoteArchive notes = TInterface.TNoteArchiveCreate(workspace.TWorkspaceDatabase);

        using (LDatabaseSession session = workspace.TWorkspaceDatabase.TDatabaseSessionStart())
        {
            LEntry entry = entries.TEntryCreate(
                TInterface.TEntryCreate(string.Empty, "word", "en", null, null, null, null), [], []);
            notes.TNoteSave(TInterface.TNoteCreate(entry.LEntryId, "a note"));
            session.TDatabaseSessionCommit();
        }

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM note;"));
    }

    [Fact]
    public void AFailurePartWayThroughASessionRollsBackEverythingBeforeIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);

        Assert.Throws<InvalidOperationException>(() =>
        {
            using LDatabaseSession session = workspace.TWorkspaceDatabase.TDatabaseSessionStart();
            entries.TEntryCreate(TInterface.TEntryCreate(string.Empty, "word", "en", null, null, null, null), [], []);
            entries.TEntryUpdate(TInterface.TEntryCreate("missing", "word", "en", null, null, null, null));
            session.TDatabaseSessionCommit();
        });

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
    }

    [Fact]
    public void MovingAMeaningAmongItsSiblingsRenumbersTheWholeGroup()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LSenseArchive senses = TInterface.TSenseArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = entries.TEntryCreate(
            TInterface.TEntryCreate(string.Empty, "word", "en", null, null, null, null), [], []);
        LSense first = senses.TSenseCreate(
            TInterface.TSenseCreate(string.Empty, entry.LEntryId, null, 0, null, "one", null, null, string.Empty));
        LSense second = senses.TSenseCreate(
            TInterface.TSenseCreate(string.Empty, entry.LEntryId, null, 0, null, "two", null, null, string.Empty));
        LSense third = senses.TSenseCreate(
            TInterface.TSenseCreate(string.Empty, entry.LEntryId, null, 0, null, "three", null, null, string.Empty));

        Assert.Equal([0, 1, 2], senses.TSenseRead(entry.LEntryId).Select(sense => sense.LSensePosition));

        senses.TSenseMove(third.LSenseId, 0);

        Assert.Equal(
            [third.LSenseId, first.LSenseId, second.LSenseId],
            senses.TSenseRead(entry.LEntryId).Select(sense => sense.LSenseId));
        Assert.Equal([0, 1, 2], senses.TSenseRead(entry.LEntryId).Select(sense => sense.LSensePosition));
    }

    [Fact]
    public void DeletingAMeaningClosesTheGapItLeaves()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LSenseArchive senses = TInterface.TSenseArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = entries.TEntryCreate(
            TInterface.TEntryCreate(string.Empty, "word", "en", null, null, null, null), [], []);
        LSense first = senses.TSenseCreate(
            TInterface.TSenseCreate(string.Empty, entry.LEntryId, null, 0, null, "one", null, null, string.Empty));
        senses.TSenseCreate(TInterface.TSenseCreate(string.Empty, entry.LEntryId, null, 0, null, "two", null, null, string.Empty));
        senses.TSenseCreate(TInterface.TSenseCreate(string.Empty, entry.LEntryId, null, 0, null, "three", null, null, string.Empty));

        senses.TSenseDelete(first.LSenseId);

        Assert.Equal([0, 1], senses.TSenseRead(entry.LEntryId).Select(sense => sense.LSensePosition));
    }

    [Fact]
    public void WritingATagLineOverAnOlderOneLeavesTheNewOrderNumberedFromZero()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LSenseArchive senses = TInterface.TSenseArchiveCreate(workspace.TWorkspaceDatabase);
        LTagArchive tags = TInterface.TTagArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = entries.TEntryCreate(
            TInterface.TEntryCreate(string.Empty, "word", "en", null, null, null, null), [], []);
        LSense sense = senses.TSenseCreate(
            TInterface.TSenseCreate(string.Empty, entry.LEntryId, null, 0, null, null, null, null, string.Empty));

        tags.TTagSenseSave(
            sense.LSenseId, [TInterface.TTagCreate("formal"), TInterface.TTagCreate("archaic"), TInterface.TTagCreate("rare")]);

        Assert.Equal(
            ["formal", "archaic", "rare"],
            tags.TTagSenseRead(sense.LSenseId).Select(tag => tag.LTagText));

        tags.TTagSenseSave(sense.LSenseId, [TInterface.TTagCreate("rare"), TInterface.TTagCreate("archaic")]);

        Assert.Equal(
            ["rare", "archaic"],
            tags.TTagSenseRead(sense.LSenseId).Select(tag => tag.LTagText));
        Assert.Equal(
            [0, 1],
            TDatabasePositionRead(workspace, "sense_tag", "sense_id", sense.LSenseId));
    }

    [Fact]
    public void RemovingAnInflectionRenumbersTheSetAndCarriesItsFeatures()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LInflectionArchive inflections = TInterface.TInflectionArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = entries.TEntryCreate(
            TInterface.TEntryCreate(string.Empty, "run", "en", null, null, null, null), [], []);
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
    public void AnExampleCarriesItsOwnTranslationThroughAnUpdate()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);

        LExample stored = examples.TExampleCreate(TInterface.TExampleCreate(
            string.Empty, "en", "a sentence", "one translation", null));

        examples.TExampleUpdate(stored with { LExampleText = "a changed sentence" });

        LExample? read = examples.TExampleRead(stored.LExampleId);
        Assert.NotNull(read);
        Assert.Equal("a changed sentence", read.LExampleText);
        Assert.Equal("one translation", read.LExampleTranslation);
    }

    [Fact]
    public void IdentifiersAreDistinctAndLongEnoughToStayThatWay()
    {
        HashSet<string> identifiers = [];
        for (int count = 0; count < 5000; count++)
        {
            string identifier = TInterface.TIdentityCreate();
            Assert.Equal(12, identifier.Length);
            Assert.True(identifiers.Add(identifier));
        }
    }

    private static IReadOnlyList<long> TDatabasePositionRead(
        TWorkspace workspace, string table, string column, string referrerId)
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
