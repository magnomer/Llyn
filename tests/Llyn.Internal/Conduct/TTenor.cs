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
    public void TenorRowsRead_NoVistaRestored_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = CTenor.CTenorCreate(
            atelier, static () => true, TEnvoyFake.TEnvoyCreate(false, []));
        engine.TEngineRegisterCreate("formal");

        Assert.Empty(tenor.CTenorRowsRead());
        Assert.Null(tenor.LTenorChosen);
        Assert.False(tenor.CTenorFiltered);
    }

    [Fact]
    public void TenorRegisterCreate_Name_OpensTheNewRegisterChosenInTheRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTenor tenor = TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        int opened = 0;
        tenor.CTenorRegisterOpened += () => opened++;

        tenor.CTenorRegisterCreate("formal");

        long register = Assert.NotNull(tenor.LTenorChosen);
        CCatalogRegister row = Assert.Single(
            tenor.CTenorRowsRead(), row => row.CCatalogRegisterStored.CRegisterId == register);
        Assert.Equal(new CRegister(register, "formal"), row.CCatalogRegisterStored);
        Assert.Equal(0, row.CCatalogRegisterUsage);
        Assert.True(row.CCatalogRegisterChosen);
        Assert.Equal(register, tenor.LTenorChosen);
        Assert.Equal(1, opened);
        Assert.Empty(asked);
    }

    [Fact]
    public void TenorRegisterCreate_BlankName_ShowsTheFailureAndOpensNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CTenor tenor = TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        int opened = 0;
        tenor.CTenorRegisterOpened += () => opened++;

        tenor.CTenorRegisterCreate(" ");

        Assert.Equal(["Register.CreateFailed"], asked);
        Assert.Equal(0, opened);
        Assert.Null(tenor.LTenorChosen);
    }

    [Fact]
    public void TenorCoinageAllowed_NothingChosenNorShown_NamesARegisterUntilOneIsChosen()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LRegister register = engine.TEngineRegisterCreate("formal");

        Assert.True(tenor.CTenorCoinageAllowed);

        tenor.CTenorRegisterSelect(register.LRegisterId);

        Assert.False(tenor.CTenorCoinageAllowed);
    }

    [Fact]
    public void TenorEmptyKey_BlankOrWrittenSearch_PicksVacantOrUnmatched()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        tenor.CTenorCohortFind(" ");

        Assert.Equal("Register.Vacant", tenor.CTenorEmptyKey);

        tenor.CTenorCohortFind("aqua");

        Assert.Equal("Register.Unmatched", tenor.CTenorEmptyKey);
        Assert.Empty(tenor.CTenorCohortRead());
    }

    [Fact]
    public void TenorEntryCreate_RegisterChosen_OpensAnEntryCarryingItThatListsOnceStored()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LRegister register = engine.TEngineRegisterCreate("formal");
        tenor.CTenorRegisterSelect(register.LRegisterId);
        List<CEntryDraft> shown = [];
        tenor.CTenorEditor.CEditorDraftChanged += shown.Add;

        tenor.CTenorEntryCreate();

        Assert.Contains(
            shown[0].CEntryDraftMeanings[0].CCardDraftRegister, row => row.CRegisterDraftId == register.LRegisterId);
        Assert.True(tenor.CTenorPanel.CPanelEditing);
        Assert.False(tenor.CTenorPanel.CPanelBinEnabled);

        tenor.CTenorEditor.CEditorHeadwordSet("fern");
        tenor.CTenorEditor.CEditorEntrySave();

        Assert.Equal(["fern"], tenor.CTenorCohortRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public void TenorEntryCreate_NoRegisterChosen_OpensABlankEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        List<CEntryDraft> shown = [];
        tenor.CTenorEditor.CEditorDraftChanged += shown.Add;

        tenor.CTenorEntryCreate();

        Assert.Empty(shown[0].CEntryDraftMeanings[0].CCardDraftRegister);
        Assert.False(tenor.CTenorEditor.CEditorDesk.CDeskChanged);
        Assert.True(tenor.CTenorPanel.CPanelEditing);
    }

    [Fact]
    public void TenorPanelEntryClose_FreshEntryHeld_DropsTheDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        tenor.CTenorEntryCreate();
        long held = tenor.CTenorEditor.CEditorDesk.CDeskId;

        tenor.CTenorPanel.CPanelEntryClose();

        Assert.Null(engine.TEngineDraftRead(held));
        Assert.False(tenor.CTenorPanel.CPanelEditing);
    }

    [Fact]
    public void TenorOrderSet_ReverseThenNull_KeepsTheChosenOrderAndTellsTheObserver()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        engine.TEngineRegisterCreate("alpha");
        engine.TEngineRegisterCreate("beta");
        int told = 0;
        tenor.CTenorObserverAttach(CSubject.CSubjectVista, _ => told++);

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
        Assert.NotNull(tenor.CTenorLanguageRead());
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

        await tenor.CTenorPortraitExport();

        Assert.False(File.Exists(path));
        Assert.Same(Task.CompletedTask, tenor.CTenorPortraitPrint());

        tenor.CTenorPanel.CPanelRowOpen(hearth.LEntryId);
        await tenor.CTenorPortraitExport();

        Assert.Equal("hearth", tenor.TTenorFileRead());
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
        tenor.CTenorRegisterSelect(first);
        List<CNavigationState> states = [];
        atelier.CAtelierNavigation.CNavigationChanged += states.Add;

        tenor.CTenorRegisterToggle(first);

        Assert.Null(tenor.LTenorChosen);
        Assert.Equal(new CVoyageState(true, false), Assert.Single(states).CNavigationStateVoyage);
    }

    [Fact]
    public void TenorRegisterOpen_Arrival_ChoosesTheRegisterAndRaisesTheOpening()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        long first = engine.TEngineRegisterCreate("formal").LRegisterId;
        int opened = 0;
        tenor.CTenorRegisterOpened += () => opened++;

        tenor.TTenorRegisterOpen(first);

        Assert.Equal(first, tenor.LTenorChosen);
        Assert.Equal(1, opened);
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
        tenor.CTenorCohortFind("zzz");
        List<string> seen = [];
        tenor.CTenorRegisterOpened += () => seen.Add(tenor.CTenorEmptyKey + " " + tenor.CTenorRowsRead().Count);

        tenor.TTenorRegisterOpen(first);

        Assert.Equal(["Register.Vacant 1"], seen);
    }

    private static CTenor TTenorPrepare(CAtelier atelier, CEnvoy envoy)
    {
        CTenor tenor = CTenor.CTenorCreate(atelier, static () => true, envoy);
        tenor.CTenorVistaRestore();
        return tenor;
    }
}
