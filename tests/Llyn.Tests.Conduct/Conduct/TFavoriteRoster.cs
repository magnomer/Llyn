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
    public void FavoriteRowsRead_FreshArea_ListsTheStoredFavorites()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TFavoriteSave(engine, "stone", "English");
        CFavorite favorite = CFavorite.CFavoriteCreate(
            atelier, static () => true, TEnvoyFake.TEnvoyCreate(false, []), static run => run());

        Assert.Single(favorite.CFavoriteRowsRead());
        Assert.False(favorite.CFavoritePanel.CPanelAperture.CApertureFiltered);
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

        favorite.CFavoritePanel.CPanelAperture.CApertureQuerySet("riv");

        Assert.Equal(["river"], favorite.CFavoriteRowsRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public void FavoriteOrderSet_NoOrderRow_KeepsTheOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CFavorite favorite = TFavoriteRosterPrepare(atelier);

        favorite.CFavoritePanel.CPanelAperture.CApertureOrderSet(CCatalogOrder.CCatalogOrderReverse);
        favorite.CFavoritePanel.CPanelAperture.CApertureOrderSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderReverse, favorite.CFavoritePanel.CPanelAperture.CApertureOrder);
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

        favorite.CFavoritePanel.CPanelAperture.CApertureFilterSet(new CCatalogFilter(["English"]));

        Assert.True(favorite.CFavoritePanel.CPanelAperture.CApertureFiltered);
        Assert.Equal(["English"], favorite.CFavoritePanel.CPanelAperture.CApertureFilter.CCatalogFilterHidden);
        Assert.Equal(["maison"], favorite.CFavoriteRowsRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public void FavoriteGraspResonate_GraspOrder_RefreshesTheRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry stone = TFavoriteSave(engine, "stone", "English");
        CFavorite favorite = TFavoriteRosterPrepare(atelier);
        int refreshed = 0;
        favorite.CFavoritePanel.CPanelAperture.CApertureRowsChanged += () => refreshed++;

        engine.TEngineBulletinRaise(LSubject.LSubjectGrasp, stone.LEntryId);

        Assert.Equal(0, refreshed);

        favorite.CFavoritePanel.CPanelAperture.CApertureOrderSet(CCatalogOrder.CCatalogOrderGrasp);
        refreshed = 0;
        engine.TEngineBulletinRaise(LSubject.LSubjectGrasp, stone.LEntryId);

        Assert.Equal(1, refreshed);
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
        Assert.Equal("stone", favorite.CFavoriteEditor.TEditorDraftRead()?.CEntryDraftHeadword);

        favorite.CFavoritePanel.CPanelEntryClose();

        Assert.Null(favorite.CFavoriteEditor.CEditorDesk.CDeskStoredRead());
    }

    internal static CFavorite TFavoriteRosterPrepare(CAtelier atelier)
    {
        CFavorite favorite = CFavorite.CFavoriteCreate(
            atelier, static () => true, TEnvoyFake.TEnvoyCreate(false, []), static run => run());
        return favorite;
    }

    internal static LEntry TFavoriteSave(LEngine engine, string headword, string language)
    {
        LEntry entry = TFavoriteEntrySave(engine, headword, language);
        engine.TEngineFavoriteSave(entry.LEntryId);
        return entry;
    }

    internal static LEntry TFavoriteEntrySave(LEngine engine, string headword, string language)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, language, string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
    }
}
