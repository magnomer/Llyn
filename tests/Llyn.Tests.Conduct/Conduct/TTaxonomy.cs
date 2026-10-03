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

public sealed class TTaxonomy
{
    [Fact]
    public void TaxonomyRowsRead_FreshArea_ListsTheStoredTags()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = CTaxonomy.CTaxonomyCreate(
            atelier, static () => true, TEnvoyFake.TEnvoyCreate(false, []), static run => run());
        engine.TEngineTagCreate("motion");

        Assert.Single(taxonomy.CTaxonomyRowsRead());
        Assert.Null(taxonomy.LTaxonomyChosen);
        Assert.False(taxonomy.CTaxonomyFiltered);
    }

    [Fact]
    public void TaxonomyEmptyKey_BlankOrWrittenSearch_PicksVacantOrUnmatched()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        taxonomy.CTaxonomyMembership.CMembershipQuerySet(" ");

        Assert.Equal("Tag.Vacant", taxonomy.CTaxonomyMembership.CMembershipEmptyKey);

        taxonomy.CTaxonomyMembership.CMembershipQuerySet("aqua");

        Assert.Equal("Tag.Unmatched", taxonomy.CTaxonomyMembership.CMembershipEmptyKey);
        Assert.Empty(taxonomy.CTaxonomyMembership.CMembershipRowsRead());
    }

    [Fact]
    public void TaxonomyOrderSet_ReverseThenNull_KeepsTheChosenOrderAndRaisesTheRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        engine.TEngineTagCreate("alpha");
        engine.TEngineTagCreate("beta");
        int told = 0;
        taxonomy.CTaxonomyRowsChanged += () => told++;

        Assert.Equal(CCatalogOrder.CCatalogOrderName, taxonomy.CTaxonomyOrder);

        taxonomy.CTaxonomyOrderSet(CCatalogOrder.CCatalogOrderReverse);
        taxonomy.CTaxonomyOrderSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderReverse, taxonomy.CTaxonomyOrder);
        Assert.Equal(
            ["beta", "alpha"],
            taxonomy.CTaxonomyRowsRead()
                .Select(row => row.CCatalogTagStored.CTagText)
                .Where(text => text is "alpha" or "beta"));
        Assert.True(told > 0);
    }

    [Fact]
    public void TaxonomyQuerySet_UnmatchedText_ListsNoTag()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        engine.TEngineTagCreate("motion");

        taxonomy.CTaxonomyQuerySet("zzz");

        Assert.Empty(taxonomy.CTaxonomyRowsRead());
    }

    [Fact]
    public void TaxonomyFilterSet_HiddenLanguage_MarksTheTaxonomyFiltered()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        taxonomy.CTaxonomyFilterSet(new CCatalogFilter(["Latin"]));

        Assert.True(taxonomy.CTaxonomyFiltered);
        Assert.Equal(["Latin"], taxonomy.CTaxonomyFilter.CCatalogFilterHidden);

        taxonomy.CTaxonomyFilterSet(new CCatalogFilter([]));

        Assert.False(taxonomy.CTaxonomyFiltered);
    }

    [Fact]
    public async Task TaxonomyPortraitExport_EntryShown_WritesItAndNothingBefore()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string path = Path.Combine(workspace.TWorkspaceFolder, "hearth.md");
        CTaxonomy taxonomy = TTaxonomyPrepare(
            atelier, TEnvoyFake.TEnvoyFileCreate(path, CPortraitMedium.CPortraitMediumMarkdown, []));
        LEntry hearth = TTaxonomyEntrySave(engine);

        await taxonomy.CTaxonomyMembership.CMembershipPortraitExport();

        Assert.False(File.Exists(path));
        Assert.Same(Task.CompletedTask, taxonomy.CTaxonomyMembership.CMembershipPortraitPrint());

        taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelRowOpen(hearth.LEntryId);
        await taxonomy.CTaxonomyMembership.CMembershipPortraitExport();

        Assert.Equal("hearth", taxonomy.CTaxonomyMembership.TMembershipFileRead());
        Assert.Contains("hearth", File.ReadAllText(path), System.StringComparison.Ordinal);
    }

    [Fact]
    public void MembershipRowSelect_EntryRow_OpensItWithoutAStation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        LEntry hearth = TTaxonomyEntrySave(engine);
        atelier.CAtelierNavigation.CNavigationTabSelect("Taxonomy");
        List<CNavigationState> states = [];
        atelier.CAtelierNavigation.CNavigationChanged += states.Add;
        CPanel panel = taxonomy.CTaxonomyMembership.CMembershipPanel;

        panel.CPanelRowSelect(null);

        Assert.False(panel.CPanelBinEnabled);

        panel.CPanelRowSelect(hearth.LEntryId);

        Assert.True(panel.CPanelBinEnabled);
        Assert.Equal(hearth.LEntryId, panel.TPanelChosenRead());
        Assert.Empty(states);
        Assert.Empty(asked);
    }

    [Fact]
    public void MembershipRowSelect_ChangedFreshEntryStayed_OpensNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCoinageCreate(null, null, asked));
        LEntry hearth = TTaxonomyEntrySave(engine);
        TTaxonomyChangePrepare(engine, taxonomy);
        CPanel panel = taxonomy.CTaxonomyMembership.CMembershipPanel;

        panel.CPanelRowSelect(hearth.LEntryId);

        Assert.Equal(["Leave"], asked);
        Assert.False(panel.CPanelBinEnabled);
        Assert.True(panel.CPanelEditing);
    }

    [Fact]
    public void TaxonomyTagToggle_RowClick_RecordsTheStationAndTogglesTheTag()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        long first = engine.TEngineTagCreate("motion").LTagId;
        atelier.CAtelierNavigation.CNavigationTabSelect("Taxonomy");
        taxonomy.TTaxonomyTagOpen(first);
        List<CNavigationState> states = [];
        atelier.CAtelierNavigation.CNavigationChanged += states.Add;
        List<string> seen = [];
        taxonomy.CTaxonomyRowsChanged += () =>
        {
            seen.Add("tag");
            taxonomy.CTaxonomyRowsRead();
        };
        taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelRowsChanged += () => seen.Add("entry");

        taxonomy.CTaxonomyTagToggle(first);

        Assert.Null(taxonomy.LTaxonomyChosen);
        Assert.Equal(new CVoyageState(true, false), Assert.Single(states).CNavigationStateVoyage);
        Assert.Equal(["tag", "entry"], seen);
    }

    [Fact]
    public void TaxonomyTagOpen_Arrival_ChoosesTheTagAndRaisesTheOpeningThenTheRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        long first = engine.TEngineTagCreate("motion").LTagId;
        List<string> seen = [];
        taxonomy.CTaxonomyTagOpened += () => seen.Add("opened");
        taxonomy.CTaxonomyRowsChanged += () => seen.Add("rows");

        taxonomy.TTaxonomyTagOpen(first);

        Assert.Equal(first, taxonomy.LTaxonomyChosen);
        Assert.Equal(["opened", "rows"], seen);
    }

    [Fact]
    public void TaxonomyTagOpen_QueriedLists_EmptiesBothQueriesBeforeTheOpening()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        long first = engine.TEngineTagCreate("motion").LTagId;
        taxonomy.CTaxonomyQuerySet("zzz");
        taxonomy.CTaxonomyMembership.CMembershipQuerySet("zzz");
        List<string> seen = [];
        taxonomy.CTaxonomyTagOpened += () =>
            seen.Add(taxonomy.CTaxonomyMembership.CMembershipEmptyKey + " " + taxonomy.CTaxonomyRowsRead().Count);

        taxonomy.TTaxonomyTagOpen(first);

        Assert.Equal(["Tag.Vacant 1"], seen);
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
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        taxonomy.CTaxonomyEditor.CEditorEntryOpen(null);
        Assert.True(taxonomy.CTaxonomyEditor.CEditorDesk.CDeskHeld);

        atelier.CAtelierClose();

        Assert.False(taxonomy.CTaxonomyEditor.CEditorDesk.CDeskHeld);
        Assert.Equal(1, stopped);
    }

    internal static LEntry TTaxonomyEntrySave(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "hearth", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
    }

    internal static void TTaxonomyChangePrepare(LEngine engine, CTaxonomy taxonomy)
    {
        long tag = engine.TEngineTagCreate("botany").LTagId;
        taxonomy.CTaxonomyTagToggle(tag);
        taxonomy.CTaxonomyEntryCreate();
        taxonomy.CTaxonomyEditor.CEditorHeadwordSet("fern");
        taxonomy.CTaxonomyTagToggle(tag);
    }

    internal static CTaxonomy TTaxonomyPrepare(CAtelier atelier, CEnvoy envoy)
    {
        CTaxonomy taxonomy = CTaxonomy.CTaxonomyCreate(atelier, static () => true, envoy, static run => run());
        return taxonomy;
    }
}
