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
    public void LibraryRowsRead_NoVista_ReadsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TLibraryEntrySave(engine, "stone", "English");
        CLibrary library = CLibrary.CLibraryCreate(
            atelier, static () => true, TInterfaceConduct.TEnvoyCreate(false, []));

        Assert.Empty(library.CLibraryRowsRead());
        Assert.False(library.CLibraryFiltered);
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
        CLibrary library = TLibraryPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));

        Assert.Equal(2, library.CLibraryRowsRead().Count);

        library.CLibraryQuerySet("riv");

        Assert.Equal(["river"], library.CLibraryRowsRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public void LibraryOrderSet_NoOrderRow_KeepsTheOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CLibrary library = TLibraryPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));

        library.CLibraryOrderSet(CCatalogOrder.CCatalogOrderReverse);
        library.CLibraryOrderSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderReverse, library.CLibraryPanel.CPanelOrder);
    }

    [Fact]
    public void LibraryFilterSet_HiddenLanguage_MarksTheListFiltered()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TLibraryEntrySave(engine, "stone", "English");
        TLibraryEntrySave(engine, "maison", "French");
        CLibrary library = TLibraryPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));

        library.CLibraryFilterSet(new CCatalogFilter(["English"]));

        Assert.True(library.CLibraryFiltered);
        Assert.Equal(["English"], library.CLibraryPanel.CPanelFilter.CCatalogFilterHidden);
        Assert.Equal(["maison"], library.CLibraryRowsRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public void LibraryPanelRowSelect_RowChosen_MarksItAndReportsItsId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TLibraryEntrySave(engine, "water", "English");
        CLibrary library = TLibraryPrepare(atelier, TInterfaceConduct.TEnvoyCreate(true, []));

        Assert.Equal(0, library.CLibraryPanel.TPanelChosenRead());

        library.CLibraryPanel.CPanelRowSelect(water.LEntryId);

        Assert.Equal(water.LEntryId, library.CLibraryPanel.TPanelChosenRead());
        Assert.Equal(["water"], library.CLibraryRowsRead()
            .Where(row => row.CVistaRowChosen)
            .Select(row => row.CVistaRowHeadword));
        Assert.Equal("water", library.TLibraryFileRead());

        library.CLibraryPanel.CPanelEntryDelete();

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
        CLibrary library = TLibraryPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));

        library.CLibraryPanel.CPanelRowOpen(stone.LEntryId);
        library.CLibraryPanel.CPanelScribeToggle(true);

        Assert.Equal(stone.LEntryId, library.CLibraryEditor.CEditorDesk.CDeskStoredRead());

        library.CLibraryPanel.CPanelEntryClose();

        Assert.Null(library.CLibraryEditor.CEditorDesk.CDeskStoredRead());
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
            atelier, TInterfaceConduct.TEnvoyFileCreate(path, CPortraitMedium.CPortraitMediumMarkdown, []));

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
            atelier, TInterfaceConduct.TEnvoyFileCreate(null, CPortraitMedium.CPortraitMediumMarkdown, asked));
        cancelled.CLibraryPanel.CPanelRowSelect(water.LEntryId);

        await cancelled.CLibraryPortraitExport();

        Assert.Equal(["File:water"], asked);

        CLibrary unwritable = TLibraryPrepare(
            atelier,
            TInterfaceConduct.TEnvoyFileCreate(
                workspace.TWorkspaceFolder, CPortraitMedium.CPortraitMediumMarkdown, asked));
        unwritable.CLibraryPanel.CPanelRowSelect(water.LEntryId);
        await unwritable.CLibraryPortraitExport();

        Assert.Equal(["File:water", "File:water", "Export.Failed"], asked);
    }

    [Fact]
    public async Task LibraryMarkupImport_NoPath_AsksNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CLibrary library = TLibraryPrepare(atelier, TLibraryEnvoyCreate(static _ => [], asked));

        await library.CLibraryMarkupImport(null);

        Assert.Empty(asked);
        Assert.Empty(library.CLibraryRowsRead());
    }

    [Fact]
    public async Task LibraryMarkupImport_FreshRows_StoresTheEntriesAndReportsTheOmission()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        List<CMarkupEntry> shown = [];
        CLibrary library = TLibraryPrepare(atelier, TLibraryEnvoyCreate(
            entries =>
            {
                shown.AddRange(entries);
                return [.. entries.Select(static _ => TLibraryRowCreate(CSCustomsMode.CSCustomsModeFresh, 0))];
            },
            asked));
        int refreshed = 0;
        library.CLibraryPanel.CPanelRowsChanged += () => refreshed++;

        await library.CLibraryMarkupImport(TInterface.TMarkupSave(workspace, TInterface.TMarkupLone));

        Assert.Equal("ember", Assert.Single(shown).CMarkupEntryHeadword);
        Assert.Equal(1, refreshed);
        Assert.Equal(["Customs", "Omission"], asked.Select(static question => question.Split(':')[0]));
        Assert.Contains("braise", asked[1], StringComparison.Ordinal);
        Assert.Equal(["ember"], library.CLibraryRowsRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public async Task LibraryMarkupImport_MergeRow_AppendsToTheTargetEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry ember = TLibraryEntrySave(engine, "ember", "English");
        CLibrary library = TLibraryPrepare(atelier, TLibraryEnvoyCreate(
            _ => [TLibraryRowCreate(CSCustomsMode.CSCustomsModeMerge, ember.LEntryId)], []));

        await library.CLibraryMarkupImport(TInterface.TMarkupSave(workspace, TInterface.TMarkupLone));

        Assert.Equal(ember.LEntryId, Assert.Single(library.CLibraryRowsRead()).CVistaRowId);
        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        Assert.Equal(2, draft.LEntryDraftMeanings.Count);
    }

    [Fact]
    public async Task LibraryMarkupImport_Declined_StoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CLibrary library = TLibraryPrepare(atelier, TLibraryEnvoyCreate(static _ => null, asked));
        int refreshed = 0;
        library.CLibraryPanel.CPanelRowsChanged += () => refreshed++;

        await library.CLibraryMarkupImport(TInterface.TMarkupSave(workspace, TInterface.TMarkupLone));

        Assert.Equal(["Customs:1"], asked);
        Assert.Equal(0, refreshed);
        Assert.Empty(library.CLibraryRowsRead());
    }

    [Fact]
    public async Task LibraryMarkupImport_MalformedFile_ShowsTheImportFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CLibrary library = TLibraryPrepare(atelier, TLibraryEnvoyCreate(static _ => [], asked));

        await library.CLibraryMarkupImport(TInterface.TMarkupSave(
            workspace, "<llyn><entry><headword>ember</headword></llyn>"));

        Assert.Equal(["List.ImportFailed"], asked);
        Assert.Empty(library.CLibraryRowsRead());
    }

    [Fact]
    public void LibraryNavigation_EntryJump_OpensTheEntryInTheArea()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TLibraryEntrySave(engine, "water", "English");
        List<string> asked = [];
        CLibrary library = TLibraryPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, asked));
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
        CLibrary library = TLibraryPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
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

    private static CLibrary TLibraryPrepare(CAtelier atelier, CEnvoy envoy)
    {
        CLibrary library = CLibrary.CLibraryCreate(atelier, static () => true, envoy);
        library.CLibraryVistaRestore();
        return library;
    }

    private static CEnvoy TLibraryEnvoyCreate(
        Func<IReadOnlyList<CMarkupEntry>, IReadOnlyList<CSCustomsRow>?> customs, List<string> asked) =>
        TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyCustomsRead"] = args =>
            {
                IReadOnlyList<CMarkupEntry> entries = (IReadOnlyList<CMarkupEntry>)args![0]!;
                asked.Add($"Customs:{entries.Count}");
                return customs(entries);
            },
            ["CEnvoyOmissionShow"] = args =>
            {
                IReadOnlyList<CMarkupOmission> omissions = (IReadOnlyList<CMarkupOmission>)args![0]!;
                asked.Add(
                    "Omission:" + string.Join("|", omissions.Select(static omission => omission.CMarkupOmissionText)));
                return null;
            },
            ["CEnvoyFailureShow"] = args =>
            {
                asked.Add((string)args![0]!);
                return null;
            },
        });

    private static CSCustomsRow TLibraryRowCreate(CSCustomsMode mode, long target) =>
        new(mode, target, mode != CSCustomsMode.CSCustomsModeFresh, 0);

    private static LEntry TLibraryEntrySave(LEngine engine, string headword, string language)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, language, string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
    }
}
