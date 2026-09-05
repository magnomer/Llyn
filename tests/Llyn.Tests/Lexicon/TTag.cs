using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTag
{
    [Fact]
    public void ATagIsItsOwnTextAndTheSameTextReadsBackFromBothCardKinds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine);
        string senseId = engine.TEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        string collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        engine.TEngineTagSave(
            senseId, [TInterface.TTagCreate("formal"), TInterface.TTagCreate("rare")], LOwner.LOwnerSense);
        engine.TEngineTagSave(
            collocationId, [TInterface.TTagCreate("formal")], LOwner.LOwnerCollocation);

        Assert.Equal(
            ["formal", "rare"],
            engine.TEngineTagRead(senseId, LOwner.LOwnerSense).Select(tag => tag.LTagText));
        Assert.Equal(
            "formal",
            Assert.Single(engine.TEngineTagRead(collocationId, LOwner.LOwnerCollocation)).LTagText);
        Assert.Equal(
            ["formal", "rare"],
            engine.TEngineTagRead().Select(tag => tag.LTagText));
    }

    [Fact]
    public void ATagHoldsItsSpacingAndPunctuationExactlyAsItWasWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine);
        string senseId = engine.TEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;

        engine.TEngineTagSave(
            senseId,
            [TInterface.TTagCreate("  This is a new text  "), TInterface.TTagCreate("chiefly British"), TInterface.TTagCreate("   ")],
            LOwner.LOwnerSense);

        Assert.Equal(
            ["This is a new text", "chiefly British"],
            engine.TEngineTagRead(senseId, LOwner.LOwnerSense).Select(tag => tag.LTagText));
    }

    [Fact]
    public void WritingTheSameTextTwiceUnderOneCardKeepsOneTag()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine);
        string senseId = engine.TEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;

        engine.TEngineTagSave(
            senseId,
            [TInterface.TTagCreate("formal"), TInterface.TTagCreate("formal"), TInterface.TTagCreate("rare")],
            LOwner.LOwnerSense);

        Assert.Equal(
            ["formal", "rare"],
            engine.TEngineTagRead(senseId, LOwner.LOwnerSense).Select(tag => tag.LTagText));
        Assert.Equal(
            [0, 1],
            TDatabasePositionRead(workspace, "sense_tag", "sense_id", senseId));
    }

    [Fact]
    public void RenamingATagCarriesEveryCardThatWroteItAndFoldsAClash()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine);
        string senseId = engine.TEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        string collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        engine.TEngineTagSave(
            senseId, [TInterface.TTagCreate("formal"), TInterface.TTagCreate("rare")], LOwner.LOwnerSense);
        engine.TEngineTagSave(
            collocationId, [TInterface.TTagCreate("formal")], LOwner.LOwnerCollocation);

        engine.TEngineTagChange("formal", "rare");

        Assert.Equal(
            ["rare"],
            engine.TEngineTagRead(senseId, LOwner.LOwnerSense).Select(tag => tag.LTagText));
        Assert.Equal(
            ["rare"],
            engine.TEngineTagRead(collocationId, LOwner.LOwnerCollocation).Select(tag => tag.LTagText));
        Assert.Equal([0], TDatabasePositionRead(workspace, "sense_tag", "sense_id", senseId));
    }

    [Fact]
    public void DeletingATagTakesItOffEveryCardAndClosesTheGapItLeaves()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine);
        string senseId = engine.TEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;

        engine.TEngineTagSave(
            senseId,
            [TInterface.TTagCreate("formal"), TInterface.TTagCreate("rare"), TInterface.TTagCreate("spoken")],
            LOwner.LOwnerSense);

        engine.TEngineTagDelete("rare");

        Assert.Equal(
            ["formal", "spoken"],
            engine.TEngineTagRead(senseId, LOwner.LOwnerSense).Select(tag => tag.LTagText));
        Assert.Equal([0, 1], TDatabasePositionRead(workspace, "sense_tag", "sense_id", senseId));
        Assert.Empty(engine.TEngineTagRead().Where(tag => tag.LTagText == "rare"));
    }

    [Fact]
    public void ASideATagNeverHangsFromIsRefusedRatherThanGuessedAt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            engine.TEngineTagSave(entry.LEntryId, [TInterface.TTagCreate("formal")], LOwner.LOwnerEntry));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            engine.TEngineTagRead(entry.LEntryId, LOwner.LOwnerEntry));
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
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], string.Empty, [], [], 1)],
            [TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], string.Empty, [], [], 1)]));
    }
}
