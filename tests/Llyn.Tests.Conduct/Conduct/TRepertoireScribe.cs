using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRepertoireScribe
{
    [Fact]
    public void RepertoireSituationCreate_NoRowChosen_StartsABlankScenario()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        List<CScenario> held = [];
        repertoire.CRepertoireScenarioChanged += held.Add;

        repertoire.CRepertoireSituationCreate();

        Assert.True(repertoire.CRepertoireDesk.CDeskHeld);
        Assert.True(repertoire.CRepertoireScenarioShown);
        Assert.True(repertoire.CRepertoireScribeChecked);
        Assert.NotNull(held[^1]);
        Assert.NotNull(repertoire.TRepertoireScenarioRead());
    }

    [Fact]
    public void RepertoireSituationSelect_UnsavedScenarioStored_StoresItAndShowsTheChosenSituation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoire.TRepertoireSituationSave(engine, "at home");
        List<string> asked = [];
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(true, asked));
        repertoire.CRepertoireSituationCreate();
        TRepertoire.TRepertoireTitleDefer(repertoire);
        int recorded = 0;
        atelier.CAtelierNavigation.CNavigationChanged += _ => recorded++;

        repertoire.CRepertoireSituationSelect(home.LSituationId);

        Assert.Equal(1, recorded);
        Assert.Equal(["Leave"], asked);
        Assert.Equal(2, repertoire.CRepertoireRowsRead().Count);
        Assert.Equal(home.LSituationId, repertoire.CRepertoireAtlas.CAtlasChosen);
    }

    [Fact]
    public void RepertoireSituationSelect_UnsavedScenarioKept_RecordsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoire.TRepertoireSituationSave(engine, "at home");
        List<string> asked = [];
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(null, asked));
        repertoire.CRepertoireSituationCreate();
        TRepertoire.TRepertoireTitleDefer(repertoire);
        int recorded = 0;
        atelier.CAtelierNavigation.CNavigationChanged += _ => recorded++;

        repertoire.CRepertoireSituationSelect(home.LSituationId);

        Assert.Equal(0, recorded);
        Assert.Equal(["Leave"], asked);
        Assert.True(repertoire.CRepertoireScenarioShown);
        Assert.Null(repertoire.CRepertoireAtlas.CAtlasChosen);
    }

    [Fact]
    public void RepertoireOccurrenceSelect_UnsavedScenarioKept_StaysOnTheScenario()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoire.TRepertoireSituationSave(engine, "at home");
        LEntry hearth = TRepertoire.TRepertoireEntrySave(engine, "hearth", home);
        List<string> asked = [];
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(null, asked));
        repertoire.CRepertoireSituationCreate();
        TRepertoire.TRepertoireTitleDefer(repertoire);

        repertoire.CRepertoireOccurrenceSelect(hearth.LEntryId);

        Assert.Equal(["Leave"], asked);
        Assert.True(repertoire.CRepertoireScenarioShown);
        Assert.False(repertoire.CRepertoireDisplayShown);
    }

    [Fact]
    public void RepertoireLeaveConfirm_UnsavedScenarioStored_StoresTheSituation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(true, asked));

        Assert.True(repertoire.TRepertoireLeaveConfirm());
        Assert.Empty(asked);

        repertoire.CRepertoireSituationCreate();
        TRepertoire.TRepertoireTitleDefer(repertoire);

        Assert.True(repertoire.TRepertoireLeaveConfirm());
        Assert.Equal(["Leave"], asked);
        Assert.Single(repertoire.CRepertoireRowsRead());
        Assert.True(repertoire.CRepertoireVignetteHeld);
    }

    [Fact]
    public void RepertoireLeaveConfirm_UnsavedScenarioKept_StaysOnTheScenario()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(null, []));
        repertoire.CRepertoireSituationCreate();
        TRepertoire.TRepertoireTitleDefer(repertoire);

        Assert.False(repertoire.TRepertoireLeaveConfirm());
        Assert.True(repertoire.CRepertoireScenarioShown);
        Assert.True(repertoire.CRepertoireDesk.CDeskHeld);
    }

    [Fact]
    public void RepertoireSessionSave_FreshScenario_ShowsTheStoredSituationOnTheVignette()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.CRepertoireSituationCreate();
        TRepertoire.TRepertoireTitleDefer(repertoire);

        Assert.True(repertoire.CRepertoireSession.CSessionSave());

        Assert.True(repertoire.CRepertoireVignetteShown);
        Assert.True(repertoire.CRepertoireVignetteHeld);
        Assert.False(repertoire.CRepertoireScribeChecked);
        Assert.Single(repertoire.CRepertoireRowsRead());
    }

    [Fact]
    public void RepertoireScribeToggle_ClosingTheScenario_CancelsTheDesk()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoire.TRepertoireSituationSave(engine, "at home");
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.TRepertoireSituationOpen(home.LSituationId);

        repertoire.CRepertoireScribeToggle(true);

        Assert.True(repertoire.CRepertoireScenarioShown);
        Assert.True(repertoire.CRepertoireDesk.CDeskHeld);

        repertoire.CRepertoireScribeToggle(false);

        Assert.True(repertoire.CRepertoireVignetteShown);
        Assert.False(repertoire.CRepertoireDesk.CDeskHeld);
        Assert.Equal(home.LSituationId, repertoire.CRepertoireAtlas.CAtlasChosen);
    }

    [Fact]
    public void RepertoireScribeToggle_ClosingTheOccurrenceEditor_FallsBackToTheChosenSituation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoire.TRepertoireSituationSave(engine, "at home");
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.TRepertoireSituationOpen(home.LSituationId);
        repertoire.CRepertoireSituationCreate();

        repertoire.CRepertoireScribeToggle(false);

        Assert.False(repertoire.CRepertoireEditorShown);
        Assert.True(repertoire.CRepertoireVignetteShown);
        Assert.Equal(home.LSituationId, repertoire.CRepertoireAtlas.CAtlasChosen);
    }

    [Fact]
    public void RepertoireEntryResonate_NoChosenOccurrence_KeepsTheScenario()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.CRepertoireSituationCreate();

        engine.TEngineBulletinRaise(LSubject.LSubjectEntry, 0);

        Assert.True(repertoire.CRepertoireScenarioShown);
        Assert.True(repertoire.CRepertoireDesk.CDeskHeld);
    }
}
