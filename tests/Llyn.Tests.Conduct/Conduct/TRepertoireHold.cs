using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRepertoireHold
{
    [Fact]
    public void RepertoireCreate_ScenarioEdited_HandsTheHeldSituationThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineDelaySet(0);
        int marshalled = 0;
        CRepertoire repertoire = CRepertoire.CRepertoireCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        repertoire.CRepertoireDiptych.CDiptychEntryCreate();
        List<CScenario> drafted = [];
        repertoire.CRepertoirePlaywright.CPlaywrightDraftChanged += drafted.Add;

        TRepertoire.TRepertoireTitleDefer(repertoire);
        repertoire.CRepertoirePlaywright.LPlaywrightDesk.CDeskDraft.CDeskDraftPersist();

        Assert.True(marshalled > 0);
        Assert.Equal("in court", drafted[^1].CScenarioDraft.CSituationDraftTitle.CStateValueText);
    }

    [Fact]
    public void RepertoireTitleSet_TitleTyped_WritesItAndAnswersTheTypedLine()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineDelaySet(0);
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.CRepertoireDiptych.CDiptychEntryCreate();

        CScenarioLine typed = repertoire.CRepertoirePlaywright.CPlaywrightTitleSet("in court");
        CScenarioLine cleared = repertoire.CRepertoirePlaywright.CPlaywrightDescriptionSet(string.Empty);

        CSituationDraft held = repertoire.TRepertoireScenarioRead()!;
        Assert.Equal("in court", held.CSituationDraftTitle.CStateValueText);
        Assert.Equal(
            ("in court", "Situation.Untitled", false),
            (typed.CScenarioLineText, typed.CScenarioLineHint, typed.CScenarioLineVacant));
        Assert.Equal(("Situation.DescriptionHint", true), (cleared.CScenarioLineHint, cleared.CScenarioLineVacant));
    }

    [Fact]
    public void RepertoireKindSet_KindTypedThenCleared_WordsTheMeasureOnlyWhileEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineDelaySet(0);
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.CRepertoireDiptych.CDiptychEntryCreate();

        CScenarioLine typed = repertoire.CRepertoirePlaywright.CPlaywrightKindSet("formal");
        string kind = repertoire.TRepertoireScenarioRead()!.CSituationDraftKind.CStateValueText;
        CScenarioLine cleared = repertoire.CRepertoirePlaywright.CPlaywrightKindSet(string.Empty);

        Assert.Equal("formal", kind);
        Assert.Equal(("Situation.Kind", null), (typed.CScenarioLineHint, typed.CScenarioLineWording));
        Assert.Equal("Situation.Kind", cleared.CScenarioLineWording);
    }

    [Fact]
    public void RepertoireScenarioChanged_UnknownKindThenCancel_RaisesTheUnknownHintThenABlankScenario()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "at home", null, LStateValue.LStateValueUnknown));
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        List<CScenario> held = [];
        repertoire.CRepertoirePlaywright.CPlaywrightScenarioChanged += held.Add;
        repertoire.TRepertoireSituationOpen(home.LSituationId);
        repertoire.CRepertoireDiptych.CDiptychScribeToggle(true);
        CScenario stored = held[^1];

        repertoire.CRepertoireSession.CSessionCancel();

        Assert.Equal(
            ("at home", "Situation.Untitled"),
            (stored.CScenarioTitle.CScenarioLineText, stored.CScenarioTitle.CScenarioLineHint));
        Assert.Equal(
            ("Display.Unknown", "Display.Unknown"),
            (stored.CScenarioKind.CScenarioLineHint, stored.CScenarioKind.CScenarioLineWording));
        CScenario blank = held[^1];
        Assert.Equal(0, blank.CScenarioDraft.CSituationDraftId);
        Assert.True(blank.CScenarioTitle.CScenarioLineVacant);
        Assert.Equal(
            ("Situation.Untitled", "Situation.Kind", "Situation.DescriptionHint"),
            (
                blank.CScenarioTitle.CScenarioLineHint,
                blank.CScenarioKind.CScenarioLineHint,
                blank.CScenarioDescription.CScenarioLineHint));
        Assert.Empty(blank.CScenarioDraft.CSituationDraftImage);
    }

    [Fact]
    public void AtelierClose_OccurrenceEditorOpen_CancelsTheEditorDeskAndStopsPlayback()
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
        LSituation home = TRepertoire.TRepertoireSituationSave(engine, "at home");
        LEntry hearth = TRepertoire.TRepertoireEntrySave(engine, "hearth", home);
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.CRepertoireDiptych.CDiptychChildSelect(hearth.LEntryId);
        repertoire.CRepertoireDiptych.CDiptychScribeToggle(true);
        Assert.True(repertoire.CRepertoireEditor.CEditorDesk.CDeskHeld);

        atelier.CAtelierClose();

        Assert.False(repertoire.CRepertoireEditor.CEditorDesk.CDeskHeld);
        Assert.Equal(1, stopped);
    }
}
