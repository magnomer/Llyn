using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTag
{
    [Fact]
    public void TagSave_SameTextOnBothCardKinds_ReadsBackFromEach()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine);
        string meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        string collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        engine.TEngineTagSave(
            meaningId, [TInterface.TTagCreate("formal"), TInterface.TTagCreate("rare")], LOwner.LOwnerMeaning);
        engine.TEngineTagSave(
            collocationId, [TInterface.TTagCreate("formal")], LOwner.LOwnerCollocation);

        Assert.Equal(
            ["formal", "rare"],
            engine.TEngineTagRead(meaningId, LOwner.LOwnerMeaning).Select(tag => tag.LTagText));
        Assert.Equal(
            "formal",
            Assert.Single(engine.TEngineTagRead(collocationId, LOwner.LOwnerCollocation)).LTagText);
        Assert.Equal(
            ["formal", "rare"],
            engine.TEngineTagRead().Select(tag => tag.LTagText));
    }

    [Fact]
    public void TagSave_SpacingAndPunctuation_ReadsBackAsWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine);
        string meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;

        engine.TEngineTagSave(
            meaningId,
            [TInterface.TTagCreate("  This is a new text  "), TInterface.TTagCreate("chiefly British"), TInterface.TTagCreate("   ")],
            LOwner.LOwnerMeaning);

        Assert.Equal(
            ["This is a new text", "chiefly British"],
            engine.TEngineTagRead(meaningId, LOwner.LOwnerMeaning).Select(tag => tag.LTagText));
    }

    [Fact]
    public void TagSave_SameTextTwiceOnOneCard_KeepsOneTag()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine);
        string meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;

        engine.TEngineTagSave(
            meaningId,
            [TInterface.TTagCreate("formal"), TInterface.TTagCreate("formal"), TInterface.TTagCreate("rare")],
            LOwner.LOwnerMeaning);

        Assert.Equal(
            ["formal", "rare"],
            engine.TEngineTagRead(meaningId, LOwner.LOwnerMeaning).Select(tag => tag.LTagText));
        Assert.Equal(
            [0, 1],
            TDatabasePositionRead(workspace, "sense_tag", "sense_id", meaningId));
    }

    [Fact]
    public void TagChange_RenamedTag_CarriesCardsAndFoldsClash()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine);
        string meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        string collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        engine.TEngineTagSave(
            meaningId, [TInterface.TTagCreate("formal"), TInterface.TTagCreate("rare")], LOwner.LOwnerMeaning);
        engine.TEngineTagSave(
            collocationId, [TInterface.TTagCreate("formal")], LOwner.LOwnerCollocation);

        engine.TEngineTagChange("formal", "rare");

        Assert.Equal(
            ["rare"],
            engine.TEngineTagRead(meaningId, LOwner.LOwnerMeaning).Select(tag => tag.LTagText));
        Assert.Equal(
            ["rare"],
            engine.TEngineTagRead(collocationId, LOwner.LOwnerCollocation).Select(tag => tag.LTagText));
        Assert.Equal([0], TDatabasePositionRead(workspace, "sense_tag", "sense_id", meaningId));
    }

    [Fact]
    public void TagDelete_TagOnManyCards_RemovesItAndClosesGap()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine);
        string meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;

        engine.TEngineTagSave(
            meaningId,
            [TInterface.TTagCreate("formal"), TInterface.TTagCreate("rare"), TInterface.TTagCreate("spoken")],
            LOwner.LOwnerMeaning);

        engine.TEngineTagDelete("rare");

        Assert.Equal(
            ["formal", "spoken"],
            engine.TEngineTagRead(meaningId, LOwner.LOwnerMeaning).Select(tag => tag.LTagText));
        Assert.Equal([0, 1], TDatabasePositionRead(workspace, "sense_tag", "sense_id", meaningId));
        Assert.Empty(engine.TEngineTagRead().Where(tag => tag.LTagText == "rare"));
    }

    [Fact]
    public void TagSave_UnknownSide_Throws()
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
