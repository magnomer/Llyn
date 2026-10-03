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

        LEntry entry = TTagEntryCreate(engine, ["formal", "rare"], ["formal"]);
        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;

        Assert.Equal(
            ["formal", "rare"],
            TInterface.TTagDraftRead(loaded.LEntryDraftMeanings[0].LCardDraftTag));
        Assert.Equal(
            ["formal"],
            TInterface.TTagDraftRead(loaded.LEntryDraftCollocations[0].LCardDraftTag));
        Assert.Equal(
            ["formal", "rare"],
            engine.TEngineTagRead().Select(tag => tag.LTagText));
    }

    [Fact]
    public void TagSave_SpacingAndPunctuation_ReadsBackAsWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine, ["  This is a new text  ", "chiefly British", "   "], []);

        Assert.Equal(
            ["This is a new text", "chiefly British"],
            TInterface.TTagDraftRead(engine.TEngineEntryLoad(entry.LEntryId)!.LEntryDraftMeanings[0].LCardDraftTag));
    }

    [Fact]
    public void TagSave_SameTextTwiceOnOneCard_KeepsOneTag()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine, ["formal", "formal", "rare"], []);
        LCardDraft meaning = engine.TEngineEntryLoad(entry.LEntryId)!.LEntryDraftMeanings[0];

        Assert.Equal(["formal", "rare"], TInterface.TTagDraftRead(meaning.LCardDraftTag));
        Assert.Equal(
            [0, 1],
            TDatabasePositionRead(workspace, "sense_tag", "sense_parent", meaning.LCardDraftId));
    }

    [Fact]
    public void TagCreate_WordingNoCardCarries_ListsItInTheCatalog()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LTag created = engine.TEngineTagCreate("  chiefly British  ");

        Assert.Equal("chiefly British", created.LTagText);
        Assert.True(created.LTagId > 0);
        Assert.Equal("chiefly British", Assert.Single(engine.TEngineTagRead()).LTagText);
        Assert.Empty(engine.TEngineEntryFind(created));
    }

    [Fact]
    public void TagCreate_WordingAlreadyStored_ReturnsTheStoredRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LTag first = engine.TEngineTagCreate("formal");
        LTag again = engine.TEngineTagCreate("formal");

        Assert.Equal(first.LTagId, again.LTagId);
        Assert.Single(engine.TEngineTagRead());
    }

    [Fact]
    public void TagSave_TwoCardsSameText_LinkOneRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine, ["formal"], ["formal"]);
        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM tag;"));
        Assert.Equal(
            Assert.Single(loaded.LEntryDraftMeanings[0].LCardDraftTag).LTagDraftId,
            Assert.Single(loaded.LEntryDraftCollocations[0].LCardDraftTag).LTagDraftId);
    }

    [Fact]
    public void TagSave_KnownId_LinksThatRowWhateverTheText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine, ["formal"], []);
        long formal = TTagIdRead(engine, "formal");
        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;
        LCardDraft collocation = loaded.LEntryDraftCollocations[0] with
        {
            LCardDraftTag = [TInterface.TTagDraftCreate("stale")[0] with { LTagDraftId = formal }],
        };

        engine.TEngineEntryUpdate(entry.LEntryId, loaded with { LEntryDraftCollocations = [collocation] });

        Assert.Equal(
            "formal",
            Assert.Single(engine.TEngineEntryLoad(entry.LEntryId)!.LEntryDraftCollocations[0].LCardDraftTag)
                .LTagDraftText);
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM tag;"));
    }

    [Fact]
    public void TagSave_DroppedFromEveryCard_KeepsRowInCatalog()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TTagEntryCreate(engine, ["formal"], []);
        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;
        LCardDraft meaning = loaded.LEntryDraftMeanings[0] with { LCardDraftTag = [] };

        engine.TEngineEntryUpdate(entry.LEntryId, loaded with { LEntryDraftMeanings = [meaning] });

        Assert.Equal(["formal"], engine.TEngineTagRead().Select(tag => tag.LTagText));
        Assert.Empty(engine.TEngineEntryFind(TInterface.TTagCreate(TTagIdRead(engine, "formal"), "formal")));
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

    private static LEntry TTagEntryCreate(
        LEngine engine, IReadOnlyList<string> meaningTags, IReadOnlyList<string> collocationTags)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], meaningTags, [], 1)],
            [TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], collocationTags, [], 1)]));
    }
}
