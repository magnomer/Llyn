using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TLibrary
{
    [Fact]
    public void LibraryRowsRead_FreshArea_ListsTheStoredEntries()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TLibraryEntrySave(engine, "stone", "English");
        CLibrary library = CLibrary.CLibraryCreate(
            atelier, static () => true, TEnvoyFake.TEnvoyCreate(false, []), static run => run());

        Assert.Single(library.CLibraryRowsRead());
        Assert.False(library.CLibraryPanel.CPanelAperture.CApertureFiltered);
        Assert.Equal("entry", library.TLibraryFileRead());
    }

    [Fact]
    public void LibraryQuerySet_MatchingHeadword_NarrowsTheRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TLibraryEntrySave(engine, "stone", "English");
        TLibraryEntrySave(engine, "river", "English");
        CLibrary library = TLibraryPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        Assert.Equal(2, library.CLibraryRowsRead().Count);

        library.CLibraryPanel.CPanelAperture.CApertureQuerySet("riv");

        Assert.Equal(["river"], library.CLibraryRowsRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public void LibraryEmpty_QueryMatchingNothing_AnswersEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TLibraryEntrySave(engine, "stone", "English");
        CLibrary library = TLibraryPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        library.CLibraryRowsRead();
        bool listed = library.CLibraryPanel.CPanelAperture.CApertureEmpty;

        library.CLibraryPanel.CPanelAperture.CApertureQuerySet("river");
        library.CLibraryRowsRead();

        Assert.False(listed);
        Assert.True(library.CLibraryPanel.CPanelAperture.CApertureEmpty);
    }

    [Fact]
    public void LibraryOrderSet_NoOrderRow_KeepsTheOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CLibrary library = TLibraryPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        library.CLibraryPanel.CPanelAperture.CApertureOrderSet(CCatalogOrder.CCatalogOrderReverse);
        library.CLibraryPanel.CPanelAperture.CApertureOrderSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderReverse, library.CLibraryPanel.CPanelAperture.CApertureOrder);
    }

    [Fact]
    public void LibraryFilterSet_HiddenLanguage_MarksTheListFiltered()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TLibraryEntrySave(engine, "stone", "English");
        TLibraryEntrySave(engine, "maison", "French");
        CLibrary library = TLibraryPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        library.CLibraryPanel.CPanelAperture.CApertureFilterSet(new CCatalogFilter(["English"]));

        Assert.True(library.CLibraryPanel.CPanelAperture.CApertureFiltered);
        Assert.Equal(["English"], library.CLibraryPanel.CPanelAperture.CApertureFilter.CCatalogFilterHidden);
        Assert.Equal(["maison"], library.CLibraryRowsRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public void LibraryPanelRowSelect_RowChosen_MarksItAndReportsItsId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TLibraryEntrySave(engine, "water", "English");
        CLibrary library = TLibraryPrepare(atelier, TEnvoyFake.TEnvoyCreate(true, []));

        Assert.Equal(0, library.CLibraryPanel.TPanelChosenRead());

        library.CLibraryPanel.CPanelRowSelect(water.LEntryId);

        Assert.Equal(water.LEntryId, library.CLibraryPanel.TPanelChosenRead());
        Assert.Equal(["water"], library.CLibraryRowsRead()
            .Where(row => row.CVistaRowChosen)
            .Select(row => row.CVistaRowHeadword));
        Assert.Equal("water", library.TLibraryFileRead());

        library.CLibraryPanel.CPanelBin.CPanelBinDelete();

        Assert.Equal(0, library.CLibraryPanel.TPanelChosenRead());
        Assert.Empty(library.CLibraryRowsRead());
    }

    [Fact]
    public void LibraryPanelRowOpen_ScribeOn_OpensTheEntryInTheEditor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry stone = TLibraryEntrySave(engine, "stone", "English");
        CLibrary library = TLibraryPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        library.CLibraryPanel.CPanelRowOpen(stone.LEntryId);
        library.CLibraryPanel.CPanelScribeToggle(true);

        Assert.Equal(stone.LEntryId, library.CLibraryEditor.CEditorDesk.CDeskStoredRead());

        library.CLibraryPanel.CPanelEntryClose();

        Assert.Null(library.CLibraryEditor.CEditorDesk.CDeskStoredRead());
    }

    [Fact]
    public void AtelierQuitConfirm_LibraryEditing_AsksOnceAndStoresTheEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry stone = TLibraryEntrySave(engine, "stone", "English");
        List<string> asked = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(true, asked);
        CLibrary library = TLibraryPrepare(atelier, envoy);
        library.CLibraryPanel.CPanelRowOpen(stone.LEntryId);
        library.CLibraryPanel.CPanelScribeToggle(true);
        library.CLibraryEditor.CEditorEntry.CEntryHeadwordSet("stones");

        bool quit = atelier.CAtelierQuitConfirm(envoy);

        Assert.True(quit);
        Assert.Equal(["Leave"], asked);
        Assert.False(library.CLibraryEditor.CEditorDesk.TDeskChangeCheck());
        Assert.Equal("stones", engine.TEngineEntryRead(stone.LEntryId)!.LEntryHeadword);
    }

    [Fact]
    public async Task LibraryPortraitExport_ChosenEntry_WritesIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TLibraryEntrySave(engine, "water", "English");
        string path = Path.Combine(workspace.TWorkspaceFolder, "water.md");
        CLibrary library = TLibraryPrepare(
            atelier, TEnvoyFake.TEnvoyFileCreate(path, CPortraitMedium.CPortraitMediumMarkdown, []));

        await library.CLibraryPortraitExport();

        Assert.False(File.Exists(path));

        library.CLibraryPanel.CPanelRowSelect(water.LEntryId);
        await library.CLibraryPortraitExport();

        Assert.Contains("water", File.ReadAllText(path), StringComparison.Ordinal);
    }

    [Fact]
    public async Task LibraryPortraitExport_CancelledOrUnwritable_ExportsNothingOrShowsTheFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TLibraryEntrySave(engine, "water", "English");
        List<string> asked = [];
        CLibrary cancelled = TLibraryPrepare(
            atelier, TEnvoyFake.TEnvoyFileCreate(null, CPortraitMedium.CPortraitMediumMarkdown, asked));
        cancelled.CLibraryPanel.CPanelRowSelect(water.LEntryId);

        await cancelled.CLibraryPortraitExport();

        Assert.Equal(["File:water"], asked);

        CLibrary unwritable = TLibraryPrepare(
            atelier,
            TEnvoyFake.TEnvoyFileCreate(
                workspace.TWorkspaceFolder, CPortraitMedium.CPortraitMediumMarkdown, asked));
        unwritable.CLibraryPanel.CPanelRowSelect(water.LEntryId);
        await unwritable.CLibraryPortraitExport();

        Assert.Equal(["File:water", "File:water", "Export.Failed"], asked);
    }

    [Fact]
    public void LibraryNavigation_EntryJump_OpensTheEntryInTheArea()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TLibraryEntrySave(engine, "water", "English");
        List<string> asked = [];
        CLibrary library = TLibraryPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        int landed = 0;
        atelier.CAtelierNavigation.CNavigationArrived += () => landed++;

        bool opened = atelier.CAtelierNavigation.CNavigationEntryOpen(water.LEntryId);

        Assert.True(opened);
        Assert.Empty(asked);
        Assert.Equal(1, landed);
        Assert.Equal(water.LEntryId, library.CLibraryPanel.TPanelChosenRead());
    }

    [Fact]
    public void LibraryPanelRowSelect_TabPanel_RecordsTheStationItLeaves()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TLibraryEntrySave(engine, "water", "English");
        LEntry fire = TLibraryEntrySave(engine, "fire", "English");
        CLibrary library = TLibraryPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        atelier.CAtelierNavigation.CNavigationTabSelect("Library");
        library.CLibraryPanel.CPanelRowSelect(water.LEntryId);
        List<CNavigationState> states = [];
        atelier.CAtelierNavigation.CNavigationChanged += states.Add;

        library.CLibraryPanel.CPanelRowSelect(fire.LEntryId);
        library.CLibraryPanel.CPanelRowSelect(null);

        CNavigationState state = Assert.Single(states);
        Assert.Equal(new CVoyageState(true, false), state.CNavigationStateVoyage);
        Assert.Equal(fire.LEntryId, library.CLibraryPanel.TPanelChosenRead());
    }

    internal static CLibrary TLibraryPrepare(CAtelier atelier, CEnvoy envoy)
    {
        CLibrary library = CLibrary.CLibraryCreate(atelier, static () => true, envoy, static run => run());
        return library;
    }

    internal static LEntry TLibraryEntrySave(LEngine engine, string headword, string language)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, language, string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
    }
}
