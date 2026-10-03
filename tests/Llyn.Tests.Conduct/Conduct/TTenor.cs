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

public sealed class TTenor
{
    [Fact]
    public void TenorRowsRead_FreshArea_ListsTheStoredRegisters()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = CTenor.CTenorCreate(
            atelier, static () => true, TEnvoyFake.TEnvoyCreate(false, []), static run => run());
        engine.TEngineRegisterCreate("formal");

        Assert.Single(tenor.CTenorRowsRead());
        Assert.Null(tenor.LTenorChosen);
        Assert.False(tenor.CTenorFiltered);
    }

    [Fact]
    public void TenorEmptyKey_BlankOrWrittenSearch_PicksVacantOrUnmatched()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        tenor.CTenorCohort.CCohortQuerySet(" ");

        Assert.Equal("Register.Vacant", tenor.CTenorCohort.CCohortEmptyKey);

        tenor.CTenorCohort.CCohortQuerySet("aqua");

        Assert.Equal("Register.Unmatched", tenor.CTenorCohort.CCohortEmptyKey);
        Assert.Empty(tenor.CTenorCohort.CCohortRowsRead());
    }

    [Fact]
    public void TenorOrderSet_ReverseThenNull_KeepsTheChosenOrderAndRaisesTheRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        engine.TEngineRegisterCreate("alpha");
        engine.TEngineRegisterCreate("beta");
        int told = 0;
        tenor.CTenorRowsChanged += () => told++;

        Assert.Equal(CCatalogOrder.CCatalogOrderName, tenor.CTenorOrder);

        tenor.CTenorOrderSet(CCatalogOrder.CCatalogOrderReverse);
        tenor.CTenorOrderSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderReverse, tenor.CTenorOrder);
        Assert.Equal(
            ["beta", "alpha"],
            tenor.CTenorRowsRead()
                .Select(row => row.CCatalogRegisterStored.CRegisterName)
                .Where(text => text is "alpha" or "beta"));
        Assert.True(told > 0);
    }

    [Fact]
    public void TenorQuerySet_UnmatchedText_ListsNoRegister()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        engine.TEngineRegisterCreate("formal");

        tenor.CTenorQuerySet("zzz");

        Assert.Empty(tenor.CTenorRowsRead());
    }

    [Fact]
    public void TenorFilterSet_HiddenLanguage_MarksTheTenorFiltered()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        tenor.CTenorFilterSet(new CCatalogFilter(["Latin"]));

        Assert.True(tenor.CTenorFiltered);
        Assert.Equal(["Latin"], tenor.CTenorFilter.CCatalogFilterHidden);

        tenor.CTenorFilterSet(new CCatalogFilter([]));

        Assert.False(tenor.CTenorFiltered);
    }

    [Fact]
    public async Task TenorPortraitExport_EntryShown_WritesItAndNothingBefore()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string path = Path.Combine(workspace.TWorkspaceFolder, "hearth.md");
        CTenor tenor = TTenorPrepare(
            atelier, TEnvoyFake.TEnvoyFileCreate(path, CPortraitMedium.CPortraitMediumMarkdown, []));
        LEntry hearth = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "hearth", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));

        await tenor.CTenorCohort.CCohortPortraitExport();

        Assert.False(File.Exists(path));
        Assert.Same(Task.CompletedTask, tenor.CTenorCohort.CCohortPortraitPrint());

        tenor.CTenorCohort.CCohortPanel.CPanelRowOpen(hearth.LEntryId);
        await tenor.CTenorCohort.CCohortPortraitExport();

        Assert.Equal("hearth", tenor.CTenorCohort.TCohortFileRead());
        Assert.Contains("hearth", File.ReadAllText(path), System.StringComparison.Ordinal);
    }

    [Fact]
    public void TenorRegisterToggle_RowClick_RecordsTheStationAndTogglesTheRegister()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        long first = engine.TEngineRegisterCreate("formal").LRegisterId;
        atelier.CAtelierNavigation.CNavigationTabSelect("Tenor");
        tenor.TTenorRegisterOpen(first);
        List<CNavigationState> states = [];
        atelier.CAtelierNavigation.CNavigationChanged += states.Add;

        int rows = 0;
        tenor.CTenorRowsChanged += () => rows++;

        tenor.CTenorRegisterToggle(first);

        Assert.Null(tenor.LTenorChosen);
        Assert.Equal(1, rows);
        Assert.Equal(new CVoyageState(true, false), Assert.Single(states).CNavigationStateVoyage);
    }

    [Fact]
    public void TenorRegisterOpen_Arrival_ChoosesTheRegisterAndRaisesTheOpeningThenTheRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        long first = engine.TEngineRegisterCreate("formal").LRegisterId;
        List<string> seen = [];
        tenor.CTenorRegisterOpened += () => seen.Add("opened");
        tenor.CTenorRowsChanged += () => seen.Add("rows");

        tenor.TTenorRegisterOpen(first);

        Assert.Equal(first, tenor.LTenorChosen);
        Assert.Equal(["opened", "rows"], seen);
    }

    [Fact]
    public void TenorRegisterOpen_QueriedLists_EmptiesBothQueriesBeforeTheOpening()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        long first = engine.TEngineRegisterCreate("formal").LRegisterId;
        tenor.CTenorQuerySet("zzz");
        tenor.CTenorCohort.CCohortQuerySet("zzz");
        List<string> seen = [];
        tenor.CTenorRegisterOpened += () =>
            seen.Add(tenor.CTenorCohort.CCohortEmptyKey + " " + tenor.CTenorRowsRead().Count);

        tenor.TTenorRegisterOpen(first);

        Assert.Equal(["Register.Vacant 1"], seen);
    }

    [Fact]
    public void AtelierClose_EntryHeldOnTheEditor_CancelsTheEditorDeskAndStopsPlayback()
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
        CTenor tenor = TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        tenor.CTenorEditor.CEditorEntryOpen(null);
        Assert.True(tenor.CTenorEditor.CEditorDesk.CDeskHeld);

        atelier.CAtelierClose();

        Assert.False(tenor.CTenorEditor.CEditorDesk.CDeskHeld);
        Assert.Equal(1, stopped);
    }

    internal static CTenor TTenorPrepare(CAtelier atelier, CEnvoy envoy)
    {
        CTenor tenor = CTenor.CTenorCreate(atelier, static () => true, envoy, static run => run());
        return tenor;
    }
}
