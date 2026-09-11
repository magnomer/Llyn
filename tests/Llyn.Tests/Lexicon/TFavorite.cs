using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TFavorite
{
    [Fact]
    public void FavoriteSave_MarkedEntry_ReadsBackAndKeepsItsIdentity()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TFavoriteEntryCreate(engine, "word");

        engine.TEngineFavoriteSave(entry.LEntryId);

        Assert.True(engine.TEngineFavoriteCheck(entry.LEntryId));

        LFavorite favorite = Assert.Single(engine.TEngineFavoriteFind(string.Empty));
        Assert.Equal(entry.LEntryId, favorite.LFavoriteEntry.LEntryId);
        Assert.Equal("word", favorite.LFavoriteEntry.LEntryHeadword);
        Assert.NotEqual(string.Empty, favorite.LFavoriteMarkedUtc);
    }

    [Fact]
    public void FavoriteSave_SameEntryTwice_KeepsOneMarkWithTheFirstStamp()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TFavoriteEntryCreate(engine, "word");

        engine.TEngineFavoriteSave(entry.LEntryId);
        string first = Assert.Single(engine.TEngineFavoriteFind(string.Empty)).LFavoriteMarkedUtc;

        engine.TEngineFavoriteSave(entry.LEntryId);

        Assert.Equal(first, Assert.Single(engine.TEngineFavoriteFind(string.Empty)).LFavoriteMarkedUtc);
    }

    [Fact]
    public void FavoriteFind_TypedQuery_ReturnsOnlyTheMarkedEntriesCarryingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry marked = TFavoriteEntryCreate(engine, "water");
        LEntry other = TFavoriteEntryCreate(engine, "stone");
        TFavoriteEntryCreate(engine, "watermark");

        engine.TEngineFavoriteSave(marked.LEntryId);
        engine.TEngineFavoriteSave(other.LEntryId);

        Assert.Equal(
            ["water"],
            engine.TEngineFavoriteFind("wat").Select(favorite => favorite.LFavoriteEntry.LEntryHeadword));
    }

    [Fact]
    public void FavoriteDelete_UnmarkedEntry_LeavesTheEntryInThePlace()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TFavoriteEntryCreate(engine, "word");
        engine.TEngineFavoriteSave(entry.LEntryId);

        engine.TEngineFavoriteDelete(entry.LEntryId);

        Assert.False(engine.TEngineFavoriteCheck(entry.LEntryId));
        Assert.Empty(engine.TEngineFavoriteFind(string.Empty));
        Assert.NotNull(engine.TEngineEntryRead(entry.LEntryId));
    }

    [Fact]
    public void EntryDelete_MarkedEntry_DropsTheMarkWithIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TFavoriteEntryCreate(engine, "word");
        engine.TEngineFavoriteSave(entry.LEntryId);

        engine.TEngineEntryDelete(entry.LEntryId);

        Assert.False(engine.TEngineFavoriteCheck(entry.LEntryId));
        Assert.Empty(engine.TEngineFavoriteFind(string.Empty));
    }

    private static LEntry TFavoriteEntryCreate(LEngine engine, string headword)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], string.Empty, [], [], 1)],
            []));
    }
}
