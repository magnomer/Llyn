using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

public sealed class TTag
{
    [Fact]
    public void ATagIsItsOwnTextAndTheSameTextReadsBackFromBothCardKinds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TTagEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        string collocationId =
            engine.LEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        engine.LEngineTagSave(
            senseId, [new LTag("formal"), new LTag("rare")], LOwner.LOwnerSense);
        engine.LEngineTagSave(
            collocationId, [new LTag("formal")], LOwner.LOwnerCollocation);

        Assert.Equal(
            ["formal", "rare"],
            engine.LEngineTagRead(senseId, LOwner.LOwnerSense).Select(tag => tag.LTagText));
        Assert.Equal(
            "formal",
            Assert.Single(engine.LEngineTagRead(collocationId, LOwner.LOwnerCollocation)).LTagText);
        Assert.Equal(
            ["formal", "rare"],
            engine.LEngineTagRead().Select(tag => tag.LTagText));
    }

    [Fact]
    public void ATagHoldsItsSpacingAndPunctuationExactlyAsItWasWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TTagEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;

        engine.LEngineTagSave(
            senseId,
            [new LTag("  This is a new text  "), new LTag("chiefly British"), new LTag("   ")],
            LOwner.LOwnerSense);

        Assert.Equal(
            ["This is a new text", "chiefly British"],
            engine.LEngineTagRead(senseId, LOwner.LOwnerSense).Select(tag => tag.LTagText));
    }

    [Fact]
    public void WritingTheSameTextTwiceUnderOneCardKeepsOneTag()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TTagEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;

        engine.LEngineTagSave(
            senseId,
            [new LTag("formal"), new LTag("formal"), new LTag("rare")],
            LOwner.LOwnerSense);

        Assert.Equal(
            ["formal", "rare"],
            engine.LEngineTagRead(senseId, LOwner.LOwnerSense).Select(tag => tag.LTagText));
        Assert.Equal(
            [0, 1],
            TDatabasePositionRead(workspace, "sense_tag", "sense_id", senseId));
    }

    [Fact]
    public void RenamingATagCarriesEveryCardThatWroteItAndFoldsAClash()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TTagEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        string collocationId =
            engine.LEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        engine.LEngineTagSave(
            senseId, [new LTag("formal"), new LTag("rare")], LOwner.LOwnerSense);
        engine.LEngineTagSave(
            collocationId, [new LTag("formal")], LOwner.LOwnerCollocation);

        engine.LEngineTagChange("formal", "rare");

        Assert.Equal(
            ["rare"],
            engine.LEngineTagRead(senseId, LOwner.LOwnerSense).Select(tag => tag.LTagText));
        Assert.Equal(
            ["rare"],
            engine.LEngineTagRead(collocationId, LOwner.LOwnerCollocation).Select(tag => tag.LTagText));
        Assert.Equal([0], TDatabasePositionRead(workspace, "sense_tag", "sense_id", senseId));
    }

    [Fact]
    public void DeletingATagTakesItOffEveryCardAndClosesTheGapItLeaves()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TTagEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;

        engine.LEngineTagSave(
            senseId,
            [new LTag("formal"), new LTag("rare"), new LTag("spoken")],
            LOwner.LOwnerSense);

        engine.LEngineTagDelete("rare");

        Assert.Equal(
            ["formal", "spoken"],
            engine.LEngineTagRead(senseId, LOwner.LOwnerSense).Select(tag => tag.LTagText));
        Assert.Equal([0, 1], TDatabasePositionRead(workspace, "sense_tag", "sense_id", senseId));
        Assert.Empty(engine.LEngineTagRead().Where(tag => tag.LTagText == "rare"));
    }

    [Fact]
    public void ASideATagNeverHangsFromIsRefusedRatherThanGuessedAt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TTagEntryCreate(engine);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            engine.LEngineTagSave(entry.LEntryId, [new LTag("formal")], LOwner.LOwnerEntry));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            engine.LEngineTagRead(entry.LEntryId, LOwner.LOwnerEntry));
    }

    private static IReadOnlyList<long> TDatabasePositionRead(
        TWorkspace workspace, string table, string column, string ownerId)
    {
        using Microsoft.Data.Sqlite.SqliteConnection connection = workspace.TWorkspaceConnectionRead();
        using Microsoft.Data.Sqlite.SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"SELECT position FROM {table} WHERE {column} = $owner ORDER BY position;";
        command.Parameters.AddWithValue("$owner", ownerId);

        List<long> positions = [];
        using Microsoft.Data.Sqlite.SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            positions.Add(reader.GetInt64(0));
        }

        return positions;
    }

    private static LEntry TTagEntryCreate(LEngine engine)
    {
        return engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(string.Empty, string.Empty, "a meaning", [], [], [], string.Empty, [], [], 1)],
            [new LCardDraft(string.Empty, "in a word", "briefly", [], [], [], string.Empty, [], [], 1)]));
    }
}
