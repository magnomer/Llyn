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
        repertoire.CRepertoirePlaywright.CPlaywrightScenarioChanged += held.Add;

        repertoire.CRepertoireDiptych.CDiptychEntryCreate();

        Assert.True(repertoire.CRepertoirePlaywright.LPlaywrightDesk.CDeskHeld);
        Assert.True(repertoire.CRepertoireDiptych.CDiptychParentEditing);
        Assert.True(repertoire.CRepertoireDiptych.CDiptychScribeChecked);
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
        repertoire.CRepertoireDiptych.CDiptychEntryCreate();
        TRepertoire.TRepertoireTitleDefer(repertoire);
        int recorded = 0;
        atelier.CAtelierNavigation.CNavigationChanged += _ => recorded++;

        repertoire.CRepertoireDiptych.CDiptychParentSelect(home.LSituationId);

        Assert.Equal(1, recorded);
        Assert.Equal(["Leave"], asked);
        Assert.Equal(2, repertoire.CRepertoireRowsRead().Count);
        Assert.Equal(home.LSituationId, repertoire.CRepertoireAtlas.CAtlasPanel.CPanelAperture.CApertureChosen);
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
        repertoire.CRepertoireDiptych.CDiptychEntryCreate();
        TRepertoire.TRepertoireTitleDefer(repertoire);
        int recorded = 0;
        atelier.CAtelierNavigation.CNavigationChanged += _ => recorded++;

        repertoire.CRepertoireDiptych.CDiptychParentSelect(home.LSituationId);

        Assert.Equal(0, recorded);
        Assert.Equal(["Leave"], asked);
        Assert.True(repertoire.CRepertoireDiptych.CDiptychParentEditing);
        Assert.Null(repertoire.CRepertoireAtlas.CAtlasPanel.CPanelAperture.CApertureChosen);
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
        repertoire.CRepertoireDiptych.CDiptychEntryCreate();
        TRepertoire.TRepertoireTitleDefer(repertoire);

        repertoire.CRepertoireDiptych.CDiptychChildSelect(hearth.LEntryId);

        Assert.Equal(["Leave"], asked);
        Assert.True(repertoire.CRepertoireDiptych.CDiptychParentEditing);
        Assert.False(repertoire.CRepertoireDiptych.CDiptychChildShown);
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

        repertoire.CRepertoireDiptych.CDiptychEntryCreate();
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
        repertoire.CRepertoireDiptych.CDiptychEntryCreate();
        TRepertoire.TRepertoireTitleDefer(repertoire);

        Assert.False(repertoire.TRepertoireLeaveConfirm());
        Assert.True(repertoire.CRepertoireDiptych.CDiptychParentEditing);
        Assert.True(repertoire.CRepertoirePlaywright.LPlaywrightDesk.CDeskHeld);
    }

    [Fact]
    public void RepertoireSessionSave_FreshScenario_ShowsTheStoredSituationOnTheVignette()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.CRepertoireDiptych.CDiptychEntryCreate();
        TRepertoire.TRepertoireTitleDefer(repertoire);

        Assert.True(repertoire.CRepertoireSession.CSessionSave());

        Assert.True(repertoire.CRepertoireDiptych.CDiptychParentShown);
        Assert.True(repertoire.CRepertoireVignetteHeld);
        Assert.False(repertoire.CRepertoireDiptych.CDiptychScribeChecked);
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

        repertoire.CRepertoireDiptych.CDiptychScribeToggle(true);

        Assert.True(repertoire.CRepertoireDiptych.CDiptychParentEditing);
        Assert.True(repertoire.CRepertoirePlaywright.LPlaywrightDesk.CDeskHeld);

        repertoire.CRepertoireDiptych.CDiptychScribeToggle(false);

        Assert.True(repertoire.CRepertoireDiptych.CDiptychParentShown);
        Assert.False(repertoire.CRepertoirePlaywright.LPlaywrightDesk.CDeskHeld);
        Assert.Equal(home.LSituationId, repertoire.CRepertoireAtlas.CAtlasPanel.CPanelAperture.CApertureChosen);
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
        repertoire.CRepertoireDiptych.CDiptychEntryCreate();

        repertoire.CRepertoireDiptych.CDiptychScribeToggle(false);

        Assert.False(repertoire.CRepertoireDiptych.CDiptychChildEditing);
        Assert.True(repertoire.CRepertoireDiptych.CDiptychParentShown);
        Assert.Equal(home.LSituationId, repertoire.CRepertoireAtlas.CAtlasPanel.CPanelAperture.CApertureChosen);
    }

    [Fact]
    public void RepertoireEntryResonate_NoChosenOccurrence_KeepsTheScenario()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.CRepertoireDiptych.CDiptychEntryCreate();

        engine.TEngineBulletinRaise(LSubject.LSubjectEntry, 0);

        Assert.True(repertoire.CRepertoireDiptych.CDiptychParentEditing);
        Assert.True(repertoire.CRepertoirePlaywright.LPlaywrightDesk.CDeskHeld);
    }
}
