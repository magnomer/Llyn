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
    public void TaxonomyRowsRead_NoVistaRestored_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = CTaxonomy.CTaxonomyCreate(
            atelier, static () => true, TInterfaceConduct.TEnvoyCreate(false, []));
        engine.TEngineTagCreate("motion");

        Assert.Empty(taxonomy.CTaxonomyRowsRead());
        Assert.Null(taxonomy.LTaxonomyChosen);
        Assert.False(taxonomy.CTaxonomyFiltered);
    }

    [Fact]
    public void TaxonomyTagCreate_Name_OpensTheNewTagChosenInTheRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, asked));
        int opened = 0;
        taxonomy.CTaxonomyTagOpened += () => opened++;

        taxonomy.CTaxonomyTagCreate("motion");

        long tag = Assert.NotNull(taxonomy.LTaxonomyChosen);
        CCatalogTag row = Assert.Single(taxonomy.CTaxonomyRowsRead(), row => row.CCatalogTagStored.CTagId == tag);
        Assert.Equal(new CTag(tag, "motion"), row.CCatalogTagStored);
        Assert.True(row.CCatalogTagChosen);
        Assert.Equal(tag, taxonomy.LTaxonomyChosen);
        Assert.Equal(1, opened);
        Assert.Empty(asked);
    }

    [Fact]
    public void TaxonomyTagCreate_BlankName_ShowsTheFailureAndOpensNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, asked));
        int opened = 0;
        taxonomy.CTaxonomyTagOpened += () => opened++;

        taxonomy.CTaxonomyTagCreate(" ");

        Assert.Equal(["Tag.CreateFailed"], asked);
        Assert.Equal(0, opened);
        Assert.Null(taxonomy.LTaxonomyChosen);
    }

    [Fact]
    public void TaxonomyCoinageAllowed_NothingChosenNorShown_NamesATagUntilOneIsChosen()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        LTag tag = engine.TEngineTagCreate("motion");

        Assert.True(taxonomy.CTaxonomyCoinageAllowed);

        taxonomy.CTaxonomyTagSelect(tag.LTagId);

        Assert.False(taxonomy.CTaxonomyCoinageAllowed);
    }

    [Fact]
    public void TaxonomyEmptyKey_BlankOrWrittenSearch_PicksVacantOrUnmatched()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));

        taxonomy.CTaxonomyMembershipFind(" ");

        Assert.Equal("Tag.Vacant", taxonomy.CTaxonomyEmptyKey);

        taxonomy.CTaxonomyMembershipFind("aqua");

        Assert.Equal("Tag.Unmatched", taxonomy.CTaxonomyEmptyKey);
        Assert.Empty(taxonomy.CTaxonomyMembershipRead());
    }

    [Fact]
    public void TaxonomyEntryCreate_TagChosen_OpensAnEntryCarryingItThatListsOnceStored()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        LTag tag = engine.TEngineTagCreate("botany");
        taxonomy.CTaxonomyTagSelect(tag.LTagId);
        List<CEntryDraft> shown = [];
        taxonomy.CTaxonomyEditor.CEditorDraftChanged += shown.Add;

        taxonomy.CTaxonomyEntryCreate();

        Assert.Contains(shown[0].CEntryDraftMeanings[0].CCardDraftTag, row => row.CTagDraftId == tag.LTagId);
        Assert.True(taxonomy.CTaxonomyPanel.CPanelEditing);
        Assert.False(taxonomy.CTaxonomyPanel.CPanelBinEnabled);

        taxonomy.CTaxonomyEditor.CEditorHeadwordSet("fern");
        taxonomy.CTaxonomyEditor.CEditorEntrySave();

        Assert.Equal(["fern"], taxonomy.CTaxonomyMembershipRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public void TaxonomyEntryCreate_NoTagChosen_OpensABlankEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        List<CEntryDraft> shown = [];
        taxonomy.CTaxonomyEditor.CEditorDraftChanged += shown.Add;

        taxonomy.CTaxonomyEntryCreate();

        Assert.Empty(shown[0].CEntryDraftMeanings[0].CCardDraftTag);
        Assert.False(taxonomy.CTaxonomyEditor.CEditorDesk.CDeskChanged);
        Assert.True(taxonomy.CTaxonomyPanel.CPanelEditing);
    }

    [Fact]
    public void TaxonomyPanelEntryClose_FreshEntryHeld_DropsTheDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        taxonomy.CTaxonomyEntryCreate();
        long held = taxonomy.CTaxonomyEditor.CEditorDesk.CDeskId;

        taxonomy.CTaxonomyPanel.CPanelEntryClose();

        Assert.Null(engine.TEngineDraftRead(held));
        Assert.False(taxonomy.CTaxonomyPanel.CPanelEditing);
    }

    [Fact]
    public void TaxonomyOrderSet_ReverseThenNull_KeepsTheChosenOrderAndTellsTheObserver()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        engine.TEngineTagCreate("alpha");
        engine.TEngineTagCreate("beta");
        int told = 0;
        taxonomy.CTaxonomyObserverAttach(CSubject.CSubjectVista, _ => told++);

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
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
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
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));

        taxonomy.CTaxonomyFilterSet(new CCatalogFilter(["Latin"]));

        Assert.True(taxonomy.CTaxonomyFiltered);
        Assert.Equal(["Latin"], taxonomy.CTaxonomyFilter.CCatalogFilterHidden);

        taxonomy.CTaxonomyFilterSet(new CCatalogFilter([]));

        Assert.False(taxonomy.CTaxonomyFiltered);
        Assert.NotNull(taxonomy.CTaxonomyLanguageRead());
    }

    [Fact]
    public async Task TaxonomyPortraitExport_EntryShown_WritesItAndNothingBefore()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string path = Path.Combine(workspace.TWorkspaceFolder, "hearth.md");
        CTaxonomy taxonomy = TTaxonomyPrepare(
            atelier, TInterfaceConduct.TEnvoyFileCreate(path, CPortraitMedium.CPortraitMediumMarkdown, []));
        LEntry hearth = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "hearth", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));

        await taxonomy.CTaxonomyPortraitExport();

        Assert.False(File.Exists(path));
        Assert.Same(Task.CompletedTask, taxonomy.CTaxonomyPortraitPrint());

        taxonomy.CTaxonomyPanel.CPanelRowOpen(hearth.LEntryId);
        await taxonomy.CTaxonomyPortraitExport();

        Assert.Equal("hearth", taxonomy.TTaxonomyFileRead());
        Assert.Contains("hearth", File.ReadAllText(path), System.StringComparison.Ordinal);
    }

    [Fact]
    public void TaxonomyTagToggle_RowClick_RecordsTheStationAndTogglesTheTag()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        long first = engine.TEngineTagCreate("motion").LTagId;
        atelier.CAtelierNavigation.CNavigationTabSelect("Taxonomy");
        taxonomy.CTaxonomyTagSelect(first);
        List<CNavigationState> states = [];
        atelier.CAtelierNavigation.CNavigationChanged += states.Add;

        taxonomy.CTaxonomyTagToggle(first);

        Assert.Null(taxonomy.LTaxonomyChosen);
        Assert.Equal(new CVoyageState(true, false), Assert.Single(states).CNavigationStateVoyage);
    }

    [Fact]
    public void TaxonomyTagOpen_Arrival_ChoosesTheTagAndRaisesTheOpening()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        long first = engine.TEngineTagCreate("motion").LTagId;
        int opened = 0;
        taxonomy.CTaxonomyTagOpened += () => opened++;

        taxonomy.TTaxonomyTagOpen(first);

        Assert.Equal(first, taxonomy.LTaxonomyChosen);
        Assert.Equal(1, opened);
    }

    [Fact]
    public void TaxonomyTagOpen_QueriedLists_EmptiesBothQueriesBeforeTheOpening()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        long first = engine.TEngineTagCreate("motion").LTagId;
        taxonomy.CTaxonomyQuerySet("zzz");
        taxonomy.CTaxonomyMembershipFind("zzz");
        List<string> seen = [];
        taxonomy.CTaxonomyTagOpened += () =>
            seen.Add(taxonomy.CTaxonomyEmptyKey + " " + taxonomy.CTaxonomyRowsRead().Count);

        taxonomy.TTaxonomyTagOpen(first);

        Assert.Equal(["Tag.Vacant 1"], seen);
    }

    private static CTaxonomy TTaxonomyPrepare(CAtelier atelier, CEnvoy envoy)
    {
        CTaxonomy taxonomy = CTaxonomy.CTaxonomyCreate(atelier, static () => true, envoy);
        taxonomy.CTaxonomyVistaRestore();
        return taxonomy;
    }
}
