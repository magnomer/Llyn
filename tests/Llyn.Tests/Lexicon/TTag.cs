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
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        long collocationId =
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
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;

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
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;

        engine.TEngineTagSave(
            meaningId,
            [TInterface.TTagCreate("formal"), TInterface.TTagCreate("formal"), TInterface.TTagCreate("rare")],
            LOwner.LOwnerMeaning);

        Assert.Equal(
            ["formal", "rare"],
            engine.TEngineTagRead(meaningId, LOwner.LOwnerMeaning).Select(tag => tag.LTagText));
        Assert.Equal(
            [0, 1],
            TDatabasePositionRead(workspace, "sense_tag", "sense_parent", meaningId));
    }

    [Fact]
    public void TagChange_RenamedTag_CarriesCardsAndFoldsClash()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        long collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        engine.TEngineTagSave(
            meaningId, [TInterface.TTagCreate("formal"), TInterface.TTagCreate("rare")], LOwner.LOwnerMeaning);
        engine.TEngineTagSave(
            collocationId, [TInterface.TTagCreate("formal")], LOwner.LOwnerCollocation);

        engine.TEngineTagChange(TTagIdRead(engine, "formal"), "rare");

        Assert.Equal(
            ["rare"],
            engine.TEngineTagRead(meaningId, LOwner.LOwnerMeaning).Select(tag => tag.LTagText));
        Assert.Equal(
            ["rare"],
            engine.TEngineTagRead(collocationId, LOwner.LOwnerCollocation).Select(tag => tag.LTagText));
        Assert.Equal([0], TDatabasePositionRead(workspace, "sense_tag", "sense_parent", meaningId));
        Assert.Equal(["rare"], engine.TEngineTagRead().Select(tag => tag.LTagText));
    }

    [Fact]
    public void TagChange_RenamedTag_KeepsIdAndFollowsOnEveryCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        long collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        engine.TEngineTagSave(
            meaningId, [TInterface.TTagCreate("formal"), TInterface.TTagCreate("rare")], LOwner.LOwnerMeaning);
        engine.TEngineTagSave(
            collocationId, [TInterface.TTagCreate("formal")], LOwner.LOwnerCollocation);

        long formal = TTagIdRead(engine, "formal");
        engine.TEngineTagChange(formal, "literary");

        Assert.Equal(formal, TTagIdRead(engine, "literary"));
        Assert.Equal(
            ["literary", "rare"],
            engine.TEngineTagRead(meaningId, LOwner.LOwnerMeaning).Select(tag => tag.LTagText));
        Assert.Equal(
            ["literary"],
            engine.TEngineTagRead(collocationId, LOwner.LOwnerCollocation).Select(tag => tag.LTagText));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LTagDraft chip = Assert.Single(loaded.LEntryDraftCollocations)
            .LCardDraftTag.Single();
        Assert.Equal(formal, chip.LTagDraftId);
        Assert.Equal("literary", chip.LTagDraftText);
    }

    [Fact]
    public void TagSave_TwoCardsSameText_LinkOneRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        long collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        engine.TEngineTagSave(meaningId, [TInterface.TTagCreate("formal")], LOwner.LOwnerMeaning);
        engine.TEngineTagSave(collocationId, [TInterface.TTagCreate("formal")], LOwner.LOwnerCollocation);

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM tag;"));
        Assert.Equal(
            Assert.Single(engine.TEngineTagRead(meaningId, LOwner.LOwnerMeaning)).LTagId,
            Assert.Single(engine.TEngineTagRead(collocationId, LOwner.LOwnerCollocation)).LTagId);
    }

    [Fact]
    public void TagSave_KnownId_LinksThatRowWhateverTheText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        long collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        engine.TEngineTagSave(meaningId, [TInterface.TTagCreate("formal")], LOwner.LOwnerMeaning);
        long formal = TTagIdRead(engine, "formal");

        engine.TEngineTagSave(
            collocationId, [TInterface.TTagCreate(formal, "stale")], LOwner.LOwnerCollocation);

        Assert.Equal(
            "formal",
            Assert.Single(engine.TEngineTagRead(collocationId, LOwner.LOwnerCollocation)).LTagText);
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM tag;"));
    }

    [Fact]
    public void TagSave_DroppedFromEveryCard_KeepsRowInCatalog()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;

        engine.TEngineTagSave(meaningId, [TInterface.TTagCreate("formal")], LOwner.LOwnerMeaning);
        engine.TEngineTagSave(meaningId, [], LOwner.LOwnerMeaning);

        Assert.Equal(["formal"], engine.TEngineTagRead().Select(tag => tag.LTagText));
        Assert.Empty(engine.TEngineEntryFind(TInterface.TTagCreate(TTagIdRead(engine, "formal"), "formal")));
    }

    [Fact]
    public void TagDelete_TagOnManyCards_RemovesItAndClosesGap()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;

        engine.TEngineTagSave(
            meaningId,
            [TInterface.TTagCreate("formal"), TInterface.TTagCreate("rare"), TInterface.TTagCreate("spoken")],
            LOwner.LOwnerMeaning);

        engine.TEngineTagDelete(TTagIdRead(engine, "rare"));

        Assert.Equal(
            ["formal", "spoken"],
            engine.TEngineTagRead(meaningId, LOwner.LOwnerMeaning).Select(tag => tag.LTagText));
        Assert.Equal([0, 1], TDatabasePositionRead(workspace, "sense_tag", "sense_parent", meaningId));
        Assert.Empty(engine.TEngineTagRead().Where(tag => tag.LTagText == "rare"));
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM tag;"));
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

    private static long TTagIdRead(LEngine engine, string text)
    {
        return engine.TEngineTagRead().Single(tag => tag.LTagText == text).LTagId;
    }

    private static IReadOnlyList<long> TDatabasePositionRead(
        TWorkspace workspace, string table, string column, long ownerId)
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
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)],
            [TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], [], [], 1)]));
    }
}
