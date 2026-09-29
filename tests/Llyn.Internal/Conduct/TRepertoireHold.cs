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
            TInterfaceConduct.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        repertoire.CRepertoireVistaRestore();
        repertoire.CRepertoireSituationCreate();
        List<CSituationDraft> drafted = [];
        repertoire.CRepertoireDraftChanged += drafted.Add;

        TRepertoire.TRepertoireTitleDefer(repertoire);
        repertoire.CRepertoireDesk.CDeskPersist();

        Assert.True(marshalled > 0);
        Assert.Equal("in court", drafted[^1].CSituationDraftTitle.CStateValueText);
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
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
        repertoire.CRepertoireOccurrenceSelect(hearth.LEntryId);
        repertoire.CRepertoireScribeToggle(true);
        Assert.True(repertoire.CRepertoireEditor.CEditorDesk.CDeskHeld);

        atelier.CAtelierClose();

        Assert.False(repertoire.CRepertoireEditor.CEditorDesk.CDeskHeld);
        Assert.Equal(1, stopped);
    }
}
