using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TLibraryVista
{
    [Fact]
    public async Task LibraryRowsLoad_StoredEntry_AnswersTheRowsAndTheLanguagesAfterTheFill()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TLibrary.TLibraryEntrySave(engine, "stone", "English");
        CLibrary library = TLibrary.TLibraryPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        CEnsignSheet<IReadOnlyList<CVistaRow>> sheet =
            await library.CLibraryRowsLoad(static (_, _) => static () => { });

        Assert.Equal(["stone"], sheet.CEnsignSheetRows.Select(static row => row.CVistaRowHeadword));
        Assert.Equal(atelier.CAtelierCatalog.CCatalogLanguageRead(), sheet.CEnsignSheetLanguages);
    }

    [Fact]
    public void LibraryVistaRestore_QueryHeld_CarriesItIntoTheFreshVista()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TLibrary.TLibraryEntrySave(engine, "stone", "English");
        TLibrary.TLibraryEntrySave(engine, "river", "English");
        CLibrary library = TLibrary.TLibraryPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        library.CLibraryQuerySet("riv");

        library.TLibraryVistaRestore();

        Assert.Equal(["river"], library.CLibraryRowsRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public void LibraryVistaRestore_ReflexNoticeAfterASecondRestore_RaisesTheRowsOnceThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        int marshalled = 0;
        CLibrary library = CLibrary.CLibraryCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        library.TLibraryVistaRestore();
        int rows = 0;
        library.CLibraryPanel.CPanelRowsChanged += () => rows++;

        engine.TEngineBulletinRaise(LSubject.LSubjectReflex, 0);

        Assert.Equal(1, rows);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void LibraryCreate_ReflexNoticeWithoutARestore_RaisesTheRowsOnceThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        int marshalled = 0;
        CLibrary library = CLibrary.CLibraryCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        int rows = 0;
        library.CLibraryPanel.CPanelRowsChanged += () => rows++;

        engine.TEngineBulletinRaise(LSubject.LSubjectReflex, 0);

        Assert.Equal(1, rows);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void LibraryVistaRestore_WorkspaceNotice_ClosesTheChosenEntryAndTellsTheDriver()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry stone = TLibrary.TLibraryEntrySave(engine, "stone", "English");
        CLibrary library = TLibrary.TLibraryPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        library.CLibraryPanel.CPanelRowOpen(stone.LEntryId);
        Assert.True(library.CLibraryPanel.CPanelBinEnabled);
        int told = 0;
        library.CLibraryWorkspaceChanged += () => told++;

        engine.TEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);

        Assert.False(library.CLibraryPanel.CPanelBinEnabled);
        Assert.Equal(1, told);
    }

    [Fact]
    public void LibraryVistaRestore_SettingsNotice_RaisesTheRowsThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CLibrary library = TLibrary.TLibraryPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        int rows = 0;
        library.CLibraryPanel.CPanelRowsChanged += () => rows++;

        engine.TEngineBulletinRaise(LSubject.LSubjectSettings, 0);

        Assert.Equal(1, rows);
    }

    [Fact]
    public void LibraryOrderRead_Menu_OffersTheFourOrderings()
    {
        Assert.Equal(
            [
                CCatalogOrder.CCatalogOrderHeadword,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderRecent,
                CCatalogOrder.CCatalogOrderEarliest,
            ],
            CLibrary.CLibraryOrderRead());
    }

    [Fact]
    public void LibraryClose_ExitGate_ClosesTheEditorAndStopsTheRecording()
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
        LEntry stone = TLibrary.TLibraryEntrySave(engine, "stone", "English");
        CLibrary library = TLibrary.TLibraryPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        library.CLibraryPanel.CPanelRowOpen(stone.LEntryId);
        library.CLibraryPanel.CPanelScribeToggle(true);
        Assert.True(library.CLibraryEditor.CEditorDesk.CDeskHeld);

        atelier.CAtelierClose();

        Assert.False(library.CLibraryEditor.CEditorDesk.CDeskHeld);
        Assert.Equal(1, stopped);
    }
}
