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

public sealed class TRepertoire
{
    [Fact]
    public void RepertoireSituationCreate_NoRowChosen_StartsABlankScenario()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        List<CSituationDraft?> held = [];
        repertoire.CRepertoireScenarioChanged += held.Add;

        repertoire.CRepertoireSituationCreate();

        Assert.True(repertoire.CRepertoireDesk.CDeskHeld);
        Assert.True(repertoire.CRepertoireScenarioShown);
        Assert.True(repertoire.CRepertoireScribeChecked);
        Assert.NotNull(held[^1]);
        Assert.NotNull(repertoire.CRepertoireScenarioRead());
    }

    [Fact]
    public void RepertoireSituationCreate_SituationChosen_OpensALinkedEntryInTheEditor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        repertoire.TRepertoireSituationOpen(home.LSituationId);
        List<CEntryDraft> shown = [];
        repertoire.CRepertoireEditor.CEditorDraftChanged += shown.Add;

        repertoire.CRepertoireSituationCreate();

        Assert.True(repertoire.CRepertoireEditorShown);
        Assert.False(repertoire.CRepertoireVignetteShown);
        Assert.False(repertoire.CRepertoireDesk.CDeskHeld);
        Assert.Equal(home.LSituationId, repertoire.CRepertoireAtlas.CAtlasChosen);
        Assert.Contains(
            shown[0].CEntryDraftMeanings[0].CCardDraftSituation,
            row => row.CSituationDraftId == home.LSituationId);
    }

    [Fact]
    public void RepertoireSessionSave_FreshScenario_ShowsTheStoredSituationOnTheVignette()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        repertoire.CRepertoireSituationCreate();
        TRepertoireTitleDefer(repertoire);

        Assert.True(repertoire.CRepertoireSession.CSessionSave());

        Assert.True(repertoire.CRepertoireVignetteShown);
        Assert.True(repertoire.CRepertoireVignetteHeld);
        Assert.False(repertoire.CRepertoireScribeChecked);
        Assert.Single(repertoire.CRepertoireRowsRead("?", "-"));
    }

    [Fact]
    public void RepertoireSituationSelect_UnsavedScenarioStored_StoresItAndShowsTheChosenSituation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        List<string> asked = [];
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(true, asked));
        repertoire.CRepertoireSituationCreate();
        TRepertoireTitleDefer(repertoire);
        int recorded = 0;
        atelier.CAtelierNavigation.CNavigationChanged += _ => recorded++;

        repertoire.CRepertoireSituationSelect(home.LSituationId);

        Assert.Equal(1, recorded);
        Assert.Equal(["Leave"], asked);
        Assert.Equal(2, repertoire.CRepertoireRowsRead("?", "-").Count);
        Assert.Equal(home.LSituationId, repertoire.CRepertoireAtlas.CAtlasChosen);
    }

    [Fact]
    public void RepertoireOccurrenceSelect_UnsavedScenarioKept_StaysOnTheScenario()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        LEntry hearth = TRepertoireEntrySave(engine, "hearth", home);
        List<string> asked = [];
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(null, asked));
        repertoire.CRepertoireSituationCreate();
        TRepertoireTitleDefer(repertoire);

        repertoire.CRepertoireOccurrenceSelect(hearth.LEntryId);

        Assert.Equal(["Leave"], asked);
        Assert.True(repertoire.CRepertoireScenarioShown);
        Assert.False(repertoire.CRepertoireDisplayShown);
    }

    [Fact]
    public void RepertoireSituationOpen_StoredSituation_ShowsItOnTheVignette()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        List<CSituationDraft> shown = [];
        repertoire.CRepertoireSituationChanged += shown.Add;

        repertoire.TRepertoireSituationOpen(home.LSituationId);

        Assert.True(repertoire.CRepertoireVignetteShown);
        Assert.True(repertoire.CRepertoireVignetteHeld);
        Assert.True(repertoire.CRepertoirePressAllowed);
        Assert.False(repertoire.CRepertoirePortraitAllowed);
        Assert.Equal("at home", Assert.Single(shown).CSituationDraftTitle.CStateValueText);
    }

    [Fact]
    public void RepertoireSituationOpen_HiddenByTheQuery_DropsTheQueryAndShowsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        TRepertoireSituationSave(engine, "in court");
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        repertoire.CRepertoireAtlas.CAtlasQuerySet("court");
        repertoire.CRepertoireAtlas.CAtlasPanel.CPanelRowsChanged += () => repertoire.CRepertoireRowsRead("?", "-");
        int cleared = 0;
        repertoire.CRepertoireQueryCleared += () => cleared++;

        repertoire.TRepertoireSituationOpen(home.LSituationId);

        Assert.Equal(1, cleared);
        Assert.Equal(2, repertoire.CRepertoireRowsRead("?", "-").Count);
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
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, asked));
        int recorded = 0;
        atelier.CAtelierNavigation.CNavigationChanged += _ => recorded++;

        repertoire.CRepertoireSituationSelect(home.LSituationId);
        repertoire.CRepertoireSituationSelect(null);

        Assert.Equal(1, recorded);
        Assert.Empty(asked);
        Assert.Equal(home.LSituationId, repertoire.CRepertoireAtlas.CAtlasChosen);
    }

    [Fact]
    public void RepertoireSituationSelect_UnsavedScenarioKept_RecordsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        List<string> asked = [];
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(null, asked));
        repertoire.CRepertoireSituationCreate();
        TRepertoireTitleDefer(repertoire);
        int recorded = 0;
        atelier.CAtelierNavigation.CNavigationChanged += _ => recorded++;

        repertoire.CRepertoireSituationSelect(home.LSituationId);

        Assert.Equal(0, recorded);
        Assert.Equal(["Leave"], asked);
        Assert.True(repertoire.CRepertoireScenarioShown);
        Assert.Null(repertoire.CRepertoireAtlas.CAtlasChosen);
    }

    [Fact]
    public void RepertoireLeaveConfirm_UnsavedScenarioStored_StoresTheSituation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(true, asked));

        Assert.True(repertoire.TRepertoireLeaveConfirm());
        Assert.Empty(asked);

        repertoire.CRepertoireSituationCreate();
        TRepertoireTitleDefer(repertoire);

        Assert.True(repertoire.TRepertoireLeaveConfirm());
        Assert.Equal(["Leave"], asked);
        Assert.Single(repertoire.CRepertoireRowsRead("?", "-"));
        Assert.True(repertoire.CRepertoireVignetteHeld);
    }

    [Fact]
    public void RepertoireLeaveConfirm_UnsavedScenarioKept_StaysOnTheScenario()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(null, []));
        repertoire.CRepertoireSituationCreate();
        TRepertoireTitleDefer(repertoire);

        Assert.False(repertoire.TRepertoireLeaveConfirm());
        Assert.True(repertoire.CRepertoireScenarioShown);
        Assert.True(repertoire.CRepertoireDesk.CDeskHeld);
    }

    [Fact]
    public void RepertoireScribeToggle_ClosingTheScenario_CancelsTheDesk()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
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
        LSituation home = TRepertoireSituationSave(engine, "at home");
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        repertoire.TRepertoireSituationOpen(home.LSituationId);
        repertoire.CRepertoireSituationCreate();

        repertoire.CRepertoireScribeToggle(false);

        Assert.False(repertoire.CRepertoireEditorShown);
        Assert.True(repertoire.CRepertoireVignetteShown);
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
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        repertoire.TRepertoireSituationOpen(home.LSituationId);

        repertoire.CRepertoireOccurrenceSelect(hearth.LEntryId);

        Assert.False(repertoire.CRepertoireVignetteShown);
        Assert.True(repertoire.CRepertoireDisplayShown);
        Assert.True(repertoire.CRepertoirePortraitAllowed);
        Assert.False(repertoire.CRepertoireBinEnabled);
        Assert.Equal(home.LSituationId, repertoire.CRepertoireAtlas.CAtlasChosen);
    }

    [Fact]
    public void RepertoireSituationClose_ChosenSituation_ClearsBothLists()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        repertoire.TRepertoireSituationOpen(home.LSituationId);

        repertoire.CRepertoireSituationClose();

        Assert.True(repertoire.CRepertoireVignetteBlank);
        Assert.False(repertoire.CRepertoireDisplayShown);
        Assert.Null(repertoire.CRepertoireAtlas.CAtlasChosen);
    }

    [Fact]
    public void RepertoireEntryResonate_NoChosenOccurrence_KeepsTheScenario()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        repertoire.CRepertoireSituationCreate();

        repertoire.CRepertoireEntryResonate();

        Assert.True(repertoire.CRepertoireScenarioShown);
        Assert.True(repertoire.CRepertoireDesk.CDeskHeld);
    }

    [Fact]
    public void RepertoireEntryResonate_OccurrenceOnDisplay_KeepsTheOccurrenceSide()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        LEntry hearth = TRepertoireEntrySave(engine, "hearth", home);
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        repertoire.TRepertoireSituationOpen(home.LSituationId);
        repertoire.CRepertoireOccurrenceSelect(hearth.LEntryId);

        repertoire.CRepertoireEntryResonate();

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
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        repertoire.TRepertoireSituationOpen(home.LSituationId);

        Assert.Equal(2, repertoire.CRepertoireRowsRead("?", "-").Count);
        Assert.True(repertoire.CRepertoireVignetteHeld);

        repertoire.CRepertoireAtlas.CAtlasQuerySet("court");

        Assert.Single(repertoire.CRepertoireRowsRead("?", "-"));
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
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(true, asked));
        repertoire.TRepertoireSituationOpen(home.LSituationId);

        repertoire.CRepertoireSituationDelete();

        Assert.Equal(["Situation.DeleteConfirm"], asked);
        Assert.Empty(repertoire.CRepertoireRowsRead("?", "-"));
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
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(true, asked));
        repertoire.TRepertoireSituationOpen(home.LSituationId);
        repertoire.CRepertoireOccurrenceSelect(hearth.LEntryId);

        repertoire.CRepertoireSituationDelete();

        Assert.Empty(asked);
        Assert.Single(repertoire.CRepertoireRowsRead("?", "-"));
        Assert.True(repertoire.CRepertoireDisplayShown);
    }

    [Fact]
    public async Task RepertoirePortraitExport_OccurrenceOnDisplay_WritesTheEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoireSituationSave(engine, "at home");
        LEntry hearth = TRepertoireEntrySave(engine, "hearth", home);
        string path = Path.Combine(workspace.TWorkspaceFolder, "hearth.md");
        CRepertoire repertoire = TRepertoirePrepare(
            atelier, TInterfaceConduct.TEnvoyFileCreate(path, CPortraitMedium.CPortraitMediumMarkdown, []));
        repertoire.TRepertoireSituationOpen(home.LSituationId);

        await repertoire.CRepertoirePortraitExport();

        Assert.False(File.Exists(path));

        repertoire.CRepertoireOccurrenceSelect(hearth.LEntryId);
        await repertoire.CRepertoirePortraitExport();

        Assert.Equal("hearth", repertoire.CRepertoireOccurrence.TOccurrenceFileRead());
        Assert.Contains("hearth", File.ReadAllText(path), StringComparison.Ordinal);
    }

    [Fact]
    public void RepertoirePortraitPrint_NothingChosen_PrintsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));

        Task printed = repertoire.CRepertoirePortraitPrint();

        Assert.Same(Task.CompletedTask, printed);
        Assert.False(repertoire.CRepertoirePressAllowed);
    }

    internal static CRepertoire TRepertoirePrepare(CAtelier atelier, CEnvoy envoy)
    {
        CRepertoire repertoire = CRepertoire.CRepertoireCreate(atelier, static () => true, envoy);
        repertoire.CRepertoireVistaRestore();
        return repertoire;
    }

    private static void TRepertoireTitleDefer(CRepertoire repertoire)
    {
        repertoire.CRepertoireDesk.TDeskDefer(TInterface.TSituationTitleCreate(
            repertoire.CRepertoireDesk.CDeskId,
            repertoire.CRepertoireScenarioRead()!.CSituationDraftId,
            TInterface.TStateValueCreate("in court")));
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
