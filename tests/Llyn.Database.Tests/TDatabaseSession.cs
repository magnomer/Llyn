using Llyn.Core;
using Llyn.Infrastructure;
using Xunit;

namespace Llyn.Database.Tests;

/// <summary>
/// Covers the unit of work and the ordering it makes possible: a session spanning several stores lands
/// as one transaction or not at all, a nested session leaves the decision to the outermost one, and
/// every ordered set stays numbered <c>0 … n-1</c> across a move, an insert into the middle, and a
/// removal — the reorders the unique position indexes used to make impossible.
/// </summary>
public sealed class TDatabaseSession
{
    [Fact]
    public void WorkInAnUncommittedSessionLeavesNothingBehind()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);

        using (LDatabaseSession session = workspace.TWorkspaceDatabase.LDatabaseSessionStart())
        {
            entries.LEntryCreate(
                new LEntry(string.Empty, "word", "en", null, null, null, null), [], []);

            // The store committed its own nested session; the outermost one never does.
        }

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
    }

    [Fact]
    public void WorkAcrossSeveralStoresLandsTogetherWhenTheSessionCommits()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        LNoteArchive notes = new(workspace.TWorkspaceDatabase);

        using (LDatabaseSession session = workspace.TWorkspaceDatabase.LDatabaseSessionStart())
        {
            LEntry entry = entries.LEntryCreate(
                new LEntry(string.Empty, "word", "en", null, null, null, null), [], []);
            notes.LNoteSave(new LNote(entry.LEntryId, "a note"));
            session.LDatabaseSessionCommit();
        }

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM note;"));
    }

    [Fact]
    public void AFailurePartWayThroughASessionRollsBackEverythingBeforeIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);

        Assert.Throws<InvalidOperationException>(() =>
        {
            using LDatabaseSession session = workspace.TWorkspaceDatabase.LDatabaseSessionStart();
            entries.LEntryCreate(new LEntry(string.Empty, "word", "en", null, null, null, null), [], []);
            entries.LEntryUpdate(new LEntry("missing", "word", "en", null, null, null, null));
            session.LDatabaseSessionCommit();
        });

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
    }

    [Fact]
    public void MovingAMeaningAmongItsSiblingsRenumbersTheWholeGroup()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        LSenseArchive senses = new(workspace.TWorkspaceDatabase);

        LEntry entry = entries.LEntryCreate(
            new LEntry(string.Empty, "word", "en", null, null, null, null), [], []);
        LSense first = senses.LSenseCreate(
            new LSense(string.Empty, entry.LEntryId, null, 0, "one", null, null, string.Empty));
        LSense second = senses.LSenseCreate(
            new LSense(string.Empty, entry.LEntryId, null, 0, "two", null, null, string.Empty));
        LSense third = senses.LSenseCreate(
            new LSense(string.Empty, entry.LEntryId, null, 0, "three", null, null, string.Empty));

        Assert.Equal([0, 1, 2], senses.LSenseRead(entry.LEntryId).Select(sense => sense.LSensePosition));

        senses.LSenseMove(third.LSenseId, 0);

        Assert.Equal(
            [third.LSenseId, first.LSenseId, second.LSenseId],
            senses.LSenseRead(entry.LEntryId).Select(sense => sense.LSenseId));
        Assert.Equal([0, 1, 2], senses.LSenseRead(entry.LEntryId).Select(sense => sense.LSensePosition));
    }

    [Fact]
    public void DeletingAMeaningClosesTheGapItLeaves()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        LSenseArchive senses = new(workspace.TWorkspaceDatabase);

        LEntry entry = entries.LEntryCreate(
            new LEntry(string.Empty, "word", "en", null, null, null, null), [], []);
        LSense first = senses.LSenseCreate(
            new LSense(string.Empty, entry.LEntryId, null, 0, "one", null, null, string.Empty));
        senses.LSenseCreate(new LSense(string.Empty, entry.LEntryId, null, 0, "two", null, null, string.Empty));
        senses.LSenseCreate(new LSense(string.Empty, entry.LEntryId, null, 0, "three", null, null, string.Empty));

        senses.LSenseDelete(first.LSenseId);

        Assert.Equal([0, 1], senses.LSenseRead(entry.LEntryId).Select(sense => sense.LSensePosition));
    }

    [Fact]
    public void AttachingATagAtAnOccupiedPositionInsertsInsteadOfFailing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        LSenseArchive senses = new(workspace.TWorkspaceDatabase);
        LTagArchive tags = new(workspace.TWorkspaceDatabase);

        LEntry entry = entries.LEntryCreate(
            new LEntry(string.Empty, "word", "en", null, null, null, null), [], []);
        LSense sense = senses.LSenseCreate(
            new LSense(string.Empty, entry.LEntryId, null, 0, null, null, null, string.Empty));
        LTag formal = tags.LTagCreate(new LTag(string.Empty, "formal"));
        LTag archaic = tags.LTagCreate(new LTag(string.Empty, "archaic"));
        LTag rare = tags.LTagCreate(new LTag(string.Empty, "rare"));

        tags.LTagSenseAttach(sense.LSenseId, formal.LTagId, 0);
        tags.LTagSenseAttach(sense.LSenseId, archaic.LTagId, 1);
        tags.LTagSenseAttach(sense.LSenseId, rare.LTagId, 0);

        Assert.Equal(
            ["rare", "formal", "archaic"],
            tags.LTagSenseRead(sense.LSenseId).Select(tag => tag.LTagText));

        tags.LTagSenseDetach(sense.LSenseId, formal.LTagId);

        Assert.Equal(
            ["rare", "archaic"],
            tags.LTagSenseRead(sense.LSenseId).Select(tag => tag.LTagText));
        Assert.Equal(
            [0, 1],
            TDatabasePositionRead(workspace, "sense_tag", "sense_id", sense.LSenseId));
    }

    [Fact]
    public void RemovingAnInflectionRenumbersTheSetAndCarriesItsFeatures()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        LInflectionArchive inflections = new(workspace.TWorkspaceDatabase);

        LEntry entry = entries.LEntryCreate(
            new LEntry(string.Empty, "run", "en", null, null, null, null), [], []);
        inflections.LInflectionAppend(entry.LEntryId,
        [
            new LInflection(entry.LEntryId, 0, "runs", null, null, [new LFeature("number", "singular")]),
            new LInflection(entry.LEntryId, 0, "ran", null, null, [new LFeature("tense", "past")]),
        ]);
        inflections.LInflectionAppend(entry.LEntryId,
        [
            new LInflection(entry.LEntryId, 0, "running", null, null, [new LFeature("aspect", "progressive")]),
        ]);

        Assert.Equal(
            ["runs", "ran", "running"],
            inflections.LInflectionRead(entry.LEntryId).Select(inflection => inflection.LInflectionText));

        inflections.LInflectionDelete(entry.LEntryId, 0);

        IReadOnlyList<LInflection> remaining = inflections.LInflectionRead(entry.LEntryId);
        Assert.Equal(["ran", "running"], remaining.Select(inflection => inflection.LInflectionText));
        Assert.Equal([0, 1], remaining.Select(inflection => inflection.LInflectionPosition));
        Assert.Equal("past", remaining[0].LInflectionFeatures[0].LFeatureValueId);
        Assert.Equal("progressive", remaining[1].LInflectionFeatures[0].LFeatureValueId);
    }

    [Fact]
    public void TranslationPositionsComeFromListOrderAndIdentifiersSurviveAnUpdate()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LExampleArchive examples = new(workspace.TWorkspaceDatabase);

        LExample stored = examples.LExampleCreate(new LExample(
            string.Empty, "en", "a sentence", null, null,
            [
                new LTranslation(string.Empty, "ko", "first", 7),
                new LTranslation(string.Empty, "ja", "second", 7),
            ]));

        Assert.Equal([0, 1], stored.LExampleTranslations.Select(t => t.LTranslationPosition));

        string kept = stored.LExampleTranslations[0].LTranslationId;
        examples.LExampleUpdate(stored with { LExampleText = "a changed sentence" });

        LExample? read = examples.LExampleRead(stored.LExampleId);
        Assert.NotNull(read);
        Assert.Equal("a changed sentence", read.LExampleText);
        Assert.Equal(kept, read.LExampleTranslations[0].LTranslationId);
    }

    [Fact]
    public void IdentifiersAreDistinctAndLongEnoughToStayThatWay()
    {
        HashSet<string> identifiers = [];
        for (int count = 0; count < 5000; count++)
        {
            string identifier = LIdentity.LIdentityCreate();
            Assert.Equal(12, identifier.Length);
            Assert.True(identifiers.Add(identifier));
        }
    }

    // The stored positions of one referrer's association rows, in order.
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
