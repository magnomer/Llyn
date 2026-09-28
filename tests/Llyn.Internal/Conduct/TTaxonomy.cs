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
        Assert.Null(taxonomy.CTaxonomyChosen);
        Assert.False(taxonomy.CTaxonomyFiltered);
    }

    [Fact]
    public void TaxonomyTagCreate_Name_AnswersTheIdTheRowsList()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));

        long tag = taxonomy.CTaxonomyTagCreate("motion");
        taxonomy.CTaxonomyTagSelect(tag);

        CCatalogTag row = Assert.Single(taxonomy.CTaxonomyRowsRead(), row => row.CCatalogTagStored.CTagId == tag);
        Assert.Equal(new CTag(tag, "motion"), row.CCatalogTagStored);
        Assert.True(row.CCatalogTagChosen);
        Assert.Equal(tag, taxonomy.CTaxonomyChosen);
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
        CTaxonomy taxonomy = TTaxonomyPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        LEntry hearth = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "hearth", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
        string path = Path.Combine(workspace.TWorkspaceFolder, "hearth.md");

        await taxonomy.CTaxonomyPortraitExport(
            path, CPortraitMedium.CPortraitMediumMarkdown, TCorpus.TCorpusLabelCreate());

        Assert.False(File.Exists(path));
        Assert.Same(Task.CompletedTask, taxonomy.CTaxonomyPortraitPrint(TCorpus.TCorpusLabelCreate(), null!));

        taxonomy.CTaxonomyPanel.CPanelRowOpen(hearth.LEntryId);
        await taxonomy.CTaxonomyPortraitExport(
            path, CPortraitMedium.CPortraitMediumMarkdown, TCorpus.TCorpusLabelCreate());

        Assert.Equal("hearth", taxonomy.CTaxonomyFileRead());
        Assert.Contains("hearth", File.ReadAllText(path), System.StringComparison.Ordinal);
    }

    private static CTaxonomy TTaxonomyPrepare(CAtelier atelier, CEnvoy envoy)
    {
        CTaxonomy taxonomy = CTaxonomy.CTaxonomyCreate(atelier, static () => true, envoy);
        taxonomy.CTaxonomyVistaRestore();
        return taxonomy;
    }
}
