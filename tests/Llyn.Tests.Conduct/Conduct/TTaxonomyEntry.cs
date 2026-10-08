using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTaxonomyEntry
{
    [Fact]
    public void TaxonomyEntryCreate_PaddedWordingNothingChosen_AsksTheWordingAndOpensTheTrimmedTagChosen()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyCoinageCreate("  motion ", null, asked);
        CTaxonomy taxonomy = TTaxonomy.TTaxonomyPrepare(atelier, envoy);
        int opened = 0;
        taxonomy.CTaxonomyTagOpened += () => opened++;

        taxonomy.CTaxonomyEntryCreate();

        long tag = Assert.NotNull(taxonomy.CTaxonomyAperture.CApertureChosen);
        CCatalogTag row = Assert.Single(taxonomy.CTaxonomyRowsRead(), row => row.CCatalogTagStored.CTagId == tag);
        Assert.Equal(new CTag(tag, "motion"), row.CCatalogTagStored);
        Assert.True(row.CCatalogTagChosen);
        Assert.Equal(1, opened);
        Assert.Equal(["Coinage:Coinage.Tag"], asked);
        Assert.False(taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelEditing);
    }

    [Fact]
    public void TaxonomyEntryCreate_BlankWording_ShowsTheFailureAndOpensNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTaxonomy taxonomy = TTaxonomy.TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCoinageCreate(" ", null, asked));
        int opened = 0;
        taxonomy.CTaxonomyTagOpened += () => opened++;

        taxonomy.CTaxonomyEntryCreate();

        Assert.Equal(["Coinage:Coinage.Tag", "Tag.CreateFailed"], asked);
        Assert.Equal(0, opened);
        Assert.Null(taxonomy.CTaxonomyAperture.CApertureChosen);
    }

    [Fact]
    public void TaxonomyEntryCreate_RetreatedWording_MakesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTaxonomy taxonomy = TTaxonomy.TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCoinageCreate(null, null, asked));
        int opened = 0;
        taxonomy.CTaxonomyTagOpened += () => opened++;

        taxonomy.CTaxonomyEntryCreate();

        Assert.Equal(["Coinage:Coinage.Tag"], asked);
        Assert.Equal(0, opened);
        Assert.Null(taxonomy.CTaxonomyAperture.CApertureChosen);
        Assert.Empty(taxonomy.CTaxonomyRowsRead());
        Assert.False(taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelEditing);
    }

    [Fact]
    public void TaxonomyEntryCreate_ChangedFreshEntryStayed_AsksOnlyTheLeaveQuestion()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTaxonomy taxonomy = TTaxonomy.TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCoinageCreate("motion", null, asked));
        TTaxonomy.TTaxonomyChangePrepare(engine, taxonomy);

        taxonomy.CTaxonomyEntryCreate();

        Assert.Equal(["Leave"], asked);
        Assert.Null(taxonomy.CTaxonomyAperture.CApertureChosen);
        Assert.True(taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelEditing);
        Assert.True(taxonomy.CTaxonomyEditor.CEditorDesk.CDeskDraft.CDeskDraftAltered);
    }

    [Fact]
    public void TaxonomyEntryCreate_ChangedFreshEntryDiscarded_AsksTheLeaveThenTheWording()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyCoinageCreate("motion", false, asked);
        CTaxonomy taxonomy = TTaxonomy.TTaxonomyPrepare(atelier, envoy);
        TTaxonomy.TTaxonomyChangePrepare(engine, taxonomy);

        taxonomy.CTaxonomyEntryCreate();

        Assert.Equal(["Leave", "Coinage:Coinage.Tag"], asked);
        Assert.NotNull(taxonomy.CTaxonomyAperture.CApertureChosen);
        Assert.False(taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelEditing);
    }

    [Fact]
    public void TaxonomyEntryCreate_TagChosen_StartsAnEntryWithoutAskingTheWording()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTaxonomy taxonomy = TTaxonomy.TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCoinageCreate("motion", null, asked));
        LTag tag = engine.TEngineTagCreate("botany");
        taxonomy.CTaxonomyTagToggle(tag.LTagId);

        taxonomy.CTaxonomyEntryCreate();

        Assert.Empty(asked);
        Assert.Equal(tag.LTagId, taxonomy.CTaxonomyAperture.CApertureChosen);
        Assert.True(taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelEditing);
    }

    [Fact]
    public void TaxonomyEntryCreate_TagChosen_OpensAnEntryCarryingItThatListsOnceStored()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomy.TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LTag tag = engine.TEngineTagCreate("botany");
        taxonomy.CTaxonomyTagToggle(tag.LTagId);
        List<CEntryDraft> shown = [];
        taxonomy.CTaxonomyEditor.CEditorEntry.CEntryDraftChanged += shown.Add;

        taxonomy.CTaxonomyEntryCreate();

        Assert.Contains(shown[0].CEntryDraftMeanings[0].CCardDraftTag, row => row.CTagDraftId == tag.LTagId);
        Assert.True(taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelEditing);
        Assert.False(taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelBinEnabled);

        taxonomy.CTaxonomyEditor.CEditorEntry.CEntryHeadwordSet("fern");
        taxonomy.CTaxonomyEditor.CEditorEntrySave();

        Assert.Equal(["fern"], taxonomy.CTaxonomyMembership.CMembershipRowsRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public void TaxonomyEntryCreate_EntryShownNoTagChosen_OpensABlankEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomy.TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LEntry hearth = TTaxonomy.TTaxonomyEntrySave(engine);
        taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelRowOpen(hearth.LEntryId);
        List<CEntryDraft> shown = [];
        taxonomy.CTaxonomyEditor.CEditorEntry.CEntryDraftChanged += shown.Add;

        taxonomy.CTaxonomyEntryCreate();

        Assert.Empty(shown[0].CEntryDraftMeanings[0].CCardDraftTag);
        Assert.False(taxonomy.CTaxonomyEditor.CEditorDesk.CDeskDraft.CDeskDraftAltered);
        Assert.True(taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelEditing);
    }

    [Fact]
    public void TaxonomyPanelEntryClose_FreshEntryHeld_DropsTheDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomy.TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        taxonomy.CTaxonomyTagToggle(engine.TEngineTagCreate("botany").LTagId);
        taxonomy.CTaxonomyEntryCreate();
        long held = taxonomy.CTaxonomyEditor.CEditorDesk.CDeskId;

        taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelEntryClose();

        Assert.Null(engine.TEngineDraftRead(held));
        Assert.False(taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelEditing);
    }
}
