using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRepertoire
{
    [Fact]
    public void RepertoireSituationCreate_SituationChosen_OpensALinkedEntryInTheEditor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        CRepertoire repertoire = TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.TRepertoireSituationOpen(home.LSituationId);
        List<CEntryDraft> shown = [];
        repertoire.CRepertoireEditor.CEditorDraftChanged += shown.Add;

        repertoire.CRepertoireSituationCreate();

        Assert.True(repertoire.CRepertoireEditorShown);
        Assert.False(repertoire.CRepertoireVignetteShown);
        Assert.False(repertoire.CRepertoirePlaywright.LPlaywrightDesk.CDeskHeld);
        Assert.Equal(home.LSituationId, repertoire.CRepertoireAtlas.CAtlasChosen);
        Assert.Contains(
            shown[0].CEntryDraftMeanings[0].CCardDraftSituation,
            row => row.CSituationDraftId == home.LSituationId);
    }

    [Fact]
    public void RepertoireSituationOpen_StoredSituation_ShowsItOnTheVignette()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        CRepertoire repertoire = TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        List<CSituation> shown = [];
        repertoire.CRepertoireSituationChanged += shown.Add;

        repertoire.TRepertoireSituationOpen(home.LSituationId);

        Assert.True(repertoire.CRepertoireVignetteShown);
        Assert.True(repertoire.CRepertoireVignetteHeld);
        Assert.True(repertoire.CRepertoirePressAllowed);
        Assert.False(repertoire.CRepertoirePortraitAllowed);
        Assert.Equal(new CStateWording("at home", null, false, null), Assert.Single(shown).CSituationTitle);
    }

    [Fact]
    public void RepertoireSituationOpen_HiddenByTheQuery_DropsTheQueryAndShowsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        TRepertoireSituationSave(engine, "in court");
        CRepertoire repertoire = TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.CRepertoireAtlas.CAtlasQuerySet("court");
        repertoire.CRepertoireAtlas.CAtlasPanel.CPanelRowsChanged += () => repertoire.CRepertoireRowsRead();
        int cleared = 0;
        repertoire.CRepertoireQueryCleared += () => cleared++;

        repertoire.TRepertoireSituationOpen(home.LSituationId);

        Assert.Equal(1, cleared);
        Assert.Equal(2, repertoire.CRepertoireRowsRead().Count);
        Assert.Equal(home.LSituationId, repertoire.CRepertoireAtlas.CAtlasChosen);
        Assert.True(repertoire.CRepertoireVignetteHeld);
    }

    [Fact]
    public void RepertoireSituationSelect_NothingUnsaved_RecordsAndShowsTheSituation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        List<string> asked = [];
        CRepertoire repertoire = TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
        int recorded = 0;
        atelier.CAtelierNavigation.CNavigationChanged += _ => recorded++;

        repertoire.CRepertoireSituationSelect(home.LSituationId);
        repertoire.CRepertoireSituationSelect(null);

        Assert.Equal(1, recorded);
        Assert.Empty(asked);
        Assert.Equal(home.LSituationId, repertoire.CRepertoireAtlas.CAtlasChosen);
    }

    [Fact]
    public void RepertoireOccurrenceSelect_ReferencingEntry_ShowsItOnTheDisplay()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        LEntry hearth = TRepertoireEntrySave(engine, "hearth", home);
        CRepertoire repertoire = TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.TRepertoireSituationOpen(home.LSituationId);

        repertoire.CRepertoireOccurrenceSelect(hearth.LEntryId);

        Assert.False(repertoire.CRepertoireVignetteShown);
        Assert.True(repertoire.CRepertoireDisplayShown);
        Assert.True(repertoire.CRepertoirePortraitAllowed);
        Assert.False(repertoire.CRepertoireBinEnabled);
        Assert.Equal(home.LSituationId, repertoire.CRepertoireAtlas.CAtlasChosen);
    }

    [Fact]
    public void RepertoireWorkspaceNotice_ChosenSituation_ClearsBothLists()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        CRepertoire repertoire = TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.TRepertoireSituationOpen(home.LSituationId);

        engine.TEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);

        Assert.True(repertoire.CRepertoireVignetteBlank);
        Assert.False(repertoire.CRepertoireDisplayShown);
        Assert.Null(repertoire.CRepertoireAtlas.CAtlasChosen);
    }

    [Fact]
    public void RepertoireEntryResonate_OccurrenceOnDisplay_KeepsTheOccurrenceSide()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        LEntry hearth = TRepertoireEntrySave(engine, "hearth", home);
        CRepertoire repertoire = TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.TRepertoireSituationOpen(home.LSituationId);
        repertoire.CRepertoireOccurrenceSelect(hearth.LEntryId);

        engine.TEngineBulletinRaise(LSubject.LSubjectEntry, 0);

        Assert.True(repertoire.CRepertoireDisplayShown);
        Assert.Equal(home.LSituationId, repertoire.CRepertoireAtlas.CAtlasChosen);
    }

    [Fact]
    public void RepertoireRowsRead_ChosenSituationNarrowedAway_ClearsBothLists()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        TRepertoireSituationSave(engine, "in court");
        CRepertoire repertoire = TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.TRepertoireSituationOpen(home.LSituationId);

        Assert.Equal(2, repertoire.CRepertoireRowsRead().Count);
        Assert.True(repertoire.CRepertoireVignetteHeld);

        repertoire.CRepertoireAtlas.CAtlasQuerySet("court");

        Assert.Single(repertoire.CRepertoireRowsRead());
        Assert.True(repertoire.CRepertoireVignetteBlank);
        Assert.Null(repertoire.CRepertoireAtlas.CAtlasChosen);
    }

    [Fact]
    public void RepertoireSituationDelete_ChosenSituationConfirmed_DeletesItAndClearsTheVignette()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        List<string> asked = [];
        CRepertoire repertoire = TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(true, asked));
        repertoire.TRepertoireSituationOpen(home.LSituationId);

        repertoire.CRepertoireSituationDelete();

        Assert.Equal(["Situation.DeleteConfirm"], asked);
        Assert.Empty(repertoire.CRepertoireRowsRead());
        Assert.True(repertoire.CRepertoireVignetteBlank);
    }

    [Fact]
    public void RepertoireSituationDelete_OccurrenceOnDisplay_DeletesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        LEntry hearth = TRepertoireEntrySave(engine, "hearth", home);
        List<string> asked = [];
        CRepertoire repertoire = TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(true, asked));
        repertoire.TRepertoireSituationOpen(home.LSituationId);
        repertoire.CRepertoireOccurrenceSelect(hearth.LEntryId);

        repertoire.CRepertoireSituationDelete();

        Assert.Empty(asked);
        Assert.Single(repertoire.CRepertoireRowsRead());
        Assert.True(repertoire.CRepertoireDisplayShown);
    }

    internal static CRepertoire TRepertoirePrepare(CAtelier atelier, CEnvoy envoy)
    {
        CRepertoire repertoire = CRepertoire.CRepertoireCreate(atelier, static () => true, envoy, static run => run());
        return repertoire;
    }

    internal static void TRepertoireTitleDefer(CRepertoire repertoire)
    {
        repertoire.CRepertoirePlaywright.LPlaywrightDesk.TDeskDefer(TInterface.TSituationTitleCreate(
            repertoire.CRepertoirePlaywright.LPlaywrightDesk.CDeskId,
            repertoire.TRepertoireScenarioRead()!.CSituationDraftId,
            TInterfaceState.TStateValueCreate("in court")));
    }

    internal static LSituation TRepertoireSituationSave(LEngine engine, string title)
    {
        return engine.TEngineSituationCreate(TInterface.TSituationCreate(0, title, null, null));
    }

    internal static LEntry TRepertoireEntrySave(LEngine engine, string headword, LSituation situation)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty,
                string.Empty,
                "a meaning",
                [],
                [TInterface.TSituationDraftCreate(situation.LSituationTitle, situation.LSituationId)],
                [],
                [],
                [],
                1)],
            []));
    }
}
