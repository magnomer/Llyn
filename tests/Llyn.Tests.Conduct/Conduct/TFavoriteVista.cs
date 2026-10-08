using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TFavoriteVista
{
    [Fact]
    public async Task FavoriteRowsLoad_MarkedEntry_AnswersTheRowsAndTheLanguagesAfterTheFill()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TFavoriteRoster.TFavoriteSave(engine, "stone", "English");
        CFavorite favorite = TFavoriteRoster.TFavoriteRosterPrepare(atelier);

        CEnsignSheet<IReadOnlyList<CVistaRow>> sheet =
            await favorite.CFavoriteRowsLoad(static (_, _) => static () => { });

        Assert.Equal(["stone"], sheet.CEnsignSheetRows.Select(static row => row.CVistaRowHeadword));
        Assert.Equal(atelier.CAtelierCatalog.CCatalogLanguageRead(), sheet.CEnsignSheetLanguages);
    }

    [Fact]
    public void FavoriteVistaRestore_QueryHeld_CarriesItIntoTheFreshVista()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TFavoriteRoster.TFavoriteSave(engine, "stone", "English");
        TFavoriteRoster.TFavoriteSave(engine, "river", "English");
        CFavorite favorite = TFavoriteRoster.TFavoriteRosterPrepare(atelier);
        favorite.CFavoritePanel.CPanelAperture.CApertureQuerySet("riv");

        favorite.TFavoriteVistaRestore();

        Assert.Equal(["river"], favorite.CFavoriteRowsRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public void FavoriteVistaRestore_FavoriteNoticeAfterASecondRestore_RaisesTheRowsOnceThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry stone = TFavoriteRoster.TFavoriteSave(engine, "stone", "English");
        int marshalled = 0;
        CFavorite favorite = CFavorite.CFavoriteCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        favorite.TFavoriteVistaRestore();
        int rows = 0;
        favorite.CFavoritePanel.CPanelAperture.CApertureRowsChanged += () => rows++;

        engine.TEngineBulletinRaise(LSubject.LSubjectFavorite, stone.LEntryId);

        Assert.Equal(1, rows);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void FavoriteCreate_FavoriteNoticeWithoutARestore_RaisesTheRowsOnceThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry stone = TFavoriteRoster.TFavoriteSave(engine, "stone", "English");
        int marshalled = 0;
        CFavorite favorite = CFavorite.CFavoriteCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        int rows = 0;
        favorite.CFavoritePanel.CPanelAperture.CApertureRowsChanged += () => rows++;

        engine.TEngineBulletinRaise(LSubject.LSubjectFavorite, stone.LEntryId);

        Assert.Equal(1, rows);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void FavoriteVistaRestore_WorkspaceNotice_ClosesTheChosenEntryAndTellsTheDriver()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry stone = TFavoriteRoster.TFavoriteSave(engine, "stone", "English");
        CFavorite favorite = TFavoriteRoster.TFavoriteRosterPrepare(atelier);
        favorite.CFavoritePanel.CPanelRowOpen(stone.LEntryId);
        Assert.True(favorite.CFavoritePanel.CPanelBinEnabled);
        int told = 0;
        favorite.CFavoriteWorkspaceChanged += () => told++;

        engine.TEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);

        Assert.False(favorite.CFavoritePanel.CPanelBinEnabled);
        Assert.Equal(1, told);
    }

    [Fact]
    public void FavoriteVistaRestore_SettingsNotice_RaisesTheRowsThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CFavorite favorite = TFavoriteRoster.TFavoriteRosterPrepare(atelier);
        int rows = 0;
        favorite.CFavoritePanel.CPanelAperture.CApertureRowsChanged += () => rows++;

        engine.TEngineBulletinRaise(LSubject.LSubjectSettings, 0);

        Assert.Equal(1, rows);
    }

    [Fact]
    public void FavoriteRowsRead_EngineFails_ShowsTheLoadFailureAndAnswersNoRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];

        Assert.Empty(TInterfaceMention.TFavoriteFailRead(engine, TEnvoyFake.TEnvoyCreate(false, asked)));
        Assert.Equal(["Favorite.LoadFailed"], asked);
    }

    [Fact]
    public void FavoriteOrderRead_Menu_OffersTheFiveOrderings()
    {
        Assert.Equal(
            [
                CCatalogOrder.CCatalogOrderHeadword,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderLanguage,
                CCatalogOrder.CCatalogOrderMarked,
                CCatalogOrder.CCatalogOrderGrasp,
            ],
            CFavorite.CFavoriteOrderRead());
    }

    [Fact]
    public void FavoriteClose_ExitGate_ClosesTheEditorAndStopsTheRecording()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        int stopped = 0;
        LMediaPort media = TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineRecordingStop"] = _ =>
            {
                stopped++;
                return null;
            },
        });
        using CAtelier atelier = TInterfaceConduct.TAtelierMediaCreate(engine, media);
        LEntry stone = TFavoriteRoster.TFavoriteSave(engine, "stone", "English");
        CFavorite favorite = TFavoriteRoster.TFavoriteRosterPrepare(atelier);
        favorite.CFavoritePanel.CPanelRowOpen(stone.LEntryId);
        favorite.CFavoritePanel.CPanelScribeToggle(true);
        Assert.True(favorite.CFavoriteEditor.CEditorDesk.CDeskHeld);

        atelier.CAtelierClose();

        Assert.False(favorite.CFavoriteEditor.CEditorDesk.CDeskHeld);
        Assert.Equal(1, stopped);
    }
}
