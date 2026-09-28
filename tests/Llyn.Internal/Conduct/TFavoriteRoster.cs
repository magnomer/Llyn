using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TFavoriteRoster
{
    [Fact]
    public void FavoriteRowsRead_NoVista_ReadsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TFavoriteSave(engine, "stone", "English");
        CFavorite favorite = CFavorite.CFavoriteCreate(
            atelier, static () => true, TInterfaceConduct.TEnvoyCreate(false, []));

        Assert.Empty(favorite.CFavoriteRowsRead());
        Assert.False(favorite.CFavoriteFiltered);
        Assert.Equal("entry", favorite.TFavoriteFileRead());
    }

    [Fact]
    public void FavoriteRowsRead_MarkedEntry_ListsOnlyTheMarked()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry stone = TFavoriteSave(engine, "stone", "English");
        TFavoriteEntrySave(engine, "apple", "English");
        CFavorite favorite = TFavoriteRosterPrepare(atelier);

        Assert.Equal(stone.LEntryId, Assert.Single(favorite.CFavoriteRowsRead()).CVistaRowId);
    }

    [Fact]
    public void FavoriteQuerySet_MatchingHeadword_NarrowsTheRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TFavoriteSave(engine, "stone", "English");
        TFavoriteSave(engine, "river", "English");
        CFavorite favorite = TFavoriteRosterPrepare(atelier);

        favorite.CFavoriteQuerySet("riv");

        Assert.Equal(["river"], favorite.CFavoriteRowsRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public void FavoriteOrderSet_NoOrderRow_KeepsTheOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CFavorite favorite = TFavoriteRosterPrepare(atelier);

        favorite.CFavoriteOrderSet(CCatalogOrder.CCatalogOrderReverse);
        favorite.CFavoriteOrderSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderReverse, favorite.CFavoritePanel.CPanelOrder);
    }

    [Fact]
    public void FavoriteFilterSet_HiddenLanguage_MarksTheListFiltered()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TFavoriteSave(engine, "stone", "English");
        TFavoriteSave(engine, "maison", "French");
        CFavorite favorite = TFavoriteRosterPrepare(atelier);

        favorite.CFavoriteFilterSet(new CCatalogFilter(["English"]));

        Assert.True(favorite.CFavoriteFiltered);
        Assert.Equal(["English"], favorite.CFavoritePanel.CPanelFilter.CCatalogFilterHidden);
        Assert.Equal(["maison"], favorite.CFavoriteRowsRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public void FavoriteGraspResonate_GraspOrder_RefreshesTheRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CFavorite favorite = TFavoriteRosterPrepare(atelier);
        int refreshed = 0;
        favorite.CFavoritePanel.CPanelRowsChanged += () => refreshed++;

        favorite.CFavoriteGraspResonate();

        Assert.Equal(0, refreshed);

        favorite.CFavoriteOrderSet(CCatalogOrder.CCatalogOrderGrasp);
        favorite.CFavoriteGraspResonate();

        Assert.Equal(1, refreshed);
    }

    [Fact]
    public void FavoriteLanguageRead_Workspace_OffersTheCatalogLanguages()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CFavorite favorite = TFavoriteRosterPrepare(atelier);

        Assert.Equal(atelier.CAtelierCatalog.CCatalogLanguageRead(), favorite.CFavoriteLanguageRead());
    }

    [Fact]
    public void FavoritePanelRowOpen_ScribeOn_OpensTheEntryInTheEditor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry stone = TFavoriteSave(engine, "stone", "English");
        CFavorite favorite = TFavoriteRosterPrepare(atelier);

        favorite.CFavoritePanel.CPanelRowOpen(stone.LEntryId);

        Assert.Equal("stone", favorite.TFavoriteFileRead());

        favorite.CFavoritePanel.CPanelScribeToggle(true);

        Assert.Equal(stone.LEntryId, favorite.CFavoriteEditor.CEditorDesk.CDeskStoredRead());
        Assert.Equal("stone", favorite.CFavoriteEditor.CEditorDraftRead()?.CEntryDraftHeadword);

        favorite.CFavoritePanel.CPanelEntryClose();

        Assert.Null(favorite.CFavoriteEditor.CEditorDesk.CDeskStoredRead());
    }

    private static CFavorite TFavoriteRosterPrepare(CAtelier atelier)
    {
        CFavorite favorite = CFavorite.CFavoriteCreate(
            atelier, static () => true, TInterfaceConduct.TEnvoyCreate(false, []));
        favorite.CFavoriteVistaRestore();
        return favorite;
    }

    private static LEntry TFavoriteSave(LEngine engine, string headword, string language)
    {
        LEntry entry = TFavoriteEntrySave(engine, headword, language);
        engine.TEngineFavoriteSave(entry.LEntryId);
        return entry;
    }

    private static LEntry TFavoriteEntrySave(LEngine engine, string headword, string language)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, language, string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
    }
}
