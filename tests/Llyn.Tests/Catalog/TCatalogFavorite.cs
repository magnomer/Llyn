using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCatalogFavorite
{
    [Fact]
    public void FavoriteFind_HeadwordOrder_ReturnsAlphabetical()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogFavoriteSave(engine, "stone", "English");
        TCatalogFavoriteSave(engine, "apple", "English");

        Assert.Equal(
            ["apple", "stone"],
            engine.TEngineFavoriteFind(string.Empty, LCatalogOrder.LCatalogOrderHeadword)
                .Select(favorite => favorite.LFavoriteEntry.LEntryHeadword));
    }

    [Fact]
    public void FavoriteFind_ReverseOrder_ReturnsReverseAlphabetical()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogFavoriteSave(engine, "apple", "English");
        TCatalogFavoriteSave(engine, "stone", "English");

        Assert.Equal(
            ["stone", "apple"],
            engine.TEngineFavoriteFind(string.Empty, LCatalogOrder.LCatalogOrderReverse)
                .Select(favorite => favorite.LFavoriteEntry.LEntryHeadword));
    }

    [Fact]
    public void FavoriteFind_LanguageOrder_ReturnsLanguageGrouped()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogFavoriteSave(engine, "apple", "Korean");
        TCatalogFavoriteSave(engine, "stone", "English");

        Assert.Equal(
            ["English", "Korean"],
            engine.TEngineFavoriteFind(string.Empty, LCatalogOrder.LCatalogOrderLanguage)
                .Select(favorite => favorite.LFavoriteEntry.LEntryLanguage));
    }

    [Fact]
    public void FavoriteFind_MarkedOrder_ReturnsNewestMarkFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TCatalogFavoriteSave(engine, "apple", "English");
        TCatalogFavoriteSave(engine, "stone", "English");

        Assert.Equal(
            ["stone", "apple"],
            engine.TEngineFavoriteFind(string.Empty, LCatalogOrder.LCatalogOrderMarked)
                .Select(favorite => favorite.LFavoriteEntry.LEntryHeadword));
    }

    private static void TCatalogFavoriteSave(LEngine engine, string headword, string language)
    {
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty,
                string.Empty,
                "a meaning",
                [],
                [],
                [],
                [],
                [],
                1)],
            []));

        engine.TEngineFavoriteSave(entry.LEntryId);
    }
}
