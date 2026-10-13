using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTenorEntry
{
    [Fact]
    public void TenorEntryCreate_PaddedWordingNothingChosen_AsksTheWordingAndOpensTheTrimmedRegisterChosen()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTenor tenor = TTenor.TTenorPrepare(atelier, TEnvoyFake.TEnvoyCoinageCreate("  formal ", null, asked));
        int opened = 0;
        tenor.CTenorRegisterOpened += () => opened++;

        tenor.CTenorEntryCreate();

        long register = Assert.NotNull(tenor.CTenorAperture.CApertureChosen);
        CCatalogRegister row = Assert.Single(
            tenor.CTenorRowsRead(), row => row.CCatalogRegisterStored.CRegisterId == register);
        Assert.Equal(new CRegister(register, "formal"), row.CCatalogRegisterStored);
        Assert.Equal(0, row.CCatalogRegisterUsage);
        Assert.Equal("register/formal", row.CCatalogRegisterIcon);
        Assert.True(row.CCatalogRegisterChosen);
        Assert.Equal(register, tenor.CTenorAperture.CApertureChosen);
        Assert.Equal(1, opened);
        Assert.Equal(["Coinage:Coinage.Register"], asked);
        Assert.False(tenor.CTenorCohort.CCohortPanel.CPanelEditing);
    }

    [Fact]
    public void TenorEntryCreate_BlankWording_ShowsTheFailureAndOpensNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTenor tenor = TTenor.TTenorPrepare(atelier, TEnvoyFake.TEnvoyCoinageCreate(" ", null, asked));
        int opened = 0;
        tenor.CTenorRegisterOpened += () => opened++;

        tenor.CTenorEntryCreate();

        Assert.Equal(["Coinage:Coinage.Register", "Register.CreateFailed"], asked);
        Assert.Equal(0, opened);
        Assert.Null(tenor.CTenorAperture.CApertureChosen);
    }

    [Fact]
    public void TenorEntryCreate_RetreatedWording_MakesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTenor tenor = TTenor.TTenorPrepare(atelier, TEnvoyFake.TEnvoyCoinageCreate(null, null, asked));
        int opened = 0;
        tenor.CTenorRegisterOpened += () => opened++;

        tenor.CTenorEntryCreate();

        Assert.Equal(["Coinage:Coinage.Register"], asked);
        Assert.Equal(0, opened);
        Assert.Null(tenor.CTenorAperture.CApertureChosen);
        Assert.False(tenor.CTenorCohort.CCohortPanel.CPanelEditing);
    }

    [Fact]
    public void TenorEntryCreate_ChangedFreshEntryStayed_AsksOnlyTheLeaveQuestion()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTenor tenor = TTenor.TTenorPrepare(atelier, TEnvoyFake.TEnvoyCoinageCreate("formal", null, asked));
        TTenorChangePrepare(engine, tenor);

        tenor.CTenorEntryCreate();

        Assert.Equal(["Leave"], asked);
        Assert.Null(tenor.CTenorAperture.CApertureChosen);
        Assert.True(tenor.CTenorCohort.CCohortPanel.CPanelEditing);
        Assert.True(tenor.CTenorEditor.CEditorDesk.CDeskDraft.CDeskDraftAltered);
    }

    [Fact]
    public void TenorEntryCreate_ChangedFreshEntryDiscarded_AsksTheLeaveThenTheWording()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTenor tenor = TTenor.TTenorPrepare(atelier, TEnvoyFake.TEnvoyCoinageCreate("formal", false, asked));
        TTenorChangePrepare(engine, tenor);

        tenor.CTenorEntryCreate();

        Assert.Equal(["Leave", "Coinage:Coinage.Register"], asked);
        Assert.NotNull(tenor.CTenorAperture.CApertureChosen);
        Assert.False(tenor.CTenorCohort.CCohortPanel.CPanelEditing);
    }

    [Fact]
    public void TenorEntryCreate_RegisterChosen_StartsAnEntryWithoutAskingTheWording()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTenor tenor = TTenor.TTenorPrepare(atelier, TEnvoyFake.TEnvoyCoinageCreate("formal", null, asked));
        LRegister register = engine.TEngineRegisterCreate("casual");
        tenor.TTenorRegisterOpen(register.LRegisterId);

        tenor.CTenorEntryCreate();

        Assert.Empty(asked);
        Assert.Equal(register.LRegisterId, tenor.CTenorAperture.CApertureChosen);
        Assert.True(tenor.CTenorCohort.CCohortPanel.CPanelEditing);
    }

    [Fact]
    public void TenorEntryCreate_RegisterChosen_OpensAnEntryCarryingItThatListsOnceStored()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenor.TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LRegister register = engine.TEngineRegisterCreate("formal");
        tenor.TTenorRegisterOpen(register.LRegisterId);
        List<CEntryDraft> shown = [];
        tenor.CTenorEditor.CEditorEntry.CEntryDraftChanged += shown.Add;

        tenor.CTenorEntryCreate();

        Assert.Contains(
            shown[0].CEntryDraftMeanings[0].CCardDraftRegister, row => row.CRegisterDraftId == register.LRegisterId);
        Assert.True(tenor.CTenorCohort.CCohortPanel.CPanelEditing);
        Assert.False(tenor.CTenorCohort.CCohortPanel.CPanelBinEnabled);

        tenor.CTenorEditor.CEditorEntry.CEntryHeadwordSet("fern");
        tenor.CTenorEditor.CEditorEntrySave();

        Assert.Equal(["fern"], tenor.CTenorCohort.CCohortRowsRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public void TenorEntryCreate_EntryShownNoRegisterChosen_OpensABlankEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenor.TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LEntry hearth = TTenorEntrySave(engine);
        tenor.CTenorCohort.CCohortPanel.CPanelRowOpen(hearth.LEntryId);
        List<CEntryDraft> shown = [];
        tenor.CTenorEditor.CEditorEntry.CEntryDraftChanged += shown.Add;

        tenor.CTenorEntryCreate();

        Assert.Empty(shown[0].CEntryDraftMeanings[0].CCardDraftRegister);
        Assert.False(tenor.CTenorEditor.CEditorDesk.CDeskDraft.CDeskDraftAltered);
        Assert.True(tenor.CTenorCohort.CCohortPanel.CPanelEditing);
    }

    [Fact]
    public void TenorPanelEntryClose_FreshEntryHeld_DropsTheDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenor.TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        tenor.TTenorRegisterOpen(engine.TEngineRegisterCreate("formal").LRegisterId);
        tenor.CTenorEntryCreate();
        long held = tenor.CTenorEditor.CEditorDesk.CDeskId;

        tenor.CTenorCohort.CCohortPanel.CPanelEntryClose();

        Assert.Null(engine.TEngineDraftRead(held));
        Assert.False(tenor.CTenorCohort.CCohortPanel.CPanelEditing);
    }

    [Fact]
    public void CohortRowSelect_EntryRow_OpensItWithoutAStation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTenor tenor = TTenor.TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        LEntry hearth = TTenorEntrySave(engine);
        atelier.CAtelierNavigation.CNavigationTabSelect("Tenor");
        List<CNavigationState> states = [];
        atelier.CAtelierNavigation.CNavigationChanged += states.Add;
        CPanel panel = tenor.CTenorCohort.CCohortPanel;

        panel.CPanelRowSelect(null);

        Assert.False(panel.CPanelBinEnabled);

        panel.CPanelRowSelect(hearth.LEntryId);

        Assert.True(panel.CPanelBinEnabled);
        Assert.Equal(hearth.LEntryId, panel.TPanelChosenRead());
        Assert.Empty(states);
        Assert.Empty(asked);
    }

    [Fact]
    public void CohortRowSelect_ChangedFreshEntryStayed_OpensNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTenor tenor = TTenor.TTenorPrepare(atelier, TEnvoyFake.TEnvoyCoinageCreate(null, null, asked));
        LEntry hearth = TTenorEntrySave(engine);
        TTenorChangePrepare(engine, tenor);
        CPanel panel = tenor.CTenorCohort.CCohortPanel;

        panel.CPanelRowSelect(hearth.LEntryId);

        Assert.Equal(["Leave"], asked);
        Assert.False(panel.CPanelBinEnabled);
        Assert.True(panel.CPanelEditing);
    }

    private static LEntry TTenorEntrySave(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "hearth", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
    }

    private static void TTenorChangePrepare(LEngine engine, CTenor tenor)
    {
        long register = engine.TEngineRegisterCreate("casual").LRegisterId;
        tenor.CTenorRegisterToggle(register);
        tenor.CTenorEntryCreate();
        tenor.CTenorEditor.CEditorEntry.CEntryHeadwordSet("fern");
        tenor.CTenorRegisterToggle(register);
    }
}
